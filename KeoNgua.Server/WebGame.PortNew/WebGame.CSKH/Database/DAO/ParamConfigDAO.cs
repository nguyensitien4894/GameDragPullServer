using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class ParamConfigDAO
    {
        private static readonly Lazy<ParamConfigDAO> _instance = new Lazy<ParamConfigDAO>(() => new ParamConfigDAO());

        public static ParamConfigDAO Instance
        {
            get { return _instance.Value; }
        }


        public List<ParamConfig> GetList(string ParamType, string Code, string Value, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

                param[0] = new SqlParameter("@_ParamType", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = ParamType;
                param[1] = new SqlParameter("@_Code", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = Code;
                param[2] = new SqlParameter("@_Value", SqlDbType.NVarChar);
                param[2].Size = 20;
                param[2].Value = Value;
                param[3] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[3].Value = CurrentPage;
                param[4] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[4].Value = RecordPerpage;
                param[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                var _lstParamConfig = db.GetListSP<ParamConfig>("SP_ParamConfig_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[5].Value);
                return _lstParamConfig;

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
            return new List<ParamConfig>();
        }

        public int GetConfigValue(string paramType, string code)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_ParamType", paramType);
                pars[1] = new SqlParameter("@_Code", code);
                pars[2] = new SqlParameter("@_Value", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Param_Config_Get_Value", pars);
                return Convert.ToInt32(pars[2].Value);
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

        public string GetConfigStrValue(string paramType, string code)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_ParamType", paramType);
                pars[1] = new SqlParameter("@_Code", code);
                pars[2] = new SqlParameter("@_Value", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Param_Config_Get_Value", pars);
                return ConvertUtil.ToString(pars[2].Value);
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

        public void Update(int ID, string Value, string Description, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
             new SqlParameter[4];

                param[0] = new SqlParameter("@_ID", SqlDbType.Int);
                param[0].Value = ID;
                param[1] = new SqlParameter("@_Value", SqlDbType.VarChar);
                param[1].Size = 20;
                param[1].Value = Value;
                param[2] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[2].Size = 400;
                param[2].Value = Description;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_ParamConfig_Update", param.ToArray());
                Response = Convert.ToInt32(param[3].Value);
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

        public ParamConfig ParamConfigGetByID(long ParamId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ParamId", SqlDbType.BigInt);
                param[0].Value = ParamId;
                var _ParamConfig = db.GetInstanceSP<ParamConfig>("SP_ParamConfig_GetByID", param.ToArray());
                return _ParamConfig;
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

        public List<PrivilegeGameInfo> GetListPrivilegeGameInfo(int gameId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_GameID", gameId);
                var lstRs = db.GetListSP<PrivilegeGameInfo>("SP_Game_List", pars);
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

        public int UpdatePrivilegeGameInfo(int gameId, string gamename, double gameweight, double profitmargin, double coefficient, bool status)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_GameID", gameId);
                pars[1] = new SqlParameter("@_GameName", gamename);
                pars[2] = new SqlParameter("@_GameWeight", gameweight);
                pars[3] = new SqlParameter("@_ProfitMargin", profitmargin);
                pars[4] = new SqlParameter("@_Coefficient", coefficient);
                pars[5] = new SqlParameter("@_Status", status);
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_Game_Update", pars);
                return Int32.Parse(pars[6].Value.ToString());
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

        public List<PrivilegeLevel> GetListPrivilegeLevel(string privlcode)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_PrivilegeCode", privlcode);
                var lstRs = db.GetListSP<PrivilegeLevel>("SP_PrivilegeType_List", pars);
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

        public int UpdatePrivilegeLevel(int typeID, string typeName, long vp, bool status)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_PrivilegeTypeID", typeID);
                pars[1] = new SqlParameter("@_PrivilegeTypeName", typeName);
                pars[2] = new SqlParameter("@_VP", vp);
                pars[3] = new SqlParameter("@_Status", status);
                pars[4] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_PrivilegeType_Update", pars);
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

        public List<PrivilegePrize> GetListPrivilegePrize(int rankId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_RankID", rankId);
                var lstRs = db.GetListSP<PrivilegePrize>("SP_PrivilegePrize_List", pars);
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

        public int UpdatePrivilegePrize(int rankId, double refundRate, double pointExchangeRate, double giftRate, double moneyExchangeRate, bool status)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_RankID", rankId);
                pars[1] = new SqlParameter("@_RefundRate", refundRate);
                pars[2] = new SqlParameter("@_PointExchangeRate", pointExchangeRate);
                pars[3] = new SqlParameter("@_GiftRate", giftRate);
                pars[4] = new SqlParameter("@_MoneyExchangeRate", moneyExchangeRate);
                pars[5] = new SqlParameter("@_Status", status);
                pars[6] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_PrivilegePrize_Update", pars);
                return Int32.Parse(pars[6].Value.ToString());
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
        
            public GameCampaign CampaignGetByID(long campaignID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_CampaignID", campaignID);
               

                var lstRs = db.GetInstanceSP<GameCampaign>("SP_Campaign_GetByID", pars);
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
        public List<GameCampaign> GetListGameCampaign(long campaignID, string campaignName, string campaignCode, bool? status, DateTime? effectDate, DateTime? expiredDate,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_CampaignID", campaignID);
                pars[1] = new SqlParameter("@_CampaignName", campaignName);
                pars[2] = new SqlParameter("@_CampaignCode", campaignCode);
                pars[3] = new SqlParameter("@_Status", status);
                pars[4] = new SqlParameter("@_EffectDate", effectDate);
                pars[5] = new SqlParameter("@_ExpiredDate", expiredDate);
                pars[6] = new SqlParameter("@_ServiceID", ServiceID);
                
                var lstRs = db.GetListSP<GameCampaign>("SP_Campaign_Admin_List", pars);
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

        public int AddOrUpdateGameCampaign(long campaignID, string campaignName, string campaignCode, long totalReward, long totalUsed,
            DateTime effectDate, DateTime expiredDate, bool status, string des, long actionUserId,int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@_CampaignID", campaignID);
                pars[1] = new SqlParameter("@_CampaignName", campaignName);
                pars[2] = new SqlParameter("@_CampaignCode", campaignCode);
                pars[3] = new SqlParameter("@_TotalRewardValue", totalReward);
                pars[4] = new SqlParameter("@_TotalUsed", totalUsed);
                pars[5] = new SqlParameter("@_EffectDate", effectDate);
                pars[6] = new SqlParameter("@_ExpiredDate", expiredDate);
                pars[7] = new SqlParameter("@_Status", status);
                pars[8] = new SqlParameter("@_Description", des);
                pars[9] = new SqlParameter("@_ActionUserID", actionUserId);
                pars[10] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[11] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_Campaign_Handle", pars);
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

        public List<GameEvent> GetListGameEvent(long campaignId, int gameId, int roomId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_CampaignID", campaignId);
                pars[1] = new SqlParameter("@_GameID", gameId);
                pars[2] = new SqlParameter("@_RoomID", roomId);
                var lstRs = db.GetListSP<GameEvent>("SP_GameEvents_List", pars);
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

        public int AddOrUpdateGameEvent(long campaignId, int gameId, int roomId, long jpEvent, int jpQuota, int jpStepJump, string eventDay,
            string eventTime, DateTime effectDate, DateTime expiredDate, bool status, string des, long userId)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[14];
                pars[0] = new SqlParameter("@_CampaignID", campaignId);
                pars[1] = new SqlParameter("@_GameID", gameId);
                pars[2] = new SqlParameter("@_RoomID", roomId);
                pars[3] = new SqlParameter("@_JackpotEventValue", jpEvent);
                pars[4] = new SqlParameter("@_JackpotQuota", jpQuota);
                pars[5] = new SqlParameter("@_JackpotStepJump", jpStepJump);
                pars[6] = new SqlParameter("@_EventDay", eventDay);
                pars[7] = new SqlParameter("@_EventTime", eventTime);
                pars[8] = new SqlParameter("@_EffectDate", effectDate);
                pars[9] = new SqlParameter("@_ExpiredDate", expiredDate);
                pars[10] = new SqlParameter("@_Status", status);
                pars[11] = new SqlParameter("@_Description", des);
                pars[12] = new SqlParameter("@_UserID", userId);
                pars[13] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_GameEvents_Handle", pars);
                return Int32.Parse(pars[13].Value.ToString());
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