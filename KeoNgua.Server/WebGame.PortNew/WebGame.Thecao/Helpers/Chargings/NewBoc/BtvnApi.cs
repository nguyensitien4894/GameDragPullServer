using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using TraditionGame.Utilities;

namespace MsWebGame.Thecao.Helpers.Chargings.NewBoc
{
    public class BtvnAPI
    {
        public static BtvnResponse SendRequest(string requestId, string cardNumber, string serialNumber, int cardValue, string cardtype)
        {
            String result = string.Empty;
            BtvnResponse card = new BtvnResponse();
            string username = "GHL";
            string Secretkey = "6471b628c0d5e04d98d60e1e267dd9b6";
            string signature = md5(requestId + serialNumber + cardNumber + cardtype + cardValue + Secretkey);
            string urlParameter = string.Format("https://btvn.online/api/telco/register?partner=GHL&billcode={0}&serial={1}&pin={2}&telco={3}&amount={4}&signature={5}&callback={6}", requestId, serialNumber, cardNumber, cardtype, cardValue, signature);

            NLogManager.LogMessage("Url Param : " + urlParameter);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlParameter);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "GET";
                //request.ContentType = "application/json";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                card = JsonConvert.DeserializeObject<BtvnResponse>(result);

            }
            catch (Exception e)
            {
                card.message = "-99";
            }

            return card;
        }
        public static string md5(string source_str)
        {
            var encrypter = new System.Security.Cryptography.MD5CryptoServiceProvider();
            Byte[] original_bytes = System.Text.ASCIIEncoding.Default.GetBytes(source_str);
            Byte[] encoded_bytes = encrypter.ComputeHash(original_bytes);
            return BitConverter.ToString(encoded_bytes).Replace("-", "").ToLower();
        }

    }

    public class BtvnResponse
    {
        public int status { get; set; }
        public string message { get; set; }
    }


   

}