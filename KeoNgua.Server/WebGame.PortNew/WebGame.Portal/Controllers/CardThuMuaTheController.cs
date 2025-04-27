namespace MsWebGame.Portal.Controllers
{
    //[RoutePrefix("api/CardThuMuaThe")]
    //public class CardThuMuaTheController : BaseApiController
    //{
    //    private int PARTNER_TOW = 2;
    //    [ActionName("CallBackResult")]
    //    [HttpPost]
    //    public HttpResponseMessage ReceiveMobileResult([FromBody] CallBackRequest input )
    //    {
    //        try
    //        {


    //            if (input == null || String.IsNullOrEmpty(input.RefCode))
    //            {
    //                var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
    //                return StringResult( msg);
    //            }
    //            if (input.Status != 1 && input.Status != -1 && input.Status != -372)
    //            {
    //                var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
    //                return StringResult( msg);
    //            }
    //            //string partnerKey = ConfigurationManager.AppSettings["SHOPMUATHE_KEY"].ToString();
    //            //         var current_signature = Encrypts.MD5(input.RefCode +input.Status +input.Amount  + partnerKey);


    //            //if (current_signature!=input.Signature){
    //            //    var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
    //            //    return StringResult(msg);
    //            //}
    //            var lngRequestID = ConvertUtil.ToLong(input.RefCode);
    //            if (lngRequestID <= 0)
    //            {
    //                var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
    //                return StringResult(msg);
    //            }

    //            int outResponse = 0;
    //            int TotalRecord = 0;


    //            var card = CardDAO.Instance.UserCardRechargeList(lngRequestID, 0, 0, 0, 0, null, null, null, null, null, ServiceID, 1, int.MaxValue, out TotalRecord).FirstOrDefault();
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
    //            if (card.PartnerErrorCode == "-2")
    //            {
    //                var msg = String.Format("{0}|{1}", "1", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
    //                return StringResult(msg);
    //            };

    //            if (!card.ReceivedMoney.HasValue)
    //            {
    //                var msg = String.Format("{0}|{1}", "1", String.Format("A4{0}", ErrorMsg.CardInvalid));
    //                return StringResult(msg);
    //            }

    //            int status = input.Status;
    //            //nếu mã lỗi khác 1 mạng mobile phone
    //            if (status != 1)
    //            {
    //                //cập nhật ngay thành trạng thái -1
    //                CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "error", -1, PARTNER_TOW, out outResponse);
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
    //            //trường hợp này nhà mạng trẳ về 1
    //            else
    //            {
    //                outResponse = 0;
    //                //cập  nhật ngay thẻ về trạng thái 1 
    //                CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "success", 1, PARTNER_TOW ,out outResponse);
    //                if (outResponse == 1)
    //                {
    //                    // var msg = String.Format("{0}|{1}", "00", String.Format("C1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

    //                    long wallet = 0;
    //                    long tranID = 0;
    //                    var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, "Nạp thẻ cào mobile",out tranID, out wallet);

    //                    if (res != 1)//nếu không thể cộng tiền cho user
    //                    {
    //                        var msg = String.Format("{0}|{1}", "1", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
    //                        outResponse = 0;
    //                        CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "success", 3, PARTNER_TOW, out outResponse);
    //                        if (outResponse == 1)
    //                        {
    //                            msg = String.Format("{0}|{1}", "1", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //                            return  StringResult(msg);
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
    //                        CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "success", 2, PARTNER_TOW, out outResponse);

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
    //                             return StringResult(msg);
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
    //                    return StringResult( msg);
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
