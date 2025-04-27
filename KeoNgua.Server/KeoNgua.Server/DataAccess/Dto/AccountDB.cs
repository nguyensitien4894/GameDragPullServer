namespace KeoNgua.Server.DataAccess.Dto
{
    public class AccountDB
    {
        public long AccountID { get; set; }
        public string NickName { get; set; }
        public long Balance { get; set; }
        public int DeviceID { get; set; }
        public int ServiceID { get; set; }
        public int Avatar { get; set; }
        public int Vip { get; set; }

        public AccountDB() { }

        public AccountDB(AccountDB data)
        {
            this.AccountID = data.AccountID;
            this.NickName = data.NickName;
            this.Balance = data.Balance;
            this.DeviceID = data.DeviceID;
            this.ServiceID = data.ServiceID;
            this.Avatar = data.Avatar;
            this.Vip = data.Vip;
        }

        public AccountDB(long accountId, string nickName, long balance, int deviceId, int serviceId, int avatar, int vip)
        {
            this.AccountID = accountId;
            this.NickName = nickName;
            this.Balance = balance;
            this.DeviceID = deviceId;
            this.ServiceID = serviceId;
            this.Avatar = avatar;
            this.Vip = vip;
        }
    }
}