using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class AccountProfileDAO
    {
        private static readonly Lazy<AccountProfileDAO> _instance = new Lazy<AccountProfileDAO>(() => new AccountProfileDAO());

        public static AccountProfileDAO Instance
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
        public Account CreateAccount(long UserID, int createType, int deviceType, string username, string password, string clientIp, int status,
            int AccountType, string PhoneNumber, string GameAccountName, out int responseStatus)
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
                param.Add(new SqlParameter("@_PhoneNumber", PhoneNumber ?? (object)DBNull.Value));
                param.Add(new SqlParameter("@_AccountType", AccountType));
                param.Add(new SqlParameter("@_GameAccountName", GameAccountName ?? (object)DBNull.Value));
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

        public void CheckAccountCheckExist(int type, string value, int serviceid, out int response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_Value", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = value;
                param[1] = new SqlParameter("@_Type", SqlDbType.Int);
                param[1].Value = type;
                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = serviceid;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Account_Check_Exist", param.ToArray());
                response = Convert.ToInt32(param[3].Value);
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

            response = -99;
        }
        public Account GetAccountInfor(long AccountID, string Phone, string UserName, int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];

                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_Phone", SqlDbType.NVarChar);
                param[1].Size = 40;
                param[1].Value = Phone;
                param[2] = new SqlParameter("@_UserName", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = UserName;
                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;
                var _Account = db.GetInstanceSP<Account>("SP_Account_GetAccountInfo", param.ToArray());
                return _Account;
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

        public int CheckAccountPhone(long accountId, string phoneNumber, int serviceid)
        {
            DBHelper db = null;
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_AccountId", accountId));
                param.Add(new SqlParameter("@_PhoneNumber", phoneNumber));
                param.Add(new SqlParameter("@_ServiceID", serviceid));
                SqlParameter response = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Account_CheckPhone2", param.ToArray());
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



        public Account UpdateProfile(long accountID, string accountName, string dateOfBirth, string phoneNumber, int gender, int ava, int? AuthenType, string Otp, out int response)
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

        public int UpdateProfile(long UserID, string UserDisplayName, string Avatar, string PhoneOTP, string Email, string Password)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_UserDisplayName", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = UserDisplayName;
                param[2] = new SqlParameter("@_Avatar", SqlDbType.VarChar);
                param[2].Size = 100;
                param[2].Value = Avatar;
                param[3] = new SqlParameter("@_PhoneOTP", SqlDbType.VarChar);
                param[3].Size = 100;
                param[3].Value = PhoneOTP;
                param[4] = new SqlParameter("@_Email", SqlDbType.VarChar);
                param[4].Size = 100;
                param[4].Value = Email;
                param[5] = new SqlParameter("@_Password", SqlDbType.VarChar);
                param[5].Size = 100;
                param[5].Value = Password;
                param[6] = new SqlParameter("@_Response", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Users_UpdateProfile", param.ToArray());
                return ConvertUtil.ToInt(param[6].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
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