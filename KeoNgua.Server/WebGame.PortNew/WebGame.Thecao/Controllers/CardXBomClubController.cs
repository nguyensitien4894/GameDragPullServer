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
using MsWebGame.Thecao.Helpers.Chargings.FconnClub;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/CardXBomClub")]
    public class CardXBomClubController : BaseApiController
    {


        //private string GetMsgSts (int status)
        //{


        //    if (status == 1)
        //    {
        //        return "Thành công";
        //    }else if (status == 2)
        //    {
        //        return "Sai định dạng mã thẻ, seri.|Giá trị thẻ cào, mã nhà mạng không đúng";
        //    }
        //    else if (status == 3)
        //    {
        //        return "Sai token hoặc token đã bị khóa";
        //    }
        //    else if (status == 4)
        //    {
        //        return "Lỗi khác";
        //    }
        //    else if (status == 5)
        //    {
        //        return "Truy cập bị từ chối";
        //    }
        //    return string.Empty;
        //}

        //private int PARTNER_THREE = 3;
        //[ActionName("CallBackResult")]
        //[HttpGet]
        //public HttpResponseMessage ReceiveMobileResult(string uuid,string cardCode,string cardSerial,int status,int amount)
        //{
        //    try
        //    {
             

        //        if ( String.IsNullOrEmpty(uuid) || String.IsNullOrEmpty(cardCode) || String.IsNullOrEmpty(cardSerial) || amount<0)
        //        {

        //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
        //            return StringResult(msg);
        //        }
        //        NLogManager.LogMessage(String.Format("P3 uuid {0}-cardCode {1}-cardSerial {2} status {3}",uuid,cardCode,cardSerial,status));
               


        //        int outResponse = 0;
        //        var card = CardRefDAO.Instance.UserCardRechargeByRefKey(uuid, PARTNER_THREE).FirstOrDefault();
        //        if (card == null)
        //        {
        //            var msg = String.Format("{0}|{1}", "-1", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
        //            return StringResult(msg);
        //        }
        //        if (card.Status != 0)
        //        {
        //            var msg = String.Format("{0}|{1}", "1", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
        //            return StringResult(msg);
        //        }
        //        if (!card.ReceivedMoney.HasValue)
        //        {
        //            var msg = String.Format("{0}|{1}", "1", String.Format("A4{0}", ErrorMsg.CardInvalid));
        //            return StringResult(msg);
        //        }

        //        if (!ArryAmountValid.Contains(card.CardValue))
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A7{0}", ErrorMsg.AmountInValid));
        //            return StringResult(msg);
        //        }
        //        if (card.TeleRate < 0 || card.TeleRate > 1)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A8{0}", ErrorMsg.CardRateInValidate));
        //            return StringResult(msg);
        //        }
        //        double KMRate = card.Rate - card.TeleRate;
        //        if (KMRate <= 0)
        //        {
        //            KMRate = 0;

        //        }
        //        //báo là error 
        //        if (status !=1|| amount == 0|| card.CardValue != amount)
        //        {
        //            //nếu sai mệnh giá thì chỉ cập nhật trạng thái về -1 chờ chăm sóc khách hàng xử lý
        //            if (card.CardValue != amount && amount > 0&& status == 1)
        //            {
        //                int value = amount;
        //                var minValue = card.CardValue >= value ? value : card.CardValue;
        //                var receiveValue = ConvertUtil.ToLong(minValue * card.TeleRate);
        //                //cập nhật lại trạng thái 4
        //                long lngRequestID = 0;
        //                lngRequestID = card.RequestID;
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, status.ToString(), GetMsgSts(status) + "Sai mệnh giá", STATUS_SMG_NOT_REFUND, PARTNER_THREE, value, minValue, receiveValue, out outResponse);

        //                if (outResponse == 1)
        //                {
        //                    try
        //                    {
        //                        var cardIndex = GetCardCodeIndex(card.OperatorCode);
        //                        SendDNA(card.RequestID,card.UserID, cardIndex, minValue, receiveValue,false, KMRate);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        NLogManager.PublishException(ex);
        //                    }
        //                    SendChargingHub(card.UserID, 0, STATUS_SMG_NOT_REFUND_REASON, 0, ServiceID);
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("H5{0}", ErrorMsg.CardUpdateStatus_Negative3));
        //                    return StringResult(msg);
                          

        //                }
        //                else
        //                {

        //                    var msg = String.Format("{0}|{1}", "00", String.Format("H7{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -3
        //                    return StringResult(msg);
        //                }
        //            }
        //            else
        //            {
        //                //cập nhật ngay thành trạng thái -1
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, status.ToString(), GetMsgSts(status), -1, PARTNER_THREE, amount, null, null, out outResponse);
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                if (outResponse == 1)
        //                {
        //                    var msg = String.Format("{0}|{1}", "1", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                    return StringResult(msg);
        //                }
        //                else if (outResponse == -202)
        //                {

        //                    var msg = String.Format("{0}|{1}", "1", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return StringResult(msg);
        //                }
        //                else if (outResponse == -508)
        //                {

        //                    var msg = String.Format("{0}|{1}", "1", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return StringResult(msg);
        //                }
        //                else
        //                {
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return StringResult(msg);
        //                }
        //            }
        //        }
        //        //trường hợp này nhà mạng trẳ về ok (có thể là đúng mệnh giá hoặc sai mệnh giá )
        //        else
        //        {
        //            outResponse = 0;
        //            //cập  nhật ngay thẻ về trạng thái 1 
        //            CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, status.ToString(), GetMsgSts(status), 1, PARTNER_THREE, amount,null,null, out outResponse);
        //            if (outResponse == 1)
        //            {
        //                //nếu đúng mệnh giá xử lý bình thường
        //                long wallet = 0;
        //                long tranID = 0;
        //                string msgTC = String.Format("Nạp thẻ phiên {0} ", card.RequestID);
        //                var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, msgTC, ServiceID, out tranID, out wallet);

        //                if (res != 1)//nếu không thể cộng tiền cho user
        //                {
        //                    var msg = String.Format("{0}|{1}", "1", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
        //                    outResponse = 0;
        //                    CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, status.ToString(), GetMsgSts(status), 3, PARTNER_THREE, amount, null,null, out outResponse);
        //                    if (outResponse == 1)
        //                    {
        //                        msg = String.Format("{0}|{1}", "1", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -202)
        //                    {

        //                        msg = String.Format("{0}|{1}", "1", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -508)
        //                    {

        //                        msg = String.Format("{0}|{1}", "1", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else
        //                    {
        //                        msg = String.Format("{0}|{1}", "1", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }


        //                }
        //                else
        //                {

        //                    //cập nhật 2 (đã nạp tiền thành công)
        //                    outResponse = 0;
        //                    CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, status.ToString(), GetMsgSts(status), 2, PARTNER_THREE, amount, null,null, out outResponse);

        //                    if (outResponse == 1)
        //                    {
        //                        SendChargingHub(card.UserID, wallet, "Nạp thẻ thành công", 1, ServiceID);
        //                        var msg = String.Format("{0}|{1}", "1", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

        //                        try
        //                        {
        //                            var cardIndex = GetCardCodeIndex(card.OperatorCode);
        //                            SendDNA(card.RequestID,card.UserID, cardIndex, card.CardValue, card.ReceivedMoney.Value,true, KMRate);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            NLogManager.PublishException(ex);
        //                        }
        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -202)
        //                    {

        //                        var msg = String.Format("{0}|{1}", "1", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -508)
        //                    {

        //                        var msg = String.Format("{0}|{1}", "1", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else
        //                    {
        //                        var msg = String.Format("{0}|{1}", "1", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                }
        //            }
        //            else if (outResponse == -202)
        //            {

        //                var msg = String.Format("{0}|{1}", "1", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msg);
        //            }
        //            else if (outResponse == -508)
        //            {

        //                var msg = String.Format("{0}|{1}", "1", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msg);
        //            }
        //            else
        //            {
        //                var msg = String.Format("{0}|{1}", "1", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msg);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = String.Format("{0}|{1}", "-1", ErrorMsg.CardBusy);
        //        NLogManager.PublishException(ex);
        //        return StringResult(msg);
        //    }

        //}


        
    }
}
