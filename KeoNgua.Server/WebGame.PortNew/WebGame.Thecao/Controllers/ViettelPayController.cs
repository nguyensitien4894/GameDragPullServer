using MsWebGame.Thecao.Database.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MsWebGame.Thecao.Database.DTO;
using TraditionGame.Utilities;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Momos.Api.Charges;
using TraditionGame.Utilities.ViettelPay.Models;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/ViettelPay")]
    public class ViettelPayController : BaseApiController
    {
    //    private string MOMO_SECRETKEY = ConfigurationManager.AppSettings["MOMO_SECRETKEY"].ToString();
    //    private List<string> AccpetProvider = new List<string>() { "MMO" };
    //    [ActionName("ViettelPayCallBackResultHDP")]
    //    [HttpPost]
    //    public HttpResponseMessage ViettelPayCallBackResultHDP([FromBody] ViettelPayRequest momoRequest)
    //    {
    //        string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
    //        if (string.IsNullOrEmpty(ip))
    //        {
    //            ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
    //        }
    //        NLogManager.LogMessage("ViettelPayCallBackResultHDP-IP: " + ip);
    //        if (momoRequest == null)
    //        {
    //            return JsonMomoResult(-100, "ParaInvalid model null");
    //        }
    //        LoggerHelper.LogUSDTMessage(String.Format("ViettelPayCallBackResultHDP CallBackModel:{0}", JsonConvert.SerializeObject(momoRequest)));
    //        if (String.IsNullOrEmpty(momoRequest.message))
    //        {
    //            return JsonMomoResult(-106, "Message null");
    //        }
    //        if (momoRequest.money <= 0 || momoRequest.money < 10000 || momoRequest.money > 30000000)
    //        {
    //            NLogManager.LogMessage(String.Format("ViettelPay:money in valid : {0}", momoRequest.money));
    //            return JsonMomoResult(-107, "money in valid");
    //        }
    //        if (String.IsNullOrEmpty(momoRequest.message))
    //        {
    //            return JsonMomoResult(-108, "Mes in valid");
    //        }
    //        if (String.IsNullOrEmpty(momoRequest.transaction_id))
    //        {
    //            return JsonMomoResult(-109, "transaction_id in valid");
    //        }
    //        string momoApiKey = ConfigurationManager.AppSettings["VIETTELPAY_MERCHANT_KEY"].ToString() + momoRequest.transaction_id.ToString();
    //        var sign = MomoChargeApi.MD5(momoApiKey);
    //        if (sign != momoRequest.signature)
    //        {
    //            return JsonMomoResult(-104, "Not Authen");
    //        }
    //        var momoEntity = ViettelPayDAO.Instance.UserViettelPayRequestGetByRefKey(momoRequest.transaction_id);
    //        if (momoEntity != null)
    //        {
    //            return JsonMomoResult(-110, "Record Exist With Transaction");
    //        }
    //        int PartnerID = 1;
    //        string PartnerErrorCode = "200";
    //        long ReceivedMoney = 0;
    //        long RemainBalance = 0;
    //        long RequestID = 0;
    //        double RequestRate = 0;
    //        int RequestType = 1;
    //        int Response = 0;
    //        int outServiceID = 0;
    //        string mess = momoRequest.message.Trim().ToLower();
    //        Account acount = AccountDAO.Instance.GetAccountInfo(0, mess, null);
    //        if (acount != null)
    //        {
    //            ViettelPayDAO.Instance.UserViettelPayRequestPartnerCheck(
    //                acount.AccountID, RequestType, momoRequest.money, PartnerID, null,
    //                "1", PartnerErrorCode,
    //                momoRequest.message, null,
    //                null, momoRequest.transaction_id, momoRequest.transaction_id, momoRequest.signature, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
    //            );
    //            if (Response == 1)
    //            {
    //                var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
    //                dnaHelper.SendTransactionPURCHASE(acount.AccountID, 10, null, momoRequest.money, momoRequest.money);
    //                LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", acount.AccountID, 7, momoRequest.money, momoRequest.money, RequestID));
    //                return JsonMomoResult(0, "Success");
    //            }
    //            else
    //            {
    //                NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", acount.AccountID, Response));
    //                return JsonMomoResult(0, "Success");
    //            }
    //        }
    //        else
    //        {
    //            NLogManager.LogMessage(String.Format("MOMODB ERROR:User Null Message : {0}", mess));
    //            return JsonMomoResult(0, "Success");
    //        }
    //    }
    }
}