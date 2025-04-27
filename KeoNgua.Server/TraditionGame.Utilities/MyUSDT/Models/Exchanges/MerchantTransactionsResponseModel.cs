using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Exchanges
{
    public class MerchantTransactionsResponseModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("wallet")]
        public long Wallet { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("transaction_hash")]
        public string TransactionHash { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("real_amount")]
        public double RealAmount { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("required_confirmation")]
        public long RequiredConfirmation { get; set; }

        [JsonProperty("is_x")]
        public bool IsX { get; set; }

        [JsonProperty("fee")]
        public string Fee { get; set; }
        public int HttpStatusCode { get; set; }
        public string HttpMsg { get; set; }
    }
}