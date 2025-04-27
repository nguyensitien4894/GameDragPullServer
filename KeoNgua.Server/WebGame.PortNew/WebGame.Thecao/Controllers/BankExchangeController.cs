using MsWebGame.Portal.Helpers;
using MsWebGame.Thecao.Database.DAO;
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
using TraditionGame.Utilities.MyUSDT.Models.Exchanges;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.Thecao.Controllers
{
    //[RoutePrefix("api/BankExchange")]
    //public class BankExchangeController : BaseApiController
    //{
    //    // GET: BankExchange
    //    [ActionName("CallBackResult")]
    //    [HttpPost]
    //    public dynamic CallBackResult([FromBody] CallBackSellorderRequestModel model)
    //    {
    //        if (model == null)
    //        {
    //            return new
    //            {
    //                ResponseCode = -1005,
    //                Message = ErrorMsg.ParamaterInvalid
    //            };
    //        }
    //        NLogManager.LogMessage("BankExChange: " + JsonConvert.SerializeObject(model));
    //        var code = model.Code;
    //        if (String.IsNullOrEmpty(code))
    //        {
    //            return new
    //            {
    //                ResponseCode = -1006,
    //                Message = "Mã code không thể trống"
    //            };
    //        }
    //        var accepStatus = new List<string>() { "completed", "canceled", "failed" };
    //        if (!accepStatus.Contains(model.Status.ToLower()))
    //        {
    //            var obj = new
    //            {
    //                ResponseCode = -1007,
    //                Message = "Mã code không nằm trong dãy ( completed, canceled,failed)"
    //            };
    //            HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.BadRequest, obj);
    //            return response;
    //        }
    //        if (model.Method.ToLower() != "sell")
    //        {
    //            return new
    //            {
    //                ResponseCode = -1008,
    //                Message = "Phương thức gửi không phải là sell"
    //            };
    //        }
    //        int TotalRecord = 0;
    //        //1.lấy thông tin order theo code
    //        var orderByCode = USDTDAO.Instance.UserBankRequestList(null, null, code, null, null, null, ServiceID, 1, 1, out TotalRecord);
    //        if (orderByCode == null || !orderByCode.Any())
    //        {
    //            return new
    //            {
    //                ResponseCode = -1005,
    //                Message = String.Format("Không tồn tại giao dịch theo mã {0}", code),
    //            };
    //        }
    //        var firstOrderCode = orderByCode.FirstOrDefault();
    //        if (firstOrderCode == null)
    //        {
    //            return new
    //            {
    //                ResponseCode = -1005,
    //                Message = String.Format("Không tồn tại giao dịch theo mã {0}", code),
    //            };
    //        }
    //        if (firstOrderCode.PartnerStatus != PENDING)
    //        {
    //            return new
    //            {
    //                ResponseCode = -1006,
    //                Message = String.Format("Đơn hàng {0} đã được cập nhật trạng thái {1}", code, firstOrderCode.PartnerStatus),
    //            };
    //        }
    //        if (firstOrderCode.Status != 3)
    //        {
    //            return new
    //            {
    //                ResponseCode = -1006,
    //                Message = String.Format("Đơn hàng {0} đã được cập nhật trạng thái  status {1}", code, firstOrderCode.Status),
    //            };
    //        }


    //        int Response;
    //        long balance = 0;
          
    //        if (model.Status.ToLower() == COMPLETED)
    //        {
              
    //            //USDTDAO.Instance.UserBankRequestUpdate(firstOrderCode.RequestID, null, firstOrderCode.UserID, firstOrderCode.PartnerID,
                    
    //            //    firstOrderCode.ServiceID.Value, 1, COMPLETED, null, null, null
    //            //    , null, null, null, null, null, null, null, COMPLETED, null, COMPLETED,
    //            //    COMPLETED, null, null, null, null, null, out Response);

    //            USDTDAO.Instance.UserBankRequestUpdateStatus(firstOrderCode.RequestID,null,firstOrderCode.ServiceID.Value,null,"200",null,COMPLETED,1,1,firstOrderCode.UserID,COMPLETED,null,null,null,null,null,null,null,out Response);
    //            var obj = new
    //            {
    //                ResponseCode = Response,
    //                Message = String.Format("Đơn hàng {0} được cập nhật trạng thái Success {1}", code, Response),
    //            };
    //            HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
    //            return response;
    //        }
    //        else
    //        {
    //            int CALLBACK_FAIL = 5;
    //            //cập nhật lại trạng thái của order 
    //            int AdminID = 5;
    //            USDTDAO.Instance.UserBankRequestExamine(firstOrderCode.RequestID, firstOrderCode.UserID,null, CALLBACK_FAIL, firstOrderCode.ServiceID.Value,0, 0, AdminID, 0,null,null
    //                   ,model.Status, null,model.Status,null, null,
    //                   out Response, out balance);
    //            //cập nhật lại trạng thái của 
    //            var obj = new
    //            {
    //                ResponseCode = Response,
    //                Message = String.Format("Đơn hàng {0} được cập nhật hoàn lại tiền {1}", code, Response),
    //            };
    //            HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
    //            return response;
    //        }


    //    }
    //}
}