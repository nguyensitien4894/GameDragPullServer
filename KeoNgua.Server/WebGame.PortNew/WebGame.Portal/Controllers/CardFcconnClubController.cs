namespace MsWebGame.Portal.Controllers
{
    //[RoutePrefix("api/CardFcconnClub")]
    //public class CardFcconnClubController : ApiController
    //{
    //    private int PARTNER_THREE = 3;
    //    [ActionName("CallBackResult")]
    //    [HttpPost]
    //    public HttpResponseMessage ReceiveMobileResult([FromBody] FconnClubCallBackRequest request,string refkey)
    //    {

    //        try
    //        {
    //            NLogManager.LogMessage(String.Format("card Id {0} and status {1} and validae amount {2}", request.card_id, request.status, request.value));

    //            if (request==null||String.IsNullOrEmpty(request.name) || String.IsNullOrEmpty(request.seri) || String.IsNullOrEmpty(request.pin)
    //                || String.IsNullOrEmpty(request.status) || request.error_code < 0  || request.card_id <= 0 || request.value < 0)
    //            {

    //                var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
    //                return StringResult(msg);
    //            }
    //            int outResponse = 0;
    //            var card = CardRefDAO.Instance.UserCardRechargeByRefKey(request.card_id.ToString(),PARTNER_THREE).FirstOrDefault();
    //            if (card == null)
    //            {
    //                var msg = String.Format("{0}|{1}", "-1", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
    //                return StringResult(msg);
    //            }
    //            if (card.Status != 0)
    //            {
    //                var msg = String.Format("{0}|{1}", "1", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
    //                return StringResult(msg);
    //            }
    //            if (!card.ReceivedMoney.HasValue)
    //            {
    //                var msg = String.Format("{0}|{1}", "1", String.Format("A4{0}", ErrorMsg.CardInvalid));
    //                return StringResult(msg);
    //            }
    //            //báo là error 
    //            if (request.status == "error")
    //            {
    //                //cập nhật ngay thành trạng thái -1
    //                CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, request.error_code.ToString(), request.error_desc, -1,PARTNER_THREE, request.value, out outResponse);
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
    //            //trường hợp này nhà mạng trẳ về ok (có thể là đúng mệnh giá hoặc sai mệnh giá )
    //            else
    //            {
    //                outResponse = 0;
    //                //cập  nhật ngay thẻ về trạng thái 1 

    //                CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, request.error_code.ToString(), request.error_desc, 1, PARTNER_THREE, request.value, out outResponse);

    //                if (outResponse == 1)
    //                {

    //                    //nếu sai mệnh giá thì chỉ cập nhật trạng thái về -1 chờ chăm sóc khách hàng xử lý
    //                    if (card.CardValue != request.value)
    //                    {
    //                        CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, request.error_code.ToString(), request.error_desc+"Sai mệnh giá", -1, PARTNER_THREE, request.value, out outResponse);
    //                        if (outResponse == 1)
    //                        {
    //                            var msg = String.Format("{0}|{1}", "1", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -202)
    //                        {

    //                            var msg = String.Format("{0}|{1}", "1", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -508)
    //                        {

    //                            var msg = String.Format("{0}|{1}", "1", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
    //                            return StringResult(msg);
    //                        }
    //                        else
    //                        {
    //                            var msg = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
    //                            return StringResult(msg);
    //                        }
    //                    }

    //                    //nếu đúng mệnh giá xử lý bình thường
    //                    long wallet = 0;
    //                    long tranID = 0;
    //                    var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, "Nạp thẻ cào ", out tranID, out wallet);

    //                    if (res != 1)//nếu không thể cộng tiền cho user
    //                    {
    //                        var msg = String.Format("{0}|{1}", "1", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
    //                        outResponse = 0;
    //                        CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, request.error_code.ToString(), request.error_desc, 3, PARTNER_THREE, request.value, out outResponse);
    //                        if (outResponse == 1)
    //                        {
    //                            msg = String.Format("{0}|{1}", "1", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -202)
    //                        {

    //                            msg = String.Format("{0}|{1}", "1", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -508)
    //                        {

    //                            msg = String.Format("{0}|{1}", "1", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else
    //                        {
    //                            msg = String.Format("{0}|{1}", "1", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }


    //                    }
    //                    else
    //                    {

    //                        //cập nhật 2 (đã nạp tiền thành công)
    //                        outResponse = 0;
    //                        CardRefDAO.Instance.UpdateCardChardRefStatus(card.RequestID, card.UserID, request.error_code.ToString(), request.error_desc, 2, PARTNER_THREE, request.value, out outResponse);

    //                        if (outResponse == 1)
    //                        {
    //                            SendChargeToHub(card.UserID, wallet, "Nạp thẻ thành công");
    //                            var msg = String.Format("{0}|{1}", "1", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -202)
    //                        {

    //                            var msg = String.Format("{0}|{1}", "1", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else if (outResponse == -508)
    //                        {

    //                            var msg = String.Format("{0}|{1}", "1", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                        else
    //                        {
    //                            var msg = String.Format("{0}|{1}", "1", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //                            return StringResult(msg);
    //                        }
    //                    }
    //                }
    //                else if (outResponse == -202)
    //                {

    //                    var msg = String.Format("{0}|{1}", "1", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //                    return StringResult(msg);
    //                }
    //                else if (outResponse == -508)
    //                {

    //                    var msg = String.Format("{0}|{1}", "1", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //                    return StringResult(msg);
    //                }
    //                else
    //                {
    //                    var msg = String.Format("{0}|{1}", "1", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //                    return StringResult(msg);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            var msg = String.Format("{0}|{1}", "-1", ErrorMsg.CardBusy);
    //            NLogManager.PublishException(ex);
    //            return StringResult(msg);
    //        }

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

    //    private HttpResponseMessage StringResult(string msg)
    //    {
    //        return new HttpResponseMessage()
    //        {
    //            Content = new StringContent(msg, Encoding.UTF8, "text/plain")
    //        };
    //    }
    //}
}
