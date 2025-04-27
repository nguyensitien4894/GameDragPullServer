using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class UserViettelPayRequest
    {
        public long RequestID { get; set; }

        public int RequestType { get; set; }

        public string RequestCode { get; set; }

        public long UserID { get; set; }

        public long Amount { get; set; }

        public double? Rate { get; set; }

        public long? ReceivedMoney { get; set; }

        public long? RefundReceivedMoney { get; set; }

        public int Status { get; set; }

        public string PartnerStatus { get; set; }

        public string PartnerErrorCode { get; set; }

        public string PartnerMessage { get; set; }

        public string FeedbackErrorCode { get; set; }

        public string FeedbackMessage { get; set; }

        public string Description { get; set; }

        public string RefKey { get; set; }

        public string RefSendKey { get; set; }

        public double? Fee { get; set; }

        public int? PartnerID { get; set; }

        public string GameCode { get; set; }

        public string GameAccount { get; set; }

        public string NccCode { get; set; }

        public string Provider { get; set; }

        public string BankAccount { get; set; }

        public string BankNumber { get; set; }

        public int? ServiceID { get; set; }

        public DateTime RequestDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }
        public string ServieName { get; set; }
        public string DisplayName { get; set; }
    }
}