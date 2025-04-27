using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class CardDAO
    {
        public UserCardRecharge UserCardRechargeGetByID(long RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;

                var _UserCardRecharge = db.GetInstanceSP<UserCardRecharge>("SP_UserCardRecharge_FrontEnd_GetByID", param.ToArray());
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
        private static readonly Lazy<CardDAO> _instance = new Lazy<CardDAO>(() => new CardDAO());

        public static CardDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<Cards> GetCardList(string CardCode, int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_CardCode", SqlDbType.NVarChar);
                param[0].Size = 40;
                param[0].Value = CardCode;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                var _lstCards = db.GetListSP<Cards>("SP_Cards_List", param.ToArray());
                return _lstCards;
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
        public List<Cards> GetCardSWapList(string CardCode, int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_CardCode", SqlDbType.NVarChar);
                param[0].Size = 40;
                param[0].Value = CardCode;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                var _lstCards = db.GetListSP<Cards>("SP_Cards_Swap_List", param.ToArray());
                return _lstCards;
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


        public void UserCardRechargeCreate(int TelOperatorID, int CardID, int CardValue, int Status, double Rate, long UserID, long ReceivedMoney, long CreateUser, string CardNumber, string SerialNumber, string PartnerErrorCode, string PartnerMessage, string Description, string Signature, int ServiceID, out int Response, out long RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[17];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_TelOperatorID", SqlDbType.Int);
                param[1].Value = TelOperatorID;
                param[2] = new SqlParameter("@_CardID", SqlDbType.Int);
                param[2].Value = CardID;
                param[3] = new SqlParameter("@_CardNumber", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = CardNumber;
                param[4] = new SqlParameter("@_SerialNumber", SqlDbType.VarChar);
                param[4].Size = 20;
                param[4].Value = SerialNumber;
                param[5] = new SqlParameter("@_CardValue", SqlDbType.Int);
                param[5].Value = CardValue;
                param[6] = new SqlParameter("@_Rate", SqlDbType.Float);
                param[6].Value = Rate;
                param[7] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[7].Value = ReceivedMoney;
                param[8] = new SqlParameter("@_Status", SqlDbType.Int);
                param[8].Value = Status;
                param[9] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[9].Size = 20;
                param[9].Value = PartnerErrorCode;
                param[10] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[10].Size = 1000;
                param[10].Value = PartnerMessage;
                param[11] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[11].Size = 1000;
                param[11].Value = Description;
                param[12] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[12].Value = CreateUser;
                param[13] = new SqlParameter("@_Signature", SqlDbType.NVarChar);
                param[13].Size = 4000;
                param[13].Value = Signature;
                param[14] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[14].Direction = ParameterDirection.Output;
                param[15] = new SqlParameter("@_Response", SqlDbType.Int);
                param[15].Direction = ParameterDirection.Output;
                param[16] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[16].Value = ServiceID;

                db.ExecuteNonQuerySP("SP_UserCardRecharge_Create", param.ToArray());
                Response = Convert.ToInt32(param[15].Value);
                if (Response == 1)
                {
                    RequestID = Convert.ToInt64(param[14].Value);
                }
                else
                {
                    RequestID = 0;
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
            RequestID = 0;
            Response = -99;
            return;
        }


        /// <summary>
        /// hàm này sẽ dùng chính
        /// </summary>
        /// <param name="OperatorCode"></param>
        /// <param name="serviceid"></param>
        /// <param name="ActionStatus"></param>
        /// <returns></returns>
        public List<TelecomOperators> GetTeleComList(string OperatorCode, int serviceid, int ActionStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = OperatorCode??(object)DBNull.Value;
                param[1] = new SqlParameter("@_ServiceID", serviceid);
                param[2] = new SqlParameter("@_ActionStatus", ActionStatus);
                var _lstTelecomOperator = db.GetListSP<TelecomOperators>("SP_TelecomOperators_Portal_List", param.ToArray());

                return _lstTelecomOperator;
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
        public List<TelecomOperators> GetTeleCom(string OperatorCode, int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = OperatorCode;
                param[1] = new SqlParameter("@_ServiceID", serviceid);
                var _lstTelecomOperator = db.GetListSP<TelecomOperators>("SP_TelecomOperators_List", param.ToArray());

                return _lstTelecomOperator;
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

        public List<TelecomOperators> GetTeleFast(int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);

                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_ServiceID", serviceid);
                var _lstTelecomOperator = db.GetListSP<TelecomOperators>("SP_TelecomOperators_ListFast", param.ToArray());

                return _lstTelecomOperator;
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

        public void ActiveByNPP(long TelOperatorID, bool ActiveByNPP, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_TelOperatorID", SqlDbType.BigInt);
                param[0].Value = TelOperatorID;

                param[1] = new SqlParameter("@_ActiveByNPP", SqlDbType.Bit);
                param[1].Value = ActiveByNPP;
                param[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_TelecomOperators_Update", param.ToArray());
                Response = Convert.ToInt32(param[2].Value);
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


        public void UserCardRechargeUpdate(long ReqestID, long UserID, int Status, string PartnerErrorCode, string PartnerMessage, string FeedbackErrorCode, string FeedbackMessage, string Description, string Signature, long CreateUser, int PartnerID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];
                param[0] = new SqlParameter("@_ReqestID", SqlDbType.BigInt);
                param[0].Value = ReqestID;
                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;
                param[2] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[2].Size = 20;
                param[2].Value = PartnerErrorCode;
                param[3] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = FeedbackErrorCode;
                param[4] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[4].Size = 1000;
                param[4].Value = PartnerMessage;
                param[5] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[5].Size = 1000;
                param[5].Value = FeedbackMessage;
                param[6] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[6].Size = 1000;
                param[6].Value = Description;
                param[7] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[7].Value = CreateUser;
                param[8] = new SqlParameter("@_Signature", SqlDbType.NVarChar);
                param[8].Size = 1000;
                param[8].Value = Signature;
                param[9] = new SqlParameter("@_Response", SqlDbType.Int);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@_Status", SqlDbType.Int);
                param[10].Value = Status;
                param[11] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[11].Value = PartnerID;


                db.ExecuteNonQuerySP("SP_UserCardRecharge_Update", param.ToArray());
                Response = Convert.ToInt32(param[9].Value);
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
        /// get list serial card
        /// </summary>
        /// <param name="TelOperatorID"></param>
        /// <param name="CardID"></param>
        /// <param name="CardValue"></param>
        /// <param name="FromRechargeDate"></param>
        /// <param name="ToRechargeDate"></param>
        /// <param name="RequestID"></param>
        /// <param name="UserName"></param>
        /// <param name="CardNumber"></param>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        public List<UserCardRecharge> UserCardRechargeList(long RequestID, long UserID, int TelOperatorID, int CardID, int CardValue, string UserName, string CardNumber, string SerialNumber, DateTime? FromRechargeDate, DateTime? ToRechargeDate, int ServiceID, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {

            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[14];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID;
                param[1] = new SqlParameter("@_UserName", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = UserName;
                param[2] = new SqlParameter("@_TelOperatorID", SqlDbType.Int);
                param[2].Value = TelOperatorID;
                param[3] = new SqlParameter("@_CardID", SqlDbType.Int);
                param[3].Value = CardID;
                param[4] = new SqlParameter("@_CardNumber", SqlDbType.VarChar);
                param[4].Size = 50;
                param[4].Value = CardNumber;
                param[5] = new SqlParameter("@_SerialNumber", SqlDbType.VarChar);
                param[5].Size = 50;
                param[5].Value = SerialNumber;
                param[6] = new SqlParameter("@_CardValue", SqlDbType.Int);
                param[6].Value = CardValue;
                param[7] = new SqlParameter("@_FromRechargeDate", SqlDbType.DateTime);
                param[7].Value = FromRechargeDate.HasValue ? FromRechargeDate.Value : (object)DBNull.Value;
                param[8] = new SqlParameter("@_ToRechargeDate", SqlDbType.DateTime);
                param[8].Value = ToRechargeDate.HasValue ? ToRechargeDate.Value : (object)DBNull.Value;
                param[9] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[9].Value = CurrentPage;
                param[10] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[10].Value = RecordPerpage;

                param[11] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
                param[12] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[12].Value = UserID;
                param[13] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[13].Value = ServiceID;


                var _lstUserCardRecharge = db.GetListSP<UserCardRecharge>("SP_UserCardRecharge_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[11].Value);
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
            TotalRecord = 0;
            return null;
        }

        public List<UserCardRecharge_New> SP_UserCardRecharge_List_New(long RequestID, long UserID, int TelOperatorID, int CardID, int CardValue, string UserName, string CardNumber, string SerialNumber, DateTime? FromRechargeDate, DateTime? ToRechargeDate, int ServiceID, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {

            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                var _lstUserCardRecharge = db.GetListSP<UserCardRecharge_New>("SP_UserCardRecharge_List_New", param.ToArray());
                TotalRecord = 0;
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
            TotalRecord = 0;
            return null;
        }

        public void UpdateCardChardStatus(long ReqestID, long UserID, string FeedbackErrorCode, string FeedbackMessage, int Status, int PartnerID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];
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
                param[6].Value = UserID;
                param[7] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[7].Value = PartnerID;

                db.ExecuteNonQuerySP("SP_UserCardRecharge_Update", param.ToArray());
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


        /// <summary>
        /// updte card result
        /// </summary>
        /// <param name="ReqestID"></param>
        /// <param name="GameAccountID"></param>
        /// <param name="ErrorCode"></param>
        /// <param name="Message"></param>
        /// <param name="Signature"></param>
        /// <param name="Response"></param>
        /// <param name="RemainBalance"></param>
        public void UpdateUserCardReceiveResult(long ReqestID, long GameAccountID, string ErrorCode, string Message, string Signature, out int Response, out long RemainBalance)
        {
            DBHelper db = null;

            try
            {
                db = new DBHelper(Config.BettingConn);



                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@_ReqestID", SqlDbType.BigInt);
                param[0].Value = ReqestID;
                param[1] = new SqlParameter("@_ErrorCode", SqlDbType.VarChar);
                param[1].Size = 20;
                param[1].Value = ErrorCode;
                param[2] = new SqlParameter("@_Message", SqlDbType.NVarChar);
                param[2].Size = 1000;
                param[2].Value = Message;
                param[3] = new SqlParameter("@_GameAccountID", SqlDbType.BigInt);
                param[3].Value = GameAccountID;
                param[6] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;

                param[4] = new SqlParameter("@_Signature", SqlDbType.NVarChar);
                param[4].Size = 4000;
                param[4].Value = Signature;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_UserCardRecharge_ReceiveResult", param.ToArray());
                Response = Convert.ToInt32(param[5].Value);
                if (Response == 1)
                {
                    RemainBalance = Convert.ToInt64(param[6].Value);
                }
                else
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
            Response = -99;
            RemainBalance = 0;
        }


        /// <summary>
        /// đổi thẻ
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="Amount"></param>
        /// <param name="TelCode"></param>
        /// <param name="DeviceType"></param>
        /// <param name="Status"></param>
        /// <param name="Response"></param>
        /// <param name="RemainBalance"></param>
        public void UserCardCashout(long AccountId, int Amount, string TelCode, int DeviceType, int ServiceID, out int Status, out int Response, out long RemainBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];
                param[0] = new SqlParameter("@_AccountId", SqlDbType.BigInt);
                param[0].Value = AccountId;
                param[1] = new SqlParameter("@_Amount", SqlDbType.Int);
                param[1].Value = Amount;
                param[2] = new SqlParameter("@_TelCode", SqlDbType.NVarChar);
                param[2].Size = 20;
                param[2].Value = TelCode;
                param[3] = new SqlParameter("@_DeviceType", SqlDbType.Int);
                param[3].Value = DeviceType;
                param[4] = new SqlParameter("@_Status", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_Response", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[7].Value = ServiceID;


                db.ExecuteNonQuerySP("SP_UserCard_Cashout", param.ToArray());
                Response = Convert.ToInt32(param[6].Value);
                if (Response == 1)
                {
                    RemainBalance = Convert.ToInt64(param[5].Value);
                    Status = Convert.ToInt32(param[4].Value);
                }
                else
                {
                    RemainBalance = 0;
                    Status = 0;
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
            Status = 0;
            Response = -99;
        }
        public void UserCardSerialCount(long AccountID, string SerialNumber, int TelOperatorID, int ServiceID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_SerialNumber", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = SerialNumber;
                param[1] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[1].Value = AccountID;


                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_TelOperatorID", SqlDbType.Int);
                param[3].Value = TelOperatorID;
                param[4] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[4].Value = ServiceID;



                db.ExecuteNonQuerySP("SP_UserCardCharge_Count", param.ToArray());
                Response = Convert.ToInt32(param[2].Value);

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