using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Momos.Models
{
    public class MomoCallBack
    {
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("game_acc")]
        public string GameAcc { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }
    }
}
