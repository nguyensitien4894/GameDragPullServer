using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class GiftCodeDAO
    {
        private static readonly Lazy<GiftCodeDAO> _instance = new Lazy<GiftCodeDAO>(() => new GiftCodeDAO());

        public static GiftCodeDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<GameCampaign> GetCampaignAutoComplete(string name, int ServiceID)
        {
            var lstData = ParamConfigDAO.Instance.GetListGameCampaign(0, name, null, true, null, null, ServiceID);
            return lstData;
        }

        public int GiftcodeGenerate(long generatorId, int generatorType, int walletType, string campaignName, int giftcodeType, long moneyReward,
           DateTime expiredDate, string description, int quantity, int serviceId, out long totalMoneyUsed, out long campaignId, out long remainWallet)
        {
            DBHelper db = null;
            int response = -1;
            try
            {
                var pars = new SqlParameter[14];
                pars[0] = new SqlParameter("@_GeneratorID", generatorId);
                pars[1] = new SqlParameter("@_GeneratorType", generatorType);
                pars[2] = new SqlParameter("@_WalletType", walletType);
                pars[3] = new SqlParameter("@_CampaignName", campaignName);
                pars[4] = new SqlParameter("@_GiftcodeType", giftcodeType);
                pars[5] = new SqlParameter("@_MoneyReward", moneyReward);
                pars[6] = new SqlParameter("@_ExpiredDate", expiredDate);
                pars[7] = new SqlParameter("@_Description", description);
                pars[8] = new SqlParameter("@_Quantity", quantity);
                pars[9] = new SqlParameter("@_ServiceID", serviceId);
                pars[10] = new SqlParameter("@_TotalMoneyUsed", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[11] = new SqlParameter("@_CampaignID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[12] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[13] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.BettingConn);
                db.ExecuteNonQuerySP("SP_Giftcode_Generate",0, pars);
                totalMoneyUsed = ConvertUtil.ToLong(pars[10].Value);
                campaignId = ConvertUtil.ToLong(pars[11].Value);
                remainWallet = ConvertUtil.ToLong(pars[12].Value);
                response = ConvertUtil.ToInt(pars[13].Value);
                return response;
            }
            catch (Exception ex)
            {
                totalMoneyUsed = 0;
                campaignId = 0;
                remainWallet = 0;
                NLogManager.PublishException(ex);
            }
            finally
            {
                NLogManager.LogMessage(string.Format("GiftcodeGenerate--GenID:{0}-GenType:{1}-WalletType:{2}-Cam:{3}-GiftType:{4}-Money:{5}-Quan:{6}-S:{7}-R:{8}",
                    generatorId, generatorType, walletType, campaignName, giftcodeType, moneyReward, quantity, serviceId, response));
                if (db != null)
                    db.Close();
            }
            return -99;
        }

        public List<GameGiftCode> GetListGameGiftCodeCheck(string giftCode)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_GiftCode", giftCode);

                var _lstGameGiftCode = db.GetListSP<GameGiftCode>("SP_Giftcode_Check", param.ToArray());

                return _lstGameGiftCode;
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
        public void GiftcodeCreate(int GeneratorType, int GiftcodeType, int Quantity, string CampaignName, string Description, DateTime ExpiredDate, long MoneyReward, long GeneratorID,int  serviceid, out int Response  , out long TotalMoneyUsed, out long CampaignID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[13];
                param[0] = new SqlParameter("@_GeneratorID", SqlDbType.BigInt);
                param[0].Value = GeneratorID;
                param[1] = new SqlParameter("@_GeneratorType", GeneratorType);
                param[1].Value = GeneratorType;
                param[2] = new SqlParameter("@_CampaignName", SqlDbType.NVarChar);
                param[2].Value = CampaignName;
                param[2].Size = 200;
                param[3] = new SqlParameter("@_GiftcodeType", SqlDbType.Int);
                param[3].Value = GiftcodeType;
                param[4] = new SqlParameter("@_MoneyReward", SqlDbType.BigInt);
                param[4].Value = MoneyReward;
                param[5] = new SqlParameter("@_ExpiredDate", SqlDbType.DateTime);
                param[5].Value = ExpiredDate;
                param[6] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[6].Size = 1000;
                param[6].Value = Description;

                param[7] = new SqlParameter("@_Quantity", SqlDbType.Int);
                param[7].Value = Quantity;
                param[8] = new SqlParameter("@_TotalMoneyUsed", SqlDbType.BigInt);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_CampaignID", SqlDbType.Int);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@_Response", SqlDbType.Int);
                param[10].Direction = ParameterDirection.Output;
                param[11] = new SqlParameter("@_ServiceID", serviceid);
                param[12] = new SqlParameter("@_WalletType", 1);
                db.ExecuteNonQuerySP("SP_Giftcode_Create", param.ToArray());
                Response = Convert.ToInt32(param[10].Value);
                if (Response == 1)
                {
                    CampaignID= Convert.ToInt64(param[9].Value);
                    TotalMoneyUsed = Convert.ToInt64(param[8].Value);

                }else
                {
                    CampaignID = 0;
                    TotalMoneyUsed = 0;

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
            CampaignID = 0;
            TotalMoneyUsed = 0;
            Response = -99;
        }
        public List<GameGiftCode> GetListGameGiftCode2(long accountId, int accountType, int giftCodeType, string CampaignName,
      string giftCode, bool? status, string NickName,int  serviceid,DateTime? FromDate, DateTime? ToDate, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[13];

                param[0] = new SqlParameter("@_GeneratorID", accountId);
                param[1] = new SqlParameter("@_GeneratorType", accountType);
                param[2] = new SqlParameter("@_GiftCodeType", giftCodeType);
                param[3] = new SqlParameter("@_CampaignName", CampaignName);
                param[4] = new SqlParameter("@_GiftCode", giftCode);
                param[5] = new SqlParameter("@_Status", status);
                param[6] = new SqlParameter("@_CurrentPage", currentPage);
                param[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                param[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                param[9] = new SqlParameter("@_NickName", NickName);
                param[10] = new SqlParameter("@_FromDate", FromDate);
                param[11] = new SqlParameter("@_ToDate", ToDate);
                param[12] = new SqlParameter("@_ServiceID", serviceid);
                var _lstGameGiftCode = db.GetListSP<GameGiftCode>("SP_Giftcode_Admin_List", param.ToArray());

                totalRecord = Convert.ToInt32(param[8].Value);
                return _lstGameGiftCode;
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
        public List<GameGiftCode> GetListGameGiftCode(long accountId, int accountType, int giftCodeType, long? campaignId, 
            string giftCode, bool? status, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_AccountID", accountId);
                param[1] = new SqlParameter("@_AccountType", accountType);
                param[2] = new SqlParameter("@_GiftCodeType", giftCodeType);
                param[3] = new SqlParameter("@_CampaignID", campaignId);
                param[4] = new SqlParameter("@_GiftCode", giftCode);
                param[5] = new SqlParameter("@_Status", status);
                param[6] = new SqlParameter("@_CurrentPage", currentPage);
                param[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                param[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output};

                var _lstGameGiftCode = db.GetListSP<GameGiftCode>("SP_Giftcode_List", param.ToArray());
                totalRecord = Convert.ToInt32(param[8].Value);
                return _lstGameGiftCode;
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


        public List<GiftcodeExport> GetGiftCodeExport(long accountId, int accountType, int giftCodeType, long? campaignId,
            string giftCode, bool? status, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_AccountID", accountId);
                param[1] = new SqlParameter("@_AccountType", accountType);
                param[2] = new SqlParameter("@_GiftCodeType", giftCodeType);
                param[3] = new SqlParameter("@_CampaignID", campaignId);
                param[4] = new SqlParameter("@_GiftCode", giftCode);
                param[5] = new SqlParameter("@_Status", status);
                param[6] = new SqlParameter("@_CurrentPage", currentPage);
                param[7] = new SqlParameter("@_RecordPerpage", recordPerpage);
                param[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var _lstGameGiftCode = db.GetListSP<GiftcodeExport>("SP_Giftcode_List", param.ToArray());
                totalRecord = Convert.ToInt32(param[8].Value);
                return _lstGameGiftCode;
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

        public GiftcodeProgress GetGiftcodeProgress(long? accountId, int? accountType,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountID", accountId);
                pars[1] = new SqlParameter("@_AccountType", accountType);
                pars[2] = new SqlParameter("@_QuantityTotal", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_AvailableQuantityTotal", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_ValueTotal", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_AvailableVallueTotal", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_Giftcode_Progress", pars);
                GiftcodeProgress rs = new GiftcodeProgress();
                rs.QuantityTotal = ConvertUtil.ToInt(pars[2].Value);
                rs.AvailableQuantityTotal = ConvertUtil.ToInt(pars[3].Value);
                rs.ValueTotal = ConvertUtil.ToInt(pars[4].Value);
                rs.AvailableVallueTotal = ConvertUtil.ToInt(pars[5].Value);
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
    }
}