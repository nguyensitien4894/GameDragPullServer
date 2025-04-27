using System;
using HorseHunter.Server.DataAccess.DAO;
using HorseHunter.Server.DataAccess.Factory;
using System.Collections.Generic;
using System.Configuration;
using TraditionGame.Utilities;
using HorseHunter.Server.DataAccess.DTO;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.IpAddress;
using System.Threading;
using System.Linq;
using MsWebGame.RedisCache.Cache;
using TraditionGame.Utilities.Api;

namespace HorseHunter.Server.Handlers
{
    public class HorseHunterHandler
    {
        private static readonly Lazy<HorseHunterHandler> _instance = new Lazy<HorseHunterHandler>(() => new HorseHunterHandler());
        private readonly IHorseHunterDao _minigameDao = AbstractFactory.Instance().CreateHorseHunterDao();
        private static Dictionary<int, int> _mapRoomValue;
        private static readonly int _serviceId = Int32.Parse(ConfigurationManager.AppSettings["ServiceID"]);

        public static HorseHunterHandler Instance
        {
            get { return _instance.Value; }
        }

        private HorseHunterHandler()
        {
            _mapRoomValue = new Dictionary<int, int>();
            string[] lstRoom = ConfigurationManager.AppSettings["EnableRoomValue"].Split(',');
            for (int i = 0; i < lstRoom.Length; i++)
            {
                if (_mapRoomValue.ContainsKey(i + 1))
                    _mapRoomValue.Remove(i + 1);

                _mapRoomValue.Add(i + 1, Convert.ToInt32(lstRoom[i]));
            }
        }

        public int GetRoomValue(int roomId)
        {
            int betValue = 0;
            _mapRoomValue.TryGetValue(roomId, out betValue);
            return betValue;
        }

        #region Play Now
        public int PlayNow(int roomId)
        {
            try
            {
                long accountId = AccountSession.AccountID;
                string nickname = AccountSession.AccountName;
                AccountInfo rs = _minigameDao.GetAccountInfo(accountId, nickname, roomId);
                string keyHu = CachingHandler.Instance.GeneralRedisKey("HorseHunter", "RoomFunds" + roomId);
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room != null)
                {
                    rs.Jackpot = room.JackpotFund;
                }
                if (rs.Response >= 0)
                    ReturnJoinGame(accountId, rs);

                NLogManager.LogMessage(string.Format("PN-Acc:{0}| Room:{1}| Res:{2}", accountId, roomId, rs.Response));
                return rs.Response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return (int)ErrorCode.Exception;
            }
        }

        private void ReturnJoinGame(long accountId, AccountInfo data)
        {
            var lstConn = ConnectionHandler.Instance.GetConnections(accountId);
            if (lstConn != null && lstConn.Any())
            {
                foreach (var conn in lstConn)
                {
                    ConnectionHandler.Instance.HubContext.Clients.Client(conn).joinGame(data);
                }
            }
        }
        #endregion

        #region Do Spin
        public int Spin(int roomId, string lines, int deviceId)
        {
            try
            {
                long accountId = AccountSession.AccountID;
                string nickname = AccountSession.AccountName;
                int sourceId = 1;
                int merchantId = 1;
                string clienIp = IPAddressHelper.GetClientIP();
                string keyHu = CachingHandler.Instance.GeneralRedisKey("HorseHunter", "RoomFunds" + roomId);
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                long PrizeFund = 0;
                long JackpotFund = 0;
                if (room != null)
                {
                    PrizeFund = room.PrizeFund;
                    JackpotFund = room.JackpotFund;

                }
                else
                {
                    RoomFunds roomfunds = _minigameDao.GetListRoomFunds(roomId);
                    if (roomfunds != null)
                    {
                        _cachePvd.Set<RoomFunds>(keyHu, roomfunds);
                        PrizeFund = roomfunds.PrizeFund;
                        JackpotFund = roomfunds.JackpotFund;
                    }
                    else
                    {
                        NLogManager.LogMessage("4-PrizeFund");
                        return -99;
                    }
                }
                
                long prizeout = 0;
                long jackpotout = 0;

                var spinData = _minigameDao.Spin(accountId, nickname, roomId, lines, sourceId, merchantId, _serviceId, clienIp, PrizeFund, JackpotFund, ref prizeout, ref jackpotout);
                if (spinData != null && spinData.Response >= 0)
                {
                    NLogManager.LogMessage("PrizeFund: " + PrizeFund.ToString("#,###") + "-JackpotFund: " + JackpotFund.ToString("#,###") + "-jackpotout: "+ jackpotout.ToString("#,###")+ "-prizeout: "+ prizeout.ToString("#,###"));
                    RoomFunds newroom = _cachePvd.Get<RoomFunds>(keyHu);
                    if (newroom != null)
                    {
                        newroom.PrizeFund = newroom.PrizeFund + (prizeout - PrizeFund);
                        if (jackpotout != 0)
                        {
                            newroom.JackpotFund = newroom.JackpotFund + jackpotout - JackpotFund;
                        }
                        newroom.RoomID = roomId;
                        _cachePvd.Set<RoomFunds>(keyHu, newroom);
                    }
                    ReturnSpin(accountId, spinData);
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        LogSpin(accountId, nickname, roomId, spinData, deviceId);
                    });

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        //Call gun effect jackpot all
                        if (spinData.IsJackpot)
                        {
                            string gamename = ConfigurationManager.AppSettings["GameName"].ToString();
                            EffectJackpotAll(nickname, spinData.TotalJackpotValue, GetRoomValue(roomId), gamename, _serviceId);
                        }
                    });
                }
                return spinData.Response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return (int)ErrorCode.Exception;
            }
        }

        private void ReturnSpin(long accountId, SpinData data)
        {
            var lstConn = ConnectionHandler.Instance.GetConnections(accountId);
            if (lstConn != null && lstConn.Any())
            {
                foreach (var conn in lstConn)
                {
                    ConnectionHandler.Instance.HubContext.Clients.Client(conn).resultSpin(data);
                }
            }
        }

        /// <summary>
        /// Call api effect jackpot in portal
        /// </summary>
        private void EffectJackpotAll(string nickName, long jackpotValue, int betValue, string gameName, int serviceId)
        {
            try
            {
                string uri = ConfigurationManager.AppSettings["ApiEffectJackpot"].ToString();
                var response = SendRequestApi.EffectJackpotAll(nickName, jackpotValue, betValue, gameName, serviceId, uri);
                NLogManager.LogMessage(string.Format("EffectJackpotAll- Res:{0}", response));
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
        #endregion

        private void LogSpin(long accountId, string nickname, int roomId, SpinData data, int deviceId)
        {
            try
            {
                long spinId = data.SpinID;
                if (spinId < 1)
                    return;

                string lines = data.LinesData;
                int totalLine = lines.Split(',').Length;
                int betValue = GetRoomValue(roomId);
                bool isjp = data.IsJackpot;
                bool iseventjp = data.IsJackpotEvent;
                int totalBet = data.TotalBet;
                long prizevalue = data.PaylinePrize;
                long totalJpVal = data.TotalJackpotValue;
                long jackpot = data.Jackpot;
                long orgbalance = data.OrgBalance;
                long balance = data.Balance;
                string slotsData = data.SlosDataStr;
                string prizesData = data.PrizeLinesStr;
                string des = data.Description;
                
                int res = _minigameDao.InsertHistory(_serviceId, deviceId, spinId, accountId, nickname, roomId, totalLine, lines,
                    betValue, isjp, iseventjp, totalBet, prizevalue, totalJpVal, orgbalance, balance, slotsData, prizesData, des, jackpot);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public void UpdateClientJackpot()
        {
            try
            {
                var lst = GetListJackpotCache();
                if (lst != null && lst.Count == 4)
                {
                    var strJp = ConvertJackpot(lst);
                    ConnectionHandler.Instance.UpdateJackpot("AllUserInHorseHunter", strJp);
                }
                else
                {
                    var lstJp = _minigameDao.GetJackpot();
                    if (lstJp != null && lstJp.Any())
                    {
                        var strJp = ConvertJackpot(lstJp);
                        ConnectionHandler.Instance.UpdateJackpot("AllUserInHorseHunter", strJp);
                    }
                }
                
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
        public List<JackpotInfo> GetListJackpotCache()
        {
            List<JackpotInfo> list = new List<JackpotInfo>();

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            for (int i = 1; i < 5; i++)
            {
                string keyHu = CachingHandler.Instance.GeneralRedisKey("HorseHunter", "RoomFunds" + i);
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room != null)
                {
                    list.Add(new JackpotInfo()
                    {
                        RoomID = i,
                        JackpotFund = room.JackpotFund
                    });
                }
            }
            return list;
        }
        public string ConvertJackpot(List<JackpotInfo> lstJp)
        {
            if (lstJp != null && lstJp.Any())
            {
                string strJp = "";
                for (int i = 0; i < lstJp.Count; i++)
                {
                    strJp += lstJp[i].JackpotFund.ToString() + "|";
                }
                strJp = StringUtil.RemoveLastStr(strJp);
                return strJp;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}