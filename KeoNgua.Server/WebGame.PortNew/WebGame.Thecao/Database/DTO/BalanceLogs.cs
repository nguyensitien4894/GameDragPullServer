using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class BalanceLogs
    {
        //public long UserId { get; set; }

        //public long? BalanceType { get; set; }

        public long? OrgBalance { get; set; }

        public long? Amount { get; set; }

        public int? Fee { get; set; }

        public long? AmountFee { get; set; }

        public long? RemainBalance { get; set; }
        public string PartnerName { get; set; }

        //public long? RemainBalanceHang { get; set; }

        //public long? RemainBalanceSafe { get; set; }

        //public long? TransID { get; set; }

        //public long SessionId { get; set; }

        //public string Otp { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        //public long? CreatedBy { get; set; }
    }
}