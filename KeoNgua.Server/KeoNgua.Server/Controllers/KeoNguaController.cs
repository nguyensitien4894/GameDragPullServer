using KeoNgua.Server.DataAccess.Dao;
using KeoNgua.Server.DataAccess.Dto;
using KeoNgua.Server.DataAccess.Factory;
using KeoNgua.Server.Handlers;
using KeoNgua.Server.Models;
using MsWebGame.RedisCache.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Session;

namespace KeoNgua.Server.Controllers
{
    public class KeoNguaController : ApiController
    {
        private readonly IKeoNguaDao _gameDao = AbstractFactory.Instance().CreateGameDao();
        
        [HttpGet]
        [ActionName("GetSoiCau")]
        public IHttpActionResult GetSoiCau()
        {
            try
            {
                //handler cache data
                List<SoiCau> lstRs = null;
                string keyCa = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "SoiCau");
                lstRs = CachingHandler.Instance.GetListCache<SoiCau>(keyCa);
                if (lstRs != null) return Ok(lstRs);

                lstRs = _gameDao.GetSoiCau();
                if (lstRs != null && lstRs.Any())
                    CachingHandler.Instance.AddListCache<SoiCau>(keyCa, lstRs, 30);
                return Ok(lstRs);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [HttpGet]
        [ActionName("GetHistory")]
        public List<History> GetHistory(int top = 50)
        {
            try
            {
                if (top >= 100) top = 100;
                long accountId = AccountSession.AccountID;
                if (accountId < 0) throw new Exception("AccountID not in token");

                var lstRs = _gameDao.GetHistory(accountId, top);
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
        public List<BigWinner> GetBigWinner()
        {
            try
            {
                //handler cache data
                List<BigWinner> lstRs = null;
                string keyCa = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "BigWinner");
                lstRs = CachingHandler.Instance.GetListCache<BigWinner>(keyCa);
                if (lstRs != null) return lstRs;

                lstRs = _gameDao.GetBigWinner();
                if (lstRs != null && lstRs.Any())
                    CachingHandler.Instance.AddListCache<BigWinner>(keyCa, lstRs, 30);

                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [HttpGet]
        [ActionName("GetPlayersNotInGame")]
        public List<GamePlayer> GetPlayersNotInGame()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                string nickName = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickName))
                    throw new Exception(string.Format("NotAuthen-Acc:{0}({1})", nickName, accountId));

                List<GamePlayer> pNotInGame = VipHandler.Instance.GetPlayersNotInGame(accountId);
                return pNotInGame;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        [HttpGet]
        [ActionName("Test")]
        public dynamic Test()
        {
            try
            {

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return null;
        }
    }
}
