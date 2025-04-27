using MsWebGame.CSKH.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class TxBackListDAO
    {
        private static readonly Lazy<TxBackListDAO> _instance = new Lazy<TxBackListDAO>(() => new TxBackListDAO());
        public static TxBackListDAO Instance
        {
            get { return _instance.Value; }
        }
        public int AddBackList(string DisplayName, int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceLogConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_DisplayName", DisplayName));
                param.Add(new SqlParameter("@_ServiceID", ServiceID));
                SqlParameter response = new SqlParameter("@_ReponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_BalackList_Create", param.ToArray());

                return Convert.ToInt32(response.Value);
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

        public int DeleteBackList(long accountId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceLogConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountId));
             
                SqlParameter response = new SqlParameter("@_ReponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_BalackList_Delete", param.ToArray());

                return Convert.ToInt32(response.Value);
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

        public List<TxBackList> GetList(string DisplayName, int? ServiceID, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.LuckyDiceLogConn);
                SqlParameter[] param = new SqlParameter[5];

                param[0] = new SqlParameter("@_DisplayName", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = DisplayName;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = ServiceID??(object)DBNull.Value;
                param[2] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[2].Value = CurrentPage;
                param[3] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[3].Value = RecordPerpage;
                param[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;

                var _lstBank = db.GetListSP<TxBackList>("SP_BalackList_GetPage", param.ToArray());

                TotalRecord = Convert.ToInt32(param[4].Value);
                return _lstBank;
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

    }
}