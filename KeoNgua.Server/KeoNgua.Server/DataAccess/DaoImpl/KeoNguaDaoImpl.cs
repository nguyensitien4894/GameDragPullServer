using KeoNgua.Server.DataAccess.Dao;
using KeoNgua.Server.DataAccess.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.RedisCache.Cache;
using TraditionGame.Utilities;

namespace KeoNgua.Server.DataAccess.DaoImpl
{
    public class KeoNguaDaoImpl : IKeoNguaDao
    {
        public long CreateSession()
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_SessionID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.GamesConn);
                db.ExecuteNonQuerySP("SP_CreateSession", pars);
                int response = ConvertUtil.ToInt(pars[1].Value);
                if (response < 0) return response;

                return ConvertUtil.ToLong(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return (int)ErrorCode.Exception;
            }
            finally
            {
                if (db != null) db.Close();
            }
        }

        public int Bet(long sessionId, long accountId, int serviceId, int deviceId, int betSide, long betValue, out long balance)
        {
            DBHelper db = null;
            int response = -1;
            balance = 0;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_SessionID", sessionId);
                pars[1] = new SqlParameter("@_AccountID", accountId);
                pars[2] = new SqlParameter("@_ServiceID", serviceId);
                pars[3] = new SqlParameter("@_DeviceID", deviceId);
                pars[4] = new SqlParameter("@_GateID", betSide);
                pars[5] = new SqlParameter("@_Amount", betValue);
                pars[6] = new SqlParameter("@_Balance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_PrizeFundAdd", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db = new DBHelper(Config.GamesConn);
                db.ExecuteNonQuerySP("SP_Bet", pars);
                response = ConvertUtil.ToInt(pars[7].Value);
                if (response >= 0)
                {
                    balance = ConvertUtil.ToLong(pars[6].Value);
                    long prizeAdd = ConvertUtil.ToLong(pars[8].Value);
                    string keyHu = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "RoomFunds");
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();
                    long prizeFund = 0;
                    if (_cachePvd.Exists(keyHu))
                    {
                        prizeFund = _cachePvd.Get<long>(keyHu);

                    }
                    prizeFund += prizeAdd;
                    _cachePvd.Set(keyHu, prizeFund);
                }

                return response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                //NLogManager.LogMessage(string.Format("Bet:{0}({1}) - {2}-{3}-{4}", sessionId, accountId, betSide, betValue, response));
                if (db != null) db.Close();
            }
            return (int)ErrorCode.Exception;
        }

        public void CreateManualDice(long sessionId, out int dice1, out int dice2, out int dice3)
        {
            DBHelper db = null;
            dice1 = 0;
            dice2 = 0;
            dice3 = 0;
            try
            {
             
           
                if (dice1 == 0 || dice2 == 0 || dice3 == 0)
                {
                    string keyHu = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "RoomFunds");
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();
                    long prizeFund = 0;
                    if (_cachePvd.Exists(keyHu))
                    {
                        prizeFund = _cachePvd.Get<long>(keyHu);
                    }
                    var pars = new SqlParameter[5];
                    pars[0] = new SqlParameter("@_SessionID", sessionId);
                    pars[1] = new SqlParameter("@_FirstDice", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    pars[2] = new SqlParameter("@_SecondDice", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    pars[3] = new SqlParameter("@_ThirdDice", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    pars[4] = new SqlParameter("@_PrizeFundIn", prizeFund);
                    db = new DBHelper(Config.GamesConn);
                    db.ExecuteNonQuerySP("SP_ManualDice_Session", pars);
                    dice1 = ConvertUtil.ToInt(pars[1].Value);
                    dice2 = ConvertUtil.ToInt(pars[2].Value);
                    dice3 = ConvertUtil.ToInt(pars[3].Value);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                NLogManager.LogMessage(string.Format("CreateManualDice-SessionID:{0}: {1}-{2}-{3}", sessionId, dice1, dice2, dice3));
                if (db != null) db.Close();
            }
        }

        public int FinishSession(long sessionId, DiceResult result)
        {
            DBHelper db = null;
            int response = -1;
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_SessionID", sessionId);
                pars[1] = new SqlParameter("@_FirstDice", result.Dice1);
                pars[2] = new SqlParameter("@_SecondDice", result.Dice2);
                pars[3] = new SqlParameter("@_ThirdDice", result.Dice3);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_PrizeFundAdd", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.GamesConn);
                db.ExecuteNonQuerySP("SP_FinishSession", pars);
                response = ConvertUtil.ToInt(pars[4].Value);
                if (response > 0)
                {
                    long prizefunadd = ConvertUtil.ToLong(pars[5].Value);
                    string keyHu = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "RoomFunds");
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();
                    long prizeFund = 0;
                    if (_cachePvd.Exists(keyHu))
                    {
                        prizeFund = _cachePvd.Get<long>(keyHu);
                    }
                    prizeFund -= prizefunadd;
                    _cachePvd.Set(keyHu, prizeFund);
                    NLogManager.LogMessage("FinishSession.PrizeFund: "+ prizeFund.ToString("#,###")+ "- PrizefundSub: "+ prizefunadd.ToString("#,###"));
                }
                return response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                NLogManager.LogMessage(string.Format("FinishSession--SessionID:{0} - R:{1}", sessionId, response));
                if (db != null) db.Close();
            }

            return (int)ErrorCode.Exception;
        }

        public List<SessionResult> GetAwardSession(long sessionId)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_SessionID", sessionId);

                db = new DBHelper(Config.GamesConn);
                var lstRs = db.GetListSP<SessionResult>("SP_AwardSession", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null) db.Close();
            }
        }

        public List<SoiCau> GetSoiCau()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.GamesConn);
                var lstRs = db.GetListSP<SoiCau>("SP_SessionHistory");
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null) db.Close();
            }
        }

        public List<BigWinner> GetBigWinner()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.GamesConn);
                var lstRs = db.GetListSP<BigWinner>("SP_Rank");
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null) db.Close();
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

                db = new DBHelper(Config.GamesConn);
                var lstRs = db.GetListSP<History>("SP_TransactionHistory", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null) db.Close();
            }
        }

        public int BotBet(long sessionId, long accountId, int serviceId, int deviceId, int betSide, long amount)
        {
            DBHelper db = null;
            int response = -1;
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_SessionID", sessionId);
                pars[1] = new SqlParameter("@_AccountID", accountId);
                pars[2] = new SqlParameter("@_ServiceID", serviceId);
                pars[3] = new SqlParameter("@_DeviceID", deviceId);
                pars[4] = new SqlParameter("@_GateID", betSide);
                pars[5] = new SqlParameter("@_Amount", amount);
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.EmulatorConn);
                db.ExecuteNonQuerySP("SP_SquashCrab_Bot_Bet", pars);
                response = ConvertUtil.ToInt(pars[6].Value);
                return response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return (int)ErrorCode.Exception;
            }
            finally
            {
                //NLogManager.LogMessage(string.Format("BotBet:{0}({1}) - {2}-{3}-{4}", sessionId, accountId, betSide, amount, response));
                if (db != null) db.Close();
            }
        }

        public List<BotDB> GetListCardBot(int gameId, out string betValues)
        {
            DBHelper db = null;
            betValues = string.Empty;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_GameID", gameId);
                pars[1] = new SqlParameter("@_BetValues", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.EmulatorConn);
                var lstRs = db.GetListSP<BotDB>("SP_BotCardGame_GetList", pars);
                betValues = ConvertUtil.ToString(pars[1].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                //NLogManager.LogMessage(string.Format("GameID:{0} - {1}", gameId, betValues));
                if (db != null) db.Close();
            }
            return null;
        }
    }
}