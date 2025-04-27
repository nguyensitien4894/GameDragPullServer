using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Helpers.Chargings.MobileSMS
{
    public class MobileSMSApi
    {
        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }

        private static MobileRequestResponse PostJson(string uri, string postData)
        {
            var card = new MobileRequestResponse();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                                        //request.Accept = "JSON";
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                card = JsonConvert.DeserializeObject<MobileRequestResponse>(result);

                return card;
            }
            catch (Exception ex)
            {
                card.errorid = -99;
                card.errordes = "Exception Code";
                NLogManager.LogMessage("STMT -99");
                NLogManager.PublishException(ex);
            }

            return card;
        }
        public static MobileRequestResponse Send(String transId, string telco, String pin , String serial, int amount)
        {


            string url = ConfigurationManager.AppSettings["MobileSMS_URL"].ToString();
            string username = ConfigurationManager.AppSettings["MobileSMS_USERNAME"].ToString();
            string password = ConfigurationManager.AppSettings["MobileSMS_PASSWORD"].ToString();
            string private_key= ConfigurationManager.AppSettings["MobileSMS_PRIVATEKEY"].ToString();

           
            string publickey = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", username, password, serial, pin, telco, amount, transId, private_key);
            var signature = MD5(publickey);
            string requestData = JsonConvert.SerializeObject(new MobileRequest()
            {
                username = username,
                password = password,
                transId = transId,
                telco = telco,
                serial = serial,
                pin = pin,
                amount = amount.ToString(),
                signature= signature,

            });
            
           
            
            var serviceResponse = PostJson(url,(requestData));
          
            return serviceResponse;
        }


        public static MobileRequestResponse SendGate3(String transId, string telco, String pin, String serial, int amount)
        {


            string url = ConfigurationManager.AppSettings["Gate3_MobileSMS_URL"].ToString();
            string username = ConfigurationManager.AppSettings["Gate3_MobileSMS_USERNAME"].ToString();
            string password = ConfigurationManager.AppSettings["Gate3_MobileSMS_PASSWORD"].ToString();
            string private_key = ConfigurationManager.AppSettings["Gate3_MobileSMS_PRIVATEKEY"].ToString();


            string publickey = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", username, password, serial, pin, telco, amount, transId, private_key);
            var signature = MD5(publickey);
            string requestData = JsonConvert.SerializeObject(new MobileRequest()
            {
                username = username,
                password = password,
                transId = transId,
                telco = telco,
                serial = serial,
                pin = pin,
                amount = amount.ToString(),
                signature = signature,

            });



            var serviceResponse = PostJson(url, (requestData));

            return serviceResponse;
        }
    }
}