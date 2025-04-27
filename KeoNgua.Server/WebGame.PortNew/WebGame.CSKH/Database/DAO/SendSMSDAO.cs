using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class SendSMSDAO
    {
        private static readonly Lazy<SendSMSDAO> _instance = new Lazy<SendSMSDAO>(() => new SendSMSDAO());

        public static SendSMSDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<SendSMS> GetList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var _lstAdmin = db.GetListSP<SendSMS>("SP_SendSMS_UserPrivilegeNotify_List");
                return _lstAdmin;
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



        public void UpdateResultSendSms(DataTable listPre, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_UserPrivNotTable", SqlDbType.Structured);
                param[0].Value = listPre;
                param[0].TypeName = "dbo.TmpUserPrivNotify";
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
               
                var _lstGetList = db.ExecuteNonQuerySP("SP_UserPrivilege_SmsResponse", param.ToArray());
                Response = Convert.ToInt32(param[1].Value);
               

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
            Response = 0;
           
        }


        /// <summary>
        /// send card Notify
        /// </summary>
        /// <param name="TeleID"></param>
        /// <param name="Response"></param>
        public void CardNotify(int TeleID, out string Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_TeleID", SqlDbType.Int);
                param[0].Value = TeleID;
                param[1] = new SqlParameter("@_Response", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Card_Notify", param.ToArray());
                Response = param[1].Value.ToString();
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
            Response = null;
        }
    }
}