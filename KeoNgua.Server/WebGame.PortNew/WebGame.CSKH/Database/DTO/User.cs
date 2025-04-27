using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class User
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string Avatar { get; set; }
        public string PhoneOTP { get; set; }
        public string PhoneContact { get; set; }
        public string BankAccount { get; set; }
        public string Email { get; set; }
        public double Rank { get; set; }
        public int TotalReviewCnt { get; set; }
        public long StarBought { get; set; }
        public long StarSold { get; set; }
        public DateTime? TranferDateNow { get; set; }
        public int? ReceiveNotification { get; set; }
        public int? Status { get; set; }
        public long? FeePercent { get; set; }
        public int? LockAdvert { get; set; }
        public int? AdvertSellCnt { get; set; }
        public int? TranSellCnt { get; set; }
        public int? TranBuyCnt { get; set; }
        public int? LockBuy { get; set; }
        public int? LockSell { get; set; }
        public string RoleList { get; set; }
        public int? IsStarAdmin { get; set; }
        public int? LockWithDraw { get; set; }
        public int? LockTopup { get; set; }
        public string Password { get; set; }
        public long? Wallet { get; set; }
        public long? WalletHang { get; set; }
        public int UserType { get; set; }
        public int AgencyLevel { get; set; }
        public long AgencyID { get; set; }
        public long WalletStar { get; set; }
        public int ChangePassword { get; set; }
    }

    public class UserManager
    {
        public string NickName { get; set; }
        public int Status { get; set; }
    }

    public class UserPrivilege
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhoneFormat { get { return Phone.PhoneNumberFormat(); } }
        public long Wallet { get; set; }
        public string WalletFormat { get { return Wallet.LongToMoneyFormat(); } }
        public int RankID { get; set; }
        public string PrivilegeName { get; set; }
        public long Point { get; set; }
        public string PointFormat { get { return Point.LongToMoneyFormat(); } }
        public long VP { get; set; }
        public string VPFormat { get { return VP.LongToMoneyFormat(); } }
        public DateTime RankedMonth { get; set; }
        public DateTime EffectDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    public class UserProfile
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string PhoneOTP { get; set; }
        public string PhoneContactFormat { get { return PhoneOTP.PhoneNumberFormat(); } }
        public string Email { get; set; }
        public int RankID { get; set; }
        public string RankIDFormat { get { return RankID.IntToRankFormat(); } }
        public long StarBought { get; set; }
        public string StarBoughtFormat { get { return StarBought.LongToMoneyFormat(); } }
        public long StarSold { get; set; }
        public string StarSoldFormat { get { return StarSold.LongToMoneyFormat(); } }
        public int TranSellCnt { get; set; }
        public int TranBuyCnt { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class UserPrivilegeHistory
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public int RankID { get; set; }
        public string RankIDFormat { get { return RankID.IntToRankFormat(); } }
        public long VP { get; set; }
        public string VPFormat { get { return VP.LongToMoneyFormat(); } }
        public long Point { get; set; }
        public string PointFormat { get { return Point.LongToMoneyFormat(); } }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
        public DateTime RankedMonth { get; set; }
        public DateTime EffectDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    public class UserRedemption
    {
        public long id { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public long PriArtID { get; set; }
        public int PrivilegeID { get; set; }
        public string PrivilegeName { get; set; }
        public string ArtifactName { get; set; }
        public long Price { get; set; }
        public string PriceFormat { get { return Price.LongToMoneyFormat(); } }
        public int Quantity { get; set; }
        public string QuantityFormat { get { return Quantity.IntToMoneyFormat(); } }
        public long RefundAmount { get; set; }
        public string RefundAmountFormat { get { return RefundAmount.LongToMoneyFormat(); } }
        public long VP { get; set; }
        public string VPFormat { get { return VP.LongToMoneyFormat(); } }
        public long Point { get; set; }
        public DateTime RankedMonth { get; set; }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
        public string Description { get; set; }
        public DateTime RefundReceiveDate { get; set; }
        public DateTime GiftReceiveDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class UserRechargeProgress
    {
        public string RechargeDate { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public int TotalQuantity { get; set; }
        public long TotalValue { get; set; }
    }

    public class UserInfo
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public int AccountType { get; set; }
        public int AccountLevel { get; set; }
        public int AccountStatus { get; set; }
        public long TelegramID { get; set; }
        public long OTPSafeID { get; set; }
        public string PhoneSafeNo { get; set; }
        public string SignalID { get; set; }
       public DateTime CreatedTime { get; set; }
        public int IsFB { get; set; }
        public int IsUpdatedFB { get; set; }
    }

    public class UserCardSwap
    {
        public long UserCardSwapID { get; set; }
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string CardNumber { get; set; }
        public string CardSerial { get; set; }
        public long? CardValue { get; set; }
        public int? Status { get; set; }
        public DateTime? BuyDate { get; set; }
      


        public string StatusStr
        {
            get
            {
                if (Status == 1) return "Thành công";
                if (Status == 2) return "Chờ duyệt";
                if (Status == 3) return "Hủy đổi thẻ";
                return "Thất bại";
            }
        }
    }
}