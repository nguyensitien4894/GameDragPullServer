using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class CardRechargeProgress
    {
        public string DateRecharge { get; set; }
        public int  TotalQuantity { get; set; }
        public long TotalRechargeValue { get; set; }
    }
}