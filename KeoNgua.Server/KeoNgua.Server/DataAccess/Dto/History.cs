using System;

namespace KeoNgua.Server.DataAccess.Dto
{
    public class History
    {
        public long SessionID { get; set; }
        public int GateID { get; set; }
        public int Dice1 { get; set; }
        public int Dice2 { get; set; }
        public int Dice3 { get; set; }
        public long Bet { get; set; }
        public long Award { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeFm { get { return CreateTime.ToString("dd/MM/yyyy HH:mm:ss"); } }
    }
}