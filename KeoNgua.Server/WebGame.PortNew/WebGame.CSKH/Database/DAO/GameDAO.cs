using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;
using MsWebGame.CSKH.Models.Warnings;
using System.Linq;

namespace MsWebGame.CSKH.Database.DAO
{
    public class GameDAO
    {
        private static readonly Lazy<GameDAO> _instance = new Lazy<GameDAO>(() => new GameDAO());

        public static GameDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<Services> GetSerivces()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var _lstGetList = db.GetListSP<Services>("SP_Services_List");
                return _lstGetList;

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

            return null;
        }

        public List<Warning> WarningUserRecharge(int WarningType, int VPLimit, int QuotaDay, long LimitAmount, int CurrentPage, int RecordPerpage, out long TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];

                param[0] = new SqlParameter("@_WarningType", SqlDbType.Int);
                param[0].Value = WarningType;
                param[1] = new SqlParameter("@_LimitAmount", SqlDbType.BigInt);
                param[1].Value = LimitAmount;
                param[2] = new SqlParameter("@_VPLimit", SqlDbType.Int);
                param[2].Value = VPLimit;
                param[3] = new SqlParameter("@_QuotaDay", SqlDbType.Int);
                param[3].Value = QuotaDay;
                param[4] = new SqlParameter("@_CurrentPage", CurrentPage);
                param[5] = new SqlParameter("@_RecordPerpage", RecordPerpage);
                param[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var _lstWarningModel = db.GetListSP<Warning>("SP_Warning_User_Recharge", param.ToArray());
                TotalRecord = ConvertUtil.ToLong(param[6].Value);
                return _lstWarningModel;
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
            TotalRecord = 0;
            return null;
        }
        public List<GameIndex> GetGameIndexList(DateTime? CheckDate)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_CheckDate", SqlDbType.DateTime);
                pars[0].Value = CheckDate??(object)DBNull.Value;

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<GameIndex>("SP_Game_Index", pars);
                return lstRs;
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
            return null;
        }

        public int SetJackPot(JackPot model)
        {
            int Result = -1;
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_GameID", model.Gameid);
                pars[1] = new SqlParameter("@_RoomID", model.Roomid);
                pars[2] = new SqlParameter("@_DisplayName", model.Displayname);
                pars[3] = new SqlParameter("@_Result", SqlDbType.Int) { Direction = ParameterDirection.Output };


                db = new DBHelper(Config.BettingConn);
                var lstRs = db.ExecuteNonQuerySP("SP_Set_Jackpot", pars.ToArray());
                Result = ConvertUtil.ToInt(pars[3].Value);
                return Result;
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
            return Result;
        }

        public List<GameFunds> GameFundsList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<GameFunds>("SP_GameFunds_List");
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
           
            return null;
        }
        public List<CCUs> CCUList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<CCUs>("SP_Get_Game_CCU");
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }

            return null;
        }
    }
}