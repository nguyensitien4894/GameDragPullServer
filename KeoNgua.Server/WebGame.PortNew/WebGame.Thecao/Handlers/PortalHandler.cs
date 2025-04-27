using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TraditionGame.Utilities;
using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Handlers
{
    public class PortalHandler
    {
        private readonly static Lazy<PortalHandler> _instance = new Lazy<PortalHandler>(() => new PortalHandler());
        private static Dictionary<int, int> _mapHourToFrameID;

        public static PortalHandler Instance
        {
            get { return _instance.Value; }
        }

        private PortalHandler()
        {

        }

        public void ReturnTopupCard(long accountId, long balance, string msg)
        {
            var lstConn = ConnectionHandler.Instance.GetConnections(accountId);
            NLogManager.LogMessage(string.Format("Send Hub :accountId: {0} | balance: {1} | msg: {2} | lstConn.Count: {3}",
                accountId, balance, msg, lstConn != null ? lstConn.Count : 0));
            if (lstConn != null && lstConn.Count > 0)
            {
                foreach (var conn in lstConn)
                {
                    ConnectionHandler.Instance.HubContext.Clients.Client(conn).topupCard(balance, msg);
                }
            }
        }

        public void GunEffectJackpot(EffectJackpot data)
        {
            ConnectionHandler.Instance.HubContext.Clients.All.effectJackpotAll(data);
        }
    }
}