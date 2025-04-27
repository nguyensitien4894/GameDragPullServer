using System;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Param;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class PhatLocDAO
    {
        private static readonly Lazy<PhatLocDAO> _instance = new Lazy<PhatLocDAO>(() => new PhatLocDAO());

        public static PhatLocDAO Instance
        {
            get { return _instance.Value; }
        }

        public UserInfo GetUsersCheckBot(string nickName)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_NickName", nickName);
                pars[1] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var rs = db.GetInstanceSP<UserInfo>("SP_Users_Check_Bot", pars);
                return rs;
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

        public int TransferToUser(ParsPhatLocTransfer input, out long transId, out long remainWallet)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_BotID", input.phatLocId);
                pars[1] = new SqlParameter("@_ReceiverId", input.receiverId);
                pars[2] = new SqlParameter("@_Amount", input.amountNum);
                pars[3] = new SqlParameter("@_Note", input.note);
                pars[4] = new SqlParameter("@_ServiceID", input.serviceId);
                pars[5] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Bot_Transfer_To_User", pars);
                transId = ConvertUtil.ToLong(pars[5].Value);
                remainWallet = ConvertUtil.ToLong(pars[6].Value);
                return ConvertUtil.ToInt(pars[7].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                remainWallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public UserInfo GetUsersCheckTest(string nickName, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_NickName", nickName);
                pars[1] = new SqlParameter("@_ServiceID", serviceid);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var rs = db.GetInstanceSP<UserInfo>("SP_Users_Check_Test", pars);
                return rs;
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

        public int TestTransferToUser(ParsPhatLocTransfer input, out long transId, out long remainWallet)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_TesterID", input.phatLocId);
                pars[1] = new SqlParameter("@_ReceiverId", input.receiverId);
                pars[2] = new SqlParameter("@_Amount", input.amountNum);
                pars[3] = new SqlParameter("@_Note", input.note);
                pars[4] = new SqlParameter("@_ServiceID", input.serviceId);
                pars[5] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Tester_Transfer_To_User", pars);
                transId = ConvertUtil.ToLong(pars[5].Value);
                remainWallet = ConvertUtil.ToLong(pars[6].Value);
                return ConvertUtil.ToInt(pars[7].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                remainWallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public Account UserLogin(string username, string password, int loginType, int deviceType, int serviceId, out int response)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_Username", username);
                pars[1] = new SqlParameter("@_Password", password);
                pars[2] = new SqlParameter("@_LoginType", loginType);
                pars[3] = new SqlParameter("@_DeviceType", deviceType);
                pars[4] = new SqlParameter("@_ServiceID", serviceId);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                Account rs = db.GetInstanceSP<Account>("SP_Account_UserLogin", pars);
                response = ConvertUtil.ToInt(pars[5].Value);
                return rs;
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

        public int UserTransferToUser(long remitterId, long receiverId, long amount, string note, int serviceId, out long transId, out long remainWallet)
        {
            DBHelper db = null;
            transId = 0;
            remainWallet = 0;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_RemitterId", remitterId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_ServiceID", serviceId);
                pars[5] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_User_Transfer_To_User", pars);
                int response = ConvertUtil.ToInt(pars[7].Value);
                if (response == 1)
                {
                    transId = ConvertUtil.ToLong(pars[5].Value);
                    remainWallet = ConvertUtil.ToLong(pars[6].Value);
                }

                return response;
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