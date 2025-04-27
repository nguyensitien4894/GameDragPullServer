using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class UserDAO
    {
        private static readonly Lazy<UserDAO> _instance = new Lazy<UserDAO>(() => new UserDAO());

        public static UserDAO Instance
        {
            get { return _instance.Value; }
        }
        public void DownLoadTracking(int ServiceID, int AppType, string UrlPage, string ClientIP, string DeviceID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_UrlPage", SqlDbType.VarChar);
                param[0].Size = 250;
                param[0].Value = UrlPage;
                param[1] = new SqlParameter("@_ClientIP", SqlDbType.VarChar);
                param[1].Size = 100;
                param[1].Value = ClientIP;
                param[2] = new SqlParameter("@_DeviceID", SqlDbType.VarChar);
                param[2].Size = 100;
                param[2].Value = DeviceID;
                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;
                param[4] = new SqlParameter("@_AppType", SqlDbType.Int);
                param[4].Value = AppType;
                param[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_PageTrackingInfo_Insert", param.ToArray());
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
        public void UserTracking(int LoginType, int ServiceID, long UserID, string UrlPage, string ClientIP, string UtmMedium, string UtmSource, string UtmCampaign, string UtmContent, string UtmDiff,string UrlPath, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[12];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_LoginType", SqlDbType.Int);
                param[1].Value = LoginType;
                param[2] = new SqlParameter("@_UrlPage", SqlDbType.VarChar);
                param[2].Size = 250;
                param[2].Value = UrlPage;
                param[3] = new SqlParameter("@_ClientIP", SqlDbType.VarChar);
                param[3].Size = 100;
                param[3].Value = ClientIP;
                param[4] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[4].Value = ServiceID;
                param[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_UtmMedium", SqlDbType.NVarChar);
                param[6].Size = 250;
                param[6].Value = UtmMedium ?? (object)DBNull.Value;
                param[7] = new SqlParameter("@_UtmSource", SqlDbType.NVarChar);
                param[7].Size = 250;
                param[7].Value = UtmSource ?? (object)DBNull.Value;
                param[8] = new SqlParameter("@_UtmCampaign", SqlDbType.NVarChar);
                param[8].Size = 250;
                param[8].Value = UtmCampaign ?? (object)DBNull.Value;
                param[9] = new SqlParameter("@_UtmContent", SqlDbType.NVarChar);
                param[9].Size = 250;
                param[9].Value = UtmContent ?? (object)DBNull.Value;
                param[9] = new SqlParameter("@_UtmContent", SqlDbType.NVarChar);
                param[9].Size = 250;
                param[9].Value = UtmContent ?? (object)DBNull.Value;
                param[10] = new SqlParameter("@_UtmDiff", SqlDbType.NVarChar);
                param[10].Size = 250;
                param[10].Value = UtmDiff ?? (object)DBNull.Value;
                param[11] = new SqlParameter("@_UrlPath", SqlDbType.VarChar);
                param[11].Size = 250;
                param[11].Value = UrlPath ?? (object)DBNull.Value;
                

                db.ExecuteNonQuerySP("SP_UserTrackingPage_Insert", param.ToArray());
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


        public void CheckUserExist(string UserName, int type, int ServiceID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];

                param[0] = new SqlParameter("@_Value", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = UserName;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                param[2] = new SqlParameter("@_Type", SqlDbType.Int);
                param[2].Value = type;
                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;
                db.ExecuteNonQuerySP("SP_CheckUserExist", param.ToArray());
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

        public UserAgency GetBotByNickName(string NickName, int serviceId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = NickName;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                param[2] = new SqlParameter("@_ServiceID", serviceId);
                var _UserAgency = db.GetInstanceSP<UserAgency>("SP_Users_Check_Bot", param.ToArray());
                return _UserAgency;
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
        public UserAgency GetUserByNickName(string NickName, int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = NickName;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);

                param[1].Value = ServiceID;
                var _UserAgency = db.GetInstanceSP<UserAgency>("SP_Get_Account_By_NickName", param.ToArray());
                return _UserAgency;
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

        public Users GetUser(int SearchType, string Value)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_SearchType", SqlDbType.Int);
                param[0].Value = SearchType;
                param[1] = new SqlParameter("@_Value", SqlDbType.VarChar);
                param[1].Value = Value;
                param[1].Size = 50;
                var _User = db.GetInstanceSP<Users>("SP_Users_Profile", param.ToArray());
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
        /// <summary>
        /// get current user Id
        /// </summary>
        /// <param name="NextSequence"></param>
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
                NextSequence = Convert.ToInt64(param[0].Value);
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

        public void MomoGetSequence(out long NextSequence)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_NextSequence", SqlDbType.BigInt);
                param[0].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Momo_Get_Sequence", param.ToArray());
                NextSequence = Convert.ToInt64(param[0].Value);
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

        public void UserComplainCreate(long UserID, long ComplainTypeID, int ServiceID, string ResponseText, string Content, bool Status, long UpdateUser, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];


                param[0] = new SqlParameter("@_ResponseText", SqlDbType.NText);
                param[0].Size = 1000;
                param[0].Value = ResponseText;
                param[1] = new SqlParameter("@_Content", SqlDbType.NText);
                param[1].Size = 1000;
                param[1].Value = Content;

                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_ComplainTypeID", SqlDbType.BigInt);
                param[3].Value = ComplainTypeID;
                param[4] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[4].Value = Status;
                param[5] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[5].Value = UpdateUser;
                param[6] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[6].Value = ServiceID;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_UserComplain_Create", param.ToArray());
                Response = ConvertUtil.ToInt(param[7].Value);
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