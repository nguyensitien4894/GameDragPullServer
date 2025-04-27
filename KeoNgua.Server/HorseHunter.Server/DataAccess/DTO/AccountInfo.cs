namespace HorseHunter.Server.DataAccess.DTO
{
    public class AccountInfo
    {
        public int EventFreeSpin { get; set; }
        public long Jackpot { get; set; }
        public long Balance { get; set; }
        public int Response { get; set; }

        public AccountInfo() { }

        public AccountInfo (int efree, long jp, long balance, int res)
        {
            this.EventFreeSpin = efree;
            this.Jackpot = jp;
            this.Balance = balance;
            this.Response = res;
        }
    }
}