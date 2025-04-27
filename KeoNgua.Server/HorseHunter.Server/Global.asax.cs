using HorseHunter.Server.Handlers;
using System.Configuration;
using System.Web.Http;
using TraditionGame.Utilities.Caching;

namespace HorseHunter.Server
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            StartTimerJackpot();
            //StartTimerAutoReset();
        }

        #region Timer Update Client Jackpot
        private static void StartTimerJackpot()
        {
            var aTimer = new System.Timers.Timer();
            aTimer.Interval = int.Parse(ConfigurationManager.AppSettings["TimerJackpot"]);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedJackpot;
            // Have the timer fire repeated events (true is the default)
            //aTimer.AutoReset = true;
            // Start the timer
            aTimer.Enabled = true;
        }

        private static void OnTimedJackpot(object source, System.Timers.ElapsedEventArgs e)
        {
            HorseHunterHandler.Instance.UpdateClientJackpot();
        }
        #endregion

        private static void StartTimerAutoReset()
        {
            var aTimer = new System.Timers.Timer();
            aTimer.Interval = 6 * 60 * 60 * 1000;
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedAutoReset;
            // Start the timer
            aTimer.Enabled = true;
        }

        private static void OnTimedAutoReset(object source, System.Timers.ElapsedEventArgs e)
        {
            GameFilter.Instance.Reset();
        }
    }
}
