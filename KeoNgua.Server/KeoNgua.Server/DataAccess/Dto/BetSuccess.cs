namespace KeoNgua.Server.DataAccess.Dto
{
    public class BetSuccess
    {
        public long AccountID { get; set; }
        public long BetValue { get; set; }
        public int BetSide { get; set; }
        public long SummaryBet { get; set; }

        public BetSuccess(long accountId, long betVal, int betSide, long summaryBet)
        {
            this.AccountID = accountId;
            this.BetValue = betVal;
            this.BetSide = betSide;
            this.SummaryBet = summaryBet;
        }
    }
}