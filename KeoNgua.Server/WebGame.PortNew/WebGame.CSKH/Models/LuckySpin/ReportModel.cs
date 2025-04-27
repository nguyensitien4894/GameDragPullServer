using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.LuckySpin
{
    public class ReportModel
    {
        public long SpinID { get; set; }
        [DisplayName("Nickname")]
        public string Username { get; set; }
        [DisplayName("Từ ngày")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? StartDate { get; set; }
        [DisplayName("Đến ngày")]
        public DateTime? EndDate { get; set; }
        [DisplayName("Giải thưởng")]
        public int PrizeID { get; set; }
        public string PrizeName { get; set; }
        [DisplayName("Giải FreeSpin")]
        public int FreeSpinID { get; set; }
        public string FreeSpinName { get; set; }
        [DisplayName("Hạng")]
        public int Rank { get; set; }
        [DisplayName("Tên hạng")]
        public string RankName { get; set; }
        [DisplayName("Tên giải")]
        public string Name { get; set; }
        public long PrizeValue { get; set; }
        [DisplayName("Thời gian")]
        public DateTime DateSpin { get; set; }
        [DisplayName("Đã trao")]
        public int ReciveAward { get; set; }
        [DisplayName("Định mức")]
        public int AwardLimit { get; set; }
        [DisplayName("Còn lại")]
        public int AwardRemain { get; set; }
        [DisplayName("Giá trị")]
        public string PrizeValueFormat
        {
            get { return PrizeValue.LongToMoneyFormat(); }
        }
        public int ServiceID { get; set; }
    }
}