using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using TraditionGame.Utilities;

namespace TraditionGame.Utilities.Momos.API
{
    public class BaseApiMomoRequest
    {
     
        protected static string BaseUrl = "https://game-api.gaique.club/wallet/get/wallet_info";
        public static String GetHashHMACSHA256(String text, String key)
        {
            Byte[] textBytes = Encoding.UTF8.GetBytes(text);
            Byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            return Convert.ToBase64String(hashBytes);
        }
        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }

        protected static T PostJson<T>(string uri,  string postData,string sign,out int statusCode,out string HttpMsg) where T : new()
        {
            var objReturn = new T();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                if (!String.IsNullOrEmpty(sign))
                {
                    request.Headers.Add("sign", sign);
                }
                request.Method = "POST";//GET

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                statusCode = 200;
                HttpMsg = response.StatusDescription;
                response.Close();
                objReturn = JsonConvert.DeserializeObject<T>(result);
                
                return objReturn;
            }
            catch (Exception ex)
            {
                statusCode = -99;
                HttpMsg = ex.Message;
                NLogManager.PublishException(ex);
            }
            
            return objReturn;
        }

       
    }
}