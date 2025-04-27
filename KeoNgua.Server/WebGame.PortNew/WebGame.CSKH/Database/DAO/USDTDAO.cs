using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class USDTDAO
    {
        private static readonly Lazy<USDTDAO> _instance = new Lazy<USDTDAO>(() => new USDTDAO());

        public static USDTDAO Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// check hoàn tiền hay ko
        /// </summary>
        /// <param name="AdjustStatus"></param>
        /// <param name="ServiceID"></param>
        /// <param name="Response"></param>
        /// <param name="RefundUSDTAmount"></param>
        /// <param name="RequestID"></param>
        /// <param name="ExamineUserID"></param>
        /// <param name="UserID"></param>
        /// <param name="RefundGameAmount"></param>
        /// <param name="RefundVNDAmount"></param>
        /// <param name="RemainBalance"></param>
        /// <param name="FeedbackMessage"></param>
        /// <param name="Note"></param>
        public void UserBankRequestAdjust(long RequestID,int AdjustStatus, int ServiceID,  double RefundUSDTAmount, long ExamineUserID, long UserID, long RefundGameAmount, long RefundVNDAmount, string FeedbackMessage, string Note, out int Response, out long RemainBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;

                param[1] = new SqlParameter("@_ExamineUserID", SqlDbType.BigInt);
                param[1].Value = ExamineUserID;
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_RefundGameAmount", SqlDbType.BigInt);
                param[3].Value = RefundGameAmount;
                param[4] = new SqlParameter("@_RefundUSDTAmount", SqlDbType.Float);
                param[4].Value = RefundUSDTAmount;

                param[5] = new SqlParameter("@_RefundVNDAmount", SqlDbType.BigInt);
                param[5].Value = RefundVNDAmount;

                param[6] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[6].Size = 400;
                param[6].Value = FeedbackMessage;
                param[7] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[7].Size = 400;
                param[7].Value = Note;
                param[8] = new SqlParameter("@_AdjustStatus", SqlDbType.Int);
                param[8].Value = AdjustStatus;
                param[9] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[9].Value = ServiceID;
                param[10] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[10].Direction = ParameterDirection.Output;
                param[11] = new SqlParameter("@_Response", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
               
              

                db.ExecuteNonQuerySP("SP_UserBankRequest_Adjust", param.ToArray());
                Response = ConvertUtil.ToInt(param[11].Value);
                RemainBalance = ConvertUtil.ToLong(param[10].Value);
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
            RemainBalance = 0;
        }

        public void UserBankRequestExchangeAdjust(long RequestID, int AdjustStatus, int ServiceID, double? USDTAmount, long ExamineUserID,
            long UserID, string FeedbackMessage, string Note,out int Response, out long RemainBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[10];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;

                param[1] = new SqlParameter("@_ExamineUserID", SqlDbType.BigInt);
                param[1].Value = ExamineUserID;
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_USDTAmount", SqlDbType.BigInt);
                param[3].Value = USDTAmount??(object)DBNull.Value;
            

                param[4] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[4].Size = 400;
                param[4].Value = FeedbackMessage;
                param[5] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[5].Size = 400;
                param[5].Value = Note;
                param[6] = new SqlParameter("@_AdjustStatus", SqlDbType.Int);
                param[6].Value = AdjustStatus;
                param[7] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[7].Value = ServiceID;
                param[8] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_Response", SqlDbType.Int);
                param[9].Direction = ParameterDirection.Output;



                db.ExecuteNonQuerySP("SP_UserBankRequest_Exchange_Adjust", param.ToArray());
                Response = ConvertUtil.ToInt(param[9].Value);
                RemainBalance = ConvertUtil.ToLong(param[8].Value);
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
            RemainBalance = 0;
        }

        /// <summary>
        /// hàm khởi tạo  khi tạo  tạo order Request
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Amount"></param>
        /// <param name="Rate"></param>
        /// <param name="ReceivedMoney"></param>
        /// <param name="Status"></param>
        /// <param name="PartnerID"></param>
        /// <param name="ServiceID"></param>
        /// <param name="Description"></param>
        /// <param name="Response"></param>
        /// <param name="Fee"></param>
        /// <param name="RemainBalance"></param>
        /// <param name="RequestID"></param>
        /// <param name="TransID"></param>
        public void UserBankRequestCreate2(int RequestType, long UserID, long Amount, double Fee, long ReceivedMoney, int Status, int PartnerID, int ServiceID, string Description,string BankAccount,string BankNumber,string BankName, out int Response, out long RemainBalance, out long RequestID, out long TransID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[19];

                param[0] = new SqlParameter("@_RequestType", SqlDbType.Int);
                param[0].Value = RequestType;


                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;

                param[2] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[2].Value = Amount;

                param[3] = new SqlParameter("@_Fee", SqlDbType.Float);
                param[3].Value = Fee;

                param[4] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[4].Value = ReceivedMoney;

                param[5] = new SqlParameter("@_Status", SqlDbType.Int);
                param[5].Value = Status;


                param[6] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[6].Value = ServiceID;

                param[7] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[7].Value = PartnerID;

                param[8] = new SqlParameter("@_RequestDate", SqlDbType.DateTime);
                param[8].Value = DateTime.Now;

                param[9] = new SqlParameter("@_UpdateDate", SqlDbType.DateTime);
                param[9].Value = DateTime.Now;

                param[10] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[10].Value = UserID;


                param[11] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[11].Size = 400;
                param[11].Value = Description;

                param[12] = new SqlParameter("@_Response", SqlDbType.Int);
                param[12].Direction = ParameterDirection.Output;

                param[13] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[13].Direction = ParameterDirection.Output;

                param[14] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[14].Direction = ParameterDirection.Output;

                param[15] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[15].Direction = ParameterDirection.Output;

                param[16] = new SqlParameter("@_BankAccount", SqlDbType.VarChar);
                param[16].Size = 100;
                param[16].Value = BankAccount;
                param[17] = new SqlParameter("@_BankNumber", SqlDbType.VarChar);
                param[17].Size = 50;
                param[17].Value = BankNumber;

                param[18] = new SqlParameter("@_BankName", SqlDbType.VarChar);
                param[18].Size = 100;
                param[18].Value = BankName;

    


                db.ExecuteNonQuerySP("SP_UserBankRequest_Create", param.ToArray());
                Response = ConvertUtil.ToInt(param[12].Value);
                RemainBalance = ConvertUtil.ToLong(param[13].Value);
                RequestID = ConvertUtil.ToLong(param[14].Value);
                TransID = ConvertUtil.ToLong(param[15].Value);
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
            RequestID = 0;
            TransID = 0;
            Response = -99;
        }
       
        /// <summary>
        /// /
        /// </summary>
        /// <param name="RequestID">Request ID</param>
        /// <param name="RequestCode">Code so sánh</param>
        /// <param name="UserID">user id</param>
        /// <param name="PartnerID">partner ID</param>
        /// <param name="ServiceID">Service ID</param>
        /// <param name="Rate">Tỉ lệ bên USDT Nạp</param>
        /// <param name="ExchangeRate">Tỉ lệ Rút bên USDT </param>
        /// <param name="Amount">Số VND muốn nạp hoặc rút</param>
        /// <param name="USDTAmount">Số USDT tương ứng Amount khi nạp Rút</param>
        /// <param name="ReceivedMoney">Số tiền Game quy đổi Amount*Rate or ExchagneRate</param>
        /// <param name="RefundReceivedMoney">Số tiền hoàn</param>
        /// <param name="BankName">BankName</param>
        /// <param name="BankAccount">Bank Account</param>
        /// <param name="BankNumber">Bank Number</param>
        /// <param name="PartnerErrorCode">Code kết nối </param>
        /// <param name="FeedbackErrorCode">Code khi call back</param>
        /// <param name="PartnerMessage">Mô tả code khi kết nối</param>
        /// <param name="FeedbackMessage">Mô tả code khi callback</param>
        /// <param name="Description">Mô tả </param>
        /// <param name="RealAmount">Số tiền thực chuyển VND</param>
        /// <param name="RealUSDTAmount">Số tiền thực USD thực chuyển</param>
        /// <param name="RealReceivedMoney">Số tiền Game thực nhận</param>
        /// <param name="Response"></param>
        public void UserBankRequestUpdate2(long RequestID, string RequestCode, long UserID, int? PartnerID, int ServiceID,int Status,string PartnerStatus, double? Rate,
            double? ExchangeRate, long? Amount, double? USDTAmount, long? ReceivedMoney, long? RefundReceivedMoney
          , string BankName, string BankAccount, string BankNumber,
            string PartnerErrorCode, string FeedbackErrorCode, string PartnerMessage, string FeedbackMessage, string Description,
            double? RealAmount , double? RealUSDTAmount, double? RealReceivedMoney,string USDTWalletAddress, out int Response)
        {
            DBHelper db = null;
            try
            {


                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[28];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;

                param[1] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = RequestCode;
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[3].Value = Amount ?? (object)DBNull.Value;
                param[4] = new SqlParameter("@_Rate", SqlDbType.Float);
                param[4].Value = Rate ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[5].Value = ReceivedMoney ?? (object)DBNull.Value;
                param[6] = new SqlParameter("@_RefundReceivedMoney", SqlDbType.BigInt);
                param[6].Value = RefundReceivedMoney ?? (object)DBNull.Value;
                param[7] = new SqlParameter("@_Status", SqlDbType.Int);
                param[7].Value = Status;
                param[8] = new SqlParameter("@_BankName", SqlDbType.VarChar);
                param[8].Size = 100;
                param[8].Value = BankName;
                param[9] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[9].Size = 20;
                param[9].Value = PartnerErrorCode;
                param[10] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[10].Size = 1000;
                param[10].Value = PartnerMessage;
                param[11] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[11].Size = 20;
                param[11].Value = FeedbackErrorCode;
                param[12] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[12].Size = 1000;
                param[12].Value = FeedbackMessage;
                param[13] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[13].Size = 1000;
                param[13].Value = Description;
                param[14] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[14].Value = UserID;
                param[15] = new SqlParameter("@_UpdateDate", SqlDbType.DateTime);
                param[15].Value = DateTime.Now;
                param[16] = new SqlParameter("@_ExchangeRate", SqlDbType.Float);
                param[16].Value = ExchangeRate ?? (object)DBNull.Value;
                param[17] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[17].Value = PartnerID ?? (object)DBNull.Value;
                param[18] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[18].Value = ServiceID;
                param[19] = new SqlParameter("@_Response", SqlDbType.Int);
                param[19].Direction = ParameterDirection.Output;
                param[20] = new SqlParameter("@_BankAccount", SqlDbType.VarChar);
                param[20].Size = 100;
                param[20].Value = BankAccount;
                param[21] = new SqlParameter("@_BankNumber", SqlDbType.VarChar);
                param[21].Size = 50;
                param[21].Value = BankNumber;

                param[22] = new SqlParameter("@_RealAmount", SqlDbType.Float);
                param[22].Value = RealAmount ?? (object)DBNull.Value;


                param[23] = new SqlParameter("@_USDTAmount", SqlDbType.Float);
                param[23].Value = USDTAmount ?? (object)DBNull.Value;

                param[24] = new SqlParameter("@_RealUSDTAmount", SqlDbType.Float);
                param[24].Value = RealUSDTAmount ?? (object)DBNull.Value;
                param[25] = new SqlParameter("@_RealReceivedMoney", SqlDbType.BigInt);
                param[25].Value = RealReceivedMoney ?? (object)DBNull.Value;
                param[26] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[26].Size = 50;
                param[26].Value = PartnerStatus;
                param[27] = new SqlParameter("@_USDTWalletAddress", SqlDbType.VarChar);
                param[27].Size = 100;
                param[27].Value = USDTWalletAddress;


                db.ExecuteNonQuerySP("SP_UserBankRequest_Update", param.ToArray());
                Response = ConvertUtil.ToInt(param[19].Value);
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
        /// hàm check khi gọi status
        /// </summary>
        /// <param name="CheckStatus">--1: thanh cong; 0: that bai</param>
        /// <param name="ServiceID"></param>
        /// <param name="Response"></param>
        /// <param name="RealUSDTAmount"></param>
        /// <param name="RequestRate"></param>
        /// <param name="CheckerID"></param>
        /// <param name="RequestID"></param>
        /// <param name="UserID"></param>
        /// <param name="RequestAmount">Số tiền thực nhận</param>
        /// <param name="RealAmount"></param>
        /// <param name="RealReceivedMoney"></param>
        /// <param name="RemainBalance"></param>
        /// <param name="PartnerStatus"></param>
        public void UserBankRequestPartnerCheck(long RequestID, long UserID, int CheckStatus, int ServiceID, double? RealUSDTAmount, double RequestRate, long CheckerID,  long RequestAmount, long? RealAmount, long? RealReceivedMoney , string PartnerStatus, out long RemainBalance, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[13];
                param[0] = new SqlParameter("@_CheckerID", SqlDbType.BigInt);
                param[0].Value = CheckerID;
                param[1] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[1].Value = RequestID;
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_CheckStatus", SqlDbType.Int);
                param[3].Value = CheckStatus;
                param[4] = new SqlParameter("@_RequestAmount", SqlDbType.BigInt);
                param[4].Value = RequestAmount;
                param[5] = new SqlParameter("@_RealAmount", SqlDbType.BigInt);
                param[5].Value = RealAmount??(object)DBNull.Value;
                param[6] = new SqlParameter("@_RealReceivedMoney", SqlDbType.BigInt);
                param[6].Value = RealReceivedMoney ?? (object)DBNull.Value;

              
                
                param[7] = new SqlParameter("@_RealUSDTAmount", SqlDbType.Float);
                param[7].Value = RealUSDTAmount ?? (object)DBNull.Value;
                param[8] = new SqlParameter("@_RequestRate", SqlDbType.Float);
                param[8].Value = RequestRate;
                param[9] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[9].Size = 50;
                param[9].Value = PartnerStatus;
                param[10] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[10].Value = ServiceID;

                param[11] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[11].Direction = ParameterDirection.Output;

                param[12] = new SqlParameter("@_Response", SqlDbType.Int);
                param[12].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_UserBankRequest_PartnerCheck", param.ToArray());
                Response = ConvertUtil.ToInt(param[12].Value);

                RemainBalance = ConvertUtil.ToLong(param[11].Value);
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
            RemainBalance = 0;
        }

        /// <summary>
        /// lấy danh sách 
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="ServiceID"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="RecordPerpage"></param>
        /// <param name="TotalRecord"></param>
        /// <param name="FromRequestDate"></param>
        /// <param name="ToRequestDate"></param>
        /// <param name="RequestID"></param>
        /// <param name="UserID"></param>
        /// <param name="RequestCode"></param>
        /// <returns></returns>
        public List<UserBankRequest> UserBankRequestList(long? RequestID, string NickName, string RequestCode, DateTime? FromRequestDate, DateTime? ToRequestDate, int? Status, int? ServiceID,int? RequestType, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[11];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID ?? (object)DBNull.Value;
                param[1] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = RequestCode;

                param[2] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Value = NickName;

                param[3] = new SqlParameter("@_Status", SqlDbType.Int);
                param[3].Value = Status ?? (object)DBNull.Value;
                param[4] = new SqlParameter("@_FromRequestDate", SqlDbType.DateTime);
                param[4].Value = FromRequestDate ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_ToRequestDate", SqlDbType.DateTime);
                param[5].Value = ToRequestDate ?? (object)DBNull.Value;
                param[6] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[6].Value = ServiceID?? (object)DBNull.Value;
                param[7] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[7].Value = CurrentPage;
                param[8] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[8].Value = RecordPerpage;
                param[9] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@_RequestType", SqlDbType.Int);
                param[10].Value = RequestType ?? (object)DBNull.Value;
                var _lstUserBankRequest = db.GetListSP<UserBankRequest>("SP_Admin_UserBankRequest_List", param.ToArray());
                TotalRecord = ConvertUtil.ToInt(param[9].Value);
                return _lstUserBankRequest;

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

        public UserBankRequest UserBankRequestGetByID(long? RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID ?? (object)DBNull.Value;
                var _lstUserBankRequest = db.GetInstanceSP<UserBankRequest>("SP_UserBankRequest_GetByID", param.ToArray());
                return _lstUserBankRequest;

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
        /// <param name="Response"></param>
        public void UserBankRequestUpdateStatus2(long RequestID, string RequestCode, int ServiceID,
            string PartnerErrorCode, string FeedbackErrorCode, string PartnerMessage, string FeedbackMessage, int? Status, int TransReqStatus
          , long UserID, string PartnerStatus, double? Fee, double? USDTAmount, double? RealUSDTAmount, double? RealReceivedMoney, double? ExchangeRate, string USDTWalletAddress, int? PartnerID, out int Response)
        {
            DBHelper db = null;
            try
            {


                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[20];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;
                param[1] = new SqlParameter("@_Status", SqlDbType.Int);
                param[1].Value = Status;
                param[2] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[2].Size = 20;
                param[2].Value = PartnerErrorCode;
                param[3] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[3].Size = 1000;
                param[3].Value = PartnerMessage;
                param[4] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[4].Size = 20;
                param[4].Value = FeedbackErrorCode;
                param[5] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[5].Size = 1000;
                param[5].Value = FeedbackMessage;
                param[6] = new SqlParameter("@_UpdateDate", SqlDbType.DateTime);
                param[6].Value = DateTime.Now;
                param[7] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[7].Value = ServiceID;
                param[8] = new SqlParameter("@_Response", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_TransReqStatus", SqlDbType.Int);
                param[9].Value = TransReqStatus;
                param[10] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[10].Size = 50;
                param[10].Value = RequestCode;
                param[11] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[11].Value = UserID;
                param[12] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[12].Size = 50;
                param[12].Value = PartnerStatus;
                param[13] = new SqlParameter("@_Fee", SqlDbType.Float);
                param[13].Value = Fee ?? (object)DBNull.Value;

                param[14] = new SqlParameter("@_USDTAmount", SqlDbType.Float);
                param[14].Value = USDTAmount ?? (object)DBNull.Value;

                param[15] = new SqlParameter("@_RealUSDTAmount", SqlDbType.Float);
                param[15].Value = RealUSDTAmount ?? (object)DBNull.Value;


                param[16] = new SqlParameter("@_ExchangeRate", SqlDbType.Float);
                param[16].Value = ExchangeRate ?? (object)DBNull.Value;

                param[17] = new SqlParameter("@_USDTWalletAddress", SqlDbType.VarChar);
                param[17].Size = 200;
                param[17].Value = USDTWalletAddress;

                param[18] = new SqlParameter("@_RealReceivedMoney", SqlDbType.BigInt);
                param[18].Value = RealReceivedMoney ?? (object)DBNull.Value;

                param[19] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[19].Value = PartnerID ?? (object)DBNull.Value;

                db.ExecuteNonQuerySP("SP_UserBankRequest_UpdateStatus", param.ToArray());
                Response = ConvertUtil.ToInt(param[8].Value);
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


        public void UserBankRequestExamine(long RequestID, long UserID, string RequestCode, int CheckStatus, int ServiceID, double RealAmount, double RealUSDTAmount, long ExamineUserID, long RealReceivedMoney, double? USDTAmount, string USDTWalletAddress, 
            string PartnerStatus,string PartnerErrorCode,string PartnerMessage,string FeedbackErrorCode,string FeedbackMessage,
            out int Response, out long RemainBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[18];

                param[0] = new SqlParameter("@_ExamineUserID", SqlDbType.BigInt);
                param[0].Value = ExamineUserID;
                param[1] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[1].Value = RequestID;
                param[2] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Value = RequestCode;
                param[3] = new SqlParameter("@_RealAmount", SqlDbType.Float);
                param[3].Value = RealAmount;
                param[4] = new SqlParameter("@_RealUSDTAmount", SqlDbType.Float);
                param[4].Value = RealUSDTAmount;
                param[5] = new SqlParameter("@_RealReceivedMoney", SqlDbType.BigInt);
                param[5].Value = RealReceivedMoney;
                param[6] = new SqlParameter("@_USDTWalletAddress", SqlDbType.VarChar);
                param[6].Size = 200;
                param[6].Value = USDTWalletAddress;
                param[7] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[7].Value = UserID;
                param[8] = new SqlParameter("@_CheckStatus", SqlDbType.Int);
                param[8].Value = CheckStatus;
                param[9] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[9].Value = ServiceID;
                param[10] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[10].Direction = ParameterDirection.Output;
                param[11] = new SqlParameter("@_Response", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
                param[12] = new SqlParameter("@_USDTAmount", SqlDbType.Float);
                param[12].Value = USDTAmount;
                param[13] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[13].Size = 50;
                param[13].Value = PartnerStatus;
                param[14] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[14].Size = 20;
                param[14].Value = PartnerErrorCode;
                param[15] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[15].Size = 500;
                param[15].Value = PartnerMessage;
                param[16] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[16].Size = 20;
                param[16].Value = FeedbackErrorCode;

                param[17] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[17].Size = 500;
                param[17].Value = FeedbackMessage;

               


                db.ExecuteNonQuerySP("SP_UserBankRequest_Examine", param.ToArray());
                Response = ConvertUtil.ToInt(param[11].Value);
                RemainBalance = ConvertUtil.ToLong(param[10].Value);
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
            RemainBalance = 0;
        }

        public void UserBankRequestChargeExamine(long RequestID, long UserID, string RequestCode, int CheckStatus, int ServiceID, double RealAmount, double RealUSDTAmount, long ExamineUserID, long RealReceivedMoney, double? USDTAmount, string USDTWalletAddress,
            string PartnerStatus, string PartnerErrorCode, string PartnerMessage, string FeedbackErrorCode, string FeedbackMessage,
            out int Response, out long RemainBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[18];

                param[0] = new SqlParameter("@_ExamineUserID", SqlDbType.BigInt);
                param[0].Value = ExamineUserID;
                param[1] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[1].Value = RequestID;
                param[2] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Value = RequestCode;
                param[3] = new SqlParameter("@_RealAmount", SqlDbType.Float);
                param[3].Value = RealAmount;
                param[4] = new SqlParameter("@_RealUSDTAmount", SqlDbType.Float);
                param[4].Value = RealUSDTAmount;
                param[5] = new SqlParameter("@_RealReceivedMoney", SqlDbType.BigInt);
                param[5].Value = RealReceivedMoney;
                param[6] = new SqlParameter("@_USDTWalletAddress", SqlDbType.VarChar);
                param[6].Size = 200;
                param[6].Value = USDTWalletAddress;
                param[7] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[7].Value = UserID;
                param[8] = new SqlParameter("@_CheckStatus", SqlDbType.Int);
                param[8].Value = CheckStatus;
                param[9] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[9].Value = ServiceID;
                param[10] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[10].Direction = ParameterDirection.Output;
                param[11] = new SqlParameter("@_Response", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
                param[12] = new SqlParameter("@_USDTAmount", SqlDbType.Float);
                param[12].Value = USDTAmount;
                param[13] = new SqlParameter("@_PartnerStatus", SqlDbType.VarChar);
                param[13].Size = 50;
                param[13].Value = PartnerStatus;
                param[14] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[14].Size = 20;
                param[14].Value = PartnerErrorCode;
                param[15] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[15].Size = 500;
                param[15].Value = PartnerMessage;
                param[16] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[16].Size = 20;
                param[16].Value = FeedbackErrorCode;

                param[17] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[17].Size = 500;
                param[17].Value = FeedbackMessage;




                db.ExecuteNonQuerySP("SP_UserBankRequestCharge_Examine", param.ToArray());
                Response = ConvertUtil.ToInt(param[11].Value);
                RemainBalance = ConvertUtil.ToLong(param[10].Value);
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
            RemainBalance = 0;
        }



        public void UserBankRequest_UpdateOnlyStatus(long RequestID, int Status ,out int Response, out string Msg)
        {
            DBHelper db = null;
            try
            {


                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;
                param[1] = new SqlParameter("@_Status", SqlDbType.Int);
                param[1].Value = Status;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_Msg", SqlDbType.NVarChar);
                param[3].Size = 500;
                param[3].Direction = ParameterDirection.Output;
                

                db.ExecuteNonQuerySP("SP_UserBankRequest_UpdateOnlyStatus", param.ToArray());
                Response = ConvertUtil.ToInt(param[2].Value);
                Msg =  ConvertUtil.ToString(param[3].Value);
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
            Msg = "Không cập nhật được";
        }

        public void UserBankRequest_AdminApprove(long RequestID, int Status, long ApproverId, out int Response, out string Msg)
        {
            DBHelper db = null;
            try
            {


                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;

                param[1] = new SqlParameter("@_Status", SqlDbType.Int);
                param[1].Value = Status;

                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;

                param[3] = new SqlParameter("@_Msg", SqlDbType.NVarChar);
                param[3].Size = 500;
                param[3].Direction = ParameterDirection.Output;

                param[4] = new SqlParameter("@_ApproverId", SqlDbType.BigInt);
                param[4].Value = ApproverId;


                db.ExecuteNonQuerySP("SP_UserBankRequest_Approve", param.ToArray());
                Response = ConvertUtil.ToInt(param[2].Value);
                Msg = ConvertUtil.ToString(param[3].Value);
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
            Msg = "Không cập nhật được";
        }

        public void UserBankRequest_UpdateCallbackResult(QienCallbackParams p, out int Response, out string Msg)
        {
            DBHelper db = null;
            try
            {


                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = p.ref_id;
                param[1] = new SqlParameter("@_Status", SqlDbType.Int);
                if (p.err_code == 0)
                {
                    param[1].Value = 3;
                }
                else
                {
                    param[1].Value = -2;
                }

                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_Msg", SqlDbType.NVarChar);
                param[3].Size = 500;
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_ErrMsg", SqlDbType.NVarChar);
                param[3].Size = 500;
                param[4].Value = p.err_msg;


                db.ExecuteNonQuerySP("SP_UserBankRequest_UpdateCallbackResult", param.ToArray());
                Response = ConvertUtil.ToInt(param[2].Value);
                Msg = ConvertUtil.ToString(param[3].Value);
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
            Msg = "Không cập nhật được";
        }
    }
}