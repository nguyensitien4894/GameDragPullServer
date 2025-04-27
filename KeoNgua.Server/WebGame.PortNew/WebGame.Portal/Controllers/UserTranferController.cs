using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using WebGame.Payment.Database.DAO;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Helpers;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Api;
using TraditionGame.Utilities.Constants;
using MsWebGame.Portal.Database;
using MsWebGame.Portal.Models;
using TraditionGame.Utilities.OneSignal;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Log;
using MsWebGame.RedisCache.Cache;
using Newtonsoft.Json;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/UserTranfer")]
    public class UserTranferController : BaseApiController
    {
        private static readonly string _ChangeBalanceUrl = System.Configuration.ConfigurationManager.AppSettings["CHANGE_BALANCE_URL"].ToString();
        private static readonly string _ApiChangeBalance = System.Configuration.ConfigurationManager.AppSettings["API_CHANGE_BALANCE"].ToString();

        string GetSTk(string content)
        {
            string[] numbers = Regex.Split(content, @"\D+");
            Array.Sort(numbers, (x1, x2) => x1.Length.CompareTo(x2.Length));
            return numbers.Last();
        }

        [ActionName("TranferBit")]
        [HttpPost]
        public dynamic TranferBit([FromBody] dynamic input)
        {
            try
            {
                string APPROVE = ConfigurationManager.AppSettings["UserTranfer_APPROVED"].ToString();
                if (APPROVE != "1")
                {
                    return AnphaHelper.Close();
                }

                long accountId = AccountSession.AccountID;
                //kiểm tra authen 
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

                //kiểm tra tham số
                string Amount = input.Amount ?? string.Empty;
                string Otp = input.Otp ?? string.Empty;
                string note = input.Note ?? string.Empty;
                string NickName = input.NickName ?? string.Empty;


                //kiểm tra user name
                if ((String.IsNullOrEmpty(NickName)))
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserNameTranferInvalid
                    };
                }
                //kiểm tra amount
                if ((String.IsNullOrEmpty(Amount)))
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountInValid
                    };
                }
                var lngAmount = ConvertUtil.ToLong(Amount);
                if (lngAmount < 10000)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.MinAmountTranfer, "10.000"),
                    };
                }

                bool Live = false; 
                try
                {
                    string keyHu = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + accountId);
                   
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();
                    if (_cachePvd.Exists(keyHu))
                    {
                        Live = true;
                        NLogManager.LogError("keyHu:" + keyHu);
                        ParConfigLive configLive = _cachePvd.Get<ParConfigLive>(keyHu);
                        if (configLive != null && configLive.Tranfer.Count > 0 && !configLive.Tranfer.Contains(Amount))
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = String.Format(ErrorMsg.BetweenAmountTranfer, string.Join(" G ,", configLive.Tranfer)+" G"),
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ex.ToString(),
                    };
                }
                
                //note nếu có thì lỗi
                if (!String.IsNullOrEmpty(note) && note.Length > 100)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.NoteLengthOverMax
                    };
                }
                // kiểm tra otp
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
                NickName = NickName.Trim();

                //logic user name 
                var userReceive = new UserAgency();
                userReceive.IsBot = false;
                int isBalackList = 0;
                AccountDAO.Instance.UserCheckBlackList(accountId, out isBalackList);
                //khong nằm trong black list
                if (isBalackList != 1)
                {
                    userReceive = UserDAO.Instance.GetUserByNickName(NickName, ServiceID);
                    if (userReceive == null)
                    {
                        userReceive = UserDAO.Instance.GetBotByNickName(NickName, ServiceID);
                        if (userReceive == null)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.AccountReceiveNotExist
                            };
                        }
                        userReceive.IsBot = true;


                    }
                }
                else
                {
                    //nằm trong blacklist
                    if (lngAmount >= 200000)
                    {
                        //giữ lại số tiền nghi ngờ đại lý
                        string DAILY = ConfigurationManager.AppSettings["NPH_DL"].ToString();
                        userReceive = UserDAO.Instance.GetUserByNickName(DAILY, ServiceID);
                        note = String.Format("{0}-NPH temporarily hold money due to suspicious transactions", note);
                    }else
                    {
                        userReceive = UserDAO.Instance.GetUserByNickName(NickName, ServiceID);
                    }
                   
                }

                if (accountId == userReceive.AccountID)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserTranferEqualUser
                    };
                }
                string keyHuUserReceiveLive = CachingHandler.Instance.GeneralRedisKey("UserReceiveLive", DateTime.Now.ToString("yyyy-MM-dd")+":"+userReceive.AccountName);
                RedisCacheProvider _cachePvdUserReceiveLive = new RedisCacheProvider();
                int countReceive = 0;
                if (Live)
                {
                    if (userReceive.AccountType != 2)
                    {
                        if (_cachePvdUserReceiveLive.Exists(keyHuUserReceiveLive))
                        {
                            countReceive = _cachePvdUserReceiveLive.Get<int>(keyHuUserReceiveLive);
                            if (countReceive > 0)
                            {
                                return new
                                {
                                    ResponseCode = 200,
                                    Message =""
                                };
                            }
                        }
                    }
                }
               
                //check limit tranfer
                //int outUpdatePhone;
                //AccountDAO.Instance.UserCheckUpdatePhone(accountId, out outUpdatePhone);
                //if (outUpdatePhone != 1)
                //{
                //    return new
                //    {
                //        ResponseCode = -232,
                //        Message = ErrorMsg.ERR_UPDATE_PHONE_LESS_24
                //    };
                //}


                //logic amount 

                double fee = 0;
                //lấy phí của đại lý
                long agencyId = 0;
                short agencyLevel = 0;
                if (userReceive.AccountType == 2)//đại lý
                {
                    if (lngAmount < 200000)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = String.Format(ErrorMsg.MinAmountTranfer, "200.000"),
                        };
                    }

                    agencyId = userReceive.AccountID;
                    agencyLevel = (short)userReceive.AccountLevel;

                    string strFee;
                    if (agencyLevel == 2)
                    {
                        ParaConfigDAO.Instance.GetConfigValue("TRANSFEE", "USER_TO_AGENCY_LEVEL2", out strFee);
                    }
                    else
                    {
                        ParaConfigDAO.Instance.GetConfigValue("TRANSFEE", "USER_TO_AGENCY_LEVEL1", out strFee);
                    }

                    fee = Convert.ToDouble(strFee);
                }
                else
                {
                    //user thường
                    string strFee;
                    ParaConfigDAO.Instance.GetCoreConfig("TRANSFEE", "TRANSFEE", out strFee);
                    fee = Convert.ToDouble(strFee);

                }
                //chưa cấu hình phí giao dịch
                if (fee < 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.FeeNotConfig
                    };
                }
                //kiểm tra có đủ phí để chuyển Bit hay không
                var balance = account.Balance;
                //long lostAmount = lngAmount + (long)Math.Round(lngAmount * fee);
                if (balance < lngAmount)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValidToTranferBit
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
                NLogManager.LogMessage(String.Format("StartTranfer: from {0} to {1} at {2} type {3}", account.AccountName, userReceive.NickName, DateTime.Now, userReceive.AccountType));
                if (userReceive.AccountType != 2)//người dùng thường
                {
                    //  string note = string.Format("{0} chuyển {1}  cho {2} vào ngày {3}", account.AccountName, lostAmount.LongToMoneyFormat(), userReceive.UserName, DateTime.UtcNow);
                    long TransID = 0;
                    long Wallet = 0;
                    int Response = 0;
                    NLogManager.LogMessage(String.Format("Add money to user :  {0} amount {1} at {2}", userReceive.NickName, lngAmount, DateTime.Now));
                    if (userReceive.IsBot)
                    {
                        UserTranferDAO.Instance.UserTransferToBot(accountId, userReceive.AccountID, lngAmount, ServiceID, note, out TransID, out Wallet, out Response);
                    }
                    else
                    {
                        UserTranferDAO.Instance.UserTransferToUser(accountId, userReceive.AccountID, lngAmount, note, ServiceID, out TransID, out Wallet, out Response);
                    }

                    NLogManager.LogMessage(String.Format(" EndTranfer :  {0} amount {1} at  Response {2} ", userReceive.NickName, lngAmount, Response));
                    if (Response == 1)
                    {
                        if (!String.IsNullOrEmpty(account.PhoneSafeNo))
                        {
                            int outResponse;
                            long msgID;
                            string msg = String.Format("You have just transferred {0} to account {1} at time {2}", lngAmount.LongToMoneyFormat(), userReceive.NickName, DateTime.Now);
                                 
                            if (account != null)
                            {
                                if (!String.IsNullOrEmpty(account.PhoneSafeNo))
                                {
                                    SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, account.PhoneSafeNo, msg, out outResponse, out msgID);
                                }
                                if (!String.IsNullOrEmpty(account.SignalID) && !String.IsNullOrEmpty(account.PhoneSafeNo))
                                {
                                    OneSignalApi.SendByPlayerID(new List<string>() { account.SignalID }, msg);
                                }
                                 
                               
                            }

                        }
                        string msgTele = String.Format("You have just transferred {0} to account {1} at time {2}, Số dư tài khoản : {3}", lngAmount.LongToMoneyFormat(), userReceive.NickName, DateTime.Now, Wallet.LongToMoneyFormat());

                        SendTelePushTelegramID(3, msgTele, account.TelegramID, false, account.AccountName);


                        if (Live)
                        {
                            countReceive++;
                            _cachePvdUserReceiveLive.SetSecond(keyHuUserReceiveLive, countReceive , 60*60*24 );
                        }

                        return new
                        {
                            ResponseCode = 1,
                            Balance = Wallet,
                            Message = ErrorMsg.TranferBitSuccess
                        };
                    }
                    else if (Response == -10)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.ParamaterInvalid
                        };
                    }
                    else if (Response == -105)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.AccountReceiveNotExist
                        };
                    }
                    else if (Response == -507)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.InProccessException
                        };
                    }
                    else if (Response == -504)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.AmountNotValidToTranferBit
                        };
                    }
                    else if (Response == -501)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = String.Format(ErrorMsg.MinAmountTranfer, (10000).IntToMoneyFormat())
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
                else
                {
                    //nghiệp vụ chuyển tiền cho đại lý 
                    int limitResponse = 0;
                    UserTranferDAO.Instance.UserCheckLimit(accountId, lngAmount, out limitResponse);
                    string stk = GetSTk(note);
                    if (!string.IsNullOrEmpty(stk))
                    {
                        int bankbanresponse = 0;
                        UserTranferDAO.Instance.AccountCheckBank(stk, out bankbanresponse);
                        int status = 2;
                        int res = 0;
                        int res2 = 0;
                        if (bankbanresponse == 1)
                        {
                            res = UserDAO.Instance.UserUpdateStatus(accountId, status);
                            UserDAO.Instance.UserComplainCreate(accountId, 3, 1, "Bank account number in dirty bank", "Bank account number in dirty bank", true, 1, out res2);
                            NLogManager.LogMessage(string.Format("UserTransferToAgencyAdmin- User key {0} has a dirty bank account number {1}", accountId, note));
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.AccountLock
                            };
                        }
                    }
                    
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

                    long RemainWallet = 0;
                    int Response = 0;
                    long TransID = 0;
                    long OrgTransID = 0;
                    long walletRecive = 0;
                    NLogManager.LogMessage(String.Format(" UserTransferToAgencyAdmin:  {0} amount {1} at {2}", userReceive.NickName, lngAmount, DateTime.Now));
                    UserTranferDAO.Instance.UserTransferToAgencyAdmin(accountId, agencyId, userReceive.NickName, 2, agencyLevel, lngAmount, note, ServiceID, out OrgTransID, out RemainWallet, out Response,out walletRecive);
                    NLogManager.LogMessage(String.Format(" End UserTransferToAgencyAdmin :  {0} amount {1} at  Response {2} ", userReceive.NickName, lngAmount, Response));
                    if (Response == 1)
                    {
                        Response = 0;

                        NLogManager.LogMessage(String.Format(" StartUserTransfer:  {0} amount {1} at {2}", userReceive.NickName, lngAmount, DateTime.Now));
                        // UserTranferDAO.Instance.UserTransfer(accountId, agencyId, 2, lngAmount, note, OrgTransID, out TransID, out RemainWalletAgency, out Response);
                        NLogManager.LogMessage(String.Format(" End UserTransfer :  {0} amount {1} at  Response {2} ", userReceive.NickName, lngAmount, Response));
                        NLogManager.LogMessage(String.Format(" Gui tele : tele  {0}| name {1}|  note {2} ", userReceive.TelegramID.HasValue && userReceive.TelegramID.Value > 0 ? userReceive.TelegramID.Value : 0, userReceive.NickName,  note));
                        //send telegram id
                        int outResponse;
                        long msgID;
                        try
                        {
                            var strNotifty = String.Format("You just received {0} from nickname {1}. Current balance is {2}", lngAmount.LongToMoneyFormat(), account.AccountName, walletRecive.LongToMoneyFormat());
                            if (userReceive.TelegramID.HasValue && userReceive.TelegramID.Value > 0)
                            {
                                GunNotifyChangeBalance(ServiceID, userReceive.TelegramID.Value, strNotifty);
                            }
                            if (userReceive != null)
                            {
                                if (!String.IsNullOrEmpty(userReceive.PhoneSafeNo))
                                {
                                    SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, userReceive.PhoneSafeNo, strNotifty, out outResponse, out msgID);
                                }
                                if (!String.IsNullOrEmpty(account.SignalID) && !String.IsNullOrEmpty(account.PhoneSafeNo))
                                {
                                    OneSignalApi.SendByPlayerID(new List<string>() { userReceive.SignalID }, strNotifty);
                                }
                                
                                SendTelePushTelegramID(4,strNotifty +"\n"+ note, userReceive.TelegramID.HasValue && userReceive.TelegramID.Value > 0 ? userReceive.TelegramID.Value : 0, true, userReceive.NickName);

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        if (!String.IsNullOrEmpty(account.PhoneSafeNo))
                        {

                            string msg = String.Format("You have just transferred {0} to account {1} at time {2}", lngAmount.LongToMoneyFormat(), userReceive.NickName, DateTime.Now);
                            if (account != null)
                            {
                                if (!String.IsNullOrEmpty(account.PhoneSafeNo))
                                {
                                    SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, account.PhoneSafeNo, msg, out outResponse, out msgID);
                                }
                                if (!String.IsNullOrEmpty(account.SignalID) && !String.IsNullOrEmpty(account.PhoneSafeNo))
                                {
                                    OneSignalApi.SendByPlayerID(new List<string>() { account.SignalID }, msg);
                                }
                            }

                        }

                        try
                        {
                            var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);

                            dnaHelper.SendTransactionSALE(accountId, 4, displayName, lngAmount, lngAmount);
                            ProfileLogger.LogMessage(String.Format("accountId:{0}|displayName:{1}|lngAmount:{2}|OrgTransID:{3}", accountId, displayName, lngAmount, OrgTransID));
                        }
                        catch (Exception ex)
                        {
                            ProfileLogger.LogMessage("ERR" + ex.Message);
                        }

                        return new
                        {
                            ResponseCode = 1,
                            Balance = RemainWallet,
                            Message = ErrorMsg.TranferBitSuccess
                        };


                    }
                    else
                    {
                        if (Response == -10)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.ParamaterInvalid
                            };
                        }
                        else if (Response == -105)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.AccountReceiveNotExist
                            };
                        }
                        else if (Response == -507)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.InProccessException
                            };
                        }
                        else if (Response == -504)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.AmountNotValidToTranferBit
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

                    //kết thúc nghiệp vụ chuyển tiề cho đại lý 


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

        [ActionName("GetHistory")]
        [HttpGet]
        public dynamic GetHistory(int type)
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

            try
            {
                int totalRecord;
                var list = UserTranferDAO.Instance.BalanceLogsList(accountId, null, type, null, null, ServiceID, 1, 100, out totalRecord);
                var result = new List<BalanceLogs>();
                if (list != null && list.Any())
                {
                    foreach (var item in list)
                    {
                        if (item!=null&&!String.IsNullOrEmpty(item.PartnerName)&&item.PartnerName.ToLower() != "daily-nph")
                        {
                            item.Description = item.Description.CusSubString(20);
                            result.Add(item);
                            continue;
                        }
                        if (item != null && String.IsNullOrEmpty(item.PartnerName))
                        {
                            item.Description = item.Description.CusSubString(20);
                            result.Add(item);
                            continue;
                        }

                    }

                }
                return new
                {
                    ResponseCode = 1,
                    List = result
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    List = new List<BalanceLogs>()
                };
            }
        }


        private void GunNotifyChangeBalance(int ServiceID, long tele_id, string content)
        {
            try
            {
                string gateName = string.Empty;
                if (ServiceID == 1)
                {
                    gateName = "[TQ]";
                }
                else if (ServiceID == 2)
                {
                    gateName = "[BB]";
                }
                if (!String.IsNullOrEmpty(content))
                {
                    content = String.Format("{0}{1}", gateName, content);
                }
                //var api = new ApiUtil<dynamic>();
                //api.ApiAddress = _ChangeBalanceUrl;
                //api.URI = _ApiChangeBalance;
                //var result = api.Send(new { ChatID = tele_id, Content = content });
                TeleNotify(tele_id.ToString(), content);
                //NLogManager.LogMessage(string.Format("GunNotifyChangeBalance-Res:{0}", result.ResponseCode));
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

    }
}
