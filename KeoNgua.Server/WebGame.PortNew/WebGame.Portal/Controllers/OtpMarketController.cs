using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Utils;

using WebGame.Header.Utils._1Pays.SMSs;
using WebGame.Payment.Database.DAO;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Helpers.OTPs.MobileSMS;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/OtpMarket")]
    public class OtpMarketController : BaseApiController
    {
        /// <summary>
        /// hàm này dùng cho đại lý (type=2)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // GET: OtpMarket
        [HttpPost]
        [Route("GetOtp")]
        public dynamic GetOtp([FromBody]dynamic input)
        {
            string phonenumber = input.PhoneNumber;
            long accountId = input.AccountID;
             int type = input.Type;
            if (type != 20 && type != 2)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }
            if (accountId <= 0)
            {
                return new
                {
                    ResponseCode =Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            if (String.IsNullOrEmpty(phonenumber))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.PhoneEmpty
                };
            }
            if (!ValidateInput.ValidatePhoneNumberWithRegion(phonenumber))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.PhoneIncorrect
                };
            }
            string outotp;
            if (type == 20)
            {
                phonenumber = phonenumber.PhoneFormat();
            }
           
            return GenerateOtpWithTeleSafe(accountId, phonenumber, type.ToString(),out outotp);
        }


        [HttpPost]
        [Route("GetOdp")]
        public dynamic GetOdp([FromBody] dynamic input)
        {
            string phonenumber = input.PhoneNumber;
            long accountId = input.AccountID;
            int type = input.Type;
            if (type != 20 && type != 2 && type != 21)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }
            if (accountId <= 0)
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
                    ResponseCode = -1005,
                    Message = ErrorMsg.PhoneEmpty
                };
            }
            if (!ValidateInput.ValidatePhoneNumber(phonenumber))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.PhoneIncorrect
                };
            }
            string outotp;
            if (type == 21)
            {
                OtpResponse result = SendTeleOdp(phonenumber.PhoneFormat(), 0,2);
                return new
                {
                    ResponseCode = 1,
                    Message = result.code
                };
            }
            else
            {
                phonenumber = phonenumber.PhoneFormat();
                return GenerateOtpWithTeleSafe2(accountId, phonenumber, type.ToString(), out outotp);
               
            }
        }

        /// <summary>
        /// common function send otp
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        //private dynamic GenerateOtp(long accountId, string phone,int type)
        //{
        //    var otp = SMSRequest.GenrateOTP();

        //    //trừ tiền mã otp
        //    long responseCurrentID = -99;
        //    long balance = 0;
        //    long currentbalance = 0;
        //    double OtpFeePerTime = 0;
        //    MarketStarSMSOTPDAO.Instance.OTPInit(accountId, type, "1", otp, phone.PhoneFormat(), out balance, out currentbalance, out responseCurrentID, out OtpFeePerTime);
        //    if (responseCurrentID <= 0)
        //    {

        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OtpBusy
        //        };
        //    }
        //  //  string Description = "Trừ " + OtpFeePerTime + " vào tài khoản do sử dụng OTP";
        //  //  int outResponse = 0;
        //   //  HistoryDAO.Instance.GameInsert(0, (int)OtpFeePerTime, 0, 0, 2, accountId, responseCurrentID, 0, balance, 0, 0, IPAddressHelper.GetClientIP(), Description, out outResponse);

        //    //AgentSmsApi test = new AgentSmsApi();
        //    //string username = ConfigurationManager.AppSettings["AZTechUserName"].ToString();
        //    //string pass =     ConfigurationManager.AppSettings["AZTechPass"].ToString();
        //    //string brand =      ConfigurationManager.AppSettings["AZTechBrandName"].ToString();

        //    var strOtpFormat= ConfigurationManager.AppSettings["OTP_MSG"].ToString();
        //    var sms = String.Format(strOtpFormat, otp);
        //    // var sendResult = test.sendSms(username, pass, brand, sms, newPhone, 1);
        //    var res = OtpSend.Send(phone, sms);
        //    int response = 0;
        //    if (res.code == OtpSuccess.SUCESS)
        //    {
        //        balance = 0;
        //        currentbalance = 0;
        //        OtpFeePerTime = 0;

        //        MarketStarSMSOTPDAO.Instance.UpdateStatus(responseCurrentID, 1, 1, DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);
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

        //        MarketStarSMSOTPDAO.Instance.UpdateStatus(responseCurrentID, -1, Convert.ToInt32(res.code), DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);


        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OtpBusy
        //        };

        //    }
        //}
    }
}