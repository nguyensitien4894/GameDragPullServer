using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.DNA.VP
{
    public partial class VPParamaters
    {
        [JsonProperty("eventList")]
        public List<VPEventItem> EventList { get; set; }
    }

    public partial class VPEventItem
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
        public VPEventParams EventParams { get; set; }
    }
    public  partial class  VPEventParams
    {

       

        [JsonProperty("vpReceived")]
        public int VpReceived { get; set; }

    }
}
