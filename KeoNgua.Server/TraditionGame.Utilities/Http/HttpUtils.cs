using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace TraditionGame.Utilities.Http
{
    public static class HttpUtils
    {
        public static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string content, string reason)
        {
            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(content),
                ReasonPhrase = reason
            };
        }

        public static HttpResponseMessage CreateResponseNonReason(HttpStatusCode statusCode, string content)
        {
            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(content),
            };
        }

        public static string SendPost(string postData, string url)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);
            //System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            System.Net.ServicePointManager.Expect100Continue = false;
            CookieContainer cookie = new CookieContainer();
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "POST";
            myRequest.ContentLength = data.Length;
            myRequest.ContentType = "application/x-www-form-urlencoded";
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
            return responseXml;
        }

        /// <summary>Send the Message to Merchant</summary>
        public static string GetStringHttpResponse(string url)
        {
            string response = null;
            try
            {
                //System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                //myRequest.ContentLength = data.Length;
                myRequest.CookieContainer = new CookieContainer();
                //myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentType = "text/xml; encoding='utf-8'";
                //myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.KeepAlive = false;
                using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                {
                    using (var reader = new StreamReader(myResponse.GetResponseStream()))
                    {
                        if (reader != null)
                        {
                            response = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                // Graph API Errors or general web exceptions
                throw ex;
            }
            catch (Exception)
            {
            }

            return response;
        }

        /// <summary>Send the Message to Merchant</summary>
        public static object GetHttpResponse(string url)
        {
            object response = null;
            try
            {
                //System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                //myRequest.ContentLength = data.Length;
                myRequest.CookieContainer = new CookieContainer();
                //myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentType = "text/xml; encoding='utf-8'";
                myRequest.KeepAlive = false;

                using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                {
                    using (var reader = new StreamReader(myResponse.GetResponseStream()))
                    {
                        if (reader != null)
                        {
                            response = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
            }

            return response;
        }

        public static string GetUserAgent()
        {
            try
            {
                var userAgent = HttpContext.Current.Request.UserAgent;
                if (!string.IsNullOrEmpty(userAgent))
                    return userAgent;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return string.Empty;
        }

        /// <summary>
        ///    Classed used to bypass self-signed server certificate
        /// </summary>
        /// <remarks>
        ///     To be used in development only.
        /// </remarks>
        public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
        {
            public TrustAllCertificatePolicy()
            {
            }

            public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
            {
                return true;
            }
        }
    }
}
