using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Param;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class UserDAO
    {
        private static readonly Lazy<UserDAO> _instance = new Lazy<UserDAO>(() => new UserDAO());
        public void VIPCheckQuaterLoan(long UserID, DateTime StartQuaterTime, DateTime EndQuaterTime, out int RankID, out int VPQuaterAcc, out int VipQuaterCoeff, out int QuaterPrizeStatus, out float LoanLimit, out float LoanRate, out long QuaterAcc, out long LoanAmount, out long OldDebt)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_StartQuaterTime", SqlDbType.DateTime);
                param[1].Value = StartQuaterTime;
                param[2] = new SqlParameter("@_EndQuaterTime", SqlDbType.DateTime);
                param[2].Value = EndQuaterTime;
                param[3] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_VPQuaterAcc", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_VipQuaterCoeff", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;


                param[6] = new SqlParameter("@_LoanLimit", SqlDbType.Float);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_LoanRate", SqlDbType.Float);
                param[7].Direction = ParameterDirection.Output;

                param[8] = new SqlParameter("@_QuaterAcc", SqlDbType.BigInt);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_LoanAmount", SqlDbType.BigInt);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@_OldDebt", SqlDbType.BigInt);
                param[10].Direction = ParameterDirection.Output;
                param[11] = new SqlParameter("@_QuaterPrizeStatus", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_Check_QuaterLoan", param.ToArray());
                RankID = ConvertUtil.ToInt(param[3].Value);
                VPQuaterAcc = ConvertUtil.ToInt(param[4].Value);
                VipQuaterCoeff = ConvertUtil.ToInt(param[5].Value);
                LoanLimit = ConvertUtil.ToFloat(param[6].Value);
                LoanRate = ConvertUtil.ToFloat(param[7].Value);
                QuaterAcc = ConvertUtil.ToLong(param[8].Value);
                LoanAmount = ConvertUtil.ToLong(param[9].Value);
                OldDebt = ConvertUtil.ToLong(param[10].Value);
                QuaterPrizeStatus = param[11].Value == DBNull.Value ? 4 : ConvertUtil.ToInt(param[11].Value);
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
            RankID = 0;
            VPQuaterAcc = 0;
            VipQuaterCoeff = 0;
            LoanLimit = 0;
            LoanRate = 0;
            QuaterAcc = 0;
            LoanAmount = 0;
            OldDebt = 0;
            QuaterPrizeStatus = 0;

        }
        public void UserWrongCreate(int ServiceID, long UserID, string DisplayName, string Note, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_DisplayName", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = DisplayName;
                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = ServiceID;
                param[3] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[3].Size = 600;
                param[3].Value = Note;
                param[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_UserWrong_Create", param.ToArray());
                Response = ConvertUtil.ToInt(param[4].Value);
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

        public void UserGetSequence(out long NextSequence)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_NextSequence", SqlDbType.BigInt);
                param[0].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_User_Get_Sequence", param.ToArray());
                NextSequence = Convert.ToInt32(param[0].Value);
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
            NextSequence = -99;
        }

        /// <summary>
        /// kiểm tra xem có tồn tại  trong bản user hay không 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="type"></param>
        /// <param name="Response"></param>
        public void CheckUserExist(string UserName, int type, int serviceid, out int Response)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_Value", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = UserName;
                param[1] = new SqlParameter("@_Type", SqlDbType.Int);
                param[2].Value = type;
                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = serviceid;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_CheckUserExist", param.ToArray());
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

        public User GetUserInfo(long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;

                var _User = db.GetInstanceSP<User>("SP_Users_GetInfo", param.ToArray());
                return _User;
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
        public static UserDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// get user id from user name
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="UserName"></param>
        /// <param name="UserID"></param>
        public void GetUserIDFromUserName(int Type, string UserName, out long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_Type", SqlDbType.Int);
                param[0].Value = Type;
                param[1] = new SqlParameter("@_UserName", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = UserName;
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Get_ID_By_UserName", param.ToArray());
                UserID = Convert.ToInt32(param[2].Value);
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
            UserID = 0;

        }

        public UserInfo GetAccountByNickName(string nickName, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_NickName", nickName);
                pars[1] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var rs = db.GetInstanceSP<UserInfo>("SP_Get_Account_By_NickName", pars);
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

        public int UserUpdateStatus(long userId, int status)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_RequestStatus", status);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Account_UpdateStatus", pars);
                return ConvertUtil.ToInt(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int UserResetPassword(long accountId, string password)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_Password", password);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Account_ResetPassword", pars);
                return Int32.Parse(pars[2].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int AccountDisableSecure(long accountId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Account_Disable_Secure", pars);
                return Int32.Parse(pars[1].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int GamePlayTrackingTrace(DateTime date)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_DatePlay", date);

                db = new DBHelper(Config.BettingConn);
                var rs = db.ExecuteNonQuerySP("SP_GamePlayTracking_Trace", pars);
                return rs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<GamePlayTrackingHour> GamePlayTrackingHour(DateTime? date, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_DatePlay", date);
                pars[1] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var rs = db.GetListSP<GamePlayTrackingHour>("SP_GamePlayTracking_Hour", pars);
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

        public int UserLockTrans(long userId, bool lockStatus)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_LockStatus", lockStatus);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_User_Lock_Trans", pars);
                return ConvertUtil.ToInt(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        /// <param name="type">1: Username, 2: Nickname, 3:Phone, 4: ID</param>
        /// <param name="value"></param>
        /// <param name="serviceid"></param>
        /// <returns></returns>
        public UserInfo GetAccountInfo(int type, string value, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Type", type);
                pars[1] = new SqlParameter("@_Value", value);
                pars[2] = new SqlParameter("@_ServiceID", serviceid);


                db = new DBHelper(Config.BettingConn);
                var rs = db.GetInstanceSP<UserInfo>("SP_Account_GetInfo", pars);
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

        public int TransfigureAgency(ParsTransfigureAgency input)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_UserID", input.userId);
                pars[1] = new SqlParameter("@_FullName", input.fullName);
                pars[2] = new SqlParameter("@_AreaName", input.areaName);
                pars[3] = new SqlParameter("@_PhoneDisplay", input.phoneDisplay);
                pars[4] = new SqlParameter("@_FBLink", input.fbLink ?? (object)DBNull.Value);
                pars[5] = new SqlParameter("@_OrderNum", input.orderNum);
                pars[6] = new SqlParameter("@_ServiceID", input.serviceId);
                pars[7] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_TeleLink", input.teleLink ?? (object)DBNull.Value);
                pars[9] = new SqlParameter("@_ZaloLink", input.zaloLink ?? (object)DBNull.Value);
                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_User_Transfigure_Agency_v2", pars);
                return ConvertUtil.ToInt(pars[7].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<long> GetBalanceFlow(int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_ServiceID", serviceid);
                pars[1] = new SqlParameter("@_Agency0Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_Agency0WalletGiftcode", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_Agency1Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_Agency1WalletGiftcode", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Agency1GiftcodeAvailable", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_UsersWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_AdminGiftcodeAvailable", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Report_Account_Balance", pars);

                long agency0Wallet = ConvertUtil.ToLong(pars[1].Value);
                long agency0Giftcode = ConvertUtil.ToLong(pars[2].Value);
                long agency1Wallet = ConvertUtil.ToLong(pars[3].Value);
                long agency1Giftcode = ConvertUtil.ToLong(pars[4].Value);
                long agency1GiftcodeAvailable = ConvertUtil.ToLong(pars[5].Value);
                long usersWallet = ConvertUtil.ToLong(pars[6].Value);
                long adminGiftcodeAvailable = ConvertUtil.ToLong(pars[7].Value);

                List<long> lstBalance = new List<long>();
                lstBalance.Add(agency0Wallet);
                lstBalance.Add(agency0Giftcode);
                lstBalance.Add(agency1Wallet);
                lstBalance.Add(agency1Giftcode);
                lstBalance.Add(agency1GiftcodeAvailable);
                lstBalance.Add(usersWallet);
                lstBalance.Add(adminGiftcodeAvailable);

                return lstBalance;
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

        public List<ReportProfit> ReportProfit(DateTime? fromDate, DateTime? toDate)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_FromDate", fromDate);
                pars[1] = new SqlParameter("@_ToDate", toDate);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<ReportProfit>("SP_Report_Profit", pars);
                return lstRs;
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
        public List<ReportUsersCreateNew> ReportUsersCreateNew(DateTime? fromDate, DateTime? toDate, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_FromDate", fromDate);
                pars[1] = new SqlParameter("@_ToDate", toDate);
                pars[2] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<ReportUsersCreateNew>("SP_Report_Users_CreateNew", pars);
                return lstRs;
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

        public int UpdateUserBlackList(long userId, int status, string description, long actionUserId)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_RequestStatus ", status);
                pars[2] = new SqlParameter("@_Description", description);
                pars[3] = new SqlParameter("@_ActionAccountID", actionUserId);
                pars[4] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_UserBlackList_Update", pars);
                return ConvertUtil.ToInt(pars[4].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int CheckUserBlackList(long userId)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_User_Check_BlackList", pars);
                return ConvertUtil.ToInt(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
    }
}