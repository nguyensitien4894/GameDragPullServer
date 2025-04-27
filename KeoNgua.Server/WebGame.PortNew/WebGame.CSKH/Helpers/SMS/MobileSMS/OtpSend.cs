using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace MsWebGame.CSKH.Helpers.OTPs.MobileSMS
{
    public class OtpSend
    {
        public static OtpResponse Send(string phone, string content)
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
    }
}