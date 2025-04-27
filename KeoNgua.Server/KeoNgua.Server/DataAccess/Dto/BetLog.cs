namespace KeoNgua.Server.DataAccess.Dto
{
    public class BetLog
    {
        public long AccountID { get; private set; }
        public long BetValue { get; private set; }
        public int BetSide { get; private set; }
        public string Nickname { get; private set; }
        public bool Bot { get; private set; } = true;
        internal BetLog(long accountId, long betVal, int betSide, string Nickname = "Bot", bool Bot = true)
        {
            this.AccountID = accountId;
            this.BetValue = betVal;
            this.BetSide = betSide;
            this.Nickname = Nickname;
            this.Bot = Bot;
        }
    }
}