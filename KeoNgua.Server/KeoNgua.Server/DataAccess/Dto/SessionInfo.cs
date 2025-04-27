using System;

namespace KeoNgua.Server.DataAccess.Dto
{
    public class SessionInfo
    {
        public string DisplayName { get; set; }
        public long Bet { get; set; }
        public long Refund { get; set; }
        public int BetSide { get; set; }
        public int ServiceID { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeFm { get { return CreateTime.ToString("dd/MM/yyyy HH:mm:ss"); } }
    }
}