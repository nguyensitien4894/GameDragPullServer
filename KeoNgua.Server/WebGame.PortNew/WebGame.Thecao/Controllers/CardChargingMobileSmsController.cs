using Newtonsoft.Json;

using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities;


using MsWebGame.Thecao.Database.DAO;
using MsWebGame.Thecao.Handlers;
using MsWebGame.Thecao.Helpers.Chargings.MobileSMS;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/CardChargingMobileSms")]
    public class CardChargingMobileSmsController : BaseApiController
    {

         private int PARTNER_TWO = 2;
        
       
        //[ActionName("ReceiveResult")]
        //[HttpPost]
        //public HttpResponseMessage ReceiveResult(MobileSmsCallBackRequest data)
        //{
        //    try
        //    {
                
        //        if (data == null)
        //        {
        //            var msg = (String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid));
        //            return JsonResult(-1, msg, "0");
        //        }
        //        NLogManager.LogMessage("P2data"+JsonConvert.SerializeObject(data));
        //        if (String.IsNullOrEmpty(data.transId))
        //        {
        //            var msg = (String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid));
        //            return JsonResult(-2, msg, "0");
        //        }
        //        if (data.amount < 0)
        //        {
        //            var msg = (String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid));
        //            return JsonResult(-3, msg, data.transId);
        //        }

        //        string private_key = ConfigurationManager.AppSettings["MobileSMS_PRIVATEKEY"].ToString();


        //        string publickey = String.Format("{0}{1}{2}{3}", data.transId, data.errorid, data.amount, private_key);

        //        var signature = Encrypts.MD5(publickey);
        //        if (signature != data.signature)
        //        {
        //            var msg = (String.Format("{0}|{1}", "01", ErrorMsg.SignatureIncorrect));
        //            return JsonResult(-4, msg, data.transId);
        //        }

        //        var lngRequestID = ConvertUtil.ToLong(data.transId);

        //        if (lngRequestID <= 0)
        //        {
        //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
        //            return JsonResult(-5, msg, data.transId);
        //        }


        //        int outResponse = 0;




        //        //lấy ra object thẻ request
        //        // var card = CardDAO.Instance.UserCardRechargeList(lngRequestID, 0, 0, 0, 0, null, null, null, null, null, 1, int.MaxValue, out TotalRecord).FirstOrDefault();
        //        var card = CardDAO.Instance.UserCardRechargeGetByID(lngRequestID);
        //        if (card == null)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
        //            return JsonResult(-6, msg, data.transId);
        //        }

        //        if (card.Status != 0)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
        //            return JsonResult(-7, msg, data.transId);
        //        }

               
        //        if (!card.ReceivedMoney.HasValue)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A4{0}", ErrorMsg.CardInvalid));
        //            return JsonResult(-8, msg, data.transId);
        //        }
        //        if (!ArryAmountValid.Contains(card.CardValue))
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A7{0}", ErrorMsg.AmountInValid));
        //            return JsonResult(-9,msg);
        //        }
        //        if (!ArryAmountValid.Contains(data.amount))
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A7{0}", ErrorMsg.AmountInValid));
        //            return JsonResult(-30,msg);
        //        }
        //        if (card.TeleRate < 0 || card.TeleRate > 1)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A8{0}", ErrorMsg.CardRateInValidate));
        //            return JsonResult(-32,msg);
        //        }
        //        //nếu mã !=1 hoặc mệnh giá gửi sang  khác mệnh giá truyền về mặc định là sai (ko cần biết)
        //        double KMRate = card.Rate - card.TeleRate;
        //        if (KMRate <= 0)
        //        {
        //            KMRate = 0;

        //        }
        //        if (data.errorid != 1 || (card.CardValue != data.amount)||data.amount<=0)
        //        {
        //            //phần sai mệnh giá ( chỗ này phải xử lý )
        //            if (card.CardValue != data.amount && data.amount > 0&& data.errorid==1)
        //            {
        //                int value = data.amount;
        //                var errorCode = data.errorid.ToString();
        //                var message = data.errordesc;

        //                var minValue = card.CardValue >= value ? value : card.CardValue;
        //                var receiveValue = ConvertUtil.ToLong(minValue * card.TeleRate);
        //                //cập nhật lại trạng thái 4
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, errorCode, message, STATUS_SMG_NOT_REFUND, PARTNER_TWO, value, minValue, receiveValue, out outResponse);
        //                if (outResponse == 1)
        //                {
        //                    try
        //                    {
        //                        var cardIndex = GetCardCodeIndex(card.OperatorCode);
        //                        SendDNA(lngRequestID,card.UserID, cardIndex, minValue, receiveValue,false, KMRate);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        NLogManager.PublishException(ex);
        //                    }
        //                    SendChargingHub(card.UserID, 0, STATUS_SMG_NOT_REFUND_REASON, 0, ServiceID);
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("H5{0}", ErrorMsg.CardUpdateStatus_Negative3));
        //                    return JsonResult(-33,msg);
        //                    ///cộng tiền khi  sai mệnh giá khi

        //                    //long wallet = 0;
        //                    //long tranID = 0;
        //                    //string msgTC = String.Format("Nạp thẻ (SMG) phiên {0} ", lngRequestID);
        //                    //var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, receiveValue, msgTC, ServiceID, out tranID, out wallet);

        //                    //if (res != 1)//nếu không thể cộng tiền cho user
        //                    //{
        //                    //    SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                    //    var msg = String.Format("{0}|{1}", "00", String.Format("H5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
        //                    //    outResponse = 0;
        //                    //    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 3, PARTNER_TWO, out outResponse);
        //                    //    if (outResponse == 1)
        //                    //    {
        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                    //        return JsonResult(-33, msg);
        //                    //    }
        //                    //    else if (outResponse == -202)
        //                    //    {

        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //        return JsonResult(-34, msg);
        //                    //    }
        //                    //    else if (outResponse == -508)
        //                    //    {

        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //        return JsonResult(-35, msg);
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //        return JsonResult(-36, msg);
        //                    //    }

        //                    //}
        //                    //else
        //                    //{
        //                    //    //đẫ cộng tiền cho khách và 
        //                    //    var msg = String.Format("{0}|{1}", "00", String.Format("H6{0}", ErrorMsg.CardUpdateStatus4));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //    return JsonResult(-37, msg);
        //                    //}

        //                }
        //                else
        //                {

        //                    var msg = String.Format("{0}|{1}", "00", String.Format("H7{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -3
        //                    return JsonResult(-38, msg);
        //                }

        //            }
        //            else
        //            {
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, data.errorid.ToString(), data.errordesc, -1, PARTNER_TWO, data.amount, null, null, out outResponse);
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                if (outResponse == 1)
        //                {
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                    return JsonResult(-9, msg, data.transId);
        //                }
        //                else if (outResponse == -202)
        //                {

        //                    var msg = String.Format("{0}|{1}", "00", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return JsonResult(-10, msg, data.transId);
        //                }
        //                else if (outResponse == -508)
        //                {

        //                    var msg = String.Format("{0}|{1}", "00", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return JsonResult(-11, msg, data.transId);
        //                }
        //                else
        //                {
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return JsonResult(-12, msg, data.transId);
        //                }
        //            }
        //        }
        //        //trường hợp này nhà mạng viettle trả về 1
        //        else
        //        {
        //            outResponse = 0;
        //            //cập  nhật ngay thẻ về trạng thái 1 
        //            // CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 1, PARTNER_ONE, out outResponse);
        //            CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, data.errorid.ToString(), data.errordesc, 1, PARTNER_TWO, data.amount,null,null,out outResponse);
        //            if (outResponse == 1)
        //            {
        //                // var msg = String.Format("{0}|{1}", "00", String.Format("C1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

        //                long wallet = 0;
        //                long tranID = 0;
        //                string msgTC = String.Format("Nạp thẻ phiên {0} ",lngRequestID);
        //                var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, msgTC, ServiceID, out tranID, out wallet);

        //                if (res != 1)//nếu không thể cộng tiền cho user
        //                {
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
        //                    outResponse = 0;
        //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, data.errorid.ToString(), data.errordesc, 3, PARTNER_TWO, out outResponse);
        //                    if (outResponse == 1)
        //                    {
        //                        msg = String.Format("{0}|{1}", "00", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                        return JsonResult(-13, msg, data.transId);
        //                    }
        //                    else if (outResponse == -202)
        //                    {

        //                        msg = String.Format("{0}|{1}", "00", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return JsonResult(-14, msg, data.transId);
        //                    }
        //                    else if (outResponse == -508)
        //                    {

        //                        msg = String.Format("{0}|{1}", "00", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return JsonResult(-15, msg, data.transId);
        //                    }
        //                    else
        //                    {
        //                        msg = String.Format("{0}|{1}", "00", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return JsonResult(-16, msg, data.transId);
        //                    }


        //                }
        //                else
        //                {

        //                    //cập nhật 2 (đã nạp tiền thành công)
        //                    outResponse = 0;
        //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, data.errorid.ToString(), data.errordesc, 2, PARTNER_TWO, out outResponse);

        //                    if (outResponse == 1)
        //                    {
        //                        SendChargingHub(card.UserID, wallet, "Nạp thẻ thành công", 1, ServiceID);
        //                        var msg = String.Format("{0}|{1}", "00", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

        //                        try
        //                        {
        //                            var cardIndex = GetCardCodeIndex(card.OperatorCode);
        //                            SendDNA(lngRequestID,card.UserID, cardIndex, card.CardValue, card.ReceivedMoney.Value,true, KMRate);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            NLogManager.PublishException(ex);
        //                        }

        //                        return JsonResult(1, msg, data.transId);
        //                    }
        //                    else if (outResponse == -202)
        //                    {

        //                        var msg = String.Format("{0}|{1}", "00", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return JsonResult(-17, msg, data.transId);
        //                    }
        //                    else if (outResponse == -508)
        //                    {

        //                        var msg = String.Format("{0}|{1}", "00", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return JsonResult(-18, msg, data.transId);
        //                    }
        //                    else
        //                    {
        //                        var msg = String.Format("{0}|{1}", "00", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return JsonResult(-18, msg, data.transId);
        //                    }
        //                }
        //            }
        //            else if (outResponse == -202)
        //            {

        //                var msg = String.Format("{0}|{1}", "00", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                return JsonResult(-19, msg, data.transId);
        //            }
        //            else if (outResponse == -508)
        //            {

        //                var msg = String.Format("{0}|{1}", "00", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                return JsonResult(-20, msg, data.transId);
        //            }
        //            else
        //            {
        //                var msg = String.Format("{0}|{1}", "00", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                return JsonResult(-21, msg, data.transId);
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = String.Format("{0}|{1}", "09", ErrorMsg.CardBusy);

        //        return JsonResult(-22, msg, "0");
        //    }
        //}
      
    }
}