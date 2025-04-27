using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;

namespace MsWebGame.Thecao.Database.DAO
{
    public class ChoSaoDAO
    {
        private static readonly Lazy<ChoSaoDAO> _instance = new Lazy<ChoSaoDAO>(() => new ChoSaoDAO());

        public static ChoSaoDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// insert chợ sao 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <param name="UserDisplayName"></param>
        /// <param name="Avatar"></param>
        /// <param name="PhoneOTP"></param>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="Response"></param>
        public void UserChoSaoInsert( string UserName, string UserDisplayName, string Avatar, string PhoneOTP, string Email, string Password,string FaceBookID, out int Response,long UserID )
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;

                param[1] = new SqlParameter("@_UserName", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = UserName;
                param[2] = new SqlParameter("@_UserDisplayName", SqlDbType.NVarChar);
                param[2].Size = 200;
                param[2].Value = UserDisplayName;
                param[3] = new SqlParameter("@_Avatar", SqlDbType.VarChar);
                param[3].Size = 100;
                param[3].Value = Avatar;
                param[4] = new SqlParameter("@_PhoneOTP", SqlDbType.VarChar);
                param[4].Size = 100;
                param[4].Value = PhoneOTP;
                param[5] = new SqlParameter("@_Email", SqlDbType.VarChar);
                param[5].Size = 100;
                param[5].Value = Email;
                param[6] = new SqlParameter("@_Password", SqlDbType.VarChar);
                param[6].Size = 100;
                param[6].Value = Password;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                param[8] = new SqlParameter("@_FaceBookID", SqlDbType.NVarChar);
                param[8].Size = 200;
                param[8].Value = FaceBookID;
                
                db.ExecuteNonQuerySP("SP_Users_Create", param.ToArray());
                Response = Convert.ToInt32(param[7].Value);
            
              
               
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
            Response = -99;
        }
       /// <summary>
       /// update profile
       /// </summary>
       /// <param name="UserID"></param>
       /// <param name="UserDisplayName"></param>
       /// <param name="Avatar"></param>
       /// <param name="PhoneOTP"></param>
       /// <param name="Email"></param>
       /// <param name="Password"></param>
       /// <param name="Response"></param>
        public void UpdateProfile(long UserID, string UserDisplayName, string Avatar, string PhoneOTP, string Email, string Password, out int Response)
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
                Response = Convert.ToInt32(param[6].Value);
               
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
        public void UsersUpdateNickName(long UserID, string NickName, out int Response)
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
                db.ExecuteNonQuerySP("SP_Users_Update_NickName", param.ToArray());
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
    }
}