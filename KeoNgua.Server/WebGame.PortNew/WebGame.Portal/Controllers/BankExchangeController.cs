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
using TraditionGame.Utilities.Utils;
using WebGame.Payment.Database.DAO;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/BankExchange")]
    public class BankExchangeController : BaseApiController
    {
        private int RequestType = 2;
        private int PartnerID = 1;

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

                var banks = BankExchangesApi.GetBanks();
                var objReturn = banks.WithdrawBankList;
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
                
                if (!firstBanks.ExchangeStatus.HasValue||(firstBanks.ExchangeStatus.HasValue&& !firstBanks.ExchangeStatus.Value))
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

        /// <summary>
        /// Tạo danh danh sách  PendingOrder(chờ duyệt)
        /// </summary>
        /// <returns></returns>
        [ActionName("CreateOrders")]
        [HttpPost]
        public dynamic CreateOrders([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
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
                //3 Lấy thông tin amount nhập vào (tiền Q)
                string Amount = input.Amount ?? string.Empty;//Tiền này là tiền Q
                var lngGameAmount = ConvertUtil.ToLong(Amount);
                if (lngGameAmount <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Invalid withdrawal value",
                    };
                }
                //Lấy thông tin BankName (đã chọn )
                string BankName = input.BankName ?? string.Empty;
                if (String.IsNullOrEmpty(BankName))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "You have not selected a bank",
                    };
                }

                //lấy thông tin masterbankaccount
                string BankAccount = input.BankAccount ?? string.Empty;
                if (String.IsNullOrEmpty(BankAccount))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "No account number entered",
                    };
                }
                string BankAccountName = input.BankAccountName ?? string.Empty;
                if (String.IsNullOrEmpty(BankAccountName))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "The account owner has not been entered",
                    };
                }


                string minValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_WITHDRAW_LOWER_LIMIT", out minValue);
                var min = ConvertUtil.ToLong(minValue);
                string maxValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_WITHDRAW_UPPER_LIMIT", out maxValue);
                var max = ConvertUtil.ToLong(maxValue);

                if(lngGameAmount<min|| lngGameAmount> max)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Withdrawal amount exceeds the allowable limit",
                    };
                }



                

                var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (account == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (account.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                if (account.Balance < lngGameAmount)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "The amount is not enough to make the transaction"
                    };
                }
                int limitResponse = 0;
                USDTDAO.Instance.UserCheckLimit(accountId, lngGameAmount, out limitResponse);
                if (limitResponse != 1)
                {
                    if (limitResponse == -202 || limitResponse == -223)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.AccountLock
                        };
                    }
                    else if (limitResponse == -227)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.TranferOverMoney
                        };
                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.InProccessException
                        };
                    }
                }

                string Otp = input.Otp ?? string.Empty;
                if (String.IsNullOrEmpty(Otp))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpEmpty
                    };
                }

                Otp = Otp.Trim();
                if (Otp.Length != OTPAPP_LENGTH && Otp.Length != OTPSMS_LENGTH && Otp.Length != OTPSAFE_LENGTH)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpLengthInValid
                    };
                }

                //logic otp
                int resOtp = 0;
                long otpID;
                string otmsg;
                SMSDAO.Instance.ValidOtp(account.AccountID, Otp.Length == OTPSAFE_LENGTH ? account.PhoneSafeNo : account.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = Otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
               // kiểm tra đủ điều kiện giao dịch
                int outUpdatePhone;
                AccountDAO.Instance.UserCheckUpdatePhone(accountId, out outUpdatePhone);
                if (outUpdatePhone != 1)
                {
                    return new
                    {
                        ResponseCode = -232,
                        Message = ErrorMsg.ERR_UPDATE_PHONE_LESS_24
                    };
                }
                //2.Lấy thông tin Rate
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
                if (!firstBanks.ExchangeStatus.HasValue || !firstBanks.ExchangeStatus.Value)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "The system pauses this function. Please come back later",
                    };
                }
                rate = firstBanks.ExchangeRate.Value;
                if (rate <= 0)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Can't find rate configuration",
                    };
                }

                var AmountVNDReceived = ConvertUtil.ToLong(lngGameAmount *rate);//Tiền game



                int Response;
                long RemainBalance;
                long RequestID;
                long TransID;
              
               
                //Khởi tạo order trong dbs 
                string description = string.Empty;
                USDTDAO.Instance.UserBankRequestCreate(RequestType, accountId, lngGameAmount , 0, AmountVNDReceived, PENDING_STATUS, PartnerID
                    , ServiceID, description, BankAccountName, BankAccount, BankName, rate, out Response, out RemainBalance, out RequestID, out TransID);
                if (Response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = "We have received your request. Please wait for admin approval"
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = String.Format(ErrorMsg.InProccessException + "|" + Response)
                    };
                }


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
