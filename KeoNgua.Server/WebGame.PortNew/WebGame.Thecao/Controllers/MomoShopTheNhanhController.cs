using MsWebGame.Thecao.Database.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using TraditionGame.Utilities;
using TraditionGame.Utilities.BanksGateTheNhanh;
using TraditionGame.Utilities.BanksGateTheNhanh.Models;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Momos.Api.Charges;
using TraditionGame.Utilities.Momos.Models;


namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/MomoShopTheNhanh")]
    public class MomoShopTheNhanhController : BaseApiController
    {
        private static string partnerKey = ConfigurationManager.AppSettings["SHOPMUATHE_KEY"].ToString();
        private static string partnerCode = ConfigurationManager.AppSettings["SHOPMUATHE_PARTNERCODE"].ToString();
        private static string serviceCode = "bankdirect";

        //hàm này dùng để callback momo shopthenhahy
        //[ActionName("ShopTheNhanhCallBack")]
        //[HttpPost]
        //public HttpResponseMessage ShopTheNhanhCallBack([FromBody] TraditionGame.Utilities.BanksGateTheNhanh.APIResponse input)
        //{

        //    NLogManager.LogMessage(String.Format("MomoCharge.CallBackResult CallBackModel:{0}", JsonConvert.SerializeObject(input)));
        //    if (input==null)
        //    {
        //        return JsonMomoResult(-100, "ShopTheNhanh-Paramater invalid");
        //    }
        //    var ResponseCode = input.ResponseCode;
        //    var responseContent = input.ResponseContent;
        //    if (ResponseCode=="1"&&string.IsNullOrEmpty(responseContent))
        //    {
        //        return JsonMomoResult(-101, "ShopTheNhanh-Paramater invalid");
        //    }
        //    var signature = input.Signature;
        //    if (string.IsNullOrEmpty(signature))
        //    {
        //        return JsonMomoResult(-102, "ShopTheNhanh-Paramater invalid");
        //    }
           
        //    if (string.IsNullOrEmpty(ResponseCode))
        //    {
        //        return JsonMomoResult(-1012, "ShopTheNhanh-Paramater invalid");
        //    }
        //    var Description = input.Description;
        //    if (string.IsNullOrEmpty(Description))
        //    {
        //        return JsonMomoResult(-103, "ShopTheNhanh-Paramater invalid");
        //    }
        //    //string commandCode = "order";
        //    //var tmpsignature = Encrypts.MD5(partnerCode + serviceCode + commandCode + responseContent + partnerKey);
        //    //if(tmpsignature!= signature)
        //    //{
        //    //    return JsonMomoResult(-104, "ShopTheNhanh-Paramater invalid");
        //    //}

        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    var data = serializer.Deserialize<DataCallback>(responseContent);
        //    if (String.IsNullOrEmpty(data.OrderNo)){
        //        return JsonMomoResult(-105, "ShopTheNhanh-Paramater invalid");
        //    }
        //    if (String.IsNullOrEmpty(data.RefCode))
        //    {
        //        return JsonMomoResult(-106, "ShopTheNhanh-Paramater invalid");
        //    }
        //    var requestAmount = ConvertUtil.ToInt(data.Amount);
        //    if (requestAmount <= 0)
        //    {
        //        return JsonMomoResult(-108, "ShopTheNhanh-Paramater invalid-GiaTriNhohonKO");
        //    }

        //    var userMomoRequest = MOMODAO.Instance.UserMomoRequestGetByID(ConvertUtil.ToLong(data.RefCode));
        //    if (userMomoRequest == null)
        //    {
               
        //            return JsonMomoResult(-108, "ShopTheNhanh-Paramater invalid-RefCodeKhongTonTai");
               
        //    }
        //    if (userMomoRequest.RequestCode != data.OrderNo)
        //    {
        //        return JsonMomoResult(-109, "ShopTheNhanh-Paramater invalid-OrderNoTruyenSai");
        //    }
        //    if (userMomoRequest.Status != 0)
        //    {
        //        return JsonMomoResult(-110, "ShopTheNhanh-Paramater invalid-Order đã cập nhật trạng thái trước đó");
        //    }
           
        //    int PartnerID = 2;


        //    long ReceivedMoney = 0;
        //    long RemainBalance = 0;
        //    long RequestID = 0;
        //    double RequestRate = 0;
        //    int RequestType = 2;
        //    int Response = 0;
           
        //    int outServiceID = 0;


        //    if (ResponseCode == "1")//nạp thành công cộng tiền gửi DNA
        //    {

        //        MOMODAO.Instance.UserMomoRequestReceiveResult(
        //        RequestType, requestAmount, PartnerID, data.OrderNo,
        //        ResponseCode, null, null, ResponseCode, Description, null, null, null, userMomoRequest.RequestID,
        //        out Response, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID


        //    );
        //        if (Response == 1)
        //        {
        //            var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
        //            dnaHelper.SendTransactionPURCHASE(userMomoRequest.UserID, 10, null, requestAmount, requestAmount);
        //            LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", userMomoRequest.UserID, 7, requestAmount, requestAmount, RequestID));
        //            return JsonMomoResult(200, "Nạp thành công");
        //        }
        //        else
        //        {
        //            NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", userMomoRequest.UserID, Response));
        //            return JsonMomoResult(Response, "Nạp thất bại|" + Response);
        //        }

        //    }else
        //    {

        //        int FAIL_STATUS = -2;
        //        MOMODAO.Instance.UserMomoRequestUpdate(userMomoRequest.RequestID, userMomoRequest.UserID, FAIL_STATUS, PartnerID, userMomoRequest.ServiceID.Value,DateTime.Now,userMomoRequest.Rate.HasValue?userMomoRequest.Rate.Value:0, 0, requestAmount,0, 0,userMomoRequest.UserID,null,null, null, null,null, ResponseCode, null, Description,Description, out Response);

        //        if (Response == 1)
        //        {
        //            var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
        //            dnaHelper.SendTransactionPURCHASE(userMomoRequest.UserID, 10, null, requestAmount, requestAmount);
        //            LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", userMomoRequest.UserID, 7, requestAmount, requestAmount, RequestID));
        //            return JsonMomoResult(200, "Ghi nhận thành công");
        //        }
        //        else
        //        {
        //            NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", userMomoRequest.UserID, Response));
        //            return JsonMomoResult(Response, "Ghi nhận bại|" + Response);
        //        }

        //    }


        //}

    }
}