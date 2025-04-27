using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using TraditionGame.Utilities;


using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class UserTranferDAO
    {
        private static readonly Lazy<UserTranferDAO> _instance = new Lazy<UserTranferDAO>(() => new UserTranferDAO());

        public static UserTranferDAO Instance
        {
            get { return _instance.Value; }
        }
        public void UserTransferToUser( long RemitterId, long ReceiverId, long Amount, string Note, out long TransID, out long RemainWallet, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];

             
                param[0] = new SqlParameter("@_RemitterId", SqlDbType.BigInt);
                param[0].Value = RemitterId;
                param[1] = new SqlParameter("@_ReceiverId", SqlDbType.BigInt);
                param[1].Value = ReceiverId;
                param[2] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[2].Value = Amount;
                param[3] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[3].Size = 400;
                param[3].Value = Note;
                param[4] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;

                param[6] = new SqlParameter("@_Response", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_User_Transfer_To_User", param.ToArray());
                Response = Convert.ToInt32(param[6].Value);
                if (Response == 1)
                {
                    TransID = Convert.ToInt64(param[4].Value);
                    RemainWallet = Convert.ToInt64(param[5].Value);
                }
                else
                {
                    TransID = 0;
                    RemainWallet = 0;
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
            TransID = 0;
            RemainWallet = 0;
            Response = -99;
        }



        /// <summary>
        /// chuyen tien cho dai ly
        /// </summary>
        /// <param name="RemitterId"></param>
        /// <param name="ReceiverId"></param>
        /// <param name="ReceiverType"></param>
        /// <param name="ReceiverLevel"></param>
        /// <param name="Amount"></param>
        /// <param name="Note"></param>
        /// <param name="TransID"></param>
        /// <param name="RemainWallet"></param>
        /// <param name="Response"></param>
        public void UserTransferToAgencyAdmin(long RemitterId, long ReceiverId,string ReceiverName,int ReceiverType, int ReceiverLevel, long Amount, string Note, out long TransID, out long RemainWallet, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);

               SqlParameter[] param = new SqlParameter[10];
                param[0] = new SqlParameter("@_RemitterId", SqlDbType.BigInt);
                param[0].Value = RemitterId;
                param[1] = new SqlParameter("@_ReceiverType", SqlDbType.Int);
                param[1].Value = ReceiverType;
                param[2] = new SqlParameter("@_ReceiverId", SqlDbType.BigInt);
                param[2].Value = ReceiverId;

                param[3] = new SqlParameter("@_ReceiverLevel", SqlDbType.Int);
                param[3].Value = ReceiverLevel;
                param[4] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[4].Value = Amount;
                param[5] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[5].Size = 400;
                param[5].Value = Note;
                //param[6] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                //param[6].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                param[8] = new SqlParameter("@_ReceiverName", SqlDbType.VarChar);
                param[8].Size = 50;
                param[8].Value = ReceiverName;
                param[9] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[9].Direction = ParameterDirection.Output;
                



                db.ExecuteNonQuerySP("SP_User_Transfer_To_Agency_Admin", param.ToArray());
                Response = Convert.ToInt32(param[7].Value);
                if (Response == 1)
                {
                    RemainWallet = Convert.ToInt64(param[6].Value);
                    TransID = Convert.ToInt64(param[9].Value);
                }
                else
                {
                    RemainWallet = 0;
                    TransID = 0;
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
            RemainWallet = 0;
            TransID = 0;
            Response = -99;
        }

        /// <summary>
        /// cập nhật lại user tranfer trên 109
        /// </summary>
        /// <param name="RemitterId"></param>
        /// <param name="ReceiverId"></param>
        /// <param name="ReceiverType"></param>
        /// <param name="Amount"></param>
        /// <param name="Note"></param>
        /// <param name="TransID"></param>
        /// <param name="Wallet"></param>
        /// <param name="Response"></param>
        public void UserTransfer(long RemitterId, long ReceiverId, int ReceiverType, long Amount, string Note,long FromTransID, out long TransID, out long Wallet, out int Response)
        {
            DBHelper db = null;
            try
            {


               db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[9];

                param[0] = new SqlParameter("@_RemitterId", SqlDbType.BigInt);
                param[0].Value = RemitterId;
                param[1] = new SqlParameter("@_ReceiverId", SqlDbType.BigInt);
                param[1].Value = ReceiverId;
                param[2] = new SqlParameter("@_ReceiverType", SqlDbType.Int);
                param[2].Value = ReceiverType;
                param[3] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[3].Value = Amount;
                param[4] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[4].Size = 400;
                param[4].Value = Note;
                param[5] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_ReceiverWallet", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                param[8] = new SqlParameter("@_FromTransID", SqlDbType.BigInt);
                param[8].Value = FromTransID;
                db.ExecuteNonQuerySP("SP_User_Transfer", param.ToArray());
                Response = Convert.ToInt32(param[7].Value);
                if (Response == 1)
                {
                    Wallet = Convert.ToInt64(param[6].Value);
                    TransID = Convert.ToInt64(param[5].Value);
                }
                else
                {
                    Wallet = 0;
                    TransID = 0;
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
            Wallet = 0;
            TransID = 0;
            Response = -99;
        }


        /// <summary>
        /// lịch sử truyển nạp
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <param name="TranType"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="RecordPerpage"></param>
        /// <param name="TotalRecord"></param>
        /// <returns></returns>
        public List<BalanceLogs> BalanceLogsList(long UserID, string UserName, int TranType, DateTime? FromDate, DateTime? ToDate, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_UserName", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = UserName;
                param[2] = new SqlParameter("@_TranType", SqlDbType.Int);
                param[2].Value = TranType;
                param[3] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[3].Value = FromDate ?? (object)DBNull.Value;
                param[4] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[4].Value = ToDate ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[5].Value = CurrentPage;
                param[6] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[6].Value = RecordPerpage;
                param[7] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                var _lstBalanceLogs = db.GetListSP<BalanceLogs>("SP_BalanceLogs_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[7].Value);
                return _lstBalanceLogs;

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

        /// <summary>
        /// hàm này chỉ được dùng cho nạp thẻ
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="nickName"></param>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <param name="note"></param>
        /// <param name="transId"></param>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public int UserRechargeCard( long userId, long amount, string note,int  ServiceID, out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_Amount", amount);
                pars[2] = new SqlParameter("@_Note", note);
                //pars[3] = new SqlParameter("@_TransID", transId);
                pars[3] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_User_Recharge_Card", pars);
                wallet = 0;
                var response=ConvertUtil.ToInt(pars[5].Value);
                if (response == 1)
                {
                    wallet= ConvertUtil.ToInt(pars[4].Value);
                    transId = 0;
                }
                transId = 0;
                return response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                wallet = 0;
                transId = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public void UserCheckLimit( long UserID,long Amount, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                param[2] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[2].Value = Amount;
                

                db.ExecuteNonQuerySP("SP_User_Check_Limit", param.ToArray());
                Response = Convert.ToInt32(param[1].Value);
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

    }
}