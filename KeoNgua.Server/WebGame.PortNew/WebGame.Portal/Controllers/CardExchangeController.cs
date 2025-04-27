using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TraditionGame.Utilities.IpAddress;

using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using WebGame.Payment.Database.DAO;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Helpers.Chargings.Cards;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using MsWebGame.Portal.Database;
using TraditionGame.Utilities.OneSignal;
using TraditionGame.Utilities.DNA;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/CardExchange")]
    public class CardExchangeController : BaseApiController
    {

        /// <summary>
        /// hàm đổi thẻ
        /// </summary>
        /// <param name="input">CardCode</param>
        /// <returns></returns>
        [ActionName("Cashout")]
        [HttpPost]
        public dynamic Cashout(dynamic input)
        {
            try
            {
                string APPROVE = ConfigurationManager.AppSettings["CARD_APPROVED"].ToString();
                if (APPROVE != "1")
                {
                    return AnphaHelper.Close();
                }
                //lock function LOCKFUNCTION lockahap
                //return AnphaHelper.Close();
                //ENDLOCKFUNCTION

                long accountId = AccountSession.AccountID;
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

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (String.IsNullOrEmpty(user.PhoneNumber)&&String.IsNullOrEmpty(user.PhoneSafeNo))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneEmpty
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
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
                string CardCode = input.CardCode ?? string.Empty;//lấy ra CardCode
                string Otp = input.Otp ?? String.Empty;
                //validate opt 

                if ((String.IsNullOrEmpty(CardCode)))
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardCodeRequired
                    };
                }
                if ((String.IsNullOrEmpty(Otp)))
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

                int resOtp = 0;
                long otpID;
                string otmsg;
                SMSDAO.Instance.ValidOtp(accountId, Otp.Length==OTPSAFE_LENGTH?user.PhoneSafeNo: user.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = Otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }


                var card = CardDAO.Instance.GetCardSWapList(CardCode,ServiceID).FirstOrDefault();
                if (card == null)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardLock
                    };
                }
                if (!card.ExchangeStatus)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardLock
                    };
                }
                var Telecom = CardDAO.Instance.GetTeleCom(card.OperatorCode,ServiceID).FirstOrDefault();
                if (Telecom == null)
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };

                }
                if (!Telecom.ExchangeStatus)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.CardTelecomInActive, Telecom.OperatorName)
                    };
                }

                var amount = card.CardValue;
                var lostAmount = amount * Telecom.ExchangeRate;
                if (user.Balance < lostAmount)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotEnoghToCardExchange
                    };
                }



                int Response;
                long Balance;
                int Status;
                CardDAO.Instance.UserCardCashout(accountId, amount, card.OperatorCode, 1,ServiceID, out Status, out Response, out Balance);
                if (Response == 1 && (Status == 1 || Status == 2))
                {
                    if (!String.IsNullOrEmpty(user.PhoneSafeNo))
                    {
                        int outResponse;
                        long msgID;
                        string msg = String.Format("You just changed the tag {0} at time {1}", amount.IntToMoneyFormat(), DateTime.Now);
                        if (user != null)
                        {
                            if (!String.IsNullOrEmpty(user.PhoneSafeNo))
                            {
                                SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, user.PhoneSafeNo, msg, out outResponse, out msgID);
                            }
                                
                            if (!String.IsNullOrEmpty(user.SignalID)&&!String.IsNullOrEmpty(user.PhoneSafeNo))
                            {
                                OneSignalApi.SendByPlayerID(new List<string>() { user.SignalID }, msg);
                            }
                        }
                        

                    }
                    var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                    dnaHelper.SendTransactionSALE(accountId, GetCardCodeIndex(card.OperatorCode), displayName, amount, ConvertUtil.ToLong(lostAmount));

                    return new
                    {
                        ResponseCode = 1,
                        Message = Status == 1 ? ErrorMsg.CardCashoutSuccess : ErrorMsg.CardNeedApproved,
                        Balance = Balance
                    };
                }
                else if (Response == -504)
                {
                    return new
                    {
                        ResponseCode = -3,
                        Message = ErrorMsg.BalanceNotEnough
                    };
                }
                else if (Response == -6)
                {
                    return new
                    {
                        ResponseCode = -6,
                        Message = ErrorMsg.CardLack
                    };
                }
                else if (Response == -514)
                {
                    return new
                    {
                        ResponseCode = -6,
                        Message = ErrorMsg.CardLack
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    };
                }


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }

        [ActionName("GetListCard")]
        [HttpGet]
        public dynamic GetListCard()
        {
            try
            {
                long accountId = AccountSession.AccountID;
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

                var list = CardDAO.Instance.GetCardSWapList(null,ServiceID);

                return new
                {
                    ResponseCode = 1,
                    list = list,


                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }



        [ActionName("GetHistory")]
        [HttpGet]
        public dynamic GetHistory()
        {
            long accountId = AccountSession.AccountID;

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

            try
            {
                int totalRecord;
                var list = UserCardSwapDAO.Instance.GetList_New(accountId, null, null, null, -1, 1, 50, out totalRecord);
                return new
                {
                    ResponseCode = 1,
                    List = list ?? new List<UserCardSwap_New>()
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    List = new List<UserCardSwap_New>()
                };
            }
        }



        [ActionName("CancelCard")]
        [HttpPost]
        public dynamic CancelCard(dynamic input)
        {
            string APPROVE = ConfigurationManager.AppSettings["CARDSWAPCANCEL_APPROVED"].ToString();
            if (APPROVE != "1")
            {
                return AnphaHelper.Close();
            }
            string CardID = input.UserCardSwapID ?? string.Empty;//lấy ra CardCode
            if (String.IsNullOrEmpty(CardID))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }
            var lngCardID = ConvertUtil.ToLong(CardID);
            if (lngCardID < 0)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }
            long accountId = AccountSession.AccountID;
            var displayName = AccountSession.AccountName;
            if (accountId <= 0 || String.IsNullOrEmpty(displayName))
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }

            var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
            if (user == null)
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
           
            if (user.Status != 1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountLock
                };
            }
            
            var card = UserCardSwapDAO.Instance.UserCardSwapGetByID(lngCardID);
            if (card == null)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapNotFound
                };
            }
            if (card.Status!=2)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapStatusInValid
                };
            }
            if (card.AccountID != accountId)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserInValid
                };
            }
            long RemainBalance = 0;
            int Response = 0;
            UserCardSwapDAO.Instance.UserCardSwapCancel(accountId, lngCardID, out RemainBalance, out Response);
            if (Response == 1)
            {
                return new
                {
                    ResponseCode = 1,
                    Message = ErrorMsg.CardSwapCancelSuccess,
                    Balance= RemainBalance
                };
            }else if(Response==-35)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapNotFound
                };
            }
            else if (Response == -36)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserInValid
                };
            }
            else if (Response == -1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapStatusInValid
                };
            }
             else if(Response==-504)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AmountNotValid
                };
            }else
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserCardSwapCancelFail,
                };
            }
        }

        private int  GetCardCodeIndex(String cardOperatorCode)
        {
            try
            {
                if (cardOperatorCode.Contains("VTT")) return 1;
                else if (cardOperatorCode.Contains("VNP")) return 2;
                else return 3;
            }catch(Exception ex)
            {
                return 0;
            }
           
        }
    }
}