using HorseHunter.Server.Handlers;
using System;

namespace HorseHunter.Server.DataAccess.DTO
{
    public class BigWinner
    {
        public string Username { get; set; }
        public int RoomID { get; set; }
        public int RoomValue { get { return HorseHunterHandler.Instance.GetRoomValue(RoomID); } }
        public int BetValue { get; set; }
        public bool IsJackpot { get; set; }
        public long PrizeValue { get; set; }
        public int ServiceID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}