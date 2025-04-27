using System;
using System.Web.Http;

using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Utils;
using System.Linq;
using MsWebGame.Portal.Database.DTO.EventBigBom;
using System.Collections.Generic;

namespace MsWebGame.Portal.Controllers
{
    //[RoutePrefix("api/BomOne")]
    //public class BomOneController : ApiController
    //{
    //    [HttpOptions, HttpGet]
    //    [Route("Bom1UserGetB1P")]
    //    public dynamic Bom1UserGetB1P()
    //    {
    //        try
    //        {
    //            //kiểm tra lại cách lấy accountId
    //            var accountId = AccountSession.AccountID;
    //            if (accountId <= 0)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.UnAuthorized
    //                };
    //            }
              
    //            var user = AccountDAO.Instance.GetProfile(accountId);
    //            if (user == null)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.UnAuthorized
    //                };
    //            }
    //            if (user.Status != 1)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.AccountLock
    //                };
    //            }

    //            var res = Bom1DAO.Instance.Bom1UserGetB1P(accountId);
    //            var lstDegreePrize = Bom1DAO.Instance.GetDegreePrize(accountId);
    //            return new
    //            {
    //                ResponseCode = 1,
    //                Result = res,
    //                LstDegreePrize = lstDegreePrize
    //            };

    //        }
    //        catch (Exception ex)
    //        {
    //            NLogManager.PublishException(ex);
    //        }

    //        return new
    //        {
    //            ResponseCode = -99,
    //            Message = ErrorMsg.InProccessException
    //        };
    //    }


    //    /// <summary>
    //    /// user nhận quà
    //    /// </summary>
    //    /// <param name="input"></param>
    //    /// <returns></returns>
    //    [HttpOptions, HttpPost]
    //    [Route("UserReceive")]
    //    public dynamic UserReceive([FromBody] dynamic input)
    //    {
    //        try
    //        {
    //            //kiểm tra lại cách lấy accountId
    //            var accountId = AccountSession.AccountID;
    //            if (accountId <= 0)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.UnAuthorized
    //                };
    //            }
               
    //            string AidBoxID = input.AidBoxID ?? string.Empty;
              
    //            if (String.IsNullOrEmpty(AidBoxID) )
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.ParamaterInvalid
    //                };

    //            }
                
    //            var intAidBoxID = ConvertUtil.ToLong(AidBoxID);
    //            if (intAidBoxID <= 0)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.ParamaterInvalid
    //                };
    //            }
    //            var bomAd = Bom1DAO.Instance.UserBom1GiftBoxAidGetByID(intAidBoxID);
    //            if (bomAd == null)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.ParamaterInvalid
    //                };
    //            }

               


    //            var user = AccountDAO.Instance.GetProfile(accountId);
    //            if (user == null)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.UnAuthorized
    //                };
    //            }
    //            if (user.Status != 1)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.AccountLock
    //                };
    //            }

    //            int Response = 0;

    //            var intGifType = bomAd.GiftType;
    //            var lngGiftValue = bomAd.GiftValue;

    //            long TransID = 0;
    //            if (intGifType != 2)
    //            {
    //                //gift type khác = 2
    //                Bom1DAO.Instance.Bom1AidBoxUserReceive(intAidBoxID, accountId, out TransID, out Response);
    //                if (Response == 1)
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = 1,
    //                        Message = ErrorMsg.AdBoxSuccess
    //                    };
    //                }
    //                else if (Response == -1)
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = -1005,
    //                        Message = ErrorMsg.AdBoxGiftCodeInValid
    //                    };
    //                }
    //                else
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = -1005,
    //                        Message = ErrorMsg.AdBoxFail
    //                    };
    //                }
    //            }
    //            else
    //            {
    //                //git type =2
    //                Bom1DAO.Instance.Bom1AidBoxUserReceive(intAidBoxID, accountId, out TransID, out Response);
    //                if (Response == 1)
    //                {
    //                    string note = "Đổi quà thưởng";
    //                    long wallet = 0;
    //                    int resUser = Bom1DAO.Instance.UserTransferReceive(9, "Admin",3, 0, accountId, ConvertUtil.ToLong(lngGiftValue??0), note, TransID,out wallet);
    //                    if (resUser == 1)
    //                    {
    //                        return new
    //                        {
    //                            ResponseCode = 1,
    //                            Message = ErrorMsg.AdBoxSuccess,
    //                            Balance= wallet
    //                        };
    //                    }
    //                    else
    //                    {
    //                        return new
    //                        {
    //                            ResponseCode = -1005,
    //                            Message = ErrorMsg.AdBoxFail
    //                        };
    //                    }


    //                }
    //                else if (Response == -1)
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = -1005,
    //                        Message = ErrorMsg.AdBoxGiftCodeInValid
    //                    };
    //                }
    //                else
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = -1005,
    //                        Message = ErrorMsg.AdBoxFail
    //                    };
    //                }

    //            }




    //        }
    //        catch (Exception ex)
    //        {
    //            NLogManager.PublishException(ex);
    //        }

    //        return new
    //        {
    //            ResponseCode = -99,
    //            Message = ErrorMsg.InProccessException
    //        };
    //    }


    //    [HttpOptions, HttpPost]
    //    [Route("BomAddToUser")]
    //    public dynamic BomAddToUser([FromBody] dynamic input)
    //    {
    //        try
    //        {
    //            //kiểm tra lại cách lấy accountId
    //            var accountId = AccountSession.AccountID;
    //            if (accountId <= 0)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.UnAuthorized
    //                };
    //            }

    //            string DegreeID = input.DegreeID ?? string.Empty;

    //            if (String.IsNullOrEmpty(DegreeID))
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.ParamaterInvalid
    //                };

    //            }

    //            var intDegreeID = ConvertUtil.ToInt(DegreeID);

    //            if (intDegreeID <= 0||intDegreeID>9)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.ParamaterInvalid
    //                };
    //            }


    //            var user = AccountDAO.Instance.GetProfile(accountId);
    //            if (user == null)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.UnAuthorized
    //                };
    //            }
    //            if (user.Status != 1)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -2,
    //                    Message = ErrorMsg.AccountLock
    //                };
    //            }

    //            int Response = 0;
              

    //            String Note = "Nhận thưởng mốc";
    //            long RefundAmount = 0;
    //            Bom1DAO.Instance.Bom1UserDegree_Gratitude(intDegreeID, accountId, Note, out Response, out RefundAmount);
    //            if (Response == 1)
    //            {

    //                long RemainBalance = 0;
    //                Bom1DAO.Instance.Bom1UserRedemption(intDegreeID, accountId, RefundAmount, Note, out RemainBalance, out Response);
    //                if (Response == 1)
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = 1,
    //                        Message = ErrorMsg.Bom1AddSuccess,
    //                        Balance= RemainBalance,
    //                    };
    //                }
    //                else if (Response == -105)
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = -1005,
    //                        Message = ErrorMsg.UserNotExist
    //                    };
    //                }
    //                else if (Response == -504)
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = -1005,
    //                        Message = ErrorMsg.AmountNotValid
    //                    };
    //                }
    //                else
    //                {
    //                    return new
    //                    {
    //                        ResponseCode = -1005,
    //                        Message = ErrorMsg.InProccessException
    //                    };
    //                }


    //            }
    //            else if (Response == -212)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -1005,
    //                    Message = ErrorMsg.UserInValid
    //                };
    //            }
    //            else if (Response == -223)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -1005,
    //                    Message = ErrorMsg.UserNotRanked
    //                };
    //            }
    //            else if (Response == -224)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -1005,
    //                    Message = ErrorMsg.Bom1UserReceived
    //                };
    //            }
    //            else if (Response == -37)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -1005,
    //                    Message = ErrorMsg.Bom1OverQuata
    //                };
    //            }
    //            else if (Response == -38)
    //            {
    //                return new
    //                {
    //                    ResponseCode = -1005,
    //                    Message = ErrorMsg.Bom1AdFail
    //                };
    //            }
    //            else
    //            {
    //                return new
    //                {
    //                    ResponseCode = -1005,
    //                    Message = ErrorMsg.InProccessException
    //                };
    //            }






    //        }
    //        catch (Exception ex)
    //        {
    //            NLogManager.PublishException(ex);
    //        }

    //        return new
    //        {
    //            ResponseCode = -99,
    //            Message = ErrorMsg.InProccessException
    //        };
    //    }

    //    [ActionName("GetHistory")]
    //    [HttpGet]
    //    public dynamic GetHistory()
    //    {
    //        long accountId = AccountSession.AccountID;

    //        if (accountId <= 0)
    //        {
    //            return new
    //            {
    //                ResponseCode = -2,
    //                Message = ErrorMsg.UnAuthorized
    //            };
    //        }

    //        try
    //        {
                
    //            var list = Bom1DAO.Instance.Bom1UserAidBoxHistory(accountId);
    //            var result = list.Select(c => new {

    //                UserID=c.UserID,
                  
    //                NickName=c.NickName,
    //                GiftID=c.GiftID,
    //                GiftType=c.GiftType,
    //                GiftValue=c.GiftValue,
    //                TimeFrameID=c.TimeFrameID,
    //                AidDate=c.AidDate,
    //                ReceiveDate=c.ReceiveDate,

    //            });
    //            return new
    //            {
    //                ResponseCode = 1,
    //                List = result
    //            };
    //        }
    //        catch (Exception ex)
    //        {
    //            NLogManager.PublishException(ex);
    //            return new
    //            {
    //                ResponseCode = -99,
    //                List = new List<UserBom1AidBoxes>(),
    //            };
    //        }
    //    }
    //}
}