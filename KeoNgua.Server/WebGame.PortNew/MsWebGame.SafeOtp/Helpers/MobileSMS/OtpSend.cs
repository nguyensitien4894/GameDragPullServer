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
using MsWebGame.SafeOtp.SendSmsServices;
using TraditionGame.Utilities;

namespace MsWebGame.SafeOtp.Helpers.OTPs.MobileSMS
{
    public class OtpSend
    {
        static bool IsVieNamMobilePhone(string phoneNumber)
        {
            var listPhone = new List<string>() { "092", "056", "058", "0186", "0188" };
            var splitPhone = phoneNumber.Substring(0, 3);
            return listPhone.Contains(splitPhone);


        }
        public static OtpResponse SendTele(string phone, string content)
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = System.Configuration.ConfigurationManager.AppSettings["OTP_TELE_URL"];

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
                var data = Encoding.ASCII.GetBytes("");
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                model.code = result;
                model.des ="";
            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                NLogManager.PublishException(e);
            }
            return model;
        }

        public static string Send3(string phone, string content)
        {
            //OtpResponse model = new OtpResponse();

            String result = string.Empty;

            String url = "http://210.211.108.20:8080/onsmsapivoice/postSMS";
            string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
            string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
            string brandname = "DAUSO1";
            string key = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_KEY"];
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                username = user,
                pass = password,
                sender = brandname,
                key = key,
                phonesend = phone,
                message = content
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
                //model = JsonConvert.DeserializeObject<OtpResponse>(result);


            }
            catch (Exception e)
            {
                //model.code = "-99";
                //model.des = e.Message;
                result = "-99";
                Console.WriteLine(e.Message);
            }
            if (result.Contains(';'))
            {
                result = result[0].ToString();
            }
            return result;
        }

        //public static sendsmsresponse Send3(string phone, string content)
        //{
        //    sendsmsresponse model = new sendsmsresponse();
        //    if (IsVieNamMobilePhone(ConvertUtil.PhoneDisplayFormat(phone)))
        //    {              
        //        string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
        //        string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
        //        string brandname = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSVNMobile_BRANCHNAME"];
        //        model = new sendClient().send(user, password, brandname, content, "1", phone, DateTime.Now.Ticks.ToString());
        //        return model;
        //    }
        //    else
        //    {               
        //        string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
        //        string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
        //        string brandname = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_BRANCHNAME"];
        //        model = new sendClient().send(user, password, brandname, content, "1", phone, DateTime.Now.Ticks.ToString());
        //    }
            
        //    return model;
        //}

        public static OtpResponse Send2(string phone, string content)
        {
            OtpResponse model = new OtpResponse();

            String result = string.Empty;

            String url = "http://210.211.108.20:8080/onsmsapivoice/postSMS";
            string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
            string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
            string brandname = "DAUSO1";
            string key = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_KEY"];
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                username = user,
                pass = password,
                sender = brandname,
                key= key,
                phonesend = phone,
                message = content
            });

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
                model = JsonConvert.DeserializeObject<OtpResponse>(result);


            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                Console.WriteLine(e.Message);
            }
            return model;
        }
        public static OtpResponse Send1(string phone,string content)
        {
            OtpResponse model = new OtpResponse();

            String result = string.Empty;

            String url = "http://api.mobiservices.vn:9009/sendMT";
            string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
            string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
            string brandname = "DAUSODAI";
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                username = user,
                password = password,
                brandname = brandname,
                phone = phone,
                content = content
            });

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
                model = JsonConvert.DeserializeObject<OtpResponse>(result);


            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                Console.WriteLine(e.Message);
            }
            return model;
        }

        public static string Send4(string phone, string content)
        {
            //OtpResponse model = new OtpResponse();
            OtpResponse model = new OtpResponse();

            String result = string.Empty;

            String url = "http://api.mobiservices.vn:9009/sendMT";
            string user = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_User"];
            string password = System.Configuration.ConfigurationManager.AppSettings["OTP_SMSMobile_PASSWORD"];
            string brandname = "DAUSODAI";
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                username = user,
                password = password,
                brandname = brandname,
                phone = phone,
                content = content
            });

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
                model = JsonConvert.DeserializeObject<OtpResponse>(result);


            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                Console.WriteLine(e.Message);
            }
            return model.code;

        }
    }

    public class SmsDetail
    {
        public string error { get; set; }
        public string description { get; set; }
    }
}