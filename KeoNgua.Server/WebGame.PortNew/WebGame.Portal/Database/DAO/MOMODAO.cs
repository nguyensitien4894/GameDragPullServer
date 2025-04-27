using MsWebGame.Portal.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class MOMODAO
    {
        private static readonly Lazy<MOMODAO> _instance = new Lazy<MOMODAO>(() => new MOMODAO());

        public static MOMODAO Instance
        {
            get { return _instance.Value; }
        }
        public UserMomoRequest UserMomoRequestGetByRefKey(string RefKey)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[0].Size = 250;
                param[0].Value = RefKey;

                var _UserMomoRequest = db.GetInstanceSP<UserMomoRequest>("SP_UserMomoRequest_GetByRefKey", param.ToArray());
                return _UserMomoRequest;
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


        public List<UserMomoRequest> UserMomoRequesList(long? RequestID, long? UserID, string NickName, string RequestCode,
           string RefKey, string RefSendKey, DateTime? FromRequestDate, DateTime? ToRequestDate,
           int? Status, int? ServiceID, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[13];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID ?? (object)DBNull.Value;
                param[1] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = RequestCode ?? (object)DBNull.Value;
                param[2] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[2].Size = 250;
                param[2].Value = RefKey ?? (object)DBNull.Value;
                param[3] = new SqlParameter("@_RefSendKey", SqlDbType.VarChar);
                param[3].Size = 250;
                param[3].Value = RefSendKey ?? (object)DBNull.Value;
                param[4] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[4].Size = 20;
                param[4].Value = NickName ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[5].Value = UserID ?? (object)DBNull.Value;
                param[6] = new SqlParameter("@_Status", SqlDbType.Int);
                param[6].Value = Status ?? (object)DBNull.Value;
                param[7] = new SqlParameter("@_FromRequestDate", SqlDbType.DateTime);
                param[7].Value = FromRequestDate ?? (object)DBNull.Value;
                param[8] = new SqlParameter("@_ToRequestDate", SqlDbType.DateTime);
                param[8].Value = ToRequestDate ?? (object)DBNull.Value;
                param[9] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[9].Value = ServiceID ?? (object)DBNull.Value;
                param[10] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[10].Value = CurrentPage;
                param[11] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[11].Value = RecordPerpage;
                param[12] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[12].Direction = ParameterDirection.Output;


                var _lstUserMomoReques = db.GetListSP<UserMomoRequest>("SP_UserMomoRequest_Admin_List", param.ToArray());
                TotalRecord = ConvertUtil.ToInt(param[12].Value);
                return _lstUserMomoReques;
                ;
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




        public void UserMomoRequestInsert(long UserID, int RequestType, int Status, int PartnerID, int ServiceID,
            DateTime RequestDate, DateTime? UpdateDate, double Rate,
            double Fee,  long Amount, long ReceivedMoney,
            long RefundReceivedMoney, long UpdateUser,
            string RequestCode, string PartnerStatus, string PartnerErrorCode, 
            string PartnerMessage, string FeedbackErrorCode,
            string FeedbackMessage, string RefKey, string RefSendKey, string Provider,
            string Description, out int Response, out long RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[25];

                param[0] = new SqlParameter("@_RequestType", SqlDbType.Int);
                param[0].Value = RequestType;
               
                param[1] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = RequestCode;
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[3].Value = Amount;
                param[4] = new SqlParameter("@_Rate", SqlDbType.Float);
                param[4].Value = Rate;
                param[5] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[5].Value = ReceivedMoney;
                param[6] = new SqlParameter("@_RefundReceivedMoney", SqlDbType.BigInt);
                param[6].Value = RefundReceivedMoney;
                param[7] = new SqlParameter("@_Status", SqlDbType.Int);
                param[7].Value = Status;
                param[8] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[8].Size = 50;
                param[8].Value = PartnerStatus??(object)DBNull.Value;
                param[9] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[9].Size = 20;
                param[9].Value = PartnerErrorCode?? (object)DBNull.Value;
                param[10] = new SqlParameter("@_PartnerMessage", SqlDbType.VarChar);
                param[10].Size = 500;
                param[10].Value = PartnerMessage?? (object)DBNull.Value;

                param[11] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[11].Size = 20;
                param[11].Value = FeedbackErrorCode ?? (object)DBNull.Value;
                param[12] = new SqlParameter("@_FeedbackMessage", SqlDbType.VarChar);
                param[12].Size = 500;
                param[12].Value = FeedbackMessage ?? (object)DBNull.Value;
                param[13] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[13].Size = 400;
                param[13].Value = Description ?? (object)DBNull.Value;
                param[14] = new SqlParameter("@_Fee", SqlDbType.Float);
                param[14].Value = Fee;

                param[15] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[15].Size = 250;
                param[15].Value = RefKey;
                param[16] = new SqlParameter("@_RefSendKey", SqlDbType.VarChar);
                param[16].Size = 250;
                param[16].Value = RefSendKey ?? (object)DBNull.Value;
                param[17] = new SqlParameter("@_Provider", SqlDbType.VarChar);
                param[17].Size = 100;
                param[17].Value = Provider ?? (object)DBNull.Value;




                param[18] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[18].Value = PartnerID;
                param[19] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[19].Value = ServiceID;
                param[20] = new SqlParameter("@_RequestDate", SqlDbType.DateTime);
                param[20].Value = RequestDate;
                param[21] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[21].Value = UpdateUser;
                param[22] = new SqlParameter("@_UpdateDate", SqlDbType.DateTime);
                param[22].Value = UpdateDate ?? (object)DBNull.Value;
                param[23] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[23].Direction = ParameterDirection.Output;
                param[24] = new SqlParameter("@_Response", SqlDbType.Int);
                param[24].Direction = ParameterDirection.Output;
          
                db.ExecuteNonQuerySP("SP_UserMomoRequest_Insert", param.ToArray());
                RequestID = ConvertUtil.ToLong(param[23].Value);
                Response = ConvertUtil.ToInt(param[24].Value);
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
            RequestID = -1;
        }

        public void UserMomoRequestUpdate(long RequestID, long UserID, int Status, int PartnerID, int ServiceID,
          DateTime UpdateDate, double Rate, double Fee,

           long Amount, long ReceivedMoney, long RefundReceivedMoney, long UpdateUser, string RequestCode,

          string PartnerStatus, string BankAccount, string BankNumber, string PartnerErrorCode,
          string FeedbackErrorCode, string PartnerMessage, string FeedbackMessage, string Description, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[22];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;
                param[1] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = RequestCode;
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[3].Value = Amount;
                param[4] = new SqlParameter("@_Rate", SqlDbType.Float);
                param[4].Value = Rate;
                param[5] = new SqlParameter("@_Fee", SqlDbType.Float);
                param[5].Value = Fee;
                param[6] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[6].Value = ReceivedMoney;
                param[7] = new SqlParameter("@_RefundReceivedMoney", SqlDbType.BigInt);
                param[7].Value = RefundReceivedMoney;
                param[8] = new SqlParameter("@_Status", SqlDbType.Int);
                param[8].Value = Status;

                param[9] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[9].Size = 50;
                param[9].Value = PartnerStatus;
                param[10] = new SqlParameter("@_BankAccount", SqlDbType.VarChar);
                param[10].Size = 100;
                param[10].Value = BankAccount;
                param[11] = new SqlParameter("@_BankNumber", SqlDbType.VarChar);
                param[11].Size = 50;
                param[11].Value = BankNumber;
                param[12] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[12].Size = 20;
                param[12].Value = PartnerErrorCode;
                param[14] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[14].Size = 20;
                param[14].Value = FeedbackErrorCode;
                param[13] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[13].Size = 1000;
                param[13].Value = PartnerMessage;
                param[15] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[15].Size = 1000;
                param[15].Value = FeedbackMessage;
                param[16] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[16].Size = 1000;
                param[16].Value = Description;
                param[17] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[17].Value = UpdateUser;
                param[18] = new SqlParameter("@_UpdateDate", SqlDbType.DateTime);
                param[18].Value = UpdateDate;

                param[19] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[19].Value = PartnerID;
                param[20] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[20].Value = ServiceID;
                param[21] = new SqlParameter("@_Response", SqlDbType.Int);
                param[21].Direction = ParameterDirection.Output;




                db.ExecuteNonQuerySP("SP_UserMomoRequest_Update", param.ToArray());
                Response = ConvertUtil.ToInt(param[21].Value);
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