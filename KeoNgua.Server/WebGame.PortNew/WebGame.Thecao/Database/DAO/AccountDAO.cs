using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;
using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class AccountDAO
    {
        private static readonly Lazy<AccountDAO> _instance = new Lazy<AccountDAO>(() => new AccountDAO());

        public static AccountDAO Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// tạo tài khoản  cho toàn hệ thống
        /// </summary>
        /// <param name="createType"></param>
        /// <param name="deviceType"></param>
        /// <param name="username"></param>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <param name="clientIp"></param>
        /// <param name="responseStatus"></param>
        /// <returns>account login infor</returns>
        public Account CreateAccount(long UserID,int createType, int deviceType, string username, string password, string clientIp,int status ,int AccountType,string PhoneNumber,string GameAccountName, out int responseStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_Type", createType));
                param.Add(new SqlParameter("@_DeviceType", deviceType));
                param.Add(new SqlParameter("@_Username", username));
                param.Add(new SqlParameter("@_Password", password));
                param.Add(new SqlParameter("@_IP", clientIp));
                param.Add(new SqlParameter("@_UserID", UserID));
                param.Add(new SqlParameter("@_Status", status));
                param.Add(new SqlParameter("@_PhoneNumber", PhoneNumber??(object)DBNull.Value));
                param.Add(new SqlParameter("@_AccountType", AccountType));
                param.Add(new SqlParameter("@_GameAccountName", GameAccountName??(object)DBNull.Value));
                SqlParameter response = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                Account acc = db.GetInstanceSP<Account>("SP_Account_Create", param.ToArray());
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
        /// user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="loginType"></param>
        /// <param name="deviceType"></param>
        /// <param name="responseStatus"></param>
        /// <returns></returns>
        public Account UserLogin(string username, string password, int loginType, int deviceType, out int responseStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_Username", username));
                param.Add(new SqlParameter("@_Password", password));
                param.Add(new SqlParameter("@_LoginType", loginType));
                param.Add(new SqlParameter("@_DeviceType", deviceType));

                SqlParameter response = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                Account acc = db.GetInstanceSP<Account>("SP_Account_UserLogin", param.ToArray());
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
        /// update nick name
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="UserID"></param>
        /// <param name="NickName"></param>
        public Account AccountUpdateNickName( long UserID, string NickName,out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_NickName", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = NickName;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;

                var account=db.GetInstanceSP<Account>("SP_Account_Update_NickName", param.ToArray());
                Response = Convert.ToInt32(param[2].Value);
                return account;
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

        /// <summary>
        /// get account profile
        /// </summary>
        /// <param name="accountId">long</param>
        /// <returns></returns>
        public Account GetProfile(long accountId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountId));
                return db.GetInstanceSP<Account>("SP_Account_GetAccountInfo", param.ToArray());
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
        /// ghi lại log khi login
        /// </summary>
        /// <param name="LoginType"></param>
        /// <param name="AccountID"></param>
        /// <param name="DeviceType"></param>
        /// <param name="Response"></param>
        public void LoginLog(int LoginType, long AccountID, long DeviceType, string ClientIP, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_DeviceType", SqlDbType.BigInt);
                param[1].Value = DeviceType;
                param[2] = new SqlParameter("@_LoginType", SqlDbType.Int);
                param[2].Value = LoginType;
                param[3] = new SqlParameter("@_ClientIP", SqlDbType.NVarChar);
                param[3].Size = 500;
                param[3].Value = ClientIP;
                param[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_History_UserLogin", param.ToArray());
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

        /// <summary>
        /// get account info
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Account GetAccountInfo(long accountId,string username,string phone)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountId));
                param.Add(new SqlParameter("@_Phone", phone));
                param.Add(new SqlParameter("@_UserName", username));
                return db.GetInstanceSP<Account>("SP_Account_GetAccountInfo2", param.ToArray());
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
        /// cập nhật thông tin  người dùng
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="accountName"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="gender"></param>
        /// <param name="ava"></param>
        /// <returns></returns>
        public Account UpdateProfile(long accountID, string accountName, string dateOfBirth, string phoneNumber, int gender, int ava,int? AuthenType,string Otp, out int response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountID));
                if (!string.IsNullOrEmpty(accountName))
                {
                    param.Add(new SqlParameter("@_AccountName", accountName));
                }
                if (!string.IsNullOrEmpty(dateOfBirth))
                {
                    param.Add(new SqlParameter("@_DateOfBirth", dateOfBirth));
                }
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    param.Add(new SqlParameter("@_PhoneNumber", phoneNumber));
                }
                if (gender > 0)
                {
                    param.Add(new SqlParameter("@_Gender", gender));
                }
                if (ava > 0)
                {
                    param.Add(new SqlParameter("@_Avatar", ava));
                }
                if (AuthenType.HasValue)
                {
                    param.Add(new SqlParameter("@_AuthenType", AuthenType));
                }
                if (!String.IsNullOrEmpty(Otp))
                {
                    param.Add(new SqlParameter("@_Otp", Otp));
                }
                SqlParameter res = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(res);
                var accountInfo= db.GetInstanceSP<Account>("SP_Account_UpdateProfile", param.ToArray());
                response = Convert.ToInt32(res.Value);
                return accountInfo;
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
            response = -99;
            return null;
        }
        /// <summary>
        /// update avatar
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="ava"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Account UpdateAvatar(long accountID, int ava, out int response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountID));
                param.Add(new SqlParameter("@_Avatar", ava));
                SqlParameter res = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(res);
                var accountInfo = db.GetInstanceSP<Account>("SP_Account_UpdateProfile", param.ToArray());
                response = Convert.ToInt32(res.Value);
                return accountInfo;
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
            response = -99;
            return null;
        }


        //update authen
        public Account UpdateAuthen(long accountID, int AuthenType, out int response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountID));
                
                 param.Add(new SqlParameter("@_AuthenType", AuthenType));
               
                SqlParameter res = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(res);
                var accountInfo = db.GetInstanceSP<Account>("SP_Account_UpdateAuthen", param.ToArray());
                response = Convert.ToInt32(res.Value);
                return accountInfo;
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
            response = -99;
            return null;
        }
        /// <summary>
        /// thay đổi mật khẩu của user login 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="oldPass"></param>
        /// <param name="newPass"></param>
        ///<param name="Otp"></param>
        /// <returns> 1 for success else fail</returns>
        public int ChangePassword(long accountId, string oldPass, string newPass,string Otp)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountId", accountId));
                param.Add(new SqlParameter("@_Password", oldPass));
                param.Add(new SqlParameter("@_Newpassword", newPass));
                param.Add(new SqlParameter("@_Otp", Otp??(object)DBNull.Value));
                SqlParameter response = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_Account_ChangePassword", param.ToArray());

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
        /// <summary>
        /// cập nhật mã otp
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="Otp"></param>
        /// <returns></returns>
        public int UpdatePhone(long accountId, string phoneNumber)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountId", accountId));
                param.Add(new SqlParameter("@_PhoneNumber", (object)phoneNumber ?? DBNull.Value));
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
        public int DeletePhone(long accountId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountId", accountId));
              
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
        
        /// <summary>
        /// kiểm tra xem số điện thoại này đã được sử dụng hay chưa
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public int CheckAccountPhone(long accountId, string phoneNumber)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountId", accountId));
                param.Add(new SqlParameter("@_PhoneNumber", phoneNumber));
                SqlParameter response = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                param.Add(response);
                db.ExecuteNonQuerySP("SP_Account_CheckPhone", param.ToArray());

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

        /// <summary>
        /// get account balance
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="Balance"></param>
        public void GetBalance(long AccountId, out long Balance,out long safebalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_AccountId", SqlDbType.BigInt);
                param[0].Value = AccountId;
                param[1] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[1].Direction = ParameterDirection.Output;

                param[2] = new SqlParameter("@_SafeBalance", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;
                
                db.ExecuteNonQuerySP("SP_Account_GetAllBalance", param.ToArray());
                Balance = Convert.ToInt64(param[1].Value);
                safebalance = Convert.ToInt64(param[2].Value);
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
            safebalance = 0;
        }


        public int OTPActive(string phone, string otp)
        {
            DBHelper db = null;
            int responseStatus;
            try
            {
                if (String.IsNullOrEmpty(phone) || string.IsNullOrEmpty(otp)) return -1;

                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_Phone", phone));
                param.Add(new SqlParameter("@_Otp", otp));
                SqlParameter rStatus = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(rStatus);
               

                db.ExecuteNonQuerySP("SP_OTP_CheckValid", param.ToArray());
                responseStatus = Convert.ToInt32(rStatus.Value);

                return responseStatus;
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
            responseStatus = -1;
            return responseStatus;
        }

        /// <summary>
        /// update account name
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="AccountID"></param>
        /// <param name="AccountName"></param>
        //public void UpdateAccountName(out int Response, long AccountID, string AccountName)
        //{
        //    DBHelper db = null;
        //    try
        //    {
        //        db = new DBHelper(Config.BettingConn);
        //        SqlParameter[] param = new SqlParameter[3];

        //        param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
        //        param[0].Value = AccountID;
        //        param[1] = new SqlParameter("@_AccountName", SqlDbType.NVarChar);
        //        param[1].Size = 100;
        //        param[1].Value = AccountName;
        //        param[2] = new SqlParameter("@_Response", SqlDbType.Int);
        //        param[2].Direction = ParameterDirection.Output;


        //        db.ExecuteNonQuerySP("SP_Account_UpdateGameAccountName", param.ToArray());
        //        Response = Convert.ToInt32(param[2].Value);
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //    }
        //    finally
        //    {
        //        if (db != null)
        //        {
        //            db.Close();
        //        }
        //    }
        //    Response = -99;
        //}

        /// <summary>
        /// forgot password
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="Password"></param>
        /// <param name="Phone"></param>
        /// <param name="Otp"></param>
        /// <param name="Response"></param>
        public void ForgotPassword( long AccountId, string Password, string Phone, string Otp, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
       
                param[0] = new SqlParameter("@_AccountId", SqlDbType.BigInt);
                param[0].Value = AccountId;
                param[1] = new SqlParameter("@_Password", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = Password;
                param[2] = new SqlParameter("@_Phone", SqlDbType.NVarChar);
                param[2].Size = 40;
                param[2].Value = Phone;
                param[3] = new SqlParameter("@_Otp", SqlDbType.NVarChar);
                param[3].Size = 40;
                param[3].Value = Otp;
                param[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Account_ForgotPassword", param.ToArray());

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

        /// <summary>
        /// get current token
        /// </summary>
        /// <param name="userId">userId long</param>
        /// <param name="token">current token</param>
        /// <returns>obj UserToken </returns>
        public UserToken GetUserToken(long userId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_UserID", userId));
               

                return db.GetInstanceSP<UserToken>("SP_GetUserToken", param.ToArray());
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
        /// update current user token
        /// </summary>
        /// <param name="userId">user Id </param>
        /// <param name="token">token </param>
        /// <param name="deviceType">android 3,  ios 2,windowphone 4,web 1</param>
        /// <param name="deviceId">deviceId </param>
        /// <returns>1 success else fail</returns>
        public int UpdateToken(long userId, string token,string deviceId,int deviceType)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
               
                param.Add(new SqlParameter("@_UserID", userId));
                param.Add(new SqlParameter("@_DeviceId", deviceId));
                param.Add(new SqlParameter("@_DeviceType", deviceType));
                param.Add(new SqlParameter("@_Token", token));
                SqlParameter response = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_Update_Token", param.ToArray());

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
        /// <summary>
        /// delete token when sign out
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int DeleteToken(long userId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();

                param.Add(new SqlParameter("@_UserID", userId));
               
                SqlParameter response = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_Delete_Token", param.ToArray());

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


        /// <summary>
        /// add tiền vào két sắt
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="deviceType"></param>
        /// <param name="clientIP"></param>
        /// <param name="Amount"></param>
        /// <param name="clientIp"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public AccountSafeInfo AddToSafe(long accountId, int deviceType, string clientIP, long Amount)
        {
            DBHelper db = null;
            AccountSafeInfo _AccountSafeInfo = new AccountSafeInfo();
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountId));
                param.Add(new SqlParameter("@_SourceID", deviceType));
                param.Add(new SqlParameter("@_ClientIP", clientIP));
                param.Add(new SqlParameter("@_Amount", Amount));
               
                SqlParameter response = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);
                SqlParameter responseSafeBalance = new SqlParameter("@_SafeBalance", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(responseSafeBalance);
                SqlParameter responseBalance = new SqlParameter("@_Balance ", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(responseBalance);
                SqlParameter responseSessionID = new SqlParameter("@_SessionID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(responseSessionID);

                db.ExecuteNonQuerySP("SP_TransferBalanceToSafeBalance", param.ToArray());
                
                _AccountSafeInfo.responseStatus = Convert.ToInt32(response.Value);
                _AccountSafeInfo.SafeBalance = Convert.ToInt64(responseSafeBalance.Value==DBNull.Value?0: responseSafeBalance.Value);
                _AccountSafeInfo.Balance = Convert.ToInt64(responseBalance.Value==DBNull.Value?0: responseBalance.Value);
                _AccountSafeInfo.SessionID = Convert.ToInt64(responseSessionID.Value == DBNull.Value ? 0 : responseSessionID.Value);
                return _AccountSafeInfo;

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

            _AccountSafeInfo.responseStatus = -99;
            return _AccountSafeInfo;


        }

        /// <summary>
        /// rút  tiền vào két sắt
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="deviceType"></param>
        /// <param name="clientIP"></param>
        /// <param name="Amount"></param>
        /// <param name="clientIp"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public AccountSafeInfo WithDrawFromSafe(long accountId, int deviceType, string clientIP, long Amount, string otp)
        {
            DBHelper db = null;
            AccountSafeInfo _AccountSafeInfo = new AccountSafeInfo();
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountID", accountId));
                param.Add(new SqlParameter("@_SourceID", deviceType));
                param.Add(new SqlParameter("@_ClientIP", clientIP));
                param.Add(new SqlParameter("@_Amount", Amount));
                param.Add(new SqlParameter("@_OTP", otp??(object)DBNull.Value));
                SqlParameter response = new SqlParameter("@_Response", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);
                SqlParameter responseSafeBalance = new SqlParameter("@_SafeBalance", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(responseSafeBalance);
                SqlParameter responseBalance = new SqlParameter("@_Balance ", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(responseBalance);

                SqlParameter responseSessionID = new SqlParameter("@_SessionID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(responseSessionID);

                db.ExecuteNonQuerySP("SP_WithdrawSafeBalanceToBalance", param.ToArray());

                _AccountSafeInfo.responseStatus = Convert.ToInt32(response.Value);
                _AccountSafeInfo.SafeBalance = Convert.ToInt64(responseSafeBalance.Value==DBNull.Value?0: responseSafeBalance.Value);
                _AccountSafeInfo.Balance = Convert.ToInt64(responseBalance.Value== DBNull.Value ? 0:responseBalance.Value);
                _AccountSafeInfo.SessionID= Convert.ToInt64(responseSessionID.Value == DBNull.Value ? 0 : responseSessionID.Value);
                return _AccountSafeInfo;

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

            _AccountSafeInfo.responseStatus = -99;
            return _AccountSafeInfo;


        }
        /// <summary>
        /// update ip check
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Response"></param>
        public void IPCheck(string IP, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@_IP", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = IP;
                param[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                
                db.ExecuteNonQuerySP("SP_IP_Check", param.ToArray());
                Response = Convert.ToInt32(param[1].Value);
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
        /// get usser balance
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BalanceType"></param>
        /// <param name="Amount"></param>
        /// <param name="RemainBalance"></param>
        public void UserBalanceExchange( long UserId, int BalanceType, long Amount, out long RemainBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[4];
                param[2] = new SqlParameter("@_BalanceType", SqlDbType.Int);
                param[2].Value = BalanceType;
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = UserId;
                param[1] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[1].Value = Amount;
                param[3] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_UserBalance_Exchange", param.ToArray());
                RemainBalance = Convert.ToInt64(param[3].Value);
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
            RemainBalance = 0;
        }
        /// <summary>
        /// get user rank
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        public UserRank GetUserRank(long UserID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
             new SqlParameter[2];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;

                var _UserRank = db.GetInstanceSP<UserRank>("SP_User_Get_Rank", param.ToArray());
                Response = Convert.ToInt32(param[1].Value);
                return _UserRank;
               
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
        /// <summary>
        /// check account exist
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        /// <param name="Response"></param>
        public void CheckAccountCheckExist(int Type, string Value, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_Value", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = Value;
                param[1] = new SqlParameter("@_Type", SqlDbType.Int);
                param[1].Value = Type;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
              

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
      

    }
}