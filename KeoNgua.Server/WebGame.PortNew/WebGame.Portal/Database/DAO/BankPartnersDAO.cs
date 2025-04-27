using System;
using System.Collections.Generic;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using System.Data.SqlClient;
using TraditionGame.Utilities;


namespace MsWebGame.Portal.Database.DAO
{
    public class BankPartnersDAO
    {
        private static readonly Lazy<BankPartnersDAO> _instance = new Lazy<BankPartnersDAO>(() => new BankPartnersDAO());

        public static BankPartnersDAO Instance
        {
            get { return _instance.Value; }
        }


        public List<USDTPartners> USDTPartnerList(int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_ServiceID", serviceid);
                var _lstCard = db.GetListSP<USDTPartners>("SP_USDTPartner_List", param.ToArray());

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