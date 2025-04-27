using System;
using System.Web.Http;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using System.Configuration;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Telecom")]
    public class TelecomController : BaseApiController
    {
        /// <summary>
        /// Lấy danh sách quà tặng
        /// </summary>
        /// <returns></returns>
        [ActionName("GetList")]
        [HttpGet]
        public dynamic GetList(int type)
        {
            try
            {
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
                if (type != 1 && type != 2)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                var list = CardDAO.Instance.GetTeleComList(null,ServiceID,type);
                
               
                return new
                {
                    ResponseCode = 1,
                   
                    List = list
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }

        }

    }
}