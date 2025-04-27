using Newtonsoft.Json;
using System;

namespace KeoNgua.Server.DataAccess.Dto
{
    public class BotDB
    {
        [JsonIgnore]
        public long AccountID { get; set; }
        [JsonIgnore]
        public long AbsAccountID { get { return Math.Abs(this.AccountID); } }
        public string NickName { get; set; }
        public long Star { get; set; }
        public long Coin { get; set; }
        public int DeviceID { get; set; }
        public int ServiceID { get; set; }
        public int Avatar { get; set; }
        public int Step { get; set; }
        public long SessionID { get; set; }
        public DateTime LastActiveTime { get; set; }

        public long IdleTime()
        {
            return (long)((DateTime.Now - this.LastActiveTime).TotalSeconds);
        }

        public void SetActive()
        {
            this.LastActiveTime = DateTime.Now;
        }
    }
}