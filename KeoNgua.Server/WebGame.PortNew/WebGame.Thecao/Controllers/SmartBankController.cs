using MsWebGame.Thecao.Database.DAO;
using MsWebGame.Thecao.Models.SmartBanks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/SmartBank")]
    public class SmartBankController : BaseApiController
    {
        private int PartnerID = 2;
        /// <summary>
        /// call back khi xử lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[ActionName("CallBackResult")]
        //[HttpPost]
        //public dynamic CallBackResult([FromBody] SmartBankCallBackModel model)
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
        //    var code = model.RequestCode;
        //    if (String.IsNullOrEmpty(code))
        //    {
        //        return new
        //        {
        //            ResponseCode = -1006,
        //            Message = "Mã code không thể trống"
        //        };
        //    }

        //    var accepStatus = new List<string>() { "completed", "canceled", "failed" };
        //    if (!accepStatus.Contains(model.StatusStr.ToLower()))
        //    {
        //        var obj = new
        //        {
        //            ResponseCode = -1007,
        //            Message = "Mã code không nằm trong dãy ( completed, canceled,failed)"
        //        };
        //        HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
        //        return response;

        //    }
            
        //    int TotalRecord = 0;
        //    //1.lấy thông tin order theo code //chỗ này phải sửa 
        //    var firstOrderCode = USDTDAO.Instance.UserBankRequestGetByCodeFromPartner(PartnerID, code);
           
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
        //            Message = String.Format("Đơn hàng {0} đã được cập nhật trạng thái {1}", code, firstOrderCode.PartnerStatus),
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

        //    int Response=0;
        //    long RemainBalance;
           
        //    if (model.StatusStr.ToLower() == COMPLETED)
        //    {
        //        var dbMoney = Math.Round((double)(model.Amount * firstOrderCode.Rate));
        //        var realAmountReceive = Convert.ToInt64(dbMoney);
        //        //1.Cộng tiền cho khách và cập nhật order
        //        USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Success, ServiceID,0, 0, CHECKER_ID, realAmountReceive,model.Amount,
        //            realAmountReceive,model.StatusStr, out RemainBalance, out Response);

               

        //        var obj = new
        //        {
        //            ResponseCode = Response,
        //            Message = String.Format("Đơn hàng {0} được cập nhật trạng thái Success {1}", code, Response),
        //        };
        //        HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
        //        return response;
        //    }
        //    else
        //    {
        //        var dbMoney = Math.Round((double)(model.Amount * firstOrderCode.Rate));
        //        var realAmountReceive = Convert.ToInt64(dbMoney);
        //        USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Fail, ServiceID,0,0, CHECKER_ID, 0, model.Amount, realAmountReceive, model.StatusStr, out RemainBalance, out Response);
               

        //        var obj = new
        //        {
        //            ResponseCode = Response,
        //            Message = String.Format("Đơn hàng {0} được cập nhật trạng thái Fail {1}", code, Response),
        //        };
        //        HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
        //        return response;
        //    }

        //}
    }
}
