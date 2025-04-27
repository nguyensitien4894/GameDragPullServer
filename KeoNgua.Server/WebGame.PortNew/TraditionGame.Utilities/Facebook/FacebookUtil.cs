using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using MsTraditionGame.Utilities.Facebook;
using MsTraditionGame.Utilities.Log;

namespace MsTraditionGame.Utilities.Facebook
{
    public class FacebookUtil
    {
        public static FBAccount GetFBAccount(string UserInformation)
        {
            try
            {
                var FacebookResponseData = JObject.Parse(UserInformation.ToString());
                if (FacebookResponseData["email"] != null && FacebookResponseData["id"] != null)
                {
                    var FacebookId = (string)FacebookResponseData["id"];
                    var Name = (string)FacebookResponseData["name"];
                    //var FirstName = (string)FacebookResponseData["first_name"];
                    //var LastName = (string)FacebookResponseData["last_name"];
                    //var Link = (string)FacebookResponseData["link"];
                    var Email = (string)FacebookResponseData["email"];
                    //var Gender = (string)FacebookResponseData["gender"] == "male" ? "1" : "0";
                    //CultureInfo culture = new CultureInfo("en-US");
                    //DateTime Birthday = Convert.ToDateTime((string)FacebookResponseData["birthday"], culture);
                    //var Verify = "0";
                    //DateTime updateTime = Convert.ToDateTime((string)FacebookResponseData["update_time"], culture);

                    var FbAccount = new FBAccount(Convert.ToInt64(FacebookId), Name, Email);

                    return FbAccount;
                }

                return new FBAccount();
            }
            catch (Exception exx)
            {
                NLogManager.PublishException(exx);
                return new FBAccount();
            }
        }

        public static List<IDs_Business> GetIDsForBusiness(string accessToken)
        {
            var returnList = new List<IDs_Business>();
            try
            {
                // xử lý lấy listapp theo business để có được scope-userid
                var requestLink = string.Format("https://graph.facebook.com/v2.0/me/ids_for_business?access_token={0}", accessToken);
                var business = GetDataUrl.RequestResponse(requestLink);
                var business_data = business.ToString().Substring(business.ToString().IndexOf('['), business.ToString().IndexOf(']') - business.ToString().IndexOf('[') + 1);
                NLogManager.LogMessage("business_data:" + business_data);
                if (business_data != "[]")
                {
                    var businessInfo = business_data.Replace("namespace", "name_space");
                    returnList = new JavaScriptSerializer().Deserialize<List<IDs_Business>>(businessInfo);
                }
                return returnList;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return returnList;
            }
        }

        public static object GetHttpResponse(string url)
        {
            try
            {
                // ReSharper disable once CSharpWarnings::CS0618
                ServicePointManager.CertificatePolicy = new WebPost.TrustAllCertificatePolicy();
                var myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                //myRequest.ContentLength = data.Length;
                myRequest.CookieContainer = new CookieContainer();
                //myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentType = "text/xml; encoding='utf-8'";
                myRequest.KeepAlive = false;

                using (var myResponse = (HttpWebResponse)myRequest.GetResponse())
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var reader = new StreamReader(myResponse.GetResponseStream()))
                    {
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        if (reader != null)
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                return null;
            }
            catch (WebException ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        public static string GraphApiUri = "https://graph.facebook.com/";
        public static FbUserInfo GetFbUserInfo(string accessToken)
        {
            try
            {
                var resultApp = GetDataUrl.RequestResponse(GraphApiUri + "app?access_token=" + accessToken);
                dynamic appInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(resultApp);
                if (appInfo.id > 0)
                {
                    var resultGetUser = GetDataUrl.RequestResponse(GraphApiUri + "me?fields=id,email&access_token=" + accessToken);
                    dynamic userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(resultGetUser);
                    if (userInfo.id > 0)
                    {
                        return new FbUserInfo()
                        {
                            UserId = userInfo.id,
                            Email = userInfo.email,
                            ResponeCode = 1
                        };
                    }
                }
                NLogManager.LogMessage("AccessTokenFail=>" + accessToken);
                return new FbUserInfo()
                {
                    ResponeCode = -1
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new FbUserInfo()
                {
                    ResponeCode = -99
                };
            }
        }

        public class FbUserInfo
        {
            public long UserId { get; set; }
            public string Email { get; set; }
            public long FbAppId { get; set; }
            public int ResponeCode { get; set; }
            public string ListFbAppBusiness { get; set; }
        }
    }

    public static class WebPost
    {
        /// <summary>Send the Message to PalPal Checkout</summary>
        public static string SendPost(string postData, string url)
        {
            bool success = false;
            string resp;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);

            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();

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

            return resp;
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

    public static class GetDataUrl
    {
        public static string GetDataInfo(string uri, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";//GET            
                request.Accept = "JSON";
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }
                var webResponse = request.GetResponse();
                if (webResponse == null)
                {
                    return string.Empty;
                }
                var sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return string.Empty;
            }
        }

        public static string RequestResponse(string url)
        {
            string pageContent = string.Empty;
            try
            {
                System.Net.HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Credentials = CredentialCache.DefaultCredentials;
                //// Get the response
                WebResponse webResponse = myRequest.GetResponse();
                Stream respStream = webResponse.GetResponseStream();
                if (respStream != null)
                {
                    ////
                    StreamReader ioStream = new StreamReader(respStream);
                    pageContent = ioStream.ReadToEnd();
                    //// Close streams
                    ioStream.Close();
                    respStream.Close();
                    return pageContent;
                }
                return string.Empty;
            }
            catch (WebException webEx)
            {
                if (webEx.Response != null)
                {
                    using (HttpWebResponse exResponse = (HttpWebResponse)webEx.Response)
                    {
                        using (StreamReader sr = new StreamReader(exResponse.GetResponseStream()))
                        {
                            pageContent = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return pageContent;
        }
    }
}