using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class CasoutPay
    {
        public string CreateUpdate { set; get; }
        public long Money { set; get; }
    }

    public class CasoutPay1
    {
        public long InCardTotal { set; get; }
        public long InMomoTotal { set; get; }

        public long InBankTotal { set; get; }

        public long InBank1STotal { set; get; }

        public long OutMomoTotal { set; get; }

        public long OutBankTotal { set; get; }

        public long SellUserTotal { set; get; }

        public long BuyUserTotal { set; get; }

    }
}