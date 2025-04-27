using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraditionGame.Utilities.SmartBanks.Model;

namespace TraditionGame.Utilities.SmartBanks.API.Charges
{
    public class SmartBankChargeApi:BaseApiRequest
    {

       
           

            protected static string CALLBACK_ORDER = ConfigurationManager.AppSettings["SMARTBANK_CALLBACK_BUY_ORDER"].ToString();

            /// <summary>
            /// lấy thông tin rate 
            /// </summary>
            /// <returns></returns>
        

            /// <summary>
            /// Get Danh sach bank Nap
            /// </summary>
            /// <returns></returns>
            public static SmartBankChargeModel  GetBankCharge()
            {
                try
                {
                    string BankUrl = BaseUrl + "/api/Partners/GetRechargeBanks";
                    int statusCode;
                    string msg;
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
            /// Hệ thống tạo lệnh mua USDT rồi trả về thông tin chi   tiết chuyển khoản  ạo lệnh MUA USDT
            /// </summary>
            /// <returns></returns>
            public static SmartBankChargeResquestModel SendBuyOrder(long TargetAmount,string bankCode)
            {
                try
                {
                    var request = new SmartBankChargeRequestModel();
             
                
                    request.Amount = TargetAmount;
                 
                    request.CallBackUrl = CALLBACK_ORDER;
                    request.BankCode = bankCode;
                NLogManager.LogMessage(JsonConvert.SerializeObject(request));

                    var _url = BaseUrl + "/api/Partners/CreateReChargeOrder";
                    int statusCode = 0;
                    string msg = string.Empty;
                    var model = PostJson<SmartBankChargeResquestModel>(_url, JsonConvert.SerializeObject(request), null, out statusCode, out msg);
                    model.HttpStatusCode = statusCode;
                    model.HttpMsg = msg;
                    return model;
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
                return null;

            }

          
        
    }
}
