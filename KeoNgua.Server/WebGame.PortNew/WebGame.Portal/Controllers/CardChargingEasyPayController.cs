namespace MsWebGame.Portal.Controllers
{
    //[RoutePrefix("api/CardChargingEasyPay")]
    //public class CardChargingEasyPayController : ApiController
    //{
    //    private int PARTNER_THREE = 3;

    //    //[ActionName("ReceiveResult")]
    //    //[HttpGet]
    //    //public HttpResponseMessage ReceiveResult(string data)
    //    //{
    //    //    try
    //    //    {
    //    //        if (String.IsNullOrEmpty(data))
    //    //        {
    //    //            var msg = (String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid));
    //    //            return StringResult(msg);
    //    //        }
    //    //        var plainTex = CardHelper.Base64Decode(data);
    //    //        var arrTmp = plainTex.Split('|');

    //    //        var requestId = arrTmp[0];
    //    //        var gameAcc = arrTmp[1];
    //    //        var cardValue = arrTmp[2];
    //    //        var errorCode = arrTmp[3];
    //    //        var message = arrTmp[4];
    //    //        var type = arrTmp[5];
    //    //        var accessKey = arrTmp[6];
    //    //        var signature = arrTmp[7];
    //    //        if (String.IsNullOrEmpty(requestId) || String.IsNullOrEmpty(gameAcc) || String.IsNullOrEmpty(cardValue)
    //    //            || String.IsNullOrEmpty(errorCode) || String.IsNullOrEmpty(message) || String.IsNullOrEmpty(type) || String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(signature))
    //    //        {

    //    //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
    //    //            return StringResult(msg);
    //    //        }
    //    //        var lngRequestID = ConvertUtil.ToLong(requestId);
    //    //        var lngGameAccountID = ConvertUtil.ToLong(gameAcc);
    //    //        if (lngRequestID <= 0)
    //    //        {
    //    //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
    //    //            return StringResult(msg);
    //    //        }
    //    //        if (lngGameAccountID <= 0)
    //    //        {
    //    //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
    //    //            return StringResult(msg);
    //    //        }

    //    //        //so sanh sig
    //    //        string secretKey = ConfigurationManager.AppSettings["EasyPay_CARD_SECREATEKEY"].ToString();
    //    //        var plainText = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", requestId, gameAcc, cardValue, errorCode, message, type, accessKey);
    //    //        var tmpSig = CardHelper.GetHashHMACSHA256(plainText, secretKey);
    //    //        if (tmpSig != signature)
    //    //        {
    //    //            var msg = String.Format("{0}|{1}", "03", ErrorMsg.SignatureIncorrect);
    //    //            return StringResult(msg);
    //    //        }

    //    //        int outResponse = 0;


    //    //        int TotalRecord = 0;

    //    //        //lấy ra object thẻ request
    //    //        var card = CardDAO.Instance.UserCardRechargeList(lngRequestID, 0, 0, 0, 0, null, null, null, null, null, 1, int.MaxValue, out TotalRecord).FirstOrDefault();
    //    //        if (card == null)
    //    //        {
    //    //            var msg = String.Format("{0}|{1}", "00", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
    //    //            return StringResult(msg);
    //    //        }

    //    //        if (card.Status != 0)
    //    //        {
    //    //            var msg = String.Format("{0}|{1}", "00", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
    //    //            return StringResult(msg);
    //    //        }
    //    //        if (card.UserID != lngGameAccountID)
    //    //        {
    //    //            var msg = String.Format("{0}|{1}", "00", ErrorMsg.GameAccountIncorrect);
    //    //            return StringResult(msg);
    //    //        }

    //    //        if (!card.ReceivedMoney.HasValue)
    //    //        {
    //    //            var msg = String.Format("{0}|{1}", "00", String.Format("A4{0}", ErrorMsg.CardInvalid));
    //    //            return StringResult(msg);
    //    //        }
    //    //        //nếu mã lỗi khác 00 với nhà mạng viettel và mã lỗi khác 1 với nhà mạng Vina phone
    //    //        if (errorCode != "00")
    //    //        {
    //    //            //cập nhật ngay thành trạng thái -1
    //    //            CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, -1, PARTNER_THREE, out outResponse);
    //    //            if (outResponse == 1)
    //    //            {
    //    //                var msg = String.Format("{0}|{1}", "00", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //    //                return StringResult(msg);
    //    //            }
    //    //            else if (outResponse == -202)
    //    //            {

    //    //                var msg = String.Format("{0}|{1}", "00", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
    //    //                return StringResult(msg);
    //    //            }
    //    //            else if (outResponse == -508)
    //    //            {

    //    //                var msg = String.Format("{0}|{1}", "00", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
    //    //                return StringResult(msg);
    //    //            }
    //    //            else
    //    //            {
    //    //                var msg = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
    //    //                return StringResult(msg);
    //    //            }
    //    //        }
    //    //        //trường hợp này nhà mạng viettle trả về 00
    //    //        else
    //    //        {
    //    //            outResponse = 0;
    //    //            //cập  nhật ngay thẻ về trạng thái 1 
    //    //            CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 1, PARTNER_THREE, out outResponse);
    //    //            if (outResponse == 1)
    //    //            {
    //    //                // var msg = String.Format("{0}|{1}", "00", String.Format("C1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

    //    //                long wallet = 0;
    //    //                long tranID = 0;
    //    //                var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, "Nạp thẻ cào ", out tranID, out wallet);

    //    //                if (res != 1)//nếu không thể cộng tiền cho user
    //    //                {
    //    //                    var msg = String.Format("{0}|{1}", "00", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
    //    //                    outResponse = 0;
    //    //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 3, PARTNER_THREE, out outResponse);
    //    //                    if (outResponse == 1)
    //    //                    {
    //    //                        msg = String.Format("{0}|{1}", "00", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
    //    //                        return StringResult(msg);
    //    //                    }
    //    //                    else if (outResponse == -202)
    //    //                    {

    //    //                        msg = String.Format("{0}|{1}", "00", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                        return StringResult(msg);
    //    //                    }
    //    //                    else if (outResponse == -508)
    //    //                    {

    //    //                        msg = String.Format("{0}|{1}", "00", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                        return StringResult(msg);
    //    //                    }
    //    //                    else
    //    //                    {
    //    //                        msg = String.Format("{0}|{1}", "00", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                        return StringResult(msg);
    //    //                    }


    //    //                }
    //    //                else
    //    //                {

    //    //                    //cập nhật 2 (đã nạp tiền thành công)
    //    //                    outResponse = 0;
    //    //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 2, PARTNER_THREE, out outResponse);
     
    //    //                    if (outResponse == 1)
    //    //                    {
    //    //                        SendChargeToHub(card.UserID, wallet, "Nạp thẻ thành công");
    //    //                        var msg = String.Format("{0}|{1}", "00", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang



    //    //                        return StringResult(msg);
    //    //                    }
    //    //                    else if (outResponse == -202)
    //    //                    {

    //    //                        var msg = String.Format("{0}|{1}", "00", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                        return StringResult(msg);
    //    //                    }
    //    //                    else if (outResponse == -508)
    //    //                    {

    //    //                        var msg = String.Format("{0}|{1}", "00", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                        return StringResult(msg);
    //    //                    }
    //    //                    else
    //    //                    {
    //    //                        var msg = String.Format("{0}|{1}", "00", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                        return StringResult(msg);
    //    //                    }
    //    //                }
    //    //            }
    //    //            else if (outResponse == -202)
    //    //            {

    //    //                var msg = String.Format("{0}|{1}", "00", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                return StringResult(msg);
    //    //            }
    //    //            else if (outResponse == -508)
    //    //            {

    //    //                var msg = String.Format("{0}|{1}", "00", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                return StringResult(msg);
    //    //            }
    //    //            else
    //    //            {
    //    //                var msg = String.Format("{0}|{1}", "00", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
    //    //                return StringResult(msg);
    //    //            }

    //    //        }


    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        var msg = String.Format("{0}|{1}", "09", ErrorMsg.CardBusy);

    //    //        return StringResult(msg);
    //    //    }

    //    //}

    //    //private bool SendChargeToHub(long accountId, long balance, string msg)
    //    //{
    //    //    try
    //    //    {
    //    //        NLogManager.LogMessage(string.Format("Call Out :accountId: {0} | balance: {1} | msg: {2} ", accountId, balance, msg));
    //    //        PortalHandler.Instance.ReturnTopupCard(accountId, balance, msg);


    //    //        return true;
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        NLogManager.LogMessage(string.Format("Exception Call Out :accountId: {0} | balance: {1} | msg: {2} ", accountId, balance, msg));
    //    //        NLogManager.PublishException(ex);
    //    //        return false;
    //    //    }
    //    //}
    //    //private HttpResponseMessage StringResult(string msg)
    //    //{
    //    //    return new HttpResponseMessage()
    //    //    {
    //    //        Content = new StringContent(msg, Encoding.UTF8, "text/plain")
    //    //    };
    //    //}
    //}
}
