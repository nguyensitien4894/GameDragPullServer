using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Lib;
using MsWebGame.Portal.Models.USDTBanks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;

namespace MsWebGame.Portal.Controllers.Transaction
{
    public class CardController : BaseApiController
    {
        private string CardCodeVTT = "VTT";
        private string CardCodeVNP = "VNP";
        private string CardCodeVMS = "MPB";

        [ActionName("Charge")]
        [HttpPost]
        public dynamic Charge([FromBody] dynamic input)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine("******************* TopupCard *****************");
            try
            {
                #region check token and ServiceID
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
                NLogManager.MomoLog(String.Format("Card:tkServiceID {0}|ServiceID{1}", tkServiceID, ServiceID));
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }

                #endregion

                #region get params
                log.AppendLine(JsonConvert.SerializeObject(input));
                string SerialNumber = input.SerialNumber ?? string.Empty;//lấy ra SerialNumber
                string CardNumber = input.CardNumber ?? string.Empty;//lấy ra CardNumber
                string CardCode = input.CardCode ?? string.Empty;//lấy ra CardCode
                string CardType = input.CardType ?? string.Empty;//lấy ra CardType
                string privateKey = input.PrivateKey;//lấy ra privte key
                string captcha = input.Captcha;//lấy ra capcha
                log.AppendLine(JsonConvert.SerializeObject(input));
                #endregion

                #region check captcha

                if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                {

                    return new
                    {
                        ResponseCode = -4,
                        Message = ErrorMsg.CapchaRequired
                    };
                }

                if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.InValidCaptCha
                    };
                }
                #endregion

                #region validate card

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

                var Telecom = CardDAO.Instance.GetTeleCom(card.OperatorCode, ServiceID).FirstOrDefault();
                if (Telecom == null)
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardLock
                    };

                }

                string[] validCodes = { "VTT", "VNP", "VMS" };
                if (!validCodes.Contains(Telecom.OperatorCode))
                {
                    log.AppendLine("Error: " + ErrorMsg.CardTelecomInActive);
                    return new
                    {
                        ResponseCode = -1,
                        Message = ErrorMsg.CardTelecomInActive
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

                }

                #endregion

                #region check status of user
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
                #endregion

                #region kiểm tra thẻ được nhập vào hệ thống có quá số lần cho phép không
                int cntResponse;
                CardDAO.Instance.UserCardSerialCount(accountId, SerialNumber, Telecom.ID, ServiceID, out cntResponse);
                if (cntResponse != 1)
                {
                    log.AppendLine("Error: -1005 " + ErrorMsg.CardOverThree);
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardOverThree
                    };
                }
                #endregion


                #region ghi nhận thông tin nạp thẻ của user
                
                int Response = 0;
                long RequestID = 0;
                var Rate = Telecom.Rate;
                var ReceiveMoney = Convert.ToInt64(card.CardValue * Rate);

                CardDAO.Instance.UserCardRechargeCreate(
                    Telecom.ID, card.ID, card.CardValue, 0, Rate, accountId, ReceiveMoney, accountId, CardNumber, SerialNumber,
                    null, null, null, null, ServiceID, out Response, out RequestID);
                if (Response != 1 || RequestID <= 0)
                {
                    log.AppendLine("Error: " + ErrorMsg.InProccessException);
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.InProccessException
                    };
                }
                log.AppendLine("RequestID: RequestID");

                #endregion

                #region call api to partner

                int outResponse = 0;
                TransferInfo transInfo = new TransferInfo();
                if (ServiceID == 1)//THANH TOÁN DÀNH CHO CỔNG SỐ 1
                {
                    
                    transInfo = QienGate.TopupCard(Telecom.OperatorCode, SerialNumber, CardNumber, card.CardValue, RequestID.ToString(), accountId);
                    int status = transInfo.status == 1 ? 0 : -2;

                    CardRefDAO.Instance.UserCardRechargeRefUpdate(
                        RequestID, accountId, status, transInfo.error_code.ToString(), transInfo.description,
                        null, null, null, null, accountId, 7, transInfo.data.partner_key, out outResponse);
                    return new
                    {
                        ResponseCode = transInfo.status,
                        Mesage = transInfo.description,
                        Data = transInfo.data
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                #endregion



            }
            catch (Exception ex)
            {
                log.AppendLine(ex.Message);
                //NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
            finally
            {
                log.AppendLine("***************** END *******************\r\n");
                NLogManager.MomoLog(log.ToString());
            }
        }


        [HttpPost]
        public dynamic CallBackForQienGate()
        {
            StringBuilder l = new StringBuilder();
            try
            {
                var checksum1 = HttpContext.Current.Request.Headers["Checksum"];
                var requestBody = WebClass.GetRequestBody();
                string checksum2 = QienGate.Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);

                var input = JsonConvert.DeserializeObject<QienCallbackParams>(requestBody);

                l.AppendLine("***********CallBackCardFromQienGate****************");
                l.AppendLine("requestBody: " + requestBody);
                l.AppendLine("checksum1: " + checksum1);
                l.AppendLine("checksum2: " + checksum2);
                if (checksum1 != checksum2)
                {
                    l.AppendLine("checksum không hợp lệ " );
                    return new { err_code = 0, err_msg = "OK" };
                }
               
                //get charge code
                var chargeCode = Database.DAO.TransactionDAO.GetChargeCode(input.ticket, input.IntergerChargeType(), input.ref_id);
                if (chargeCode is null || string.IsNullOrEmpty(chargeCode.Data))
                {
                    l.AppendLine("Not found charge code");
                    return new { err_code = 0, err_msg = "OK" };
                }
                if (chargeCode.Status != (int)Lib.Constant.CHARGE_CODE_STATUS.Processing)
                {
                    l.AppendLine("chargeCode.Status: is not processing" );
                    return new { err_code = 0, err_msg = "OK" };
                }

                //add log topup
                if (input.err_code == 0)
                {
                    if (input.amount <= 0)
                    {
                        l.AppendLine("Amount is invalid");
                        return new { err_code = 0, err_msg = "OK" };
                    }
                    chargeCode.ReceivedMoney = input.amount_real;
                    int rs = Database.DAO.TransactionDAO.AddTopupCard(chargeCode, requestBody);
                    if (rs == 1) //cộng tiền user thành công
                    {
                        var myAccount = AccountDAO.Instance.GetProfile(chargeCode.UserId, 1);
                        if (myAccount.TelegramID != null && myAccount.TelegramID > 0)
                        {
                            string msgTele2 = "💰💰💰 NẠP TIỀN QUA THẺ CÀO THÀNH CÔNG 💰💰💰" +
                                                            "\n- Số tiền đã chuyển 💵" + Lib.General.FormatMoneyVND(chargeCode.ReceivedMoney) +
                                                            "\n- AccountID 😀" + myAccount.AccountID +
                                                            "\n- AccountName: " + myAccount.AccountName +
                                                            "\n- 🕜 " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");

                            TeleNotify(myAccount.TelegramID.ToString(), msgTele2);
                        }

                    }
                }
                else
                {
                    //update status fail for charge code
                    TransactionDAO.UpdateChargeCodeStatus(chargeCode, requestBody);
                }

            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                l.AppendLine("" + ex);
                return new { err_code = 0, err_msg = "OK" };
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());

            }
            return new { err_code = 0, err_msg = "OK" };

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
           
            isValid = true;
            Msg = string.Empty;
            return;
        }


    }
}
