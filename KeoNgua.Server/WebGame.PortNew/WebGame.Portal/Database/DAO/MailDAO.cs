using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class MailDAO
    {
        private static readonly Lazy<MailDAO> _instance = new Lazy<MailDAO>(() => new MailDAO());

        public static MailDAO Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        public List<Mail> GetMail(int MailType, int CurrentPage, int RecordPerpage, out int TotalRecord, long AccountID,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[6];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_MailType", SqlDbType.Int);
                param[1].Value = MailType;
                param[2] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[2].Value = CurrentPage;
                param[3] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[3].Value = RecordPerpage;
                param[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[5].Value = ServiceID;
                

                var _lstMail = db.GetListSP<Mail>("SP_Portal_GetMail", param.ToArray());
                TotalRecord = Convert.ToInt32(param[4].Value);
                return _lstMail;
               
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
        /// send mail
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="AccountID"></param>
        /// <param name="Title"></param>
        /// <param name="Content"></param>
        public void SendMail(out int Response, long AccountID, string Title, string Content)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[4];

               
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_Title", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = Title;
                param[2] = new SqlParameter("@_Content", SqlDbType.NVarChar);
                param[2].Size = 8000;
                param[2].Value = Content;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Portal_SendMail", param.ToArray());
                Response = Convert.ToInt32(param[3].Value);
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
        /// update read mail
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="AccountID"></param>
        /// <param name="MailID"></param>
        public void UpdateStatus(long AccountID, long MailID,int Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[4];

               
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_MailID", SqlDbType.BigInt);
                param[1].Value = MailID;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_Status", SqlDbType.Int);
                param[3].Value = Status;
                db.ExecuteNonQuerySP("SP_Portal_UpdateMail", param.ToArray());
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

        public void UpdateMailReading( long MailID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_MailID", SqlDbType.BigInt);
                param[0].Value = MailID;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
               
                db.ExecuteNonQuerySP("SP_Portal_UpdateMail_Reading", param.ToArray());
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

        /// <summary>
        /// get system email
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<SystemMail> GetSystemMail(int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[0].Value = ServiceID;
                
                var _lstSystemMail = db.GetListSP<SystemMail>("SP_Portal_GetSystemMail");
                return _lstSystemMail;
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
        /// cập nhật  read mail
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="AccountID"></param>
        /// <param name="MailID"></param>
        public void UpdateReadMail(out int Response, long AccountID, long MailID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_MailID", SqlDbType.BigInt);
                param[1].Value = MailID;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Portal_UpdateReadMail", param.ToArray());
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
        public void MailUnReadCnt(long AccountID,out int Total)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[2];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_Total", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                

                db.ExecuteNonQuerySP("SP_Portal_MailUnRead_Cnt", param.ToArray());
                Total = Convert.ToInt32(param[1].Value);
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
            Total = 0;
        }

    }
}