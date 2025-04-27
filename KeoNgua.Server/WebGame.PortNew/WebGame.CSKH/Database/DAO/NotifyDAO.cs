using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class NotifyDAO
    {
        private static readonly Lazy<NotifyDAO> _instance = new Lazy<NotifyDAO>(() => new NotifyDAO());

        public static NotifyDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<PortalNotify> GetPortalGameNotifyList(bool? status, DateTime? fromDate, DateTime? toDate,int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_Status", status);
                pars[1] = new SqlParameter("@_FromDate", fromDate);
                pars[2] = new SqlParameter("@_ToDate", toDate);
                pars[3] = new SqlParameter("@_ServiceID", serviceid);
                var lstRs = db.GetListSP<PortalNotify>("SP_PortalGameNotify_List", pars);
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

        public int CreatePortalGameNotify(string content,int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Content", content);
                pars[1] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ServiceID", serviceid);
                
                db.ExecuteNonQuerySP("SP_PortalGameNotify_Create", pars);
                return ConvertUtil.ToInt(pars[1].Value);
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

        public int UpdatePortalGameNotify(PortalNotify input)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_NotifyID", input.ID);
                pars[1] = new SqlParameter("@_Content", input.Content);
                pars[2] = new SqlParameter("@_Status", input.Status);
                pars[3] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_ServiceID", input.ServiceID);
                db.ExecuteNonQuerySP("SP_PortalGameNotify_Update", pars);
                return ConvertUtil.ToInt(pars[3].Value);
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
    }
}