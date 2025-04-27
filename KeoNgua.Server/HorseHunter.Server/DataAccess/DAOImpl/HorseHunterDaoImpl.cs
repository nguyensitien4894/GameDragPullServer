using System;
using System.Collections.Generic;
using HorseHunter.Server.DataAccess.DAO;
using HorseHunter.Server.DataAccess.DTO;
using TraditionGame.Utilities;
using System.Data.SqlClient;
using System.Data;
using HorseHunter.Server.Handlers;
using MsWebGame.RedisCache.Cache;

namespace HorseHunter.Server.DataAccess.DAOImpl
{
    public class HorseHunterDaoImpl : IHorseHunterDao
    {
        public RoomFunds GetListRoomFunds(int id)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.HorseHunterConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_RoomID", id);
                var lstRs = db.GetInstanceSP<RoomFunds>("SP_RoomFunds_GetList_New", pars);
                if (lstRs != null)
                {
                    NLogManager.LogMessage("GetDb: " + lstRs.JackpotFund);
                }
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public AccountInfo GetAccountInfo(long accountId, string username, int roomId)
        {
            DBHelper db = null;
            try
            {
                AccountRooms accountRooms = GetAccountRoomsRedis((int)accountId, roomId);
                if (accountRooms == null)
                {
                    accountRooms = new AccountRooms();
                    accountRooms.AccountID = (int)accountId;
                    accountRooms.Username = username;
                    accountRooms.RoomID = roomId;
                }
                SetAccountRooms(accountRooms, (int)accountId, roomId);
                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_Username", username);
                pars[2] = new SqlParameter("@_RoomID", roomId);
                pars[3] = new SqlParameter("@_BetValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_FreeSpins", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_EventFreeSpins", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Jackpot", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_LastLineData", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_LastPrizeValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[9] = new SqlParameter("@_SessionFreeSpins", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[10] = new SqlParameter("@_PrizeValueFreeSpins", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[11] = new SqlParameter("@_Balance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.HorseHunterConn);
                db.ExecuteNonQuerySP("SP_Accounts_GetAccountInfo", pars);

                int res = ConvertUtil.ToInt(pars[12].Value);
                if (res < 0)
                    return new AccountInfo() { Response = res };

                int efreespin = accountRooms.EventFreeSpins;
                long jackpot = ConvertUtil.ToLong(pars[6].Value);
                long balance = ConvertUtil.ToLong(pars[11].Value);

                AccountInfo acc = new AccountInfo(efreespin, jackpot, balance, res);
                return acc;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new AccountInfo() { Response = (int)ErrorCode.Exception };
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public SpinData Spin(long accountId, string username, int roomId, string linesData, int sourceId, int merchantId, int serviceId, string clientIp, long prizein, long jacpotin, ref long prizeout, ref long jackpotout)
        {
            DBHelper db = null;
            try
            {
                AccountRooms accountRooms = GetAccountRoomsRedis((int)accountId, roomId);
                var pars = new SqlParameter[41];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_Username", username);
                pars[2] = new SqlParameter("@_RoomID", roomId);
                pars[3] = new SqlParameter("@_LineData", linesData);
                pars[4] = new SqlParameter("@_SourceID", sourceId);
                pars[5] = new SqlParameter("@_MerchantID", merchantId);
                pars[6] = new SqlParameter("@_ServiceID", serviceId);
                pars[7] = new SqlParameter("@_ClientIP", clientIp);
                pars[8] = new SqlParameter("@_SpinID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[9] = new SqlParameter("@_SlotData", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                pars[10] = new SqlParameter("@_PrizeData", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                pars[11] = new SqlParameter("@_IsJackpot", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                pars[12] = new SqlParameter("@_IsEventWinJackpot", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                pars[13] = new SqlParameter("@_Jackpot", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[14] = new SqlParameter("@_TotalJackpotValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[15] = new SqlParameter("@_PositionData", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                pars[16] = new SqlParameter("@_FreeSpins", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[17] = new SqlParameter("@_EventFreeSpins", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[18] = new SqlParameter("@_TotalBetValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[19] = new SqlParameter("@_PaylinePrizeValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[20] = new SqlParameter("@_PrizeFund", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[21] = new SqlParameter("@_OrgBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[22] = new SqlParameter("@_Balance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[23] = new SqlParameter("@_TotalJackpot", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                pars[24] = new SqlParameter("@_Description", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                pars[25] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[26] = new SqlParameter("@_PrizeFundIn", prizein);
                pars[27] = new SqlParameter("@_JackpotFundIn", jacpotin);
                pars[28] = new SqlParameter("@_PrizeFundOut", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[29] = new SqlParameter("@_JackPotFundOut", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[30] = new SqlParameter("@_FreeSpinsIn", accountRooms.FreeSpins);
                pars[31] = new SqlParameter("@_EventFreeSpinsIn", accountRooms.EventFreeSpins);
                pars[32] = new SqlParameter("@_LastLineDataIn", accountRooms.LastLineData);
                pars[33] = new SqlParameter("@_TotalFreeSpinsIn", accountRooms.TotalFreeSpins);
                pars[34] = new SqlParameter("@_PrizeValueFreeSpinsIn", accountRooms.PrizeValueFreeSpins);
                pars[35] = new SqlParameter("@_FreeSpinsOut", SqlDbType.Int){ Direction = ParameterDirection.Output };
                pars[36] = new SqlParameter("@_TotalFreeSpinsOut", SqlDbType.Int){ Direction = ParameterDirection.Output };
                pars[37] = new SqlParameter("@_EventFreeSpinsOut", SqlDbType.Int){ Direction = ParameterDirection.Output };
                pars[38] = new SqlParameter("@_LastLineDataOut", SqlDbType.VarChar,100){ Direction = ParameterDirection.Output };
                pars[39] = new SqlParameter("@_LastPrizeValueOut", SqlDbType.BigInt){ Direction = ParameterDirection.Output };
                pars[40] = new SqlParameter("@_PrizeValueFreeSpinsOut", SqlDbType.Int){ Direction = ParameterDirection.Output };
                db = new DBHelper(Config.HorseHunterConn);
                db.ExecuteNonQuerySP("SP_Spins_CreateTransaction", pars);

                int res = ConvertUtil.ToInt(pars[25].Value);
                if (res < 0)
                    return new SpinData() { Response = res };
                accountRooms.FreeSpins = ConvertUtil.ToInt(pars[35].Value);
                accountRooms.TotalFreeSpins = ConvertUtil.ToInt(pars[36].Value);
                accountRooms.EventFreeSpins = ConvertUtil.ToInt(pars[37].Value);
                accountRooms.LastLineData = ConvertUtil.ToString(pars[38].Value);
                accountRooms.LastPrizeValue = ConvertUtil.ToLong(pars[39].Value);
                accountRooms.PrizeValueFreeSpins = ConvertUtil.ToInt(pars[40].Value);
                SetAccountRooms(accountRooms,(int)accountId,roomId);
                long spinId = ConvertUtil.ToLong(pars[8].Value);
                string slotData = ConvertUtil.ToString(pars[9].Value);
                string prizeData = ConvertUtil.ToString(pars[10].Value);
                bool isJackpot = ConvertUtil.ToBool(pars[11].Value);
                bool isjpevent = ConvertUtil.ToBool(pars[12].Value);
                long jackpot = ConvertUtil.ToLong(pars[13].Value);
                long totalJackpot = ConvertUtil.ToLong(pars[14].Value);
                string posData = ConvertUtil.ToString(pars[15].Value);
                int freespin = ConvertUtil.ToInt(pars[16].Value);
                int efreespin = ConvertUtil.ToInt(pars[17].Value);
                int totalBet = ConvertUtil.ToInt(pars[18].Value);
                long payline = ConvertUtil.ToLong(pars[19].Value);
                long orgbalance = ConvertUtil.ToLong(pars[21].Value);
                long balance = ConvertUtil.ToLong(pars[22].Value);
                int jpNum = ConvertUtil.ToInt(pars[23].Value);
                string des = ConvertUtil.ToString(pars[24].Value);
                prizeout = ConvertUtil.ToLong(pars[28].Value);
                jackpotout = ConvertUtil.ToLong(pars[29].Value);
                SpinData spinData = new SpinData(spinId, linesData, totalBet, slotData, prizeData, posData, isJackpot, isjpevent,
                    jpNum, jackpot, totalJackpot, payline, orgbalance, balance, des, freespin, efreespin, res);
                return spinData;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new SpinData() { Response = (int)ErrorCode.Exception };
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<JackpotInfo> GetJackpot()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.HorseHunterConn);
                var lstRs = db.GetListSP<JackpotInfo>("SP_RoomFunds_GetList");
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<History> GetHistory(long accountId, int top)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_TopCount", top);

                db = new DBHelper(Config.HorseHunterLogConn);
                var lstRs = db.GetListSP<History>("SP_AccountHistory_GetPaged", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<BigWinner> GetBigWinner(int type, int top)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_TopCount", top);
                pars[1] = new SqlParameter("@_PrizeType", type);

                db = new DBHelper(Config.HorseHunterLogConn);
                var lstRs = db.GetListSP<BigWinner>("SP_BigWinnerHistory_Get", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int InsertHistory(int serviceId, int deviceId, long spinId, long accountId, string username, int roomId,
            int totalLine, string lines, int betValue, bool isjp, bool iseventjp, int totalBet, long totalPrize,
            long totalJpVal, long orgBalance, long balance, string slotsData, string prizesData, string des, long jackpot)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[21];
                pars[0] = new SqlParameter("@_ServiceID", serviceId);
                pars[1] = new SqlParameter("@_DeviceID", deviceId);
                pars[2] = new SqlParameter("@_SpinID", spinId);
                pars[3] = new SqlParameter("@_AccountID", accountId);
                pars[4] = new SqlParameter("@_UserName", username);
                pars[5] = new SqlParameter("@_RoomID", roomId);
                pars[6] = new SqlParameter("@_TotalLines", totalLine);
                pars[7] = new SqlParameter("@_LineData", lines);
                pars[8] = new SqlParameter("@_BetValue", betValue);
                pars[9] = new SqlParameter("@_IsJackpot", isjp);
                pars[10] = new SqlParameter("@_IsEventWinJackpot", iseventjp);
                pars[11] = new SqlParameter("@_TotalBetValue", totalBet);
                pars[12] = new SqlParameter("@_TotalPrizeValue", totalPrize);
                pars[13] = new SqlParameter("@_TotalJackPotValue", totalJpVal);
                pars[14] = new SqlParameter("@_OrgBalance", orgBalance);
                pars[15] = new SqlParameter("@_Balance", balance);
                pars[16] = new SqlParameter("@_SlotsData", slotsData);
                pars[17] = new SqlParameter("@_PrizesData", prizesData);
                pars[18] = new SqlParameter("@_Description", des);
                pars[19] = new SqlParameter("@_Jackpot", jackpot);
                pars[20] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.HorseHunterLogConn);
                db.ExecuteNonQuerySP("SP_SlotMachineHistory_Create", pars);
                return ConvertUtil.ToInt(pars[20].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return (int)ErrorCode.Exception;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public AccountRooms GetAccountRooms(int accountid, int roomid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", accountid);
                pars[1] = new SqlParameter("@_RoomID", roomid);
                db = new DBHelper(Config.ThuyCungConn);
                var accountRooms = db.GetInstanceSP<AccountRooms>("SP_AccountRooms", pars);
                return accountRooms;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        private AccountRooms GetAccountRoomsRedis(int accountID, int roomId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("HorseHunter", "AccountRooms" + "." + roomId + "." + accountID);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            AccountRooms room = _cachePvd.Get<AccountRooms>(keyHu);
            if (room == null)
            {
                room = GetAccountRooms(accountID, roomId);
            }
            if (room != null)
            {
                if (room.LastLineData == null)
                {
                    room.LastLineData = "";
                }
            }
            return room;
        }
        private void SetAccountRooms(AccountRooms accountRooms, int accountID, int roomId)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("HorseHunter", "AccountRooms" + "." + roomId + "." + accountID);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.SetSecond(keyHu, accountRooms, 86400);
        }
    }
}