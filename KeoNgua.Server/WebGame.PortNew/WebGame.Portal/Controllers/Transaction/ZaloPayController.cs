using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Lib;
using MsWebGame.Portal.Models.USDTBanks;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;

namespace MsWebGame.Portal.Controllers.Transaction
{
    public class ZaloPayController : BaseApiController
    {
        /// <summary>
        /// Nạp zalo pay: Qien Gate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public dynamic RequestTopup([FromBody] BankRequestParams input)
        {

            TransferInfo rs2 = new TransferInfo();
            StringBuilder l = new StringBuilder();
            try
            {
                long accountID = AccountSession.AccountID;
                var accountName = AccountSession.AccountName;
                if (accountID <= 0 || String.IsNullOrEmpty(accountName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                string APPROVE = Lib.Constant.ZALO_APPROVED;
                if (APPROVE != "1")
                {
                    return new
                    {
                        ResponseCode = -1,
                        Description = "Hệ thống đang tạm ngưng để bảo trì!"
                    };
                }
                int amount = input.Amount;
                string captcha = input.Captcha;
                string captchaKey = input.PrivateKey;

                l.AppendLine("*************** Get zalopay Info **************");
                l.AppendLine("amount: " + amount);
                l.AppendLine("captcha: " + captcha);
                l.AppendLine("token: " + captchaKey);


                //if (CaptchaCache.Instance.VerifyCaptcha(captcha, token) < 0)
                int captchaVeriryStatus = CaptchaCache.Instance.VerifyCaptcha(captcha, captchaKey);
                if (captchaVeriryStatus < 0)
                {
                    l.AppendLine("Mã captcha không chính xác!");
                    return new
                    {
                        ResponseCode = 0,
                        Description = "Mã captcha không chính xác!"
                    };
                }

                var rs1 = TransactionDAO.GetRate("zalo");

                if (amount < rs1.MinTopup || amount > rs1.MaxTopup)
                {
                    l.AppendLine("Số tiền không hợp lệ!");
                    return new
                    {
                        ResponseCode = -3,
                        Description = "Số tiền nạp tối thiểu là " + Lib.General.FormatMoneyVND(rs1.MinTopup) + ", tối đa là " + Lib.General.FormatMoneyVND(rs1.MaxTopup)
                    };
                }

                rs2 = QienGate.TopupZalo(amount, accountID);

                return new
                {
                    ResponseCode = rs2.status,
                    Description = rs2.description,
                    Data = rs2.data
                };

            }
            catch (Exception ex)
            {
                l.AppendLine("Error get zalo info " + ex);
                return new
                {
                    ResponseCode = -99,
                    Description = "Hệ thống bận!"
                };
            }
            finally
            {
                ///l.AppendLine("****************************");
                NLogManager.MomoLog(l.ToString());
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

                l.AppendLine("***********CallBackZaloFromQienGate****************");
                l.AppendLine("requestBody: " + requestBody);
                l.AppendLine("checksum1: " + checksum1);
                l.AppendLine("checksum2: " + checksum2);
                if (checksum1 != checksum2)
                {
                    return new { err_code = 0, err_msg = "OK" };
                }

                //get charge code
                var chargeCode = Database.DAO.TransactionDAO.GetChargeCode(input.ticket, input.IntergerChargeType(), input.ref_id);
                if (chargeCode is null || string.IsNullOrEmpty(chargeCode.Data))
                {
                    return new { err_code = 0, err_msg = "OK" };
                }
                if (chargeCode.Status != (int)Lib.Constant.CHARGE_CODE_STATUS.Processing)
                {
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
                    chargeCode.ReceivedMoney = input.amount;
                    int rs = Database.DAO.TransactionDAO.AddTopupZalo(chargeCode, requestBody);
                    if (rs == 1) //cộng tiền user thành công
                    {
                        var myAccount = AccountDAO.Instance.GetProfile(chargeCode.UserId, 1);
                        if (myAccount.TelegramID != null && myAccount.TelegramID > 0)
                        {
                            string msgTele2 = "💰💰💰 NẠP TIỀN QUA ZALO THÀNH CÔNG 💰💰💰" +
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
    }
}
