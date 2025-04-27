using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers.MyUSDT.Models.Exchanges;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Exchanges;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.MyUSDT.Charges;
using TraditionGame.Utilities.MyUSDT.Models.Exchanges;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.SmartBanks.API.Withdraws;
using TraditionGame.Utilities.Utils;
using WebGame.Payment.Database.DAO;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/SmartBank")]
    public class SmartBankController : BaseApiController
    {
        /// <summary>
        /// Lấy danh sách banks và config
        /// </summary>
        /// <returns></returns>
        [ActionName("GetExchangeConfig")]
        [HttpGet]
        public dynamic GetExchangeConfig()
        {
            try
            {
                var accountId = AccountSession.AccountID;

                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }

                var banks = SmartbankWithdrawApi.GetBanks();
                var objReturn = banks.Banks;
                //2.Lấy thông tin Rate
                string minValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_WITHDRAW_LOWER_LIMIT", out minValue);
                var min = ConvertUtil.ToLong(minValue);
                string maxValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_WITHDRAW_UPPER_LIMIT", out maxValue);
                var max = ConvertUtil.ToLong(maxValue);

                double rate = 0;
                var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
                if (bankOperator == null || !bankOperator.Any())
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Can't find rate configuration",
                    };
                }

                var firstBanks = bankOperator.FirstOrDefault();
                if (firstBanks == null)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Can't find rate configuration",
                    };
                }

                if (!firstBanks.ExchangeStatus.HasValue || (firstBanks.ExchangeStatus.HasValue && !firstBanks.ExchangeStatus.Value))
                {
                    return new
                    {
                        ResponseCode = -100089,
                        Message = "The bank withdrawal system is under maintenance",
                    };
                }
                rate = firstBanks.ExchangeRate.Value;
                return new
                {
                    ResponseCode = 1,
                    Rate = rate,
                    Min = min,
                    Max = max,
                    Banks = objReturn
                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = "The system is busy, please come back later"
                };
            }
        }
    }
}
