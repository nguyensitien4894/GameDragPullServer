namespace MsWebGame.CSKH.Database.DTO
{
    public class TransferDTO
    {
        public string ReceiverName { get; set; }
        public string Amount { get; set; }
        public string OrgAmount { get; set; }
        public int WalletType { get; set; }
        public string Note { get; set; }
        public long Balance { get; set; }
    }
}