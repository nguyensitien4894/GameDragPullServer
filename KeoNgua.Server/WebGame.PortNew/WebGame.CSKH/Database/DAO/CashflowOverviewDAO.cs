using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class CashflowOverviewDAO
    {
        private static readonly Lazy<CashflowOverviewDAO> _instance = new Lazy<CashflowOverviewDAO>(() => new CashflowOverviewDAO());

        public static CashflowOverviewDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<CashflowOverview> GetOtherList(DateTime? fromDate, DateTime? toDate, int serviceId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_FromDate", fromDate);
                pars[1] = new SqlParameter("@_ToDate", toDate);
                pars[2] = new SqlParameter("@_ServiceID", serviceId);
                var lstRs = db.GetListSP<CashflowOverview>("SP_Report_Overview_Other_List", pars);

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
                {
                    db.Close();
                }
            }
        }
        public List<CashflowOverview> GetList(DateTime ?fromDate,DateTime ?toDate,int serviceId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_FromDate", fromDate);
                pars[1] = new SqlParameter("@_ToDate", toDate);
                pars[2] = new SqlParameter("@_ServiceID", serviceId);
                var lstRs = db.GetListSP<CashflowOverview>("SP_Report_Overview_List", pars);
                
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
                {
                    db.Close();
                }
            }
        }
    }
}