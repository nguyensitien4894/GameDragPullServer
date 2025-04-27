using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models.Momo
{
    public class MomoInfoResponse
    {
        public int status { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
    }

    public class MomoInfor
    {
        public int error { get; set; }
        public string phone { get; set; }
    }
}