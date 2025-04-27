using MsWebGame.Portal.Helpers.MyUSDT.Models.Exchanges;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraditionGame.Utilities.SmartBanks.Model;

namespace TraditionGame.Utilities.SmartBanks.API.Withdraws
{
    public  class SmartbankWithdrawApi:BaseApiRequest
    {

        protected static string CALLBACK_ORDER = ConfigurationManager.AppSettings["SMARTBANK_CALLBACK_SELL_ORDER"].ToString();
        /// <summary>
        /// Lấy danh sách bank khi rút
        /// </summary>
        /// <returns></returns>
        public static SmartBankChargeModel GetBanks()
        {
            try
            {
                string BankUrl = BaseUrl + "/api/Partners/GetWithDrawBanks";
                int statusCode = 0;
                string msg = string.Empty;
                var model = GetJson<SmartBankChargeModel>(BankUrl, null, out statusCode, out msg);

                return model;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

   


        /// <summary>
        ///Gọi lệnh tạo order USDT (bán )
        /// </summary>
        /// <param name="TargetAmount"></param>
        /// <param name="BankAccount">số tài khoản người nhận VND</param>
        /// <param name="BankAccountName">Tên chủ tài khoản nhận VND</param>
        /// <param name="BankName">Tên ngân hàng</param>
        /// <returns></returns>
        //public static SellOrderResponseModel SendSellOrder(long TargetAmount, string BankAccount, string BankAccountName, string BankName)
        //{
        //    try
        //    {
        //        var request = new SellOrderRequestModel();
        //        request.CurrencyPair = CURRENT_PAIR;
        //        request.Method = METHOD;

        //        request.TargetAmount = TargetAmount;
        //        request.Email = EMAIL_BANK;
        //        request.CallBack = CALLBACK_ORDER;
        //        request.Note = new SellOrderRequestNoteModel()
        //        {
        //            BankAccount = BankAccount,
        //            BankAccountName = BankAccountName,
        //            BankName = BankName
        //        };

        //        var _url = BaseUrl + "/v1/orders/";
        //        int satusCode = 0;
        //        string msg = string.Empty;
        //        var model = PostJson<SellOrderResponseModel>(_url, JsonConvert.SerializeObject(request), null, out satusCode, out msg);
        //        model.HttpStatusCode = satusCode;
        //        model.HttpStatusMsg = msg;
        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //    }
        //    return null;

        //}

        /// <summary>
        /// hàm này là hàm xét duyệt của admin (chờ phản hồi call back
        /// </summary>
        /// <param name="Amount"></param>

        /// <returns></returns>
      


    }
}
