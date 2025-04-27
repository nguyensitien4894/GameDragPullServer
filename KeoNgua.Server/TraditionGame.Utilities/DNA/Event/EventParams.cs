using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.DNA.Event
{
    
    public partial class EventParamaters
    {
        [JsonProperty("eventList")]
        public List<EventEventItem> EventList { get; set; }
    }

    public partial class EventEventItem
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
        public EventParams EventParams { get; set; }
    }

  

    public partial class EventParams
    {

     
        [JsonProperty("eName")]
        public string EName { get; set; }

        
        [JsonProperty("eAward")]
        public long EAward { get; set; }

    }
}
