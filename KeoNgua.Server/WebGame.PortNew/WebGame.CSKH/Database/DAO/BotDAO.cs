using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Bot;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class BotDAO
    {
        private static readonly Lazy<BotDAO> _instance = new Lazy<BotDAO>(() => new BotDAO());

        public static BotDAO Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Danh sách các bot theo khung giờ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<BotModel> GetBotSpecialTimeSetList(TimeSetModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_Botname", model.BotName);
                pars[1] = new SqlParameter("@_StartTime", model.StartTime);
                pars[2] = new SqlParameter("@_FinishTime", model.FinishTime);
                pars[3] = new SqlParameter("@_CurrentPage", currentPage);
                pars[4] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = db.GetListSP<BotModel>("SP_BotSpecial_TimeSet_GetList", pars);
                totalRecord = Convert.ToInt32(pars[5].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<BotModel> GetPermanentList(BotModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_StartTime", model.StartTime);
                pars[1] = new SqlParameter("@_FinishTime", model.FinishTime);
                pars[2] = new SqlParameter("@_BotType", model.BotType);
                pars[3] = new SqlParameter("@_CurrentPage", currentPage);
                pars[4] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = db.GetListSP<BotModel>("SP_Permanent_GetList", pars);
                totalRecord = Convert.ToInt32(pars[5].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public int PermanentUpdate(BotModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_BotGroupID", model.BotGroupId);
                pars[1] = new SqlParameter("@_BotsPlayMin", model.BotsPlayMin);
                pars[2] = new SqlParameter("@_BotsPlayMax", model.BotsPlayMax);
                pars[3] = new SqlParameter("@_PermanentBots", model.PermanentBots);
                pars[4] = new SqlParameter("@_BotType", model.BotType);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Permanent_Update", pars);
                return Int32.Parse(pars[5].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public List<TimeSetModel> GetTimeSetList(TimeSetModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_StartTime", model.StartTime);
                pars[1] = new SqlParameter("@_FinishTime", model.FinishTime);
                pars[2] = new SqlParameter("@_CurrentPage", currentPage);
                pars[3] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = db.GetListSP<TimeSetModel>("SP_Timeset_GetList", pars);
                totalRecord = Convert.ToInt32(pars[4].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public int TimeSetUpdate(TimeSetModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_TimeSetId", model.TimeSetId);
                pars[1] = new SqlParameter("@_StartTime", model.StartTime);
                pars[2] = new SqlParameter("@_FinishTime", model.FinishTime);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_TimeSet_Update", pars);
                return Int32.Parse(pars[3].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public List<BotInfo> GetListBot(string botname, int type)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_BotType", type);
                pars[1] = new SqlParameter("@_BotName", botname);
                var lstRs = db.GetListSP<BotInfo>("SP_Bot_List", pars);
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

        public int BotInjectMoney(long accountid, long amount)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", accountid);
                pars[1] = new SqlParameter("@_LimitMoney", amount);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Bot_Inject_Money", pars);
                return Int32.Parse(pars[2].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        /// <param name="typeFund">1: Tài xỉu, 2: Slot</param>
        public int AllInjectMoney(int typeFund, long amount)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_TypeFund", typeFund);
                pars[1] = new SqlParameter("@_Amount", amount);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.EmulatorConn);
                db.ExecuteNonQuerySP("SP_Inject_Money", pars);
                return ConvertUtil.ToInt(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int SpecialBotInjecMoney(long amount)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Amount", amount);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_SpecialBot_Inject_Money", pars);
                return Int32.Parse(pars[1].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<BotBet> GetTimelineSpecialList(BotModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceLogConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_StartDate", model.StartDate);
                pars[1] = new SqlParameter("@_EndDate", model.EndDate);
                pars[2] = new SqlParameter("@_ByResult", model.ByResult);
                pars[3] = new SqlParameter("@_CurrentPage", currentPage);
                pars[4] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var lstRs = db.GetListSP<BotBet>("SP_Timeline_BotSpecial", pars);
                totalRecord = Convert.ToInt32(pars[5].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int BotTransferToBot(long remitterId, long receiverId, long amount, string note, out long transId, out long remainWallet)
        {
            DBHelper db = null;
            transId = 0;
            remainWallet = 0;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_RemitterId", remitterId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Bot_Transfer_To_Bot", pars);
                int response = ConvertUtil.ToInt(pars[6].Value);
                if (response == 1)
                {
                    transId = ConvertUtil.ToLong(pars[4].Value);
                    remainWallet = ConvertUtil.ToLong(pars[5].Value);
                }

                return response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }

            return -99;
        }

        public List<BotFund> GetListBotFundSet()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.EmulatorConn);
                var lstRs = db.GetListSP<BotFund>("SP_BotFundSets_List");
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
    }
}