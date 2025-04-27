using System;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Cookies;
using TraditionGame.Utilities.Facebook;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.IpAddress;

using TraditionGame.Utilities.Security;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models;
using System.Configuration;
using TraditionGame.Utilities.Messages;
using MsWebGame.Portal.Server.Hubs;
using WebGame.Payment.Database.DAO;
using TraditionGame.Utilities.Utils;
using System.Diagnostics;
using MsWebGame.Portal.Helper;
using TraditionGame.Utilities;
using MsWebGame.Portal.Database;
using TraditionGame.Utilities.OneSignal;
using System.Collections.Generic;
using TraditionGame.Utilities.DNA;
using System.Linq;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/ServiceOff")]
    public class ServiceOffController : BaseApiController
    {
        private int serviceMerge = 3;
        private int FACEBOOKTYPE = 1;
        private int NORMALTYPE = 2;
        private List<string> notAcceptName = new List<string> { "admin" };
        [Route("BBOffCheck")]
        [AllowAnonymous]
        [HttpOptions, HttpGet]
        public dynamic BBOffCheck()
        {
            if (ServiceID != 2)
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
            string UserName;
            string NickName;
            int UserType;
            int Response;
            ServiceOffDAO.Instance.BbUserCutOffCheck(accountId, out UserName, out NickName, out UserType, out Response);
            bool OtptNeed = !String.IsNullOrEmpty(user.PhoneNumber) || !String.IsNullOrEmpty(user.PhoneSafeNo);

            return new
            {
                ResponseCode = 1,
                UserType = UserType,//--1: fb; 2: thuong
                OtpNeed = OtptNeed,
                CutOffType = Response,//--0: khong nam trong danh sach; 1: can thay doi; 2: da thay doi
            };

        }



        [Route("BBOffUpdate")]
        [AllowAnonymous]
        [HttpOptions, HttpPost]
        public dynamic BBOffUpdate([FromBody] dynamic input)
        {
            if (ServiceID != 2)
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
            string username = input.UserName ?? string.Empty;
            username = username != null ? username.ToLower() : string.Empty;
            //lấy nick name
            string nickname = input.NickName ?? string.Empty;
            if (!String.IsNullOrEmpty(username)) username = username.Trim();
            if (!String.IsNullOrEmpty(nickname)) nickname = nickname.Trim();
            string password = input.Password ?? string.Empty;
            if (!String.IsNullOrEmpty(password)) password = password.Trim();


            //kiểm tra username 

            if (username.Length < 6 || username.Length > 18)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserNameLength
                };
            }

            if (username.Contains(" "))
            {
                return new
                {
                    ResponseCode = -101,
                    Message = ErrorMsg.UserNameContainSpace
                };
            }
            if (ValidateInput.IsNickNameContainNotAllowString(username))
            {
                return new
                {
                    ResponseCode = -102,
                    Message = ErrorMsg.UserNameContainsNotAllowString
                };
            }
            if (!ValidateInput.ValidateUserName(username))
            {
                return new
                {
                    ResponseCode = -102,
                    Message = ErrorMsg.UsernameIncorrect
                };
            }
            if (notAcceptName.Contains(username))
            {
                return new
                {
                    ResponseCode = -102,
                    Message = "Username contains characters that are not allowed"
                };
            }
            //kiểm tra username có trùng trong cổng 3 hay không
            int outResponse = 0;
            AccountDAO.Instance.CheckAccountCheckExist(1, username, serviceMerge, out outResponse);
            if (outResponse != 1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = "Username already exist"
                };
            }

            //end kiểm tra username


            //kiểm tra displayname 


            if (String.IsNullOrEmpty(nickname))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.NickNameEmpty
                };
            }
            nickname = nickname.Trim();
            //độ dài không phù hợp
            if (nickname.Length < 6 || nickname.Length > 16)
            {
                return new
                {
                    ResponseCode = -102,
                    Message = ErrorMsg.NickNameLength
                };
            }
            //chứa khoảng trắng
            if (nickname.Contains(" "))
            {
                return new
                {
                    ResponseCode = -101,
                    Message = ErrorMsg.NickNameNotContainSpace
                };
            }
            //chứa kí tự ko cho phép
            if (ValidateInput.IsNickNameContainNotAllowString(nickname))
            {
                return new
                {
                    ResponseCode = -102,
                    Message = ErrorMsg.NickNameContainNotAlowString
                };
            }


            if (!ValidateInput.ValidateUserName(nickname))
            {

                return new
                {
                    ResponseCode = -103,
                    Message = ErrorMsg.DisplayNameIncorrect
                };
            }
            if (username.ToLower().Contains(nickname.ToLower()))
            {
                return new
                {
                    ResponseCode = -103,
                    Message = "Username cannot contain display name"
                };
            }
            if (notAcceptName.Contains(nickname.ToLower()))
            {
                return new
                {
                    ResponseCode = -102,
                    Message = "Display name contains disallowed characters"
                };
            }
            //kiểm tra displayname trùng
            outResponse = 0;
            AccountDAO.Instance.CheckAccountCheckExist(2, nickname, serviceMerge, out outResponse);
            if (outResponse != 1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message ="Display name already exists "
                };
            }

            //kết thúc kiểm tra displayname


            string outUserName;
            string outNickName;
            int outUserType;
            outResponse = -1;
            ServiceOffDAO.Instance.BbUserCutOffCheck(accountId, out outUserName, out outNickName, out outUserType, out outResponse);
            bool OtptNeed = !String.IsNullOrEmpty(user.PhoneNumber) || !String.IsNullOrEmpty(user.PhoneSafeNo);
            //kiểm tra password nếu là tài khoản facebook
            if (outUserType == FACEBOOKTYPE)
            {
                if (String.IsNullOrEmpty(password))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PasswordEmpty
                    };
                }
                if (password.Length < 6 || password.Length > 16)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PasswordLength
                    };

                }

                if (!ValidateInput.IsValidatePass(password))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PasswordIncorrect
                    };
                }
            }else
            {
                if (!String.IsNullOrEmpty(password))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Password cannot be updated with accounts that are not facebook"
                    };
                }
            }
            //kết thúc kiểm tra facebook password





            if (outResponse == 0)
            {

                return new
                {
                    ResponseCode = -1005,
                    Message = "The account is not on the list to change"
                };
            }
            else if (outResponse == 2)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = "Account changed to port B3"
                };
            }
            else if (outResponse == 1)
            {
                //kiểm tra otp nếu otp cần
                if (OtptNeed)
                {
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
                    int resOtp = 0;
                    long otpID;
                    string otmsg;
                    SMSDAO.Instance.ValidOtp(user.AccountID, Otp.Length == OTPSAFE_LENGTH ? user.PhoneSafeNo : user.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
                    if (resOtp != 1)
                    {
                        return new
                        {
                            ResponseCode = -5,
                            Message =   ErrorMsg.OtpIncorrect
                        };
                    }
                }
                //kết thúc kiểm tra otp với tài khoản cần thay đổi
                //cập nhật thay đổi

                int Response = 0;
                var encryptPass = !String.IsNullOrEmpty(password) ? Security.SHA256Encrypt(password) : null;
                password= !String.IsNullOrEmpty(password) ? password : null;
                ServiceOffDAO.Instance.BbUserCutOffUpdate(user.AccountID, username, nickname, encryptPass, password, out Response);
                if (Response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Update successful"
                    };
                }
                else if (Response == -4)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Username already exists"
                    };
                }
                else if (Response == -5)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Display name already exists"
                    };
                }
                else if (Response == -6)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Account registered to convert ."
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = Response,
                        Message = "Update failed|" + Response
                    };
                }
                //kết thúc cập nhật thay đổi
            }
            else
            {
                return new




                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.InProccessException + "|" + outResponse
                };
            }
        }
    }
}