using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Log
{
   public  class LoggerHelper
    {
        private static readonly Logger loggerUSDT = LogManager.GetLogger("USDT");
        private static readonly Logger loggerUSDTMoMo = LogManager.GetLogger("Momo");
        public static void LogUSDTMessage(string message)
        {
            loggerUSDT.Info(message);
            loggerUSDT.Info("----------------------------------------------------------------");
        }
        public static void LogMomoMessage(string message)
        {
            loggerUSDTMoMo.Info(message);
            loggerUSDTMoMo.Info("----------------------------------------------------------------");
        }

    }
}
