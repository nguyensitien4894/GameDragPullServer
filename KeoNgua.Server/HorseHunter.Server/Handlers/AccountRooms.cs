using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseHunter.Server.Handlers
{
    public class AccountRooms
    {
        public int AccountID { get; set; }
        public int RoomID { get; set; }
        public string Username { get; set; }
        public long LastSpinID { get; set; }
        public int FreeSpins { get; set; }
        public int SessionFreeSpins { get; set; }
        public long TotalFreeSpins { get; set; }
        public int PrizeValueFreeSpins { get; set; }
        public string LastLineData { get; set; }
        public long LastPrizeValue { get; set; }
        public DateTime CreatedTime { get; set; }
        public int EventFreeSpins { get; set; }

        public AccountRooms()
        {
            CreatedTime = DateTime.Now;
        }
    }
}