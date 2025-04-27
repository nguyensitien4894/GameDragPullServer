using System.ComponentModel.DataAnnotations;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Agencies
{
    public class AgencyModel
    {
        public long AccountId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "AccountRequired")]
        public string AccountName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "DisplayNameRequired")]
        public string DisplayName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "FullNameRequired")]
        public string FullName { get; set; }

        public short? AccountLevel { get; set; }
       
        public long? ParentID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "StatusRequired")]
        public int? Status { get; set; }
        public string StatusFormat
        {
            get { return Status.IntToAgencyStatusFormat(); }
        }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "AreaRequired")]
        public string AreaName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "PhoneRequired")]
        public string PhoneOTP { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "PhoneDisplayRequired")]
        public string PhoneDisplay { get; set; }

        public string FBLink { get; set; }
        public string TelegramLink { get; set; }
        public string ZaloLink { get; set; }
        public string ParrentAccountName { get; set; }

        public string ParrentDisplayName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "PasswordRequired")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OrderNumRequired")]
        public int OrderNum { get; set; }
        public long Wallet { get; set; }
        public long GiftcodeWallet { get; set; }

        public string WalletFormat
        {
            get { return Wallet.LongToMoneyFormat(); }
        }
        public string GiftcodeWallettFormat
        {
            get { return GiftcodeWallet.LongToMoneyFormat(); }
        }

        public int ServiceID { get; set; }
    }
}