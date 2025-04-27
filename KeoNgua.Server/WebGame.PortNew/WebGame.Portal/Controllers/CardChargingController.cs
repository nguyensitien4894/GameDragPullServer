using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Handlers;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Helpers.Chargings.Cards;
using MsWebGame.Portal.Helpers.Chargings.FconnClub;
using MsWebGame.Portal.Helpers.Chargings.NewBoc;

using MsWebGame.Portal.Helpers.Chargings.MobileSMS;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using MsWebGame.Portal.ShopMuaThe;
using TraditionGame.Utilities.DNA;
using System.Threading;
using MsWebGame.Portal.Helpers.Chargings.HaDongPho;
using TraditionGame.Utilities.XBomms;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/CardCharging")]
    public class CardChargingController : BaseApiController
    {
        private int PARTNER_ONE = 1;
        private int PARTNER_TOW = 2;
        private int PARTNER_THREE = 3;
        private int PARTNER_FOUR = 4;
        private int PARTNER_FIVE = 5;

        #region cardcode
        private string CardCodeVTT = "VTT";
        private string CardCodeVNP = "VNP";
        private string CardCodeVMS = "MPB";
        private string CardCodeZING = "ZING";
        private string CardCodeVCOIN = "VCOIN";
        private double Rate50 = 1.2;
        private int MAX50 = 50000;
        #endregion
        [ActionName("GetListCard")]
        [HttpGet]
        public dynamic GetListCard()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }

                var list = CardDAO.Instance.GetCardList(null, ServiceID);
                var listTelecomFast = CardDAO.Instance.GetTeleFast(ServiceID);
                string Fast = string.Empty;
                if (listTelecomFast != null && listTelecomFast.Any())
                {
                    var entity = listTelecomFast.FirstOrDefault();
                    if (entity != null)
                    {
                        Fast = entity.OperatorCode;
                    }

                }
                // VIP
                var active = ConfigurationManager.AppSettings["CARD_VIP_ACTIVE"] == null ? "" : ConfigurationManager.AppSettings["CARD_VIP_ACTIVE"].ToString();

                int total = 0;
                if (active == "1")
                {
                    total = VIPDAO.Instance.VIPCardBonusCheckQuantity(accountId);
                    if (total > 0)
                    {
                        foreach (var item in list)
                        {
                            item.ChargeRate = 1;

                        }
                    }

                }
                //sửa mẹnh giá < 50k
                //foreach(var item in list)
                //{
                //    if (item.CardValue <= MAX50)
                //    {
                //        item.ChargeRate = Rate50;
                        
                //    }
                //}
                //END VIP

                return new
                {
                    ResponseCode = 1,
                    list = list,
                    fast = Fast,
                   // PromotionRate = active == "1" && total > 0 ? 0.25 : 0,
                    PromotionRate=0,

                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }

        [ActionName("History")]
        [HttpGet]
        public dynamic History()
        {

            long accountId = AccountSession.AccountID;

            var displayName = AccountSession.AccountName;
            if (accountId <= 0 || String.IsNullOrEmpty(displayName))
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            var tkServiceID = AccountSession.ServiceID;
            if (tkServiceID != ServiceID)
            {
                return new
                {
                    ResponseCode = Constants.NOT_IN_SERVICE,
                    Message = ErrorMsg.NOTINSERVICE
                };
            }

            int TotalRecord = 0;

            var list = CardDAO.Instance.SP_UserCardRecharge_List_New(0, accountId, 0, 0, 0, null, null, null, null, null, ServiceID, 1, 200, out TotalRecord);

            return new
            {
                ResponseCode = 1,
                List = list
            };


        }

        private void ValidateCardNumber(string CardNumber, string CardCode, out bool isValid, out string Msg)
        {

            if ((String.IsNullOrEmpty(CardNumber)))
            {
                isValid = false;
                Msg = ErrorMsg.CardNumberRequired;
                return;


            }
            if (ValidateInput.IsContainSpace(CardNumber))
            {
                isValid = false;
                Msg = ErrorMsg.CardNumberNotContainSpace;
                return;
            }
            if (CardCode.Contains(CardCodeVTT) || CardCode.Contains(CardCodeVNP) || CardCode.Contains(CardCodeVMS))
            {
                if (!ValidateInput.ValidateStringNumber(CardNumber))
                {
                    isValid = false;
                    Msg = ErrorMsg.CardNumberContainNumber;
                    return;

                }
                if (CardNumber.Length < 11 || CardNumber.Length >= 16)
                {
                    isValid = false;
                    Msg = ErrorMsg.CardNumberLenghInValid;
                    return;
                }
            }
            if (CardCode.Contains(CardCodeZING) || CardCode.Contains(CardCodeVCOIN))
            {
                if (!ValidateInput.ValidateStringAnpha(CardNumber))
                {
                    isValid = false;
                    Msg = ErrorMsg.CardnumberInValid;
                    return;

                }
                if (CardNumber.Length < 5 || CardNumber.Length >= 16)
                {
                    isValid = false;
                    Msg = ErrorMsg.CardNumberLenghInValid;
                    return;
                }

            }


            isValid = true;
            Msg = string.Empty;
            return;
        }

        private void ValidateSerial(string SerialNumber, string CardCode, out bool isValid, out string Msg)
        {

            if ((String.IsNullOrEmpty(SerialNumber)))
            {
                isValid = false;
                Msg = ErrorMsg.SerialNumberRequired;
                return;

            }
            if (ValidateInput.IsContainSpace(SerialNumber))
            {
                isValid = false;
                Msg = ErrorMsg.SerialNumberNotContainSpace;
                return;

            }
            if (CardCode.Contains(CardCodeVTT) || CardCode.Contains(CardCodeVNP) || CardCode.Contains(CardCodeVMS))
            {
                if (!ValidateInput.ValidateStringNumber(SerialNumber))
                {
                    isValid = false;
                    Msg = ErrorMsg.SerialNumberContainNumber;
                    return;

                }
                if (SerialNumber.Length < 11 || SerialNumber.Length >= 16)
                {
                    isValid = false;
                    Msg = ErrorMsg.CardSerialLenghInValid;
                    return;


                }
            }
            if (CardCode.Contains(CardCodeZING) || CardCode.Contains(CardCodeVCOIN))
            {
                if (!ValidateInput.ValidateStringAnpha(SerialNumber))
                {
                    isValid = false;
                    Msg = ErrorMsg.SerialCardInvalid;
                    return;

                }
                if (SerialNumber.Length < 5 || SerialNumber.Length >= 16)
                {
                    isValid = false;
                    Msg = ErrorMsg.CardSerialLenghInValid;
                    return;
                }

            }


            isValid = true;
            Msg = string.Empty;
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("Charge")]
        [HttpPost]
        public dynamic Charge([FromBody] dynamic input)
        {
            try
            {
                string APPROVE = ConfigurationManager.AppSettings["Charge_APPROVED"].ToString();
                if (APPROVE != "1")
                {
                    return AnphaHelper.Close();
                }

                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                NLogManager.LogMessage(String.Format("Card:tkServiceID {0}|ServiceID{1}", tkServiceID, ServiceID));
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }

                string SerialNumber = input.SerialNumber ?? string.Empty;//lấy ra SerialNumber
                string CardNumber = input.CardNumber ?? string.Empty;//lấy ra CardNumber
                string CardCode = input.CardCode ?? string.Empty;//lấy ra CardCode
                string CardType = input.CardType ?? string.Empty;//lấy ra CardType
                string privateKey = input.PrivateKey;//lấy ra privte key
                string captcha = input.Captcha;//lấy ra capcha
                NLogManager.LogMessage(String.Format("Charge-CardCode:{0}",CardCode));
                //CardCode required
                if ((String.IsNullOrEmpty(CardCode)))
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardCodeRequired
                    };
                }
                bool isValid = false;
                string msg = string.Empty;
                ValidateCardNumber(CardNumber, CardCode, out isValid, out msg);
                if (!isValid)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = msg
                    };
                }
                isValid = false;
                msg = string.Empty;

                ValidateSerial(SerialNumber, CardCode, out isValid, out msg);
                if (!isValid)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = msg
                    };
                }

                //CardNumber required
                //if ((String.IsNullOrEmpty(CardNumber)))
                //{

                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.CardNumberRequired
                //    };
                //}


                //if (ValidateInput.IsContainSpace(CardNumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.CardNumberNotContainSpace
                //    };
                //}
                //if (!ValidateInput.ValidateStringNumber(CardNumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.CardNumberContainNumber
                //    };
                //}
                //if (CardNumber.Length < 11 || CardNumber.Length >= 16)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.CardNumberLenghInValid
                //    };

                //}


                //SerialNumber required
                //if ((String.IsNullOrEmpty(SerialNumber)))
                //{

                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.SerialNumberRequired
                //    };
                //}
                //if (ValidateInput.IsContainSpace(SerialNumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.SerialNumberNotContainSpace
                //    };
                //}

                //if (!ValidateInput.ValidateStringNumber(SerialNumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.SerialNumberContainNumber
                //    };
                //}
                //if (SerialNumber.Length < 11 || SerialNumber.Length >= 16)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.CardSerialLenghInValid
                //    };

                //}

                CardNumber = CardNumber.Trim();
                SerialNumber = SerialNumber.Trim();
                var card = CardDAO.Instance.GetCardList(CardCode, ServiceID).FirstOrDefault();
                if (card == null)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardLock
                    };
                }
                if (!card.Status)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardLock
                    };
                }

                //VALIDATE CHO TỪNG LOẠI THẺ
                //vina phone
                if (card.OperatorCode.Contains("VNP"))
                {
                    string VNP_CardNumber_Length = ConfigurationManager.AppSettings["VNP_CardNumber_Length"].ToString();
                    var arrVNP = VNP_CardNumber_Length.Split(',');
                    if (!arrVNP.Contains(CardNumber.Length.ToString()))
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.VinaphoneCardNumberLength
                        };
                    }
                    //if (SerialNumber.Length != 12&& SerialNumber.Length != 14)
                    //{
                    //    return new
                    //    {
                    //        ResponseCode = -1005,
                    //        Message = ErrorMsg.VinaphoneSeriNumberLength
                    //    };
                    //}

                }
                if (card.OperatorCode.Contains("VTT"))
                {
                    string VTT_CardNumber_Length = ConfigurationManager.AppSettings["VTT_CardNumber_Length"].ToString();
                    var arrVTT = VTT_CardNumber_Length.Split(',');


                    if (!arrVTT.Contains(CardNumber.Length.ToString()))
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.ViettelCardNumberLength
                        };
                    }
                    //if (SerialNumber.Length != 11 && SerialNumber.Length != 14)
                    //{
                    //    return new
                    //    {
                    //        ResponseCode = -1005,
                    //        Message = ErrorMsg.ViettelSeriNumberLength
                    //    };
                    //}

                }
                if (card.OperatorCode.Contains("VMS"))
                {
                    string VMS_CardNumber_Length = ConfigurationManager.AppSettings["VMS_CardNumber_Length"].ToString();
                    var arrVMS = VMS_CardNumber_Length.Split(',');
                    if (!arrVMS.Contains(CardNumber.Length.ToString()))
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.MobilePhoneCardNumberLength
                        };
                    }
                    //if (SerialNumber.Length != 15)
                    //{
                    //    return new
                    //    {
                    //        ResponseCode = -1005,
                    //        Message = ErrorMsg.MobilePhoneSeriNumberLength
                    //    };
                    //}

                }
                if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                {

                    return new
                    {
                        ResponseCode = -4,
                        Message = ErrorMsg.CapchaRequired
                    };
                }

                if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.InValidCaptCha
                    };
                }

                //validate capcha


                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };

                }

                //card Code invalid


                var Telecom = CardDAO.Instance.GetTeleCom(card.OperatorCode, ServiceID).FirstOrDefault();
                if (Telecom == null)
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardLock
                    };

                }
                //int cntResponse;
                //CardDAO.Instance.UserCardSerialCount(accountId, SerialNumber, Telecom.ID, ServiceID, out cntResponse);
                //if (cntResponse != 1)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.CardOverThree
                //    };
                //}

                //lấy ra mớ radamom và kiểm tra 
                long lngValue;
                try
                {
                    var listP = new List<string>();
                    var listPartners = CardPartnersDAO.Instance.GetList(ServiceID);
                    if (listPartners == null || !listPartners.Any())
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.CardLock
                        };
                    }

                    if (Telecom.OperatorCode.Contains("VTT"))
                    {
                        listP = listPartners.Where(c => c.VTT != "0").Select(c => c.VTT).ToList();
                    }
                    else if (Telecom.OperatorCode.Contains("VNP"))
                    {
                        listP = listPartners.Where(c => c.VNP != "0").Select(c => c.VNP).ToList();
                    }
                    else if (Telecom.OperatorCode.Contains("VMS"))
                    {
                        listP = listPartners.Where(c => c.VMS != "0").Select(c => c.VMS).ToList();
                    }
                    else if (Telecom.OperatorCode.Contains("ZING"))
                    {
                        listP = listPartners.Where(c => c.ZING != "0" && !String.IsNullOrEmpty(c.VCOIN)).Select(c => c.ZING).ToList();
                    }
                    else if (Telecom.OperatorCode.Contains("VCOIN"))
                    {
                        listP = listPartners.Where(c => c.VCOIN != "0" && !String.IsNullOrEmpty(c.VCOIN)).Select(c => c.VCOIN).ToList();
                    }
                    if (listP == null || !listP.Any())
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.CardLock
                        };
                    }


                    var strRandom = String.Join(",", listP);
                    var listAcitve = strRandom.Split(',');
                    var ListIntActive = listAcitve.Select(long.Parse).ToList().Where(c => c > 0).ToList();
                    if (ListIntActive == null || !ListIntActive.Any())
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.CardLock
                        };
                    }

                    Random rndActives = new Random();
                    var intRandom = rndActives.Next(0, ListIntActive.Count);
                    lngValue = ListIntActive[intRandom];
                    //kiểm tra xem có phải thẻ đặc biệt hay không (nếu là thẻ đặc biệt thì phải gán giá trị vào )
                    var PartnerId = card.PartnerId;
                    if (PartnerId.HasValue && PartnerId.Value > 0)
                    {
                        if (!ListIntActive.Any(c => c == PartnerId))
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }
                        lngValue = PartnerId.Value;
                    }

                }
                catch
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardLock
                    };
                }



                int Response = 0;
                long RequestID = 0;
                var Rate = Telecom.Rate;
                long VipCardID = 0;
                int VipResponse = 0;

                //VIP Tính toán lại ReceiveMoney ở đây
                var active = ConfigurationManager.AppSettings["CARD_VIP_ACTIVE"] == null ? "" : ConfigurationManager.AppSettings["CARD_VIP_ACTIVE"].ToString();

                if (active == "1")
                {
                    var kmobj = VIPDAO.Instance.VIPCardBonusCheck(accountId);
                    if (kmobj != null)
                    {
                        NLogManager.LogMessage(String.Format("VIPCARD KMOJ:{0}", "NOT NULL"));
                        //tạm thời tích lại  là 1 lần khuyến mại 

                        VIPDAO.Instance.VIPCardBonusTick(accountId, out VipCardID, out VipResponse);

                        if (VipResponse == 1 && VipCardID > 0)
                        {

                            if (kmobj.CardRate > 0)
                                Rate = kmobj.CardRate;
                        }

                    }
                }
                // sửa mệnh giá 50k
                //if(card.CardValue<= MAX50)
                //{
                //    Rate = Rate50;
                //}

                //End VIP

                var ReceiveMoney = Convert.ToInt64(card.CardValue * Rate);

                CardDAO.Instance.UserCardRechargeCreate(Telecom.ID, card.ID, card.CardValue, 0, Rate, accountId, ReceiveMoney, accountId, CardNumber, SerialNumber, null, null, null, null, ServiceID, out Response, out RequestID);
                if (Response != 1 || RequestID <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.InProccessException
                    };
                }
                // VIP cập nhật lại trạng thái thẻ đã được trừ
                if (active == "1")
                {
                    if (VipResponse == 1 && VipCardID > 0)
                    {
                        VIPDAO.Instance.VIPCardBonusUpdate(1, accountId, VipCardID, RequestID, out VipResponse);
                        NLogManager.LogMessage(String.Format("VIPCARD2 VipCardID:{0}|VipResponse {1}", VipCardID, VipResponse));
                    }
                }

                //END VIP

                int outResponse = 0;

                if (Telecom.OperatorCode.Contains("VTT"))//vitell
                {

                    long radom = lngValue;
                    if (ServiceID == 1)//THANH TOÁN DÀNH CHO CỔNG SỐ 1
                    {
                        var resShopMUaThe = BtvnAPI.SendRequest(RequestID.ToString(), CardNumber,SerialNumber, card.CardValue, "vt", accountId);
                        //int status = resShopMUaThe.status == 1 ? 0 : -2;
                        //CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.status.ToString(), resShopMUaThe.data!=null?resShopMUaThe.data.transid:"", null, null, null, resShopMUaThe.Signature, accountId, 1, out outResponse);
                        CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, 0, resShopMUaThe.msg.ToString(), resShopMUaThe.msg.ToString(), null, null, null, null, accountId, 7, RequestID.ToString(), out outResponse);
                        if (resShopMUaThe.errorCode != 0)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardCharingException
                            };
                        }
                        /*
                        if (radom == 1)
                        {
                            //đối tác heo siêu nạc
                            var res = CardApi.SendRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, card.OperatorCode);

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, PARTNER_ONE, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 2)
                        {

                            var res = MobileSMSApi.Send(RequestID.ToString(), "Viettel", CardNumber, SerialNumber, card.CardValue);

                            int status = res.errorid == 0 ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.errorid.ToString(), res.errordes, null, null, null, string.Empty, accountId, PARTNER_TOW, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.errorid == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }



                        }
                        else if (radom == 3)
                        {
                           
                            var res = XBommApi.SendRequest("VTT", card.CardValue, SerialNumber, CardNumber);
                            if (res == null)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            int status = res.code == 1 ? 0 : -2;
                          
                            CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, PARTNER_THREE, res.data.id, out outResponse);
                            if (res.code == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }

                        }
                        else if (radom == 5)
                        {
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.Send(RequestID.ToString(), "viettel", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_FIVE, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_FIVE, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_FIVE);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            //default là thằng smile (p4)
                            var res = CardApi.SendRequestFunny(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, card.OperatorCode);

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;


                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, PARTNER_FOUR, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        */

                    }
                    else if (ServiceID == 2)//THANH TOÁN DÀNH CHO CỔNG SỐ 2
                    {
                        /*
                        if (radom == 6)
                        {
                            //đối tác heo siêu nạc
                            var res = CardApi.SendGate2FunnyRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, "VTT");

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, 6, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 7)
                        {
                            var res = FconnClubApi.SendGate2Request(RequestID.ToString(), CardNumber, SerialNumber, card.CardValue, "Viettel");

                            int status = res.code == 200 ? 0 : -2;

                            CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, 7, res.card_id.ToString(), out outResponse);
                            if (res.code == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 11)
                        {
                            int PARTNER_ELEVEN = 11;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate2(RequestID.ToString(), "viettel", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_ELEVEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_ELEVEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_ELEVEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }
                        */
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                    }
                    else if (ServiceID == 3)//THANH TOÁN DÀNH CHO CỔNG SỐ 3
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                        /*
                        if (radom == 8)//GATE 3_P8
                        {
                            //đối tác heo siêu nạc
                            var res = CardApi.SendGate3FunnyRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, "VTT");

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, 8, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 9)//GATE 3_P9
                        {
                            var res = MobileSMSApi.SendGate3(RequestID.ToString(), "Viettel", CardNumber, SerialNumber, card.CardValue);

                            int status = res.errorid == 0 ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.errorid.ToString(), res.errordes, null, null, null, string.Empty, accountId, 9, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.errorid == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 10) //GATE3 P5 SHOP MUA THE KHAN
                        {
                            int PARTNER_TEN = 10;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate3(RequestID.ToString(), "viettel", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_TEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_TEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_TEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }
                        */
                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                    }



                }
                else if (Telecom.OperatorCode.Contains("VNP"))//via phone 
                {

                    long radom = lngValue; // creates a number between 1 and 3
                    if (ServiceID == 1)
                    {
                        var resShopMUaThe = BtvnAPI.SendRequest(RequestID.ToString(), CardNumber, SerialNumber, card.CardValue, "vina", accountId);
                        //int status = resShopMUaThe.status == 1 ? 0 : -2;
                        //CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.status.ToString(), resShopMUaThe.data!=null?resShopMUaThe.data.transid:"", null, null, null, resShopMUaThe.Signature, accountId, 1, out outResponse);
                        CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, 0, resShopMUaThe.msg.ToString(), resShopMUaThe.msg.ToString(), null, null, null, null, accountId, 7, RequestID.ToString(), out outResponse);
                        if (resShopMUaThe.errorCode != 0)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardCharingException
                            };
                        }
                        /*
                        if (radom == 1)
                        {

                            var res = CardApi.SendRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, card.OperatorCode);

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, PARTNER_ONE, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }


                        }
                        else if (radom == 2)
                        {
                            var res = MobileSMSApi.Send(RequestID.ToString(), "Vinaphone", CardNumber, SerialNumber, card.CardValue);

                            int status = res.errorid == 0 ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.errorid.ToString(), res.errordes, null, null, null, string.Empty, accountId, PARTNER_TOW, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.errorid == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 3)
                        {
                            //goi sang doi tac thu 3 Fncoon
                            //bên này chỉ chấp nhận 3 đối số Viettel, Mobiphone, Vinaphone
                            //var res = FconnClubApi.SendRequest(RequestID.ToString(), CardNumber, SerialNumber, card.CardValue, "Vinaphone");

                            //int status = res.code == 200 ? 0 : -2;

                            //CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, PARTNER_THREE, res.card_id.ToString(), out outResponse);
                            //if (status == -2)
                            //{
                            //    PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            //}
                            //if (res.code == -99)
                            //{
                            //    return new
                            //    {
                            //        ResponseCode = -1005,
                            //        Message = ErrorMsg.CardCharingException
                            //    };
                            //}
                            var res = XBommApi.SendRequest("VINA", card.CardValue, SerialNumber, CardNumber);
                            if (res == null)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            int status = res.code == 1 ? 0 : -2;
                            CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, PARTNER_THREE, res.data.id, out outResponse);
                            if (res.code == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }

                        }
                        else if (radom == 5)
                        {
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.Send(RequestID.ToString(), "vnp", CardNumber, SerialNumber, card.CardValue, accountId.ToString());
                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_FIVE, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền

                            //
                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_FIVE, resShopMUaThe.Description, valueCharge);
                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_FIVE);
                            }
                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };
                        }
                        else
                        {

                            //P4-Funny
                            var res = CardApi.SendRequestFunny(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, card.OperatorCode);

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, PARTNER_FOUR, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }


                        }
                        */
                    }
                    else if (ServiceID == 2)
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                        /*
                        if (radom == 6)
                        {
                            //đối tác heo siêu nạc
                            var res = CardApi.SendGate2FunnyRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, "VNP");

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, 6, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 7)
                        {
                            var res = FconnClubApi.SendGate2Request(RequestID.ToString(), CardNumber, SerialNumber, card.CardValue, "Vinaphone");

                            int status = res.code == 200 ? 0 : -2;

                            CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, 7, res.card_id.ToString(), out outResponse);
                            if (res.code == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 11)
                        {
                            int PARTNER_ELEVEN = 11;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate2(RequestID.ToString(), "vnp", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_ELEVEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_ELEVEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_ELEVEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }
                        */
                    }
                    else if (ServiceID == 3)//THANH TOÁN DÀNH CHO CỔNG SỐ 3
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                        /*
                        if (radom == 8)//GATE 3_P8
                        {
                            //đối tác heo siêu nạc
                            var res = CardApi.SendGate3FunnyRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, "VNP");

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, 8, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 9)//GATE 3_P9
                        {
                            var res = MobileSMSApi.SendGate3(RequestID.ToString(), "Vinaphone", CardNumber, SerialNumber, card.CardValue);
                            int status = res.errorid == 0 ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.errorid.ToString(), res.errordes, null, null, null, string.Empty, accountId, 9, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.errorid == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 10) //GATE3 P5 SHOP MUA THE KHAN
                        {
                            int PARTNER_TEN = 10;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate3(RequestID.ToString(), "vnp", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_TEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_TEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_TEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }
                        */
                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                    }

                }
                else if (Telecom.OperatorCode.Contains("VMS"))//mobile phone
                {

                    long radom = lngValue;
                    if (ServiceID == 1)
                    {
                        var resShopMUaThe = BtvnAPI.SendRequest(RequestID.ToString(), CardNumber, SerialNumber, card.CardValue, "mobi", accountId);
                        //int status = resShopMUaThe.status == 1 ? 0 : -2;
                        //CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.status.ToString(), resShopMUaThe.data!=null?resShopMUaThe.data.transid:"", null, null, null, resShopMUaThe.Signature, accountId, 1, out outResponse);
                        CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, 0, resShopMUaThe.msg.ToString(), resShopMUaThe.msg.ToString(), null, null, null, null, accountId, 7, RequestID.ToString(), out outResponse);
                        if (resShopMUaThe.errorCode != 0)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardCharingException
                            };
                        }
                        /*
                        if (radom == 1)
                        {

                            //heo sieu nac
                            var res = CardApi.SendRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, card.OperatorCode);

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, PARTNER_ONE, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }

                        }
                        else if (radom == 2)
                        {
                            var res = MobileSMSApi.Send(RequestID.ToString(), "Mobifone", CardNumber, SerialNumber, card.CardValue);

                            int status = res.errorid == 0 ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.errorid.ToString(), res.errordes, null, null, null, string.Empty, accountId, PARTNER_TOW, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.errorid == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }



                        }
                        else if (radom == 3)
                        {
                            //goi sang doi tac thu 3 Fncoon
                            //bên này chỉ chấp nhận 3 đối số Viettel, Mobiphone, Vinaphone
                            //var res = FconnClubApi.SendRequest(RequestID.ToString(), CardNumber, SerialNumber, card.CardValue, "Mobiphone");

                            //int status = res.code == 200 ? 0 : -2;

                            //CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, PARTNER_THREE, res.card_id.ToString(), out outResponse);
                            //if (status == -2)
                            //{
                            //    PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            //}
                            //if (res.code == -99)
                            //{
                            //    return new
                            //    {
                            //        ResponseCode = -1005,
                            //        Message = ErrorMsg.CardCharingException
                            //    };
                            //}

                            var res = XBommApi.SendRequest("VMS", card.CardValue, SerialNumber, CardNumber);
                            if (res == null)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            int status = res.code == 1 ? 0 : -2;
                            CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, PARTNER_THREE, res.data.id, out outResponse);
                            if (res.code == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }


                        }
                        else if (radom == 5)
                        {
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.Send(RequestID.ToString(), "vms", CardNumber, SerialNumber, card.CardValue, accountId.ToString());
                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_FIVE, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền

                            //
                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_FIVE, resShopMUaThe.Description, valueCharge);
                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_FIVE);
                            }
                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };
                        }
                        else
                        {

                            //P-4 Funny
                            var res = CardApi.SendRequestFunny(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, card.OperatorCode);

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, PARTNER_FOUR, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        */
                    }
                    else if (ServiceID == 2)//NẠP THẺ CỔNG 2
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.CardLock
                        };
                        /*
                        if (radom == 6)
                        {
                            //đối tác heo siêu nạc
                            var res = CardApi.SendGate2FunnyRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, "VMS");

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, 6, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Nạp thẻ thất bại", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 7)
                        {
                            var res = FconnClubApi.SendGate2Request(RequestID.ToString(), CardNumber, SerialNumber, card.CardValue, "Mobiphone");

                            int status = res.code == 200 ? 0 : -2;

                            CardRefDAO.Instance.UserCardRechargeRefUpdate(RequestID, accountId, status, res.code.ToString(), res.message, null, null, null, null, accountId, 7, res.card_id.ToString(), out outResponse);
                            if (res.code == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 11)
                        {
                            int PARTNER_ELEVEN = 11;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate2(RequestID.ToString(), "vms", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_ELEVEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_ELEVEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_ELEVEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }
                        */
                    }
                    else if (ServiceID == 3)//THANH TOÁN DÀNH CHO CỔNG SỐ 3
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.CardLock
                        };
                        if (radom == 8)//GATE 3_P8
                        {
                            //đối tác heo siêu nạc
                            var res = CardApi.SendGate3FunnyRequest(RequestID.ToString(), accountId.ToString(), CardNumber, SerialNumber, card.CardValue, "VMS");

                            int status = !res.IsError && res.ErrorCode == "00" ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.ErrorCode, res.Message, null, null, null, res.Signature, accountId, 8, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Card loading failed", ServiceID, 0);
                            }
                            if (res.ErrorCode == "-99")
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 9)//GATE 3_P9
                        {
                            var res = MobileSMSApi.SendGate3(RequestID.ToString(), "Mobifone", CardNumber, SerialNumber, card.CardValue);
                            int status = res.errorid == 0 ? 0 : -2;

                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, res.errorid.ToString(), res.errordes, null, null, null, string.Empty, accountId, 9, out outResponse);
                            if (status == -2)
                            {
                                PortalHandler.Instance.ReturnTopupCard(accountId, 0, "Card loading failed", ServiceID, 0);
                            }
                            if (res.errorid == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                        }
                        else if (radom == 10) //GATE3 P5 SHOP MUA THE KHAN
                        {
                            int PARTNER_TEN = 10;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate3(RequestID.ToString(), "vms", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_TEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_TEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_TEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }

                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                    }



                }
                else if (Telecom.OperatorCode.Contains("ZING"))//đói tác thẻ zing
                {

                    long radom = lngValue;
                    if (ServiceID == 1)//THANH TOÁN DÀNH CHO CỔNG SỐ 1
                    {


                        if (radom == 5)
                        {
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.Send(RequestID.ToString(), "zing", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_FIVE, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_FIVE, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_FIVE);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }


                    }
                    else if (ServiceID == 2)//THANH TOÁN DÀNH CHO CỔNG SỐ 2
                    {
                        if (radom == 11)
                        {
                            int PARTNER_ELEVEN = 11;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate2(RequestID.ToString(), "zing", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_ELEVEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_ELEVEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_ELEVEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }



                    }
                    else if (ServiceID == 3)//THANH TOÁN DÀNH CHO CỔNG SỐ 3
                    {


                        if (radom == 10) //GATE3 P5 SHOP MUA THE KHAN
                        {
                            int PARTNER_TEN = 10;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate3(RequestID.ToString(), "zing", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_TEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_TEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_TEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }

                    }

                    else
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                    }
                }
                else if (Telecom.OperatorCode.Contains("VCOIN"))//Thanh toán cho thẻ vcoin
                {

                    long radom = lngValue;
                    if (ServiceID == 1)//THANH TOÁN DÀNH CHO CỔNG SỐ 1
                    {


                        if (radom == 5)
                        {
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.Send(RequestID.ToString(), "vcoin", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_FIVE, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_FIVE, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_FIVE);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }


                    }
                    else if (ServiceID == 2)//THANH TOÁN DÀNH CHO CỔNG SỐ 2
                    {

                        if (radom == 11)
                        {
                            int PARTNER_ELEVEN = 11;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate2(RequestID.ToString(), "vcoin", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_ELEVEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_ELEVEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_ELEVEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }


                    }
                    else if (ServiceID == 3)//THANH TOÁN DÀNH CHO CỔNG SỐ 3
                    {


                        if (radom == 10) //GATE3 P5 SHOP MUA THE KHAN
                        {
                            int PARTNER_TEN = 10;
                            //gọi sang đối tăc thức 5 shop mua the (khan )
                            var resShopMUaThe = ShopMuaTheApi.SendGate3(RequestID.ToString(), "vcoin", CardNumber, SerialNumber, card.CardValue, accountId.ToString());

                            int status = resShopMUaThe.ResponseCode == 1 || resShopMUaThe.ResponseCode == -2 || resShopMUaThe.ResponseCode == -372 ? 0 : -2;
                            CardDAO.Instance.UserCardRechargeUpdate(RequestID, accountId, status, resShopMUaThe.ResponseCode.ToString(), resShopMUaThe.Description + "|" + resShopMUaThe.ResponseContent, null, null, null, resShopMUaThe.Signature, accountId, PARTNER_TEN, out outResponse);
                            if (resShopMUaThe.ResponseCode == -99)
                            {
                                return new
                                {
                                    ResponseCode = -1001,
                                    Message = ErrorMsg.CardCharingException
                                };
                            }
                            // trường hợp này là thẻ đugns cộng tiền


                            var valueCharge = ConvertUtil.ToInt(resShopMUaThe.ResponseContent);
                            //  nạp thẻ thành công
                            if (card != null && resShopMUaThe.ResponseCode == 1 && card.CardValue == valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingMoney(RequestID, user.AccountID, status, PARTNER_TEN, resShopMUaThe.Description, valueCharge);

                            }
                            //nếu nạp thẻ sai mệnh giá
                            if (card != null && resShopMUaThe.ResponseCode == -372 && card.CardValue != valueCharge && status == 0 && valueCharge > 0)
                            {
                                return ChargingInvalidMoney(RequestID, valueCharge, PARTNER_TEN);
                            }

                            return new
                            {
                                ResponseCode = 1,
                                Message = ErrorMsg.CardChargingSuccess
                            };

                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CardLock
                            };
                        }

                    }

                    else
                    {
                        return new
                        {
                            ResponseCode = Constants.NOT_IN_SERVICE,
                            Message = ErrorMsg.NOTINSERVICE
                        };
                    }
                }



                else
                {
                    return new
                    {
                        ResponseCode = -1,
                        Message = ErrorMsg.CardTelecomInActive
                    };
                }



                return new
                {
                    ResponseCode = 1,
                    Message = ErrorMsg.CardChargingSuccess
                };



            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }
        /// <summary>
        /// chỉ dùng cho p5
        /// </summary>
        /// <param name="lngRequestID"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private dynamic ChargingInvalidMoney(long lngRequestID, int value, int partnerID)
        {
            var card = CardDAO.Instance.UserCardRechargeGetByID(lngRequestID);
            var minValue = card.CardValue >= value ? value : card.CardValue;
            var receiveValue = ConvertUtil.ToLong(minValue * card.TeleRate);
            int outResponse = 0;
            //cập nhật lại trạng thái 4
            CardRefDAO.Instance.UpdateCardChardRefStatus2(lngRequestID, card.UserID, "0", null, STATUS_SMG_NOT_REFUND, partnerID, value, minValue, receiveValue, out outResponse);
            if (outResponse == 1)
            {
                try
                {
                    var cardIndex = GetCardCodeIndex(card.OperatorCode);
                    SendDNA(lngRequestID, card.UserID, cardIndex, minValue, receiveValue);
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }

                return new
                {
                    ResponseCode = -1005,
                    Message = STATUS_SMG_NOT_REFUND_REASON
                };

                ///cộng tiền khi  sai mệnh giá khi

                //long wallet = 0;
                //long tranID = 0;
                //string msgTC = String.Format("Nạp thẻ (SMG) phiên {0} ", lngRequestID);
                //var res = UserTranferDAO.Instance.UserRechargeCard(card.UserID, receiveValue, msgTC, ServiceID, out tranID, out wallet);

                //if (res != 1)//nếu không thể cộng tiền cho user
                //{
                //    //SendChargingHub(card.UserID, 0, "Nạp thẻ thất bại", 0, ServiceID);
                //    var msg = String.Format("{0}|{1}", "00", String.Format("H5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
                //    outResponse = 0;
                //    CardDAO.Instance.UpdateCardChardStatus(lngRequestID, card.UserID, "0", null, 3, PARTNER_FIVE, out outResponse);
                //    if (outResponse == 1)
                //    {
                //        //  msg = String.Format("{0}|{1}", "00", String.Format("H1{0}", ErrorMsg.CardUpdateStatusOK_3));// trạng thái thẻ khác 00 và khác 1   cập nhật ngay về trang
                //        msg = ErrorMsg.CardUpdateStatusOK_3;
                //        return new
                //        {
                //            ResponseCode = -1005,
                //            Message = msg
                //        };
                //        // return StringResult(msg);
                //    }
                //    else if (outResponse == -202)
                //    {

                //        // msg = String.Format("{0}|{1}", "00", String.Format("H2{0}", ErrorMsg.UserNotValid));// không tìm thấy user khi cập nhật trạng thái 1
                //        msg = ErrorMsg.UserNotValid;
                //        return new
                //        {
                //            ResponseCode = -1005,
                //            Message = msg
                //        };
                //        // return StringResult(msg);
                //    }
                //    else if (outResponse == -508)
                //    {

                //        // msg = String.Format("{0}|{1}", "00", String.Format("H3{0}", ErrorMsg.UserCardNotFind));// không tìm thấy user khi cập nhật trạng thái 1
                //        msg = ErrorMsg.UserCardNotFind;
                //        return new
                //        {
                //            ResponseCode = -1005,
                //            Message = msg
                //        };
                //        // return StringResult(msg);
                //    }
                //    else
                //    {
                //        //  msg = String.Format("{0}|{1}", "00", String.Format("H4{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái 1
                //        msg = ErrorMsg.CardBusy;
                //        return new
                //        {
                //            ResponseCode = -1005,
                //            Message = msg
                //        };
                //        // return StringResult(msg);

                //    }

                //}
                //else
                //{
                //    //đẫ cộng tiền cho khách và 
                //    // var msg = String.Format("{0}|{1}", "00", String.Format("H6{0}", ErrorMsg.CardUpdateStatus4));// không tìm thấy user khi cập nhật trạng thái 1
                //    var msg = ErrorMsg.CardSuccessOnCharging;                                                                                            //  return StringResult(msg);
                //    return new
                //    {
                //        ResponseCode = 1,
                //        Message = msg
                //    };
                //}

            }
            else
            {

                //var msg = String.Format("{0}|{1}", "00", String.Format("H7{0}", ErrorMsg.CardBusy));// không tìm thấy user khi cập nhật trạng thái -3
                var msg = ErrorMsg.CardBusy;
                return new
                {
                    ResponseCode = -1005,
                    Message = msg
                };
                //return StringResult(msg);
            }
        }
        /// <summary>
        /// chỉ dugnf cho p5
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="AccountID"></param>
        /// <param name="status"></param>
        /// <param name="partnerID"></param>
        /// <returns></returns>
        private dynamic ChargingMoney(long RequestID, long AccountID, int status, int partnerID, string msg, int value)
        {
            int outResponse = 0;
            //  int TotalRecord;
            // var cardRequest = CardDAO.Instance.UserCardRechargeList(RequestID, 0, 0, 0, 0, null, null, null, null, null, ServiceID, 1, int.MaxValue, out TotalRecord).FirstOrDefault();
            var cardRequest = CardDAO.Instance.UserCardRechargeGetByID(RequestID);
            if (cardRequest == null)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardErrorContactCSKH
                };

            }
            //cộng tiền cho khách hàng
            long wallet = 0;
            long tranID = 0;
            var resMoney = UserTranferDAO.Instance.UserRechargeCard(AccountID, cardRequest.ReceivedMoney.Value, "Nạp thẻ cào phiên" + RequestID, ServiceID, out tranID, out wallet);
            if (resMoney != 1)//nếu không thể cộng tiền cho user
            {
                // var msg = String.Format("{0}|{1}", "00", String.Format("C5{0}", ErrorMsg.CardChargingNotAddAmount));// không thể cộng tiền cho user
                outResponse = 0;
                CardDAO.Instance.UpdateCardChardStatus(RequestID, AccountID, status.ToString(), "success", 3, partnerID, out outResponse);
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardErrorContactCSKH
                };


            }
            else
            {

                outResponse = 0;
                //   CardDAO.Instance.UpdateCardChardStatus(RequestID, AccountID, status.ToString(), "success", 2, partnerID, out outResponse);//cập nhật cho user thành công
                CardRefDAO.Instance.UpdateCardChardRefStatus2(RequestID, AccountID, status.ToString(), msg, 2, partnerID, value, 0, 0, out outResponse);
                try
                {
                    var cardIndex = GetCardCodeIndex(cardRequest.OperatorCode);
                    SendDNA(cardRequest.RequestID, cardRequest.UserID, cardIndex, cardRequest.CardValue, cardRequest.ReceivedMoney.Value);

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        int intResposne;
                        TreasureDAO.Instance.CarrotCollectRechargeCard(cardRequest.UserID, cardRequest.CardValue, ServiceID, out intResposne);
                        double KMRate = cardRequest.Rate - cardRequest.TeleRate;
                        //Gửi thông tin khuyến mại value
                        if (KMRate <= 0)
                        {
                            KMRate = 0;

                        }
                        if (KMRate > 0)
                        {
                            var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                            var KMValue = ConvertUtil.ToLong(cardRequest.CardValue * KMRate);
                            dnaHelper.SendTransactionBounus(cardRequest.UserID, cardIndex, null, KMValue);
                        }
                       


                    });

                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
                if (outResponse != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardErrorContactCSKH
                    };

                }


            }

            //null tức là  return success  (hàm này chỉ có ý nghĩa bắn ra lỗi 
            return new
            {
                ResponseCode = 1,
                Message = ErrorMsg.CardSuccessOnCharging,
                Balance = wallet,
            };
        }

        ///// <summary>
        ///// string result set success
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //private HttpResponseMessage StringResult(string msg)
        //{
        //    return new HttpResponseMessage()
        //    {
        //        Content = new StringContent(msg, Encoding.UTF8, "text/plain")
        //    };
        //}


        [ActionName("SendChargeResult")]
        [HttpPost]
        public dynamic SendChargeResult([FromBody] dynamic input)
        {
            long accountId = input.AccountId;
            long balance = input.Balance;
            string msg = input.Msg;
            int sID = input.ServiceID;
            int Status = input.Status;
            try
            {

                NLogManager.LogMessage(string.Format("Call Out :accountId: {0} | balance: {1} | msg: {2} ", accountId, balance, msg));
                PortalHandler.Instance.ReturnTopupCard(accountId, balance, msg, sID, Status);


                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogMessage(string.Format("Exception Call Out :accountId: {0} | balance: {1} | msg: {2} ", accountId, balance, msg));
                NLogManager.PublishException(ex);
                return false;
            }

        }



        //private HttpResponseMessage JsonResult(int status, string msg)
        //{
        //    var resp = new HttpResponseMessage();
        //    var obj = new
        //    {
        //        status = status,
        //        msg = msg,
        //    };
        //    string jsonRes = JsonConvert.SerializeObject(obj);
        //    resp.Content = new StringContent(jsonRes, System.Text.Encoding.UTF8, "application/json");
        //    return resp;
        //}

    }
}
