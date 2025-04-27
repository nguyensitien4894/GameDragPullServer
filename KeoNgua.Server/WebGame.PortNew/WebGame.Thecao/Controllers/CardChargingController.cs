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
    [RoutePrefix("api/CardCharging")]
    public class CardChargingController : BaseApiController
    {
        private int PARTNER_ONE = 1;

        /// <summary>
        /// call back cổng thẻ 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [ActionName("ReceiveResult")]
        [HttpGet]
        //public HttpResponseMessage ReceiveResult(string card_id, string requestId, string status, long menhGiaThuc, string result)
        public HttpResponseMessage ReceiveResult(string TransID, string Amount, string ReadAmount, string Status, string Signature, string CardSeri, string CardCode)
        {
            NLogManager.LogMessage(Request.RequestUri.ToString());
      
            try
            {
                int statusCode;
                if (Status == "0" || Status == "-100")
                {
                    statusCode = 1;
                }
                else statusCode = 0;

                var msg = "";

                NLogManager.LogMessage(Request.RequestUri.ToString());


                //var arrTmp = card_id.Split('|');
                //var requestId = arrTmp[1];
                //var gameAcc = arrTmp[0];
                //var requestId = requestId;
                
                var cardValue = ReadAmount;
                //var message = result;

                var lngRequestID = ConvertUtil.ToLong(TransID);
                //var lngGameAccountID = ConvertUtil.ToLong(gameAcc);
                if (lngRequestID <= 0)
                {
                    var msgr = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
                    return StringResult(msgr);
                }
                //if (lngGameAccountID <= 0)
                //{
                //    var msgr = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
                //    return StringResult(msgr);
                //}

            

                int outResponse = 0;
                var card = CardDAO.Instance.UserCardRechargeGetByID(lngRequestID);
                var RealReceiveMoney = (long)(card.Rate * long.Parse(cardValue));
                if (card == null)
                {
                    var msgr = String.Format("{0}|{1}", "00", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
                    return StringResult(msgr);
                }

                if (card.Status != 0)
                {
                    var msgr = String.Format("{0}|{1}", "00", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
                    return StringResult(msgr);
                }
                //if (card.UserID != lngGameAccountID)
                //{
                //    var msgr = String.Format("{0}|{1}", "00", ErrorMsg.GameAccountIncorrect);
                //    return StringResult(msgr);
                //}

                if (!card.ReceivedMoney.HasValue)
                {
                    var msgr = String.Format("{0}|{1}", "00", String.Format("A4{0}", ErrorMsg.CardInvalid));
                    return StringResult(msgr);
                }
                int value = ConvertUtil.ToInt(cardValue);
                if (statusCode == 0 && card.CardValue != value)
                {
                    var msgr = String.Format("{0}|{1}", "00", String.Format("A5{0}", ErrorMsg.AmountInValid));
                    return StringResult(msgr);
                }
                if (!ArryAmountValid.Contains(value))
                {
                    var msgr = String.Format("{0}|{1}", "00", String.Format("A6{0}", ErrorMsg.AmountInValid));
                    return StringResult(msgr);
                }
                if (!ArryAmountValid.Contains(card.CardValue))
                {
                    var msgr = String.Format("{0}|{1}", "00", String.Format("A7{0}", ErrorMsg.AmountInValid));
                    return StringResult(msgr);
                }
                if (card.TeleRate < 0)
                {
                    var msgr = String.Format("{0}|{1}", "00", String.Format("A8{0}", ErrorMsg.CardRateInValidate));
                    return StringResult(msgr);
                }
                //lay a ti le  khuyen mai

                double KMRate = card.Rate - card.TeleRate;
                if (KMRate <= 0)
                {
                    KMRate = 0;

                }

                // thẻ nạp thất bại hay sai mệnh giá 
                //if (statusCode == 0 || value != card.CardValue)
                if (statusCode == 0)
                {
                    //thẻ nạp sai mệnh giá 
                    if (statusCode == 0 && value > 0 && card.CardValue > 0)//trường hợp lỗi ko sai mệnh giá
                    {
                        var minValue = card.CardValue >= value ? value : card.CardValue;
                        var receiveValue = ConvertUtil.ToLong(minValue * card.TeleRate);
                        //cập nhật lại trạng thái -3
                        CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, Status.ToString(), "", STATUS_SMG_NOT_REFUND, PARTNER_ONE, value, minValue, receiveValue, out outResponse);
                        if (outResponse == 1)
                        {
                            try
                            {
                                var cardIndex = GetCardCodeIndex(card.OperatorCode);
                                SendDNA(lngRequestID, card.UserID, cardIndex, minValue, receiveValue, false, KMRate);
                            }
                            catch (Exception ex)
                            {
                                NLogManager.PublishException(ex);
                            }
                            SendChargingHub(card.UserID, 0, STATUS_SMG_NOT_REFUND_REASON, 0, ServiceID);
                            var msgr = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatus_Negative3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                            return StringResult(msgr);
                            ///cộng tiền khi  sai mệnh giá khi

                            //long wallet = 0;
                            //long tranID = 0;
                            //string msgTC = String.Format("Nạp thẻ (SMG) phiên {0} ", lngRequestID);
                            //var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, receiveValue, msgTC, ServiceID, out tranID, out wallet);

                            //if (res != 1)//nếu không thể cộng tiền cho user
                            //{
                            //    SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                            //    var msg = String.Format("{0}|{1}", "00", String.Format("H5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
                            //    outResponse = 0;
                            //    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 3, PARTNER_ONE, out outResponse);
                            //    if (outResponse == 1)
                            //    {
                            //        msg = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                            //        return StringResult(msg);
                            //    }
                            //    else if (outResponse == -202)
                            //    {

                            //        msg = String.Format("{0}|{1}", "00", String.Format("H2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                            //        return StringResult(msg);
                            //    }
                            //    else if (outResponse == -508)
                            //    {

                            //        msg = String.Format("{0}|{1}", "00", String.Format("H3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                            //        return StringResult(msg);
                            //    }
                            //    else
                            //    {
                            //        msg = String.Format("{0}|{1}", "00", String.Format("H4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                            //        return StringResult(msg);
                            //    }

                            //}else
                            //{
                            //    //đẫ cộng tiền cho khách và 
                            //   var msg = String.Format("{0}|{1}", "00", String.Format("H6{0}", ErrorMsg.CardUpdateStatus4));// không tìm thấy user khi cập nhật trạng thái 1
                            //    return StringResult(msg);
                            //}

                        }
                        else
                        {

                            var msgr = String.Format("{0}|{1}", "00", String.Format("H7{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -3
                            return StringResult(msgr);
                        }

                    }
                    else
                    {
                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                        CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, Status.ToString(), "", -1, PARTNER_ONE, value, null, null, out outResponse);
                        if (outResponse == 1)
                        {
                            var msgr = String.Format("{0}|{1}", "00", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                            return StringResult(msgr);
                        }
                        else if (outResponse == -202)
                        {

                            var msgr = String.Format("{0}|{1}", "00", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
                            return StringResult(msgr);
                        }
                        else if (outResponse == -508)
                        {

                            var msgr = String.Format("{0}|{1}", "00", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
                            return StringResult(msgr);
                        }
                        else
                        {
                            var msgr = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
                            return StringResult(msgr);
                        }
                    }

                }
                //trường hợp này nhà mạng viettle trả về 00
                else
                {
                    outResponse = 0;

                    CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, Status.ToString(), "", 1, PARTNER_ONE, value, null, null, out outResponse);
                    if (outResponse == 1)
                    {
                        // var msg = String.Format("{0}|{1}", "00", String.Format("C1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

                        long wallet = 0;
                        long tranID = 0;
                        string msgTC = String.Format("Nạp thẻ phiên {0} ", lngRequestID);
                        var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, RealReceiveMoney, msgTC, ServiceID, out tranID, out wallet);

                        if (res != 1)//nếu không thể cộng tiền cho user
                        {
                            SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                            var msgr = String.Format("{0}|{1}", "00", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
                            outResponse = 0;
                            CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, Status.ToString(), "", 3, PARTNER_ONE, out outResponse);
                            if (outResponse == 1)
                            {
                                msg = String.Format("{0}|{1}", "00", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                                return StringResult(msg);
                            }
                            else if (outResponse == -202)
                            {

                                msg = String.Format("{0}|{1}", "00", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                                return StringResult(msg);
                            }
                            else if (outResponse == -508)
                            {

                                msg = String.Format("{0}|{1}", "00", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                                return StringResult(msg);
                            }
                            else
                            {
                                msg = String.Format("{0}|{1}", "00", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                                return StringResult(msg);
                            }


                        }
                        else
                        {

                            //cập nhật 2 (đã nạp tiền thành công)
                            outResponse = 0;
                            CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, Status.ToString(), "", 2, PARTNER_ONE, out outResponse);

                            if (outResponse == 1)
                            {
                                SendChargingHub(card.UserID, RealReceiveMoney, "Nạp thẻ thành công", 1, ServiceID);
                                try
                                {
                                    var cardIndex = GetCardCodeIndex(card.OperatorCode);
                                    SendDNA(lngRequestID, card.UserID, cardIndex, RealReceiveMoney, RealReceiveMoney, true, KMRate);
                                }
                                catch (Exception ex)
                                {
                                    NLogManager.PublishException(ex);
                                }

                                var msgr = String.Format("{0}|{1}", "00", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang



                                return StringResult(msgr);
                            }
                            else if (outResponse == -202)
                            {
                                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                                var msgr = String.Format("{0}|{1}", "00", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                                return StringResult(msgr);
                            }
                            else if (outResponse == -508)
                            {
                                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                                var msgr = String.Format("{0}|{1}", "00", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                                return StringResult(msgr);
                            }
                            else
                            {
                                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                                var msgr = String.Format("{0}|{1}", "00", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                                return StringResult(msgr);
                            }
                        }
                    }
                    else if (outResponse == -202)
                    {
                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                        var msgr = String.Format("{0}|{1}", "00", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                        return StringResult(msgr);
                    }
                    else if (outResponse == -508)
                    {
                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                        var msgr = String.Format("{0}|{1}", "00", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                        return StringResult(msgr);
                    }
                    else
                    {
                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                        var msgr = String.Format("{0}|{1}", "00", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                        return StringResult(msgr);
                    }

                }


            }
            catch (Exception ex)
            {
                var msgr = String.Format("{0}|{1}", "09", ErrorMsg.CardBusy);

                return StringResult(msgr);
            }

        }

        //public HttpResponseMessage ReceiveResultOld(string billcode, int status, long amount, string msg)
        //{
        //    NLogManager.LogMessage(Request.RequestUri.ToString());

        //    try
        //    {

        //        NLogManager.LogMessage(Request.RequestUri.ToString());


        //        var arrTmp = billcode.Split('|');
        //        var requestId = arrTmp[1];
        //        var gameAcc = arrTmp[0];
        //        var cardValue = amount;
        //        var message = msg;

        //        var lngRequestID = ConvertUtil.ToLong(requestId);
        //        var lngGameAccountID = ConvertUtil.ToLong(gameAcc);
        //        if (lngRequestID <= 0)
        //        {
        //            var msgr = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
        //            return StringResult(msgr);
        //        }
        //        if (lngGameAccountID <= 0)
        //        {
        //            var msgr = String.Format("{0}|{1}", "01", ErrorMsg.ParamaterInvalid);
        //            return StringResult(msgr);
        //        }



        //        int outResponse = 0;
        //        var card = CardDAO.Instance.UserCardRechargeGetByID(lngRequestID);
        //        if (card == null)
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
        //            return StringResult(msgr);
        //        }

        //        if (card.Status != 0)
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
        //            return StringResult(msgr);
        //        }
        //        if (card.UserID != lngGameAccountID)
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", ErrorMsg.GameAccountIncorrect);
        //            return StringResult(msgr);
        //        }

        //        if (!card.ReceivedMoney.HasValue)
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", String.Format("A4{0}", ErrorMsg.CardInvalid));
        //            return StringResult(msgr);
        //        }
        //        int value = ConvertUtil.ToInt(cardValue);
        //        if (status == 0 && card.CardValue != value)
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", String.Format("A5{0}", ErrorMsg.AmountInValid));
        //            return StringResult(msgr);
        //        }
        //        if (!ArryAmountValid.Contains(value))
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", String.Format("A6{0}", ErrorMsg.AmountInValid));
        //            return StringResult(msgr);
        //        }
        //        if (!ArryAmountValid.Contains(card.CardValue))
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", String.Format("A7{0}", ErrorMsg.AmountInValid));
        //            return StringResult(msgr);
        //        }
        //        if (card.TeleRate < 0)
        //        {
        //            var msgr = String.Format("{0}|{1}", "00", String.Format("A8{0}", ErrorMsg.CardRateInValidate));
        //            return StringResult(msgr);
        //        }
        //        //lay a ti le  khuyen mai

        //        double KMRate = card.Rate - card.TeleRate;
        //        if (KMRate <= 0)
        //        {
        //            KMRate = 0;

        //        }

        //        // thẻ nạp thất bại hay sai mệnh giá 
        //        if (status == 0 || value != card.CardValue)
        //        {
        //            //thẻ nạp sai mệnh giá 
        //            if (status == 0 && value > 0 && card.CardValue > 0)//trường hợp lỗi ko sai mệnh giá
        //            {
        //                var minValue = card.CardValue >= value ? value : card.CardValue;
        //                var receiveValue = ConvertUtil.ToLong(minValue * card.TeleRate);
        //                //cập nhật lại trạng thái -3
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, status.ToString(), message, STATUS_SMG_NOT_REFUND, PARTNER_ONE, value, minValue, receiveValue, out outResponse);
        //                if (outResponse == 1)
        //                {
        //                    try
        //                    {
        //                        var cardIndex = GetCardCodeIndex(card.OperatorCode);
        //                        SendDNA(lngRequestID, card.UserID, cardIndex, minValue, receiveValue, false, KMRate);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        NLogManager.PublishException(ex);
        //                    }
        //                    SendChargingHub(card.UserID, 0, STATUS_SMG_NOT_REFUND_REASON, 0, ServiceID);
        //                    var msgr = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatus_Negative3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                    return StringResult(msgr);
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
        //                    //    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, errorCode, message, 3, PARTNER_ONE, out outResponse);
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

        //                    //}else
        //                    //{
        //                    //    //đẫ cộng tiền cho khách và 
        //                    //   var msg = String.Format("{0}|{1}", "00", String.Format("H6{0}", ErrorMsg.CardUpdateStatus4));// không tìm thấy user khi cập nhật trạng thái 1
        //                    //    return StringResult(msg);
        //                    //}

        //                }
        //                else
        //                {

        //                    var msgr = String.Format("{0}|{1}", "00", String.Format("H7{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -3
        //                    return StringResult(msgr);
        //                }

        //            }
        //            else
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, status.ToString(), message, -1, PARTNER_ONE, value, null, null, out outResponse);
        //                if (outResponse == 1)
        //                {
        //                    var msgr = String.Format("{0}|{1}", "00", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
        //                    return StringResult(msgr);
        //                }
        //                else if (outResponse == -202)
        //                {

        //                    var msgr = String.Format("{0}|{1}", "00", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return StringResult(msgr);
        //                }
        //                else if (outResponse == -508)
        //                {

        //                    var msgr = String.Format("{0}|{1}", "00", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return StringResult(msgr);
        //                }
        //                else
        //                {
        //                    var msgr = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
        //                    return StringResult(msgr);
        //                }
        //            }

        //        }
        //        //trường hợp này nhà mạng viettle trả về 00
        //        else
        //        {
        //            outResponse = 0;

        //            CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, status.ToString(), message, 1, PARTNER_ONE, value, null, null, out outResponse);
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
        //                    var msgr = String.Format("{0}|{1}", "00", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
        //                    outResponse = 0;
        //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), message, 3, PARTNER_ONE, out outResponse);
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
        //                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), message, 2, PARTNER_ONE, out outResponse);

        //                    if (outResponse == 1)
        //                    {
        //                        SendChargingHub(card.UserID, wallet, "Nạp thẻ thành công", 1, ServiceID);
        //                        try
        //                        {
        //                            var cardIndex = GetCardCodeIndex(card.OperatorCode);
        //                            SendDNA(lngRequestID, card.UserID, cardIndex, card.CardValue, card.ReceivedMoney.Value, true, KMRate);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            NLogManager.PublishException(ex);
        //                        }

        //                        var msgr = String.Format("{0}|{1}", "00", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang



        //                        return StringResult(msgr);
        //                    }
        //                    else if (outResponse == -202)
        //                    {
        //                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                        var msgr = String.Format("{0}|{1}", "00", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msgr);
        //                    }
        //                    else if (outResponse == -508)
        //                    {
        //                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                        var msgr = String.Format("{0}|{1}", "00", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msgr);
        //                    }
        //                    else
        //                    {
        //                        SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                        var msgr = String.Format("{0}|{1}", "00", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                        return StringResult(msgr);
        //                    }
        //                }
        //            }
        //            else if (outResponse == -202)
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                var msgr = String.Format("{0}|{1}", "00", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msgr);
        //            }
        //            else if (outResponse == -508)
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                var msgr = String.Format("{0}|{1}", "00", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msgr);
        //            }
        //            else
        //            {
        //                SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
        //                var msgr = String.Format("{0}|{1}", "00", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
        //                return StringResult(msgr);
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        var msgr = String.Format("{0}|{1}", "09", ErrorMsg.CardBusy);

        //        return StringResult(msgr);
        //    }

        //}


    }
}
