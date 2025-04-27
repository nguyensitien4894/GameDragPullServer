using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using MsWebGame.Portal.SendSmsServices;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Helpers.OTPs.MobileSMS
{
    public class OtpSend
    {
        public static OtpResponse Send1(string phone, string content)
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_URL"];
            string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
            string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
            string brandname = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_BRANCHNAME"];
            String urlParameter = String.Format("user={0}&password={1}&brandname={2}&phone={3}&content={4}", user, password, brandname, phone, content);

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

                var xdoc = XDocument.Parse(result);
                XElement xCode = (from xml2 in xdoc.Descendants("CODE")
                                  select xml2).FirstOrDefault();
                XElement DES = (from xml2 in xdoc.Descendants("DES")
                                select xml2).FirstOrDefault();
                model.code = xCode.Value;
                model.des = xCode.Value;

            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                Console.WriteLine(e.Message);
            }
            return model;
        }
        static bool IsVieNamMobilePhone(string phoneNumber)
        {
            var listPhone = new List<string>() { "092", "056", "058", "0186", "0188" };
            var splitPhone = phoneNumber.Substring(0, 3);
            return listPhone.Contains(splitPhone);


        }

        /// <summary>
        /// gửi otp mới trên cổng mới
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static sendsmsresponse Send2(string phone, string content)
        {
            if (IsVieNamMobilePhone(ConvertUtil.PhoneDisplayFormat(phone)))
            {
                sendsmsresponse model = new sendsmsresponse();
                string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
                string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
                string brandname = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSVNMobile_BRANCHNAME"];
                model = new sendClient().send(user, password, brandname, content, "1", phone, DateTime.Now.Ticks.ToString());
                return model;
            }
            else
            {
                sendsmsresponse model = new sendsmsresponse();
                string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
                string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
                string brandname = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_BRANCHNAME"];
                model = new sendClient().send(user, password, brandname, content, "1", phone, DateTime.Now.Ticks.ToString());
                return model;
            }
            
        }

        public static string Send3(string phone, string content)
        {
            //OtpResponse model = new OtpResponse();

            String result = string.Empty;

            String url = "http://api.mobiservices.vn:9009/sendMT";
            string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
            string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
            string brandname = "DAUSODAI";
            string key = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_KEY"];
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                user = user,
                password = password,
                brandname = brandname,
                phone = phone,
                content = content
            });
            NLogManager.LogMessage("SMS="+urlParameter);
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
                var model = JsonConvert.DeserializeObject<SmsDetail1>(result);


            }
            catch (Exception e)
            {
                //model.code = "-99";
                //model.des = e.Message;
                result = "-99";
                Console.WriteLine(e.Message);
            }

            if (result.Contains("SUCCESS"))
            {
                result = "1";
            }
            return result;
        }

        public static string Send4(string phone, string content)
        {
            //OtpResponse model = new OtpResponse();

            String result = string.Empty;

            
            string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
            string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
            String url = "http://167.179.105.5:1168/Callotp?username="+user+ "&password="+password+ "&msisdn="+phone+ "&mes="+content+ "&request_id="+DateTime.Now.Ticks.ToString();
            NLogManager.LogMessage("SMS=" + url);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                SmsDetail ob = JsonConvert.DeserializeObject<SmsDetail>(result);
                if(ob!=null)
                    return ob.error;
                else
                {
                    return "-99";
                }
            }
            catch (Exception e)
            {
                return "-99";
            }

        }
        public static string Send5(string phone, string content)
        {
            //OtpResponse model = new OtpResponse();

            String result = string.Empty;

            String url = "http://rest.simosms.com/MainService.svc/xml/SendMultipleMessage_V4_post";
            //{
            //    "apiKey": "94291297",
            //    "secretKey": "FC5D9CA9",
            //    "phone": "855318187799",
            //    "content": "Welcome to Vihat Global",
            //    "isUnicode": "0",
            //    "originalSender": "SMS Info"
            //}
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                apiKey = "94291297",
                secretKey = "FC5D9CA9",
                phone = phone,
                content = content,
                isUnicode = "0",
                originalSender = "SMS Info"
            });
            NLogManager.LogMessage("SMS=" + urlParameter);
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
                var model = JsonConvert.DeserializeObject<SmsDetail1>(result);


            }
            catch (Exception e)
            {
                //model.code = "-99";
                //model.des = e.Message;
                result = "-99";
                Console.WriteLine(e.Message);
            }

            if (result.Contains("\"CodeResult\":\"100\""))
            {
                result = "1";
            }
            return result;
        }
    }

    public class SmsDetail
    {
        public string error { get; set; }
        public string description { get; set; }
    }
    public class SmsDetail1
    {
        public int code { get; set; }
        public string des { get; set; }
    }
}