using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Charges
{
    public  class OrderRequestModel
    {
        /// <summary>
        /// id cặp tiền tệ (ở đây 1 là id cặp USDT-VCC)
        /// </summary>
        [JsonProperty("currency_pair")]
        public int CurrencyPair { get; set; }
        /// <summary>
        /// số lượng tiền VND muốn gửi
        /// </summary>
        [JsonProperty("target_amount")]
        public long TargetAmount { get; set; }
        /// <summary>
        /// email nhận notify cập nhật tình trạng đơn hàng
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// buy - khi muốn nạp tiền
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        /// <summary>
        /// địa chỉ URL mà bạn muốn nhận được notify khi đơn hàng.(Chúng tôi sẽ gửi cho đến khi nhận được response 200 return từ bạn. Tần
        /// suất gửi lại là 2, 3, 5, 8, 13, 21, 34, 55, 89 phút)
        /// </summary>
        [JsonProperty("call_back")]
        public string CallBack { get; set; }
        /// <summary>
        /// note địa chỉ nhận  usdt address
        /// </summary>
        [JsonProperty("note")]
        public OrderRequestNote Note { get; set; }
    }

    public  class OrderRequestNote
    {
        /// <summary>
        /// địa chỉ nhận USDT (bạn nhập địa chỉ ví trên MyUSDTWallet vào đây)
        /// </summary>
        [JsonProperty("receive_usdt_address")]
        public string ReceiveUsdtAddress { get; set; }
    }
}