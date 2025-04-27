namespace MsWebGame.Portal.Database.DTO
{
    public class EventXJackpot
    {
        public int GameID { get; set; }
        public int RoomID { get; set; }
        public int Multiplier { get; set; }
        public int Remain { get; set; }
        public int JackpotRemainInDay { get; set; }
    }
}