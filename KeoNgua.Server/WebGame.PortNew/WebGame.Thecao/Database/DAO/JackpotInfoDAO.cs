using System;
using System.Collections.Generic;

using TraditionGame.Utilities;
using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class JackpotInfoDAO
    {
        private static readonly Lazy<JackpotInfoDAO> _instance = new Lazy<JackpotInfoDAO>(() => new JackpotInfoDAO());

        public static JackpotInfoDAO Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public List<JackpotInfo> GetGameJackPot()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
              
                var list=db.GetListSP<JackpotInfo>("SP_GameSlot_GetListJackpot");
                return list;
               
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

        public List<UserJackPotInfo> GetUserJackportInfo()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                return db.GetListSP<UserJackPotInfo>("SP_Gate_GetAwardsJackpot");
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

        public List<EventXJackpot> GetEventXJackpot()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var rs = db.GetListSP<EventXJackpot>("SP_Portal_Slots_Event");
                return rs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
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