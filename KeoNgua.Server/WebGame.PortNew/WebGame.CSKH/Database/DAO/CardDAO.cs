using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class CardDAO
    {
        private static readonly Lazy<CardDAO> _instance = new Lazy<CardDAO>(() => new CardDAO());

        public static CardDAO Instance
        {
            get { return _instance.Value; }
        }
        public Card CardsGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _Card = db.GetInstanceSP<Card>("SP_Cards_GetByID", param.ToArray());
                return _Card;
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

        public List<Card> GetList(long ID, long TellID, string CardCode, string CardName, bool? Status, int CurrentPage, int RecordPerpage,int ServiceID ,  out long TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;
                param[1] = new SqlParameter("@_TellID", SqlDbType.BigInt);
                param[1].Value = TellID;
                param[2] = new SqlParameter("@_CardCode", SqlDbType.NVarChar);
                param[2].Size = 40;
                param[2].Value = CardCode;
                param[3] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[3].Value = Status.HasValue ? Status.Value : (object)DBNull.Value;
                param[4] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[4].Value = CurrentPage;
                param[5] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[5].Value = RecordPerpage;
                param[6] = new SqlParameter("@_TotalRecord", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_CardName", SqlDbType.NVarChar);
                param[7].Size = 40;
                param[7].Value = CardName;
                param[8] = new SqlParameter("@_ServiceID", ServiceID);
                
                var _lstCard = db.GetListSP<Card>("SP_Cards_List_Paging", param.ToArray());
                TotalRecord = Convert.ToInt64(param[6].Value);
                return _lstCard;

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


        public void Cards_Handle(string CardCode, string CardName, long TeleID, int CardValue, double CardRate, double CardSwapRate, bool Status,bool ExchangeStatus, long CreateUser,long? PartnerID,int ServiceID, out int ResponseStatus)
        {
            if (!PartnerID.HasValue) PartnerID = 0;
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];


                param[0] = new SqlParameter("@_CardCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = CardCode;
                param[1] = new SqlParameter("@_CardName", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = CardName;
                param[2] = new SqlParameter("@_CardValue", SqlDbType.Int);
                param[2].Value = CardValue;
                param[3] = new SqlParameter("@_CardRate", SqlDbType.Float);
                param[3].Value = CardRate;
                param[4] = new SqlParameter("@_CardSwapRate", SqlDbType.Float);
                param[4].Value = CardSwapRate;
                param[5] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[5].Value = Status;
                param[6] = new SqlParameter("@_TeleID", SqlDbType.BigInt);
                param[6].Value = TeleID;
                param[7] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[7].Value = CreateUser;
                param[8] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_ExchangeStatus", SqlDbType.Bit);
                param[9].Value = ExchangeStatus;
                param[10] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[10].Value = PartnerID;
                param[11] = new SqlParameter("@_ServiceID", ServiceID);

                db.ExecuteNonQuerySP("SP_Cards_Handle", param.ToArray());
                ResponseStatus = Convert.ToInt32(param[8].Value);
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
            ResponseStatus = -99;
        }
        public List<CardRechargeTopList> FnCardRechargeTopList(DateTime? FromDate, DateTime? ToDate,int serviceid, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] pars = new SqlParameter[6];

                pars[0] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                pars[0].Value = FromDate;
                pars[1] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                pars[1].Value = ToDate;
                pars[2] = new SqlParameter("@_CurrentPage", currentPage);
                pars[3] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_ServiceID", serviceid);
                var _lstCardRechargeTopList = db.GetListSP<CardRechargeTopList>("SP_CardRecharge_Top_List", pars.ToArray());
                totalRecord = ConvertUtil.ToInt(pars[4].Value);
                return _lstCardRechargeTopList;
            }
            catch (Exception ex)
            {
                totalRecord = 0;
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
        public List<UserCardRecharge> UserCardRechargeList(long? RequestID,int telOperatorId, string nickName, int cardValue, string cardNumber, string serialNumber, DateTime? fromRechargeDate,
            DateTime? toRechargeDate,int? status,int? PartnerID,int ?smg,int serviceid, string PartnerErrorCode, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] pars = new SqlParameter[19];
                pars[0] = new SqlParameter("@_RequestID", null);
                pars[1] = new SqlParameter("@_UserName", null);
                pars[2] = new SqlParameter("@_TelOperatorID", telOperatorId);
                pars[3] = new SqlParameter("@_CardID", null);
                pars[4] = new SqlParameter("@_UserID", null);
                pars[5] = new SqlParameter("@_NickName", nickName);
                pars[6] = new SqlParameter("@_CardNumber", cardNumber);
                pars[7] = new SqlParameter("@_SerialNumber", serialNumber);
                pars[8] = new SqlParameter("@_CardValue", cardValue);
                pars[9] = new SqlParameter("@_FromRechargeDate", fromRechargeDate);
                pars[10] = new SqlParameter("@_ToRechargeDate", toRechargeDate);
                pars[11] = new SqlParameter("@_CurrentPage", currentPage);
                pars[12] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[13] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[14] = new SqlParameter("@_Status", status);
                pars[15] = new SqlParameter("@_PartnerID", PartnerID);
                pars[16] = new SqlParameter("@_smg", smg);
                pars[17] = new SqlParameter("@_ServiceID", serviceid);
                pars[18] = new SqlParameter("@_PartnerErrorCode", PartnerErrorCode);
                var _lstUserCardRecharge = db.GetListSP<UserCardRecharge>("SP_UserCardRecharge_Admin_List", pars.ToArray());
                totalRecord = Convert.ToInt32(pars[13].Value);
                return _lstUserCardRecharge;
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
            totalRecord = 0;
            return null;
        }

        public List<UserCardSwap> UserCardSwapList(string userName, string nickName, string cardNumber, string cardSerial, int? cardValue, DateTime? buyDate,
            DateTime? expriredDate, int status, DateTime? checkDate,int serviceid, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_UserName", userName);
                pars[1] = new SqlParameter("@_NickName", nickName);
                pars[2] = new SqlParameter("@_CardNumber", cardNumber);
                pars[3] = new SqlParameter("@_CardSerial", cardSerial);
                pars[4] = new SqlParameter("@_CardValue", cardValue);
                pars[5] = new SqlParameter("@_BuyDate", buyDate);
                pars[6] = new SqlParameter("@_ExpiredDate", expriredDate);
                pars[7] = new SqlParameter("@_Status", status);
                pars[8] = new SqlParameter("@_CheckDate", checkDate);
                pars[9] = new SqlParameter("@_CurrentPage", currentPage);
                pars[10] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[11] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
             
                pars[12] = new SqlParameter("@_ServiceID", serviceid);
                var lstRs = db.GetListSP<UserCardSwap>("SP_UserCardSwap_List", pars);
                totalRecord = Convert.ToInt32(pars[11].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<ImportCardInfo> ImportCard(DataTable dtCard, out int response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_CardBankTable", dtCard);
                pars[1] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var lstRs = db.GetListSP<ImportCardInfo>("SP_CardBanks_Import_File", pars);
                response = Convert.ToInt32(pars[1].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                response = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
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

        public int UserCardSwapExamine(long examineUserId, long userCardSwapId, long userId, int status, out long balance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_ExamineUserID", examineUserId);
                pars[1] = new SqlParameter("@_UserCardSwapID", userCardSwapId);
                pars[2] = new SqlParameter("@_UserID", userId);
                pars[3] = new SqlParameter("@_CheckStatus", status);
                pars[4] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_UserCardSwap_Examine", pars);

                balance = ConvertUtil.ToLong(pars[4].Value);
                return ConvertUtil.ToInt(pars[5].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                balance = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<CardBankInfo> GetInventoryCardBankList(int telId, string cardCode, string cardName, int? cardValue, string cardNumber, string cardSerial, 
            int status,int currentPage,int recordPerpage,out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_TelOperatorID", telId);
                pars[1] = new SqlParameter("@_CardCode", cardCode);
                pars[2] = new SqlParameter("@_CardName", cardName);
                pars[3] = new SqlParameter("@_CardValue", cardValue);
                pars[4] = new SqlParameter("@_CardNumber", cardNumber);
                pars[5] = new SqlParameter("@_CardSerial", cardSerial);
                pars[6] = new SqlParameter("@_Status", status);
                pars[7] = new SqlParameter("@_CurrentPage", currentPage);
                pars[8] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[9] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<CardBankInfo>("SP_Inventory_CardBank_List", pars);
                totalRecord = ConvertUtil.ToInt(pars[9].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<CardBankCheck> GetInventoryCardBankCheck()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<CardBankCheck>("SP_Inventory_CardBank_Check");
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        /// <summary>
        /// lấy user card theo request id 
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public UserCardRecharge UserCardRechargeGetByID(long RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;

                var _UserCardRecharge = db.GetInstanceSP<UserCardRecharge>("SP_UserCardRecharge_GetByID", param.ToArray());
                return _UserCardRecharge;
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

        /// <summary>
        /// cập nhật trạng thái card status
        /// </summary>
        /// <param name="ReqestID"></param>
        /// <param name="UserID"></param>
        /// <param name="FeedbackErrorCode"></param>
        /// <param name="FeedbackMessage"></param>
        /// <param name="Status"></param>
        /// <param name="Description"></param>
        /// <param name="Response"></param>
        public void UserCardRechargeAdminUpdate(long ReqestID, long UserID,long AdminID, string FeedbackErrorCode, string FeedbackMessage, int Status, 
            string Description,int?  RefundCardValude,long? RefundReceivedMoney, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[10];
                param[0] = new SqlParameter("@_ReqestID", SqlDbType.BigInt);
                param[0].Value = ReqestID;
                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;
                param[2] = new SqlParameter("@_Status", SqlDbType.Int);
                param[2].Value = Status;
                param[3] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = FeedbackErrorCode;
                param[4] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[4].Size = 1000;
                param[4].Value = FeedbackMessage;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[6].Value = AdminID;
                param[7] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[7].Size = 500;
                param[7].Value = Description;
                param[8] = new SqlParameter("@_RefundCardValude", SqlDbType.Int);
                param[8].Value = RefundCardValude;
                param[9] = new SqlParameter("@_RefundReceivedMoney", SqlDbType.BigInt);
                param[9].Value = RefundReceivedMoney;

                db.ExecuteNonQuerySP("SP_UserCardRecharge_Admin_Update", param.ToArray());
                Response = Convert.ToInt32(param[5].Value);
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

        public List<UserRechargeProgress> GetUserRechargeProgress(string nickName, DateTime fromDate, DateTime toDate,int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_NickName", nickName);
                pars[1] = new SqlParameter("@_FromDate", fromDate);
                pars[2] = new SqlParameter("@_ToDate", toDate);
                pars[3] = new SqlParameter("@_ServiceID", serviceid);
                var lstRs = db.GetListSP<UserRechargeProgress>("SP_UserRecharge_Progress", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        /// <summary>
        /// thống kê admin
        /// </summary>
        /// <returns></returns>
        public List<CardRechargeProgress> GetCardRechargeProgress(int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_TelOperatorId", 0);
                pars[1] = new SqlParameter("@_StartDate", null);
                pars[2] = new SqlParameter("@_EndDate", null);
                pars[3] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<CardRechargeProgress>("SP_CardRecharge_Progress", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<TopUpReport> GetTopUpReport(DateTime? fromDate, DateTime? toDate, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_StartDate", fromDate);
                pars[1] = new SqlParameter("@_ToDate", toDate);
                pars[2] = new SqlParameter("@_ServiceID", 1);
                pars[3] = new SqlParameter("@_MomoSum", SqlDbType.BigInt);
                pars[3].Direction = ParameterDirection.Output;
                pars[4] = new SqlParameter("@_ViettelPaySum", SqlDbType.BigInt);
                pars[4].Direction = ParameterDirection.Output;
                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<TopUpReport>("SP_CardRecharge_Sum", pars);

                long momo = Convert.ToInt32(pars[3].Value);
                long viettelpay = Convert.ToInt32(pars[4].Value);

                if (lstRs == null)
                {
                    lstRs = new List<TopUpReport>();
                }

                if (momo > 0)
                {
                    lstRs.Add(new TopUpReport()
                    {
                        CardValue = momo,
                        TelOperatorID = 4,
                    });
                }
                if (viettelpay > 0)
                {
                    lstRs.Add(new TopUpReport()
                    {
                        CardValue = viettelpay,
                        TelOperatorID = 5,
                    });
                }
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

    }
}