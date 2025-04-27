using System.ComponentModel;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Games
{
    public class GameIndexModel
    {
        public int GameID { get; set; }
    
        public  long TotalBet { get; set; }
        public  long TotalPrizeValue { get; set; }

        [DisplayName("Tổng đặt")]
        public string TotalBetFormat
        {
            get
            {
                return TotalBet.LongToMoneyFormat();
            }
        }
        [DisplayName("Tổng thưởng")]
        public string TotalPrizeValueFormat
        {
            get
            {
                return TotalPrizeValue.LongToMoneyFormat();
            }
        }
        [DisplayName("Tên game")]
        public string GameName { get; set; }

    }
}