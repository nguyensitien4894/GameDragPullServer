using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Models.Momo
{
    public class MomoResult
    {
        public MomoResult(long Amount,double Rate )
        {
            this.Amount = Amount;
            this.Rate = Rate;
        }
        public long Amount { get; set; }
        private double Rate { get; set; }
        public long AmountReceive { get {
                return ConvertUtil.ToLong(Amount * Rate);
            } }
    }
}