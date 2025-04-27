using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class ManagerDAO
    {
        private static readonly Lazy<ManagerDAO> _instance = new Lazy<ManagerDAO>(() => new ManagerDAO());

        public static ManagerDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<Artifacts> GetListArtifacts(int atfsid, string atfscode, string atfsname, bool? status, DateTime? createdate, 
            int currentpage, int recordperpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_ArtifactID", atfsid);
                pars[1] = new SqlParameter("@_ArtifactCode", atfscode);
                pars[2] = new SqlParameter("@_ArtifactName", atfsname);
                pars[3] = new SqlParameter("@_Status", status);
                pars[4] = new SqlParameter("@_CreateDate", createdate);
                pars[5] = new SqlParameter("@_CurrentPage", currentpage);
                pars[6] = new SqlParameter("@_RecordPerpage", recordperpage);
                pars[7] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var lstRs = db.GetListSP<Artifacts>("SP_Artifacts_List", pars);
                totalrecord = Int32.Parse(pars[7].Value.ToString());
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

        public int AddOrUpdateArtifacts(int atfsid, string atfscode, string atfsname, long price, bool status, string des, 
            long createuser, DateTime createdate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_ArtifactID", atfsid);
                pars[1] = new SqlParameter("@_ArtifactCode", atfscode);
                pars[2] = new SqlParameter("@_ArtifactName", atfsname);
                pars[3] = new SqlParameter("@_Price", price);
                pars[4] = new SqlParameter("@_Status", status);
                pars[5] = new SqlParameter("@_Description", des);
                pars[6] = new SqlParameter("@_CreateUser", createuser);
                pars[7] = new SqlParameter("@_CreateDate", createdate);
                pars[8] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Artifacts_Handle", pars);
                return Int32.Parse(pars[8].Value.ToString());
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

        public List<PrivilegeArtifacts> GetListPrivilegeArtifacts(long priartid, int rankid, string atfsname, DateTime? createdate, 
            int currentpage, int recordperpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_PriArtID", priartid);
                pars[1] = new SqlParameter("@_RankID", rankid);
                pars[2] = new SqlParameter("@_ArtifactName", atfsname);
                pars[3] = new SqlParameter("@_CreateDate", createdate);
                pars[4] = new SqlParameter("@_CurrentPage", currentpage);
                pars[5] = new SqlParameter("@_RecordPerpage", recordperpage);
                pars[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var lstRs = db.GetListSP<PrivilegeArtifacts>("SP_PrivilegeArtifacts_List", pars);
                totalrecord = Int32.Parse(pars[6].Value.ToString());
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

        public int AddOrUpdatePrivilegeArtifacts(long priartid, int rankid, int atfsid, int quantity, int remainquantity, 
            long totalprize, bool status, string des, long createuser)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_PriArtID", priartid);
                pars[1] = new SqlParameter("@_RankID", rankid);
                pars[2] = new SqlParameter("@_ArtifactID", atfsid);
                pars[3] = new SqlParameter("@_Quantity", quantity);
                pars[4] = new SqlParameter("@_RemainQuantity", remainquantity);
                pars[5] = new SqlParameter("@_TotalPrize", totalprize);
                pars[6] = new SqlParameter("@_Status", status);
                pars[7] = new SqlParameter("@_Description", des);
                pars[8] = new SqlParameter("@_CreateUser", createuser);
                pars[9] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_PrivilegeArtifacts_Handle", pars);
                return Int32.Parse(pars[9].Value.ToString());
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

        public List<UserRedemption> GetListUserRedemption(long userRdtId, long userId, string username, int? rankid, DateTime? createdate, int serviceid, 
            int currentpage, int recordperpage, out int totalrecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_UserRedemptionID", userRdtId);
                pars[1] = new SqlParameter("@_UserID", userId);
                pars[2] = new SqlParameter("@_UserName", username);
                pars[3] = new SqlParameter("@_RankID", rankid);
                pars[4] = new SqlParameter("@_CreateDate", createdate);
                pars[5] = new SqlParameter("@_ServiceID", serviceid);
                pars[6] = new SqlParameter("@_CurrentPage", currentpage);
                pars[7] = new SqlParameter("@_RecordPerpage", recordperpage);
                pars[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                var lstRs = db.GetListSP<UserRedemption>("SP_UserRedemption_List", pars);
                totalrecord = Int32.Parse(pars[8].Value.ToString());
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

        public int AddOrUpdateUserRedemption(long userrdtid, long userid, long refundamount, long priartid, int quantity, int rankid, long vp, long point, 
            DateTime rankedmonth, bool status, string des, DateTime refundrevdate, DateTime giftrevdate, long createuser)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[15];
                pars[0] = new SqlParameter("@_UserRedemptionID", userrdtid);
                pars[1] = new SqlParameter("@_UserID", userid);
                pars[2] = new SqlParameter("@_RefundAmount", refundamount);
                pars[3] = new SqlParameter("@_PriArtID", priartid);
                pars[4] = new SqlParameter("@_Quantity", quantity);
                pars[5] = new SqlParameter("@_RankID", rankid);
                pars[6] = new SqlParameter("@_VP", vp);
                pars[7] = new SqlParameter("@_Point", point);
                pars[8] = new SqlParameter("@_RankedMonth", rankedmonth);
                pars[9] = new SqlParameter("@_Status", status);
                pars[10] = new SqlParameter("@_Description", des);
                pars[11] = new SqlParameter("@_RefundReceiveDate", refundrevdate);
                pars[12] = new SqlParameter("@_GiftReceiveDate", giftrevdate);
                pars[13] = new SqlParameter("@_CreateUser", createuser);
                pars[14] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_UserRedemption_Handle", pars);
                return Int32.Parse(pars[14].Value.ToString());
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
    }
}