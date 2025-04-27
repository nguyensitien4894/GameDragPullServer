using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MsTraditionGame.Utilities.Log;

namespace MsTraditionGame.Utilities.Security
{
    public class Security
    {
        public Security()
        {
        }
        public static string CreateSignRSA(string data, string privateKey)
        {
            CspParameters _cpsParameter;
            RSACryptoServiceProvider rsaCryptoIPT;
            _cpsParameter = new CspParameters();
            _cpsParameter.Flags = CspProviderFlags.UseMachineKeyStore;
            rsaCryptoIPT = new RSACryptoServiceProvider(1024, _cpsParameter);

            rsaCryptoIPT.FromXmlString(privateKey);
            return
                Convert.ToBase64String(rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data),
                                                             new SHA1CryptoServiceProvider()));
        }
        public static bool CheckSignRSA(string data, string sign, string publicKey)
        {
            try
            {
                var rsacp = new RSACryptoServiceProvider();
                rsacp.FromXmlString(publicKey);
                return rsacp.VerifyData(new ASCIIEncoding().GetBytes(data), "SHA1", Convert.FromBase64String(sign));
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }

        public static string Decrypt(string key, string data)
        {
            byte[] keydata = Encoding.ASCII.GetBytes(key);

            string md5String = BitConverter.ToString(new
                                                         MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-",
                                                                                                                  "").
                Replace(" ", "+").ToLower();

            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

            TripleDES tripdes = TripleDES.Create();

            tripdes.Mode = CipherMode.ECB;

            tripdes.Key = tripleDesKey;

            byte[] cryptByte = Convert.FromBase64String(data);

            var ms = new MemoryStream(cryptByte, 0, cryptByte.Length);

            ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();

            var decStream = new CryptoStream(ms, cryptoTransform,
                                             CryptoStreamMode.Read);

            var read = new StreamReader(decStream);

            return (read.ReadToEnd());
        }
        public static string GetVerifyToken(ref string verifyToken)
        {
            var key = "123456789ABCDEFGHIJKLMNPQRSTUVXYZ";
            var keyLenght = key.Length;
            var rnd = new Random();
            var s = "";
            for (var i = 0; i < 6; i++)
            {
                s = string.Format("{0}{1}", s, key[rnd.Next(keyLenght)]);
            }

            long time = DateTime.Now.Ticks;
            verifyToken = string.Format("{0}.{1}", time, Security.SHA256Encrypt(string.Format("{0}{1}", s, time.ToString())));

            return System.Web.HttpUtility.UrlEncode(Security.TripleDESEncrypt(Security.SHA256Encrypt(Environment.MachineName), s.ToLower()));
        }

        public static string GetTokenPlainText(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            return Security.TripleDESDecrypt(Security.SHA256Encrypt(Environment.MachineName), System.Web.HttpUtility.UrlDecode(token).Replace(" ", "+"));
        }

        public static string SHA256Encrypt(string plainText)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string RandomPassword()
        {
            string text1 = string.Empty;
            Random random1 = new Random(DateTime.Now.Millisecond);
            for (int num1 = 1; num1 < 10; num1++)
            {
                text1 = string.Format("{0}{1}", text1, random1.Next(0, 9));
            }
            return text1;
        }

        public static string RandomString(int length)
        {
            string text1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int num1 = text1.Length;
            Random random1 = new Random();
            string text2 = string.Empty;
            for (int num2 = 0; num2 < length; num2++)
            {
                text2 = string.Format("{0}{1}", text2, text1[random1.Next(num1)]);
            }
            return text2;
        }

        public static string TripleDESEncrypt(string key, string data)
        {
            data = data.Trim();

            byte[] keydata = Encoding.ASCII.GetBytes(key);

            string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();

            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();

            tripdes.Mode = CipherMode.ECB;

            tripdes.Key = tripleDesKey;

            tripdes.GenerateIV();

            MemoryStream ms = new MemoryStream();

            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(),
                CryptoStreamMode.Write);

            encStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));

            encStream.FlushFinalBlock();

            byte[] cryptoByte = ms.ToArray();

            ms.Close();

            encStream.Close();

            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0)).Trim();
        }
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        public static string TripleDESDecrypt(string key, string data)
        {
            try
            {
                byte[] keydata = Encoding.ASCII.GetBytes(key);

                string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();

                byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

                TripleDES tripdes = TripleDESCryptoServiceProvider.Create();

                tripdes.Mode = CipherMode.ECB;

                tripdes.Key = tripleDesKey;

                byte[] cryptByte = Convert.FromBase64String(data);

                MemoryStream ms = new MemoryStream(cryptByte, 0, cryptByte.Length);

                ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();

                CryptoStream decStream = new CryptoStream(ms, cryptoTransform,
                    CryptoStreamMode.Read);

                StreamReader read = new StreamReader(decStream);

                return (read.ReadToEnd());
            }
            catch
            {
                // Wrong key
                // throw new Exception("Sai key mã hóa\t key: " + key + "\t Data: " + data);
                return string.Empty;
            }
        }
    }
}
