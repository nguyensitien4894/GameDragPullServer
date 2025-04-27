using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Handlers;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/System")]
    public class SystemController : BaseApiController
    {
        [ HttpGet]
        [Route("ChargeDefault")]
        public dynamic ChargeDefault([FromBody] dynamic input)
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
                var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
                if (bankOperator == null || !bankOperator.Any())
                {
                    return new
                    {
                        ResponseCode = 1,
                        Default = "BANK" //CARD OR BANK
                    };
                }
                var firstBanks = bankOperator.FirstOrDefault();
                if (firstBanks == null)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Default = "BANK" //CARD OR BANK
                    };
                }
                if (!firstBanks.Status)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Default = "BANK" //CARD OR BANK
                    };
                }

                return new
                {
                    ResponseCode = 1,
                    Default = "BANK" //CARD OR BANK
                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }


            return new
            {
                ResponseCode = 1,
                Default = "BANK" //CARD OR BANK
            };


        }
        [HttpOptions, HttpPost]
        [Route("UpdateReading")]
        public dynamic UpdateReading([FromBody] dynamic input)
        {
            int response = -99;
            try
            {
                //kiểm tra lại cách lấy accountId
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

                string strMailID = input.MailID ?? string.Empty;
                long MailID = ConvertUtil.ToLong(strMailID);

                MailDAO.Instance.UpdateMailReading(MailID, out response);
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateSuccess
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.InProccessException
                    };
                }



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

        /// <summary>
        /// update user read email
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("UpdateStatus")]
        public dynamic UpdateStatus([FromBody] dynamic input)
        {
            int response = -99;
            try
            {
                //kiểm tra lại cách lấy accountId
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
                string strMailID = input.MailID ?? string.Empty;
                long MailID = Convert.ToInt64(strMailID);
                string Type = input.Type ?? string.Empty;
                int TypeStatus = Convert.ToInt16(Type);
                if (TypeStatus != 2 && TypeStatus != -1)
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                MailDAO.Instance.UpdateStatus(accountId, MailID, TypeStatus, out response);
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateSuccess
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.InProccessException
                    };
                }



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
        /// <summary>
        /// Get user email
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("GetUserMail")]
        public dynamic GetUserMail([FromBody] dynamic input)
        {
            try
            {
                //kiểm tra lại cách lấy accountId
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
                int MailType = Convert.ToInt16(input.MailType);
                int CurrentPage = Convert.ToInt16(input.CurrentPage);
                int PageSize = Convert.ToInt16(input.PageSize);

                if (MailType != 1 && MailType != 2)
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.ParamaterInvalid
                    };

                }

                int TotalRecord = 0;
                var listMail = MailDAO.Instance.GetMail(MailType, CurrentPage, PageSize, out TotalRecord, accountId, ServiceID);


                return new
                {
                    ResponseCode = 1,
                    List = listMail,
                    TotalRecord = TotalRecord

                };


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = 1,
                List = new List<Mail>(),
                TotalRecord = 0

            };
        }

        /// <summary>
        /// get system mail
        /// </summary>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("GetSystemMail")]
        public dynamic GetSystemMail()
        {
            try
            {
                //kiểm tra lại cách lấy accountId
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

                int TotalRecord = 0;
                var listMail = MailDAO.Instance.GetSystemMail(ServiceID);


                return new
                {
                    ResponseCode = 1,
                    List = listMail,
                    TotalRecord = TotalRecord

                };


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = 1,
                List = new List<SystemMail>(),
                TotalRecord = 0

            };
        }


        [HttpGet]
        [Route("MailUnRead")]
        public dynamic MailUnRead()
        {
            try
            {
                //kiểm tra lại cách lấy accountId
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

                int cnt = 0;
                MailDAO.Instance.MailUnReadCnt(accountId, out cnt);
                return new
                {
                    ResponseCode = 1,
                    Count = cnt,


                };


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = 1,

                Count = 0

            };
        }
        /// <summary>
        /// dòng chạy ngang
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNotify")]
        public dynamic GetNotify()
        {
            try
            {
                var lstRs = CachingHandler.GetListCache<BigWiner>("SystemGetNotify", ServiceID);
                if (lstRs != null)
                    return new { ResponseCode = 1, List = lstRs };
                lstRs = NotificationDAO.Instance.GetBigWiner(ServiceID);

                if (lstRs == null)
                    lstRs = new List<BigWiner>();

                CachingHandler.AddListCache<BigWiner>("SystemGetNotify", ServiceID, lstRs, 60);
                return new
                {
                    ResponseCode = 1,
                    List = lstRs,


                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return new
            {
                ResponseCode = 1,
                List = new List<BigWiner>(),


            };
        }



    }
}
