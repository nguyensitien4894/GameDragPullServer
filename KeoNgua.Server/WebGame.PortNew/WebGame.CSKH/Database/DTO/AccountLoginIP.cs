using System;
using System.Collections.Generic;

namespace MsWebGame.CSKH.Database.DTO
{
    public class AccountLoginIP
    {
        public int DeviceType { get; set; }
        public DateTime LoginTime { get; set; }
        public string ClientIP { get; set; }
    }
    public class ParConfigLive
    {
        public long UserId { get; set; }
        public List<string> Tranfer { get; set; } = new List<string>();
        public List<string> Game { get; set; } = new List<string>();
        public byte Type { get; set; } = 0;
    }
}