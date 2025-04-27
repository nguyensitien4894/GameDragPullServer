using MsWebGame.SafeOtp.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.SafeOtp.Database.DAO
{
    public class MessageDAO
    {
        private static readonly Lazy<MessageDAO> _instance = new Lazy<MessageDAO>(() => new MessageDAO());

        public static MessageDAO Instance
        {
            get { return _instance.Value; }
        }
        public void OTPSafeMessageCommand(int ServiceID, long SafeID, string Command, string Otp, out string ResponseMsg, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

               
               
                param[0] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
                param[0].Value = SafeID;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_Command", SqlDbType.VarChar);
                param[2].Size = 20;
                param[2].Value = Command;
                param[3] = new SqlParameter("@_Otp", SqlDbType.VarChar);
                param[3].Size = 10;
                param[3].Value = Otp;
                param[4] = new SqlParameter("@_ResponseMsg", SqlDbType.NVarChar);
                param[4].Size = 1000;
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_OTPSafeMessage_Command", param.ToArray());
                ResponseMsg = ConvertUtil.ToString(param[4].Value);
                Response = ConvertUtil.ToInt(param[5].Value);
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
            ResponseMsg = string.Empty;
        }

        public void OTPSafeMessageCheck(int ServiceID, bool IsRead, long SafeID, out string UnReadMsgCount)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[4];
                param[0] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
                param[0].Value = SafeID;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_IsRead", SqlDbType.Bit);
                param[2].Value = IsRead;
                param[3] = new SqlParameter("@_UnReadMsg", SqlDbType.VarChar);
                param[3].Size = 50;
                param[3].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_OTPSafeMessage_Check", param.ToArray());
                UnReadMsgCount = ConvertUtil.ToString(param[3].Value);
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
            UnReadMsgCount = string.Empty;
        }

        public void OTPSafeMessageDelete( long MessageID, long SafeID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[3];
                param[0] = new SqlParameter("@_MessageID", SqlDbType.BigInt);
                param[0].Value = MessageID;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                param[2] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
                param[2].Value = SafeID;
                db.ExecuteNonQuerySP("SP_OTPSafeMessage_Delete", param.ToArray());
                Response = ConvertUtil.ToInt(param[1].Value);
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
        public void OTPSafeMessageClear(int ServiceID ,long SafeID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
                param[0].Value = SafeID;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
    
                db.ExecuteNonQuerySP("SP_OTPSafeMessage_Clear", param.ToArray());
                Response = ConvertUtil.ToInt(param[2].Value);
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

        public List<OTPSafeMessage> OTPSafeMessageLastest(long SafeID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[1];
                param[0] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
                param[0].Value = SafeID;

                var _lstOTPSafeMessage = db.GetListSP<OTPSafeMessage>("SP_OTPSafeMessage_Lastest", param.ToArray());
                return _lstOTPSafeMessage;
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

        public List<OTPSafeMessage> GetList(int ServiceID, long SafeID, string Content,int page,int pageSize,out int  totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[0] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
                param[0].Value = SafeID;
                param[2] = new SqlParameter("@_Content", SqlDbType.NVarChar);
                param[2].Size = 1000;
                param[2].Value = Content;
                param[3] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[3].Value = page;
                param[4] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[4].Value = pageSize;
                param[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output; ;
               
   
                var _lstOTPSafeMessage = db.GetListSP<OTPSafeMessage>("SP_OTPSafeMessage_Search", param.ToArray());
                totalRecord = ConvertUtil.ToInt(param[5].Value);
                return _lstOTPSafeMessage;
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
    }
}