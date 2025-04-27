using MsWebGame.SafeOtp.Controllers;
using MsWebGame.SafeOtp.Database.DAO;
using MsWebGame.SafeOtp.Helpers;
using MsWebGame.SafeOtp.Helpers.OTPs.MobileSMS;
using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.SafeOtp.Controllerss
{
    [RoutePrefix("api/Otp")]
    public class OtpController : BaseApiController
    {


        [ActionName("GetOtp")]
        [Route("GetOtp")]
        [HttpOptions, HttpPost]
        public dynamic GetOtp([FromBody] dynamic input)
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
                //var formatPhone = phoneNumber.PhoneFormat();

                var formatPhone = phoneNumber;
                int Response = 0;
                AccountDAO.Instance.CheckLoginOverTime(formatPhone, out Response);
                if (Response != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLoginOvertime
                    };
                }
                string content = System.Configuration.ConfigurationManager.AppSettings["OTP_Mess"];
                var otp = OTPHelper.GenrateOTP(6);

                //test - bo phan gui SMS
                //int response = 0;
                //OtpDAO.Instance.SmsOTPInsert(0, 0, "10", otp, formatPhone, 1, out response);
                //if (response == 1)
                //{
                //    return new
                //    {
                //        ResponseCode = 1,

                //    };
                //}
                //end test
                var otpResponse = "1";
                if (formatPhone.StartsWith("84"))
                {
                    // Send OTP to phone number
                    otpResponse = OtpSend.Send4(formatPhone, otp);
                }


                int response = 0;
                if (otpResponse == "1")
                {

                    OtpDAO.Instance.SmsOTPInsert(0, 0, "10", otp, formatPhone, 1, out response);
                    if (response == 1)
                    {
                        return new
                        {
                            ResponseCode = 1,

                        };
                    }
                }
                else
                {
                    OtpDAO.Instance.SmsOTPInsert(0, 0, "10", otp, formatPhone, ConvertUtil.ToInt(otpResponse), out response);

                }



                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.MSNErr
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




    }
}
