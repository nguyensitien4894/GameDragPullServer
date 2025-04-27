using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.SafeOtp.Database.DTO
{
    public class OTPSafeMessage
    {
        public long ID { get; set; }

        public long ParentID { get; set; }
        public long SafeID { get; set; }
        public string Title { get; set; }

        public string Message { get; set; }

        public int? Status { get; set; }

        public int? MessageType { get; set; }

        public int? ServiceID { get; set; }

        public DateTime? SentDate { get; set; }

        public long? CreateBy { get; set; }
        public string ServiceName { get; set; }
    }
}