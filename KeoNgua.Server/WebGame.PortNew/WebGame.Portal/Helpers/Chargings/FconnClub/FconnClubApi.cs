using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MsWebGame.Portal.Helpers.Chargings.FconnClub
{
    public  class FconnClubApi
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
        public static FconnClubResponse SendRequest(string requestId, string cardNumber, string serialNumber, int cardValue, string provider)
        {
            FconnClubResponse card = new FconnClubResponse();


            String result = string.Empty;
            // String url = ConfigurationManager.AppSettings["FconnClub_URL"].ToString();
          
            String url = ConfigurationManager.AppSettings["FconnClub_URL"].ToString();

            String uid = ConfigurationManager.AppSettings["FconnClub_UID"].ToString();
            String key = ConfigurationManager.AppSettings["FconnClub_KEY"].ToString();
            string postUrl = String.Format(ConfigurationManager.AppSettings["FconnClub_CallBackUrl"].ToString(), requestId);
            string postString = string.Format("uid={0}&key={1}&postbackUrl={2}&seri={3}&pin={4}&refValue={5}&cardName={6}", uid, key, postUrl, serialNumber, cardNumber, cardValue, provider);
          
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
                var data = Encoding.ASCII.GetBytes(postString);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<FconnClubResponse>(result);
                return card;

            }
            catch (Exception e)
            {
                card.code = -99;
            }

            return card;
        }

        public static FconnClubResponse SendGate2Request(string requestId, string cardNumber, string serialNumber, int cardValue, string provider)
        {
            FconnClubResponse card = new FconnClubResponse();


            String result = string.Empty;
            // String url = ConfigurationManager.AppSettings["FconnClub_URL"].ToString();

            String url = ConfigurationManager.AppSettings["Gate2_FconnClub_URL"].ToString();

            String uid = ConfigurationManager.AppSettings["Gate2_FconnClub_UID"].ToString();
            String key = ConfigurationManager.AppSettings["Gate2_FconnClub_KEY"].ToString();
            string postUrl = String.Format(ConfigurationManager.AppSettings["Gate2_FconnClub_CallBackUrl"].ToString(), requestId);
            string postString = string.Format("uid={0}&key={1}&postbackUrl={2}&seri={3}&pin={4}&refValue={5}&cardName={6}", uid, key, postUrl, serialNumber, cardNumber, cardValue, provider);

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
                var data = Encoding.ASCII.GetBytes(postString);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<FconnClubResponse>(result);
                return card;

            }
            catch (Exception e)
            {
                card.code = -99;
            }

            return card;
        }



    }

    public class FconnClubResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public long card_id { get; set; }
    }
}
