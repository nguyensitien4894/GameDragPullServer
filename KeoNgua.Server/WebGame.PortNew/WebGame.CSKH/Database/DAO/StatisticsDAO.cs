using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;
using System.Data;
using System.Linq;

namespace MsWebGame.CSKH.Database.DAO
{
    public class StatisticsDAO
    {
        private static readonly Lazy<StatisticsDAO> _instance = new Lazy<StatisticsDAO>(() => new StatisticsDAO());

        public static StatisticsDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<NruDeviceStatistics> NruDeviceList(int ServiceID, DateTime? FromDate, DateTime? ToDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[0].Value = FromDate ?? (object)DBNull.Value;
                param[1] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[1].Value = ToDate ?? (object)DBNull.Value;
                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = ServiceID;
               

                var _lstNruDeviceStatistics = db.GetListSP<NruDeviceStatistics>("SP_NruDevice_List", param.ToArray());
                return _lstNruDeviceStatistics;
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

        public List<PageTrackingInfo> PageTrackingInfoStatistic(int? AppType, int ServiceID, DateTime? FromDate, DateTime? ToDate, string UrlPage)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[0].Value = FromDate;
                param[1] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[1].Value = ToDate??(object)DBNull.Value;
                param[2] = new SqlParameter("@_UrlPage", SqlDbType.VarChar);
                param[2].Size = 250;
                param[2].Value = UrlPage?? (object)DBNull.Value;
                param[3] = new SqlParameter("@_AppType", SqlDbType.Int);
                param[3].Value = AppType ?? (object)DBNull.Value;
                param[4] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[4].Value = ServiceID;
        
                var _lstPageTrackingInfo = db.GetListSP<PageTrackingInfo>("SP_PageTrackingInfo_Statistic", param.ToArray());
                return _lstPageTrackingInfo;
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


        public List<UserTrackingPage> UserTrackingPageStatistic(int ServiceID, DateTime? FromDate, DateTime? ToDate, string UrlPage,string UrlPath,string UtmMedium,string UtmSource,string UtmCampaign,string UtmContent,int ? LoginType)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[10];
                param[0] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[0].Value = FromDate??(object)DBNull.Value;
                param[1] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[1].Value = ToDate ?? (object)DBNull.Value;
                param[2] = new SqlParameter("@_UrlPage", SqlDbType.VarChar);
                param[2].Size = 250;
                param[2].Value = UrlPage??(object)DBNull.Value;
                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;
                param[4] = new SqlParameter("@_UrlPath", SqlDbType.VarChar);
                param[4].Size = 250;
                param[4].Value = UrlPath ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_UtmMedium", SqlDbType.VarChar);
                param[5].Size = 250;
                param[5].Value = UtmMedium ?? (object)DBNull.Value;
                param[6] = new SqlParameter("@_UtmSource", SqlDbType.NVarChar);
                param[6].Size = 250;
                param[6].Value = UtmSource ?? (object)DBNull.Value;
                param[7] = new SqlParameter("@_UtmCampaign", SqlDbType.NVarChar);
                param[7].Size = 250;
                param[7].Value = UtmCampaign ?? (object)DBNull.Value;
                param[8] = new SqlParameter("@_UtmContent", SqlDbType.NVarChar);
                param[8].Size = 250;
                param[8].Value = UtmContent ?? (object)DBNull.Value;
                param[9] = new SqlParameter("@_LoginType", SqlDbType.Int);
                param[9].Value = LoginType ?? (object)DBNull.Value;
                
                var _lstUserTrackingPage = db.GetListSP<UserTrackingPage>("SP_UserTrackingPage_Statistic", param.ToArray());
                return _lstUserTrackingPage;
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


        public List<TrackingJackpot> GetTrackingJackpot(int gameid, DateTime? from, DateTime? to, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_GameID", gameid);
                pars[1] = new SqlParameter("@_FromDate", from);
                pars[2] = new SqlParameter("@_ToDate", to);
                pars[3] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<TrackingJackpot>("SP_Tracking_Jackpot", pars);
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