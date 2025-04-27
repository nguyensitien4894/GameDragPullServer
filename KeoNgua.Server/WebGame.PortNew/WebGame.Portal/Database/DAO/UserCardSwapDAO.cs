using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class UserCardSwapDAO
    {
        private static readonly Lazy<UserCardSwapDAO> _instance = new Lazy<UserCardSwapDAO>(() => new UserCardSwapDAO());

        public static UserCardSwapDAO Instance
        {
            get { return _instance.Value; }
        }
        public UserCardSwap UserCardSwapGetByID(long RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;

                var _obj = db.GetInstanceSP<UserCardSwap>("SP_UserCardSwap_GetByID", param.ToArray());
                return _obj;
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
        public List<UserCardSwap_New> GetList_New(long AccountID, long? CardValue, string CardNumber, string CardSerial, int Status, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
        
                var _lstUserCardSwap = db.GetListSP<UserCardSwap_New>("SP_UserCardSwap_New", param.ToArray());
                TotalRecord = 0;
                return _lstUserCardSwap;

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
        public List<UserCardSwap> GetList( long AccountID, long?CardValue, string CardNumber, string CardSerial, int Status, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[7];
               
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_CardNumber", SqlDbType.NVarChar);
                param[1].Size = 40;
                param[1].Value = CardNumber;
                param[2] = new SqlParameter("@_CardSerial", SqlDbType.NVarChar);
                param[2].Size = 40;
                param[2].Value = CardSerial;
              
                param[3] = new SqlParameter("@_Status", SqlDbType.Int);
                param[3].Value = Status;
                param[4] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[4].Value = CurrentPage;
                param[5] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[5].Value = RecordPerpage;
                param[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                var _lstUserCardSwap = db.GetListSP<UserCardSwap>("SP_UserCardSwap_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[6].Value);
                return _lstUserCardSwap;
                
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
        public void UserCardSwapCancel( long UserID, long UserCardSwapID, out long RemainBalance, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];

               
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_UserCardSwapID", SqlDbType.BigInt);
                param[1].Value = UserCardSwapID;
                param[2] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_UserCardSwap_Cancel", param.ToArray());
                Response = Convert.ToInt32(param[3].Value);
                if (Response == 1)
                {
                    RemainBalance = Convert.ToInt64(param[2].Value);
                }else
                {
                    RemainBalance = 0;
                }
                
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
            RemainBalance = 0;
            Response = -99;
        }
    }
}