using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Models.GiftCodes
{
    public class GameGiftCodeModel
    {
        [DisplayName("ID")]
        public long ID { get; set; }

        [DisplayName("Chiến dịch")]
        public long CampaignID { get; set; }

        [DisplayName("Chiến dịch")]
        public string CampaignName { get; set; }

        [DisplayName("Giftcode")]
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "GiftCodeRequired")]
        public string GiftCode { get; set; }

        [DisplayName("Loại giftcode")]
        public int GiftCodeType { get; set; }
        [DisplayName("Loại giftcode")]
        public string GiftCodeTypeFormat
        {
            get { return GiftCodeType.IntToGiftCodeTypeFomat(); }
        }

        [DisplayName("Tổng tiền quỹ  cho chiến dịch")]
        [Range(1, long.MaxValue, ErrorMessage = "Tiền thưởng lớn hơn {1}")]
        public long MoneyReward { get; set; }
        [DisplayName("Tiền thưởng")]
        public string MoneyRewardFormat
        {
            get { return MoneyReward.LongToMoneyFormat(); }
        }

        [DisplayName("Số người tối đa nhận gift code")]
        public long TotalUsed { get; set; }
        [DisplayName("Tổng người chơi")]
        public string TotalUsedFormat
        {
            get { return TotalUsed.LongToMoneyFormat(); }
        }

        [DisplayName("Giới hạn")]
        [Range(1, long.MaxValue, ErrorMessage = "Giới hạn lớn hơn {1}")]
        public long LimitQuota { get; set; }
        [DisplayName("Giới hạn")]
        public string LimitQuotaFormat
        {
            get { return LimitQuota.LongToMoneyFormat(); }
        }

        [DisplayName("Tài khoản tạo")]
        public long UserID { get; set; }

        [DisplayName("Trạng thái")]
        public bool Status { get; set; }
        [DisplayName("Trạng thái")]
        public string StatusFormat
        {
            get { return Status.BoolToConfigGameFormat(); }
        }

        [DisplayName("Ngày hết hạn")]
        public DateTime ExpiredDate { get; set; }
    }
}