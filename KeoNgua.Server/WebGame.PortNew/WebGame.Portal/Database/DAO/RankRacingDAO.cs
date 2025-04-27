using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.Portal.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class RankRacingDAO
    {
        private static readonly Lazy<RankRacingDAO> _instance = new Lazy<RankRacingDAO>(() => new RankRacingDAO());

        public static RankRacingDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<RankRacingInfo> GetRankRacing(DateTime? from, DateTime? to, int start, int quantity, int? prizeId, int? prizeVal, bool isClose)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_FromDate", from);
                pars[1] = new SqlParameter("@_ToDate", to);
                pars[2] = new SqlParameter("@_StartPos", start);
                pars[3] = new SqlParameter("@_Quantity", quantity);
                pars[4] = new SqlParameter("@_PrizeID", prizeId);
                pars[5] = new SqlParameter("@_PrizeValue", prizeVal);
                pars[6] = new SqlParameter("@_IsClosed", isClose);
                var lstRs = db.GetListSP<RankRacingInfo>("SP_Game_Alpha_List", pars);
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

        public List<TopVpToDate> GetTopVpToDate(DateTime? from, DateTime? to, int start, int quantity, int? prizeId, long? prizeVal, bool isClosed, DateTime raceDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_FromDate", from);
                pars[1] = new SqlParameter("@_ToDate", to);
                pars[2] = new SqlParameter("@_StartPos", start);
                pars[3] = new SqlParameter("@_Quantity", quantity);
                pars[4] = new SqlParameter("@_PrizeID", prizeId);
                pars[5] = new SqlParameter("@_PrizeValue", prizeVal);
                pars[6] = new SqlParameter("@_IsClosed", isClosed);
                pars[7] = new SqlParameter("@_RaceDate", raceDate);
                var lstRs = db.GetListSP<TopVpToDate>("SP_Game_Beta_List", pars);
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

        public List<TopVpToDate> GetBetaForecast(DateTime? from, DateTime? to, int start, int quantity, long userId, out int userPos)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_FromDate", from);
                pars[1] = new SqlParameter("@_ToDate", to);
                pars[2] = new SqlParameter("@_StartPos", start);
                pars[3] = new SqlParameter("@_Quantity", quantity);
                pars[4] = new SqlParameter("@_UserID", userId);
                pars[5] = new SqlParameter("@_UserPosition", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var lstRs = db.GetListSP<TopVpToDate>("SP_Game_Beta_Forecast", pars);
                userPos = ConvertUtil.ToInt(pars[5].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                userPos = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<TopBalance> GetGameTopBalance(int top)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_TopCount", top);
                var lstRs = db.GetListSP<TopBalance>("SP_AccountProfit_GetList", pars);
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

        public List<TopVpToDate> GetVpTopMonth(int quantity)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_Quantity", quantity);
                var lstRs = db.GetListSP<TopVpToDate>("SP_Game_VP_GetTopMonth", pars);
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