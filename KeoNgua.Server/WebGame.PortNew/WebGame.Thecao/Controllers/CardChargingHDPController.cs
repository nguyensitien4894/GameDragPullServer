using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MsWebGame.Thecao.Database.DAO;
using MsWebGame.Thecao.Helpers.Chargings.Cards;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Momos.Api.Charges;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/CardChargingHDP")]
    public class CardChargingHDPController : BaseApiController
    {
        //// GET api/<controller>
        [ActionName("CallBackResult")]
        [HttpGet]
        public HttpResponseMessage CallBackResult(string username, string password, string status, string telco, string amount, string requestid, string serial, string signature)
        {
            NLogManager.LogMessage("CallBackResult.Status : " + status + "-refcode: " + requestid);
            try
            {
                string secretKey = System.Configuration.ConfigurationManager.AppSettings["HDP_SECRETKEY"];
                string IpsCard = System.Configuration.ConfigurationManager.AppSettings["IpsCard"];
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
             
                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                NLogManager.LogMessage("BankCallBackResult-IP: " + ip + " IpsCard:"+ IpsCard);
                if (!string.IsNullOrEmpty(IpsCard) && ip != IpsCard)
                {
                    var msg = String.Format("{0}|{1}", "-1", "Ip Invalid");
                    return StringResult(msg);
                }
                if (String.IsNullOrEmpty(requestid))
                {
                    NLogManager.LogMessage("requestid null");
                    var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
                    return StringResult(msg);
                }
                if (status != "1" && status != "-1")
                {
                    NLogManager.LogMessage("status Fail");
                    var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
                    return StringResult(msg);
                }

                if (string.IsNullOrEmpty(signature))
                {
                    NLogManager.LogMessage("signature null");
                    var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
                    return StringResult(msg);
                }

                string content = username + password + requestid + serial + secretKey;
                string checkSum = MomoChargeApi.MD5(content);
                if (checkSum != signature)
                {
                    NLogManager.LogMessage("signature Fail");
                    var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
                    return StringResult(msg);
                }
                //string partnerKey = ConfigurationManager.AppSettings["SHOPMUATHE_KEY"].ToString();
                //         var current_signature = Encrypts.MD5(input.RefCode +input.Status +input.Amount  + partnerKey);


                //if (current_signature!=input.Signature){
                //    var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
                //    return StringResult(msg);
                //}
                var lngRequestID = ConvertUtil.ToLong(requestid);
                var realvalue = ConvertUtil.ToInt(amount);
                if (lngRequestID <= 0)
                {
                    var msg = String.Format("{0}|{1}", "-1", ErrorMsg.ParamaterInvalid);
                    return StringResult(msg);
                }

                int outResponse = 0;



                // var card = CardDAO.Instance.UserCardRechargeList(lngRequestID, 0, 0, 0, 0, null, null, null, null, null, 1, int.MaxValue, out TotalRecord).FirstOrDefault();
                var card = CardDAO.Instance.UserCardRechargeGetByID(lngRequestID);
                if (card == null)
                {
                    var msg = String.Format("{0}|{1}", "-1", String.Format("A1{0}", ErrorMsg.ParamaterInvalid));
                    return StringResult(msg);
                }
                var notAcceptStatus = new List<int> { 2, 4, -3 };
                if (notAcceptStatus.Contains(card.Status))
                {
                    var msg = String.Format("{0}|{1}", "1", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
                    return StringResult(msg);
                }

                //if (card.PartnerErrorCode == "-2")
                //{
                //    var msg = String.Format("{0}|{1}", "1", String.Format("A2{0}", ErrorMsg.CardIsUpdateStatus));
                //    return StringResult(msg);
                //};

                if (!card.ReceivedMoney.HasValue)
                {
                    var msg = String.Format("{0}|{1}", "1", String.Format("A4{0}", ErrorMsg.CardInvalid));
                    return StringResult(msg);
                }

                //nếu mã lỗi khác 1 mạng mobile phone
                if (status != "1")
                {
                    //cập nhật ngay thành trạng thái -1
                    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "error", -1, 1, out outResponse);
                    if (outResponse == 1)
                    {
                        var msg = String.Format("{0}|{1}", "1", String.Format("B1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                        return StringResult(msg);
                    }
                    else if (outResponse == -202)
                    {

                        var msg = String.Format("{0}|{1}", "1", String.Format("B2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái -1
                        return StringResult(msg);
                    }
                    else if (outResponse == -508)
                    {

                        var msg = String.Format("{0}|{1}", "1", String.Format("B3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái -1
                        return StringResult(msg);
                    }
                    else
                    {
                        var msg = String.Format("{0}|{1}", "00", String.Format("B4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -1
                        return StringResult(msg);
                    }
                }
                //trường hợp này nhà mạng trẳ về 1
                else
                {
                    if (card.CardValue != realvalue && realvalue > 0 && status == "1")
                    {
                        int value = realvalue;
                        var minValue = card.CardValue >= value ? value : card.CardValue;
                        var receiveValue = ConvertUtil.ToLong(minValue * card.TeleRate);
                        //cập nhật lại trạng thái 4
                        CardRefDAO.Instance.UpdateCardChardRefStatus(lngRequestID, card.UserID, status.ToString(), "Thành công" + " Sai mệnh giá", STATUS_SMG_NOT_REFUND, 1, value, minValue, receiveValue, out outResponse);
                        NLogManager.LogMessage("card.RequestID:" + card.RequestID.ToString() + "-card.UserID:" + card.UserID.ToString() + "-outResponse:" + outResponse.ToString());
                        if (outResponse == 1)
                        {
                            try
                            {
                                var cardIndex = GetCardCodeIndex(card.OperatorCode);
                                SendDNA(card.RequestID, card.UserID, cardIndex, minValue, receiveValue, false, 0);
                            }
                            catch (Exception ex)
                            {
                                NLogManager.PublishException(ex);
                            }
                            SendChargingHub(card.UserID, 0, STATUS_SMG_NOT_REFUND_REASON, 0, ServiceID);
                            var msg = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatus_Negative3));
                            return StringResult(msg);
                        }
                        else
                        {
                            var msg = String.Format("{0}|{1}", "00", String.Format("H2{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -3
                            return StringResult(msg);
                        }
                    }
                    else
                    {
                        outResponse = 0;
                        //cập  nhật ngay thẻ về trạng thái 1 
                        CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "success", 1, 1, out outResponse);
                        if (outResponse == 1)
                        {
                            // var msg = String.Format("{0}|{1}", "00", String.Format("C1{0}", ErrorMsg.CardUpdateStatusOKMinus_1));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang

                            long wallet = 0;
                            long tranID = 0;
                            var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, card.ReceivedMoney.Value, "Nạp thẻ cào mobile", ServiceID, out tranID, out wallet);

                            if (res != 1)//nếu không thể cộng tiền cho user
                            {
                                var msg = String.Format("{0}|{1}", "1", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
                                outResponse = 0;
                                CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "success", 3, 1, out outResponse);
                                if (outResponse == 1)
                                {
                                    msg = String.Format("{0}|{1}", "1", String.Format("D1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                                    return StringResult(msg);
                                }
                                else if (outResponse == -202)
                                {

                                    msg = String.Format("{0}|{1}", "1", String.Format("D2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                                    return StringResult(msg);
                                }
                                else if (outResponse == -508)
                                {

                                    msg = String.Format("{0}|{1}", "1", String.Format("D3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                                    return StringResult(msg);
                                }
                                else
                                {
                                    msg = String.Format("{0}|{1}", "1", String.Format("D4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                                    return StringResult(msg);
                                }


                            }
                            else
                            {

                                //cập nhật 2 (đã nạp tiền thành công)
                                outResponse = 0;
                                CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, status.ToString(), "success", 2, 1, out outResponse);

                                if (outResponse == 1)
                                {
                                    SendChargingHub(card.UserID, wallet, "Nạp thẻ thành công", 1, ServiceID);
                                    var msg = String.Format("{0}|{1}", "1", String.Format("E1{0}", ErrorMsg.CardUpdateStatus_2));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                                    try
                                    {
                                        string msgPush = "Tài khoản có mã : " + card.UserID + " Nạp Thẻ cào seri:"+ serial + " "+ " số tiền :" + realvalue;
                                        SendTelePush(msgPush, 9);
                                    }
                                    catch
                                    {

                                    }
                                    
                                    return StringResult(msg);
                                }
                                else if (outResponse == -202)
                                {

                                    var msg = String.Format("{0}|{1}", "1", String.Format("E2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                                    return StringResult(msg);
                                }
                                else if (outResponse == -508)
                                {

                                    var msg = String.Format("{0}|{1}", "1", String.Format("E3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                                    return StringResult(msg);
                                }
                                else
                                {
                                    var msg = String.Format("{0}|{1}", "1", String.Format("E4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                                    return StringResult(msg);
                                }
                            }
                        }
                        else if (outResponse == -202)
                        {

                            var msg = String.Format("{0}|{1}", "1", String.Format("C2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                            return StringResult(msg);
                        }
                        else if (outResponse == -508)
                        {

                            var msg = String.Format("{0}|{1}", "1", String.Format("C3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                            return StringResult(msg);
                        }
                        else
                        {
                            var msg = String.Format("{0}|{1}", "1", String.Format("C4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                            return StringResult(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = String.Format("{0}|{1}", "09", ErrorMsg.CardBusy);

                return StringResult(msg);
            }

        }
    }
}