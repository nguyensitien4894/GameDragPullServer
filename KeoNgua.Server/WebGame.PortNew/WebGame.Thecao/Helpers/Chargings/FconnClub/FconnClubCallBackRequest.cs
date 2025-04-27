using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Helpers.Chargings.FconnClub
{
    public class FconnClubCallBackRequest
    {
        public long card_id { get; set; }
        public string name { get; set; }
        public string seri { get; set; }
        public string pin { get; set; }
        public string status { get; set; }
        public int value { get; set; }
        public int error_code { get; set; }
        public string error_desc { get; set; }
        public string refkey { get; set; }
    }
}