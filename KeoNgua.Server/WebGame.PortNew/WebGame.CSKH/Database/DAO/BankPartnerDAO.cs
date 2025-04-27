using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class BankPartnersDAO
    {
        private static readonly Lazy<BankPartnersDAO> _instance = new Lazy<BankPartnersDAO>(() => new BankPartnersDAO());

        public static BankPartnersDAO Instance
        {
            get { return _instance.Value; }
        }
        public void BankPartnerUpdate(long ID, string Momo, string Bank, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];


                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;
                param[1] = new SqlParameter("@_Momo", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = Momo;
                param[2] = new SqlParameter("@_Bank", SqlDbType.NVarChar);
                param[2].Size = 200;
                param[2].Value = Bank??(object)DBNull.Value;
              

                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                
                db.ExecuteNonQuerySP("SP_BankPartner_Update", param.ToArray());
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
        public BankPartners CardPartnerGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _CardPartners = db.GetInstanceSP<BankPartners>("SP_CardPartner_GetByID", param.ToArray());
                return _CardPartners;
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

        public List<BankPartners> GetList(int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_ServiceID", serviceid);
                var _lstCard = db.GetListSP<BankPartners>("SP_BankPartner_List", param.ToArray());

                return _lstCard;

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