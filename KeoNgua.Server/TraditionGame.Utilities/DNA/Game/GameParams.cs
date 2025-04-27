using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.DNA.Game
{
    public partial class GameParamaters
    {
        [JsonProperty("eventList")]
        public List<GameEventItem> EventList { get; set; }
    }

    public partial class GameEventItem
    {
        [JsonProperty("eventName")]
        public string EventName { get; set; }

        [JsonProperty("userID")]
        public string UserId { get; set; }

        [JsonProperty("eventTimestamp")]
        public string EventTimestamp { get; set; }

        [JsonProperty("eventUUID")]
        public string EventUuid { get; set; }

        [JsonProperty("eventParams")]
        public GameEventParams EventParams { get; set; }
    }
  
    public partial class GameEventParams
    {
        [JsonProperty("userScore")]
        public long UserScore { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("betAmount")]
        public long BetAmount { get; set; }
        [JsonProperty("winAmount")]
        public long WinAmount { get; set; }
        [JsonProperty("refundAmount")]
        public long RefundAmount { get; set; }

       

    }
}
