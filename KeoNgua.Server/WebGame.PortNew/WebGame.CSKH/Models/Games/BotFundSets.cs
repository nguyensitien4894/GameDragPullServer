using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Games
{
    public class BotFundSets
    {
        public long PrizeValue { get; set; }
        public long Balance { get; set; }
        public long TotalAddBalance { get; set; }
        public long PrizeMaxWin { get; set; }
        public long PrizeMaxLose { get; set; }
    }
}