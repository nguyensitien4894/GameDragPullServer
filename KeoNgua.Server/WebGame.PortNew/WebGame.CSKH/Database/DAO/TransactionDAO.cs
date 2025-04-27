using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class TransactionDAO
    {

        private static readonly Lazy<TransactionDAO> _instance = new Lazy<TransactionDAO>(() => new TransactionDAO());

        public static TransactionDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// get danh sachs transaction refund
        /// </summary>
        /// <param name="UserType"></param>
        /// <param name="Type"></param>
        /// <param name="Status"></param>
        /// <param name="PartnerType"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="UserID"></param>
        /// <param name="TransID"></param>
        /// <param name="PartnerName"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="RecordPerpage"></param>
        /// <param name="TotalRecord"></param>
        /// <returns></returns>
        public List<TransactionRefund> TransactionSearch(int UserType, int Type, int Status, int PartnerType, string PartnerName,DateTime? FromDate, DateTime? ToDate, long UserID, long? TransID, int CurrentPage, int RecordPerpage, out int TotalRecord,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[13];

                param[0] = new SqlParameter("@_UserType", SqlDbType.Int);
                param[0].Value = UserType;
                param[1] = new SqlParameter("@_Type", SqlDbType.Int);
                param[1].Value = Type;
                param[2] = new SqlParameter("@_Status", SqlDbType.Int);
                param[2].Value = Status;
                param[3] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[3].Value = UserID;
                param[4] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[4].Value = TransID??(object)DBNull.Value;
                param[5] = new SqlParameter("@_PartnerType", SqlDbType.Int);
                param[5].Value = PartnerType;
                param[6] = new SqlParameter("@_PartnerName", SqlDbType.VarChar);
                param[6].Size = 20;
                param[6].Value = String.IsNullOrEmpty(PartnerName)?(object)DBNull.Value: PartnerName;
                param[7] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[7].Value = FromDate;
                param[8] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[8].Value = ToDate;

                param[9] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[9].Value = CurrentPage;
                param[10] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[10].Value = RecordPerpage;
                param[11] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
                param[12] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[12].Value = ServiceID;



                var _lstTransactionRefund = db.GetListSP<TransactionRefund>("SP_Transaction_Search_For_Retrieval", param.ToArray());
                TotalRecord = Convert.ToInt32(param[11].Value);
                return _lstTransactionRefund;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                TotalRecord = 0;
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



        public TransactionRefund TransactionByID(  long TransID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[0].Value = TransID;
   
                var _lstTransactionRefund = db.GetInstanceSP<TransactionRefund>("SP_Transaction_Get_By_ID", param.ToArray());
                
                return _lstTransactionRefund;

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

        public void UserRollbackTransAgency(long UserID, long RequestAmount, long FromTransID, string Note,int ServiceID, int ActionUserID, long RevokedAgencyID, out long TransID, out long RemainBalance, out long RemainWallet, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[11];

                param[0] = new SqlParameter("@_RevokedAgencyID", SqlDbType.BigInt);
                param[0].Value = RevokedAgencyID;
                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;
                param[2] = new SqlParameter("@_RequestAmount", SqlDbType.BigInt);
                param[2].Value = RequestAmount;
                param[3] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[3].Size = 400;
                param[3].Value = Note;
                param[4] = new SqlParameter("@_FromTransID", SqlDbType.BigInt);
                param[4].Value = FromTransID;
                param[5] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[5].Value = ServiceID;
                param[6] = new SqlParameter("@_ActionUserID", SqlDbType.Int);
                param[6].Value = ActionUserID;
                param[7] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[7].Direction = ParameterDirection.Output;
                param[8] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@_Response", SqlDbType.Int);
                param[10].Direction = ParameterDirection.Output;
               
              
              
   

                db.ExecuteNonQuerySP("SP_User_Rollback_Trans_Agency", param.ToArray());
                Response = Convert.ToInt32(param[10].Value);
                if (Response == 1)
                {
                    RemainWallet = Convert.ToInt64(param[9].Value);
                    RemainBalance = Convert.ToInt64(param[8].Value);
                    TransID = Convert.ToInt64(param[7].Value);
                }else
                {
                    RemainWallet = 0;
                    RemainBalance = 0;
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
            RemainBalance = 0;
            TransID = 0;
            Response = -99;
        }

        public void UserTransferRetrieve(long RevokedUserID, long UserID, long Amount, string Note, long FromTransID,int ServiceID, out long TransID, out long RemainWallet, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];


                param[0] = new SqlParameter("@_RevokedUserID", SqlDbType.BigInt);
                param[0].Value = RevokedUserID;
                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;
                param[2] = new SqlParameter("@_RequestAmount", SqlDbType.BigInt);
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
                param[7] = new SqlParameter("@_FromTransID", SqlDbType.BigInt);
                param[7].Value = FromTransID;
                param[8] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[8].Value = ServiceID;

                
                db.ExecuteNonQuerySP("SP_User_Transfer_Retrieve", param.ToArray());
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
    }
}