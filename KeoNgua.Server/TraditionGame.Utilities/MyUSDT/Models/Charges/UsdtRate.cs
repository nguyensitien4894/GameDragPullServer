using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.MyUSDT.Models.Charges
{
    public partial class UsdtRate
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("sell_rate")]
        public double SellRate { get; set; }

        [JsonProperty("buy_rate")]
        public double BuyRate { get; set; }

        [JsonProperty("deposit_fee_flat")]
        public double DepositFeeFlat { get; set; }

        [JsonProperty("withdraw_fee_flat")]
        public double WithdrawFeeFlat { get; set; }

        [JsonProperty("deposit_fee_pct")]
        public double DepositFeePct { get; set; }

        [JsonProperty("withdraw_fee_pct")]
        public double WithdrawFeePct { get; set; }

        [JsonProperty("user")]
        public double User { get; set; }

        public int HttpStatusCode { get; set; }
    }
}
