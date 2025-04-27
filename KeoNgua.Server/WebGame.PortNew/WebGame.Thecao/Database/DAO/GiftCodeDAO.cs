using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using TraditionGame.Utilities;
using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class GiftCodeDAO
    {
        private static readonly Lazy<GiftCodeDAO> _instance = new Lazy<GiftCodeDAO>(() => new GiftCodeDAO());

        public static GiftCodeDAO Instance
        {
            get { return _instance.Value; }
        }

        public int ReceiveGiftCode(long UserID, string GiftCode, out long balance,out long giftCodeId,out long GiftCodeAmount)
        {
            balance = 0;
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_UserID", UserID));
                param.Add(new SqlParameter("@_GiftCode", GiftCode));
                param.Add(new SqlParameter("@_Balance", SqlDbType.BigInt) { Direction = ParameterDirection.Output });
                
                param.Add(new SqlParameter("@_GiftCodeID", SqlDbType.BigInt) { Direction = ParameterDirection.Output });
                param.Add(new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output });
                param.Add(new SqlParameter("@_GiftCodeAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output });
                
                db.ExecuteNonQuerySP("SP_GiftCode_Receive", param.ToArray());

                int response = Convert.ToInt32(param[4].Value);
                if (response == 1)
                {
                    balance = Convert.ToInt64(param[2].Value);
                    giftCodeId= Convert.ToInt64(param[3].Value);
                    GiftCodeAmount = Convert.ToInt64(param[5].Value); ;
                }else
                {
                    balance = 0;
                    giftCodeId = 0;
                    GiftCodeAmount = 0;

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
            GiftCodeAmount = 0;
            balance = 0;
            giftCodeId = 0;
            return -99;
        }
        public List<GameGiftCode> GetListGameGiftCode(long giftCodeID, long campaignID, string giftCode, bool? status)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_GiftCodeID", giftCodeID);
                pars[1] = new SqlParameter("@_CampaignID", campaignID);
                pars[2] = new SqlParameter("@_GiftCode", giftCode);
                pars[3] = new SqlParameter("@_Status", status);
                var lstRs = db.GetListSP<GameGiftCode>("SP_Giftcode_List", pars);
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

        public int AddOrUpdateGameGiftCode(long giftCodeID, long campaignID, string giftCode, long moneyReward, int giftCodeType,
            long totalUsed, long limitQuota, long userID, bool status, DateTime expiredDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[11];
                pars[0] = new SqlParameter("@_GiftCodeID", giftCodeID);
                pars[1] = new SqlParameter("@_CampaignID", campaignID);
                pars[2] = new SqlParameter("@_GiftCode", giftCode);
                pars[3] = new SqlParameter("@_MoneyReward", moneyReward);
                pars[4] = new SqlParameter("@_GiftCodeType", giftCodeType);
                pars[5] = new SqlParameter("@_TotalUsed ", totalUsed);
                pars[6] = new SqlParameter("@_LimitQuota", limitQuota);
                pars[7] = new SqlParameter("@_UserID", userID);
                pars[8] = new SqlParameter("@_Status", status);
                pars[9] = new SqlParameter("@_ExpiredDate", expiredDate);
                pars[10] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Giftcode_Handle", pars);
                return Int32.Parse(pars[10].Value.ToString());
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