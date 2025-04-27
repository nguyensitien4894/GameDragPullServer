using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Redis.Egypt
{
    public class RoomFunds
    {
        public long RoomID { get; set; }
        public long PrizeFund { get; set; }
        public long DoubleFund { get; set; }
        public long JackpotFund { get; set; }
    }
}