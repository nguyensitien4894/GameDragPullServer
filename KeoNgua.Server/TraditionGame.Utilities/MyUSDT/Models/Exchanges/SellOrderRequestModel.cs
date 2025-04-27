using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Exchanges
{
    public  class SellOrderRequestModel
    {
        /// <summary>
        /// id cặp tiền tệ (ở đây 1 là id cặp USDT-VCC)
        /// </summary>
        [JsonProperty("currency_pair")]
        public int  CurrencyPair { get; set; }
        /// <summary>
        /// : số lượng tiền VND muốn rút (chưa trì phí)
        /// </summary>
        [JsonProperty("target_amount")]
        public long TargetAmount { get; set; }
        /// <summary>
        ///  email nhận notify cập nhật tình trạng đơn hàng
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// sell - khi muốn rút tiền
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        /// <summary>
        ///  Thông tin tài khoản ngân hàng được rút VND về
        /// </summary>
        [JsonProperty("note")]
        public SellOrderRequestNoteModel Note { get; set; }
        /// <summary>
        /// địa chỉ URL mà bạn muốn nhận được notify đơn hàng.
        /// Chúng tôi sẽ gửi cho đến khi nhận được response 200 return từ bạn.
        /// Tần suất gửi lại là 2, 3, 5, 8, 13, 21, 34, 55, 89 phút
        /// </summary>
        [JsonProperty("call_back")]
        public string CallBack { get; set; }
    }

    public  class SellOrderRequestNoteModel
    {
        /// <summary>
        /// số tài khoản người nhận VND
        /// </summary>
        [JsonProperty("bank_account")]
        public string BankAccount { get; set; }
        /// <summary>
        /// Tên chủ tài khoản nhận VND
        /// </summary>
        [JsonProperty("bank_account_name")]
        public string BankAccountName { get; set; }
        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        [JsonProperty("bank_name")]
        public string BankName { get; set; }
    }
}