using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
namespace MsWebGame.CSKH.Utils
{
    public class Telegram
    {
        public static dynamic send(string chatID, string content)
        {
            try
            {
                string apiToken = ConfigurationManager.AppSettings["TOKEN_NOTIFY_TELE"].ToString();
                string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";


                urlString = String.Format(urlString, apiToken, chatID, content);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebRequest request = WebRequest.Create(urlString);
                Stream rs = request.GetResponse().GetResponseStream();

                StreamReader reader = new StreamReader(rs);
                string line = "";
                StringBuilder sb = new StringBuilder();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null)
                        sb.Append(line);
                }
                string response = sb.ToString();
                return new
                {
                    ResponseCode = 1,

                };
            }
            catch (Exception ex)
            {
                return new
                {
                    ResponseCode = 0,

                };
            }


        }
    }
}