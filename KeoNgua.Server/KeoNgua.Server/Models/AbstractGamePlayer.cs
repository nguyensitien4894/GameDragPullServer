using KeoNgua.Server.DataAccess.Dto;
using Newtonsoft.Json;
using System;

namespace KeoNgua.Server.Models
{
    public abstract class AbstractGamePlayer
    {
        public AccountDB Account { get; set; }
        public long AccountID { get; set; }
        [JsonIgnore]
        public DateTime LastActiveTime { get; set; }

        public AbstractGamePlayer(AccountDB data)
        {
            this.Account = data;
            this.AccountID = data.AccountID;
        }

        public long IdleTime()
        {
            return (long)((DateTime.Now - LastActiveTime).TotalSeconds);
        }
    }
}