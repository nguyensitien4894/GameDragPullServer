using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class AgencyDAO
    {
        private static readonly Lazy<AgencyDAO> _instance = new Lazy<AgencyDAO>(() => new AgencyDAO());

        public static AgencyDAO Instance
        {
            get { return _instance.Value; }
        }

        public void AdminWithdrawAgencyWallet(long AdminId, long AgencyID, int ServiceID,int WalletType, long Amount, string Note, out long TransID, out long Wallet, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];
                param[0] = new SqlParameter("@_AdminId", SqlDbType.BigInt);
                param[0].Value = AdminId;
                param[1] = new SqlParameter("@_AgencyID", SqlDbType.BigInt);
                param[1].Value = AgencyID;
                param[2] = new SqlParameter("@_WalletType", SqlDbType.Int);
                param[2].Value = WalletType;
                param[3] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[3].Value = Amount;

                param[4] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[4].Size = 400;
                param[4].Value = Note;
                param[5] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[5].Value = ServiceID;
                
                param[6] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_Wallet", SqlDbType.BigInt);
                param[7].Direction = ParameterDirection.Output;
                param[8] = new SqlParameter("@_Response", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;


                db.ExecuteNonQuerySP("SP_Admin_Withdraw_Agency_Wallet", param.ToArray());
                TransID = ConvertUtil.ToLong(param[6].Value);
                Wallet = ConvertUtil.ToLong(param[7].Value);
                Response = ConvertUtil.ToInt(param[8].Value);
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
            TransID = 0;
            Wallet = 0;
            Response = -99;
        }
        public void AgencyRefund(int ServiceID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[0].Value = ServiceID;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Admin_Withdraw_Giftcode_Agency", param.ToArray());
                Response = ConvertUtil.ToInt(param[1].Value);
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
        public List<TmpAgencyC2> ImportAgencyC2(DataTable listAgency, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_AgencyTable", SqlDbType.Structured);
                param[0].Value = listAgency;

                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;

                var _lstGetList = db.GetListSP<TmpAgencyC2>("SP_Agency_Import_File", param.ToArray());
                Response = Convert.ToInt32(param[1].Value);
                return _lstGetList;

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
            Response = 0;
            return null;
        }

        public List<Agency> GetList(string accountName, string phoneNo, int? accountLevel, int? status, long? parrentId,
             int serviceid, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_AccountName", accountName);
                pars[1] = new SqlParameter("@_PhoneNo", phoneNo);
                pars[2] = new SqlParameter("@_AccountLevel", accountLevel);
                pars[3] = new SqlParameter("@_Status", status);
                pars[4] = new SqlParameter("@_ParentID", parrentId);
                pars[5] = new SqlParameter("@_ServiceID", serviceid);
                pars[6] = new SqlParameter("@_CurrentPage", currentPage);
                pars[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<Agency>("SP_Agency_Search", pars);
                totalRecord = ConvertUtil.ToInt(pars[8].Value);
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
                {
                    db.Close();
                }
            }
        }

        public void Delete(long AccountId, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
             new SqlParameter[2];

                param[0] = new SqlParameter("@_AccountId", SqlDbType.BigInt);
                param[0].Value = AccountId;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Agency_Delete", param.ToArray());
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

        public Agency GetById(long AccountID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;

                var _Agency = db.GetInstanceSP<Agency>("SP_Agency_GetByID", param.ToArray());
                return _Agency;
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
        public Agency AdminGetById(long AccountID, long serviceid)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                pars[0].Value = AccountID;
                pars[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                pars[1].Value = serviceid;

                db = new DBHelper(Config.BettingConn);
                var _Agency = db.GetInstanceSP<Agency>("SP_Agency_Admin_GetByID", pars);
                return _Agency;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            return null;
        }

        public int AgencyCreate(string accountName, string displayName, string fullName, int accountLevel, long parentId,
            int status, string areaName, string phoneNo, string phoneDisplay, string fbLink, string password, long accountId, int orderNum, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[16];
                pars[0] = new SqlParameter("@_AccountName", accountName);
                pars[1] = new SqlParameter("@_DisplayName", displayName);
                pars[2] = new SqlParameter("@_FullName", fullName);
                pars[3] = new SqlParameter("@_AccountLevel", accountLevel);
                pars[4] = new SqlParameter("@_ParentID", parentId);
                pars[5] = new SqlParameter("@_Status", status);
                pars[6] = new SqlParameter("@_AreaName", areaName);
                pars[7] = new SqlParameter("@_PhoneNo", phoneNo);
                pars[8] = new SqlParameter("@_PhoneDisplay", phoneDisplay);
                pars[9] = new SqlParameter("@_FBLink", fbLink ?? (object)DBNull.Value);
                pars[10] = new SqlParameter("@_Password", password);
                pars[11] = new SqlParameter("@_AccountId", accountId);
                pars[12] = new SqlParameter("@_OrderNum", orderNum);
                pars[13] = new SqlParameter("@_TelegramID", (object)DBNull.Value);
                pars[14] = new SqlParameter("@_ServiceID", serviceid);
                pars[15] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Agency_Create", pars);
                return ConvertUtil.ToInt(pars[15].Value);
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

        public int AgencyUpdate(long accountId, string fullName, int accountLevel, long parentId, int? status, string areaName,
            string phoneNo, string phoneDisplay, string fbLink, int orderNum,string telelink,string zalolink)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[16];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_FullName", fullName);
                pars[2] = new SqlParameter("@_AccountLevel", accountLevel);
                pars[3] = new SqlParameter("@_ParentID", parentId);
                pars[4] = new SqlParameter("@_Status", status);
                pars[5] = new SqlParameter("@_AreaName", areaName);
                pars[6] = new SqlParameter("@_PhoneNo", phoneNo);
                pars[7] = new SqlParameter("@_PhoneDisplay", phoneDisplay);
                pars[8] = new SqlParameter("@_FBLink", fbLink);
                pars[9] = new SqlParameter("@_Password", null);
                pars[10] = new SqlParameter("@_OrderNum", orderNum);
                pars[11] = new SqlParameter("@_TelegramID", null);
                pars[12] = new SqlParameter("@_ServiceID", null);
                pars[13] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[14] = new SqlParameter("@_TeleLink", telelink);
                pars[15] = new SqlParameter("@_ZaloLink", zalolink);
                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Agency_Update2", pars);
                return ConvertUtil.ToInt(pars[13].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        public List<AgencyRevenue> GetAgencyRevenue(long accountId, int level, DateTime? startDate, DateTime? endDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_Level", level);
                pars[2] = new SqlParameter("@_StartDate", SqlDbType.DateTime);
                pars[2].Value = startDate;
                pars[3] = new SqlParameter("@_EndDate", SqlDbType.DateTime);
                pars[3].Value = endDate;
                var rs = db.GetListSP<AgencyRevenue>("SP_Agency_Revenue", pars);
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

        //1:Username, 2:Phone, 3:email, 4:ID
        public Agency GetAgencyInfo(int keyType, string keyValue, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_KeyType", keyType);
                pars[1] = new SqlParameter("@_KeyValue", keyValue);
                pars[2] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var rs = db.GetInstanceSP<Agency>("SP_Agency_GetInfo", pars);
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

        public CashFlow GetAgencyCashFlow(long accountId, int accountType, DateTime? startDate, DateTime? endDate, int serviceId)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_AccountType", accountType);
                pars[2] = new SqlParameter("@_StartDate", startDate);
                pars[3] = new SqlParameter("@_EndDate", endDate);
                pars[4] = new SqlParameter("@_ServiceID", serviceId);

                db = new DBHelper(Config.BettingConn);
                var rs = db.GetInstanceSP<CashFlow>("SP_Agency_CashFlow", pars);
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
                {
                    db.Close();
                }
            }
        }


        public List<AgencyRaceTopAward> GetListAgencyRaceTopAward(DateTime? raceDate, int serviceid, bool isClosed)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_RaceDate", raceDate);
                pars[1] = new SqlParameter("@_ServiceID", serviceid);
                pars[2] = new SqlParameter("@_IsClosed", isClosed);

                db = new DBHelper(Config.BettingConn);
                var _lstRaceTop = db.GetListSP<AgencyRaceTopAward>("SP_AgencyRaceTop_Close_List", pars);
                return _lstRaceTop;
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

        public void AgencyRaceTopAwardCreate(long AccountID, int AccountLevel, string AccountName,string DisplayName,string FullName, long ParentID, string ParentName, int PrizeID, DateTime RaceDate, DateTime CreateDate, bool IsClosed,  long TotalAmount, decimal TotalVP, long PrizeValue,int ServiceID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[16];

                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_AccountName", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = AccountName;
                param[2] = new SqlParameter("@_AccountLevel", SqlDbType.SmallInt);
                param[2].Value = AccountLevel;
                param[3] = new SqlParameter("@_ParentID", SqlDbType.BigInt);
                param[3].Value = ParentID;
                param[4] = new SqlParameter("@_ParentName", SqlDbType.NVarChar);
                param[4].Size = 200;
                param[4].Value = ParentName;
                param[5] = new SqlParameter("@_TotalAmount", SqlDbType.BigInt);
                param[5].Value = TotalAmount;
                param[6] = new SqlParameter("@_TotalVP", SqlDbType.Decimal);
                param[6].Value = TotalVP;
                param[7] = new SqlParameter("@_PrizeID", SqlDbType.Int);
                param[7].Value = PrizeID;
                param[8] = new SqlParameter("@_PrizeValue", SqlDbType.BigInt);
                param[8].Value = PrizeValue;
                param[9] = new SqlParameter("@_RaceDate", SqlDbType.DateTime);
                param[9].Value = RaceDate;
                param[10] = new SqlParameter("@_IsClosed", SqlDbType.Bit);
                param[10].Value = IsClosed;
                param[11] = new SqlParameter("@_CreateDate", SqlDbType.DateTime);
                param[11].Value = CreateDate;
                param[12] = new SqlParameter("@_Response", SqlDbType.Int);
                param[12].Direction = ParameterDirection.Output;
                param[13] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[13].Value = ServiceID;

                param[14] = new SqlParameter("@_DisplayName", SqlDbType.NVarChar);
                param[14].Size = 200;
                param[14].Value = DisplayName;

                param[15] = new SqlParameter("@_FullName", SqlDbType.NVarChar);
                param[15].Size = 200;
                param[15].Value = FullName;

                db.ExecuteNonQuerySP("SP_AgencyRaceTopAward_Create", param.ToArray());
                Response = Convert.ToInt32(param[12].Value);
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
        public void AgencyRaceTopWeekClose(int ServiceID, DateTime RaceDate, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =   new SqlParameter[3];
                param[0] = new SqlParameter("@_RaceDate", SqlDbType.DateTime);
                param[0].Value = RaceDate;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
               

                db.ExecuteNonQuerySP("SP_AgencyRaceTop_Week_Close", param.ToArray());
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


        public List<AgencyRaceTopAward> GetAgencyRaceTopCloseList(DateTime? raceDate, int serviceid, bool isClosed)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_RaceDate", raceDate);
                pars[1] = new SqlParameter("@_ServiceID", serviceid);
                pars[2] = new SqlParameter("@_IsClosed", isClosed);

                db = new DBHelper(Config.BettingConn);
                var _lstRaceTop = db.GetListSP<AgencyRaceTopAward>("SP_AgencyRaceTop_Close_List", pars);
                return _lstRaceTop;
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


        public List<AgencyRaceTopAward> GetPrepareListAgencyRaceTop(DateTime? raceDate, int serviceid, bool? isClosed)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_RaceDate", raceDate);
                pars[1] = new SqlParameter("@_ServiceID", serviceid);
                pars[2] = new SqlParameter("@_IsClosed", isClosed??(object)DBNull.Value);

                db = new DBHelper(Config.BettingConn);
                var _lstRaceTop = db.GetListSP<AgencyRaceTopAward>("SP_AgencyRaceTop_PreClose_List", pars);
                return _lstRaceTop;
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
        public List<AgencyRaceTop> GetListAgencyRaceTop(DateTime? raceDate, DateTime? fromDate, DateTime? toDate, int serviceid, bool isClosed)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_RaceDate", raceDate);
                pars[1] = new SqlParameter("@_FromDate", fromDate);
                pars[2] = new SqlParameter("@_ToDate", toDate);
                pars[3] = new SqlParameter("@_StartPos", 1);
                pars[4] = new SqlParameter("@_Quantity", 50);
                pars[5] = new SqlParameter("@_ServiceID", serviceid);
                pars[6] = new SqlParameter("@_IsClosed", isClosed);

                db = new DBHelper(Config.BettingConn);
                var _lstRaceTop = db.GetListSP<AgencyRaceTop>("SP_AgencyRaceTop_List", pars);
                return _lstRaceTop;
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

        public List<RaceTopListRaceDate> GetAgencyRaceTopListRaceDate(int serviceid)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<RaceTopListRaceDate>("SP_AgencyRaceTop_ListRaceDate", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            return null;
        }

        public List<AgencyTransaction> GetAgencyTransaction(long agencyid, int userType, string partnername, int transtype, int status, DateTime? fromDate, DateTime? toDate, int serviceid,int CurrentPage,int RecordPerpage,out long TotalRecord)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@_AgencyID", agencyid);
                pars[1] = new SqlParameter("@_UserType", userType);
                pars[2] = new SqlParameter("@_PartnerName", partnername);
                pars[3] = new SqlParameter("@_TransferType", transtype);
                pars[4] = new SqlParameter("@_Status", status);
                pars[5] = new SqlParameter("@_TransCode", null);
                pars[6] = new SqlParameter("@_FromDate", fromDate);
                pars[7] = new SqlParameter("@_ToDate", toDate);
                pars[8] = new SqlParameter("@_ServiceID", serviceid);

                pars[9] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                pars[9].Value = CurrentPage;
                pars[10] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                pars[10].Value = RecordPerpage;
                pars[11] = new SqlParameter("@_TotalRecord", SqlDbType.BigInt);
                pars[11].Direction = ParameterDirection.Output;
                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<AgencyTransaction>("SP_Agency_WalletLogs_List", pars);
                TotalRecord = ConvertUtil.ToLong(pars[11].Value);
                return lstRs;
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

        /// Thống kê tổng đại lý với c1
        public List<RootAgencyReport> AgencyC1GeneralRevenue(long accountid, DateTime? fromDate, DateTime? toDate, int serviceid)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountID", accountid);
                pars[1] = new SqlParameter("@_FromDate", fromDate ?? (object)DBNull.Value);
                pars[2] = new SqlParameter("@_ToDate", toDate ?? (object)DBNull.Value);
                pars[3] = new SqlParameter("@_ServiceID", serviceid);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<RootAgencyReport>("SP_Agency_General_Revenue", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            return null;
        }

        /// chi tiết tổng đại lý với c1
        public List<ReportAgencyGeneralTrans> AgencyGeneralTrans(long accountId, int TransferType, string PartnerName, DateTime? FromDate, DateTime? ToDate, int serviceId)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                pars[0].Value = accountId;
                pars[1] = new SqlParameter("@_TransferType", SqlDbType.Int);
                pars[1].Value = TransferType;
                pars[2] = new SqlParameter("@_PartnerName", SqlDbType.VarChar, 50);
                pars[2].Value = PartnerName;
                pars[3] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                pars[3].Value = FromDate ?? (object)DBNull.Value;
                pars[4] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                pars[4].Value = ToDate ?? (object)DBNull.Value;
                pars[5] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                pars[5].Value = serviceId;

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<ReportAgencyGeneralTrans>("SP_Agency_General_Trans", pars);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            return null;
        }

        /// thống kê tổng luồng C1 với user 
        public List<ReportAgencyL1CashFlowUsers> AgencyL1CashFlowUsers(long AccountID, DateTime? StartDate, DateTime? EndDate, int serviceid)
        {
            DBHelper db = null;
            try
            {

                SqlParameter[] pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                pars[0].Value = AccountID;
                pars[1] = new SqlParameter("@_StartDate", SqlDbType.DateTime);
                pars[1].Value = StartDate ?? (object)DBNull.Value;
                pars[2] = new SqlParameter("@_EndDate", SqlDbType.DateTime);
                pars[2].Value = EndDate ?? (object)DBNull.Value;
                pars[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                pars[3].Value = serviceid;

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<ReportAgencyL1CashFlowUsers>("SP_Agency_L1_CashFlow_Users", pars);
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

        /// <param name="PartnerType">1:User;2:Agency;3:Admin</param>
        /// <param name="TransferType">0:all;1:OUT;2:IN</param>
        public List<WalletLogs> WalletLogsAgencyList(long AccountID, int PartnerType, int TransferType, string PartnerName, int Status, string TransCode, DateTime? FromDate,
            DateTime? ToDate, int serviceid, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                pars[0].Value = AccountID;
                pars[1] = new SqlParameter("@_PartnerType", SqlDbType.Int);
                pars[1].Value = PartnerType;
                pars[2] = new SqlParameter("@_PartnerName", SqlDbType.VarChar);
                pars[2].Size = 50;
                pars[2].Value = PartnerName;
                pars[3] = new SqlParameter("@_TransferType", SqlDbType.Int);
                pars[3].Value = TransferType;
                pars[4] = new SqlParameter("@_Status", SqlDbType.Int);
                pars[4].Value = Status;
                pars[5] = new SqlParameter("@_TransCode", SqlDbType.VarChar);
                pars[5].Size = 50;
                pars[5].Value = TransCode;
                pars[6] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                pars[6].Value = FromDate ?? (object)DBNull.Value;
                pars[7] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                pars[7].Value = ToDate ?? (object)DBNull.Value;
                pars[8] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                pars[8].Value = serviceid;
                pars[9] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                pars[9].Value = CurrentPage;
                pars[10] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                pars[10].Value = RecordPerpage;
                pars[11] = new SqlParameter("@_TotalRecord", SqlDbType.BigInt);
                pars[11].Direction = ParameterDirection.Output;

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<WalletLogs>("SP_WalletLogs_Agency_List", pars);
                TotalRecord = Convert.ToInt32(pars[11].Value);
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
            TotalRecord = 0;
            return null;
        }

        public List<CashFlowOfEachAgency> GetCashFlowOfEachAgency(string accountName, string nickName, string phoneNo, int status, DateTime fromDate, DateTime toDate, int serviceId)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountName", accountName);
                pars[1] = new SqlParameter("@_DisplayName", nickName);
                pars[2] = new SqlParameter("@_PhoneNo", phoneNo);
                pars[3] = new SqlParameter("@_Status", status);
                pars[4] = new SqlParameter("@_FromDate", fromDate);
                pars[5] = new SqlParameter("@_ToDate", toDate);
                pars[6] = new SqlParameter("@_ServiceID", serviceId);

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<CashFlowOfEachAgency>("SP_Agency_L1_List", pars);
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
                {
                    db.Close();
                }
            }
        }

        public List<Agency> GetListAgencyTotal(string accountName, string phoneNo, int accountLevel, int status, long parentId, int serviceId, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                SqlParameter[] pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_AccountName", accountName);
                pars[1] = new SqlParameter("@_PhoneNo", phoneNo);
                pars[2] = new SqlParameter("@_AccountLevel", accountLevel);
                pars[3] = new SqlParameter("@_Status", status);
                pars[4] = new SqlParameter("@_ParentID", parentId);
                pars[5] = new SqlParameter("@_ServiceID", serviceId);
                pars[6] = new SqlParameter("@_CurrentPage", currentPage);
                pars[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<Agency>("SP_Agency_Admin_List", pars);
                totalRecord = ConvertUtil.ToInt(pars[8].Value);
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

        public int ChangePasswordCore(string username, string password, int serviceid)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountName", username);
                pars[1] = new SqlParameter("@_Password", password);
                pars[2] = new SqlParameter("@_ServiceID", serviceid);
                pars[3] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Agency_Change_Password_Cskh", pars);
                return ConvertUtil.ToInt(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            return -99;
        }

        public int UpdateLoginFail(string username, bool status, int serviceid, out int failnumber)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_UserName", username);
                pars[1] = new SqlParameter("@_LoginStatus", status);
                pars[2] = new SqlParameter("@_ServiceID", serviceid);
                pars[3] = new SqlParameter("@_LoginFailNumber", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Agency_UpdateLoginFail", pars);
                failnumber = ConvertUtil.ToInt(pars[3].Value);
                return ConvertUtil.ToInt(pars[4].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                failnumber = 0;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
            return -99;
        }
    }
}