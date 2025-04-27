using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Bot
{
    public class BotModel
    {
        [DisplayName("Bot")]
        public long BotId { get; set; }
        public long BotGroupId { get; set; }
        [DisplayName("Tên bot")]
        public string BotName { get; set; }
        [DisplayName("Tên hiển thị")]
        public string DisplayName { get; set; }
        public TimeSpan StartTime { get; set; }
        [DataType(DataType.DateTime)]
        public TimeSpan FinishTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        public int BotsPlayMin { get; set; }
        public int BotsPlayMax { get; set; }
        public int PermanentBots { get; set; }
        public int QuantityBots { get; set; }

        public int BotType { get; set; }
        public string BotTypeName
        {
            get
            {
                return BotType == 1 ? "Thường" : BotType == 2 ? "Đặc biệt" : "";
            }
        }

        public int ByResult { get; set; }
    }

    public class TimeSetModel
    {
        public int TimeSetId { get; set; }
        public string BotName { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan? FinishTime { get; set; }
    }

    public class BotBet
    {
        public long SessionID { get; set; }
        public long AccountID { get; set; }
        public string BotName { get; set; }
        public long Bet { get; set; }
        public int BetSide { get; set; }
        public long Award { get; set; }
        public long Refund { get; set; }
        public long Balance { get; set; }
        public string BetFmt {
            get
            {
                return MoneyExtension.LongToMoneyFormat(Bet);
            }
        }

        public string AwardFmt {
            get {
                return MoneyExtension.LongToMoneyFormat(Award);
            }
        }
        public string RefundFmt {
            get
            {
                return MoneyExtension.LongToMoneyFormat(Refund);
            }
        }
        public string BalanceFmt {
            get
            {
                return MoneyExtension.LongToMoneyFormat(Balance);
            }
        }
        public int Dice1 { get; set; }
        public int Dice2 { get; set; }
        public int Dice3 { get; set; }
        public DateTime CreateTime { get; set; }
    }

}