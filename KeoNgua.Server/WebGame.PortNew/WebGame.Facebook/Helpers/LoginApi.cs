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

namespace MsWebGame.Facebook.Helpers
{
    public class LoginApi
    {
        public class Account
        {
            public long AccountID { get; set; }
            public string AccountName { get; set; }
            public int AvatarID { get; set; }
            public long Balance { get; set; }
            public int Status { get; set; }
            public int Gender { get; set; }
            public DateTime BirthDay { get; set; }
            public string PhoneNumber { get; set; }
            public int PendingMessage { get; set; }
            public int PendingGiftcode { get; set; }
            public int TotalWin { get; set; }
            public int TotalLose { get; set; }
            public int TotalDraw { get; set; }
            public bool IsUpdateAccountName { get; set; }
            public int? AuthenType { get; set; }
            public long RankID { get; set; }
            public long VP { get; set; }
            public string RankName { get; set; }
        }

        public class ResponseLogin
        {
            public int ResponseCode { get; set; }
            public Account AccountInfo { get; set; }
        }

        public static ResponseLogin Send(String Token)
        {





            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestData = serializer.Serialize(new
            {
                LoginType = 2,
                AccessToken = Token,
                DeviceId = "ios",
                DeviceType = "1",



            });
            var serviceResponse = PostJson("https://portal.g63.vin/api/Account/Login", requestData);

            return serviceResponse;
        }

        private static ResponseLogin PostJson(string uri, string postData)
        {
            var rtObj = new ResponseLogin();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                                        //request.Accept = "JSON";
                ((HttpWebRequest)request).CookieContainer = new CookieContainer();
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                rtObj = JsonConvert.DeserializeObject<ResponseLogin>(result);

                return rtObj;
            }
            catch (Exception ex)
            {

                return null;
            }

            return rtObj;
        }



    }

}