using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Helpers.Muathe24
{
    public class MuatheResult
    {
        public MuatheResult()
        {
            LstCards = new List<CardBuy>();
        }
        [JsonProperty("errorCode")]
        public long ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public string DefineMsg
        {
            get
            {
                if (ErrorCode == 0)
                {
                    return "Thành công | " + ErrorCode;
                }
                if (ErrorCode == -1)
                {
                    return "Đăng nhập không thành công | " + ErrorCode;
                }
                if (ErrorCode == 5)
                {
                    return "Kho hết thẻ | " + ErrorCode;
                }
                if (ErrorCode == 278)
                {
                    return "Không tồn tại tài khoản | " + ErrorCode;
                }
                if (ErrorCode == 777)
                {
                    return "Lỗi hệ thống |" + ErrorCode;
                }
                if (ErrorCode == 778)
                {
                    return "Không đủ số dư | " + ErrorCode;
                }
                else
                {
                    return "Mã lỗi không quy ước với khách hàng |" + ErrorCode;
                }
            }
        }
        public List<CardBuy> LstCards { get; set; }

    }

    public class CardBuy
    {
        [JsonProperty("PinCode")]
        public string PinCode { get; set; }

        [JsonProperty("Telco")]
        public string Telco { get; set; }

        [JsonProperty("Serial")]
        public string Serial { get; set; }

        [JsonProperty("Amount")]
        public int Amount { get; set; }

        [JsonProperty("Trace")]

        public long Trace { get; set; }
    }
}