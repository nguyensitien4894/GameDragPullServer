using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TraditionGame.Utilities;
using MsWebGame.CSKH.Database.DTO;

namespace MsWebGame.CSKH.Database.DAO
{
    public class AnalyticsDAO
    {
        private static readonly Lazy<AnalyticsDAO> _instance = new Lazy<AnalyticsDAO>(() => new AnalyticsDAO());

        public static AnalyticsDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<CasoutPay> GetCasoutPay(DateTime? DateStart, DateTime? DateEnd , string PROCEDURE )
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);

                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_DateStart", DateStart));
                param.Add(new SqlParameter("@_DateEnd", DateEnd));
                var lstRs = db.GetListSP<CasoutPay>(PROCEDURE, param.ToArray());
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

        public List<CasoutPay1> GetCasoutPay1(DateTime? DateStart, DateTime? DateEnd, string PROCEDURE)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);

                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_DateStart", DateStart));
                param.Add(new SqlParameter("@_DateEnd", DateEnd));
                var lstRs = db.GetListSP<CasoutPay1>(PROCEDURE, param.ToArray());
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
    }
}