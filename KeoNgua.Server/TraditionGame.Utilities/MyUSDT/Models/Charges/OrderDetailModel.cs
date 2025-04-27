using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Charges
{
    public  class OrderDetailModel
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
        public string Amount { get; set; }
        /// <summary>
        /// tỷ giá
        /// </summary>
        [JsonProperty("rate")]
        public string Rate { get; set; }
        /// <summary>
        /// địa chỉ URL mà bạn muốn nhận được notify khi đơn hàng.(Chúng tôi sẽ gửi cho đến khi nhận được response 200 return từ bạn. Tần
        /// suất gửi lại là 2, 3, 5, 8, 13, 21, 34, 55, 89 phút)
        /// </summary>
        [JsonProperty("call_back")]
        public string CallBack { get; set; }
        /// <summary>
        /// mã code order
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// time out
        /// </summary>
        [JsonProperty("timeout")]
        public long Timeout { get; set; }

        [JsonProperty("note")]
        public OrderDetailNote Note { get; set; }
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
        public long Remaining { get; set; }
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

    public  class OrderDetailNote
    {
        /// <summary>
        /// Danh sách bank giao dịch
        /// </summary>
        [JsonProperty("banks")]
        public OrderDetailBank[] Banks { get; set; }
        /// <summary>
        /// số lượng tiền VND muốn gửi
        /// </summary>
        [JsonProperty("target_amount")]
        public long TargetAmount { get; set; }
        /// <summary>
        /// địa chỉ nhận USDT (bạn nhập địa chỉ ví trên MyUSDTWallet vào đây)
        /// </summary>
        [JsonProperty("receive_usdt_address")]
        public string ReceiveUsdtAddress { get; set; }
    }

    public  class OrderDetailBank
    {
        /// <summary>
        /// SHOW.Bank_name: tên ngân hàng
        /// </summary>
        [JsonProperty("bank_name")]
        public string BankName { get; set; }
        /// <summary>
        ///SHOW. Master_bank_name: tên chủ tài khoản cần gửi VND vào
        /// </summary>
        [JsonProperty("master_bank_name")]
        public string MasterBankName { get; set; }
        /// <summary>
        /// SHOW.số tài khoản cần gửi VND vào 
        /// </summary>
        [JsonProperty("master_bank_account")]
        public string MasterBankAccount { get; set; }
    }
}