using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Helpers.Chargings.Cards
{
    public class CardApi
    {
        /// <summary>
        /// send cho thẻ viettel
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="account"></param>
        /// <param name="cardNumber"></param>
        /// <param name="serialNumber"></param>
        /// <param name="cardValue"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static CardResponse SendRequest(string requestId, string account, string cardNumber, string serialNumber, int cardValue, string provider)
        {
            CardResponse card = new CardResponse();


            String result = string.Empty;
            String url = ConfigurationManager.AppSettings["CARD_CHARGING_URL"].ToString();
            string nccCode = ConfigurationManager.AppSettings["CARD_NCCCODE"].ToString();
            string gameCode = ConfigurationManager.AppSettings["CARD_GAMECODE"].ToString();
            byte type = 1;
            string secretKey = ConfigurationManager.AppSettings["CARD_SECREATEKEY"].ToString();
            string accessKey = ConfigurationManager.AppSettings["CARD_ACCESSKEY"].ToString();
            string signature = CardHelper.GenerateSignature(requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, secretKey);
            string plainText = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
                requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, signature);

            string basedata = CardHelper.Base64Encode(plainText);
            String urlParameter = String.Format("data={0}", basedata);
            CardResponse resObj = new CardResponse();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
               
                card = JsonConvert.DeserializeObject<CardResponse>(result);
                card.Signature = signature;
                return card;

            }
            catch (Exception e)
            {
                card.ErrorCode = "-99";
                NLogManager.PublishException(e);
            }
           
            return card;
        }




        public static CardResponse SendRequestFunny(string requestId, string account, string cardNumber, string serialNumber, int cardValue, string provider)
        {
            CardResponse card = new CardResponse();


            String result = string.Empty;
            String url = ConfigurationManager.AppSettings["CARD_CHARGING_URL_P4"].ToString();
            string nccCode = ConfigurationManager.AppSettings["CARD_NCCCODE_P4"].ToString();
            string gameCode = ConfigurationManager.AppSettings["CARD_GAMECODE_P4"].ToString();
            byte type = 1;
            string secretKey = ConfigurationManager.AppSettings["CARD_SECREATEKEY_P4"].ToString();
            string accessKey = ConfigurationManager.AppSettings["CARD_ACCESSKEY_P4"].ToString();
            string signature = CardHelper.GenerateSignature(requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, secretKey);
            string plainText = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
                requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, signature);

            string basedata = CardHelper.Base64Encode(plainText);
            String urlParameter = String.Format("data={0}", basedata);
            CardResponse resObj = new CardResponse();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<CardResponse>(result);
                card.Signature = signature;
                return card;

            }
            catch (Exception e)
            {
                card.ErrorCode = "-99";
                NLogManager.PublishException(e);
            }

            return card;
        }


        /// <summary>
        /// send cho thẻ viettel
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="account"></param>
        /// <param name="cardNumber"></param>
        /// <param name="serialNumber"></param>
        /// <param name="cardValue"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static CardResponse SendGate2FunnyRequest(string requestId, string account, string cardNumber, string serialNumber, int cardValue, string provider)
        {
            CardResponse card = new CardResponse();


            String result = string.Empty;
            String url = ConfigurationManager.AppSettings["Gate2_EasyPay_CARD_CHARGING_URL"].ToString();
            string nccCode = ConfigurationManager.AppSettings["Gate2_EasyPay_CARD_NCCCODE"].ToString();
            string gameCode = ConfigurationManager.AppSettings["Gate2_EasyPay_CARD_GAMECODE"].ToString();
            byte type = 1;
            string secretKey = ConfigurationManager.AppSettings["Gate2_EasyPay_CARD_SECREATEKEY"].ToString();
            string accessKey = ConfigurationManager.AppSettings["Gate2_EasyPay_CARD_ACCESSKEY"].ToString();
            string signature = CardHelper.GenerateSignature(requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, secretKey);
            string plainText = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
                requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, signature);

            string basedata = CardHelper.Base64Encode(plainText);
            String urlParameter = String.Format("data={0}", basedata);
            CardResponse resObj = new CardResponse();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<CardResponse>(result);
                card.Signature = signature;
                return card;

            }
            catch (Exception e)
            {
                card.ErrorCode = "-99";
                NLogManager.PublishException(e);
            }

            return card;
        }


        public static CardResponse SendGate3FunnyRequest(string requestId, string account, string cardNumber, string serialNumber, int cardValue, string provider)
        {
            CardResponse card = new CardResponse();


            String result = string.Empty;
            String url = ConfigurationManager.AppSettings["Gate3_EasyPay_CARD_CHARGING_URL"].ToString();
            string nccCode = ConfigurationManager.AppSettings["Gate3_EasyPay_CARD_NCCCODE"].ToString();
            string gameCode = ConfigurationManager.AppSettings["Gate3_EasyPay_CARD_GAMECODE"].ToString();
            byte type = 1;
            string secretKey = ConfigurationManager.AppSettings["Gate3_EasyPay_CARD_SECREATEKEY"].ToString();
            string accessKey = ConfigurationManager.AppSettings["Gate3_EasyPay_CARD_ACCESSKEY"].ToString();
            string signature = CardHelper.GenerateSignature(requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, secretKey);
            string plainText = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
                requestId, nccCode, gameCode, account, cardNumber, serialNumber, cardValue, provider, type, accessKey, signature);

            string basedata = CardHelper.Base64Encode(plainText);
            String urlParameter = String.Format("data={0}", basedata);
            CardResponse resObj = new CardResponse();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<CardResponse>(result);
                card.Signature = signature;
                return card;

            }
            catch (Exception e)
            {
                card.ErrorCode = "-99";
                NLogManager.PublishException(e);
            }

            return card;
        }

        /// <summary>
        /// aip dành riêng cho vina phone
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cardNumber"></param>
        /// <param name="serialNumber"></param>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public static VinaResponse SendVinaRequest(string requestId, string cardNumber, string serialNumber, int cardValue,string provider)
        {
            VinaResponse card = new VinaResponse();


            String result = string.Empty;
            String url = ConfigurationManager.AppSettings["VINA_CARD_CHARGING_URL"].ToString();
         
            string secretKey = ConfigurationManager.AppSettings["VINA_SECRETEKEY"].ToString();
            string IDCP = ConfigurationManager.AppSettings["VINA_IDCP"].ToString();
            string VINA_TELCO = provider;
            
            string signature = CardVinaHelper.GenerateSignature(requestId, cardNumber,serialNumber, VINA_TELCO, cardValue, secretKey);
          

            String urlParameter = String.Format("idcp={0}&requestid={1}&serial={2}&pin={3}&Telco={4}&amount={5}&signature={6}", IDCP,requestId,serialNumber,cardNumber,VINA_TELCO,cardValue,signature);
           
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}?{1}",url,urlParameter));
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                //var data = Encoding.ASCII.GetBytes(urlParameter);
                //request.ContentLength = data.Length;
                //Stream requestStream = request.GetRequestStream();
                //// send url param
                //requestStream.Write(data, 0, data.Length);
                //requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<VinaResponse>(result);
                card.Signature = signature;
                return card;

            }
            catch (Exception e)
            {
                card.status = -99;
            }

            return card;
        }
    }
}