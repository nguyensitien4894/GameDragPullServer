using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using MsWebGame.Portal.Helpers.Chargings.Cards;
using MsWebGame.Portal.ShopMuaThe;
using Newtonsoft.Json;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Helpers.Chargings.HaDongPho
{
    public class HaDongPhoAPI
    {
        public static HaDongPhoResponse SendRequest(string requestId, string cardNumber, string serialNumber, int cardValue, string cardtype)
        {
            String result = string.Empty;
            HaDongPhoResponse card = new HaDongPhoResponse();
            String url = ConfigurationManager.AppSettings["HDP_CHARGING_URL"].ToString();
            string username = ConfigurationManager.AppSettings["HDP_USERNAME"].ToString();
            string password = ConfigurationManager.AppSettings["HDP_PASSWORD"].ToString();
            
            
            
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                username = username,
                password = password,
                requestid = requestId,
                serial = serialNumber,
                pincode = cardNumber,
                telco = cardtype,
                amount = cardValue
            }); NLogManager.LogMessage("Url Param : "+urlParameter);
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
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                card = JsonConvert.DeserializeObject<HaDongPhoResponse>(result);

            }
            catch (Exception e)
            {
                card.errorcode = "-99";
            }

            return card;
        }
    }
}