using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;
using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class CardRefDAO
    {
        private static readonly Lazy<CardRefDAO> _instance = new Lazy<CardRefDAO>(() => new CardRefDAO());

        public static CardRefDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<UserCardRecharge> UserCardRechargeByRefKey(string RefKey, int PartnerID)
        {

            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@_RefID", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = RefKey;
                param[1] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[1].Value = PartnerID;

                var _lstUserCardRecharge = db.GetListSP<UserCardRecharge>("SP_UserCardRecharge_GetByRefKey", param.ToArray());

                return _lstUserCardRecharge;
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

        public void UpdateCardChardRefStatus(long ReqestID, long UserID, string FeedbackErrorCode, string FeedbackMessage, int Status, int PartnerID, int ValidAmount,long? RefundCardValude,long? RefundReceivedMoney, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[11];
                param[0] = new SqlParameter("@_ReqestID", SqlDbType.BigInt);
                param[0].Value = ReqestID;
                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;
                param[2] = new SqlParameter("@_Status", SqlDbType.Int);
                param[2].Value = Status;
                param[3] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = FeedbackErrorCode;
                param[4] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[4].Size = 1000;
                param[4].Value = FeedbackMessage;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[6].Value = UserID;
                param[7] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[7].Value = PartnerID;
                param[8] = new SqlParameter("@_ValidAmount", SqlDbType.Int);
                param[8].Value = ValidAmount;
                param[9] = new SqlParameter("@_RefundCardValude", SqlDbType.BigInt);
                param[9].Value = RefundCardValude??(object)DBNull.Value;
                param[10] = new SqlParameter("@_RefundReceivedMoney", SqlDbType.BigInt);
                param[10].Value = RefundReceivedMoney??(object)DBNull.Value;
                db.ExecuteNonQuerySP("SP_UserCardRecharge_UpdateRefKey", param.ToArray());
                Response = Convert.ToInt32(param[5].Value);
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
        public void UserCardRechargeRefUpdate(long ReqestID, long UserID, int Status, string PartnerErrorCode, string PartnerMessage, string FeedbackErrorCode, string FeedbackMessage, string Description, string Signature, long CreateUser, int PartnerID, string refKey, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[13];
                param[0] = new SqlParameter("@_ReqestID", SqlDbType.BigInt);
                param[0].Value = ReqestID;
                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;
                param[2] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[2].Size = 20;
                param[2].Value = PartnerErrorCode;
                param[3] = new SqlParameter("@_FeedbackErrorCode", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = FeedbackErrorCode;
                param[4] = new SqlParameter("@_PartnerMessage", SqlDbType.NVarChar);
                param[4].Size = 1000;
                param[4].Value = PartnerMessage;
                param[5] = new SqlParameter("@_FeedbackMessage", SqlDbType.NVarChar);
                param[5].Size = 1000;
                param[5].Value = FeedbackMessage;
                param[6] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[6].Size = 1000;
                param[6].Value = Description;
                param[7] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[7].Value = CreateUser;
                param[8] = new SqlParameter("@_Signature", SqlDbType.NVarChar);
                param[8].Size = 1000;
                param[8].Value = Signature;
                param[9] = new SqlParameter("@_Response", SqlDbType.Int);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@_Status", SqlDbType.Int);
                param[10].Value = Status;
                param[11] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[11].Value = PartnerID;
                param[12] = new SqlParameter("@_RefKey", SqlDbType.NVarChar);
                param[12].Size = 100;
                param[12].Value = refKey;

                db.ExecuteNonQuerySP("SP_UserCardRecharge_UpdateRefKey", param.ToArray());
                Response = Convert.ToInt32(param[9].Value);
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