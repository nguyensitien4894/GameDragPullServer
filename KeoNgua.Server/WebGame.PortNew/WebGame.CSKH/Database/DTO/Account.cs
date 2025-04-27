using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Account
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public int AvatarID { get; set; }
        public long Balance { get; set; }
        public int Status { get; set; }
        public int Gender { get; set; }
        public DateTime BirthDay { get; set; }
        public string PhoneNumber { get; set; }
        public int PendingMessage { get; set; }
        public int PendingGiftcode { get; set; }
        public int TotalWin { get; set; }
        public int TotalLose { get; set; }
        public int TotalDraw { get; set; }
        public bool IsUpdateAccountName { get; set; }
        public int? AuthenType { get; set; }
        public long RankID { get; set; }
        public long VP { get; set; }
        public string RankName { get; set; }
        public int ServiceID { get; set; }


    
        public string PhoneSafeNo { get; set; }
        public string SignalID { get; set; }
        public long SafeBalance { get; set; }

        public int IsFB { get; set; }
        public int IsFBReward { get; set; }

        public int IsUpdatedFB { get; set; }

        public long TelegramID { get; set; }
    }
}