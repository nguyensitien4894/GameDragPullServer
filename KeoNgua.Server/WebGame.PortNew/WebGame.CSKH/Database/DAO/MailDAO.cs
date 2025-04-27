using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class MailDAO
    {
        private static readonly Lazy<MailDAO> _instance = new Lazy<MailDAO>(() => new MailDAO());

        public static MailDAO Instance
        {
            get { return _instance.Value; }
        }

        public void SendMail(long SenderID, long ReceiverID, string Title, string Content, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_SenderID", SqlDbType.BigInt);
                param[0].Value = SenderID;
                param[1] = new SqlParameter("@_ReceiverID", SqlDbType.BigInt);
                param[1].Value = ReceiverID;
                param[2] = new SqlParameter("@_Title", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = Title;
                param[3] = new SqlParameter("@_Content", SqlDbType.NVarChar);
                param[3].Size = 8000;
                param[3].Value = Content;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Portal_SendMail", param.ToArray());
                Response = Convert.ToInt32(param[4].Value);
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

        public int SystemSendMailToUser(long senderId, long receiverId, string receiverName, string title, string content, int status, DateTime createdTime, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_SenderID", senderId);
                pars[1] = new SqlParameter("@_ReceiverID", receiverId);
                pars[2] = new SqlParameter("@_ReceiverName", receiverName);
                pars[3] = new SqlParameter("@_Title", title);
                pars[4] = new SqlParameter("@_Content", content);
                pars[5] = new SqlParameter("@_Status", status);
                pars[6] = new SqlParameter("@_CreatedTime", createdTime);
                pars[7] = new SqlParameter("@_ServiceID", serviceid);
                pars[8] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_System_SendMail", pars);
                return ConvertUtil.ToInt(pars[8].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public void UpdateMail(long AccountID, long MailID, string Title, string Content, int? Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_MailID", SqlDbType.BigInt);
                param[1].Value = MailID;
                param[2] = new SqlParameter("@_Title", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = Title;
                param[3] = new SqlParameter("@_Content", SqlDbType.NVarChar);
                param[3].Size = 8000;
                param[3].Value = Content;
                param[4] = new SqlParameter("@_Status", SqlDbType.Int);
                param[4].Value = Status.HasValue ? Status.Value : (object)DBNull.Value;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Portal_UpdateMail", param.ToArray());
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

        public List<Mail> GetList(long ID, string ReceiverName, DateTime? FromDate, DateTime? Todate, int serviceid, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                pars[0].Value = ID;
                pars[1] = new SqlParameter("@_ReceiverName", SqlDbType.NVarChar);
                pars[1].Size = 100;
                pars[1].Value = ReceiverName;
                pars[2] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                pars[2].Value = FromDate;
                pars[3] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                pars[3].Value = Todate;
                pars[4] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                pars[4].Value = serviceid;
                pars[5] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                pars[5].Value = CurrentPage;
                pars[6] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                pars[6].Value = RecordPerpage;
                pars[7] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                pars[7].Direction = ParameterDirection.Output;

                db = new DBHelper(Config.BettingConn);
                var _lstMail = db.GetListSP<Mail>("SP_Mail_List", pars);
                TotalRecord = ConvertUtil.ToInt(pars[7].Value);
                return _lstMail;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            TotalRecord = 0;
            return null;
        }
    }
}