using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class AccountDAO
    {
        private static readonly Lazy<AccountDAO> _instance = new Lazy<AccountDAO>(() => new AccountDAO());
        public int DeletePhone(long accountId,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountId", accountId));
                param.Add(new SqlParameter("@_ServiceID", ServiceID));
                SqlParameter response = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_Account_DeletePhone", param.ToArray());

                return Convert.ToInt32(response.Value);
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

            return -99;
        }
        public static AccountDAO Instance
        {
            get { return _instance.Value; }
        }
        public int ChangeFbInfor(long accountId, string username, string Pass, string note, int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_UserID", accountId));
                param.Add(new SqlParameter("@_UserName", username));
                param.Add(new SqlParameter("@_Password", Pass));
                param.Add(new SqlParameter("@_ServiceID", ServiceID));
                param.Add(new SqlParameter("@_Note", note));

                SqlParameter response = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);
                db.ExecuteNonQuerySP("SP_Account_FB_Update_UserName", param.ToArray());

                return Convert.ToInt32(response.Value);
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
            return -99;
        }



        public void CheckAccountCheckExist(int Type, string Value, int SericeID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_Value", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = Value;
                param[1] = new SqlParameter("@_Type", SqlDbType.Int);
                param[1].Value = Type;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;

                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = SericeID;
                db.ExecuteNonQuerySP("SP_Account_Check_Exist", param.ToArray());
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

        public void Admin_UpdateLoginFail(string UserName,bool LoginStatus, out int Response,out int LoginFailNumber)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];

               
                param[1] = new SqlParameter("@_LoginStatus", SqlDbType.Bit);
                param[1].Value = LoginStatus;
                param[0] = new SqlParameter("@_UserName", SqlDbType.NVarChar);
                param[0].Size = 200;
                param[0].Value = UserName;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_LoginFailNumber", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                
                db.ExecuteNonQuerySP("SP_Admin_UpdateLoginFail", param.ToArray());
                Response = Convert.ToInt32(param[2].Value);
                LoginFailNumber= Convert.ToInt32(param[3].Value);
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
            LoginFailNumber = 0;
            Response = -99;
        }

        /// <summary>
        /// user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="loginType"></param>
        /// <param name="deviceType"></param>
        /// <param name="responseStatus"></param>
        /// <returns></returns>
        public Admin AdminLogin(string username, string password, out int responseStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_Username", username));
                param.Add(new SqlParameter("@_Password", password));

                SqlParameter response = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);
                Admin acc = db.GetInstanceSP<Admin>("SP_Admin_Login", param.ToArray());
                responseStatus = Convert.ToInt32(response.Value);
                return acc;
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

            responseStatus = -99;
            return null;
        }

       
        /// <summary>
        /// thêm mới tài khoản
        /// </summary>
        /// <param name="AccountName"></param>
        /// <param name="Password"></param>
        /// <param name="RoleID"></param>
        /// <param name="AccountID"></param>
        /// <param name="Response"></param>
        public void AccountCreate(string AccountName, string Password, int RoleID, out long AccountID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[5];

                param[0] = new SqlParameter("@_AccountName", SqlDbType.NVarChar);
                param[0].Size = 200;
                param[0].Value = AccountName;
                param[1] = new SqlParameter("@_Password", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = Password;
                param[2] = new SqlParameter("@_RoleID", SqlDbType.Int);
                param[2].Value = RoleID;
                param[3] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Account_Create", param.ToArray());
                AccountID = Convert.ToInt32(param[3].Value);
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
            AccountID = 0;
            Response = -99;
        }

        public List<AccountLoginIP> GetAccountLoginIP(long accountId, int top)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_TopCount", top);

                db = new DBHelper(Config.BettingLogConn);
                var rs = db.GetListSP<AccountLoginIP>("SP_AccountLoginIP_GetList", pars);
                return rs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public int UpdatePhone(long accountId, string phoneNumber, int ServiceID, long TeleId = 0)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountId", accountId));
                param.Add(new SqlParameter("@_PhoneNumber", (object)phoneNumber ?? DBNull.Value));
                param.Add(new SqlParameter("@_ServiceID", ServiceID));
                param.Add(new SqlParameter("@_TeleID", TeleId));
                SqlParameter response = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_Account_UpdatePhone", param.ToArray());

                return Convert.ToInt32(response.Value);
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

            return -99;
        }
        public string GetTelegramId(string phone)
        {
            DBHelper db = null;
            try
            {
                
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@_PhoneOTP", phone);
                param[1] = new SqlParameter("@_TelegramId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Users_GetProfile_Tele", param.ToArray());

                return ConvertUtil.ToString(param[1].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return "0";
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }
    }
}