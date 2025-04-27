using System;
using System.Collections.Generic;
using MsWebGame.Portal.Database.DTO;
using TraditionGame.Utilities;
using System.Threading;
using MsWebGame.Portal.Controllers;
using MsWebGame.RedisCache.Cache;
using TraditionGame.Utilities.Api;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MsWebGame.Portal.Handlers
{
    public class PortalHandler
    {
        private readonly static Lazy<PortalHandler> _instance = new Lazy<PortalHandler>(() => new PortalHandler());
        private static Dictionary<int, int> _mapHourToFrameID;

        public static PortalHandler Instance
        {
            get { return _instance.Value; }
        }
        public Timer _timer;
      


        private PortalHandler()
        {
            _timer = new Timer(Callback, null, 5000, Timeout.Infinite);
        }

        private void Callback(Object state)
        {

            try
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {

                        this.SessionID = 123;
                        var data = new JackpotController();
                        var GetTopJackPot = data.GetTopJackPot();
                        var GetJackpotInfo = data.GetJackpotInfo();
                        var GetGameJackpotInfo = data.GetGameJackpotInfo();


                        ConnectionHandler.Instance.HubContext.Clients.All.GetTopJackPot(GetTopJackPot);
                        ConnectionHandler.Instance.HubContext.Clients.All.GetJackpotInfo(GetJackpotInfo);
                        ConnectionHandler.Instance.HubContext.Clients.All.GetGameJackpotInfo(GetGameJackpotInfo);

                        dynamic res = JsonConvert.DeserializeObject<dynamic>(Get("https://fishapi.sicbet.net/api/BanCa/GetJackpots"));
                        ConnectionHandler.Instance.HubContext.Clients.All.GetJackpotShootFish(res);

                        // dynamic res1 = JsonConvert.DeserializeObject<dynamic>(Get("http://127.0.0.1:81/bancaapi/getslotconfig/5f67cff4ff68ad17a534d8c1f1ec6cdd/slot5b"));
                        dynamic res1 = JsonConvert.DeserializeObject<dynamic>(Get("http://127.0.0.1:81/bancaapi/getslotconfig/5f67cff4ff68ad17a534d8c1f1ec6cdd/slot5b"));
                        ConnectionHandler.Instance.HubContext.Clients.All.GetJackpotSlot5b(new long[] { res1["data"]["JACKPOT_SLOT53B_100"], res1["data"]["JACKPOT_SLOT53B_1000"], res1["data"]["JACKPOT_SLOT53B_10000"] });

                        _timer.Change(5000, Timeout.Infinite);

                    }
                    catch (Exception ex)
                    {
                        NLogManager.PublishException(ex);
                        _timer.Change(5000, Timeout.Infinite);

                    }
                });
            }
            catch (Exception ex)
            {
                _timer.Change(5000, Timeout.Infinite);

                NLogManager.PublishException(ex);
            }

        }
        public long SessionID { get; set; }

        private void SessionCallBack(object o)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        this.SessionID = 123;
                        ConnectionHandler.Instance.HubContext.Clients.All.Ahihi(this);
                        NLogManager.LogMessage("TESTP: " + this.SessionID);

                    }
                    catch (Exception ex)
                    {
                        NLogManager.PublishException(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public void ReturnTopupCard(long accountId, long balance, string msg,int ServiceID,int Status)
        {
           try
            {
                var lstConn = ConnectionHandler.Instance.GetConnections(accountId);
                NLogManager.LogMessage(string.Format("Send Hub Charge Connect:accountId: {0} | balance: {1} | msg: {2} | lstConn.Count: {3}",
                  accountId, balance, msg, lstConn == null ? 0 : lstConn.Count));
                if (lstConn != null && lstConn.Count > 0)
                {
                    foreach (var conn in lstConn)
                    {
                        ConnectionHandler.Instance.HubContext.Clients.Client(conn).topupCard(balance, msg, ServiceID, Status);
                    }
                }
            }catch(Exception ex)
            {
                NLogManager.PublishException(ex);
            }
           
        }

        public void GunEffectJackpot(EffectJackpot data)
        {
            ConnectionHandler.Instance.HubContext.Clients.All.effectJackpotAll(data);
        }
        public string Get(string uri, string Method = "GET")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = Method;
            request.ContentLength = 0;
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public class CommonModel
    {
        public int Name { get; set; }
        public double Value { get; set; }
    }
}