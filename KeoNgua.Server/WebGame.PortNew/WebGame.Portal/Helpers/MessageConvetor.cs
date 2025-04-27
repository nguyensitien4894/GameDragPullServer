using TraditionGame.Utilities.Messages;

namespace MsWebGame.Portal.Helper
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
                else if (response == -5)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.NickNameExist
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