using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class SmsOtp
    {
        public long RequestId { get; set; }

        public string RequestTime { get; set; }

        public string Type { get; set; }

        public string Msisdn { get; set; }

        public int? Active { get; set; }
  public string Otp { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public long? UserID { get; set; }

        public int? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public long? Amount { get; set; }
        public string DisplayName { get; set; }


    }
}