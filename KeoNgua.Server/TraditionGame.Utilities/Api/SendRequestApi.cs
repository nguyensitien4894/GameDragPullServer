using System;
using System.Configuration;

namespace TraditionGame.Utilities.Api
{
    public class SendRequestApi
    {
        private static readonly string URL = ConfigurationManager.AppSettings["PortalUrl"].ToString();

        public static bool EffectJackpotAll(string nickName, long jackpotValue, int betValue, string gameName, int serviceId, string uri)
        {
            try
            {
                var api = new ApiUtil<bool>();
                api.ApiAddress = URL;
                api.URI = uri;
                var res = api.Send(new { NickName = nickName, JackpotValue = jackpotValue, BetValue = betValue, GameName = gameName, ServiceID = serviceId });
                return res;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }
    }
}