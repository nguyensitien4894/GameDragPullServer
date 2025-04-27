using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class AccountInfo
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public int AvatarID { get; set; }
        public long Balance { get; set; }
        public int TotalDraw { get; set; }
        public int TotalLose { get; set; }
        public int TotalWin { get; set; }
        public DateTime LastActiveTime { get; set; }
        public bool IsFriend { get; set; }
    }
}