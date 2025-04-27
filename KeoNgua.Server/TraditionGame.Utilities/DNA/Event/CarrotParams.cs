using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.DNA.Event
{
    public class CarrotParamaters
    {
        [JsonProperty("eventList")]
        public List<CarrotEventItem> EventList { get; set; }
    }


    public class CarrotEventItem
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
        public CarrotDetailParams EventParams { get; set; }
    }



    public class CarrotDetailParams
    {

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("action")]
        public string action { get; set; }
        [JsonProperty("rewardAmount")]
        public long RewardAmount { get; set; }
        [JsonProperty("rewardType")]
        public string RewardType { get; set; }

    }
}

