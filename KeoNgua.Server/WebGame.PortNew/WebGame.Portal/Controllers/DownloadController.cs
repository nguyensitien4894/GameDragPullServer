using MsWebGame.Portal.Database.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Download")]
    public class DownloadController : BaseApiController
    {
        [HttpPost]
        [Route("DownloadTracking")]
        public dynamic DownloadTracking([FromBody]dynamic input)
        {

            string Url = input.ReferUrl ?? string.Empty;
            string deviceId = input.DeviceId ?? string.Empty;
            string clientIp = IPAddressHelper.GetClientIP();
            int AppType = input.AppType != null ? Convert.ToInt32(input.AppType.Value) : -1;
            if (!new List<int>() { 1, 2, 3, 4 }.Contains(AppType) || String.IsNullOrEmpty(Url) || String.IsNullOrEmpty(deviceId))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }
            int res = 0;
            UserDAO.Instance.DownLoadTracking(ServiceID, AppType, Url ,clientIp, deviceId, out res);

            return new
            {
                ResponseCode = 1,
                Message = ErrorMsg.UpdateSuccess
            };



        }
    }
}
