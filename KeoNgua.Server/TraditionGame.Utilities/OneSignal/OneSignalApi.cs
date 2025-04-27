using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TraditionGame.Utilities.OneSignal
{
    public class OneSignalApi: BaseOneSignal
    {

        public static void  SendByPlayerID(List<string> playerIDs,string content)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var request = WebRequest.Create(OneSignalUrl) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    app_id = APP_ID,
                    contents = new { en = content },
                    small_icon = "ic_launcher",
                    large_icon = "Small Icon",
                    include_player_ids = playerIDs.ToArray()
                };



                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;

                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    NLogManager.PublishException(ex);
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                }

            });
               


        }


        public static void UpdateTags(string oneSignalID,long UserID,string DisplayName,int ServiceID)
        {

            var request = WebRequest.Create(OneSignalUpdate_User+ oneSignalID) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "PUT";
            request.ContentType = "application/json; charset=utf-8";

            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = APP_ID,
                
                tags = new object[] { new { UserID = UserID,DisplayName= DisplayName, ServiceID= ServiceID } },
            };
        



            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                NLogManager.PublishException(ex);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }



        }
    }
}
