using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class BankOeratorSecondaryDAO
    {
        private static readonly Lazy<BankOeratorSecondaryDAO> _instance = new Lazy<BankOeratorSecondaryDAO>(() => new BankOeratorSecondaryDAO());
        public static BankOeratorSecondaryDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<BankOperatorsSecondary> GetList(long id, string OperatorCode, int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = String.IsNullOrEmpty(OperatorCode) ? (object)DBNull.Value : OperatorCode;
                param[1] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[1].Value = id;
                param[2] = new SqlParameter("@_ServiceID", serviceid);
                var _lstTelecomOperators = db.GetListSP<BankOperatorsSecondary>("SP_BankOperatorsSecondary_List", param.ToArray());
                return _lstTelecomOperators;
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

        public void TelecomOperatorsHandle(string OperatorCode, string OperatorName, double Rate, bool Status, long TelOperatorID, long CreateUser, int serviceid, out int ResponseStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];
                param[0] = new SqlParameter("@_TelOperatorID", SqlDbType.BigInt);
                param[0].Value = TelOperatorID;
                param[1] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[1].Size = 20;
                param[1].Value = OperatorCode;
                param[2] = new SqlParameter("@_OperatorName", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = OperatorName;
                param[3] = new SqlParameter("@_Rate", SqlDbType.Float);
                param[3].Value = Rate;
                param[4] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[4].Value = Status;
                param[5] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[5].Value = CreateUser;
                param[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_ServiceID", serviceid);

                db.ExecuteNonQuerySP("SP_BankOperatorsSecondary_Handle", param.ToArray());
                ResponseStatus = Convert.ToInt32(param[6].Value);
                return;
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
            ResponseStatus = -99;
        }

        public BankOperatorsSecondary BankOperatorsSecondaryGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _Telecom = db.GetInstanceSP<BankOperatorsSecondary>("SP_BankOperatorsSecondary_GetByID", param.ToArray());
                return _Telecom;
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