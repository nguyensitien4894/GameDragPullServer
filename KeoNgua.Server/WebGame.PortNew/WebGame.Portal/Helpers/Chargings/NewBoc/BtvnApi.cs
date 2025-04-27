using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Helpers.Chargings.NewBoc
{
    public class BtvnAPI
    {
        public static DogiadungResponse SendRequest(string requestId, string cardNumber, string serialNumber, int cardValue, string cardtype, long accountId)
        {
            string urlService = "http://66.42.62.119:8082/partner/RequestPayment";
            string partnerKey = "JeCQLvEBkuslB2VyLhGscUyDTFeI3nYsuErH2U9ekQpf6D1mWpbCGHi7kFBbtiyE";
            string CallBackUrl = "https://callback01052023.sicbet.net/api/CardCharging/ReceiveResult";
            var serviceResponse = new DogiadungResponse();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var httpClient = new HttpClient();
                var signature = md5(partnerKey + cardValue + requestId + serialNumber);

                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("CardSeri", serialNumber));
                postData.Add(new KeyValuePair<string, string>("CardCode", cardNumber));
                postData.Add(new KeyValuePair<string, string>("CardType", cardtype));
                postData.Add(new KeyValuePair<string, string>("AccountName", accountId.ToString()));
                postData.Add(new KeyValuePair<string, string>("TransID", requestId));
                postData.Add(new KeyValuePair<string, string>("Amount", cardValue.ToString()));
                postData.Add(new KeyValuePair<string, string>("Signature", signature));
                postData.Add(new KeyValuePair<string, string>("ApiToken", partnerKey));
                postData.Add(new KeyValuePair<string, string>("UrlCallBack", CallBackUrl));
                var content = new FormUrlEncodedContent(postData);
                var response = httpClient.PostAsync(urlService, content).Result;
                var htmlData = response.Content.ReadAsStringAsync().Result;

                serviceResponse = JsonConvert.DeserializeObject<DogiadungResponse>(htmlData);

                NLogManager.LogMessage("Response: " + htmlData);

            }
            catch(Exception ex)
            {
                //card.ResponseCode = -99;
                //card.ResponseContent = "Exception Code";
                NLogManager.LogMessage("SendRequest -99" + ex.StackTrace);
                NLogManager.PublishException(ex);
            }
            return serviceResponse;
        }

        private static BtvnResponse PostJson(string uri, string postData)
        {
            //NLogManager.LogMessage("P2-Postdata:" + postData);
            var card = new BtvnResponse();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                
                 request.Timeout = 1000 * 120;
                request.Method = "POST";//GET
                                        //request.Accept = "JSON";
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }
              

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var  result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
           
                card = JsonConvert.DeserializeObject<BtvnResponse>(result);

                return card;
            }catch(Exception ex)
            {
                //card.ResponseCode = -99;
                //card.ResponseContent = "Exception Code";
                NLogManager.LogMessage("PostJson -99" + ex.StackTrace);
                NLogManager.PublishException(ex);
            }
           
            return card;
        }


        public static BtvnResponse SendRequest_OLD(string requestId, string cardNumber, string serialNumber, int cardValue, string cardtype, long accountId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            String result = string.Empty;
            var card = new BtvnResponse();
            // string username = "G32";
            // string Secretkey = "2333560f-1281-48f2-83d9-a4d9a4687a97";
            string apikey = "JeCQLvEBkuslB2VyLhGscUyDTFeI3nYsuErH2U9ekQpf6D1mWpbCGHi7kFBbtiyE";
            string billcode = accountId + "|" + requestId;
            // string signature = md5(billcode + serialNumber + cardNumber + cardtype + cardValue + Secretkey);
            string signature = md5(apikey + cardValue + billcode + serialNumber);
            string urlCallback = HttpContext.Current.Server.UrlEncode("https://callback01052023.sicbet.net/api/CardCharging/ReceiveResult");
            //{{ApiUrl}}/api/SIM/RegCharge?apiKey={{apiKey}}&code={{mã_thẻ}}&serial={{serial}}&type={{loại_thẻ}}&menhGia={{menhGia}}&requestId={{requestId}}
            //string url = "http://gakon.club:10004/api/SIM/RegCharge?";
            string postString1 = $"http://66.42.62.119:8082/partner/RequestPayment?apiKey={apikey}&code={cardNumber}&serial={serialNumber}&type={cardtype}&menhGia={cardValue}&requestId={requestId}&callback={urlCallback}";
            NLogManager.LogMessage("Url Param : " + postString1);

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(postString1);

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                
                card = JsonConvert.DeserializeObject<BtvnResponse>(responseString);
                NLogManager.LogMessage("result : " + responseString);
                recheck:
                Task.Delay(6000).Wait();
                if(card?.data.id != null)
                {
                    var queryCheck = $@"http://gakon.club:10004/api/SIM/CheckCharge?apikey={apikey}&id={card?.data.id}";
                    var resultCheck = (HttpWebRequest)WebRequest.Create(queryCheck);
                    var responseCheck = (HttpWebResponse)resultCheck.GetResponse();
                    var jsonCheckStr = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var tempcard = JsonConvert.DeserializeObject<BtvnResponse>(jsonCheckStr);
                    card.status = tempcard.stt;
                    card.message = tempcard.data.status;
                    if (tempcard.data.status == "waiting" || tempcard.data.status == "processing") goto recheck;
                }

                //GOI
                //var request = (HttpWebRequest)WebRequest.Create(postString2);
                //var response2 = (HttpWebResponse)request.GetResponse();

            }

            catch (Exception e)
            {
                card.message = "-99";
                NLogManager.PublishException(e);
            }



            return card;
        }

        public static BtvnResponse SendRequest_OLD_1(string requestId, string cardNumber, string serialNumber, int cardValue, string cardtype, long accountId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            String result = string.Empty;
            var card = new BtvnResponse();
            string username = "G32";
            string Secretkey = "2333560f-1281-48f2-83d9-a4d9a4687a97";
            string apikey = "18cfcc1b-6811-4fe2-a94a-02b085012d42";
            string billcode = accountId + "|" + requestId;
            string signature = md5(billcode + serialNumber + cardNumber + cardtype + cardValue + Secretkey);
            string urlCallback = HttpContext.Current.Server.UrlEncode("https://callback01052023.sicbet.net/api/CardCharging/ReceiveResult");
            //{{ApiUrl}}/api/SIM/RegCharge?apiKey={{apiKey}}&code={{mã_thẻ}}&serial={{serial}}&type={{loại_thẻ}}&menhGia={{menhGia}}&requestId={{requestId}}
            //string url = "http://gakon.club:10004/api/SIM/RegCharge?";
            //string postString = string.Format("http://gakon.club:10004/api/SIM/RegCharge?apiKey={apikey}&billcode={0}&serial={1}&pin={2}&telco={3}&amount={4}&signature={5}&callback_url={6}", billcode, serialNumber, cardNumber, cardtype, cardValue, signature, urlCallback);
            string postString1 = $"http://gakon.club:10004/api/SIM/RegCharge?apiKey={apikey}&code={cardNumber}&serial={serialNumber}&type={cardtype}&menhGia={cardValue}&requestId={requestId}&callback={urlCallback}";
            NLogManager.LogMessage("Url Param : " + postString1);

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(postString1);

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                
                card = JsonConvert.DeserializeObject<BtvnResponse>(responseString);
                NLogManager.LogMessage("result : " + responseString);
                recheck:
                Task.Delay(6000).Wait();
                if(card?.data.id != null)
                {
                    var queryCheck = $@"http://gakon.club:10004/api/SIM/CheckCharge?apikey={apikey}&id={card?.data.id}";
                    var resultCheck = (HttpWebRequest)WebRequest.Create(queryCheck);
                    var responseCheck = (HttpWebResponse)resultCheck.GetResponse();
                    var jsonCheckStr = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var tempcard = JsonConvert.DeserializeObject<BtvnResponse>(jsonCheckStr);
                    card.status = tempcard.stt;
                    card.message = tempcard.data.status;
                    if (tempcard.data.status == "waiting" || tempcard.data.status == "processing") goto recheck;
                }

                //GOI
                //var request = (HttpWebRequest)WebRequest.Create(postString2);
                //var response2 = (HttpWebResponse)request.GetResponse();

            }

            catch (Exception e)
            {
                card.message = "-99";
                NLogManager.PublishException(e);
            }



            return card;
        }

        public static BtvnResponse SendRequest_OLD_2(string requestId, string cardNumber, string serialNumber, int cardValue, string cardtype, long accountId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            String result = string.Empty;
            var card = new BtvnResponse();
            string username = "GHL";
            string Secretkey = "6471b628c0d5e04d98d60e1e267dd9b6_NEED_CHANGE";
            string billcode = accountId + "|" + requestId;
            string signature = md5(billcode + serialNumber + cardNumber + cardtype + cardValue + Secretkey);
            string urlCallback = HttpContext.Current.Server.UrlEncode("https://callback01052023.sicbet.net/api/CardCharging/ReceiveResult");
            string url = "http://btvn.online/api/telco/register";
            string postString = string.Format("http://btvn.online/api/telco/register?partner=GHL&billcode={0}&serial={1}&pin={2}&telco={3}&amount={4}&signature={5}&callback_url={6}", billcode, serialNumber, cardNumber, cardtype, cardValue, signature, urlCallback);
            NLogManager.LogMessage("Url Param : " + postString);
       
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(postString);

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                card = JsonConvert.DeserializeObject<BtvnResponse>(responseString);
                NLogManager.LogMessage("result : " + responseString);

            }

            catch (Exception e)
            {
                card.message = "-99";
                NLogManager.PublishException(e);
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

    public class DogiadungResponse
    {
        public int errorCode { get; set; }
        public string msg { get; set; }
    }

    public class BtvnResponse
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public CardId data { get; set; }

        public int status { get; set; }
        public string message { get; set; }
    }

    public class CardResponse
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public CardId CardId { get; set; }
    }
   
    public class CardId
    {
        public int id { get; set; }
        public string status { get; set; }
    }
}