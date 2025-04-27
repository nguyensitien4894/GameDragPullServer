using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Thecao.Database.DAO
{
    public class SmsChargeDAO
    {
        private static readonly Lazy<SmsChargeDAO> _instance = new Lazy<SmsChargeDAO>(() => new SmsChargeDAO());

        public static SmsChargeDAO Instance
        {
            get { return _instance.Value; }
        }
        public void UserSmsRequestPartnerCheck(string Phone,int Status, int PartnerID, long UserID,
            long Amount, string PartnerErrorCode, 
            string PartnerMessage, string Signature, string RefKey, string Description
            ,out long RequestID, out long ReceivedMoney, 
            out long RemainBalance , out double RequestRate, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[15];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_Phone", SqlDbType.VarChar);
                param[1].Size = 20;
                param[1].Value = Phone;
                param[2] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[2].Value = Amount;
                param[3] = new SqlParameter("@_Status", SqlDbType.Int);
                param[3].Value = Status;
                param[4] = new SqlParameter("@_PartnerErrorCode", SqlDbType.VarChar);
                param[4].Size = 20;
                param[4].Value = PartnerErrorCode;
                param[5] = new SqlParameter("@_PartnerMessage", SqlDbType.VarChar);
                param[5].Size = 500;
                param[5].Value = PartnerMessage;
                param[6] = new SqlParameter("@_Signature", SqlDbType.VarChar);
                param[6].Size = 500;
                param[6].Value = Signature;
                param[7] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[7].Size = 400;
                param[7].Value = Description;
                param[8] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[8].Size = 250;
                param[8].Value = RefKey;


                param[9] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[9].Value = PartnerID;
              
            
             

             
                param[10] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[10].Direction = ParameterDirection.Output;
                param[11] = new SqlParameter("@_ReceivedMoney", SqlDbType.BigInt);
                param[11].Direction = ParameterDirection.Output;
                param[12] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[12].Direction = ParameterDirection.Output;
                param[13] = new SqlParameter("@_Response", SqlDbType.Int);
                param[13].Direction = ParameterDirection.Output;
                param[14] = new SqlParameter("@_RequestRate", SqlDbType.Float);
                param[14].Direction = ParameterDirection.Output;
                
                db.ExecuteNonQuerySP("SP_UserSmsRequest_PartnerCheck", param.ToArray());
                RequestID = ConvertUtil.ToLong(param[10].Value);
                ReceivedMoney = ConvertUtil.ToLong(param[11].Value);
                RemainBalance = ConvertUtil.ToLong(param[12].Value);
                Response = ConvertUtil.ToInt(param[13].Value);
                RequestRate = ConvertUtil.ToFloat(param[14].Value);
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
            RequestID = 0;
            ReceivedMoney = 0;
            RemainBalance =0;
          
            RequestRate =0;
        }
    }
}