using System;
using System.ComponentModel;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.AccountWalletLogs
{
    public class AccountWalletLogsModel
    {
        [DisplayName("Mã giao dịch")]
        public long TranId { get; set; }
        [DisplayName("Mã giao dịch")]
        public string TransCode { get; set; }
        [DisplayName("Loại giao dịch")]
        public string Code { get; set; }
  
        public long AccountID { get; set; }
        [DisplayName("Tài khoản nhận bit")]
        public string ReceiverName { get; set; }
      
        public long? Wallet { get; set; }
       
        public long? Amount { get; set; }
        [DisplayName("Ngày thực hiện giao dịch")]
        public DateTime? TransDate { get; set; }
        [DisplayName("Số bit giao dịch")]
        public string AmountFormat { get
            {
                return Amount.LongToMoneyFormat();
            }
        }
        [DisplayName("Số bit còn lại")]
        public string WalletFormat{
            get{
                return Wallet.LongToMoneyFormat();
            }
        }
    }
}