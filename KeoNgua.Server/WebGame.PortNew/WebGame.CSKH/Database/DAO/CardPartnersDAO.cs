using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class CardPartnersDAO
    {
        private static readonly Lazy<CardPartnersDAO> _instance = new Lazy<CardPartnersDAO>(() => new CardPartnersDAO());

        public static CardPartnersDAO Instance
        {
            get { return _instance.Value; }
        }
        public void CardPartnerUpdate(long ID, string VTT, string VMS, string VNP,string ZING, string VCOIN, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[7];

             
                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;
                param[1] = new SqlParameter("@_VTT", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = VTT;
                param[2] = new SqlParameter("@_VMS", SqlDbType.NVarChar);
                param[2].Size = 200;
                param[2].Value = VMS;
                param[3] = new SqlParameter("@_VNP", SqlDbType.NVarChar);
                param[3].Size = 200;
                param[3].Value = VNP;
              
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_ZING", SqlDbType.NVarChar);
                param[5].Size = 200;
                param[5].Value = ZING;
                param[6] = new SqlParameter("@_VCOIN", SqlDbType.NVarChar);
                param[6].Size = 200;
                param[6].Value = VCOIN;

                db.ExecuteNonQuerySP("SP_CardPartner_Update", param.ToArray());
                Response = Convert.ToInt32(param[4].Value);
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
        public CardPartners CardPartnerGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _CardPartners = db.GetInstanceSP<CardPartners>("SP_CardPartner_GetByID", param.ToArray());
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