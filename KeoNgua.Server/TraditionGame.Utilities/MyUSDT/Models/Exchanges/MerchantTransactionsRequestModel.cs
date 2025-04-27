using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Exchanges
{
    public class MerchantTransactionsRequestModel
    {
        /// <summary>
        /// số lượng rút (chưa trừ phí)
        /// </summary>
        [JsonProperty("amount")]
        public double Amount { get; set; }
        /// <summary>
        /// Address: địa chỉ nhận USDT (nhập địa chỉ cần thanh toán USDT của đơn hàng bán USDT trên traodoiUSDT)
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }
    }
}