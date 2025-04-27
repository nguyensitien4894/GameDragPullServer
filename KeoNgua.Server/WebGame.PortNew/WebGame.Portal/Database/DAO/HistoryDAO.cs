using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class HistoryDAO
    {
        private static readonly Lazy<HistoryDAO> _instance = new Lazy<HistoryDAO>(() => new HistoryDAO());

        public static HistoryDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// lịch sử chơi game trên portal
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="ExchangeType">-1 lấy tất</param>
        /// <param name="CurrentPage"></param>
        /// <param name="RecordPerpage"></param>
        /// <param name="DateTransaction"></param>
        /// <param name="totalRecord"></param>
        /// <returns></returns>
        public List<UserHistory> GetBalanceHistory(long AccountId,int ExchangeType, int CurrentPage,  int RecordPerpage,DateTime DateTransaction, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                List<UserHistory> lstRs = new List<UserHistory>();
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_CurrentPage", CurrentPage);
                pars[2] = new SqlParameter("@_RecordPerpage", RecordPerpage);
                pars[3] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_ExchangeType", ExchangeType);
                pars[5] = new SqlParameter("@_DateTransaction", DateTransaction);
                lstRs = db.GetListSP<UserHistory>("SP_HistoryBalance_GetPaged", pars);
                totalRecord = (int)pars[3].Value;
                return lstRs;
            }
            catch (Exception e)
            {
                NLogManager.PublishException(e);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="GameType">-1 lấy tất</param>
        /// <param name="CurrentPage">start with 1</param>
        /// <param name="RecordPerpage"></param>
        /// <param name="DateTransaction"></param>
        /// <param name="totalRecord"></param>
        /// <returns></returns>
        public List<GameHistory> GetGameHistory(long AccountId, int GameType, int CurrentPage, int RecordPerpage, DateTime DateTransaction, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                List<GameHistory> lstRs = new List<GameHistory>();
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_CurrentPage", CurrentPage);
                pars[2] = new SqlParameter("@_RecordPerpage", RecordPerpage);
                pars[3] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_GameType", GameType);
                pars[5] = new SqlParameter("@_DateTransaction", DateTransaction);
                lstRs = db.GetListSP<GameHistory>("SP_HistoryPlay_GetPaged", pars);
                totalRecord = (int)pars[3].Value;
                return lstRs;
            }
            catch (Exception e)
            {
                NLogManager.PublishException(e);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }

        }


        /// <summary>
        /// ghi lại log opt
        /// </summary>
        /// <param name="SourceID"></param>
        /// <param name="Amount"></param>
        /// <param name="Fee"></param>
        /// <param name="AmountFee"></param>
        /// <param name="ExchangeType"></param>
        /// <param name="AccountID"></param>
        /// <param name="SessionID"></param>
        /// <param name="PreBalance"></param>
        /// <param name="Balance"></param>
        /// <param name="PreSafeBalance"></param>
        /// <param name="SafeBalance"></param>
        /// <param name="ClientIP"></param>
        /// <param name="Description"></param>
        /// <param name="Response"></param>
        public void GameInsert(int SourceID, long Amount, int Fee, int AmountFee, int ExchangeType, long AccountID, long SessionID, long PreBalance, long Balance, long PreSafeBalance, long SafeBalance, string ClientIP, string Description, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[14];



                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_SessionID", SqlDbType.BigInt);
                param[1].Value = SessionID;
                param[2] = new SqlParameter("@_SourceID", SqlDbType.Int);
                param[2].Value = SourceID;
                param[3] = new SqlParameter("@_ClientIP", SqlDbType.NVarChar);
                param[3].Size = 32;
                param[3].Value = ClientIP;
                param[4] = new SqlParameter("@_PreBalance", SqlDbType.BigInt);
                param[4].Value = PreBalance;
                param[5] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[5].Value = Balance;
                param[6] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[6].Value = Amount;
                param[7] = new SqlParameter("@_Fee", SqlDbType.Int);
                param[7].Value = Fee;
                param[8] = new SqlParameter("@_AmountFee", SqlDbType.Int);
                param[8].Value = AmountFee;

                param[9] = new SqlParameter("@_PreSafeBalance", SqlDbType.BigInt);
                param[9].Value = PreSafeBalance;
                param[10] = new SqlParameter("@_SafeBalance", SqlDbType.BigInt);
                param[10].Value = SafeBalance;
                param[11] = new SqlParameter("@_ExchangeType", SqlDbType.Int);
                param[11].Value = ExchangeType;

                param[12] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[12].Size = 1000;
                param[12].Value = Description;
                param[13] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[13].Direction = ParameterDirection.Output;

             
                db.ExecuteNonQuerySP("SP_HistoryBalance_Insert", param.ToArray());
                Response = Convert.ToInt32(param[13].Value);
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
        /// <summary>
        /// get notify 
        /// </summary>
        /// <returns></returns>
       

        
        
    }
}