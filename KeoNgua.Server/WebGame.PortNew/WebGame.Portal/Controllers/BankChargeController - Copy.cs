using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.MyUSDT.Charges;
using TraditionGame.Utilities.MyUSDT.Models.Charges;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/BankChargeCopy")]
    public class BankChargeCopyController : BaseApiController
    {
        protected int RequestType = 1;
        private int USDTPartnerID = 1;

        /// <summary>
        /// Lấy danh sách banks
        /// </summary>
        /// <returns></returns>
        //[ActionName("GetChargeConfigs")]
        //[HttpGet]
        //public dynamic GetChargeConfigs()
        //{
        //    try
        //    {
        //        var accountId = AccountSession.AccountID;

        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        //Lấy tỉ lệ quy đổi 
        //        double rate = 0;
        //        var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
        //        if (bankOperator == null || !bankOperator.Any())
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Không tìm được cấu hình tỉ giá",
        //            };
        //        }
        //        var firstBanks = bankOperator.FirstOrDefault();
        //        if (firstBanks == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Không tìm được cấu hình tỉ giá",
        //            };
        //        }
        //        rate = firstBanks.Rate;

        //        var banks = BankChargeApiApi.GetBankCharge();
        //        var objReturn = banks.Select(c => new
        //        {
        //            c.BankName,
                    
                  

        //        });
        //        //Lấy tỉ giá min max
        //        string minValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
        //        var min = ConvertUtil.ToLong(minValue);
        //        string maxValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
        //        var max = ConvertUtil.ToLong(maxValue);
        //        return new
        //        {
        //            ResponseCode = 1,
        //            Banks = objReturn,
        //            Rate = rate,
        //            Min=min,
        //            Max=max,
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Description = "Hệ thống đang bận, vui lòng quay lại sau"
        //        };
        //    }
        //}
        ///// <summary>
        ///// Lấy danh sách banks
        ///// </summary>
        ///// <returns></returns>
        //[ActionName("CreateBuyOrders")]
        //[HttpPost]
        //public dynamic CreateBuyOrders([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
        //        if (isOption)
        //        {
        //            return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
        //        }
        //        var accountId = AccountSession.AccountID;

        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
        //        if (account == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (account.Status != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountLock
        //            };
        //        }
        //        string privateKey = input.PrivateKey ?? string.Empty;//lấy ra privte key
        //        string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
        //        if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
        //        {

        //            return new
        //            {
        //                ResponseCode = -4,
        //                Message = ErrorMsg.CapchaRequired
        //            };
        //        }
        //        else
        //        {
        //            if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
        //            {
        //                return new
        //                {
        //                    ResponseCode = -100,
        //                    Message = ErrorMsg.InValidCaptCha
        //                };
        //            }


        //        }
        //        string minValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
        //        var min = ConvertUtil.ToLong(minValue);
        //        string maxValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
        //        var max = ConvertUtil.ToLong(maxValue);

        //        string Amount = input.Amount ?? string.Empty; //Tiền VND
        //        var lngAmount = ConvertUtil.ToLong(Amount);
        //        if (lngAmount <= 0)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format("Số tiền không hợp lệ {0}", lngAmount),
        //            };
        //        }
        //        if (lngAmount < min)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format(ErrorMsg.MinAmountTranfer, min.LongToMoneyFormat()),
        //            };
        //        }
        //        if (lngAmount > max)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format("Số tiền tối đa được giao dịch {0}", max.LongToMoneyFormat()),
        //            };
        //        }
        //        string BankName = input.BankName ?? string.Empty;
        //        if (String.IsNullOrEmpty(BankName))
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = "Bạn chưa chọn ngân hàng",
        //            };
        //        }
        //        //lấy ra rate  trong dbs 
        //        double rate = 0;
        //        var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
        //        if (bankOperator == null || !bankOperator.Any())
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //            };
        //        }
              
        //        var firstBanks = bankOperator.FirstOrDefault();
        //        if (firstBanks == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //            };
        //        }
        //        if (!firstBanks.Status)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //            };
        //        }
        //        rate = firstBanks.Rate;
        //        if (rate <= 0)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Không tìm được cấu hình tỉ giá",
        //            };
        //        }
        //        ///Lấy ra phí cấu hình
        //        var feeObjet = BankChargeApiApi.GetRateConfig();
        //        if (feeObjet == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Hệ thống kết nối ngân hàng lỗi",
        //            };

        //        }
        //        double fee = feeObjet.DepositFeeFlat + feeObjet.DepositFeePct;
        //        var dbMoney = Math.Round(lngAmount * rate);
        //        var AmountReceived = Convert.ToInt64(dbMoney);//Tiền Game
        //        var description = String.Format("User:{0} nạp {1}",displayName, lngAmount);
               
        //        int Response;
        //        long RemainBalance;
        //        long RequestID;
        //        long TransID;
        //        //Khởi tạo order trong dbs 
        //        int PartnerID = 1;
        //        USDTDAO.Instance.UserBankRequestCreate(RequestType, accountId, lngAmount , 0, AmountReceived, PENDING_STATUS, PartnerID
        //            , ServiceID, description,null,null,null,rate, out Response, out RemainBalance, out RequestID, out TransID);
        //        if (Response != 1|| RequestID<=0)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Không khởi tạo được order .Xin quay lại sau ",
        //            };
        //        }
        //        var ordersApi = BankChargeApiApi.SendBuyOrder(lngAmount);
              
        //        LoggerHelper.LogUSDTMessage(String.Format("BankChargeApiApi.SendBuyOrder Amount:{0}|AccountID {1}|Result:{2}", lngAmount, accountId, JsonConvert.SerializeObject(ordersApi, Formatting.None)));
        //        if (ordersApi == null||String.IsNullOrEmpty(ordersApi.Code))
        //        {
        //            USDTDAO.Instance.UserBankRequestUpdate(RequestID,null, accountId, PartnerID, ServiceID,FAIL_API1, PENDING, null,null, null,null, null, null, null, null, null, null, null, null, null, description, null, null, null, null,null, out Response);
        //            return new
        //            {
        //                ResponseCode =-99,
        //                Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau",
        //            };
        //        }
        //        if (String.IsNullOrEmpty(ordersApi.Code)||ordersApi.Status!=PENDING)
        //        {
        //            return new
        //            {
        //                ResponseCode = -99,
        //                Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau",
        //            };
        //        }

        //        //cập nhật tiếp các thông tin cơ bản  order trong dbs

        //        //trả về thông tin cho người dùng
        //        var acceptBanks = ordersApi.Note.Banks.FirstOrDefault(c => c.BankName == BankName);
        //        if (acceptBanks == null)
        //        {
        //            acceptBanks = ordersApi.Note.Banks.FirstOrDefault();
        //            if (acceptBanks == null)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -99,
        //                    Message = "Hệ thống kết nối ngân hàng lỗi.Không tìm thấy tài khoản bank thích hợp tại " + BankName,
        //                };
        //            }

        //        }

        //        //cập nhật lại thông tin order 

        //        USDTDAO.Instance.UserBankRequestUpdate(RequestID, ordersApi.Code,accountId, PartnerID, ServiceID, APPROVED_STATUS, PENDING,null,ordersApi.Rate,null,ordersApi.Amount,null,null, acceptBanks.BankName,acceptBanks.MasterBankName,acceptBanks.MasterBankAccount,((int)ordersApi.HttpStatusCode).ToString(),null,ordersApi.HttpMsg,null,description,null,null,null, ordersApi.Note.ReceiveUsdtAddress,fee,out Response);
        //        if (Response != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -99,
        //                Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau",
        //            };
        //        }
            
        //        return new
        //        {
        //            ResponseCode = 1,
        //            Orders = new
        //            {
        //                Amount= ordersApi.AmountVcc,//Số tiền
        //                Code= ordersApi.Code,//mã order
        //                Timeout=ordersApi.Timeout,//Thời gian hết hạn của order
        //                Remain=ordersApi.Remaining,
        //                AmountReceived= AmountReceived,
        //                Banks =new
        //                {
        //                    BankName= acceptBanks.BankName,//Tên ngân hàng như vietcombank,bidv
        //                    MasterBankAccount = acceptBanks.MasterBankAccount,//Số tài khoản cần chuyển vào
        //                    MasterBankName= acceptBanks.MasterBankName//Tên chủ tài khoản cần chuyển tiền vào
        //                }
        //            }
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Description = "Hệ thống đang bận, vui lòng quay lại sau"
        //        };
        //    }
        //}
        ///// <summary>
        ///// call back khi xử lý
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[ActionName("CallBackResult")]
        //[HttpPost]
        //public dynamic CallBackResult([FromBody] CallBackBuyOrderRequest model)
        //{
           
        //    if (model == null)
        //    {
        //        return new
        //        {
        //            ResponseCode = -1005,
        //            Message = ErrorMsg.ParamaterInvalid
        //        };
        //    }
        //    LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBackResult CallBackModel:{0}", JsonConvert.SerializeObject(model, Formatting.None)));
        //    var code = model.Code;
        //    if (String.IsNullOrEmpty(code))
        //    {
        //        return new
        //        {
        //            ResponseCode = -1006,
        //            Message = "Mã code không thể trống"
        //        };
        //    }
          
        //    var accepStatus = new List<string>() {  "completed", "canceled", "failed" };
        //    if (!accepStatus.Contains(model.Status.ToLower()))
        //    {
        //        var obj = new
        //        {
        //            ResponseCode = -1007,
        //            Message = "Mã code không nằm trong dãy ( completed, canceled,failed)"
        //        };
        //        HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
        //        return response;
                
        //    }
        //    if(model.Method.ToLower()!="buy")
        //    {
        //        return new
        //        {
        //            ResponseCode = -1008,
        //            Message = "Phương thức gửi không phải là buy"
        //        };
        //    }
        //    int TotalRecord = 0;
        //    //1.lấy thông tin order theo code
        //    var orderByCode = USDTDAO.Instance.UserBankRequestList(null, null, code, null, null, null, ServiceID, 1, 1, out TotalRecord);
        //    if (orderByCode == null || !orderByCode.Any())
        //    {
        //        return new
        //        {
        //            ResponseCode = -1005,
        //            Message = String.Format("Không tồn tại giao dịch theo mã {0}", code),
        //        };
        //    }
        //    var firstOrderCode = orderByCode.FirstOrDefault();
        //    if (firstOrderCode == null)
        //    {
        //        return new
        //        {
        //            ResponseCode = -1005,
        //            Message = String.Format("Không tồn tại giao dịch theo mã {0}", code),
        //        };
        //    }
        //    if (firstOrderCode.PartnerStatus != PENDING)
        //    {
        //        return new
        //        {
        //            ResponseCode = -1006,
        //            Message = String.Format("Đơn hàng {0} đã được cập nhật trạng thái {1}",code, firstOrderCode.PartnerStatus),
        //        };
        //    }
        //    if (firstOrderCode.Status != APPROVED_STATUS)
        //    {
        //        return new
        //        {
        //            ResponseCode = -1006,
        //            Message = String.Format("Đơn hàng {0} đã được cập nhật trạng thái {1}", code, firstOrderCode.Status),
        //        };
        //    }

        //    int Response;
        //    long RemainBalance;
        //    var Status = MappingStatus(model.Status.ToLower());
        //    if (model.Status.ToLower()== COMPLETED)
        //    {
        //        var dbMoney = Math.Round((double)(model.AmountVcc * firstOrderCode.Rate));
        //        var realAmountReceive =Convert.ToInt64 (dbMoney);
        //        //1.Cộng tiền cho khách và cập nhật order
        //        USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Success, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, realAmountReceive, model.AmountVcc, realAmountReceive, model.Status, out RemainBalance, out Response);

        //        LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBack.Completed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount:{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus:{10}|RemainBalance:{11}| Response:{12}",
        //            firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Success, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, realAmountReceive, model.AmountVcc, realAmountReceive, model.Status,  RemainBalance,  Response
        //            ));
        //        if (Response == 1)
        //        {
        //            try
        //            {
        //                var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
        //                dnaHelper.SendTransactionPURCHASE(firstOrderCode.UserID,7, null, model.AmountVcc, model.AmountVcc);
        //                LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", firstOrderCode.UserID, 7, model.AmountVcc, model.AmountVcc, firstOrderCode.RequestID));
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                NLogManager.PublishException(ex);
        //                return false;

        //            }
        //        }

        //        var obj = new
        //        {
        //            ResponseCode = Response,
        //            Message = String.Format("Đơn hàng {0} được cập nhật trạng thái Success {1}", code, Response),
        //        };
        //        HttpResponseMessage response = Request.CreateResponse<Object>( HttpStatusCode.OK, obj);
        //        return response;
        //    }
        //    else
        //    {
        //        var dbMoney = model.AmountVcc * firstOrderCode.Rate;
        //        var realAmountReceive = Convert.ToInt64(dbMoney);
        //        USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID,firstOrderCode.UserID,Bank_Fail,ServiceID,model.AmountUsdt,model.Rate, CHECKER_ID, 0,model.AmountVcc, realAmountReceive, model.Status,out RemainBalance,out Response);
        //        //cập nhật lại trạng thái của order 
        //        LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBack.Failed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount:{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus:{10}|RemainBalance:{11}| Response:{12}",
        //           firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Fail, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, 0, model.AmountVcc, realAmountReceive, model.Status,  RemainBalance,  Response
        //           ));

        //        var obj = new
        //        {
        //            ResponseCode = Response,
        //            Message = String.Format("Đơn hàng {0} được cập nhật trạng thái Fail {1}", code,Response),
        //        };
        //        HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
        //        return response;
        //    }
            
        //}

        ///// <summary>
        ///// Lấy thông tin chi tiếp đơn hàng thông qua code
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[ActionName("OrderDetails")]
        //[HttpPost]
        //public dynamic OrderDetails([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
        //        if (isOption)
        //        {
        //            return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
        //        }
        //        var accountId = AccountSession.AccountID;

        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
        //        if (account == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (account.Status != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountLock
        //            };
        //        }

        //        string Code = input.Code ?? string.Empty;
        //        int  TotalRecord =0;
        //        //1.Lấy ra order details từ dbs (accountId,orderCode)
        //        var orderByCode = USDTDAO.Instance.UserBankRequestList(null, accountId, Code, null, null, null, ServiceID, 1, 1, out TotalRecord);
        //        if(orderByCode==null|| !orderByCode.Any())
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message =String.Format( "Không tồn tại giao dịch theo mã {0}",Code),
        //            };
        //        }
        //        var firstOrderCode = orderByCode.FirstOrDefault();
        //        if (firstOrderCode == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format("Không tồn tại giao dịch theo mã {0}", Code),
        //            };
        //        }
        //        if (firstOrderCode.UserID != accountId)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format("Tài khoản không đủ quyền để xem mã giao dịch {0}", Code),
        //            };
        //        }
        //        //khởi tạo order trong api
        //        var ordersApi = BankChargeApiApi.GetOrderDeails(Code);
        //        if (ordersApi == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = -99,
        //                Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau",
        //            };
        //        }
        //        var acceptBanks = ordersApi.Note.Banks.FirstOrDefault();
        //        return new
        //        {
        //            ResponseCode = 1,
        //            Orders = new
        //            {
        //                Amount = ordersApi.AmountVcc,//Số tiền

        //                Code = ordersApi.Code,//mã order
        //                Timeout = ordersApi.Timeout,//Thời gian hết hạn của order
        //                AmountReceived = firstOrderCode.ReceivedMoney,
        //                Status= firstOrderCode.Status,
        //                StatusStr= MappingStatuStr(firstOrderCode.Status,firstOrderCode.RequestType),
        //                Banks = new
        //                {
        //                    BankName = acceptBanks.BankName,//Tên ngân hàng như vietcombank,bidv
        //                    MasterBankAccount = acceptBanks.MasterBankAccount,//Số tài khoản cần chuyển vào
        //                    MasterBankName = acceptBanks.MasterBankName//Tên chủ tài khoản cần chuyển tiền vào
        //                }

        //            }
        //        };






        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Description = "Hệ thống đang bận, vui lòng quay lại sau"
        //        };
        //    }
        //}


        //[ActionName("GetHistory")]
        //[HttpGet]
        //public dynamic GetHistory()
        //{
        //    try
        //    {

        //        var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
        //        if (isOption)
        //        {
        //            return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
        //        }
        //        var accountId = AccountSession.AccountID;

        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
        //        if (account == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (account.Status != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountLock
        //            };
        //        }
        //        int TotalRecord = 0;
        //        var list = USDTDAO.Instance.UserBankRequestList(null, accountId,null, null, null, null, ServiceID,1,50, out TotalRecord);
        //        var objReturn = list.Select(c => new
        //        {
        //            RequestID = c.RequestID,
        //            StatusStr = MappingStatuStr(c.Status,c.RequestType),
                   
        //            RequestType=c.RequestType,
        //            Type = c.RequestType == 1 ? "Nạp tiền" : "Rút tiền ",
        //            Code = c.RequestCode,
        //            RequestDate = c.RequestDate,
        //            Status=c.Status,
        //            AmountGame = c.RequestType == 1 ? ((c.Status==SUCCESS_STATUS)?c.RealReceivedMoney: c.ReceivedMoney) : c.Amount,
                    

        //        });
        //        return new
        //        {
        //            ResponseCode = 1,
        //            List = objReturn
        //        };
        //    }
        //    catch(Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Description = "Hệ thống đang bận, vui lòng quay lại sau"
        //        };
        //    }
        //}
    }
}
