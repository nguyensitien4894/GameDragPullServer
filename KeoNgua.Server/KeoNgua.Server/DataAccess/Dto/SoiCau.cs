using System;

namespace KeoNgua.Server.DataAccess.Dto
{
    public class SoiCau
    {
        public long SessionID { get; set; }
        public int FirstDice { get; set; }
        public int SecondDice { get; set; }
        public int ThirdDice { get; set; }
        //public DateTime CreatedDate { get; set; }
    }
}