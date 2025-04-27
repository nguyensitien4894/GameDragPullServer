using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MsWebGame.CSKH.Controllers
{
    [RoutePrefix("api/Notify")]
    public class NotifyController : ApiController
    {
        [HttpPost]
        [Route("SendTeleNotify")]
        public dynamic SendTeleNotify(dynamic input)
        {

            try
            {
                // Program.SendVinaRequest("1", "99441437470332", "51500118554742", 20000);
                string chatID = input.ChatID;
                string content = input.Content;
                if (String.IsNullOrEmpty(chatID)|| String.IsNullOrEmpty(content))
                {
                    return new
                    {
                        ResponseCode =- 1,

                    };
                }
                
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
                // Do what you want with response

            }
            catch (Exception ex)
            {
                return new
                {
                    ResponseCode = 0,

                };
            }



        }


        [HttpPost]
        [Route("SendPreMoney")]
        public dynamic SendPreMoney(dynamic input)
        {

            try
            {
                // Program.SendVinaRequest("1", "99441437470332", "51500118554742", 20000);
                string chatID = input.ChatID;
                string content = input.Content;
                if (String.IsNullOrEmpty(chatID) || String.IsNullOrEmpty(content))
                {
                    return new
                    {
                        ResponseCode = -1,

                    };
                }

                string apiToken = "790372753:AAEPdM2KWY6d_5W3ZoEc7OaqNipmVzMNXMw";
                string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";


                urlString = String.Format(urlString, apiToken, chatID, content);
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
                // Do what you want with response

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
