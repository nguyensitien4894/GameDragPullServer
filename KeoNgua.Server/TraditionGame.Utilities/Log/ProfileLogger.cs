using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Log
{
   public  class ProfileLogger
    {
        private static readonly Logger logger = LogManager.GetLogger("Profile");


        public static void LogMessage(string message)
        {
            logger.Info(message);
        }

        public static void LogAction(byte gameID, long roomID, long sessionID,
            long accountID, byte action, long money, byte moneyType)
        {
            logger.Info(string.Format("A,{0},{1},{2},{3},{4},{5},{6}",
                gameID, roomID, sessionID, accountID, action, money, moneyType));
        }

    }
}
