using System;
using System.ComponentModel;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class GameCampaign
    {
        public long CampaignID { get; set; }
        public string CampaignName { get; set; }
        public string CampaignCode { get; set; }
        public DateTime EffectDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
        public string Description { get; set; }
        public int ServiceID { get; set; }

        public string ServiceName { get; set; }

    }

    public class GameEvent
    {
        public long ID { get; set; }
        public long CampaignID { get; set; }
        public string CampaignName { get; set; }
        public int GameID { get; set; }
        public string GameIDFormat { get { return GameID.IntToGameFormat(); } }
        public int RoomID { get; set; }
        public string RoomIDFormat { get { return RoomID.IntToRoomFormat(); } }
        public long CurrentJackpotValue { get; set; }
        public string CurrentJackpotValueFormat { get { return CurrentJackpotValue.LongToMoneyFormat(); } }
        public long JackpotEventInit { get; set; }
        public string JackpotEventInitStr { get; set; }
        public string JackpotEventInitFormat { get { return JackpotEventInit.LongToMoneyFormat(); } }
        public int JackpotEventQuota { get; set; }
        public int JackpotStepJump { get; set; }
        public string EventDay { get; set; }
        public string EventDayFormat { get { return EventDay.WeekDayFormat(); } }
        public string EventTime { get; set; }
        public DateTime EffectDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
        public string Description { get; set; }
        public long CreateUserID { get; set; }
    }

    public class GameGiftCode
    {
        public long ID { get; set; }
        public long CampaignID { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string GiftCode { get; set; }
        public int GiftCodeType { get; set; }
        public string GiftCodeTypeFormat { get { return GiftCodeType.IntToGiftCodeTypeFomat(); } }
        public long MoneyReward { get; set; }
        public string MoneyRewardFormat { get { return MoneyReward.LongToMoneyFormat(); } }
        public long TotalUsed { get; set; }
        public string TotalUsedFormat { get { return TotalUsed.LongToMoneyFormat(); } }
        public long LimitQuota { get; set; }
        public string LimitQuotaFormat { get { return LimitQuota.LongToMoneyFormat(); } }
        public long UserID { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int CreateUserType { get; set; }
        public string CreateUserTypeFormat { get { return CreateUserType == 1 ? "Admin" : "Đại lý"; } }
        public string CreateUserName { get; set; }
        public int IsExpired { get; set; }
        public bool Status { get; set; }
        public string StatusFormat { get { return (IsExpired > 0 && Status) ? "Hết hạn" : Status.BoolToConfigGameFormat(); } }
        public DateTime ExpiredDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }

    public class GameParamConfig
    {
        public int ID { get; set; }
        [DisplayName("Loại cấu hình")]
        public string ParamType { get; set; }
        [DisplayName("Mã")]
        public string Code { get; set; }
        [DisplayName("Giá trị")]
        public string Value { get; set; }
        [DisplayName("Mô tả ")]
        public string Description { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
    }

    public class GamePlayTrackingHour
    {
        public DateTime DatePlay { get; set; }
        public DateTime PeriodHour { get; set; }
        public long Quantity { get; set; }
    }
}