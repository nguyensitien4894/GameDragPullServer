using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Lib
{
    public class WebClass
    {
        public static string GetRequestBody()
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }
        public static string SendPost(string postData, string url, string contentType = "application/json; charset=UTF-8")
        {
            StringBuilder s = new StringBuilder();
            s.Append(contentType + "\r\nSendPost: " + url + "\r\nJSON DATA: " + postData);
            bool success = false;
            string resp;
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);
            CookieContainer cookie = new CookieContainer();
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "POST";
            myRequest.ContentLength = data.Length;
            myRequest.ContentType = contentType;
            myRequest.KeepAlive = false;
            myRequest.CookieContainer = cookie;
            myRequest.AllowAutoRedirect = false;
            using (Stream requestStream = myRequest.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }
            string responseXml = string.Empty;
            try
            {
                using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                {
                    if (myResponse.StatusCode != HttpStatusCode.OK)
                        success = false;
                    else
                        success = true;
                    using (Stream respStream = myResponse.GetResponseStream())
                    {
                        using (StreamReader respReader = new StreamReader(respStream))
                        {
                            responseXml = respReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Response != null)
                {
                    using (HttpWebResponse exResponse = (HttpWebResponse)webEx.Response)
                    {
                        using (StreamReader sr = new StreamReader(exResponse.GetResponseStream()))
                        {
                            responseXml = sr.ReadToEnd();
                        }
                    }
                }
            }
            if (success)
            {
                resp = responseXml;
            }
            else
            {
                resp = responseXml;

            }
            s.Append("\r\nResponse Data: " + resp);
            //Lib.Logs.SaveTrackingLog(s.ToString());
            return resp;
        }

        public static string SendBotTelegram(string data, int type = 1)
        {
            try
            {
                string url = "http://127.0.0.1:1316?type=" + type + "&p=" + TraditionGame.Utilities.Security.Security.Base64Encode(data);
                NLogManager.LogMessage("SendBotTelegram: " + url);
                string response = WebHelper.WebRequest(WebHelper.Method.GET, url, string.Empty);
                NLogManager.LogMessage(response);
                return response;
            }
            catch (Exception ex)
            {
                NLogManager.LogMessage("Error SendBotTelegram: " + ex);
                return "ERROR";
            }
        }
    }


    public class WebHelper
    {
        public enum Method { GET, POST, PUT };

        public static string WebRequest(Method method, string url, string postData)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "";
            webRequest.Timeout = 30000;

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());

                try
                {
                    requestWriter.Write(postData);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);
            webRequest = null;
            NLogManager.LogMessage(method.ToString() + ": " + url + "\r\nPost: " + postData + "\r\nResponse: " + responseData);
            return responseData;
        }

        public static string WebRequest(Method method, string contentType, string url, string postData)
        {
            NLogManager.LogMessage("WebRequest: " + url +
                "\r\nParam: " + postData +
                "\r\nMethod: " + method.ToString());
            try
            {
                HttpWebRequest webRequest = null;
                StreamWriter requestWriter = null;
                string responseData = "";

                webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
                webRequest.Method = method.ToString();
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.UserAgent = "";
                webRequest.Timeout = 60000;
                webRequest.ContentType = contentType;
                if (method == Method.POST)
                {
                    //webRequest.ContentType = contentType;

                    //POST the data.
                    requestWriter = new StreamWriter(webRequest.GetRequestStream());

                    try
                    {
                        requestWriter.Write(postData);
                    }
                    catch
                    {
                        throw;
                    }

                    finally
                    {
                        requestWriter.Close();
                        requestWriter = null;
                    }
                }
                else if (method == Method.PUT)
                {
                    //webRequest.ContentType = contentType;

                    //PUT the data.
                    webRequest.Headers["X-My-Custom-Header"] = "PUT";
                    requestWriter = new StreamWriter(webRequest.GetRequestStream());

                    try
                    {
                        requestWriter.Write(postData);
                    }
                    catch
                    {
                        throw;
                    }

                    finally
                    {
                        requestWriter.Close();
                        requestWriter = null;
                    }
                }

                responseData = WebResponseGet(webRequest);
                webRequest = null;
                NLogManager.LogMessage(responseData);
                return responseData;
            }
            catch (Exception ex)
            {
                NLogManager.LogError(ex.ToString());
                return "";
            }
        }

        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }
}