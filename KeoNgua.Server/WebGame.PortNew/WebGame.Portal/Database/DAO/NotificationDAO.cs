using System;
using System.Collections.Generic;
using MsWebGame.Portal.Database.DTO;
using System.Data.SqlClient;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class NotificationDAO
    {
        private static readonly Lazy<NotificationDAO> _instance = new Lazy<NotificationDAO>(() => new NotificationDAO());

        public static NotificationDAO Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        /// <summary>
        /// get list notification
        /// </summary>
        /// <returns></returns>
        public List<Notification> GetList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
            
                
                var _lsttNotification = db.GetListSP<Notification>("SP_Portal_GetNotification");
                return _lsttNotification;
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

        public List<BigWiner> GetBigWiner(int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                List<BigWiner> lst = new List<BigWiner>();
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_ServiceID", ServiceID);
                lst = db.GetListSP<BigWiner>("SP_BigWinner_GetNotify", param);

                return lst;
            }
            catch (Exception e)
            {
                NLogManager.PublishException(e);

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