using MsWebGame.Portal.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class SmsChargeDAO
    {

        private static readonly Lazy<SmsChargeDAO> _instance = new Lazy<SmsChargeDAO>(() => new SmsChargeDAO());

        public static SmsChargeDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<SmsCard> SmsValuesList(int DisplayValue, int Value, int PartnerID, int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];

                param[0] = new SqlParameter("@_DisplayValue", SqlDbType.Int);
                param[0].Value = DisplayValue ;
                param[1] = new SqlParameter("@_Value", SqlDbType.Int);
                param[1].Value = Value;
                param[2] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[2].Value = PartnerID;
                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;

                var _lstSmsValues = db.GetListSP<SmsCard>("SP_SmsCard_List", param.ToArray());
                return _lstSmsValues;
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

        public List<SMSOperators> SmsOperatorList(int ServiceID, string OperatorCode, string Telecom, string OperatorName)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = OperatorCode??(object)DBNull.Value;
                param[1] = new SqlParameter("@_OperatorName", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = OperatorName ?? (object)DBNull.Value;
                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = ServiceID;
               
                param[3] = new SqlParameter("@_Telecom", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = Telecom ?? (object)DBNull.Value;
              

                var _lstSmsOperator = db.GetListSP<SMSOperators>("SP_SmsOperator_List", param.ToArray());
                return _lstSmsOperator;
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

    }
}