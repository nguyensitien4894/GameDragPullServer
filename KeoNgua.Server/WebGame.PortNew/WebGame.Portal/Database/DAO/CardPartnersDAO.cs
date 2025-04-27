using System;
using System.Collections.Generic;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using System.Data.SqlClient;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class CardPartnersDAO
    {
        private static readonly Lazy<CardPartnersDAO> _instance = new Lazy<CardPartnersDAO>(() => new CardPartnersDAO());

        public static CardPartnersDAO Instance
        {
            get { return _instance.Value; }
        }


        public List<CardPartners> GetList(int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_ServiceID", serviceid);
                var _lstCard = db.GetListSP<CardPartners>("SP_CardPartner_List",param.ToArray());

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