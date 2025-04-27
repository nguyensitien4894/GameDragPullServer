using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TraditionGame.Utilities.XBomms.Models;

namespace TraditionGame.Utilities.XBomms
{
   public  class XBommApi
    {
        //private static string url = "https://cv3.xboom.net/restapi/v3.0.1/partner/card";
        // private static string token = ConfigurationManager.AppSettings["XBOOM_TOKEN"].ToString();
        private static string token = ConfigurationManager.AppSettings["XBOOM_TOKEN"].ToString();
         private static string url = ConfigurationManager.AppSettings["XBOOM_URL"].ToString();
        private static string CallbackUrl = ConfigurationManager.AppSettings["XBOOM_CallbackUrl"].ToString();

        public static XBoomResponseModel SendRequest(string Telco, int Amount, string Serial, string Code)
        {
            try
            { 
            



                var requestData = new XBoomRequestModel()
                {
                    Telco = Telco,
                    Amount = Amount,
                    Serial = Serial,
                    Code = Code,
                    ScratchCallbackUrl = CallbackUrl
                };



                var serviceResponse = PostJson(url, JsonConvert.SerializeObject(requestData),token);
                
                return serviceResponse;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }


        }

        private static XBoomResponseModel PostJson(string uri, string postData,string token)
        {
            var card = new XBoomResponseModel();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Headers["token"] = token;
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
                card = JsonConvert.DeserializeObject<XBoomResponseModel>(result);

                return card;
            }
            catch (Exception ex)
            {
                card.code =-99;
                card.message = ex.Message;
            }
            return card;
        }


    }
}
