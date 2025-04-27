using MsWebGame.Thecao.Database.DAO;
using MsWebGame.Thecao.Models.SmsCharges;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Security;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/SmsCharge")]
    public class SmsChargeController : BaseApiController
    {
        //private string secretkey="CFiFgzlNTOCuRKiXgh0GZWfWI43F2fh3";
        //private string signalFormat = "{0}{1}{2}";
        //private string Errror_02 = "02";
        //private string Errror_03 = "03";
        //private string SUCCESS_00 = "00";
        //private string ERROR_99 = "99";
        //[ActionName("CallBackResult")]
        //[HttpPost]
        //public dynamic CallBackResult([FromBody] SmsChargeModel model)
        //{
        //    var result= JsonConvert.SerializeObject(model);
        //    LoggerHelper.LogUSDTMessage(String.Format("SmsCharge-:{0}", result));
        //    if (model == null)
        //    {

                
        //            return new
        //            {
        //                requestId = "null",
        //                status = Errror_02,
        //               // responseTime = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss"),
        //                signature = Security.MD5Encrypt(String.Format(signalFormat,null, Errror_02, secretkey))
        //            };

                
        //    }
        //    if (model.amount <= 0|| String.IsNullOrEmpty(model.info) || String.IsNullOrEmpty(model.msisdn) || String.IsNullOrEmpty(model.requestId)|| String.IsNullOrEmpty(model.signature))
        //    {
        //        return new
        //        {
        //            requestId = model.requestId,
        //            status = Errror_02,
        //            //responseTime = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss"),
        //            signature = Security.MD5Encrypt(String.Format(signalFormat, model.requestId, Errror_02, secretkey))
        //        };

        //    }

        //    var key = String.Format("{0}{1}{2}{3}{4}", model.msisdn, model.requestId, model.amount, model.info, secretkey);
        //    var encryptKey = Security.MD5Encrypt(key);
        //    if (encryptKey != model.signature)
        //    {
        //        return new
        //        {
        //            requestId = model.requestId,
        //            status = Errror_03,
        //           // responseTime = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss"),
        //            signature = Security.MD5Encrypt(String.Format(signalFormat, model.requestId, Errror_03, secretkey))
        //        };
        //    }


          
        //    int SUCCESS_STATUS = 1;
        //    int PartnerID = SUCCESS_STATUS;
        //    long UserID = ConvertUtil.ToLong(model.info);
        //    string PartnerErrorCode = "200";
        //    string PartnerMessage = "OK";
        //    string Description = String.Format("Nạp thẻ thành công");
        //    long RequestID = 0;
        //    long ReceivedMoney = 0;
        //    long RemainBalance = 0;
        //    double RequestRate = 0;
        //     int Response = 0;
        //    SmsChargeDAO.Instance.UserSmsRequestPartnerCheck(model.msisdn, SUCCESS_STATUS, PartnerID, UserID,
        //    Convert.ToInt64(model.amount), PartnerErrorCode,
        //     PartnerMessage, model.signature, model.requestId, Description
        //    , out RequestID, out ReceivedMoney,
        //     out RemainBalance, out RequestRate, out Response);
        //    if (Response == 1)
        //    {

        //        return new
        //        {
        //            requestId =model.requestId,
        //            status = SUCCESS_00,
        //           // responseTime =DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss") ,
        //            signature = Security.MD5Encrypt(String.Format(signalFormat, model.requestId, SUCCESS_00, secretkey))
        //        };
        //    }else
        //    {
        //        LoggerHelper.LogUSDTMessage(String.Format("SmsCharge-Response:{0}", Response));
        //        return new
        //        {
        //            requestId = model.requestId,
        //            status = ERROR_99,
        //          //  responseTime = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss"),
        //            signature = Security.MD5Encrypt(String.Format(signalFormat, model.requestId, ERROR_99, secretkey))
        //        };
        //    }

           





        //}
    }
}
