using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.AccountHack;
using TraditionGame.Utilities;
namespace MsWebGame.CSKH.Database.DAO
{
    public class AccountHackDAO
    {
        private static readonly Lazy<AccountHackDAO> _instance = new Lazy<AccountHackDAO>(() => new AccountHackDAO());
        public static AccountHackDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<AccountHack> GetList(string bankname, int currentpage, int recordperpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_ObjectName", bankname);
                pars[1] = new SqlParameter("@_Status", null);
                pars[2] = new SqlParameter("@_CurrentPage", currentpage);
                pars[3] = new SqlParameter("@_RecordPerpage", recordperpage);
                pars[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var _ltsBanks = db.GetListSP<AccountHack>("SP_AccountHack_List", pars);
                totalrecord = ConvertUtil.ToInt(pars[4].Value);
                return _ltsBanks;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalrecord = 0;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            return null;
        }

        public AccountHack AccountHackGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _AccountHack = db.GetInstanceSP<AccountHack>("SP_AccountHack_GetByID", param.ToArray());
                return _AccountHack;
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

        public void AccountHackHandle(string AccountBankName, string AccountBankNumber,string BankName,string Reason, out int ResponseStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_AccountBankName", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = AccountBankName;
                param[1] = new SqlParameter("@_AccountBankNumber", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = AccountBankNumber;
                param[2] = new SqlParameter("@_BankName", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = BankName;
                param[3] = new SqlParameter("@_Reason", SqlDbType.NText);
                param[3].Size = 1000;
                param[3].Value = Reason;
                param[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_AccountHack_Handle", param.ToArray());
                ResponseStatus = Convert.ToInt32(param[4].Value);
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
            ResponseStatus = -99;
        }
        public void AccountHackEdit(long id,string AccountBankName, string AccountBankNumber, string BankName, string Reason, out int ResponseStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_AccountBankName", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = AccountBankName;
                param[1] = new SqlParameter("@_AccountBankNumber", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = AccountBankNumber;
                param[2] = new SqlParameter("@_BankName", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = BankName;
                param[3] = new SqlParameter("@_Reason", SqlDbType.NText);
                param[3].Size = 1000;
                param[3].Value = Reason;
                param[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_ID ", SqlDbType.BigInt);
                param[5].Value = id;
                db.ExecuteNonQuerySP("SP_AccountHack_Update", param.ToArray());
                ResponseStatus = Convert.ToInt32(param[4].Value);
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
            ResponseStatus = -99;
        }


        public void Delete(long id, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
                    new SqlParameter[2];

                param[0] = new SqlParameter("@_Id", SqlDbType.BigInt);
                param[0].Value = id;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_AccountHack_Delete", param.ToArray());
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
    }
}