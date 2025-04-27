using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class GameIndex
    {
        public int GameID { get; set; }

        public decimal TotalBet { get; set; }
        public decimal TotalPrizeValue { get; set; }
        public string GameName { get; set; }
    }
}