using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Models.SeDice;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class SeDiceDAO
    {
        private static readonly Lazy<SeDiceDAO> _instance = new Lazy<SeDiceDAO>(() => new SeDiceDAO());

        public static SeDiceDAO Instance
        {
            get { return _instance.Value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SeDiceEventModel> GetEventList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                return db.GetListSP<SeDiceEventModel>("SP_EventConfig_GetList");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
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
        public List<SeDiceSoiCau> GetSoiCau(int top)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_TopCount", top);

                db = new DBHelper(Config.LuckyDiceConn);
                var lstRs = db.GetListSP<SeDiceSoiCau>("SP_GetSoiCau", pars);
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
        public SeDiceEventModel GetEventInfo(long eventID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_EventID", eventID);
                var list = db.GetListSP<SeDiceEventModel>("SP_EventConfig_GetInfo", pars);
                if (list != null && list.Any()) return list.FirstOrDefault();
                return new SeDiceEventModel();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new SeDiceEventModel();
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public int SaveEvent(SeDiceEventModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_EventID", model.EventID);
                pars[1] = new SqlParameter("@_BetValueMin", model.BetValueMin);
                pars[2] = new SqlParameter("@_QuantityCordWin", model.QuantityCordWin);
                pars[3] = new SqlParameter("@_QuantityCordLost", model.QuantityCordLost);
                pars[4] = new SqlParameter("@_PrizeValueMin", model.PrizeValueMin);
                pars[5] = new SqlParameter("@_PrizeValueMax", model.PrizeValueMax);
                pars[6] = new SqlParameter("@_QuantityAwardCordWinInit", model.QuantityAwardCordWinInit);
                pars[7] = new SqlParameter("@_QuantityAwardCordLostInit", model.QuantityAwardCordLostInit);
                pars[8] = new SqlParameter("@_StartEventTimes", model.StartEventTimes);
                pars[9] = new SqlParameter("@_EndEventTimes", model.EndEventTimes);
                pars[10] = new SqlParameter("@_EventDays", model.EventDays);
                pars[11] = new SqlParameter("@_Hours", model.Hours);
                pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_EventConfig_Save", pars);
                return Int32.Parse(pars[12].Value.ToString());
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

        public int DeleteEvent(SeDiceEventModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_EventID", model.EventID);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_EventConfig_Delete", pars);
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
                {
                    db.Close();
                }
            }
        }

        public List<SeDiceRaceTopModel> GetRaceTopList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                return db.GetListSP<SeDiceRaceTopModel>("SP_RaceTopConfig_GetList");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
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

        public SeDiceRaceTopModel GetRaceTopInfo(long raceTopID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_RaceTopID", raceTopID);
                var list = db.GetListSP<SeDiceRaceTopModel>("SP_RaceTopConfig_GetInfo", pars);
                if (list != null && list.Any()) return list.FirstOrDefault();
                return new SeDiceRaceTopModel();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new SeDiceRaceTopModel();
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public List<Refunds> GetRefundsInfo(long uid,long session)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", uid);
                pars[1] = new SqlParameter("@_SessionID", session);
                var list = db.GetListSP<Refunds>("SP_GetRefundsInfo", pars);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
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

        public int SaveRaceTop(SeDiceRaceTopModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_RaceTopID", model.RaceTopID);
                pars[1] = new SqlParameter("@_Quantity", model.Quantity);
                pars[2] = new SqlParameter("@_PrizeValue", model.PrizeValue);
                pars[3] = new SqlParameter("@_Description", model.Description);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_RaceTopConfig_Save", pars);
                return Int32.Parse(pars[4].Value.ToString());
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

        public int DeleteRaceTop(SeDiceRaceTopModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_RaceTopID", model.RaceTopID);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_RaceTopConfig_Delete", pars);
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
                {
                    db.Close();
                }
            }
        }

        public List<SeDiceReportEventModel> GetEventAwardList(SeDiceReportEventModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceLogConn);
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_EventID", model.EventID);
                pars[1] = new SqlParameter("@_RaceTopID", model.RaceTopID);
                pars[2] = new SqlParameter("@_Username", model.Username);
                pars[3] = new SqlParameter("@_CordWinOrLost", model.CordWinOrLost);
                pars[4] = new SqlParameter("@_StartDate", model.StartDate);
                pars[5] = new SqlParameter("@_EndDate", model.EndDate);
                pars[6] = new SqlParameter("@_CurrentPage", currentPage);
                pars[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = db.GetListSP<SeDiceReportEventModel>("SP_EventAward_GetList", pars);
                totalRecord = Convert.ToInt32(pars[8].Value);
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
        public List<SeDiceUserBetModel> GetUsersSession(long SessionId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_SessionId", SessionId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                return db.GetListSP<SeDiceUserBetModel>("SP_GetUsersSession", pars);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new List<SeDiceUserBetModel>();
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }
    }
}