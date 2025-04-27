using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Utils;
using WebGame.Header.Utils._1Pays.SMSs;
using WebGame.Payment.Database.DAO;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers.OTPs.MobileSMS;
using TraditionGame.Utilities;
using System.IO;
using System.Text;
using MsWebGame.Portal.Database;
using TraditionGame.Utilities.OneSignal;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Log;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using MsWebGame.Portal.Database.DTO;
using System.Web.Http.Results;

namespace MsWebGame.Portal.Controllers
{
    public class BaseApiController : ApiController
    {
        protected HttpContext _Context;
        public BaseApiController()
        {

        }
        protected string LowestVips = "Vip 1";
        protected int VipIndex = 1;
        protected int STATUS_SMG_NOT_REFUND = -3;
        protected string STATUS_SMG_NOT_REFUND_REASON = "Card loading failed - wrong denomination";
        protected int OTPSAFE_LENGTH = 5;
        protected int OTPSMS_LENGTH = 7;
        protected int OTPAPP_LENGTH = 6;
        protected int SAFEMSG_TYPE = 2;
        protected int DNA_PLATFORM = ConvertUtil.ToInt(ConfigurationManager.AppSettings["DNA_PLATFORM"].ToString());//1 dev,2 deploy
        protected bool IsVieNamMobilePhone(string phoneNumber)
        {
            var listPhone = new List<string>() { "092", "056", "058", "0186", "0188" };
            var splitPhone = phoneNumber.Substring(0, 3);
            return listPhone.Contains(splitPhone);


        }
        protected string GetReferUrlByService()
        {
            try
            {
                if (ServiceID == 1)
                {
                    return "https://ibom.cc";
                }
                else if (ServiceID == 2)
                {
                    return "https://ibom2.cc";
                }
                else if (ServiceID == 3)
                {
                    return "https://ibom3.cc";
                }
                return string.Empty;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return string.Empty;

            }

        }
        protected int GetCardCodeIndex(String cardOperatorCode)
        {
            try
            {
                if (cardOperatorCode.Contains("VTT")) return 1;
                else if (cardOperatorCode.Contains("VNP")) return 2;
                else if (cardOperatorCode.Contains("ZING")) return 8;
                else if (cardOperatorCode.Contains("VCOIN")) return 9;
                else return 3;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public OtpResponse SendTelePushTelegramID(int Action,string Content, long TelegramID = 0, bool type = true , string AccountName = "")
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = "http://localhost:5005/push-system-send";
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                Content = Content,
                TelegramID = TelegramID,
                Type = type,
                AccountName = AccountName,
                Action = Action
            });

            NLogManager.LogMessage("data:" + urlParameter);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.UTF8.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();
                model.code = result;
                model.des = "";
            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                NLogManager.PublishException(e);
            }
            return model;
        }
        public OtpResponse SendTelePush(string Content, long TelegramID = 0)
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = "http://localhost:5005/push-system-send";
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                Content = Content,
                Action = TelegramID
            });
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.UTF8.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();
                model.code = result;
                model.des = "";
            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                NLogManager.PublishException(e);
            }
            return model;
        }
        public OtpResponse SendTeleOdp(string phone, long TelegramID = 0,int Type = 2)
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = "http://localhost:5005/odp-send";
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                phone = phone,
                TelegramID = TelegramID,
                Type = Type
            });
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();
                model.code = result;
                model.des = "";
            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                NLogManager.PublishException(e);
            }
            return model;
        }
        public OtpResponse SendTeleOtp(string phone,long TelegramID = 0)
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = "http://localhost:5005/otp-send";
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                phone = phone,
                TelegramID = TelegramID
            });
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                
                response.Close();
                model.code = result;
                model.des = "";
            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                NLogManager.PublishException(e);
            }
            return model;
        }
        protected dynamic TeleNotify(string chatID, string content)
        {
            try
            {
                string apiToken = ConfigurationManager.AppSettings["TOKEN_NOTIFY_TELE"].ToString();
                string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";


                urlString = String.Format(urlString, apiToken, chatID, content);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebRequest request = WebRequest.Create(urlString);
                Stream rs = request.GetResponse().GetResponseStream();

                StreamReader reader = new StreamReader(rs);
                string line = "";
                StringBuilder sb = new StringBuilder();
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null)
                        sb.Append(line);
                }
                string response = sb.ToString();
                return new
                {
                    ResponseCode = 1,

                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = 0,

                };
            }


        }
        protected int USER_TYPE = 1;
        protected int AGENCY_TYPE = 2;
        protected int ServiceID = ConvertUtil.ToInt(ConfigurationManager.AppSettings["SERVICE_ID"].ToString());
        /// <summary>
        /// Generate Otp
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        protected dynamic GenerateOtpWithTeleSafe(long accountId, string phone, string type, out string outotp)
        {
            if (type == "10" || type == "20")
            {
                var otp = SMSRequest.GenrateOTP(OTPSAFE_LENGTH);
                outotp = otp;
                int response = 0;
                SMSDAO.Instance.SPSmsOTPVerify(ServiceID, accountId, type, otp, phone, 1, out response);
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,

                    };
                }
                else if (response == -234)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountNotLinkSafe
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.MSNErr
                    };
                }
            }
            else
            {
                var otp = SMSRequest.GenrateOTP(7);
                outotp = otp;
                //trừ tiền mã otp
                long responseCurrentID = -99;
                long balance = 0;
                long currentbalance = 0;
                int OtpFeePerTime = 0;
                SMSDAO.Instance.OTPInit(accountId, type, otp, phone.PhoneFormat(), ServiceID, out balance, out currentbalance, out responseCurrentID, out OtpFeePerTime);

                if (responseCurrentID == -2)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OTPBitNotEngough
                    };
                }
                else if (responseCurrentID <= 0 && responseCurrentID != -2)
                {

                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OtpBusy
                    };
                }

                //var strOtpFormat = ConfigurationManager.AppSettings["OTP_MSG"].ToString();
                var sms = String.Format(otp);
                //  var sendResult = test.sendSms(username, pass, brand, sms, newPhone, 1);

                var res = OtpSend.Send5(phone.PhoneFormat(), sms);

                //OtpResponse res = SendTeleOtp(phone);
                //OtpResponse res = TeleNotify(phone,sms);

                //return Json(new { ResponseCode = (int)ErrorCode.Success, Message = otpResponse.code });


                //NLogManager.LogMessage("Ket qua OTP: " + res.code);

                int response = 0;
                if (res == "1")
                {
                    balance = 0;
                    currentbalance = 0;
                    OtpFeePerTime = 0;

                    SMSDAO.Instance.UpdateStatus(responseCurrentID, 1, 1, DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);



                    if (response >= 1)
                    {
                        return new
                        {
                            ResponseCode = 1,

                        };
                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = -99,
                            Message = ErrorMsg.OtpBusy
                        };
                    }

                }
                else
                {
                    balance = 0;
                    currentbalance = 0;
                    OtpFeePerTime = 0;

                    SMSDAO.Instance.UpdateStatus(responseCurrentID, -1, Convert.ToInt32(res), DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);

                    //Description = "Hoàn " + OtpFeePerTime + " Bit vào tài khoản vì OTP gửi lỗi";
                    //  HistoryDAO.Instance.GameInsert(0, (int)OtpFeePerTime, 0, 0, 2, accountId, responseCurrentID, currentbalance, balance, 0, 0, IPAddressHelper.GetClientIP(), Description, out outResponse);
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OtpBusy
                    };

                }

            }

        }



        protected dynamic GenerateOtpWithTeleSafe2(long accountId, string phone, string type, out string outotp)
        {
            if (type == "10" || type == "20" || type == "21")
            {
                var otp = SMSRequest.GenrateOTP(OTPSAFE_LENGTH);
                outotp = otp;
                int response = 0;
                SMSDAO.Instance.SPSmsODPVerify(ServiceID, accountId, type, otp, phone, 1, out response);
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,

                    };
                }
                else if (response == -234)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountNotLinkSafe
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.MSNErr
                    };
                }
            }
            else
            {
                var otp = SMSRequest.GenrateOTP(6);
                outotp = otp;
                //trừ tiền mã otp
                long responseCurrentID = -99;
                long balance = 0;
                long currentbalance = 0;
                int OtpFeePerTime = 0;
                SMSDAO.Instance.ODPInit(accountId, type, otp, phone.PhoneFormat(), ServiceID, out balance, out currentbalance, out responseCurrentID, out OtpFeePerTime);

                if (responseCurrentID == -2)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OTPBitNotEngough
                    };
                }
                else if (responseCurrentID <= 0 && responseCurrentID != -2)
                {

                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OtpBusy
                    };
                }

                //var strOtpFormat = ConfigurationManager.AppSettings["OTP_MSG"].ToString();
                var sms = String.Format(otp);
                //  var sendResult = test.sendSms(username, pass, brand, sms, newPhone, 1);

                var res = OtpSend.Send3(phone, sms);
                NLogManager.LogMessage("Ket qua OTP: " + res);

                int response = 0;
                if (res == "1")
                {
                    balance = 0;
                    currentbalance = 0;
                    OtpFeePerTime = 0;

                    SMSDAO.Instance.UpdateStatusODP(responseCurrentID, 1, 1, DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);
                    if (response >= 1)
                    {
                        return new
                        {
                            ResponseCode = 1,

                        };
                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = -99,
                            Message = ErrorMsg.OtpBusy
                        };
                    }

                }
                else
                {
                    balance = 0;
                    currentbalance = 0;
                    OtpFeePerTime = 0;

                    SMSDAO.Instance.UpdateStatusODP(responseCurrentID, -1, Convert.ToInt32(res), DateTime.Now.ToShortDateString(), out response, out OtpFeePerTime, out balance, out currentbalance);

                    //Description = "Hoàn " + OtpFeePerTime + " Bit vào tài khoản vì OTP gửi lỗi";
                    //  HistoryDAO.Instance.GameInsert(0, (int)OtpFeePerTime, 0, 0, 2, accountId, responseCurrentID, currentbalance, balance, 0, 0, IPAddressHelper.GetClientIP(), Description, out outResponse);
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.OtpBusy
                    };

                }

            }

        }








        /// <summary>
        /// hàm này chuyên dùng gửi safe Otp msg
        /// </summary>
        /// <param name="PhoneSafeNo"></param>
        /// <param name="msg"></param>
        /// <param name="SignalID"></param>
        /// <returns></returns>
        protected bool GenerateSafeOtpMsg(string PhoneSafeNo, string SignalID, string msg)
        {
            if (!String.IsNullOrEmpty(PhoneSafeNo))
            {
                int outResponse;
                long msgID;



                if (!String.IsNullOrEmpty(PhoneSafeNo))
                {
                    SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, PhoneSafeNo, msg, out outResponse, out msgID);
                }
                if (!String.IsNullOrEmpty(SignalID) && !String.IsNullOrEmpty(PhoneSafeNo))
                {
                    OneSignalApi.SendByPlayerID(new List<string>() { SignalID }, msg);
                }
                return true;


            }
            return false;
        }
        //protected dynamic GenerateOtp(long accountId, string phone, string type)
        //{
        //    var otp = SMSRequest.GenrateOTP();

        //    //trừ tiền mã otp
        //    long responseCurrentID = -99;
        //    long balance = 0;
        //    long currentbalance = 0;
        //    int OtpFeePerTime = 0;
        //    SMSDAO.Instance.OTPInit(accountId, type, otp, phone.PhoneFormat(), ServiceID, out balance, out currentbalance, out responseCurrentID, out OtpFeePerTime);

        //    if (responseCurrentID == -2)
        //    {
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OTPBitNotEngough
        //        };
        //    }
        //    else if (responseCurrentID <= 0 && responseCurrentID != -2)
        //    {

        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OtpBusy
        //        };
        //    }

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
        //        //  HistoryDAO.Instance.GameInsert(0, (int)OtpFeePerTime, 0, 0, 2, accountId, responseCurrentID, currentbalance, balance, 0, 0, IPAddressHelper.GetClientIP(), Description, out outResponse);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.OtpBusy
        //        };

        //    }
        //}

        protected bool SendDNA(long requestId, long accountId, int transType, long amount, long amountGame)
        {
            try
            {
                var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                dnaHelper.SendTransactionPURCHASE(accountId, transType, null, amount, amountGame);
                ProfileLogger.LogMessage(String.Format("accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", accountId, transType, amount, amountGame, requestId));
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;

            }

        }

        #region Bank
        protected static int HTTP_OK = 200;

        //protected int PartnerID = 1;
        protected int PENDING_STATUS = 0;
        protected int CONFIRM_STATUS = 2;
        protected int APPROVED_STATUS = 3;
        protected int FAIL_API1 = -1;
        protected int FAIL_API2 = -2;
        protected int SUCCESS_STATUS = 1;
        protected int CANCELLED_STATUS = 4;
        protected int REFUSED_STATUS = 5;


        protected int Bank_Success = 1;
        protected int Bank_Fail = 0;

        protected static string PENDING = "pending";//Đang chờ xử lý, khi mới tạo order
        protected static string PROCESSING = "processing";//Sau khi tiền/USDT nộp vào tk thành công
        protected static string COMPLETED = "completed";//xử lý thành công
        protected static string CANCELED = "canceled";//order bị cancel sau khi hết thừoi gian xử lý
        protected static string FAILED = "failed";//Order bị lỗi trogn quá tình xử lý
        protected string BankOperatorConfig
        {

            get
            {
                if (ServiceID == 1) return "BANK_B1";
                if (ServiceID == 2) return "BANK_B2";
                if (ServiceID == 3) return "BANK B3";
                return string.Empty;
            }
        }
        protected string MOMOConfig
        {

            get
            {
                if (ServiceID == 1) return "MOMO_B1";
                if (ServiceID == 2) return "MOMO_B2";
                if (ServiceID == 3) return "MOMO_B3";
                return string.Empty;
            }
        }

        protected string VIETTELPAYConfig
        {

            get
            {
                if (ServiceID == 1) return "VIETTELPAY_B1";
                if (ServiceID == 2) return "VIETTELPAY_B2";
                if (ServiceID == 3) return "VIETTELPAY_B3";
                return string.Empty;
            }
        }
        protected string MOMOConfigShopTheNhanh
        {

            get
            {
                if (ServiceID == 1) return "MOMO_STN_B1";

                if (ServiceID == 3) return "MOMO_STN_B3";
                return string.Empty;
            }
        }
        protected static int CHECKER_ID = 5;

        protected int MappingStatus(string USDTStatus)
        {
            if (USDTStatus == PENDING)
            {
                return PENDING_STATUS;
            }
            if (USDTStatus == PROCESSING)
            {
                return PENDING_STATUS;
            }
            if (USDTStatus == COMPLETED)
            {
                return SUCCESS_STATUS;
            }
            if (USDTStatus == CANCELED)
            {
                return CANCELLED_STATUS;
            }
            if (USDTStatus == FAILED)
            {
                return REFUSED_STATUS;
            }
            return 0;

        }

        protected string MappingStatuStr(int Status, int RequestType)
        {
            if (RequestType == 1)
            {
                if (Status == 0)
                {
                    return "Waiting for progressing";
                }
                if (Status == 1)
                {
                    return "Success";
                }
                if (Status == 3)
                {
                    return "Pending";
                }
                if (Status == 4)
                {
                    return "Admin-Reject";
                }
                if (Status == 5)
                {
                    return "Failure";
                }
                if (Status == 6)
                {
                    return "Admin-Refund";
                }
                var fail = new List<int> { -1, -2 };
                if (fail.Contains(Status))
                {
                    return "Failure";
                }
            }
            if (RequestType == 2)
            {
                if (Status == 0)
                {
                    return "Waiting for approval";
                }
                if (Status == 1)
                {
                    return "Success";
                }
                if (Status == 3)
                {
                    return "Waiting for the bank";
                }
                if (Status == 4)
                {
                    return "Admin-Reject";
                }
                if (Status == 5)
                {
                    return "Admin-Refund";
                }
                if (Status == -2)
                {
                    return "Wait for admin to process";
                }
                var fail = new List<int> { -1 };
                if (fail.Contains(Status))
                {
                    return "Admin-Refund";
                }
            }


            return "Failure";

        }
        #endregion
    }
}
