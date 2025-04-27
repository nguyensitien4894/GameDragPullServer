using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class BigWiner
    {
        public int GameID { get; set; }
        public string AccountName { get; set; }
        public long PrizeValue { get; set; }
        public bool IsJackpot { get; set; }
        public int ServiceID { get; set; }



    }
}