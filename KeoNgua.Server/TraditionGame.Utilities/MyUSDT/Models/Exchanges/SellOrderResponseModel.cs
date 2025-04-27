using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Helpers.MyUSDT.Models.Exchanges
{
    public  class SellOrderResponseModel
    {
        /// <summary>
        /// id cặp tiền tệ (ở đây 1 là id cặp USDT-VCC)
        /// </summary>
        [JsonProperty("currency_pair")]
        public long CurrencyPair { get; set; }
        /// <summary>
        /// Status: trạng thái đơn hàng (pending, processing, completed,  canceled, failed)
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// sell
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        /// <summary>
        /// Amount: số lượng USDT (khi tạo order)
        /// </summary>
        [JsonProperty("amount")]
        public double Amount { get; set; }
        /// <summary>
        /// Rate: tỷ giá
        /// </summary>
        [JsonProperty("rate")]
        public double Rate { get; set; }
        /// <summary>
        /// Call_back: địa chỉ URL mà bạn muốn nhận được notify khi đơn hàng.
        // Chúng tôi sẽ gửi cho đến khi nhận được response 200 return từ bạn.
        //Tần suất gửi lại là 2, 3, 5, 8, 13, 21, 34, 55, 89 phút
        /// </summary>
        [JsonProperty("call_back")]
        public Uri CallBack { get; set; }
        /// <summary>
        /// Code: mã đơn hàng
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("timeout")]
        public long Timeout { get; set; }

        [JsonProperty("note")]
        public SellOrderResponseNote Note { get; set; }
        /// <summary>
        /// Email nhận thông báo số dư
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// Date_added: thời gian tạo đơn hàng
        /// </summary>
        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("remaining")]
        public long Remaining { get; set; }
        /// <summary>
        /// Amount_vcc: số lượng VND được chuyển tới tài khoản nhận (thực   chuyển)
        /// </summary>
        [JsonProperty("amount_vcc")]
        public long AmountVcc { get; set; }
        /// <summary>
        /// Amount_usdt: Số lượng USDT thanh toán (thực nhận)
        /// </summary>
        [JsonProperty("amount_usdt")]
        public double AmountUsdt { get; set; }
        public int HttpStatusCode { get; set; }
        public string HttpStatusMsg { get; set; }
    }
   

    public  class SellOrderResponseNote
    {
        /// <summary>
        ///  Target_amount: số lượng VND lúc tạo đơn hàng
        /// </summary>
        [JsonProperty("target_amount")]
        public long TargetAmount { get; set; }
        /// <summary>
        ///  Bank_name: tên ngân hàng
        /// </summary>
        [JsonProperty("bank_name")]
        public string BankName { get; set; }
        /// <summary>
        ///  Receive_usdt_address: địa chỉ USDT(rút USDT từ MyUSDTWallet tới  địa chỉ này để thanh toán đơn hàng này
        /// </summary>
        [JsonProperty("receive_usdt_address")]
        public string ReceiveUsdtAddress { get; set; }
        /// <summary>
        ///  Bank_account_name: tên chủ tài khoản sẽ nhận được VND
        /// </summary>
        [JsonProperty("bank_account_name")]
        public string BankAccountName { get; set; }
        /// <summary>
        /// Bank_account: số tài khoản sẽ nhận được VND
        /// </summary>
        [JsonProperty("bank_account")]
        public string BankAccount { get; set; }
    }
}