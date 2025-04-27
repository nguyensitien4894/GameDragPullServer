using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.CSKH.Utils
{
    public static class MessageConvetor
    {
        public static class MsgGiftCode
        {
            public static string GetMessageGiftCodeAdd(int type)
            {
                string Description = string.Empty;


                switch (type)
                {
                    case -17:
                        Description = Message.CampaignExist;
                        break;
                    case -303:
                        Description = Message.CampaignInvalid;
                        break;
                    
                    default:
                        Description = Message.SystemProccessing;
                        break;
                }
                return Description;
            }
        }
        /// <summary>
        /// tranfer money to agency
        /// </summary>
        public static class MsgAgencyTranferMoney
        {

          
            public static string GetMessage(int type)
            {
                string Description = string.Empty;


                switch (type)
                {
                    case -302:
                        Description = Message.AgencyNotExist;
                        break;
                    case -303:
                        Description = Message.AgencyDisabled;
                        break;
                    case -105:
                        Description = Message.AccountInvalid;
                        break;
                    case -305:
                        Description = Message.AgencyLock;
                        break;
                    case -306:
                        Description = Message.AgencyStatusInvalid;
                        break;
                    case -307:
                        Description = Message.AgencyNotExist;
                        break;
                    case -301:
                        Description = Message.AdminNotExist;
                        break;
                    case -308:
                        Description = Message.AgencyLevelInvalid;
                        break;
                    case -501:
                        Description = Message.AdminAmountInvalid;
                        break;
                    case -502:
                        Description = Message.WalletInvalid;
                        break;
                    default:
                        Description = Message.SystemProccessing;
                        break;
                }
                return Description;
            }
        }
        /// <summary>
        /// agency create
        /// </summary>
        public static class MsgAgencyCreate
        {
            public static string GetMessage(int outResponse)
            {
                string message;
                if (outResponse == -102)
                {
                    message = (ErrorMsg.PhoneInUse);

                }
                else if (outResponse == -103)
                {
                    message = (ErrorMsg.EmailInUse);

                }
                else if (outResponse == -104)
                {
                    message = (ErrorMsg.AgencyCodeInUse);

                }
                else if (outResponse == -105)
                {
                    message = (ErrorMsg.AgencySecretInUse);

                }
                else if (outResponse == -106)
                {
                    message = (ErrorMsg.AgencyFBLinkInUse);
                }
                else if (outResponse == -107)
                {
                    message = (ErrorMsg.AgencyFBLinkInUse);
                }
                else if (outResponse == -306)
                {
                    message = (ErrorMsg.AgencyExist);
                }
                else if (outResponse == -303)
                {
                    message = (ErrorMsg.AgencyDisable);
                }
                else
                {
                    message = (Message.SystemProccessing);
                }
                return message;
            }
        }
        /// <summary>
        /// agency edit
        /// </summary>
        public static class MsgAgencyEdit
        {
            public static string GetMessage(int outResponse)
            {
                string message;
                if (outResponse == -102)
                {
                    message = (ErrorMsg.PhoneInUse);

                }
                else if (outResponse == -103)
                {
                    message = (ErrorMsg.EmailInUse);

                }
                else if (outResponse == -104)
                {
                    message = (ErrorMsg.AgencyCodeInUse);

                }
                else if (outResponse == -105)
                {
                    message = (ErrorMsg.AgencySecretInUse);

                }
                else if (outResponse == -106)
                {
                    message = (ErrorMsg.AgencyFBLinkInUse);
                }
                else if (outResponse == -107)
                {
                    message = (ErrorMsg.AgencyFBLinkInUse);
                }
                else if (outResponse == -306)
                {
                    message = (ErrorMsg.AgencyExist);
                }
                else if (outResponse == -303)
                {
                    message = (ErrorMsg.AgencyDisable);
                }
                else
                {
                    message = (Message.SystemProccessing);
                }
                return message;
            }
        }
        /// <summary>
        /// create customer support
        /// </summary>
        public static class MsgCustomerSupportCreate
        {
            public static string GetMessage(int type)
            {
                string msg = string.Empty;
                if (type == -201)
                {
                    msg = (Message.CSKHExist);


                }
                else
                {
                    msg = (Message.SystemProccessing);

                }
                return msg;
            }
        }
        /// <summary>
        /// tranfer admin to user
        /// </summary>
        public static class MsgAdminTranferToUser
        {
            public static string GetMessage(int type)
            {
               

                string Description = string.Empty;
                switch (type)
                {
                    case -105:
                        Description = Message.AccountInvalid;
                        break;
                    
                    case -301:
                        Description = Message.AdminNotExist;
                        break;
                    case -302:
                        Description = Message.AgencyNotExist;
                        break;
                    case -303:
                        Description = Message.AgencyDisabled;
                        break;
                    
                    case -305:
                        Description = Message.AgencyLock;
                        break;
                    case -306:
                        Description = Message.AgencyStatusInvalid;
                        break;
                    case -307:
                        Description = Message.AgencyNotExist;
                        break;
                    case -308:
                        Description = Message.AgencyLevelInvalid;
                        break;
                    case -501:
                        Description = Message.AdminAmountInvalid;
                        break;
                    case -504:
                        Description = Message.WalletInvalid;
                        break;
                    case -507:
                        Description = Message.TransasactionCouldNotCreate;
                        break;
                  
                    default:
                        Description = Message.SystemProccessing;
                        break;
                }
                return Description;
            }
        }

        public static class MsgLuckySpin
        {
            public static string GetMessage(int responseStatus)
            {
                string Description = string.Empty;
                switch (responseStatus)
                {
                    case -100:
                        Description = Message.LuckySpin_ErrQuantity;
                        break;
                    case -101:
                        Description = Message.LuckySpin_ErrEmptyDate;
                        break;
                    case -102:
                        Description = Message.LuckySpin_ErrStartGreatEnd;
                        break;
                    case -103:
                        Description = Message.LuckySpin_ErrSave;
                        break;
                    case -104:
                        Description = Message.LuckySpin_ErrExistConfigPresent;
                        break;
                    default:
                        Description = Message.SystemProccessing;
                        break;
                }
                return Description;
            }
        }

        public static class MsgLuckyDice
        {
            public static string GetEventMessage(int responseStatus)
            {
                string Description = string.Empty;
                switch (responseStatus)
                {
                    case -1:
                        Description = Message.LD_Event_EndTimeLessStartTime;
                        break;
                    case -2:
                        Description = Message.LD_Event_PrizeMaxLessPrizeMin;
                        break;
                    case -3:
                        Description = Message.LD_Event_ExistRecord;
                        break;
                    default:
                        Description = Message.SystemProccessing;
                        break;
                }
                return Description;
            }
        }
    }
}