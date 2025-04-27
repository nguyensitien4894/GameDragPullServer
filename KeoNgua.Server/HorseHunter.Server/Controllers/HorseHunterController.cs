using HorseHunter.Server.DataAccess.DTO;
using HorseHunter.Server.DataAccess.Factory;
using MsWebGame.RedisCache.Cache;
using System;
using System.Collections.Generic;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Session;

namespace HorseHunter.Server.Controllers
{
    public class HorseHunterController : ApiController
    {
        [HttpGet]
        [ActionName("GetHistory")]
        public List<History> GetHistory(int top = 50)
        {
            try
            {
                long accountId = AccountSession.AccountID;
                if (accountId < 1)
                    return null;

                var lstRs = AbstractFactory.Instance().CreateHorseHunterDao().GetHistory(accountId, top);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [HttpGet]
        [ActionName("GetBigWinner")]
        public List<BigWinner> GetBigWinner(int type = 1, int top = 50)
        {
            try
            {
                //1:Bigwinner; 2:Jackpot
                //handler cache data
                List<BigWinner> lstRs = null;
                string keyCa = CachingHandler.Instance.GeneralRedisKey("HorseHunter", type == 1 ? "BigWinner" : "Jackpot");
                lstRs = CachingHandler.Instance.GetListCache<BigWinner>(keyCa);
                if (lstRs != null)
                    return lstRs;

                lstRs = AbstractFactory.Instance().CreateHorseHunterDao().GetBigWinner(type, top);
                if (lstRs == null)
                    lstRs = new List<BigWinner>();

                int timeCa = type == 1 ? 120 : 60;
                CachingHandler.Instance.AddListCache<BigWinner>(keyCa, lstRs, timeCa);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }
    }
}
