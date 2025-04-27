using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Warnings
{
    public class WarningModel
    {
        public long UserID { get; set; }
        public string DisplayName { get; set; }
        public int ServiceID { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalAmountFormat {
            get {

                return TotalAmount.DecimalToMoneyFormat();

            }

             }
        public string ServiceName
        {
            get
            {
                if (ServiceID == 1) return "Cổng B1";
                if (ServiceID == 2) return "Cổng B2";
                if (ServiceID == 3) return "Cổng B3";
                return string.Empty;

            }

        }
    }
}