using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;

namespace MsWebGame.Thecao.Database.DAO
{
    public class CashDAO
    {
        private static readonly Lazy<CashDAO> _instance = new Lazy<CashDAO>(() => new CashDAO());

        public static CashDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// nap tien vao cho
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Amount"></param>
        /// <param name="Note"></param>
        /// <param name="FromTransID"></param>
        /// <param name="Balance"></param>
        /// <param name="TransID"></param>
        /// <param name="Response"></param>
        public void CashInMarket( long UserID, long Amount, string Note,long FromTransID, out long Balance, out long TransID, out int Response )
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[7];

               
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[1].Value = Amount;
                param[2] = new SqlParameter("@_FromTransID", SqlDbType.BigInt);
                param[2].Value = FromTransID;
                param[3] = new SqlParameter("@_Note", SqlDbType.VarChar);
                param[3].Size = 200;
                param[3].Value = Note;
                param[4] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_Response", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Cash_In_Market", param.ToArray());
                Balance = Convert.ToInt64(param[4].Value);
                TransID = Convert.ToInt64(param[5].Value);
                Response = Convert.ToInt32(param[6].Value);
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
            Balance = 0;
            TransID = 0;
            Response = -99;
        }

        /// <summary>
        /// rut tien ra khoi cho
        /// </summary>
        /// <param name="Note"></param>
        /// <param name="UserID"></param>
        /// <param name="OrgAmount"></param>
        /// <param name="Balance"></param>
        /// <param name="BalanceHang"></param>
        /// <param name="TransID"></param>
        /// <param name="Response"></param>
        public void CashOutMarket( long UserID, long OrgAmount, string Note,out long Balance,out long BalanceHang, out long TransID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];

            
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_OrgAmount", SqlDbType.BigInt);
                param[1].Value = OrgAmount;
                param[2] = new SqlParameter("@_Note", SqlDbType.VarChar);
                param[2].Size = 200;
                param[2].Value = Note;
                param[3] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_BalanceHang", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                
                param[6] = new SqlParameter("@_Response", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Cash_Out_Market", param.ToArray());
                Balance = Convert.ToInt64(param[3].Value);
                BalanceHang = Convert.ToInt64(param[4].Value);
                TransID = Convert.ToInt64(param[5].Value);
                Response = Convert.ToInt32(param[6].Value);
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
            Balance = 0;
            BalanceHang = 0;
            TransID = 0;
            Response = 0;
            Response = -99;
        }
        /// <summary>
        /// cap nhat so tien
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="TransID"></param>
        /// <param name="TransStatus"></param>
        /// <param name="Response"></param>
        /// <param name="Balance"></param>
        public void CashUpdate( long UserID, long TransID, string TransStatus, out long Balance,out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[5];

               
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[1].Value = TransID;
               
                param[2] = new SqlParameter("@_TransStatus", SqlDbType.VarChar);
                param[2].Size = 20;
                param[2].Value = TransStatus;
                param[3] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Cash_Update", param.ToArray());
                Response = Convert.ToInt32(param[4].Value);
                Balance = Convert.ToInt32(param[3].Value);
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
            Response = 0;
            Balance = 0;
            Response = -99;
        }
    }
}