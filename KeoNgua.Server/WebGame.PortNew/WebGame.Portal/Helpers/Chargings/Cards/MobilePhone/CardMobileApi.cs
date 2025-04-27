using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace MsWebGame.Portal.Helpers.Chargings.Cards.MobilePhone
{
    public class CardMobileApi
    {
        public static MobileResponse SendMobileRequest(string transRef, string cardCode, string cardSerial, int price, string accountId, string userGame)
        {
            MobileResponse card = new MobileResponse();


            String result = string.Empty;

            String url = ConfigurationManager.AppSettings["MOBILE_CHARGING_URL"].ToString();
            string username = ConfigurationManager.AppSettings["MOBILE_USERNAME"].ToString();
            string password = ConfigurationManager.AppSettings["MOBILE_PASSWORD"].ToString();
            string issuer = "VMS";//fix riêng cho mobile phone
            string signature = CardMobileHelper.GenerateSignature(transRef, cardCode, cardSerial);
            var obj = new
            {
                username = username,
                password = password,
                cardCode = cardCode,
                cardSerial = cardSerial,
                issuer = issuer,
                transRef = transRef,
                accountId = accountId,
                userGame = userGame,
                price = price,
                signature = signature
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(json);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<MobileResponse>(result);

                return card;

            }
            catch (Exception e)
            {
                card.status = "-99";
                card.description = e.Message;
            }
            card.Signature = signature;
            return card;
        }
    }
}