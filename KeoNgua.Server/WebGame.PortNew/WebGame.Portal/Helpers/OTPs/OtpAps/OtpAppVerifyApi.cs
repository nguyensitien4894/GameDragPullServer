using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Helpers.OTPs.OtpAps
{
    public class OtpAppVerifyApi
    {
        public static OtpAppResponse SendRequest(long AccountID, string Otp)
        {

            OtpAppResponse objRes = new OtpAppResponse();
            try
            {


                String url = ConfigurationManager.AppSettings["OTP_APP_VERIFY_URL"].ToString() + "/api/Otp/VerifyOtp";
               
                var obj = new
                {
                    AccountID = AccountID,
                    Otp = Otp

                };
                var json = new JavaScriptSerializer().Serialize(obj);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(json);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                objRes = JsonConvert.DeserializeObject<OtpAppResponse>(result);
                return objRes;

            }
            catch (Exception e)
            {
                NLogManager.PublishException(e);
                objRes = new OtpAppResponse();
                objRes.ResponseCode = -99;
            }

            return objRes;
        }
    }
}