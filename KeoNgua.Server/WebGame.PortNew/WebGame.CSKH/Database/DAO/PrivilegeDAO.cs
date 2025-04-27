using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Param;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class PrivilegeDAO
    {
        private static readonly Lazy<PrivilegeDAO> _instance = new Lazy<PrivilegeDAO>(() => new PrivilegeDAO());

        public static PrivilegeDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<Privilege> GetReportAccountVIP(DateTime? startDate, DateTime? endDate, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_StartDate", startDate);
                pars[1] = new SqlParameter("@_EndDate", endDate);
                pars[2] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<Privilege>("SP_Report_AccountVIP", pars);
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

        public List<PrivilegeLookup> GetAccountVipPoint(ParsLookup input, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_StartDate", input.startDate);
                pars[1] = new SqlParameter("@_EndDate", input.endDate);
                pars[2] = new SqlParameter("@_SearchType", input.searchType);
                pars[3] = new SqlParameter("@_Value", input.value);
                pars[4] = new SqlParameter("@_RankID", input.rankId);
                pars[5] = new SqlParameter("@_ServiceID", input.serviceId);
                pars[6] = new SqlParameter("@_CurrentPage", currentPage);
                pars[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<PrivilegeLookup>("SP_AccountVipPoint_GetList", pars);
                totalRecord = ConvertUtil.ToInt(pars[8].Value);
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

        public List<ReportVP> GetReportVP(int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[1];
         
                pars[0] = new SqlParameter("@_ServiceID", serviceid);
              

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<ReportVP>("SP_Report_VP", pars);

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
        public List<ReportVP> GetReportPU(int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[1];

                pars[0] = new SqlParameter("@_ServiceID", serviceid);


                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<ReportVP>("SP_Report_PU", pars);

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