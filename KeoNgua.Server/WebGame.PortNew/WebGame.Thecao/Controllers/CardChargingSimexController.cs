using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using TraditionGame.Utilities;



using MsWebGame.Thecao.Database.DAO;
using MsWebGame.Thecao.Handlers;
using MsWebGame.Thecao.Helpers;
using MsWebGame.Thecao.Helpers.Chargings.Cards;
using MsWebGame.Thecao.Helpers.Chargings.Cards.MobilePhone;
using MsWebGame.Thecao.Helpers.Chargings.FconnClub;
using MsWebGame.Thecao.ShopMuaThe;

namespace MsWebGame.Thecao.Controllers
{
    //[RoutePrefix("api/CardChargingSimex")]
    //public class CardChargingSimexController : ApiController
    //{
    //    private int PARTNER_FIVE = 5;
    //    [ActionName("ReceiveResult")]
    //    [HttpGet]
    //    public HttpResponseMessage ReceiveResult(string requestid, int code, int amount, string signature, string errormessage, int value)
    //    {
    //        try
    //        {
    //            NLogManager.LogMessage(String.Format("requestid:{0}|code:{1}|amount:{2}|signature:{3}|errormessage:{4}", requestid, code, amount, signature, errormessage));
    //            if (String.IsNullOrEmpty(requestid) || String.IsNullOrEmpty(code.ToString()) || String.IsNullOrEmpty(amount.ToString())
    //                || String.IsNullOrEmpty(signature) || String.IsNullOrEmpty(errormessage))
    //            {

    //                var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
    //                return StringResult(msg);
    //            }
    //            var lngRequestID = ConvertUtil.ToLong(requestid);
    //            if (lngRequestID <= 0)
    //            {
    //                var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
    //                return StringResult(msg);
    //            }

    //            //so sanh sig
    //            string secretKey = ConfigurationManager.AppSettings["VINA_SECRETEKEY"].ToString();

    //            var plainText = String.Format("{0}{1}{2}{3}", requestid, code, amount, secretKey);
    //            var tmpSig = CardVinaHelper.md5(plainText);
    //            if (tmpSig != signature)
    //            {
    //                var msg = String.Format("{0}|{1}", "03", ErrorMsg.SignatureIncorrect);
    //                return StringResult(msg);
    //            }

    //            int outResponse = 0;
    //            long RemainBalacne = 0;
    //            int TotalRecord = 0;


    //            var card = CardDAO.Instance.UserCardRechargeList(lngRequestID, 0, 0, 0, 0, null, null, null, null, null, 1, int.MaxValue, out TotalRecord).FirstOrDefault();
    //            if (card == null)
    //            {
    //                var msg = String.Format("{0}|{1}", "00", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
    //                return StringResult(msg);
    //            }

    //            if (card.Status != 0)
    //            {
    //                var msg = String.Format("{0}|{1}", "00", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
    //                return StringResult(msg);
    //            }

    //            //if (card.OperatorCode != "VMS"&& card.OperatorCode != "VNP")//báo lỗi vì lại thẻ không chính xác vinaphone
    //            //{
    //            //    var msg = String.Format("{0}|{1}", "00", String.Format("A3{0}", ErrorMsg.CardTeleIDInValid));// trạng thái thẻ =00  không tìm thấy 
    //            //    return StringResult(msg);
    //            //}
    //            if (!card.ReceivedMoney.HasValue)
    //            {
    //                var msg = String.Format("{0}|{1}", "00", String.Format("A4{0}", ErrorMsg.CardInvalid));
    //                return StringResult(msg);
    //            }

    //            //nếu mã lỗi khác 00 với nhà mạng viettel và mã lỗi khác 1 với nhà mạng Vina phone
    //            if (code != 1 || (card.CardValue != value))
    //            {
    //                //cập nhật ngay thành trạng thái -1
    //                // CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, code.ToString(), errormessage, -1, PARTNER_ONE, out outResponse);
    //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, code.ToString(), errormessage, -1, PARTNER_FIVE, value, out outResponse);
    //                if (outResponse == 1)
    //                {
    //                    var msg = String.Format("{0}|{1}", "00", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //                    return StringResult(msg);
    //                }
    //                else if (outResponse == -202)
    //                {

    //                    var msg = String.Format("{0}|{1}", "00", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
    //                    return StringResult(msg);
    //                }
    //                else if (outResponse == -508)
    //                {

    //                    var msg = String.Format("{0}|{1}", "00", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
    //                    return StringResult(msg);
    //                }
    //                else
    //                {
    //                    var msg = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
    //                    return StringResult(msg);
    //                }
    //            }
    //            //trường hợp này nhà mạng vina trả về 1
    //            else
    //            {
    //                //nếu trạng thái thành công thì 2 value phải giống nhau
    //                if (code == 1 && card.CardValue != value)
    //                {
    //                    var msg = String.Format("{0}|{1}", "00", String.Format("A5{0}", ErrorMsg.AmountInValid));
    //                    return StringResult(msg);
    //                }
    //                outResponse = 0;
    //                //cập  nhật ngay thẻ về trạng thái 1 
    //                // CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, code.ToString(), errormessage, 1, PARTNER_ONE, out outResponse);
    //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, code.ToString(), errormessage, 1, PARTNER_FIVE, value, out outResponse);
    //                if (outResponse == 1)
    //                {
    //                    // var msg = String.Format("{0}|{1}", "00", String.Format("C1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

    //                    long wallet = 0;
    //                    long tranID = 0;
    //                    var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, "Nạp thẻ cào vina", out tranID, out wallet);

    //                    if (res != 1)//nếu không thể cộng tiền cho user
    //                    {
    //                        var msg = String.Format("{0}|{1}", "00", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
    //                        outResponse = 0;
    //                        CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, code.ToString(), errormessage, 3, PARTNER_FIVE, out outResponse);
    //                        if (outResponse == 1)
    //                        {
    //                            msg = String.Format("{0}|{1}", "00", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -202)
    //                        {

    //                            msg = String.Format("{0}|{1}", "00", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -508)
    //                        {

    //                            msg = String.Format("{0}|{1}", "00", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else
    //                        {
    //                            msg = String.Format("{0}|{1}", "00", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }


    //                    }
    //                    else
    //                    {

    //                        //cập nhật 2 (đã nạp tiền thành công)
    //                        outResponse = 0;
    //                        CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, code.ToString(), errormessage, 2, PARTNER_FIVE, out outResponse);

    //                        if (outResponse == 1)
    //                        {
    //                            SendChargeToHub(card.UserID, wallet, "Nạp thẻ thành công");
    //                            var msg = String.Format("{0}|{1}", "00", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -202)
    //                        {

    //                            var msg = String.Format("{0}|{1}", "00", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -508)
    //                        {

    //                            var msg = String.Format("{0}|{1}", "00", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else
    //                        {
    //                            var msg = String.Format("{0}|{1}", "00", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                    }
    //                }
    //                else if (outResponse == -202)
    //                {

    //                    var msg = String.Format("{0}|{1}", "00", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //                    return StringResult(msg);
    //                }
    //                else if (outResponse == -508)
    //                {

    //                    var msg = String.Format("{0}|{1}", "00", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //                    return StringResult(msg);
    //                }
    //                else
    //                {
    //                    var msg = String.Format("{0}|{1}", "00", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //                    return StringResult(msg);
    //                }






    //            }





    //        }
    //        catch (Exception ex)
    //        {
    //            var msg = String.Format("{0}|{1}", "09", ErrorMsg.CardBusy);
    //            return StringResult(msg);
    //        }










    //    }


    //    /// <summary>
    //    /// string result set success
    //    /// </summary>
    //    /// <param name="msg"></param>
    //    /// <returns></returns>
    //    private HttpResponseMessage StringResult(string msg)
    //    {
    //        return new HttpResponseMessage()
    //        {
    //            Content = new StringContent(msg, Encoding.UTF8, "text/plain")
    //        };
    //    }

    //    /// <summary>
    //    /// gửi msg cộng tiền thành công
    //    /// </summary>
    //    /// <param name="accountId"></param>
    //    /// <param name="ammount"></param>
    //    /// <param name="msg"></param>
    //    /// <returns></returns>
    //    private bool SendChargeToHub(long accountId, long balance, string msg)
    //    {
    //        try
    //        {
    //            NLogManager.LogMessage(string.Format("Call Out :accountId: {0} | balance: {1} | msg: {2} ", accountId, balance, msg));
    //            PortalHandler.Instance.ReturnTopupCard(accountId, balance, msg);


    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            NLogManager.LogMessage(string.Format("Exception Call Out :accountId: {0} | balance: {1} | msg: {2} ", accountId, balance, msg));
    //            NLogManager.PublishException(ex);
    //            return false;
    //        }
    //    }

    //    private HttpResponseMessage JsonResult(int status, string msg)
    //    {
    //        var resp = new HttpResponseMessage();
    //        var obj = new
    //        {
    //            status = status,
    //            msg = msg,
    //        };
    //        string jsonRes = JsonConvert.SerializeObject(obj);
    //        resp.Content = new StringContent(jsonRes, System.Text.Encoding.UTF8, "application/json");
    //        return resp;
    //    }
    //}
}
