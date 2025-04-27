using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Momos.Models
{
    public class MomoResponse
    {
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("game_acc")]
        public string GameAcc { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("wallet_account")]
        public string WalletAccount { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("wallet_account_name")]
        public string walletAccountName { get; set; }
        public int statusCode { get; set; }
    }
}
