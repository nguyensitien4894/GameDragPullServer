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
    public class AccountDAO
    {
        private static readonly Lazy<AccountDAO> _instance = new Lazy<AccountDAO>(() => new AccountDAO());

        public static AccountDAO Instance
        {
            get { return _instance.Value; }
        }
        public void OTPSafeLogout( string Token, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

               
                param[0] = new SqlParameter("@_Token", SqlDbType.VarChar);
                param[0].Size = 64;
                param[0].Value = Token;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_OTPSafe_Logout", param.ToArray());
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

        public void CheckLoginOverTime(string PhoneOTP, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

  
             
                param[0] = new SqlParameter("@_PhoneOTP", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = PhoneOTP;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_OTPSafe_Check_Login_OverTime", param.ToArray());
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

        /// <summary>
        /// hàm login 
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="PhoneOTP"></param>
        /// <param name="OTP"></param>
        /// <param name="Token"></param>
        public void OTPSafeLogin(string PhoneOTP, string OTP, string Token, out int Response, out long  SafeID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[5];
                param[0] = new SqlParameter("@_PhoneOTP", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = PhoneOTP;
                param[1] = new SqlParameter("@_OTP", SqlDbType.VarChar);
                param[1].Size = 10;
                param[1].Value = OTP;
                param[2] = new SqlParameter("@_Token", SqlDbType.VarChar);
                param[2].Size = 64;
                param[2].Value = Token;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                

                db.ExecuteNonQuerySP("SP_OTPSafe_Login", param.ToArray());
                Response = ConvertUtil.ToInt(param[3].Value);
                SafeID = ConvertUtil.ToLong(param[4].Value);
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
            SafeID = 0;
            Response = -99;
        }
        public OTPSafeAuthen GetAccountByToken( string Token, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@_Token", SqlDbType.NVarChar);
                param[0].Size = 200;
                param[0].Value = Token;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;


                var _OTPSafeAuthen = db.GetInstanceSP<OTPSafeAuthen>("SP_OTPSafe_GetAccountByToken", param.ToArray());
                Response = ConvertUtil.ToInt(param[1].Value);
                return _OTPSafeAuthen;
           
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
            return null;
        }


        public void OTPSafeUpdate( long SafeID, string FirstName, string LastName, string SignalID, out bool Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[5];
                param[0] = new SqlParameter("@_SafeID", SqlDbType.BigInt);
               
                param[0].Value = SafeID;
                param[1] = new SqlParameter("@_FirstName", SqlDbType.NVarChar);
                param[1].Size = 50;
                param[1].Value = FirstName ?? (object)DBNull.Value;
                param[2] = new SqlParameter("@_LastName", SqlDbType.NVarChar);
                param[2].Size = 50;
                param[2].Value = LastName ?? (object)DBNull.Value;
                param[3] = new SqlParameter("@_SignalID", SqlDbType.VarChar);
                param[3].Size = 100;
                param[3].Value = SignalID??(object)DBNull.Value
                    ;
                param[4] = new SqlParameter("@_Response", SqlDbType.Bit);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_OTPSafe_Update", param.ToArray());
                Response = ConvertUtil.ToBool(param[4].Value);
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
            Response =false;
        }
    }
}