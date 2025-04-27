using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MsWebGame.CSKH.Database.DTO
{
    public class BotInfo
    {
        [DisplayName("Bot")]
        public long BotId { get; set; }
        [DisplayName("Tên bot")]
        public string BotName { get; set; }
        [DisplayName("Tên hiển thị")]
        public string DisplayName { get; set; }
        [DisplayName("Số dư")]
        public long Balance { get; set; }
        [DisplayName("Số lần cược")]
        public long BetNumber { get; set; }
        [DisplayName("Tổng cược")]
        public long TotalBet { get; set; }
        [DisplayName("Tổng thưởng")]
        public long TotalPrize { get; set; }
        [DisplayName("Độ lệch")]
        public long Delta { get; set; }
        [DisplayName("Trạng thái")]
        public bool Status { get; set; }
        [DisplayName("Nhóm bot")]
        public long BotGroupId { get; set; }
        [DisplayName("Loại")]
        public int Type { get; set; }
        public int TypeFund { get; set; }

        [DisplayName("Số bit")]
        public long Amount { get; set; }
        [DisplayName("Thời gian bắt đầu")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan StartTime { get; set; }
        [DisplayName("Thời gian kết thúc")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan EndTime { get; set; }
    }
}