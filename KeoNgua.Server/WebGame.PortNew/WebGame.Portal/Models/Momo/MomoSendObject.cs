using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models.Momo
{
    public class MomoSendObject
    {
        public string user { get; set; }
        public string msgType { get; set; }
        public string pass { get; set; }
        public string cmdId { get; set; }
        public string lang { get; set; }
        public long time { get; set; }
        public string channel { get; set; }
        public int appVer { get; set; }
        public string appCode { get; set; }
        public string deviceOS { get; set; }
        public bool result { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public MomoMsg momoMsg { get; set; }
        public Extra extra { get; set; }
    }
    public class MomoMsg
    {
        public string _class { get; set; }
        public bool isSetup { get; set; }
    }

    public class Extra
    {
        public string pHash { get; set; }
    }
}