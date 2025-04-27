using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class AccountWalletLogsDAO
    {
        private static readonly Lazy<AccountWalletLogsDAO> _instance = new Lazy<AccountWalletLogsDAO>(() => new AccountWalletLogsDAO());

        public static AccountWalletLogsDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// get log tranfer
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="AccountType"></param>
        /// <param name="SearchType"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="RecordPerpage"></param>
        /// <param name="TotalRecord"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        public List<AccountWalletLog> GetList(long AccountId, int AccountType, int SearchType, DateTime?StartDate, DateTime ?EndDate,long objectID, int CurrentPage, int RecordPerpage, out int TotalRecord, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[10];

                param[0] = new SqlParameter("@_AccountId", SqlDbType.BigInt);
                param[0].Value = AccountId;
                param[1] = new SqlParameter("@_AccountType", SqlDbType.Int);
                param[1].Value = AccountType;
                param[2] = new SqlParameter("@_SearchType", SqlDbType.Int);
                param[2].Value = SearchType;
                param[3] = new SqlParameter("@_StartDate", SqlDbType.DateTime);
                param[3].Value = StartDate;
                param[4] = new SqlParameter("@_EndDate", SqlDbType.DateTime);
                param[4].Value = EndDate;
                param[5] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[5].Value = CurrentPage;
                param[6] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[6].Value = RecordPerpage;
                param[7] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                param[8] = new SqlParameter("@_Response", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_ObjectId", SqlDbType.BigInt);
                param[9].Value = objectID;
                var _lstAccountWalletLog = db.GetListSP<AccountWalletLog>("SP_AccountWalletLogs_List", param.ToArray());
               
                Response = Convert.ToInt32(param[8].Value);
                if (Response == AppConstants.DBS.SUCCESS)
                {
                    TotalRecord = 0;
                }
                else
                {
                    TotalRecord = 0;
                }
                return _lstAccountWalletLog;
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
            Response = 0;
            return null;
        }

    }
}