using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;
using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class UserDAO
    {
        private static readonly Lazy<UserDAO> _instance = new Lazy<UserDAO>(() => new UserDAO());

        public static UserDAO Instance
        {
            get { return _instance.Value; }
        }

        public void CheckUserExist(string UserName, int type,int ServiceID, out int Response)
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
        
        public UserAgency GetBotByNickName(string NickName)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = NickName;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
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
        public UserAgency GetUserByNickName(string NickName)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = NickName;

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
       
        public Users GetUser(int SearchType,string Value)
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
                SqlParameter[] param =new SqlParameter[1];

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


    }
}