using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class GameFunds
    {
        public int GameID { get; set; }
        public int RoomID { get; set; }
        public long? PrizeFund { get; set; }
        public long? JackpotFund { get; set; }
        public string GameName { get; set; }

        public int CCU { get; set; }
        public string Displayname { get; set; }
    }
    public class CCUs
    {

        public string gamename { get; set; }

        public int CCU { get; set; }
    }
    public class JPRates
    {
        public int RoomID { get; set; }
        public long? JpRate { get; set; }

    }
}