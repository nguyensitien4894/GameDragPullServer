using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TraditionGame.Utilities.Utils;
namespace MsWebGame.CSKH.Models.LuckyDice
{
 
    public class LuckyDiceModel
    {
        public int Model
        {
            get;
            set;
        }
    }
    public class SoiCau
    {
        public long SessionId { get; set; }
        public int FirstDice { get; set; }
        public int SecondDice { get; set; }
        public int ThirdDice { get; set; }
        public int DiceSum { get; set; }
        public int BetSide { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class EventModel
    {
        public int EventID { get; set; }
        [DisplayName("Bit đặt cược MIN")]
        public long BetValueMin { get; set; }
        [DisplayName("Số dây THẮNG")]
        public int QuantityCordWin { get; set; }
        [DisplayName("Số dây THUA")]
        public int QuantityCordLost { get; set; }
        [DisplayName("Bit thưởng MIN ")]
        public long PrizeValueMin { get; set; }
        [DisplayName("Bit thưởng MAX")]
        public long PrizeValueMax { get; set; }
        [DisplayName("Số giải dây THUA/1 khung giờ")]
        public int QuantityAwardCordWin { get; set; }
        [DisplayName("Số giải dây THẮNG/1 khung giờ")]
        public int QuantityAwardCordLost { get; set; }
        [DisplayName("Số giải dây THUA/1 khung giờ")]
        public int QuantityAwardCordWinInit { get; set; }
        [DisplayName("Số giải dây THẮNG/1 khung giờ")]
        public int QuantityAwardCordLostInit { get; set; }
        [DisplayName("Thời gian bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public TimeSpan StartEventTimes { get; set; }
        [DisplayName("Thời gian kết thúc")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public TimeSpan EndEventTimes { get; set; }
        [DisplayName("Ngày sự kiện")]
        public string EventDays { get; set; }
        public bool IsActive { get; set; }
        [DisplayName("Khung giờ")]
        public string Hours { get; set; }

        [DisplayName("Số dây thắng - thua")]
        public string QuantityCordStr
        {
            get
            {
                return QuantityCordWin.ToString() + " - " + QuantityCordLost.ToString();
            }
        }

        [DisplayName("Bit thưởng MIN - MAX")]
        public string PrizeValueMinMaxStr
        {
            get
            {
                return PrizeValueMin.LongToMoneyFormat() + " - " + PrizeValueMax.LongToMoneyFormat();
            }
        }


        [DisplayName("Thời gian bắt đầu - kết thúc")]
        public string EventTimesStr
        {
            get
            {
                return DateUtil.TimeSpanToString(StartEventTimes) + " - " + DateUtil.TimeSpanToString(EndEventTimes);
            }
        }

        [DisplayName("Số giải dây thắng-thua")]
        public string QuantityAwardCordInitStr
        {
            get
            {
                return QuantityAwardCordWinInit.ToString() + " - " + QuantityAwardCordLostInit.ToString();
            }
        }

        [DisplayName("Số giải còn lại dây thắng-thua")]
        public string QuantityAwardCordStr
        {
            get
            {
                return QuantityAwardCordWin.ToString() + " - " + QuantityAwardCordLost.ToString();
            }
        }
    }

    public class RaceTopModel
    {
        public int RaceTopID { get; set; }
        [DisplayName("Số lượng giải")]
        public int Quantity { get; set; }
        [DisplayName("Giá trị")]
        public long PrizeValue { get; set; }
        [DisplayName("Tên giải")]
        public string Description { get; set; }
    }

    public class ReportEventModel
    {
        public int? EventID { get; set; }
        public string EventName { get; set; }
        public int? RaceTopID { get; set; }
        public string RaceTopName { get; set; }
        [DisplayName("Tài khoản")]
        public string Username { get; set; }
        [DisplayName("Tổng DBit đặt")]
        public long BetValue { get; set; }
        [DisplayName("Tổng DBit nhận")]
        public long PrizeValue { get; set; }
        public bool IsRecall { get; set; }
        [DisplayName("Dây")]
        public int? CordWinOrLost { get; set; }
        public int QuantityCordEvent { get; set; }
        [DisplayName("Ngày")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Số dây đạt được")]
        public int CountCord { get; set; }

        public TimeSpan StartEventTimes { get; set; }
        public TimeSpan EndEventTimes { get; set; }

        // Searching
        [DisplayName("Từ ngày")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }
        [DisplayName("Đến ngày")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        [DisplayName("Khung giờ")]
        public string Hours
        {
            get
            {
                return DateUtil.TimeSpanToString(StartEventTimes) + " - " + DateUtil.TimeSpanToString(EndEventTimes);
            }
        }

        [DisplayName("Dây")]
        public string CordWinOrLostStr {
            get {
                return CordWinOrLost == 1 ? "Thắng" : CordWinOrLost == 2 ? "Thua" : string.Empty;
            }
        }

        [DisplayName("Tên giải")]
        public string AwardName {
            get
            {
                return !IsRecall ? RaceTopName : "Triệu hồi";
            }
        }
    }
    public class SessionDice
    {
        public long SessionId { get; set; }
        public long BetTotalTai { get; set; }
        public long BetTotalXiu { get; set; }
        public long UserTotalTai { get; set; }
        public long UserTotalXiu { get; set; }
    }
    public class UserBetModel
    {
        //AccountID,BetSide,Amount,BetTime
        public long AccountID { get; set; }
        public int BetSide { get; set; }
        public long Amount { get; set; }
        public DateTime BetTime { get; set; }
    }
    public class UserBetBalance
    {
        public long AccountID { get; set; }
        public long SessionID { get; set; }
        public long Amount { get; set; }
        public long Refund { get; set; }
        public string AccountName { get; set; } = "Bot";
        public string Ip { get; set; } = "127.0.0.1";
        public string History { get; set; } = "";
        public byte Type { get; set; } = 0;
        public string Balance { get; set; } = "0";
        public string HistoryInOut { get; set; } = "";
    }
}