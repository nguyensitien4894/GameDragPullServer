using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class CashFlowOfEachAgency
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int Status { get; set; }
        public string StatusFormat { get { return Status == 1 ? "Hoạt động" : Status == 2 ? "Bị khóa" : Status == 0 ? "Bị xóa" : string.Empty; } }
        public int OrderNum { get; set; }
        public long Wallet { get; set; }
        public string WalletFormat { get { return Wallet.LongToMoneyFormat(); } }
        public long WalletGiftcode { get; set; }
        public string WalletGiftcodeFormat { get { return WalletGiftcode.LongToMoneyFormat(); } }
        public long Total_Sell { get; set; }
        public string TotalSellFormat { get { return Total_Sell.LongToMoneyFormat(); } }
        public long Total_Buy { get; set; }
        public string TotalBuyFormat { get { return Total_Buy.LongToMoneyFormat(); } }
        public int? LoginFailNumber { get; set; }
        public string DeltaBuySelFormat { get { return (Total_Buy - Total_Sell).LongToMoneyFormat(); } }
        public string TotalCashFlowFormat { get { return (Total_Buy + Total_Sell).LongToMoneyFormat(); } }
    }
}