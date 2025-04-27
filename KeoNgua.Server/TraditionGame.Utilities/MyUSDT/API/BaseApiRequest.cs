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

namespace TraditionGame.Utilities.MyUSDT.API
{
    public class BaseApiRequest
    {
        protected static int CURRENT_PAIR = 1;
        protected static string BaseUrl = "https://api.traodoiusdt.com";
        protected static string EMAIL_BANK = ConfigurationManager.AppSettings["EMAIL_BANK"].ToString();
        protected static string Access_Token = ConfigurationManager.AppSettings["Access_Token"].ToString();
        protected static string ReceiveUsdtAddress = ConfigurationManager.AppSettings["ReceiveUsdtAddress"].ToString();
        protected static T PostJson<T>(string uri,  string postData,string accesstoken,out int statusCode,out string HttpMsg) where T : new()
        {
            var objReturn = new T();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                if (!String.IsNullOrEmpty(accesstoken))
                {
                    request.Headers.Add("Authorization", accesstoken);
                }
                request.Method = "POST";//GET

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                statusCode = (int)response.StatusCode;
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

        protected static T GetJson<T>(string uri,string accesstoken,  out int statusCode,out string HttpMsg) where T : new()
        {
            var objReturn = new T();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                if (!String.IsNullOrEmpty(accesstoken))
                {
                    request.Headers.Add("Authorization", accesstoken);
                }



                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                statusCode = (int)response.StatusCode;
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