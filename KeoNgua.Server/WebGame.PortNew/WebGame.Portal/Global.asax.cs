using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MsWebGame.Portal.App_Start;
using System;
using TraditionGame.Utilities;
using System.Net.Http;
using System.Configuration;

namespace MsWebGame.Portal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly string _TX1BASE = ConfigurationManager.AppSettings["TAIXIU_BASE"].ToString();
        private static readonly string _TX2 = ConfigurationManager.AppSettings["TX_CLIENT"].ToString();
        protected static int ServiceID = ConvertUtil.ToInt(ConfigurationManager.AppSettings["SERVICE_ID"].ToString());
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DeviceDetectionConfig.RegisterDevices();
            //StartTimerNeverStop();
        }


        #region neverstop
        private static void StartTimerNeverStop()
        {
            try
            {
                var aTimer = new System.Timers.Timer();
                aTimer.Interval = 1 * 50 * 1000;//chạy 50 s  1 lần
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedNeverStop;
                // Have the timer fire repeated events (true is the default)
                //aTimer.AutoReset = true;
                // Start the timer
                aTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
        private static void OnTimedNeverStop(object source, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                if (ServiceID == 1)
                {


                    using (var client = new HttpClient())
                    {
                        var url = _TX1BASE;
                        var response = client.GetAsync(url).Result;
                        NLogManager.LogMessage("TXBASE_RESULT" + response.IsSuccessStatusCode);
                    }
                }


                using (var client = new HttpClient())
                {
                    var url = _TX2;
                    var response = client.GetAsync(url).Result;
                    NLogManager.LogMessage("TXSV1_RESULT" + response.IsSuccessStatusCode);
                }
              

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

        }
        #endregion

    }
}
