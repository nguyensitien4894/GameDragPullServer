using MsWebGame.SafeOtp.Database.DAO;
using MsWebGame.SafeOtp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.OneSignal;

namespace MsWebGame.SafeOtp.Controllers
{
    [RoutePrefix("api/Message")]
    public class MessageController : BaseApiController
    {
        private string recmd = ConfigurationManager.AppSettings["COMMAND_REGISTER"].ToString();



        /// <summary>
        ///  thực thi command chat (để lấy các mã otp
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("InsertCommand")]
        [Route("InsertCommand")]
        [HttpOptions, HttpPost]
        public dynamic InsertCommand([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                int ServiceID = input.ServiceID ?? 0;
                if (ServiceID <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }

                string Token = input.Token ?? string.Empty;
                Token = !String.IsNullOrEmpty(Token) ? Token.Trim() : string.Empty;
                string command = input.command ?? string.Empty;
                command = !String.IsNullOrEmpty(command) ? command.Trim() : string.Empty;

                if (String.IsNullOrEmpty(Token))
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
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
                //var registercommands = recmd.Split(',').ToArray();
                //if (String.IsNullOrEmpty(command)|| !registercommands.Contains(command.ToLower()))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.CommandNotRegister
                //    };
                //}

                response = 0;
                var otp = OTPHelper.GenrateOTP(OTPSAFE_LENGTH);
                string msg = string.Empty;
                MessageDAO.Instance.OTPSafeMessageCommand(ServiceID, account.ID, command, otp, out msg, out response);
                if (response == 1)
                {
                    NLogManager.LogMessage("SignNalID" + account.SignalID);
                    if (!String.IsNullOrEmpty(account.SignalID))
                    {
                        var lstPlayerIDs = new List<string>();
                        lstPlayerIDs.Add(account.SignalID);
                        OneSignalApi.SendByPlayerID(lstPlayerIDs, msg);
                    }

                    return new
                    {
                        ResponseCode = 1,
                        Message = msg
                    };

                }
                else if (response == -233)
                {
                    return new
                    {
                        ResponseCode = -233,
                        Message = ErrorMsg.AccountSafeInValid
                    };
                }
                else if (response == -234)
                {
                    return new
                    {
                        ResponseCode = -234,
                        Message = ErrorMsg.AccountSaveNotLink
                    };
                }
                else if (response == -237)
                {
                    return new
                    {
                        ResponseCode = -237,
                        Message = ErrorMsg.OtpBusy
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

        /// <summary>
        /// Get List Message
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("GetMessage")]
        [Route("GetMessage")]
        [HttpOptions, HttpPost]
        public dynamic GetMessage([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                int ServiceID = input.ServiceID ?? 0;
                int page = input.Page ?? 1;
                if (ServiceID <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }

                string token = input.Token ?? string.Empty;
                string content = input.Content ?? string.Empty;
                token = !String.IsNullOrEmpty(token) ? token : string.Empty;
                content = !String.IsNullOrEmpty(content) ? content.Trim() : string.Empty;

                if (String.IsNullOrEmpty(token))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UnAuthorized
                    };
                }


                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(token, out response);
                if (response != 1 || account == null)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                int TotalRecord = 0;
                var list = MessageDAO.Instance.GetList(ServiceID, account.ID, content, page, PageSize, out TotalRecord);
                list = list.OrderBy(c => c.SentDate).ToList();
                return new
                {
                    ResponseCode = 1,
                    TotalRecord = TotalRecord,
                    Page = page,
                    PageSize = PageSize,
                    TotalPage = ConvertUtil.ToInt(TotalRecord / PageSize) + 1,
                    List = list

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
        [ActionName("GetHomeMessage")]
        [Route("GetHomeMessage")]
        [HttpOptions, HttpPost]
        public dynamic GetHomeMessage([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                int ServiceID = input.ServiceID ?? 0;


                string token = input.Token ?? string.Empty;
                string content = input.Content ?? string.Empty;
                token = !String.IsNullOrEmpty(token) ? token : string.Empty;
                content = !String.IsNullOrEmpty(content) ? content.Trim() : string.Empty;

                if (String.IsNullOrEmpty(token))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UnAuthorized
                    };
                }


                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(token, out response);
                if (response != 1 || account == null)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var list = MessageDAO.Instance.OTPSafeMessageLastest(account.ID);
                list = list.OrderBy(c => c.SentDate).ToList();
                return new
                {
                    ResponseCode = 1,

                    List = list

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

        /// <summary>
        /// Get List Message
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("ClearHistory")]
        [Route("ClearHistory")]
        [HttpOptions, HttpPost]
        public dynamic ClearHistory([FromBody] dynamic input)
        {
            try
            {



                string token = input.Token ?? string.Empty;

                token = !String.IsNullOrEmpty(token) ? token.Trim() : string.Empty;

                int ServiceID = input.ServiceID ?? 0;
                if (ServiceID <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }

                if (String.IsNullOrEmpty(token))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(token, out response);
                if (account == null || response != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UnAuthorized
                    };
                }


                response = 0;
                MessageDAO.Instance.OTPSafeMessageClear(ServiceID, account.ID, out response);
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.MessageClearSuccess
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.MessageClearFail
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


        [ActionName("DeleteMesage")]
        [Route("DeleteMesage")]
        [HttpOptions, HttpPost]
        public dynamic DeleteMesage([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string token = input.Token ?? string.Empty;

                token = !String.IsNullOrEmpty(token) ? token : string.Empty;



                if (String.IsNullOrEmpty(token))
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(token, out response);
                if (account == null || response != 1)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                int MessageID = input.MessageID ?? 0;
                if (MessageID <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                response = 0;
                MessageDAO.Instance.OTPSafeMessageDelete(MessageID, account.ID, out response);
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.MessageClearSuccess
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.MessageClearFail
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



        [ActionName("UpdateReadingMesage")]
        [Route("UpdateReadingMesage")]
        [HttpOptions, HttpPost]
        public dynamic UpdateReadingMesage([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string token = input.Token ?? string.Empty;
                token = !String.IsNullOrEmpty(token) ? token : string.Empty;
                string ServiceID = input.ServiceID ?? string.Empty;
                ServiceID = !String.IsNullOrEmpty(ServiceID) ? ServiceID : string.Empty;
                if (String.IsNullOrEmpty(token))
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                var intServiceID = ConvertUtil.ToInt(ServiceID);
                if (String.IsNullOrEmpty(ServiceID) || intServiceID <= 0)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }

                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(token, out response);
                if (account == null || response != 1)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }


                response = 0;
                string UnRead = string.Empty;
                MessageDAO.Instance.OTPSafeMessageCheck(intServiceID, true, account.ID, out UnRead);
                Dictionary<string, string> lst = new Dictionary<string, string>();
                var arrSpit = UnRead.Split(';');
                foreach (var item in arrSpit)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        var arr = item.Split('-');
                        lst.Add(arr[0], arr[1]);
                    }

                }

                return new
                {
                    ResponseCode = 1,
                    ListCount = lst,
                    Message = ErrorMsg.MessageClearSuccess
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

        [ActionName("TotalMessage")]
        [Route("TotalMessage")]
        [HttpOptions, HttpPost]
        public dynamic TotalMessage([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                string token = input.Token ?? string.Empty;
                token = !String.IsNullOrEmpty(token) ? token : string.Empty;



                int response = 0;

                var account = AccountDAO.Instance.GetAccountByToken(token, out response);
                if (account == null || response != 1)
                {
                    return new
                    {
                        ResponseCode = -1001,
                        Message = ErrorMsg.UnAuthorized
                    };
                }


                response = 0;
                string UnRead = string.Empty;
                MessageDAO.Instance.OTPSafeMessageCheck(0, false, account.ID, out UnRead);
                var arrSpit = UnRead.Split(';');
                Dictionary<string, string> lst = new Dictionary<string, string>();
                foreach (var item in arrSpit)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        var arr = item.Split('-');
                        lst.Add(arr[0], arr[1]);
                    }

                }

                return new
                {
                    ResponseCode = 1,
                    ListCount = lst,

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
