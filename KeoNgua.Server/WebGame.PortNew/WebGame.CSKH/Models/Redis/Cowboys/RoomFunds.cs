using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Redis.Cowboys
{
    public class RoomFunds
    {
        public long RoomID { get; set; }
        public long PrizeFund { get; set; }
        public long JackpotFund { get; set; }
        public long PyramidFund { get; set; }
        public long DoubleFund { get; set; }
    }
}