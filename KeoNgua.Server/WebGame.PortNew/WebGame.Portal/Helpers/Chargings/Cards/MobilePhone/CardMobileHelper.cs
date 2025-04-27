using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MsWebGame.Portal.Helpers.Chargings.Cards.MobilePhone
{
    public class CardMobileHelper
    {
        public static string md5(string source_str)
        {
            MD5 encrypter = new MD5CryptoServiceProvider();
            Byte[] original_bytes = ASCIIEncoding.Default.GetBytes(source_str);
            Byte[] encoded_bytes = encrypter.ComputeHash(original_bytes);
            return BitConverter.ToString(encoded_bytes).Replace("-", "").ToLower();
        }
        /// <summary>
        /// tạo signature cho vi naphone
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cardNumber"></param>
        /// <param name="serialNumber"></param>
        /// <param name="Telco"></param>
        /// <param name="cardValue"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static String GenerateSignature(string transRef, string cardCode, string cardSerial)
        {
            string plainText = String.Format("{0}{1}{2}", transRef, cardCode, cardSerial);
            return md5(plainText);
        }
    }
}