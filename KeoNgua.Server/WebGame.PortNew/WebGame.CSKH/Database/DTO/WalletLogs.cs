using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class WalletLogs
    {
        public long ID { get; set; }

        public long AccountID { get; set; }

        public int AccountType { get; set; }

        public int? WalletType { get; set; }

        public long? OrgBalance { get; set; }

        public long? Amount { get; set; }

        public string AmountFormat { get { return Amount.HasValue ? TraditionGame.Utilities.Utils.MoneyExtension.LongToMoneyFormatNew(Amount.Value) : "0"; } }

        public long? Balance { get; set; }

        public int? ReasonID { get; set; }

        public int? Status { get; set; }

        public long TranId { get; set; }

        public string Description { get; set; }
        public double Fee { get; set; }

        public DateTime? TransDate { get; set; }

        public long? CreateBy { get; set; }
        public string TransCode { get; set; }
        public string PartnerName { get; set; }
    }
}