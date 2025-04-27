using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;
using MsWebGame.Thecao.Helpers.OTPs.OtpAps;

namespace WebGame.Payment.Database.DAO
{
    public class SMSDAO
    {
        private static readonly Lazy<SMSDAO> _instance = new Lazy<SMSDAO>(() => new SMSDAO());

        public static SMSDAO Instance
        {
            get { return _instance.Value; }
        }


        public void OTPInit( long UserID, string Type, string Otp, string Msisdn,out long Balance, out long CurrentBalance,out long Response,out int OtpFeePerTime)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =   new SqlParameter[8];

                
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
                param[4] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_CurrentBalance", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_Response", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_OtpFeePerTime", SqlDbType.Float);
                param[7].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_AZTechOTP_Insert", param.ToArray());
                Response = Convert.ToInt32(param[6].Value);
                if (Response >0)
                {
                    Balance = Convert.ToInt64(param[4].Value);
                    CurrentBalance = Convert.ToInt64(param[5].Value);
                    OtpFeePerTime = Convert.ToInt32(param[7].Value);
                }
                else
                {
                    Balance = 0;
                    CurrentBalance = 0;
                    OtpFeePerTime = 0;
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
            Balance = 0;
            CurrentBalance = 0;
            OtpFeePerTime = 0;
            Response = -99;
        }


        public void UpdateStatus(long RequestId ,int Active, int Status, string RequestTime, out int Response, out int OtpFeePerTime, out long Balance, out long CurrentBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];
                param[0] = new SqlParameter("@_RequestId", SqlDbType.BigInt);
                param[0].Value = RequestId;
                param[1] = new SqlParameter("@_RequestTime", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = RequestTime;
                param[2] = new SqlParameter("@_Active", SqlDbType.Int);
                param[2].Value = Active;
                param[3] = new SqlParameter("@_Status", SqlDbType.Int);
                param[3].Value = Status;
                param[4] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_OtpFeePerTime", SqlDbType.Float);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_CurrentBalance", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_AZTechOTP_UpdateStatus", param.ToArray());
                Response = Convert.ToInt32(param[7].Value);
                if (Response == 1)
                {
                    Balance= Convert.ToInt64(param[4].Value);
                    OtpFeePerTime = Convert.ToInt32(param[5].Value);
                    CurrentBalance = Convert.ToInt64(param[6].Value);
                }else
                {
                    Balance = 0;
                    OtpFeePerTime = 0;
                    CurrentBalance = 0;
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
            Balance = 0;
            OtpFeePerTime = 0;
            CurrentBalance = 0;
            Response = -99;
        }
        /// <summary>
        /// check valid otp
        /// </summary>
        /// <param name="AccountID"></param>
        /// <param name="Phone"></param>
        /// <param name="Otp"></param>
        /// <param name="Response"></param>
        public void ValidOtp( long AccountID, string Phone, string Otp, int ServiceID, out int Response,out long OtpID,out string msg)
        {
            Otp = Otp.Trim();
            msg = string.Empty;
            if (Otp.Length != 6 && Otp.Length != 7)
            {
                Response = -1;
                OtpID = 0;
                return;
            }
            //kiểm tra otp in app
            if (Otp.Length == 6)
            {

                var res = OtpAppVerifyApi.SendRequest(AccountID, Otp);
                OtpID = 0;
                if (res!=null&&res.ResponseCode == 1)
                {
                    Response = 1;
                 
                }
                else
                {
                    Response = res.ResponseCode;
                    msg = res.Message;
                }
               
            
                return;
            }

            //

            DBHelper db = null;
            Otp = Otp.ToUpper();
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_Phone", SqlDbType.NVarChar);
                param[1].Size = 30;
                param[1].Value = Phone;
                param[2] = new SqlParameter("@_Otp", SqlDbType.NVarChar);
                param[2].Size = 40;
                param[2].Value = Otp;


                param[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_OtpID", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[5].Value = ServiceID;



                db.ExecuteNonQuerySP("SP_OTP_CheckValid_AZTech", param.ToArray());
                Response = Convert.ToInt32(param[3].Value);
                OtpID = Convert.ToInt64(param[4].Value==DBNull.Value?0:param[4].Value);
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
            OtpID = 0;
            Response = -99;
        }
        public void OtpDeactive( long RequestId, long AccountID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                
                param[0] = new SqlParameter("@_RequestId", SqlDbType.BigInt);
                param[0].Value = RequestId;
                param[1] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[1].Value = AccountID;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_AZTechOTP_Deactive", param.ToArray());
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