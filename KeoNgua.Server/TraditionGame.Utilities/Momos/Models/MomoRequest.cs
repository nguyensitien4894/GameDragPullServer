using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Momos.Models
{
    public class MomoRequest
    {
        [JsonProperty("amount")]
        public long Amount { get; set; }
        public string momo_id { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("provider")]
        public string Provider { get; set; }
    }
}
