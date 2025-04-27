using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database
{
    public class SafeOtpDAO
    {
        private static readonly Lazy<SafeOtpDAO> _instance = new Lazy<SafeOtpDAO>(() => new SafeOtpDAO());

        public static SafeOtpDAO Instance
        {
            get { return _instance.Value; }
        }
        public void OTPSafeMessageSend(int ServiceID, string PhoneSafeNo, string Message, out int Response, out long MsgID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_PhoneSafeNo", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = PhoneSafeNo;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_Message", SqlDbType.NVarChar);
                param[2].Size = 1000;
                param[2].Value = Message;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_MsgID", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
               
    
                db.ExecuteNonQuerySP("SP_OTPSafeMessage_Send", param.ToArray());
                MsgID = ConvertUtil.ToLong(param[3].Value);
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
            MsgID = 0;
            Response = -99;
        }
      
    }
}