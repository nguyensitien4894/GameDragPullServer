using MsWebGame.SafeOtp.Database.DAO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Messages;
namespace MsWebGame.SafeOtp.Controllers
{
    [RoutePrefix("api/Game")]
    public class GameController : BaseApiController
    {
        [ActionName("GetService")]
        [Route("GetService")]
        [HttpGet]
        public dynamic GetService([FromBody] dynamic input)
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                var list = GameDAO.Instance.GetSerivces();
                return new
                {
                    ResponseCode = 1,
                    List= list

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
