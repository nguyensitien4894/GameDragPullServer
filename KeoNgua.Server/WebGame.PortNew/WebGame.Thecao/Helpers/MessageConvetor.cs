using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Thecao.Helper
{
    public static class MessageConvetor
    {
        public static class AccountUpdateNickName
        {
            public static dynamic  GetMsg(int response)
            {
                 if (response == -1)
                {

                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.NameInUse
                    };

                }
                else if (response == -2)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.UpdateFail
                    };
                }

                else if (response == -4)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.NickNameContainUsername
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
        }
       
    }
}