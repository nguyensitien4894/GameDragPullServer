using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class TreasureDAO
    {
        private static readonly Lazy<TreasureDAO> _instance = new Lazy<TreasureDAO>(() => new TreasureDAO());

        public static TreasureDAO Instance
        {
            get { return _instance.Value; }
        }

        public void CarrotCollectRechargeCard(long UserID, long CardValue,int ServiceID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingEventDBConn);
                SqlParameter[] param =  new SqlParameter[4];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_CardValue", SqlDbType.BigInt);
                param[2].Value = CardValue;
                param[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_CarrotCollect_RechargeCard", param.ToArray());
                Response = ConvertUtil.ToInt(param[3].Value);
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


        public void CarrotCollectRechargeBank(long UserID, long BankValue, int ServiceID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingEventDBConn);
                SqlParameter[] param = new SqlParameter[4];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_BankValue", SqlDbType.BigInt);
                param[2].Value = BankValue;
                param[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_CarrotCollect_RechargeBank", param.ToArray());
                Response = ConvertUtil.ToInt(param[3].Value);
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