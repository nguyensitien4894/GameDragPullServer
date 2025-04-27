using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;
using System.ComponentModel.DataAnnotations;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Agency
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public short? AccountLevel { get; set; }
        public long? ParentID { get; set; }
        public int? Status { get; set; }
        public string StatusFormat { get { return Status.IntToAgencyStatusFormat(); } }
        public string AreaName { get; set; }
        public string PhoneOTP { get; set; }
        public string PhoneDisplay { get; set; }
        public string FBLink { get; set; }
        public string TelegramLink{ get; set; }
        public string ZaloLink { get; set; }
        public string ParrentAccountName { get; set; }
        public string ParrentDisplayName { get; set; }
        public int OrderNum { get; set; }
        public long Wallet { get; set; }
        public long? GiftcodeWallet { get; set; }
        public long? TelegramID { get; set; }
        public int ServiceID { get; set; }
        public int No { get; set; }

        public string WalletFormat
        {
            get { return Wallet.LongToMoneyFormat(); }
        }
        public string GiftcodeWallettFormat
        {
            get { return GiftcodeWallet.LongToMoneyFormat(); }
        }
    }

    public class AgencyManager
    {
        public string NickName { get; set; }
        public int Status { get; set; }
    }

    public class AgencyRaceTop
    {
        public int ServiceID { get; set; }
        public long AccountID { get; set; }
        public string DisplayName { get; set; }
        public long TotalTransfer { get; set; }
        public decimal TotalVP { get; set; }
        public decimal TotalBom { get; set; }
        public long PrizeID { get; set; }
        public long PrizeValue { get; set; }
        [DisplayFormat(NullDisplayText = "")]
        [UIHint("DateNullable")]
        public DateTime? RaceDate { get; set; }
        public int BonusRate { get; set; }
        public bool IsClosed { get; set; }



    }
    public class AgencyRaceTopAward
    {
        public long AccountID { get; set; }
        public string DisplayName { get; set; }
        public long TotalTransfer { get; set; }
        public decimal TotalVP { get; set; }
        public decimal TotalBom { get; set; }
        public long PrizeID { get; set; }
        public long PrizeValue { get; set; }
        public DateTime? RaceDate { get; set; }
        public int BonusRate { get; set; }
        public bool IsClosed { get; set; }

    }

    public class AgencyRevenue
    {
        public int TransDateInt { get; set; }
        public long UserID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public long TotalOrgAmount { get; set; }
        public long TotalFeeAmount { get; set; }
        public long TotalAmount { get; set; }
        public string TotalAmountFormat { get { return TotalAmount.LongToMoneyFormat(); } }
        public string TotalFeeAmountFormat { get { return TotalFeeAmount.LongToMoneyFormat(); } }
        public string TotalOrgAmountFormat { get { return TotalOrgAmount.LongToMoneyFormat(); } }
    }

    public class AgencyTransaction
    {
        public long AccountID { get; set; }
        public long OrgBalance { get; set; }
        public string OrgBalanceFormat { get { return OrgBalance.LongToMoneyFormat(); } }
        public long Amount { get; set; }
        public string AmountFormat { get { return Amount.LongToMoneyFormat(); } }
        public long Balance { get; set; }
        public string BalanceFormat { get { return Balance.LongToMoneyFormat(); } }
        public long TranId { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string TranStatus { get; set; }
        public string TranStatusFormat { get { return TranStatus.StrToTransStatusFormat(); } }
        public string CreateDisplayName { get; set; }
        public string ReceiverDisplayName { get; set; }
        public int WalletType { get; set; }
        public string WalletTypeName
        {
            get
            {
                if (WalletType == 1) return "Ví chính";
                else if (WalletType == 3) return "Ví gift code";
                return string.Empty;
            }
        }
    }
}