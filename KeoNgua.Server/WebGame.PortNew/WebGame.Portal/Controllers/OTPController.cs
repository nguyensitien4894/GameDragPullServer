using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Facebook;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Security;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using WebGame.Header.Utils._1Pays.SMSs;
using WebGame.Payment.Database.DAO;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Helpers.OTPs.MobileSMS;
using MsWebGame.Portal.Models.OTPs;
using TraditionGame.Utilities;
using System.Collections.Generic;
using TraditionGame.Utilities.OneSignal;
using MsWebGame.Portal.Database;
using Microsoft.Ajax.Utilities;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/OTP")]
    public class OTPController : BaseApiController
    {

        /// <summary>
        /// lấy otp khi login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOTP")]
        public dynamic GetOTP([FromBody] dynamic input)
        {
          

            string phonenumber = input.PhoneNumber;

            var accountId = AccountSession.AccountID;
            var displayName = AccountSession.AccountName;
            string type = input.Type ?? "1";
            //return new
            //{
            //    ResponseCode = -100,
            //    Message = "Mã Otp là "+ type
            //};
            if (type != "1" && type != "10" && type != "20")
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }

            if (accountId <= 0 || String.IsNullOrEmpty(displayName))
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            var accountInfo = AccountDAO.Instance.GetAccountInfo(accountId, null, null, ServiceID);
            if (accountInfo == null)
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            if (!String.IsNullOrEmpty(phonenumber))
            {
                if (!ValidateInput.ValidatePhoneNumber(phonenumber))
                {
                    return new
                    {
                        ResponseCode = -3,
                        Message = ErrorMsg.PhoneIncorrect
                    };
                }
                //if (IsVieNamMobilePhone(phonenumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.PhoneVietnammobileNotSupport
                //    };
                //}
            }
            if (type == "1")
            {
                if (!String.IsNullOrEmpty(phonenumber))
                {
                    //validate phone number
                    //nếu số điện thoại đã  sử dụng thì ko validate được phone number
                    int IsUsed = AccountDAO.Instance.CheckAccountPhone(accountId, phonenumber.PhoneFormat(), ServiceID);
                    if (IsUsed != 1)
                    {
                        return new
                        {
                            ResponseCode = -3,
                            Message = ErrorMsg.PhoneInUse
                        };
                    }
                }
                var currentphone = accountInfo.PhoneNumber;
                //số điện thoại chưa được đăng ký để lấy mã xác nhận
                if (String.IsNullOrEmpty(currentphone) && String.IsNullOrEmpty(phonenumber))
                {
                    return new
                    {
                        ResponseCode = -3,
                        Message = ErrorMsg.PhoneNotRegister
                    };
                }
                //nếu ko lấy được current phone number và số phone đăng ký chưa được xử dụng
                if (String.IsNullOrEmpty(currentphone) && !String.IsNullOrEmpty(phonenumber))
                {
                    currentphone = phonenumber;
                }
                var newPhone = currentphone.PhoneDisplayFormat();
                // var result = GenerateOtp(accountId, currentphone, USER_TYPE.ToString());
                string outotp;
                var result = GenerateOtpWithTeleSafe(accountId, currentphone, type, out outotp);
                return result;
            }else  if (type == "20")
            {
                if (!String.IsNullOrEmpty(phonenumber))
                {
                    //validate phone number
                    //nếu số điện thoại đã  sử dụng thì ko validate được phone number
                    int IsUsed = AccountDAO.Instance.CheckAccountPhone(accountId, phonenumber.PhoneFormat(), ServiceID);
                    if (IsUsed != 1)
                    {
                        return new
                        {
                            ResponseCode = -3,
                            Message = ErrorMsg.PhoneInUse,
                            Type = type
                        };
                    }
                }
                var currentphone = accountInfo.PhoneNumber;
                //số điện thoại chưa được đăng ký để lấy mã xác nhận
                if (String.IsNullOrEmpty(currentphone) && String.IsNullOrEmpty(phonenumber))
                {
                    return new
                    {
                        ResponseCode = -3,
                        Message = ErrorMsg.PhoneNotRegister
                    };
                }
                long TelegramID = accountInfo.TelegramID;


                //số điện thoại chưa được đăng ký để lấy mã xác nhận
                //"Số điện thoại chưa liên kết Tele "
                if (TelegramID < 1)
                {
                    return new
                    {
                        ResponseCode = -4,
                        Message = "Phone number not linked Telegram",
                        Type = type
                    };
                }
        
                //nếu ko lấy được current phone number và số phone đăng ký chưa được xử dụng
                if (String.IsNullOrEmpty(currentphone) && !String.IsNullOrEmpty(phonenumber))
                {
                    currentphone = phonenumber;
                }
                var newPhone = currentphone.PhoneFormat();
                OtpResponse result = SendTeleOtp(newPhone, TelegramID);
                // "Otp đã đươc gửi vào Telegram của bạn! "
                return new
                {
                    ResponseCode = 1,
                    Message = "Otp has been sent to your Telegram!"  
                };
            }
            else
            {
                if (!String.IsNullOrEmpty(phonenumber))
                {
                    //validate phone number
                    //nếu số điện thoại đã  sử dụng thì ko validate được phone number
                    int IsUsed = AccountDAO.Instance.AccountCheckSafePhone(accountId, phonenumber.PhoneFormat(), ServiceID);
                    if (IsUsed != 1)
                    {
                        return new
                        {
                            ResponseCode = -3,
                            Message = ErrorMsg.PhoneSafeOtpInUse
                        };
                    }

                }
                var currentphone = phonenumber.PhoneFormat();
                if (String.IsNullOrEmpty(phonenumber))
                {
                    currentphone = accountInfo.PhoneSafeNo;
                }
                string outotp;
                var result = GenerateOtpWithTeleSafe(accountId, currentphone, type, out outotp);
                if (type == "10" && result != null && result.ResponseCode == 1 && accountInfo != null && !String.IsNullOrEmpty(accountInfo.SignalID) && !String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
                {

                    OneSignalApi.SendByPlayerID(new List<string>() { accountInfo.SignalID }, String.Format(ErrorMsg.SafeNotiyType10, outotp));
                }
                return result;
            }




        }
        /// <summary>
        /// lấy otp khi chưa login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetLoginOtp")]
        public dynamic GetLoginOtp(dynamic input)
        {
            //return new
            //{
            //    ResponseCode = -100,
            //    Message = "Mã Otp là 1234567"
            //};
            //para
            string username = input.UserName;
            username = username != null ? username.ToLower() : string.Empty;
            string password = input.Password;
            int loginType = input.LoginType ?? -1;
            int response = -99;
            int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;
            string msg;
            int resCode;
            //validate para
            //if (!ValidateInput.IsValidated(username, password, out msg, out resCode))
            //{
            //    return new
            //    {
            //        ResponseCode = resCode,
            //        Message = msg
            //    };
            //}
            string otp = input.Otp ?? String.Empty;
            string type = input.Type ?? "1";

            if (type != "1" && type != "10" && type != "20")
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }

            switch (loginType)
            {
                case (int)Constants.enmAuthenType.AUTHEN_ID://normal login
                    var loginAccount = AccountDAO.Instance.UserLogin(username, Security.SHA256Encrypt(password), loginType, deviceType, ServiceID, out response);
                    if (response == 1)
                    {//login success
                        //phone validate
                        if (type == "1" && String.IsNullOrEmpty(loginAccount.PhoneNumber))
                        {
                            return new
                            {
                                ResponseCode = -100,
                                Message = ErrorMsg.PhoneEmpty
                            };
                        }
                        if (type == "10" && String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
                        {
                            return new
                            {
                                ResponseCode = -100,
                                Message = ErrorMsg.PhoneSafeOtpEmty
                            };
                        }

                        if (type == "20" &&  loginAccount.TelegramID <= 0)
                        {
                            return new
                            {
                                ResponseCode = -4,
                                Message = "Phone number not linked Telegram ",
                                Type = type
                            };
                        }
                        //else if (loginAccount.AuthenType != 1)
                        //{//authen type valdate
                        //    return new
                        //    {
                        //        ResponseCode = -101,
                        //        Message = ErrorMsg.AcountNotConfigOtp
                        //    };
                        //}
                        //send otp
                        // var result = GenerateOtp(loginAccount.AccountID, loginAccount.PhoneNumber, USER_TYPE.ToString());
                        string outotp;
                        if (type == "20")
                        {
                            var newPhone = loginAccount.PhoneNumber.PhoneFormat();
                            var TelegramID = loginAccount.TelegramID;


                            bool v = String.IsNullOrEmpty(TelegramID.ToString());
                            if (!v) {
                                OtpResponse result = SendTeleOtp(newPhone, TelegramID);
                            }
                            
                            else SendTeleOtp(newPhone);
                            return new
                            {
                                ResponseCode = 1
                            };
                        }
                        else
                        {
                            var result = GenerateOtpWithTeleSafe(loginAccount.AccountID, type == "1" ? loginAccount.PhoneNumber : loginAccount.PhoneSafeNo, type, out outotp);
                            if (type == "10" && result != null && result.ResponseCode == 1 && loginAccount != null && !String.IsNullOrEmpty(loginAccount.SignalID) && !String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
                            {

                                OneSignalApi.SendByPlayerID(new List<string>() { loginAccount.SignalID }, String.Format(ErrorMsg.SafeNotiyType10, outotp));
                            }
                            return result;
                        }
                       

                    }
                    else
                    {//login fail
                        return new
                        {
                            ResponseCode = -99,
                            Message = ErrorMsg.OtpBusy
                        };
                    }

                case (int)Constants.enmAuthenType.AUTHEN_FB:
                    string accessToken = input.AccessToken;
                    if (accessToken != null)
                    {
                        //get facebook info
                        FacebookUtil.FbUserInfo userFBInfo = FacebookUtil.GetFbUserInfo(accessToken);
                        if (userFBInfo.ResponeCode < 0)
                        {
                            return new
                            {
                                ResponseCode = -103,
                                Message = ErrorMsg.FacebookGetFail
                            };
                        }
                        //facebook id
                        string fbid = userFBInfo.UserId.ToString();
                        loginAccount = AccountDAO.Instance.UserLoginFB(fbid, string.Empty, loginType, deviceType, ServiceID, out response);
                        if (loginAccount != null)
                        {
                            //phone validate
                            if (type == "1" && String.IsNullOrEmpty(loginAccount.PhoneNumber))
                            {
                                return new
                                {
                                    ResponseCode = -100,
                                    Message = ErrorMsg.PhoneEmpty
                                };
                            }
                            if (type == "10" && String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
                            {
                                return new
                                {
                                    ResponseCode = -100,
                                    Message = ErrorMsg.PhoneSafeOtpEmty
                                };
                            }
                            if (type == "20" && loginAccount.TelegramID <= 0)
                            {
                                return new
                                {
                                    ResponseCode = -4,
                                    Message = "Phone number not linked Telegram",
                                    Type = type
                                };
                            }
                            //otp validate
                            else if (loginAccount.AuthenType != 1)
                            {
                                return new
                                {
                                    ResponseCode = -101,
                                    Message = ErrorMsg.AcountNotConfigOtp
                                };
                            }
                            //send otp
                            // var result = GenerateOtp(loginAccount.AccountID, loginAccount.PhoneNumber, USER_TYPE.ToString());
                            string outotp;
                            if (type == "20")
                            {
                                var newPhone = loginAccount.PhoneNumber.PhoneFormat();
                                SendTeleOtp(newPhone);
                                return new
                                {
                                    ResponseCode = 1
                                };
                            }
                            else
                            {
                                var result = GenerateOtpWithTeleSafe(loginAccount.AccountID, type == "1" ? loginAccount.PhoneNumber : loginAccount.PhoneSafeNo, type, out outotp);

                                if (type == "10" && result != null && result.ResponseCode == 1 && loginAccount != null && !String.IsNullOrEmpty(loginAccount.SignalID) && !String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
                                {

                                    OneSignalApi.SendByPlayerID(new List<string>() { loginAccount.SignalID }, String.Format(ErrorMsg.SafeNotiyType10, outotp));
                                }
                                return result;
                            }
                        }

                    }
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OtpBusy
                    };

            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.OtpBusy
            };


        }



        [HttpPost]
        [Route("GetTOTP")]
        public dynamic GetTOTP(dynamic input)
        {


            //para
            string username = input.UserName;
            username = username != null ? username.ToLower() : string.Empty;
            string password = input.Password;
            int loginType = input.LoginType ?? -1;
            int response = -99;
            int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;
            string msg;
            int resCode;
            //validate para
            //if (!ValidateInput.IsValidated(username, password, out msg, out resCode))
            //{
            //    return new
            //    {
            //        ResponseCode = resCode,
            //        Message = msg
            //    };
            //}
            string otp = input.Otp ?? String.Empty;
            switch (loginType)
            {
                case (int)Constants.enmAuthenType.AUTHEN_ID://normal login
                    var loginAccount = AccountDAO.Instance.UserLogin(username, Security.SHA256Encrypt(password), loginType, deviceType, ServiceID, out response);
                    if (response == 1 && loginAccount != null)
                    {//login success
                        //phone validate
                        if (String.IsNullOrEmpty(loginAccount.PhoneNumber))
                        {
                            return new
                            {
                                ResponseCode = -100,
                                Message = ErrorMsg.PhoneEmpty
                            };
                        }
                        //else if (loginAccount.AuthenType != 1)
                        //{//authen type valdate
                        //    return new
                        //    {
                        //        ResponseCode = -101,
                        //        Message = ErrorMsg.AcountNotConfigOtp
                        //    };
                        //}
                        //send otp
                        string outotp;
                        var agencyInfo = AgencyDAO.Instance.GetAgencyById(loginAccount.AccountID, ServiceID);
                        var type = agencyInfo != null ? AGENCY_TYPE.ToString() : USER_TYPE.ToString();
                        var result = GenerateOtpWithTeleSafe(loginAccount.AccountID, loginAccount.PhoneNumber, type, out outotp);

                        return result;

                    }
                    else
                    {//login fail
                        return new
                        {
                            ResponseCode = -99,
                            Message = ErrorMsg.OtpBusy
                        };
                    }

                case (int)Constants.enmAuthenType.AUTHEN_FB:
                    string accessToken = input.AccessToken;
                    if (accessToken != null)
                    {
                        //get facebook info
                        //FacebookUtil.FbUserInfo userFBInfo = FacebookUtil.GetFbUserInfo(accessToken);
                        //if (userFBInfo.ResponeCode < 0)
                        //{
                        //    return new
                        //    {
                        //        ResponseCode = -103,
                        //        Message = ErrorMsg.FacebookGetFail
                        //    };
                        //}
                        //facebook id
                        var list = FacebookUtil.GetIDsForBusiness(accessToken);
                        if (list == null || !list.Any())
                        {
                            return new
                            {
                                ResponseCode = -103,
                                Message = ErrorMsg.FacebookGetFail
                            };
                        }
                        string appID = ConfigurationManager.AppSettings["APP_FB_ID"].ToString().Trim();
                        var userFBInfo = list.FirstOrDefault(c => c.app.id == appID);
                        if (userFBInfo == null || userFBInfo.app == null)
                        {
                            return new
                            {
                                ResponseCode = -103,
                                Message = ErrorMsg.FacebookGetFail
                            };
                        }
                        string fbid = userFBInfo.id.ToString();//lấy facebook id
                        loginAccount = AccountDAO.Instance.UserLoginFB(fbid, string.Empty, loginType, deviceType, ServiceID, out response);
                        if (loginAccount != null)
                        {
                            //phone validate
                            if (String.IsNullOrEmpty(loginAccount.PhoneNumber))
                            {
                                return new
                                {
                                    ResponseCode = -100,
                                    Message = ErrorMsg.PhoneEmpty
                                };
                            }
                            //otp validate

                            //send otp
                            string outotp;
                            var agencyInfo = AgencyDAO.Instance.GetAgencyById(loginAccount.AccountID, ServiceID);
                            var type = agencyInfo != null ? AGENCY_TYPE.ToString() : USER_TYPE.ToString();
                            var result = GenerateOtpWithTeleSafe(loginAccount.AccountID, loginAccount.PhoneNumber, type, out outotp);

                            return result;
                        }

                    }
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OtpBusy
                    };

            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.OtpBusy
            };


        }


        /// <summary>
        /// get otp forgot pass word
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetOTPForgotPassword")]
        public dynamic GetOTPForgotPassword(dynamic input)
        {

            //para
            string username = input.UserName;
            username = username != null ? username.ToLower() : string.Empty;

            if (username.Length < 6 || username.Length > 18)
            {
                return new
                {
                    ResponseCode = -101,
                    Message = ErrorMsg.UserNameLength
                };

            }

            string phoneNumber = input.PhoneNumber ?? String.Empty;


            if (!ValidateInput.ValidatePhoneNumber(phoneNumber))
            {
                return new
                {
                    ResponseCode = -203,
                    Message = ErrorMsg.PhoneIncorrect
                };
            }
            //if (IsVieNamMobilePhone(phoneNumber))
            //{
            //    return new
            //    {
            //        ResponseCode = -1005,
            //        Message = ErrorMsg.PhoneVietnammobileNotSupport
            //    };
            //}
            string type = input.Type ?? "1";

            //if (type != "1" && type != "10" )
            //{
            //    return new
            //    {
            //        ResponseCode = -1005,
            //        Message = ErrorMsg.ParamaterInvalid
            //    };
            //}
            string newPhone = phoneNumber.Substring(1);
            var loginAccount = AccountDAO.Instance.GetAccountInfo(0, username, null, ServiceID);
            if (loginAccount == null)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserNotExist
                };

            }
            if (type == "1" && String.IsNullOrEmpty(loginAccount.PhoneNumber))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AcountNotConfigOtp
                };
            }
            if (type == "10" && String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountLinkedSafeOtp
                };
            }

            //if (loginAccount.PhoneNumber != newPhone)
            //{
            //    return new
            //    {
            //        ResponseCode = -1005,
            //        Message = ErrorMsg.PhoneRegisterDiffPhonePass
            //    };
            //}

            if ("85" + newPhone != loginAccount.PhoneNumber && "1" + newPhone == loginAccount.PhoneNumber && "0" == loginAccount.PhoneNumber)//check phone number with area/country code
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.PhoneRegisterDiffPhonePass
                };
            }

            if (loginAccount != null)
            {
                string outotp;
                var result = GenerateOtpWithTeleSafe(loginAccount.AccountID, loginAccount.PhoneNumber, "1", out outotp);
                return result;
                //if (type == "10" && result != null && result.ResponseCode == 1 && loginAccount != null && !String.IsNullOrEmpty(loginAccount.SignalID) && !String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
                //{

                //    OneSignalApi.SendByPlayerID(new List<string>() { loginAccount.SignalID }, String.Format(ErrorMsg.SafeNotiyType10, outotp));
                //}
                //return result;
            }




            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.OtpBusy
            };


        }




        [HttpPost]
        [Route("OtpSafeLinkAccount")]
        public dynamic OtpSafeLinkAccount([FromBody] dynamic input)
        {


            string phonenumber = input.PhoneNumber;
            string Otp = input.Otp;
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
            if (String.IsNullOrEmpty(phonenumber))
            {
                return new
                {
                    ResponseCode = -3,
                    Message = ErrorMsg.PhoneEmpty
                };
            }
            //validate phone number

            if (!ValidateInput.ValidatePhoneNumber(phonenumber))
            {
                return new
                {
                    ResponseCode = -3,
                    Message = ErrorMsg.PhoneIncorrect
                };
            }
            //if (IsVieNamMobilePhone(phonenumber))
            //{
            //    return new
            //    {
            //        ResponseCode = -1005,
            //        Message = ErrorMsg.PhoneVietnammobileNotSupport
            //    };
            //}
            if (String.IsNullOrEmpty(Otp))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.OtpEmpty
                };
            }

            if (Otp.Length != OTPSAFE_LENGTH)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.SafeOtpOnlyAccept
                };
            }

            var accountInfo = AccountDAO.Instance.GetAccountInfo(accountId, null, null, ServiceID);
            if (accountInfo == null)
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            if (accountInfo.Status != 1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountLock
                };
            }
            if (!String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountLinkedSafeOtp
                };

            }
            int resOtp = 0;
            long otpID;
            string otmsg;
            SMSDAO.Instance.ValidOtp(accountInfo.AccountID, phonenumber.PhoneFormat(), Otp, ServiceID, out resOtp, out otpID, out otmsg);
            if (resOtp != 1)
            {
                return new
                {
                    ResponseCode = -5,
                    Message = Otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                };
            }

            int Response = 0;
            long SafeID = 0;
            AccountDAO.Instance.OTPSafeLinkAccount(ServiceID, accountId, phonenumber.PhoneFormat(), out SafeID, out Response);
            if (Response == 1)
            {
                accountInfo = AccountDAO.Instance.GetAccountInfo(accountId, null, null, ServiceID);
                if (accountInfo != null)
                {
                    int outResponse;
                    long msgID;
                    var msg = String.Format("You are linked to the account {0}", accountInfo.AccountName);
                    if (accountInfo != null && !String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accountInfo.PhoneSafeNo, msg, out outResponse, out msgID);
                    }
                    if (accountInfo != null && !String.IsNullOrEmpty(accountInfo.SignalID) && !String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accountInfo.SignalID }, msg);
                    }
                }
                return new
                {
                    ResponseCode = 1,
                    Message = ErrorMsg.SafeIDSuccess
                };
            }
            else if (Response == -241)
            {
                return new
                {
                    ResponseCode = -241,
                    Message = ErrorMsg.SafeIDNotCreate
                };
            }
            else if (Response == -105)
            {
                return new
                {
                    ResponseCode = -241,
                    Message = ErrorMsg.AccountLock
                };
            }
            else if (Response == -242)
            {
                return new
                {
                    ResponseCode = -242,
                    Message = ErrorMsg.AccountLinkedSafeOtp
                };
            }
            else if (Response == -243)
            {
                return new
                {
                    ResponseCode = -243,
                    Message = ErrorMsg.OtpSafeUsed
                };
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };



        }
        [HttpPost]
        [Route("OtpSafeUnLinkAccount")]
        public dynamic OtpSafeUnLinkAccount([FromBody] dynamic input)
        {


            string Otp = input.Otp;
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
            if (String.IsNullOrEmpty(Otp))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.OtpEmpty
                };
            }

            if (Otp.Length != OTPSAFE_LENGTH)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.SafeOtpOnlyAccept
                };
            }




            var accountInfo = AccountDAO.Instance.GetAccountInfo(accountId, null, null, ServiceID);
            if (accountInfo == null)
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            if (accountInfo.Status != 1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountLock
                };
            }
            if (String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountNotLinkSafe
                };
            }
            if (accountInfo.AuthenType == 1 && String.IsNullOrEmpty(accountInfo.PhoneNumber) && !String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AuthenTypeNeedRemove
                };
            }


            int resOtp = 0;
            long otpID;
            string otmsg;
            SMSDAO.Instance.ValidOtp(accountInfo.AccountID, accountInfo.PhoneSafeNo, Otp, ServiceID, out resOtp, out otpID, out otmsg);
            if (resOtp != 1)
            {
                return new
                {
                    ResponseCode = -5,
                    Message = Otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                };
            }


            int Response = 0;

            AccountDAO.Instance.OtpSafeDisLinkAccount(ServiceID, accountId, out Response);
            if (Response == 1)
            {

                if (accountInfo != null)
                {
                    int outResponse;
                    long msgID;
                    var msg = String.Format("You unlink the account {0}", accountInfo.AccountName);
                    if (accountInfo != null && !String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accountInfo.PhoneSafeNo, msg, out outResponse, out msgID);
                    }
                    if (accountInfo != null && !String.IsNullOrEmpty(accountInfo.SignalID) && !String.IsNullOrEmpty(accountInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accountInfo.SignalID }, msg);
                    }
                    if (otpID > 0)
                    {
                        SMSDAO.Instance.OtpDeactive(otpID, accountInfo.AccountID, out outResponse);
                    }
                }


                return new
                {
                    ResponseCode = 1,
                    Message = ErrorMsg.UnSafeIDSuccess
                };
            }
            else if (Response == -241)
            {
                return new
                {
                    ResponseCode = -241,
                    Message = ErrorMsg.SafeIDNotCreate
                };
            }
            else if (Response == -105)
            {
                return new
                {
                    ResponseCode = -105,
                    Message = ErrorMsg.AccountLock
                };
            }
            else if (Response == -242)
            {
                return new
                {
                    ResponseCode = -242,
                    Message = ErrorMsg.AccountLinkedSafeOtp
                };
            }
            else if (Response == -243)
            {
                return new
                {
                    ResponseCode = -243,
                    Message = ErrorMsg.OtpSafeUsed
                };
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };



        }
        [HttpPost]
        [Route("OtpType")]
        public dynamic OtpType([FromBody] dynamic input)
        {
            List<OtpType> list = new List<OtpType>();
            list.Add(new OtpType { ID = 1, Name = "Sms Otp" });
            list.Add(new OtpType { ID = 2, Name = "Safe Otp" });


            return new
            {
                ResponseCode = 1,
                List = list
            };



        }

        /// <summary>
        /// common function send otp
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        //private dynamic GenerateOtp(long accountId, string phone)
        //{
        //    var otp = SMSRequest.GenrateOTP();

        //    //trừ tiền mã otp
        //    long responseCurrentID = -99;
        //    long balance = 0;
        //    long currentbalance = 0;
        //    int OtpFeePerTime = 0;
        //    SMSDAO.Instance.OTPInit(accountId, "1", otp, phone.PhoneFormat(), out balance, out currentbalance, out responseCurrentID, out OtpFeePerTime);

        //    if (responseCurrentID == -2)
        //    {
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OTPBitNotEngough
        //        };
        //    }
        //    else if(responseCurrentID<=0 && responseCurrentID!=-2)
        //    {

        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OtpBusy
        //        };
        //    }
        //    //string Description = "Trừ " + OtpFeePerTime + " vào tài khoản do sử dụng OTP";
        //    //int outResponse = 0;
        //   // HistoryDAO.Instance.GameInsert(0, (int)OtpFeePerTime, 0, 0, 2, accountId, responseCurrentID,currentbalance, balance, 0, 0, IPAddressHelper.GetClientIP(), Description, out outResponse);

        //    //AgentSmsApi test = new AgentSmsApi();
        //    //string username = ConfigurationManager.AppSettings["AZTechUserName"].ToString();
        //    //string pass = ConfigurationManager.AppSettings["AZTechPass"].ToString();
        //    //string brand = ConfigurationManager.AppSettings["AZTechBrandName"].ToString();
        //    var strOtpFormat = ConfigurationManager.AppSettings["OTP_MSG"].ToString();
        //    var sms = String.Format(strOtpFormat, otp);
        //    //  var sendResult = test.sendSms(username, pass, brand, sms, newPhone, 1);

        //    var res = OtpSend.Send(phone, sms);


        //    int response = 0;
        //    if (res.code == OtpSuccess.SUCESS)
        //    {
        //        balance = 0;
        //        currentbalance = 0;
        //        OtpFeePerTime = 0;

        //        SMSDAO.Instance.UpdateStatus(responseCurrentID, 1, 1, DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);
        //        if (response >= 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = 1,

        //            };
        //        }
        //        else
        //        {
        //            return new
        //            {
        //                ResponseCode = -99,
        //                Message = ErrorMsg.OtpBusy
        //            };
        //        }

        //    }
        //    else
        //    {
        //        balance = 0;
        //        currentbalance = 0;
        //        OtpFeePerTime = 0;

        //        SMSDAO.Instance.UpdateStatus(responseCurrentID, -1, Convert.ToInt32(res.code), DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);

        //        //Description = "Hoàn " + OtpFeePerTime + " Bit vào tài khoản vì OTP gửi lỗi";
        //      //  HistoryDAO.Instance.GameInsert(0, (int)OtpFeePerTime, 0, 0, 2, accountId, responseCurrentID, currentbalance, balance, 0, 0, IPAddressHelper.GetClientIP(), Description, out outResponse);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OtpBusy
        //        };

        //    }
        //}


    }


}

