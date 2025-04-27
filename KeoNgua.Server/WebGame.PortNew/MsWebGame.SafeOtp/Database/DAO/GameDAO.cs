using MsWebGame.SafeOtp.Database.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.SafeOtp.Database.DAO
{
    public class GameDAO
    {
        private static readonly Lazy<GameDAO> _instance = new Lazy<GameDAO>(() => new GameDAO());

        public static GameDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<Services> GetSerivces()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var _lstGetList = db.GetListSP<Services>("SP_Services_List");
                return _lstGetList;

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