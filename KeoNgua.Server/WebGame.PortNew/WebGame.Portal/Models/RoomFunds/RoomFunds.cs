using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models.RoomFunds
{
    public class RoomFunds
    {
        public long RoomID { get; set; }
        public long PrizeFund { get; set; }
        public long JackpotFund { get; set; }
    }
}