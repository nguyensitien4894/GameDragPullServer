using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class TopJackPot
    {
        public int GameID { get; set; }
        public int RoomID { get; set; }
        public long JackpotFund { get; set; }
        public bool IsEventJackpot { get; set; }
       
    }
}