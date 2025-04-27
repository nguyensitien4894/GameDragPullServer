using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TraditionGame.Utilities.XBomms.Models
{
   public  class XBoomRequestModel
    {
        [JsonProperty("telco")]
        public string Telco { get; set; }

        [JsonProperty("amount")]
   
        public long Amount { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("scratchCallbackUrl")]
        public string ScratchCallbackUrl { get; set; }
    }
}
