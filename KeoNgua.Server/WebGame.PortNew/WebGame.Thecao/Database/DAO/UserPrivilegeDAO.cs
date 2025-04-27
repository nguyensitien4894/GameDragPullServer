using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;

using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class UserPrivilegeDAO
    {
        private static readonly Lazy<UserPrivilegeDAO> _instance = new Lazy<UserPrivilegeDAO>(() => new UserPrivilegeDAO());

        public static UserPrivilegeDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// hàm lấy số tiền quy đổi thưởng
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="RankID"></param>
        /// <param name="VP"></param>
        /// <param name="RefundLimit"></param>
        /// <param name="GiftAmountLimit"></param>
        /// <param name="ExchangeRate"></param>
        /// <param name="Response"></param>
        public void UserPrivilegeRedeemVP(long UserID, long RankID, long VP, out long RefundLimit, out long GiftAmountLimit, out long ExchangeRate, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RankID", SqlDbType.BigInt);
                param[1].Value = RankID;
                param[2] = new SqlParameter("@_VP", SqlDbType.BigInt);
                param[2].Value = VP;
                param[3] = new SqlParameter("@_RefundLimit", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_GiftAmountLimit", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_ExchangeRate", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_Response", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_UserPrivilege_Redeem_VP", param.ToArray());
                Response = Convert.ToInt32(param[6].Value);
                if (Response == 1)
                {
                    RefundLimit = Convert.ToInt64(param[3].Value);
                    GiftAmountLimit = Convert.ToInt64(param[4].Value);
                    ExchangeRate = Convert.ToInt64(param[5].Value);
                }else
                {
                    RefundLimit = 0;
                    GiftAmountLimit = 0;
                    ExchangeRate = 0;

                }
             
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
            RefundLimit = 0;
            GiftAmountLimit = 0;
            ExchangeRate = 0;
            Response = -99;
        }



        /// <summary>
        /// hàm đổi thưởng
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="RefundAmount"></param>
        /// <param name="PriArtID"></param>
        /// <param name="ArtifactQuantity"></param>
        /// <param name="Description"></param>
        /// <param name="Response"></param>
        public void UserPrivilegeGratitude(long UserID, int RankID, string Description, out long RefundAmount, out long UserRedemptionID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[1].Value = RankID;
                param[2] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[2].Size = 1000;
                param[2].Value = Description;
                param[3] = new SqlParameter("@_RefundAmount", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_UserRedemptionID", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_UserPrivilege_Gratitude", param.ToArray());
                Response = Convert.ToInt32(param[5].Value);
                if (Response == 1)
                {
                    UserRedemptionID = Convert.ToInt64(param[3].Value);
                    RefundAmount = Convert.ToInt64(param[4].Value);
                }else
                {
                    UserRedemptionID = 0;
                    RefundAmount = 0;
                }
               
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
            UserRedemptionID = 0;
            RefundAmount = 0;
            Response = -99;
        }
        public void UserRedemption(long UserID, int RankID, string Note,  long RefundAmount, out long RemainBalance, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[1].Value = RankID;
                param[2] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[2].Size = 1000;
                param[2].Value = Note;
                param[3] = new SqlParameter("@_RefundAmount", SqlDbType.BigInt);
                param[3].Value = RefundAmount;
                param[4] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_User_Redemption", param.ToArray());
                Response = Convert.ToInt32(param[5].Value);
                if (Response == 1)
                {
                    RemainBalance = Convert.ToInt64(param[4].Value);
                  
                }
                else
                {
                    RemainBalance = 0;
                  
                }

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

            RemainBalance = 0;
            Response = -99;
        }

        public List<DTO.PrivilegeArtifacts> PrivilegeArtifacts_List(long ?PriArtID, long RankID, string ArtifactName, DateTime?CreateDate, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =      new SqlParameter[7];

                param[0] = new SqlParameter("@_PriArtID", SqlDbType.BigInt);
                param[0].Value = PriArtID.HasValue?PriArtID.Value:(object)DBNull.Value;
                param[1] = new SqlParameter("@_RankID", SqlDbType.BigInt);
                param[1].Value = RankID;
                param[2] = new SqlParameter("@_ArtifactName", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = ArtifactName;
                param[3] = new SqlParameter("@_CreateDate", SqlDbType.DateTime);
                param[3].Value = CreateDate.HasValue? CreateDate.Value:(object)DBNull.Value;
                param[4] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[4].Value = CurrentPage;
                param[5] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[5].Value = RecordPerpage;
                param[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
              
                var _lstPrivilegeArtifacts = db.GetListSP<DTO.PrivilegeArtifacts>("SP_PrivilegeArtifacts_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[6].Value);
                return _lstPrivilegeArtifacts;
               
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


        public List<PrivilegeType> PrivilegeTypeList(string PrivilegeCode)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_PrivilegeCode", SqlDbType.NVarChar);

                param[0].Value = PrivilegeCode;

                var _lstPrivilegeType = db.GetListSP<PrivilegeType>("SP_PrivilegeType_List", param.ToArray());
                return _lstPrivilegeType;
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

        public List<UserVip> UserRedemptionPrize(long userID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);

                param[0].Value = userID;

                var _lstPrivilegeType = db.GetListSP<UserVip>("SP_UserPrivilege_Prize", param.ToArray());
                return _lstPrivilegeType;
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

        public void UserPrivilegeCreate(long UserID, int RankID, long VP, long Point, DateTime EffectDate, DateTime ExpiredDate, DateTime RankedMonth, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[8];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[1].Value = RankID;
                param[2] = new SqlParameter("@_VP", SqlDbType.BigInt);
                param[2].Value = VP;
                param[3] = new SqlParameter("@_Point", SqlDbType.BigInt);
                param[3].Value = Point;
                param[4] = new SqlParameter("@_EffectDate", SqlDbType.DateTime);
                param[4].Value = EffectDate;
                param[5] = new SqlParameter("@_ExpiredDate", SqlDbType.DateTime);
                param[5].Value = ExpiredDate;
                param[6] = new SqlParameter("@_RankedMonth", SqlDbType.DateTime);
                param[6].Value = RankedMonth;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_UserPrivilege_Create", param.ToArray());
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
            Response = -99;
        }


        public List<DTO.UserRedemption> UserRedemptionHistoryList(long UserRedemptionID,long UserID, string UserName, int? RankID, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_UserName", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = UserName;
                param[2] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[2].Value = RankID.HasValue?RankID.Value:(object)DBNull.Value;
                param[3] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[3].Value = CurrentPage;
                param[4] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[4].Value = RecordPerpage;
                param[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
            
                var _lstUserRedemption = db.GetListSP<DTO.UserRedemption>("SP_UserRedemptionHistory_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[5].Value);
                return _lstUserRedemption;
                
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
    }
}