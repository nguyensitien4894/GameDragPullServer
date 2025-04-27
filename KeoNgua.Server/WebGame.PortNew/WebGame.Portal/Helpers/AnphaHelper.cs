using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Portal.Helpers
{
    public class AnphaHelper
    {
        public static dynamic Close()
        {
            return new
            {
                ResponseCode = -1005,
                Message = ErrorMsg.FunctionLock
            };
        }
    }
}