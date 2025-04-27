using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class UserToken
    {
        public long UserId { get; set; }
        public string Token { get; set; }
        public int ? DeviceType { get; set; }
        public string DeviceID { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}