using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraditionGame.Utilities.Momos.API;
using TraditionGame.Utilities.Momos.Models;

namespace TraditionGame.Utilities.Momos.Api.Charges
{
   public  class MomoChargeApi : BaseApiMomoRequest
    {
        protected static string MOMO_GameCode = ConfigurationManager.AppSettings["MOMO_GameCode"].ToString();
        protected static string MOMO_NccCode = ConfigurationManager.AppSettings["MOMO_NccCode"].ToString();
        protected static string MOMO_SECRETKEY = ConfigurationManager.AppSettings["MOMO_SECRETKEY"].ToString();
        
    }
}
