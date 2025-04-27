using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class UserBankRequest
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

        public double? ExchangeRate { get; set; }

        public double? Fee { get; set; }

        public int? PartnerID { get; set; }

        public double? OrgUSDTmount { get; set; }

        public double? USDTAmount { get; set; }

        public double? RemainUSDTAmount { get; set; }

        public string USDTWalletAddress { get; set; }

        public string Email { get; set; }

        public string BankName { get; set; }

        public string BankAccount { get; set; }

        public string BankNumber { get; set; }

        public int? ServiceID { get; set; }

        public DateTime RequestDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Số tiền VND thực nhận
        /// </summary>
        public double? RealAmount { get; set; }
        /// <summary>
        /// Số tiền USD giao dịch
        /// </summary>
        public double? RealUSDTAmount { get; set; }
        public long? RealReceivedMoney { get; set; }
    }
}