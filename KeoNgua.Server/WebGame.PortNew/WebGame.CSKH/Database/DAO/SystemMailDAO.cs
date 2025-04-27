using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class SystemMailDAO
    {
        private static readonly Lazy<SystemMailDAO> _instance = new Lazy<SystemMailDAO>(() => new SystemMailDAO());

        public static SystemMailDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<SystemMail> GetSystemMailList(int id,int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[4];

                param[0] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[0].Value = CurrentPage;
                param[1] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[1].Value = RecordPerpage;
                param[2] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[3].Value = id;
                var _lstSystemMail = db.GetListSP<SystemMail>("SP_Portal_GetSystemMail_Admin", param.ToArray());
                TotalRecord = Convert.ToInt32(param[2].Value);
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
            TotalRecord = 0;
            return null;
        }


        public void SendMail( string Title, string Content, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_Title", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = Title;
                param[1] = new SqlParameter("@_Content", SqlDbType.NVarChar);
                param[1].Size = 8000;
                param[1].Value = Content;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Portal_Send_SystemMail", param.ToArray());
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



        public void SystemMailUpdate( long MailID, string Title, string Content, bool Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =   new SqlParameter[5];
                param[0] = new SqlParameter("@_MailID", SqlDbType.BigInt);
                param[0].Value = MailID;
                param[1] = new SqlParameter("@_Title", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = Title;
                param[2] = new SqlParameter("@_Content", SqlDbType.NVarChar);
                param[2].Size = 8000;
                param[2].Value = Content;
                param[3] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[3].Value = Status;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_SystemMail_Update", param.ToArray());
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

    }
}