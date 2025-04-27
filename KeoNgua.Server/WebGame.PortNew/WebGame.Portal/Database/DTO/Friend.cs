using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class Friend
    {
        public long FriendID { get; set; }
        public string AccountName { get; set; }
        public bool IsOnline { get; set; }
    }
}