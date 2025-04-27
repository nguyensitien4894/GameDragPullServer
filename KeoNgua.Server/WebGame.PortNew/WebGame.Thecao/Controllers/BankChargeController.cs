
using MsWebGame.Portal.Helpers;
using MsWebGame.Thecao.Database.DAO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/BankCharge")]
    public class BankChargeController : BaseApiController
    {
        protected int RequestType = 1;


        [ActionName("ReceiveResultPayment1s")]
        [HttpPost]
        public dynamic ReceiveResultPayment1s(dynamic model)
        {
            var request = HttpContext.Current.Request;
            //string receivedHmac = request.Headers["x-nowpayments-sig"];
            string requestJson = new StreamReader(request.InputStream).ReadToEnd();

            JObject requestData = JObject.Parse(requestJson);
            SortedDictionary<string, object> sortedRequestData = new SortedDictionary<string, object>();

            foreach (var property in requestData.Properties())
            {
                sortedRequestData.Add(property.Name, property.Value);
            }

            string sortedRequestJson = JsonConvert.SerializeObject(sortedRequestData, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });

            string debugmsg = "api/BankCharge/ReceiveResultPayment1s: " + JsonConvert.SerializeObject(requestData, Formatting.None);
            NLogManager.LogMessage("URL: " + Request.RequestUri.ToString());
            NLogManager.LogMessage("DEBUG: " + debugmsg);
            //NLogManager.LogMessage("x-nowpayments-sig: " + receivedHmac);

            return JsonMomoResult(-108, "SUCCESSFUL!");

        }


        /// <summary>
        /// call back khi xử lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        //    var accepStatus = new List<string>() { "completed", "canceled", "failed" };
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
        //    if (model.Method.ToLower() != "buy")
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

        //    int Response;
        //    long RemainBalance;
        //    var Status = MappingStatus(model.Status.ToLower());
        //    if (model.Status.ToLower() == COMPLETED)
        //    {
        //        var dbMoney = Math.Round((double)(model.AmountVcc * firstOrderCode.Rate));
        //        var realAmountReceive = Convert.ToInt64(dbMoney);
        //        //1.Cộng tiền cho khách và cập nhật order
        //        USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Success, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, realAmountReceive, model.AmountVcc, realAmountReceive, model.Status, out RemainBalance, out Response);

        //        LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBack.Completed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount:{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus:{10}|RemainBalance:{11}| Response:{12}",
        //            firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Success, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, realAmountReceive, model.AmountVcc, realAmountReceive, model.Status, RemainBalance, Response
        //            ));
        //        if (Response == 1)
        //        {
        //            try
        //            {
        //                var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
        //                dnaHelper.SendTransactionPURCHASE(firstOrderCode.UserID, 7, null, model.AmountVcc, model.AmountVcc);
        //                LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", firstOrderCode.UserID, 7, model.AmountVcc, model.AmountVcc, firstOrderCode.RequestID));
        //                int intResponse = 0;
        //                TreasureDAO.Instance.CarrotCollectRechargeBank(firstOrderCode.UserID, model.AmountVcc, ServiceID, out intResponse);

        //            }
        //            catch (Exception ex)
        //            {
        //                NLogManager.PublishException(ex);
        //            }
        //        }

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
        //        var dbMoney = model.AmountVcc * firstOrderCode.Rate;
        //        var realAmountReceive = Convert.ToInt64(dbMoney);
        //        USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Fail, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, 0, model.AmountVcc, realAmountReceive, model.Status, out RemainBalance, out Response);
        //        //cập nhật lại trạng thái của order 
        //        LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBack.Failed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount:{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus:{10}|RemainBalance:{11}| Response:{12}",
        //           firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Fail, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, 0, model.AmountVcc, realAmountReceive, model.Status, RemainBalance, Response
        //           ));

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
