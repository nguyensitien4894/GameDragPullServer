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
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/CardChargingFunny")]
    public class CardChargingFunnyController : BaseApiController
    {
        private int PARTNER_FOUR = 4;




        /// <summary>
        /// call back cổng thẻ 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //[ActionName("ReceiveResult")]
        //[HttpGet]
        //public HttpResponseMessage ReceiveResult(string data)
        //{
        //    try
        //    {

        //        NLogManager.LogMessage(Request.RequestUri.ToString());
        //        if (String.IsNullOrEmpty(data))
        //        {
        //            var msg = (String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid));
        //            return StringResult(msg);
        //        }
        //        var plainTex = CardHelper.Base64Decode(data);
        //        var arrTmp = plainTex.Split('|');

        //        var requestId = arrTmp[0];
        //        var gameAcc = arrTmp[1];
        //        var cardValue = arrTmp[2];
        //        var errorCode = arrTmp[3];
        //        var message = arrTmp[4];
        //        var type = arrTmp[5];
        //        var accessKey = arrTmp[6];
        //        var signature = arrTmp[7];
        //        if (String.IsNullOrEmpty(requestId) || String.IsNullOrEmpty(gameAcc) || String.IsNullOrEmpty(cardValue)
        //            || String.IsNullOrEmpty(errorCode) || String.IsNullOrEmpty(message) || String.IsNullOrEmpty(type) || String.IsNullOrEmpty(accessKey) || String.IsNullOrEmpty(signature))
        //        {

        //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
        //            //return StringLogResult(PARTNER_ONE, requestId,)
        //            return StringResult(msg);
        //        }
        //        var lngRequestID = ConvertUtil.ToLong(requestId);
        //        var lngGameAccountID = ConvertUtil.ToLong(gameAcc);
        //        if (lngRequestID <= 0)
        //        {
        //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
        //            return StringResult(msg);
        //        }
        //        if (lngGameAccountID <= 0)
        //        {
        //            var msg = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
        //            return StringResult(msg);
        //        }

        //        //so sanh sig
        //        string secretKey = ConfigurationManager.AppSettings["CARD_SECREATEKEY_P4"].ToString();
        //        var plainText = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", requestId, gameAcc, cardValue, errorCode, message, type, accessKey);
        //        var tmpSig = CardHelper.GetHashHMACSHA256(plainText, secretKey);
        //        if (tmpSig != signature)
        //        {
        //            var msg = String.Format("{0}|{1}", "03", ErrorMsg.SignatureIncorrect);
        //            return StringResult(msg);
        //        }

        //        int outResponse = 0;
        //        var card = CardDAO.Instance.UserCardRechargeGetByID(lngRequestID);
        //        if (card == null)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
        //            return StringResult(msg);
        //        }

        //        if (card.Status != 0)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
        //            return StringResult(msg);
        //        }
        //        if (card.UserID != lngGameAccountID)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", ErrorMsg.GameAccountIncorrect);
        //            return StringResult(msg);
        //        }

        //        if (!card.ReceivedMoney.HasValue)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A4{0}", ErrorMsg.CardInvalid));
        //            return StringResult(msg);
        //        }
        //        int value = ConvertUtil.ToInt(cardValue);
        //        if (errorCode == "00" && card.CardValue != value)
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A5{0}", ErrorMsg.AmountInValid));
        //            return StringResult(msg);
        //        }
        //        if (!ArryAmountValid.Contains(value))
        //        {
        //            var msg = String.Format("{0}|{1}", "00", String.Format("A6{0}", ErrorMsg.AmountInValid));
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
        //        // thẻ nạp thất bại hay sai mệnh giá 
        //        if (errorCode != "00" || value != card.CardValue)
        //        {
        //            if (errorCode == "10" && value > 0 && card.CardValue > 0)//trường hợp lỗi ko sai mệnh giá
        //            {
        //                var minValue = card.CardValue >= value ? value : card.CardValue;
        //                var receiveValue = ConvertUtil.ToLong(minValue * card.TeleRate);
        //                //cập nhật lại trạng thái 4
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, errorCode, message, STATUS_SMG_NOT_REFUND, PARTNER_FOUR, value, minValue, receiveValue, out outResponse);
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
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatus_Negative3));
        //                    return StringResult(msg);
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
        //                    //    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 3, PARTNER_FOUR, out outResponse);
        //                    //    if (outResponse == 1)
        //                    //    {
        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                    //        return StringResult(msg);
        //                    //    }
        //                    //    else if (outResponse == -202)
        //                    //    {

        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //        return StringResult(msg);
        //                    //    }
        //                    //    else if (outResponse == -508)
        //                    //    {

        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //        return StringResult(msg);
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        msg = String.Format("{0}|{1}", "00", String.Format("H4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //        return StringResult(msg);
        //                    //    }

        //                    //}
        //                    //else
        //                    //{
        //                    //    //đẫ cộng tiền cho khách và 
        //                    //    var msg = String.Format("{0}|{1}", "00", String.Format("H6{0}", ErrorMsg.CardUpdateStatus4));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //    return StringResult(msg);
        //                    //}

        //                }
        //                else
        //                {

        //                    var msg = String.Format("{0}|{1}", "00", String.Format("H7{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -3
        //                    return StringResult(msg);
        //                }

        //            }
        //            else
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, errorCode, message, -1, PARTNER_FOUR, value, null, null, out outResponse);
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

        //        }
        //        //trường hợp này nhà mạng viettle trả về 00
        //        else
        //        {
        //            outResponse = 0;

        //            CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, errorCode.ToString(), message, 1, PARTNER_FOUR, value, null, null, out outResponse);
        //            if (outResponse == 1)
        //            {
        //                // var msg = String.Format("{0}|{1}", "00", String.Format("C1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

        //                long wallet = 0;
        //                long tranID = 0;
        //                string msgTC = String.Format("Nạp thẻ phiên {0} ", lngRequestID);
        //                var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, msgTC, ServiceID, out tranID, out wallet);

        //                if (res != 1)//nếu không thể cộng tiền cho user
        //                {
        //                    SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                    var msg = String.Format("{0}|{1}", "00", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
        //                    outResponse = 0;
        //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 3, PARTNER_FOUR, out outResponse);
        //                    if (outResponse == 1)
        //                    {
        //                        msg = String.Format("{0}|{1}", "00", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -202)
        //                    {

        //                        msg = String.Format("{0}|{1}", "00", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -508)
        //                    {

        //                        msg = String.Format("{0}|{1}", "00", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else
        //                    {
        //                        msg = String.Format("{0}|{1}", "00", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }


        //                }
        //                else
        //                {

        //                    //cập nhật 2 (đã nạp tiền thành công)
        //                    outResponse = 0;
        //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 2, PARTNER_FOUR, out outResponse);

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


        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -202)
        //                    {
        //                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                        var msg = String.Format("{0}|{1}", "00", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else if (outResponse == -508)
        //                    {
        //                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                        var msg = String.Format("{0}|{1}", "00", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                    else
        //                    {
        //                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                        var msg = String.Format("{0}|{1}", "00", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msg);
        //                    }
        //                }
        //            }
        //            else if (outResponse == -202)
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                var msg = String.Format("{0}|{1}", "00", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msg);
        //            }
        //            else if (outResponse == -508)
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                var msg = String.Format("{0}|{1}", "00", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msg);
        //            }
        //            else
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                var msg = String.Format("{0}|{1}", "00", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msg);
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = String.Format("{0}|{1}", "09", ErrorMsg.CardBusy);

        //        return StringResult(msg);
        //    }

        //}



    }
}
