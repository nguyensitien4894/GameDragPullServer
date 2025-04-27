using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Helpers.MyUSDT.Models.Exchanges
{
    public class WithDrawBankListModel
    {
        /// <summary>
        /// Danh sách bank khi rut
        /// </summary>
        [JsonProperty("withdraw_bank_list")]
        public string[] WithdrawBankList { get; set; }

    }
}