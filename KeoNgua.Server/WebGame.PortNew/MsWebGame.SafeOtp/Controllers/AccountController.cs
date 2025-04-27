using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using MsWebGame.SafeOtp.Models;
using MsWebGame.SafeOtp.Providers;
using MsWebGame.SafeOtp.Results;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.IpAddress;
using System.Net;
using TraditionGame.Utilities.Messages;
using MsWebGame.SafeOtp.Helpers;
using System.Diagnostics;
using TraditionGame.Utilities;
using MsWebGame.SafeOtp.Database.DAO;
using TraditionGame.Utilities.Security;

namespace MsWebGame.SafeOtp.Controllers
{
   
    [RoutePrefix("api/Account")]
    public class AccountController : BaseApiController
    {
        [ActionName("PhoneValid")]
        [Route("PhoneValid")]
        [HttpOptions, HttpPost]
        public dynamic PhoneValid([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string phoneNumber = input.PhoneNumber ?? string.Empty;
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneEmpty
                    };
                }
                phoneNumber = phoneNumber != null ? phoneNumber.ToLower() : string.Empty;//lấy ra userme
                if (!String.IsNullOrEmpty(phoneNumber)) phoneNumber = phoneNumber.Trim();


                phoneNumber = phoneNumber.Trim();
                //phoneNumber = AddZeroToPhone(phoneNumber);
                if (!ValidateInput.ValidatePhoneNumberWithRegion(phoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneIncorrect
                    };
                }
                //if (ValidateInput.IsVieNamMobilePhone(phoneNumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.PhoneVietnammobileNotSupport
                //    };
                //}
                int Response = 0;
                AccountDAO.Instance.CheckLoginOverTime(phoneNumber.PhoneFormat(), out Response);
                if (Response != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLoginOvertime
                    };
                }
                return new
                {
                    ResponseCode = 1,
                    Message = ErrorMsg.PhoneValid
                };
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                NLogManager.LogError("Line" + line);
                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }


        [ActionName("Login")]
        [Route("Login")]
        [HttpOptions, HttpPost]
        public dynamic Login([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }

                int loginType = input.LoginType ?? -1;
                int response = -99;
                int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;

                string clientIp = IPAddressHelper.GetClientIP();
                string deviceId = input.DeviceId ?? string.Empty;//lấy ra deviceId
                string otp = input.Otp ?? String.Empty;
               
                string phoneNumber = input.PhoneNumber ?? string.Empty;
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneEmpty
                    };
                }
                if (string.IsNullOrEmpty(otp))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpIncorrect
                    };
                }

                phoneNumber = phoneNumber != null ? phoneNumber.ToLower() : string.Empty;//lấy ra userme
                if (!String.IsNullOrEmpty(phoneNumber)) phoneNumber = phoneNumber.Trim();

               
                phoneNumber = phoneNumber.Trim();
                //phoneNumber = AddZeroToPhone(phoneNumber);
                if (!ValidateInput.ValidatePhoneNumberWithRegion(phoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneIncorrect
                    };
                }
                //if (ValidateInput.IsVieNamMobilePhone(phoneNumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.PhoneVietnammobileNotSupport
                //    };
                //}
              
                var token = SafeOtpTokenProvider.GenerateToken(0, phoneNumber, clientIp);
                response = 0;
                long SafeID = 0;
                AccountDAO.Instance.OTPSafeLogin(phoneNumber, otp.Trim(), token, out response,out SafeID);
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        SafeID= SafeID,
                        Token = token
                    };
                }else if (response == -239)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Token = ErrorMsg.AccountLoginOvertime
                    };
                }else if(response == -240)
                {
                    return new
                    {
                        ResponseCode = -240,
                        Token = ErrorMsg.OtpIncorrect
                    };
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                NLogManager.LogError("Line" + line);
                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }

        [ActionName("GetAccountInfor")]
        [Route("GetAccountInfor")]
        [HttpOptions, HttpPost]
        public dynamic GetAccountInfor([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string Token = input.Token ?? string.Empty;
               


                if (String.IsNullOrEmpty(Token))
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                int response = 0;
                Token = Token.Trim();
                var account = AccountDAO.Instance.GetAccountByToken(Token,out response);
                account.PhoneOTP = account.PhoneOTP.PhoneDisplayFormat();
                return new
                {

                    ResponseCode = 1,
                    AccountInfo = account
                };
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }

        [ActionName("Logout")]
        [Route("Logout")]
        [HttpOptions, HttpPost]
        public dynamic Logout([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string Token = input.Token ?? string.Empty;
                if (string.IsNullOrEmpty(Token))
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                Token = Token != null ? Token : string.Empty;//lấy ra userme
                int Response = 0;
                AccountDAO.Instance.OTPSafeLogout(Token, out Response);
                return new
                {
                    ResponseCode = 1,

                };

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                NLogManager.LogError("Line" + line);
                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        [ActionName("UpdateAccountInfo")]
        [Route("UpdateAccountInfo")]
        [HttpOptions, HttpPost]
        public dynamic UpdateAccountInfo([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string Token = input.Token ?? string.Empty;
                string Firstname = input.FirstName ?? string.Empty;
                string LastName = input.LastName ?? string.Empty;
                
                 Token = !String.IsNullOrEmpty(Token) ? Token.Trim() : string.Empty;
                 Firstname = !String.IsNullOrEmpty(Firstname) ? Firstname.Trim(): string.Empty;
                 LastName = !String.IsNullOrEmpty(LastName) ? LastName.Trim() : string.Empty;
                if (String.IsNullOrEmpty(Token))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (String.IsNullOrEmpty(Firstname))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.FirstnameRequired
                    };
                }
                if (String.IsNullOrEmpty(LastName))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.LastnameRequired
                    };
                }
                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(Token, out response);
                if (response != 1 || account == null)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
               bool  outresponse = false;
                AccountDAO.Instance.OTPSafeUpdate(account.ID, Firstname, LastName, null, out outresponse);
                if (outresponse)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateSuccess
                    };
                }


                return new
                {

                    ResponseCode = -1,
                    Message = ErrorMsg.UpdateFail
                };
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        [ActionName("UpdateSignalID")]
        [Route("UpdateSignalID")]
        [HttpOptions, HttpPost]
        public dynamic UpdateSignalID([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string Token = input.Token ?? string.Empty;
                string SignalID = input.SignalID ?? string.Empty;
               

                Token = !String.IsNullOrEmpty(Token) ? Token.Trim() : string.Empty;
                SignalID = !String.IsNullOrEmpty(SignalID) ? SignalID.Trim() : string.Empty;
               
                if (String.IsNullOrEmpty(Token))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (String.IsNullOrEmpty(SignalID))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                
                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(Token, out response);
                if (response != 1 || account == null)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                bool outresponse = false;
                AccountDAO.Instance.OTPSafeUpdate(account.ID, null, null, SignalID, out outresponse);
                if (outresponse)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateSuccess
                    };
                }


                return new
                {

                    ResponseCode = -1,
                    Message = ErrorMsg.UpdateFail
                };
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
    }
}
