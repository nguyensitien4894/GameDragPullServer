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
    public class AdminDAO
    {
        private static readonly Lazy<AdminDAO> _instance = new Lazy<AdminDAO>(() => new AdminDAO());

        public static AdminDAO Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// thêm mới người  tài khoản admin
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PhoneContact"></param>
        /// <param name="Email"></param>
        /// <param name="Level"></param>
        /// <param name="Status"></param>
        /// <param name="RoleID"></param>
        /// <param name="AccountID"></param>
        /// <param name="Password"></param>
        /// <param name="Wallet"></param>
        /// <param name="Response"></param>
        public void Insert(string UserName, string PhoneContact, string Email, long Level, string RoleID, long AccountID, string Password, long Wallet, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_UserName", SqlDbType.NVarChar);
                param[0].Size = 200;
                param[0].Value = UserName;
                param[1] = new SqlParameter("@_PhoneContact", SqlDbType.VarChar);
                param[1].Size = 100;
                param[1].Value = PhoneContact;
                param[2] = new SqlParameter("@_Email", SqlDbType.VarChar);
                param[2].Size = 100;
                param[2].Value = Email;
                param[3] = new SqlParameter("@_Level", SqlDbType.BigInt);
                param[3].Value = Level;
                param[4] = new SqlParameter("@_RoleID", SqlDbType.VarChar);
                param[4].Size = 100;
                param[4].Value = RoleID;
                param[5] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[5].Value = AccountID;
                param[6] = new SqlParameter("@_Password", SqlDbType.NVarChar);
                param[6].Size = 200;
                param[6].Value = Password;
                param[7] = new SqlParameter("@_Wallet", SqlDbType.BigInt);
                param[7].Value = Wallet;
                param[8] = new SqlParameter("@_Response", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Admin_Insert", param.ToArray());
                Response = Convert.ToInt32(param[8].Value);
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

        //1:admin, 2:cskh, 4:hoptac
        public List<Admin> GetList(int RoleID, string UserName, string PhoneContact, int serviceid)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_RoleID", SqlDbType.Int);
                pars[0].Value = RoleID;
                pars[1] = new SqlParameter("@_UserName", SqlDbType.NVarChar);
                pars[1].Size = 100;
                pars[1].Value = UserName;
                pars[2] = new SqlParameter("@_PhoneContact", SqlDbType.NVarChar);
                pars[2].Size = 20;
                pars[2].Value = PhoneContact;
              

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<Admin>("SP_Admin_List", pars);
                return lstRs;
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
        /// get active admin
        /// </summary>
        /// <param name="AdminID"></param>
        /// <returns></returns>
        public Admin GetById(long AdminID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_AdminID", SqlDbType.BigInt);
                param[0].Value = AdminID;

                var _Admin = db.GetInstanceSP<Admin>("SP_Admin_GetByID", param.ToArray());
                return _Admin;
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
        /// cập nhật admin
        /// </summary>
        /// <param name="AdminName"></param>
        /// <param name="Password"></param>
        /// <param name="RoleID"></param>
        /// <param name="Phone"></param>
        /// <param name="Email"></param>
        /// <param name="Level"></param>
        /// <param name="Wallet"></param>
        /// <param name="Status"></param>
        /// <param name="Response"></param>
        public void Update(string AdminName, string Password, int RoleID, string Phone, string Email, int Level, long Wallet, int Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_AdminName", SqlDbType.NVarChar);
                param[0].Size = 200;
                param[0].Value = AdminName;
                param[1] = new SqlParameter("@_Password", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = Password;
                param[2] = new SqlParameter("@_RoleID", SqlDbType.Int);
                param[2].Value = RoleID;
                param[3] = new SqlParameter("@_Phone", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = Phone;
                param[4] = new SqlParameter("@_Email", SqlDbType.VarChar);
                param[4].Size = 50;
                param[4].Value = Email;
                param[5] = new SqlParameter("@_Level", SqlDbType.Int);
                param[5].Value = Level;
                param[6] = new SqlParameter("@_Wallet", SqlDbType.BigInt);
                param[6].Value = Wallet;
                param[7] = new SqlParameter("@_Status", SqlDbType.Int);
                param[7].Value = Status;
                param[8] = new SqlParameter("@_Response", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;
               
                db.ExecuteNonQuerySP("SP_Admin_Update", param.ToArray());
                Response = Convert.ToInt32(param[8].Value);
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
        public void UpdateAuthen(long AdminID, int Authen, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_AdminID", SqlDbType.BigInt);
             
                param[0].Value = AdminID;
                param[1] = new SqlParameter("@_IsFirstGoogleAuthen", SqlDbType.Int);
                
                param[1].Value = Authen;
               
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Admin_UpdateAuthen", param.ToArray());
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
        
        /// <summary>
        /// cập nhật trạng thái về 0
        /// </summary>
        /// <param name="AdminName"></param>
        /// <param name="Response"></param>
        public void Delete(string AdminName, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
             new SqlParameter[2];

                param[0] = new SqlParameter("@_AdminName", SqlDbType.NVarChar);
                param[0].Size = 200;
                param[0].Value = AdminName;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Admin_Delete", param.ToArray());
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

        public int AdminTransferToAgency(long adminId, long receiverId, int walletType, long amount, string note,int ServiceID, out long transId, out long wallet,out long walletrecive)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_WalletType", walletType);
                pars[3] = new SqlParameter("@_Amount", amount);
                pars[4] = new SqlParameter("@_Note", note);
                pars[5] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_ServiceID", ServiceID);
                pars[9] = new SqlParameter("@_ReceiverWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Admin_Transfer_To_Agency", pars);
                transId = ConvertUtil.ToLong(pars[5].Value);
                wallet = ConvertUtil.ToLong(pars[6].Value);
                walletrecive = ConvertUtil.ToLong(pars[9].Value);
                return ConvertUtil.ToInt(pars[7].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                walletrecive = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int AdminTransferToUserSub(long adminId, long receiverId, long amount, string note, int ServiceID, out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_Admin_Transfer_To_User_Cskh_Sub", pars);
                transId = ConvertUtil.ToLong(pars[4].Value);
                wallet = ConvertUtil.ToLong(pars[5].Value);
                return ConvertUtil.ToInt(pars[6].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int AdminTransferToUser2(long adminId, long receiverId, long amount, string note, int ServiceID,out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_Admin_Transfer_To_User_Cskh", pars);
                transId = ConvertUtil.ToLong(pars[4].Value);
                wallet = ConvertUtil.ToLong(pars[5].Value);
                return ConvertUtil.ToInt(pars[6].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public int AdminTransferToUserSafeBalance(long adminId, long receiverId, long amount, string note, int ServiceID, out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_Admin_Transfer_To_User_SafeBalance", pars);
                transId = ConvertUtil.ToLong(pars[4].Value);
                wallet = ConvertUtil.ToLong(pars[5].Value);
                return ConvertUtil.ToInt(pars[6].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int AdminTransferToUserSafeBalanceSub(long adminId, long receiverId, long amount, string note, int ServiceID, out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_Admin_Transfer_To_User_SafeBalance_Sub", pars);
                transId = ConvertUtil.ToLong(pars[4].Value);
                wallet = ConvertUtil.ToLong(pars[5].Value);
                return ConvertUtil.ToInt(pars[6].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }


        public int AdminTransferUserFish(long adminId,int type, long receiverId, long amount, string note, int ServiceID, out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ServiceID", ServiceID);
                pars[8] = new SqlParameter("@_ExchangeType", type);// 1 là cộng vào tài khoản bắn cá , 2 trừ
                db.ExecuteNonQuerySP("SP_Admin_Tranfer_AccountFish", pars);
                transId = ConvertUtil.ToLong(pars[4].Value);
                wallet = ConvertUtil.ToLong(pars[5].Value);
                return ConvertUtil.ToInt(pars[6].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public int AdminTransferToUserSubtraction(long adminId, long receiverId, long amount, string note, int ServiceID, out long transId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_AdminTest_Subtraction_User_Cskh", pars);
                transId = ConvertUtil.ToLong(pars[4].Value);
                return ConvertUtil.ToInt(pars[5].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int AdminTransferToAdmin(long adminId, long receiverId, long amount, string note, out long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_AdminId", adminId);
                pars[1] = new SqlParameter("@_ReceiverId", receiverId);
                pars[2] = new SqlParameter("@_Amount", amount);
                pars[3] = new SqlParameter("@_Note", note);
                pars[4] = new SqlParameter("@_TransID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_Wallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_ReceiverWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                if (receiverId == 10)
                {
                    db.ExecuteNonQuerySP("SP_AdminTest_Transfer_To_Admin", pars);
                }
                else
                {
                    db.ExecuteNonQuerySP("SP_Admin_Transfer_To_Admin", pars);
                }
                
                transId = ConvertUtil.ToLong(pars[4].Value);
                wallet = ConvertUtil.ToLong(pars[5].Value);
                return ConvertUtil.ToInt(pars[7].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                transId = 0;
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int UserTransferReceive(long adminId, string nickName, long userId, long amount, string note, long transId, long wallet)
        {
         DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_RemitterID", adminId);
                pars[1] = new SqlParameter("@_RemitterName", nickName);
                pars[2] = new SqlParameter("@_RemitterType", 3);
                pars[3] = new SqlParameter("@_RemitterLevel", 0);
                pars[4] = new SqlParameter("@_UserID", userId);
                pars[5] = new SqlParameter("@_Amount", amount);
                pars[6] = new SqlParameter("@_Note", note);
                pars[7] = new SqlParameter("@_TransID", transId);
                pars[8] = new SqlParameter("@_RemainWallet", wallet);
                pars[9] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_User_Transfer_Receive", pars);
                return ConvertUtil.ToInt(pars[9].Value);
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

        public int UserCardRechargeRefund(long adminId, string nickName, long userId, long amount, string note, long wallet,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_RemitterID", adminId);
                pars[1] = new SqlParameter("@_RemitterName", nickName);
                pars[2] = new SqlParameter("@_RemitterType", 3);
                pars[3] = new SqlParameter("@_RemitterLevel", 0);
                pars[4] = new SqlParameter("@_UserID", userId);
                pars[5] = new SqlParameter("@_Amount", amount);
                pars[6] = new SqlParameter("@_Note", note);
               
                pars[7] = new SqlParameter("@_RemainWallet", wallet);
                pars[8] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[9] = new SqlParameter("@_ServiceID", ServiceID) ;
                
                db.ExecuteNonQuerySP("SP_User_CardRecharge_Refund", pars);
                return ConvertUtil.ToInt(pars[8].Value);
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

        public List<GameBankInfo> GetGameBankInfo(int gameId, int roomId, DateTime startDate, DateTime endDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_GameID", gameId);
                pars[1] = new SqlParameter("@_RoomID", roomId);
                pars[2] = new SqlParameter("@_StartDate", startDate);
                pars[3] = new SqlParameter("@_EndDate", endDate);
                var lstRs = db.GetListSP<GameBankInfo>("SP_Gamebank_Search", pars);
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

        public int TrackingSpin(int gameId, int roomId, DateTime startDate, DateTime endDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_GameID", gameId);
                pars[1] = new SqlParameter("@_RoomID", roomId);
                pars[2] = new SqlParameter("@_StartDate", startDate);
                pars[3] = new SqlParameter("@_EndDate", endDate);
                pars[4] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Tracking_Spins", pars);
                return Int32.Parse(pars[4].Value.ToString());
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

        public List<GameBankExpertise> GameBankExamine(int gameId, int roomId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_GameID", gameId);
                pars[1] = new SqlParameter("@_RoomID", roomId);
                var lstRs = db.GetListSP<GameBankExpertise>("SP_Gamebank_Examine", pars);
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

        public long TrackingExamine(int gameId, int roomId, DateTime startDate, DateTime endDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_GameID", gameId);
                pars[1] = new SqlParameter("@_RoomID", roomId);
                pars[2] = new SqlParameter("@_StartDate", startDate);
                pars[3] = new SqlParameter("@_EndDate", endDate);
                pars[4] = new SqlParameter("@_PoolAccumulate", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_PrizeAccumulate", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Flag", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Tracking_Examine", pars);
                return Int64.Parse(pars[6].Value.ToString());
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


        public int UserLock(long userId, int lockType)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_LockType", lockType);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Users_Lock", pars);
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

        public int UserUnLock(long userId, int lockType)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_LockType", lockType);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Users_UnLock", pars);
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

        public int AgencyLock(long accountId, long accountLock)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_AccountLockedId", accountLock);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Agency_Lock", pars);
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

        public int AgencyUnLock(long accountId, long accountLock)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_AccountLockedId", accountLock);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Agency_UnLock", pars);
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


        public List<Complain> GetComplainList(int SearchType, int ComplainType, long ComplainID, string UserName, int Status, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
             new SqlParameter[8];

                param[0] = new SqlParameter("@_SearchType", SqlDbType.Int);
                param[0].Value = SearchType;
                param[1] = new SqlParameter("@_ComplainType", SqlDbType.Int);
                param[1].Value = ComplainType;
                param[2] = new SqlParameter("@_ComplainID", SqlDbType.BigInt);
                param[2].Value = ComplainID;
                param[3] = new SqlParameter("@_UserName", SqlDbType.VarChar);
                param[3].Size = 50;
                param[3].Value = UserName;
                param[4] = new SqlParameter("@_Status", SqlDbType.Int);
                param[4].Value = Status;
                param[5] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[5].Value = CurrentPage;
                param[6] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[6].Value = RecordPerpage;
                param[7] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;

                var _lstComplain = db.GetListSP<Complain>("SP_Complain_List", param.ToArray());

                TotalRecord = Convert.ToInt32(param[7].Value);
                return _lstComplain;
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
            TotalRecord = 0;
            return null;
        }

        public int ComplainVerify(long complainid, int complaintype, string title, string content, long userid, long transid, string transimage, string userimage, string userexplain,
            string useprorst, long defid, string defimage, string defexplain, string defprorst, long mediatorid, string result, int status, long? refcompid, DateTime? createdate, DateTime? updatedate, string ProcessCall)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[22];
                pars[0] = new SqlParameter("@_ComplainID", complainid);
                pars[1] = new SqlParameter("@_ComplainType", complaintype);
                pars[2] = new SqlParameter("@_Title", title);
                pars[3] = new SqlParameter("@_Content", content);
                pars[4] = new SqlParameter("@_UserID", userid);
                pars[5] = new SqlParameter("@_TransID", transid);
                pars[6] = new SqlParameter("@_TranferImage", transimage);
                pars[7] = new SqlParameter("@_UserImage", userimage);
                pars[8] = new SqlParameter("@_UserExplain", userexplain);
                pars[9] = new SqlParameter("@_UserProcessResult", useprorst);
                pars[10] = new SqlParameter("@_DefendantID", defid);
                pars[11] = new SqlParameter("@_DefendantImage", defimage);
                pars[12] = new SqlParameter("@_DefendantExplain", defexplain);
                pars[13] = new SqlParameter("@_DefendantProcessResult", defprorst);
                pars[14] = new SqlParameter("@_MediatorID", mediatorid);
                pars[15] = new SqlParameter("@_Result", result);
                pars[16] = new SqlParameter("@_Status", status);
                pars[17] = new SqlParameter("@_ReferComplainID", refcompid);
                pars[18] = new SqlParameter("@_CreateDate", createdate);
                pars[19] = new SqlParameter("@_UpdateDate", updatedate);
                pars[20] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[21] = new SqlParameter("@_ProcessCall", ProcessCall);
                db.ExecuteNonQuerySP("SP_Complain_Verify", pars);
                return Int32.Parse(pars[20].Value.ToString());
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

        public int UserMarketUpdate(long transid, long userid, int resstaus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_TransID", transid);
                pars[1] = new SqlParameter("@_UserID", userid);
                pars[2] = new SqlParameter("@_RequestStatus", resstaus);
                pars[3] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_UserMarket_Update", pars);
                return Int32.Parse(pars[3].Value.ToString());
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

        public int UserMarketCancel(long transid, long userid, int resstaus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_TransID", transid);
                pars[1] = new SqlParameter("@_UserID", userid);
                pars[2] = new SqlParameter("@_RequestStatus", resstaus);
                pars[3] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_UserMarket_Cancel", pars);
                return Int32.Parse(pars[3].Value.ToString());
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
  


        public List<HistoryTransfer> GetListHistoryTranfers(long UserID, int UserType, int Type, int? TranType, int? Status, long? TransID, string PartnerName, DateTime? FromDate, DateTime? ToDate, int ServiceID,int CurrentPage, int RecordPerpage, out int TotalRecord)

        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];

                param[0] = new SqlParameter("@_UserType", SqlDbType.Int);
                param[0].Value = UserType;
                param[1] = new SqlParameter("@_Type", SqlDbType.Int);
                param[1].Value = Type;
                param[2] = new SqlParameter("@_ServiceID", ServiceID);

                
                param[3] = new SqlParameter("@_Status", SqlDbType.Int);
                param[3].Value = Status;
                param[4] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[4].Value = UserID;
                param[5] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[5].Value = TransID;
                param[6] = new SqlParameter("@_PartnerName", SqlDbType.VarChar);
                param[6].Size = 20;
                param[6].Value = PartnerName;
                param[7] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[7].Value = FromDate.HasValue ? FromDate.Value : (object)DBNull.Value;
                param[8] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[8].Value = ToDate.HasValue ? ToDate.Value : (object)DBNull.Value;
                param[9] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[9].Value = CurrentPage;
                param[10] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[10].Value = RecordPerpage;
                param[11] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
              
                var _lstHistoryTransfer = db.GetListSP<HistoryTransfer>("SP_Transaction_List_All", param.ToArray());
                TotalRecord = Convert.ToInt32(param[11].Value);
                return _lstHistoryTransfer;


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                TotalRecord = 0;
                return null;

            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }


        public void AdminWalletExchange(long AdminId, long Amount, out long Wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_AdminId", SqlDbType.BigInt);
                param[0].Value = AdminId;
                param[1] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[1].Value = Amount;
                param[2] = new SqlParameter("@_Wallet", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_AdminWallet_Exchange", param.ToArray());
                Wallet = Convert.ToInt64(param[2].Value);
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
            Wallet = -1;
        }

        public int WalletLogsCreate(long accountId, int accountType, int walletType, long orgBalance, long amount, long balance,
            int reasonId, int status, long tranId, string description, DateTime createDate, long createBy,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[14];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_AccountType", accountType);
                pars[2] = new SqlParameter("@_WalletType", walletType);
                pars[3] = new SqlParameter("@_OrgBalance", orgBalance);
                pars[4] = new SqlParameter("@_Amount", amount);
                pars[5] = new SqlParameter("@_Balance", balance);
                pars[6] = new SqlParameter("@_ReasonID", reasonId);
                pars[7] = new SqlParameter("@_Status", status);
                pars[8] = new SqlParameter("@_TranId", tranId);
                pars[9] = new SqlParameter("@_Description", description);
                pars[10] = new SqlParameter("@_CreateDate", createDate);
                pars[11] = new SqlParameter("@_CreateBy", createBy);
                pars[12] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[13] = new SqlParameter("@_ServiceID", ServiceID);
                var rs = db.ExecuteNonQuerySP("SP_WalletLogs_Create", pars);

                return ConvertUtil.ToInt(pars[11].Value);
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

        public int TransactionUpdate(long transId, int status)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_TransID", transId);
                pars[1] = new SqlParameter("@_Status", status);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Transaction_Update", pars);
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

        public List<AdminRevenue> GetAdminRevenue(long adminId, int partnertype, DateTime? fromDate, DateTime? toDate, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_AdminID", adminId);
                pars[1] = new SqlParameter("@_PartnerType", partnertype);
                pars[2] = new SqlParameter("@_FromDate", fromDate);
                pars[3] = new SqlParameter("@_ToDate", toDate);
                pars[4] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var rs = db.GetListSP<AdminRevenue>("SP_Admin_Revenue", pars);
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

        public List<AdminTrans> GetAdminTrans(long adminId, ParsAdminTrans input, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[11];
                pars[0] = new SqlParameter("@_AdminID", adminId);
                pars[1] = new SqlParameter("@_TransferType", input.transferType);
                pars[2] = new SqlParameter("@_PartnerType", input.partnerType);
                pars[3] = new SqlParameter("@_PartnerName", input.partnerName);
                pars[4] = new SqlParameter("@_ReasonID", input.reasonId);
                pars[5] = new SqlParameter("@_FromDate", input.fromDate);
                pars[6] = new SqlParameter("@_ToDate", input.toDate);
                pars[7] = new SqlParameter("@_ServiceID", input.serviceId);
                pars[8] = new SqlParameter("@_CurrentPage", currentPage);
                pars[9] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[10] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<AdminTrans>("SP_Admin_Trans", pars);
                totalRecord = ConvertUtil.ToInt(pars[10].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public int AdminChangePassword(string adminName, string password)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AdminName", adminName);
                pars[1] = new SqlParameter("@_Password", password);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
              
                db.ExecuteNonQuerySP("SP_Admin_Change_Password", pars);
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

        public int AdminLockCallcenter(string lockName, bool lockStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Lockname", lockName);
                pars[1] = new SqlParameter("@_LockStatus", lockStatus);
                pars[2] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Admin_Lock_Callcenter", pars);
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

        public List<CallCenterInfo> GetCallCenterList(string username, string phone,int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_UserName", username);
                pars[1] = new SqlParameter("@_PhoneContact", phone);
               
                var lstRs = db.GetListSP<CallCenterInfo>("SP_Callcenter_List", pars);
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
    }
}