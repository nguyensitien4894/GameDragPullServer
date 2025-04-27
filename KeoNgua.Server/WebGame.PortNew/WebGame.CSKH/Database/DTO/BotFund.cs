namespace MsWebGame.CSKH.Database.DTO
{
    public class BotFund
    {
        public int FundSetID { get; set; }
        public long Balance { get; set; }
        public long TotalAddBalance { get; set; }
        public long InitValue { get; set; }
        public long PrizeValue { get; set; }
        public long PrizeMaxWin { get; set; }
        public long PrizeMaxLose { get; set; }
    }
    public class TruQuy
    {
        public int Gameid { get; set; }
        public long Roomid { get; set; }
        public long moneyde { get; set; }

    }
    public class JackPot
    {
        public int Gameid { get; set; }
        public long Roomid { get; set; }
        public string Displayname { get; set; }

    }
    public class RateJp
    {
        public int Roomid { get; set; }
        public long Rate { get; set; }

    }
}