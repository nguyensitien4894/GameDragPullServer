using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models
{
    public class VIP2QuaterInfo
    {
        public QuaterInfo QuaterInfo { get; set; }
        public QuaterInfo BeforeQuaterInfo { get; set; }
        public Loan LoanInfor { get; set; }

    }
    public class Loan
    {
        public long LoanAmount { get; set; }
        public long OldDebt { get; set; }
    }
    public class QuaterInfo
    {
        public int VPQuaterAcc { get; set; }
        public long QuaterAcc { get; set; }
        public int QuaterPrizeStatus { get; set; }
        public int CurrentQuater { get; set; }

        public bool IsBefore { get; set; }
    }

    public class CurrentCard
    {
        public double Percent { get; set; }
        public long PointAcc { get; set; }
        public int CardVpPeriodBonus { get; set; }
        public int CardVpPeriod { get; set; }
    }
}