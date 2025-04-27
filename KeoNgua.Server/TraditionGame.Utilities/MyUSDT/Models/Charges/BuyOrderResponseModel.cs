using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Charges
{
    public  class OrderResponseModel
    {
        /// <summary>
        /// id cặp tiền tệ (ở đây 1 là id cặp USDT-VCC)
        /// </summary>
        [JsonProperty("currency_pair")]
        public long CurrencyPair { get; set; }
        /// <summary>
        /// Status: trạng thái đơn hàng (pending, processing, completed, canceled,failed)
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// buy - khi muốn nạp tiền
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        /// <summary>
        /// Amount: Lượng USDT (lúc tạo đơn hàng)
        /// </summary>
        [JsonProperty("amount")]
        public double Amount { get; set; }
        /// <summary>
        /// tỷ giá
        /// </summary>
        [JsonProperty("rate")]
        public double Rate { get; set; }
        /// <summary>
        /// địa chỉ URL mà bạn muốn nhận được notify khi đơn hàng. Chúng tôi sẽ gửi cho đến khi nhận được response 200 return từ bạn.Tần suất gửi lại là2, 3, 5, 8, 13, 21, 34, 55, 89 phút
        /// </summary>
        [JsonProperty("call_back")]
        public string CallBack { get; set; }
        /// <summary>
        /// SHOW.Nội dung chuyển khoản(User phải điền chính xác nội dung chuyển khoản (không phân biệt hoa thường)
         /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// SHOW.Thời gian hết hạn của order
        /// </summary>
        [JsonProperty("timeout")]
        public long Timeout { get; set; }
        /// <summary>
        /// Thông tin note
        /// </summary>
        [JsonProperty("note")]
        public OrderResponseNote Note { get; set; }
        /// <summary>
        /// Email nhận thông báo
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// thời gian tạo đơn hàng
        /// </summary>
        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("remaining")]
        public double Remaining { get; set; }
        /// <summary>
        /// số lượng VND chúng tôi nhận được thanh toán
        /// </summary>
        [JsonProperty("amount_vcc")]
        public long AmountVcc { get; set; }
        /// <summary>
        /// Amount_usdt: số lượng USDT nạp vào ví
        /// </summary>
        [JsonProperty("amount_usdt")]
        public double AmountUsdt { get; set; }

        public int HttpStatusCode { get; set; }
        public string HttpMsg { get; set; }
    }

    public  class OrderResponseNote
    {
        /// <summary>
        /// địa chỉ USDT của ví sẽ nhận được USDT
        /// </summary>
        [JsonProperty("receive_usdt_address")]
        public string ReceiveUsdtAddress { get; set; }
        /// <summary>
        /// số lượng VND lúc tạo đơn hàng
        /// </summary>
        [JsonProperty("target_amount")]
        public long TargetAmount { get; set; }
        /// <summary>
        /// Banks: danh sách thông tin tài khoản ngân hàng để thanh toán
        /// </summary>
        [JsonProperty("banks")]
        public OrderResponseBank[] Banks { get; set; }
    }

    public  class OrderResponseBank
    {
        /// <summary>
        /// SHOW.số tài khoản cần gửi VND vào 
        /// </summary>
        [JsonProperty("master_bank_account")]
        public string MasterBankAccount { get; set; }
        /// <summary>
        ///SHOW. Master_bank_name: tên chủ tài khoản cần gửi VND vào
        /// </summary>
        [JsonProperty("master_bank_name")]
        public string MasterBankName { get; set; }
        /// <summary>
        /// SHOW.Bank_name: tên ngân hàng
        /// </summary>
        [JsonProperty("bank_name")]
        public string BankName { get; set; }
    }
}