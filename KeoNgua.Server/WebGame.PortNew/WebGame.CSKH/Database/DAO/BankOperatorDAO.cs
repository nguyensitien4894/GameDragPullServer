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
    public class BankOperatorDAO
    {
        private static readonly Lazy<BankOperatorDAO> _instance = new Lazy<BankOperatorDAO>(() => new BankOperatorDAO());


        /// <summary>
        /// Get Bank Operator
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <param name="OperatorCode"></param>
        /// <returns></returns>
        public List<BankOperators> BankOperatorsList(int? ServiceID, string OperatorCode,long ? ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = OperatorCode ?? (object)DBNull.Value;

                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID??(object)DBNull.Value;
                param[2] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[2].Value = ID ?? (object)DBNull.Value;

                var _BankOperators = db.GetListSP<BankOperators>("SP_BankOperators_Admin_List", param.ToArray());
                return _BankOperators ;
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


        public void BankOperatorsUpdate(long ID, string OperatorName, double Rate, double ExchangeRate, bool Status, bool ExchangeStatus, long UpdateUser, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];

               
              
                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                param[1] = new SqlParameter("@_Rate", SqlDbType.Float);
                param[1].Value = Rate;
                param[2] = new SqlParameter("@_ExchangeRate", SqlDbType.Float);
                param[2].Value = ExchangeRate;
                param[3] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[3].Value = Status;
                param[4] = new SqlParameter("@_ExchangeStatus", SqlDbType.Bit);
                param[4].Value = ExchangeStatus;
                param[5] = new SqlParameter("@_OperatorName", SqlDbType.NVarChar);
                param[5].Size = 100;
                param[5].Value = OperatorName;
                param[6] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[6].Value = UpdateUser;
               

                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_BankOperators_Update", param.ToArray());
                Response = ConvertUtil.ToInt(param[7].Value);
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
            Response = -99;
        }


        public static BankOperatorDAO Instance
        {
            get { return _instance.Value; }
        }
    }
}