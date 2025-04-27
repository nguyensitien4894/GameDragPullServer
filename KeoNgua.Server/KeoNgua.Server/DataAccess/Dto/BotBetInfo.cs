using Newtonsoft.Json;
using System;

namespace KeoNgua.Server.DataAccess.Dto
{
    public class BotBetInfo
    {
        [JsonIgnore]
        public long BotID { get; set; }
        public long AccountID { get { return Math.Abs(this.BotID); } }
        public long BetValue { get; set; }
        public int BetSide { get; set; }
        public int Group { get; set; }

        public BotBetInfo() { }

        public BotBetInfo(long botId, long betVal, int betSide)
        {
            this.BotID = botId;
            this.BetValue = betVal;
            this.BetSide = betSide;
        }
    }
}