using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class EsportsDAO
    {
        private static readonly Lazy<EsportsDAO> _instance = new Lazy<EsportsDAO>(() => new EsportsDAO());

        public static EsportsDAO Instance
        {
            get { return _instance.Value; }
        }
        public void EvoBet(dynamic data, out long Balance, out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = 104;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];

                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = data.AccountID;

                param[1] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[1].Value = data.amount;

                param[2] = new SqlParameter("@_RoundId", SqlDbType.VarChar);
                param[2].Size = 8000;
                param[2].Value = data.roundId; 

                param[3] = new SqlParameter("@_TransactionId", SqlDbType.VarChar);
                param[3].Size = 8000;
                param[3].Value = data.transactionId; 

                param[4] = new SqlParameter("@_Status", SqlDbType.Int);
                param[4].Value = Convert.ToInt32(data.status); 

                param[5] = new SqlParameter("@_BetDetail", SqlDbType.VarChar);
                param[5].Size = 8000;
                param[5].Value = data.betDetail; 

      
                param[6] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;

                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Evolution_Bet", param.ToArray());

                Balance = Convert.ToInt64(param[6].Value);
                Response = Convert.ToInt32(param[7].Value);
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
        }
        public void EvoUpdateResult(dynamic data, out long Balance, out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = 104;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];

                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = data.AccountID;

                param[1] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[1].Value = data.payout;

                param[2] = new SqlParameter("@_RoundId", SqlDbType.VarChar);
                param[2].Size = 8000;
                param[2].Value = data.roundId;

                param[3] = new SqlParameter("@_TransactionId", SqlDbType.VarChar);
                param[3].Size = 8000;
                param[3].Value = data.transactionId;

                param[4] = new SqlParameter("@_Status", SqlDbType.Int);
                param[4].Value = Convert.ToInt32(data.status);

                param[5] = new SqlParameter("@_BetDetail", SqlDbType.VarChar);
                param[5].Size = 8000;
                param[5].Value = data.betDetail;


                param[6] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;

                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Evolution_UpdateResult", param.ToArray());

                Balance = Convert.ToInt64(param[6].Value);
                Response = Convert.ToInt32(param[7].Value);
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
        }
        public void Esports_Deposit_To_Game(string Wallet, long UserID, long Amount, int ServiceID, out long Balance, out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = -99;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = UserID;

                param[1] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[1].Value = Amount;


                param[2] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;

                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Esports_Deposit_To_Game", param.ToArray());

                Balance = Convert.ToInt64(param[2].Value);
                Response = Convert.ToInt32(param[3].Value);
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
        }
        public void SP_Esports_Withdraw_To_Game(string Wallet,long UserID, long Amount, int ServiceID,out long Balance,  out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = -99;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = UserID;

                param[1] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[1].Value = Amount;


                param[2] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;

                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;


                db.ExecuteNonQuerySP("SP_Esports_Withdraw_To_Game", param.ToArray());

                Balance = Convert.ToInt64(param[2].Value);
                Response = Convert.ToInt32(param[3].Value);
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
             
        }
    }
}