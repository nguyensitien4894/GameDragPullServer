using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.SafeOtp.Database.DAO
{
    public class OtpDAO
    {

        private static readonly Lazy<OtpDAO> _instance = new Lazy<OtpDAO>(() => new OtpDAO());

        public static OtpDAO Instance
        {
            get { return _instance.Value; }
        }
        public void SmsOTPInsert(int ServiceID, long UserID, string Type, string Otp, string Msisdn,int Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@_Type", SqlDbType.NVarChar);
                param[0].Size = 20;
                param[0].Value = Type;
                param[1] = new SqlParameter("@_Otp", SqlDbType.NVarChar);
                param[1].Size = 40;
                param[1].Value = Otp;
                param[2] = new SqlParameter("@_Msisdn", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = Msisdn;
                param[3] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[3].Value = UserID;
                param[4] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[4].Value = ServiceID;
               
                param[5] = new SqlParameter("@_Response", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_Status", SqlDbType.Int);
                param[6].Value = Status;
                db.ExecuteNonQuerySP("SP_SmsOTP_Insert", param.ToArray());
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
        }

    }
}