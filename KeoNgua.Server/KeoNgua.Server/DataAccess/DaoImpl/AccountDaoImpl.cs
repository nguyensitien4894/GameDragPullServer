using KeoNgua.Server.DataAccess.Dao;
using KeoNgua.Server.DataAccess.Dto;
using System;
using System.Data;
using System.Data.SqlClient;
using TraditionGame.Utilities;

namespace KeoNgua.Server.DataAccess.DaoImpl
{
    public class AccountDaoImpl : IAccountDao
    {
        public AccountDB GetAccount(long accountId, string nickName, int deviceId, int serviceId, int avatar, int vip)
        {
            DBHelper db = null;
            //int response = -1;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_Balance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Account_GetBalance", pars);
                long balance = ConvertUtil.ToLong((pars[1].Value));
                return new AccountDB(accountId, nickName, balance, deviceId, serviceId, avatar, vip);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                //NLogManager.LogMessage(string.Format("GetAccount:{0}({1}) - {2}", nickName, accountId, response));
                if (db != null) db.Close();
            }
            return new AccountDB(accountId, nickName, 0, deviceId, serviceId, avatar, vip);
        }
    }
}