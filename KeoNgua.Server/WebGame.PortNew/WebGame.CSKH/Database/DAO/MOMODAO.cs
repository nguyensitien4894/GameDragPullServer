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
    public class MOMODAO
    {
       
            private static readonly Lazy<MOMODAO> _instance = new Lazy<MOMODAO>(() => new MOMODAO());

            public static MOMODAO Instance
            {
                get { return _instance.Value; }
            }


        public UserMomoRequest UserMomoRequestGetByID(long RequestID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);

                param[0].Value = RequestID;

                var _UserMomoRequest = db.GetInstanceSP<UserMomoRequest>("SP_UserMomoRequest_GetByID", param.ToArray());
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
        public void UserMomoRequestUpdate(long RequestID, long UserID, int Status, long UpdateUser, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[1].Value = RequestID;
                param[2] = new SqlParameter("@_Status", SqlDbType.Int);
                param[2].Value = Status;
                param[3] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[3].Value = UpdateUser;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_UserMomoRequest_Update", param.ToArray());
                Response = ConvertUtil.ToInt(param[4].Value);
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

        public void UserMomoRequestPartnerCheck(
            long UserID, int RequestType, long RequestAmount, int PartnerID, string RequestCode
           , string PartnerStatus, string PartnerErrorCode,
            string PartnerMessage, string FeedbackErrorCode,
            string FeedbackMessage, string RefKey, string RefSendKey, string Provider,
            out int Response, out long RequestID, out long ReceivedMoney, out long RemainBalance, out double RequestRate, out int ServiceID


            )
        {
            NLogManager.LogMessage(String.Format("DEEEEBUUUUGGGGGG : {0}", "OK"));

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



                db.ExecuteNonQuerySP("SP_UserMomoRequest_PartnerCheck", param.ToArray());
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
        public List<UserMomoRequest> UserMomoRequesList(int Type,  long? RequestID, long? UserID, string NickName, string RequestCode, 
            string RefKey, string RefSendKey, DateTime? FromRequestDate, DateTime? ToRequestDate,
            int? Status, int? ServiceID,  int ? PartnerID,string MomoReceive,int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[16];
                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID??(object)DBNull.Value;
                param[1] = new SqlParameter("@_RequestCode", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = RequestCode??(object)DBNull.Value;
                param[2] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[2].Size = 250;
                param[2].Value = RefKey??(object)DBNull.Value;
                param[3] = new SqlParameter("@_RefSendKey", SqlDbType.VarChar);
                param[3].Size = 250;
                param[3].Value = RefSendKey??(object)DBNull.Value;
                param[4] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[4].Size = 20;
                param[4].Value = NickName??(object)DBNull.Value;
                param[5] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[5].Value = UserID??(object)DBNull.Value;
                param[6] = new SqlParameter("@_Status", SqlDbType.Int);
                param[6].Value = Status??(object)DBNull.Value;
                param[15] = new SqlParameter("@_RequestType", SqlDbType.Int);
                param[15].Value = Type ;
                param[7] = new SqlParameter("@_FromRequestDate", SqlDbType.DateTime);
                param[7].Value = FromRequestDate??(object)DBNull.Value;
                param[8] = new SqlParameter("@_ToRequestDate", SqlDbType.DateTime);
                param[8].Value = ToRequestDate??(object)DBNull.Value;
                param[9] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[9].Value = ServiceID??(object)DBNull.Value;
                param[10] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[10].Value = CurrentPage;
                param[11] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[11].Value = RecordPerpage;
                param[12] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[12].Direction = ParameterDirection.Output;
                param[13] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[13].Value = PartnerID ?? (object)DBNull.Value;
                param[14] = new SqlParameter("@_MomoReceive", SqlDbType.VarChar);

                param[14].Size = 200;
                param[14].Value = MomoReceive ?? (object)DBNull.Value;

                var _lstUserMomoReques = db.GetListSP<UserMomoRequest>("SP_UserMomoRequest_Admin_List", param.ToArray());
                TotalRecord = ConvertUtil.ToInt(param[12].Value);
                return _lstUserMomoReques;
             
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


        public void UserMomoRequest_AdminApprove(long RequestID, int Status, long ApproverId, out int Response, out string Msg)
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


                db.ExecuteNonQuerySP("SP_UserMomoRequest_Approve", param.ToArray());
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

        public void UserMomoRequest_UpdateOnlyStatus(long RequestID, int Status, out int Response, out string Msg, string ErrMsg = null)
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
                param[4] = new SqlParameter("@_ErrMsgFromPartner", SqlDbType.NVarChar);
                param[4].Size = 500;
                param[4].Value = ErrMsg ?? (object)DBNull.Value;

                db.ExecuteNonQuerySP("SP_UserMomoRequest_UpdateOnlyStatus", param.ToArray());
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

        public void UserMomoRequest_UpdateCallbackResult(QienCallbackParams p, out int Response, out string Msg)
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


                db.ExecuteNonQuerySP("SP_UserMomoRequest_UpdateCallbackResult", param.ToArray());
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