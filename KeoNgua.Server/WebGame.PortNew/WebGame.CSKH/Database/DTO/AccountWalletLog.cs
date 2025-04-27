using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class AccountWalletLog
    {
        public long TranId { get; set; }
        public string TransCode { get; set; }
        public string Code { get; set; }
        public long AccountID { get; set; }
        public string  ReceiverName { get; set; }
        public long? Wallet { get; set; }
        public long? Amount { get; set; }
        public int? Status { get; set; }
        public DateTime? TransDate { get; set; }
    }
}