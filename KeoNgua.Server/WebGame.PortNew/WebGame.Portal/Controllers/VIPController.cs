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
    [RoutePrefix("api/VIP")]
    public class VIPController : BaseApiController
    {
        /// <summary>
        /// Lấy danh sách quà tặng
        /// </summary>
        /// <returns></returns>
        //[ActionName("GetVPToGift")]
        //[HttpGet]
        //public dynamic GetVPToGift()
        //{
        //    try
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

        //        var user = AccountDAO.Instance.GetProfile(accountId);
        //        if (user == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = -2,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        int Response = 0;
        //        var userRank = AccountDAO.Instance.GetUserRank(accountId, out Response);
        //        if (userRank == null)
        //        {

        //            return new
        //            {
        //                ResponseCode = -1005,
        //                RefundLimit =0,

        //            };

        //        }
        //        long RefundLimit = 0;//số tiền quy đổi (cần số này để hiển thị cho khách hàng
        //        long GiftAmountLimit = 0;
        //        long ExchangeRate = 0;
        //        UserPrivilegeDAO.Instance.UserPrivilegeRedeemVP(accountId, userRank.RankID, userRank.VP, out RefundLimit, out GiftAmountLimit, out ExchangeRate, out Response);
        //        //int TotalRecord = 0;
        //        //var listAtifacts = UserPrivilegeDAO.Instance.PrivilegeArtifacts_List(null, userRank.RankID, null, null, 1, Int32.MaxValue, out TotalRecord);
        //        //if (listAtifacts == null && !listAtifacts.Any())
        //        //{
        //        //    return new
        //        //    {
        //        //        ResponseCode = -1005,
        //        //        Message = "Quỹ quà tặng đã hết"
        //        //    };
        //        //}
        //        return new
        //        {
        //            ResponseCode = 1,
        //            //listAtifacts = listAtifacts,//--đây là quỹ quà tặng,
        //            RefundLimit = RefundLimit//--đây là số tiền  có thể quy đổi
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.InProccessException
        //        };
        //    }

        //}
        /// <summary>
        /// đổi quà và tiền
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[ActionName("ChangeVPToGif")]
        //[HttpPost]
        //public dynamic ChangeVPToGif([FromBody]dynamic input)
        //{
        //    try
        //    {

        //        string APPROVE = ConfigurationManager.AppSettings["VP_APPROVED"].ToString();
        //        if (APPROVE != "1")
        //        {
        //            return AnphaHelper.Close();
        //        }
                
        //        long accountId = AccountSession.AccountID;
        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }

        //        var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
        //        if (user == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (user.Status != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountLock
        //            };
        //        }

        //        string RankID = input.RankID ?? string.Empty;

        //        if ((String.IsNullOrEmpty(RankID)))
        //        {

        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.ParamaterInvalid
        //            };
        //        }

        //        var intRankID = ConvertUtil.ToInt(RankID);//ranh muôn đổi 
        //        if (intRankID != 1 && intRankID != 2 && intRankID != 3 && intRankID != 4 && intRankID != 0)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.ParamaterInvalid
        //            };
        //        }
              

        //        int Response = 0;
              
        //        int rankResponse = 0;
        //        var currentRank = 5;
        //        var userRank = AccountDAO.Instance.GetUserRank(accountId, out rankResponse);
        //        if (userRank == null)
        //        {

        //            currentRank = (int)userRank.RankID;

        //         }
        //        //nếu ranh muốn đổi lớn hơn ranh hiện tại báo lỗi  (quy ước ranh 0 là lớn nhất ,5 là nhỏ nhất nên làm củ chuối )
        //        if(intRankID> currentRank)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.UserRankNotVerify
        //            };
        //        }

               

        //       long UserRedemptionID = 0;

        //        string Description = String.Format("Nhận thưởng VP");
        //        long RefundAmount = 0;
        //        long RemainBalance = 0;
        //        UserPrivilegeDAO.Instance.UserPrivilegeGratitude(accountId, intRankID, Description,ServiceID, out UserRedemptionID, out RefundAmount,out RemainBalance, out Response);
               
              
        //        if (Response == 1)
        //        {
                   
                    
        //            //int outRes = 0;
        //           // UserPrivilegeDAO.Instance.UserRedemption(accountId, intRankID, Description, RefundAmount, out RemainBalance, out outRes);
        //            //if (outRes == 1)
        //            //{
        //                return new
        //                {
        //                    ResponseCode = 1,
        //                    Message = ErrorMsg.VPSuccess,

        //                    Balance = RemainBalance
        //                };
        //            //}else
        //            //{
        //            //    return new
        //            //    {
        //            //        ResponseCode = -99,
        //            //        Message = ErrorMsg.VPFail,

                           
        //            //    };
        //            //}
                    
        //        }
        //        else if (Response == -212)
        //        {
        //            return new
        //            {
        //                ResponseCode = -212,
        //                Message = ErrorMsg.UserNotActive
        //            };
        //        }
        //        else if (Response == -223)
        //        {
        //            return new
        //            {
        //                ResponseCode = -223,
        //                Message = ErrorMsg.UserNotRanked
        //            };
        //        }
        //        else if (Response == -224)
        //        {
        //            return new
        //            {
        //                ResponseCode = -224,
        //                Message = ErrorMsg.UserChangeVP
        //            };
        //        }
        //        else
        //        {
        //            return new
        //            {
        //                ResponseCode = -99,
        //                Message = ErrorMsg.VPFail
        //            };
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.InProccessException
        //        };
        //    }
        //}
        [ActionName("GetHistory")]
        [HttpGet]
        public dynamic GetHistory()
        {
            try
            {
                long accountId = AccountSession.AccountID;
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
                var list = UserPrivilegeDAO.Instance.UserRedemptionHistoryList(0, accountId, null, null,ServiceID, 1, 100, out TotalRecord);
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
            return null;
        }
        /// <summary>
        /// Lấy danh sách hạng
        /// </summary>
        /// <returns></returns>
        [ActionName("GetPrivilegeType")]
        [HttpGet]
        public dynamic GetPrivilegeType()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var list = UserPrivilegeDAO.Instance.PrivilegeTypeList(null);

                return new
                {
                    ResponseCode = 1,
                    List = list,//--đây là quỹ quà tặng,

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


        /// <summary>
        /// lấy ra mã bảng vp hiện tại của user
        /// </summary>
        /// <returns></returns>
        [ActionName("GetUserVP")]
        [HttpGet]
        public dynamic GetUserVP()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var list = UserPrivilegeDAO.Instance.UserRedemptionPrize(accountId);
                long VP = 0;
                int outResponse = 0;
                var userRank = AccountDAO.Instance.GetUserRank(accountId, out outResponse);
                if (userRank != null)
                {

                    VP = userRank.VP;

                }

                return new
                {
                    ResponseCode = 1,
                    List = list,//--đây là quỹ quà tặng,
                    VP = VP,

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