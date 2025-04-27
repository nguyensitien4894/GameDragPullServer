using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.Param;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class CustomerSupportDAO
    {
        private static readonly Lazy<CustomerSupportDAO> _instance = new Lazy<CustomerSupportDAO>(() => new CustomerSupportDAO());

        public static CustomerSupportDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<Complain> GetListComplain(int searchtype, long complainid, int complaintype, string username, int status)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_SearchType", searchtype);
                pars[1] = new SqlParameter("@_ComplainID", complainid);
                pars[2] = new SqlParameter("@_ComplainType", complaintype);
                pars[3] = new SqlParameter("@_UserName", username);
                pars[4] = new SqlParameter("@_Status", status);

                var lstRs = db.GetListSP<Complain>("SP_Complain_List", pars);
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

        public int ComplainVerify(long complainid, int complaintype, string title, string content, long userid, long transid, string transimage,
            string userimage, string userexplain, string useprorst, long defid, string defimage, string defexplain, string defprorst,
            long mediatorid, string result, int status, long? refcompid, DateTime createdate, DateTime updatedate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[21];
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

        public List<UserProfile> GetListUserProfile(int type, string value)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_SearchType", type);
                pars[1] = new SqlParameter("@_Value", value);

                var lstRs = db.GetListSP<UserProfile>("SP_Users_Profile", pars);
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

        public List<UserPrivilege> GetListUserPrivilege(int type, string value, int rankid, DateTime? date, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_SearchType", type);
                pars[1] = new SqlParameter("@_Value", value);
                pars[2] = new SqlParameter("@_RankID", rankid);
                pars[3] = new SqlParameter("@_RankedMonth", date);
                pars[4] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<UserPrivilege>("SP_UserPrivilege_List", pars);
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

        public List<UserPrivilegeHistory> GetUserPrivilegeHistory(long accountid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_UserID", accountid);

                var lstRs = db.GetListSP<UserPrivilegeHistory>("SP_UserPrivilege_History", pars);
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

        public List<HistoryPlay> GetHistoryPlay(long accountid, int gametype, long? spinId, DateTime fromDate, DateTime toDate,int OrderBy, int currpage, int recpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_AccountID", accountid);
                pars[1] = new SqlParameter("@_GameType", gametype);
                pars[2] = new SqlParameter("@_SpinID", spinId);
                pars[3] = new SqlParameter("@_FromDate", fromDate);
                pars[4] = new SqlParameter("@_ToDate", toDate);
                pars[5] = new SqlParameter("@_CurrentPage", currpage);
                pars[6] = new SqlParameter("@_RecordPerpage", recpage);
                pars[7] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_OrderBy", OrderBy);
                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryPlay>("SP_HistoryPlay_Search", pars);
                totalrecord = ConvertUtil.ToInt(pars[7].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalrecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<AccountIP> GetAccountClone(string nickname)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_tnv", nickname);
                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<AccountIP>("SP_AccountIP_GetList_ByAccountID", pars);
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
        public List<HistoryPlay> GetHistoryPlay(long spinId)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_GameType", 8);
                pars[1] = new SqlParameter("@_SpinID", spinId);
                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryPlay>("SP_HistoryPlay_Search_TX", pars);
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


        public List<HistoryPlay> GetHistoryPlayAll(long accountid, int gametype, long? spinId, DateTime fromDate, DateTime toDate, int OrderBy, int currpage, int recpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_AccountID", accountid);
                pars[1] = new SqlParameter("@_GameType", gametype);
                pars[2] = new SqlParameter("@_SpinID", spinId);
                pars[3] = new SqlParameter("@_FromDate", fromDate);
                pars[4] = new SqlParameter("@_ToDate", toDate);
                pars[5] = new SqlParameter("@_CurrentPage", currpage);
                pars[6] = new SqlParameter("@_RecordPerpage", recpage);
                pars[7] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_OrderBy", OrderBy);
                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryPlay>("SP_HistoryPlay_SearchAll", pars);
                totalrecord = ConvertUtil.ToInt(pars[7].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalrecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<HistoryWalletLog> GetListWalletLog(long accountId, string partnerName, int tranType, DateTime? fromdate, DateTime? todate, int serviceid,
            int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_UserID", accountId);
                pars[1] = new SqlParameter("@_UserName", null);
                pars[2] = new SqlParameter("@_TranType", tranType);
                pars[3] = new SqlParameter("@_PartnerName", partnerName);
                pars[4] = new SqlParameter("@_FromDate", fromdate);
                pars[5] = new SqlParameter("@_ToDate", todate);
                pars[6] = new SqlParameter("@_ServiceID", serviceid);
                pars[7] = new SqlParameter("@_CurrentPage", currentPage);
                pars[8] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[9] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<HistoryWalletLog>("SP_BalanceLogs_Admin_List", pars);
                totalRecord = ConvertUtil.ToInt(pars[9].Value);
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

        public List<HistoryJackpot> GetHistoryJackpot(string username, int gameid, DateTime? from, DateTime? to, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_UserName", username);
                pars[1] = new SqlParameter("@_GameID", gameid);
                pars[2] = new SqlParameter("@_FromDate", from);
                pars[3] = new SqlParameter("@_ToDate", to);
                pars[4] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryJackpot>("SP_Users_JackpotHistory", pars);
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

        public List<HistoryJackpot> GetHistoryJackpot( DateTime? from, DateTime? to, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_UserName", DBNull.Value);
                pars[1] = new SqlParameter("@_GameID", DBNull.Value);
                pars[2] = new SqlParameter("@_FromDate", from);
                pars[3] = new SqlParameter("@_ToDate", to);
                pars[4] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryJackpot>("SP_Users_JackpotHistoryAll", pars);
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

        public List<HistoryThankful> GetHistoryThankful(string username, DateTime? from, DateTime? to)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountID", username);
                //pars[1] = new SqlParameter("@_GameID", gameid);
                pars[1] = new SqlParameter("@_FromDate", from);
                pars[2] = new SqlParameter("@_ToDate", to);
                //pars[4] = new SqlParameter("@_ServiceID", serviceid);
                //pars[3] = new SqlParameter("@_SumLoose", SqlDbType.Int) { Direction = ParameterDirection.Output };
                //pars[4] = new SqlParameter("@_TriAn", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryThankful>("SP_HistoryThankful_by_UserId", pars);
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

        public List<HistoryThankful> GetHistoryThankful(DateTime? from, DateTime? to)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountID", DBNull.Value);
                //pars[1] = new SqlParameter("@_GameID", DBNull.Value);
                pars[1] = new SqlParameter("@_FromDate", from);
                pars[2] = new SqlParameter("@_ToDate", to);
                //pars[4] = new SqlParameter("@_ServiceID", serviceid);
                //pars[3] = new SqlParameter("@_SumLoose", SqlDbType.Int) { Direction = ParameterDirection.Output };
                //pars[4] = new SqlParameter("@_TriAn", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryThankful>("SP_HistoryThankful_by_UserId", pars);
                //var lstRs = db.GetListSP<HistoryThankful>("SP_HistoryThankful_All", pars);
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

        public List<HistoryThankfulAccountList> GetHistoryThankfulAccountList(DateTime? from, DateTime? to, int currentpage, int recordperpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[6];
                //@_CurrentPage = 2,@_RecordPerpage=10,@_TotalRecord=100
                pars[0] = new SqlParameter("@_AccountID", DBNull.Value);
                //pars[1] = new SqlParameter("@_GameID", DBNull.Value);
                pars[1] = new SqlParameter("@_FromDate", from);
                pars[2] = new SqlParameter("@_ToDate", to);
                pars[3] = new SqlParameter("@_CurrentPage", currentpage);
                pars[4] = new SqlParameter("@_RecordPerpage", recordperpage);
                pars[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<HistoryThankfulAccountList>("SP_HistoryThankful_GetAccountList", pars);
                totalrecord = ConvertUtil.ToInt(pars[5].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalrecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public List<TransactionHistory> GetListTransaction(ParsTransactionHistory input, int currentPage, int recordPerpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {

                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_ObjectType ", input.ObjectType);
                pars[1] = new SqlParameter("@_ObjectValue", input.ObjectValue??(object)DBNull.Value);
                pars[2] = new SqlParameter("@_SearchType", input.SearchType);
                pars[3] = new SqlParameter("@_PartnerType", input.PartnerType);
                pars[4] = new SqlParameter("@_TransCode", input.TransCode??(object)DBNull.Value);
                pars[5] = new SqlParameter("@_ServiceID", input.ServiceID);
                pars[6] = new SqlParameter("@_StartDate", input.StartDate);

                pars[7] = new SqlParameter("@_CurrentPage", currentPage);
                pars[8] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[9] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[10] = new SqlParameter("@_EndDate", input.EndDate.AddDays(1).AddSeconds(-1));
                pars[11] = new SqlParameter("@_PartnerName", input.PartnerName ?? (object)DBNull.Value);
                pars[12] = new SqlParameter("@_TransType", input.TransType);


                db = new DBHelper(Config.BettingConn);

                var lstRs = db.GetListSP<TransactionHistory>("SP_UserTransactions_GetList", pars);
                totalrecord = ConvertUtil.ToInt(pars[9].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalrecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public List<TransactionHistory> GetCallDragonHis(ParsTransactionHistory input, int currentPage, int recordPerpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {

                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_ObjectType ", input.ObjectType);
                pars[1] = new SqlParameter("@_ObjectValue", input.ObjectValue ?? (object)DBNull.Value);
                pars[2] = new SqlParameter("@_SearchType", input.SearchType);
                pars[3] = new SqlParameter("@_PartnerType", input.PartnerType);
                pars[4] = new SqlParameter("@_TransCode", input.TransCode ?? (object)DBNull.Value);
                pars[5] = new SqlParameter("@_ServiceID", input.ServiceID);
                pars[6] = new SqlParameter("@_StartDate", input.StartDate);

                pars[7] = new SqlParameter("@_CurrentPage", currentPage);
                pars[8] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[9] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[10] = new SqlParameter("@_EndDate", input.EndDate.AddDays(1).AddSeconds(-1));
                pars[11] = new SqlParameter("@_PartnerName", input.PartnerName ?? (object)DBNull.Value);
                pars[12] = new SqlParameter("@_TransType", input.TransType);


                db = new DBHelper(Config.BettingConn);

                var lstRs = db.GetListSP<TransactionHistory>("SP_GetCallDragonHis_GetList", pars);
                totalrecord = ConvertUtil.ToInt(pars[9].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalrecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public List<UserProfit> GetListBangXepHang(ParsBangXepHang input, int currentPage, int recordPerpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                string query = "";
                if (input.Type == 1)
                {
                    query = "SP_UserProfitWin_GetList";
                }
                else
                {
                    if (input.Type == 2)
                    {
                        query = "SP_UserProfitLose_GetList";
                    }
                    else
                    {
                        if (input.Type == 3)
                        {
                            query = "SP_UserProfitVip_GetList";
                        }
                    }
                }
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_CurrentPage", currentPage);
                pars[1] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[2] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<UserProfit>(query, pars);
                totalrecord = ConvertUtil.ToInt(pars[2].Value);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalrecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        /// <summary>
        /// Lấy thông tin tổng quan Account từ gamecore
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public AccountOverview GetAccountInfoOverview(int type, string value, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_SearchType", type);
                pars[1] = new SqlParameter("@_Value", value);
                pars[2] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<AccountOverview>("SP_Account_Overview", pars);
                if (lstRs == null) return null;
                return lstRs.FirstOrDefault();
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

        /// <summary>
        /// Lấy tổng quan giao dịch của account
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public AccountOverview GetAccountTransactionOverview(long accountid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_AccountID", accountid);
                pars[1] = new SqlParameter("@_RankID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_VipPoint", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_RefundAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_TotalRefundAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_TotalBetValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_TotalPrizeValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_TotalValueOut", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_TotalValueIn", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingLogConn);
                var lstRs = db.GetListSP<AccountPlayGame>("SP_Account_Overview", pars);
                var overview = new AccountOverview();
                overview.RankID = ConvertUtil.ToInt(pars[1].Value);
                overview.VipPoint = ConvertUtil.ToInt(pars[2].Value);
                overview.RefundAmount = ConvertUtil.ToInt(pars[3].Value);
                overview.TotalRefundAmount = ConvertUtil.ToLong(pars[4].Value);
                return overview;
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

        public List<VpProgress> GetVpProgress(int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<VpProgress>("SP_VP_Progress", pars);
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

        public AccountPlayGame GetAccountGameProfit(int gameId, long accountId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_GameID", gameId);
                pars[1] = new SqlParameter("@_AccountID", accountId);
                pars[2] = new SqlParameter("@_TotalBetValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_TotalPrizeValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_AccountGame_Profit", pars);
                long totalBet = ConvertUtil.ToLong(pars[2].Value);
                long totalPrize = ConvertUtil.ToLong(pars[3].Value);
                AccountPlayGame result = new AccountPlayGame();
                result.GameID = gameId;
                result.TotalBetValue = totalBet;
                result.TotalPrizeValue = totalPrize;
                return result;
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
        public int GetAccountInOut(long accountId, out long TotalRecharge,out long TotalRechargeBank,out long TotalMomo,out long TotalValueInAgency,out long TotalValueOutAgency)
        {
            DBHelper db = null;

            TotalRecharge = 0;
            TotalRechargeBank = 0;
            TotalMomo = 0;
            TotalValueInAgency = 0;
            TotalValueOutAgency = 0;

            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserID", accountId);
                pars[1] = new SqlParameter("@_TotalRecharge", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_TotalRechargeBank", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_TotalMomo", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_TotalValueInAgency", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_TotalValueOutAgency", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_AccountInOut_Profit", pars);
                TotalRecharge = ConvertUtil.ToLong(pars[1].Value);
                TotalRechargeBank = ConvertUtil.ToLong(pars[2].Value);
                TotalMomo = ConvertUtil.ToLong(pars[3].Value);
                TotalValueInAgency = ConvertUtil.ToLong(pars[4].Value);
                TotalValueOutAgency = ConvertUtil.ToLong(pars[5].Value);
                return 1;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 0;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        public long GetAccountBalance(long accountId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[2];

                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Balance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Account_GetBalance", pars);
                return ConvertUtil.ToLong(pars[1].Value); ;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -1;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

    }
}