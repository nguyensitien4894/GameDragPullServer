using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class ServiceOffDAO
    {
        private static readonly Lazy<ServiceOffDAO> _instance = new Lazy<ServiceOffDAO>(() => new ServiceOffDAO());

        public static ServiceOffDAO Instance
        {
            get { return _instance.Value; }
        }

        public void BbUserCutOffCheck(long UserID, out string UserName, out string NickName,
            out int UserType, out int Response
            )
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[5];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;

                param[1] = new SqlParameter("@_UserType", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;
                param[2] = new SqlParameter("@_UserName", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[3].Size = 50;
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_BbUserCutOff_Check", param.ToArray());
                UserType = ConvertUtil.ToInt(param[1].Value);
                UserName = ConvertUtil.ToString(param[2].Value);
                NickName = ConvertUtil.ToString(param[3].Value);
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
            UserType = -1;
            UserName = string.Empty;
            NickName = string.Empty;

        }




        public void BbUserCutOffUpdate( long UserID, string UserName, string NickName, string Password,string Note, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[6];

               
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_UserName", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = UserName;
                param[2] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Value = NickName;
                param[3] = new SqlParameter("@_Password", SqlDbType.VarChar);
                param[3].Size = 64;
                param[3].Value = Password??(object)DBNull.Value;
                param[4] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[4].Size = 64;
                param[4].Value = Note ?? (object)DBNull.Value;

                
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_BbUserCutOff_Update", param.ToArray());
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