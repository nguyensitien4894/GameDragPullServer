using MsWebGame.Thecao.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Thecao.Database.DAO
{
    public class ViettelPayDAO
    {
        private static readonly Lazy<ViettelPayDAO> _instance = new Lazy<ViettelPayDAO>(() => new ViettelPayDAO());

        public static ViettelPayDAO Instance
        {
            get { return _instance.Value; }
        }
        public UserViettelPayRequest UserViettelPayRequestGetByRefKey(string RefKey)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[0].Size = 250;
                param[0].Value = RefKey;

                var _UserMomoRequest = db.GetInstanceSP<UserViettelPayRequest>("SP_UserViettelPayRequest_GetByRefKey", param.ToArray());
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
        public UserViettelPayRequest UserViettelPayRequestGetByID(long RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);

                param[0].Value = RequestID;

                var _UserMomoRequest = db.GetInstanceSP<UserViettelPayRequest>("SP_UserViettelPayRequest_GetByID", param.ToArray());
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
        public void UserViettelPayRequestReceiveResult(
            int RequestType, long RequestAmount, int PartnerID, string RequestCode
           , string PartnerStatus, string PartnerErrorCode,
            string PartnerMessage, string FeedbackErrorCode,
            string FeedbackMessage, string RefKey, string RefSendKey, string Provider,
            long RequestID, out int Response, out long ReceivedMoney, out long RemainBalance, out double RequestRate, out int ServiceID


            )
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[18];


                param[0] = new SqlParameter("@_RequestAmount", SqlDbType.BigInt);
                param[0].Value = RequestAmount;
                param[1] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = PartnerStatus;
                param[2] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Value = PartnerErrorCode ?? (object)DBNull.Value;
                param[3] = new SqlParameter("@_PartnerMessage", SqlDbType.VarChar);
                param[3].Size = 100;
                param[3].Value = PartnerMessage ?? (object)DBNull.Value;
                param[4] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[4].Size = 50;
                param[4].Value = FeedbackErrorCode ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_FeedbackMessage", SqlDbType.VarChar);
                param[5].Size = 100;
                param[5].Value = FeedbackMessage ?? (object)DBNull.Value;
                param[6] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[6].Value = PartnerID;
                param[7] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[7].Size = 250;
                param[7].Value = RefKey;
                param[8] = new SqlParameter("@_RefSendKey", SqlDbType.VarChar);
                param[8].Size = 250;
                param[8].Value = RefSendKey;
                param[9] = new SqlParameter("@_Provider", SqlDbType.VarChar);
                param[9].Size = 50;
                param[9].Value = Provider;
                param[10] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[10].Value = RequestID;
                param[11] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[11].Direction = ParameterDirection.Output;
                param[12] = new SqlParameter("@_RequestRate", SqlDbType.Float);
                param[12].Direction = ParameterDirection.Output;
                param[13] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[13].Direction = ParameterDirection.Output;

                param[14] = new SqlParameter("@_Response", SqlDbType.Int);
                param[14].Direction = ParameterDirection.Output;

                param[15] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[15].Size = 50;
                param[15].Value = RequestCode ?? (object)DBNull.Value;
                param[16] = new SqlParameter("@_RequestType", SqlDbType.Int);
                param[16].Value = RequestType;

                param[17] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[17].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_UserViettelPayRequest_ReceiveResult", param.ToArray());
                ReceivedMoney = ConvertUtil.ToLong(param[11].Value);
                RequestRate = ConvertUtil.ToFloat(param[12].Value);
                RemainBalance = ConvertUtil.ToLong(param[13].Value);
                Response = ConvertUtil.ToInt(param[14].Value);
                ServiceID = ConvertUtil.ToInt(param[17].Value);

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

            ReceivedMoney = 0;
            RequestRate = 0;
            RemainBalance = 0;
            Response = -99;
            ServiceID = 0;
        }



        public void UserViettelPayRequestPartnerCheck(
            long UserID, int RequestType, long RequestAmount, int PartnerID, string RequestCode
           , string PartnerStatus, string PartnerErrorCode,
            string PartnerMessage, string FeedbackErrorCode,
            string FeedbackMessage, string RefKey, string RefSendKey, string Provider,
            out int Response, out long RequestID, out long ReceivedMoney, out long RemainBalance, out double RequestRate, out int ServiceID


            )
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[19];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RequestAmount", SqlDbType.BigInt);
                param[1].Value = RequestAmount;
                param[2] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Value = PartnerStatus;
                param[3] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[3].Size = 50;
                param[3].Value = PartnerErrorCode ?? (object)DBNull.Value;
                param[4] = new SqlParameter("@_PartnerMessage", SqlDbType.VarChar);
                param[4].Size = 100;
                param[4].Value = PartnerMessage ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[5].Size = 50;
                param[5].Value = FeedbackErrorCode ?? (object)DBNull.Value;
                param[6] = new SqlParameter("@_FeedbackMessage", SqlDbType.VarChar);
                param[6].Size = 100;
                param[6].Value = FeedbackMessage ?? (object)DBNull.Value;
                param[7] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[7].Value = PartnerID;
                param[8] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[8].Size = 250;
                param[8].Value = RefKey;
                param[9] = new SqlParameter("@_RefSendKey", SqlDbType.VarChar);
                param[9].Size = 250;
                param[9].Value = RefSendKey;
                param[10] = new SqlParameter("@_Provider", SqlDbType.VarChar);
                param[10].Size = 50;
                param[10].Value = Provider;
                param[11] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[11].Direction = ParameterDirection.Output;
                param[12] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[12].Direction = ParameterDirection.Output;
                param[13] = new SqlParameter("@_RequestRate", SqlDbType.Float);
                param[13].Direction = ParameterDirection.Output;
                param[14] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[14].Direction = ParameterDirection.Output;

                param[15] = new SqlParameter("@_Response", SqlDbType.Int);
                param[15].Direction = ParameterDirection.Output;

                param[16] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[16].Size = 50;
                param[16].Value = RequestCode ?? (object)DBNull.Value;
                param[17] = new SqlParameter("@_RequestType", SqlDbType.Int);
                param[17].Value = RequestType;

                param[18] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[18].Direction = ParameterDirection.Output;





                db.ExecuteNonQuerySP("SP_UserViettelPayRequest_PartnerCheck", param.ToArray());
                RequestID = ConvertUtil.ToLong(param[11].Value);
                ReceivedMoney = ConvertUtil.ToLong(param[12].Value);
                RequestRate = ConvertUtil.ToFloat(param[13].Value);
                RemainBalance = ConvertUtil.ToLong(param[14].Value);
                Response = ConvertUtil.ToInt(param[15].Value);
                ServiceID = ConvertUtil.ToInt(param[18].Value);

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
            RequestID = 0;
            ReceivedMoney = 0;
            RequestRate = 0;
            RemainBalance = 0;
            Response = -99;
            ServiceID = 0;
        }

        public void UserViettelPayRequestUpdate(long RequestID, long UserID, int Status, int PartnerID, int ServiceID,
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




                db.ExecuteNonQuerySP("SP_UserViettelPayRequest_Update", param.ToArray());
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