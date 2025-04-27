using Microsoft.AspNet.SignalR.Json;
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
using WebGame.Payment.Database.DAO;

namespace MsWebGame.Portal.Controllers.Transaction
{
    public class MomoV2Controller : BaseApiController
    {
        /// <summary>
        /// Nạp momo: Qien Gate
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

                string APPROVE = Lib.Constant.MOMO_APPROVED;
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

                l.AppendLine("*************** GetMomoInfo **************");
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

                var rs1 = TransactionDAO.GetRate("momo");

                if (amount < rs1.MinTopup || amount > rs1.MaxTopup)
                {
                    l.AppendLine("Số tiền không hợp lệ!");
                    return new
                    {
                        ResponseCode = -3,
                        Description = "Số tiền nạp tối thiểu là " + Lib.General.FormatMoneyVND(rs1.MinTopup) + ", tối đa là " + Lib.General.FormatMoneyVND(rs1.MaxTopup)
                    };
                }

                rs2 = QienGate.TopupMomo(amount, accountID);

                return new
                {
                    ResponseCode = rs2.status,
                    Description = rs2.description,
                    Data = rs2.data
                };

            }
            catch (Exception ex)
            {
                l.AppendLine("Error get momo info " + ex);
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

                var input  = JsonConvert.DeserializeObject<QienCallbackParams>(requestBody);

                l.AppendLine("***********CallBackMomoFromQienGate****************");
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
                    int rs = Database.DAO.TransactionDAO.AddTopupMomo(chargeCode, requestBody);
                    if (rs == 1) //cộng tiền user thành công
                    {
                        var myAccount = AccountDAO.Instance.GetProfile(chargeCode.UserId, 1);
                        if (myAccount.TelegramID != null && myAccount.TelegramID > 0)
                        {
                            string msgTele2 = "💰💰💰 NẠP TIỀN QUA MOMO THÀNH CÔNG 💰💰💰" +
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

        [HttpPost]
        public dynamic Cashout(CashoutMomoParam p)
        {
            StringBuilder l = new StringBuilder();
            l.AppendLine("************ Cashout Momo **************\r\n");
            l.AppendLine("params: " + JsonConvert.SerializeObject(p));
            long accountID = AccountSession.AccountID;
            string accountName = AccountSession.AccountName;


            if (accountID == 0)
            {
                return new
                {
                    ResponseCode = -99,
                    Msg = "Hệ thống bận!"
                };
            }
            var myAccount = AccountDAO.Instance.GetProfile(accountID, 1);

            if (myAccount == null)
            {
                return new
                {
                    ResponseCode = -99,
                    Msg = "Hệ thống bận!"
                };
            }

            l.AppendLine("account: " + JsonConvert.SerializeObject(myAccount));
            try
            {
                if (p == null)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Msg = "Hệ thống bận!"
                    };
                }

                int resOtp;
                long otpID;
                string otmsg;
                //--Otp temp--

                if (String.IsNullOrEmpty(p.Otp))
                {
                    return new
                    {
                        ResponseCode = -99,
                        Msg = "Vui lòng nhập mã otp!"
                    };
                }
                SMSDAO.Instance.ValidOtp(accountID, myAccount.PhoneNumber, p.Otp, ServiceID, out resOtp, out otpID, out otmsg);
                l.AppendLine("resOtp: " + resOtp +
                    "\r\notmsg: " + otmsg +
                    "\r\notpID: " + otpID);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -1,
                        Msg = "Vui long nhập mã otp hợp lệ"
                    };
                }
                //-- ENd Otp temp--

                if (string.IsNullOrEmpty(p.PhoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1,
                        Msg = "Vui lòng nhập số điện thoại đăng ký momo!"
                    };
                }

                if (string.IsNullOrEmpty(p.ReceiverName))
                {
                    return new
                    {
                        ResponseCode = -1,
                        Msg = "Tên chủ tài khoản không được bỏ trống!"
                    };
                }
  



                var rs = TransactionDAO.GetRate("momo");

                if (p.Amount < rs.MinCashout || p.Amount > rs.MaxCashout)
                {
                    return new
                    {
                        Status = -1,
                        Msg = $"Số tiền rút tối thiểu từ {Lib.General.FormatMoneyVND(rs.MinCashout)}, tối đa là {Lib.General.FormatMoneyVND(rs.MaxCashout)}"
                    };
                }




                var deductAmount = Convert.ToInt64(p.Amount * 1.0 * rs.CashoutRate);

                if (myAccount.Balance < deductAmount)
                {
                    return new
                    {
                        Status = -62,
                        Msg = "Số dư không đủ để thực hiện giao dịch!"
                    };
                }

                long Balance = 0;
                int Response;
                //var r = TransactionDAO.CashoutBank(myAccount.AccountID, myAccount.AccountName, p.Amount, p.AccountBankNumber, p.AccountBankName, p.BankCode);
                CasoutDAO.Instance.Casout_Momo(myAccount.AccountID, myAccount.AccountName, 0, p.PhoneNumber, p.ReceiverName, p.Amount, 1, out Balance, out Response);
                //l.AppendLine("Cashout_Bank: " + JsonConvert.SerializeObject(r));
                if (Response == 1)
                {
                    var balance1 = myAccount.Balance;
                    myAccount = AccountDAO.Instance.GetProfile(accountID, 1);
                    if (myAccount != null)
                    {

                        string msgTele = "💰💰💰 ĐỔI THƯỞNG MOMO 💰💰💰" +
                                    "\n- Số tiền chuyển 💵" + Lib.General.FormatMoneyVND(p.Amount) +
                                    "\n- Tài khoản momo: " + p.PhoneNumber + " - " + p.ReceiverName +
                                    "\n- 🕜 " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") +
                                   "\n- Số dư trước 💲" + Lib.General.FormatMoneyVND(balance1) +
                                    "\n- Số dư sau 💲" + Lib.General.FormatMoneyVND(myAccount.Balance) +
                                    "\n- AccountID 😀" + myAccount.AccountID +
                                    "\n- Username: " + myAccount.AccountName +
                                    //"\n- displayname: " + myaccount.displayname +
                                    "\n- Tel: " + myAccount.PhoneNumber +
                                    "\n- VIP: " + myAccount.VP +
                                    "\n- Tổng nạp: " + Lib.General.FormatMoneyVND(myAccount.TotalTopup) +
                                    "\n- Tổng đổi: " + Lib.General.FormatMoneyVND(myAccount.TotalCashout) +
                                    "\n- Tổng cược: " + Lib.General.FormatMoneyVND(myAccount.TotalBet);
                        //"\n- vipcode: " + lib.general.formatmoneyvnd(myaccount.giftcode);
                        TeleNotify(Constant.CASHOUT_TELEGRAM_GROUP_CHAT_ID, msgTele);

                        if (myAccount.TelegramID != null && myAccount.TelegramID > 0)
                        {
                            string msgTele2 = "💰💰💰 ĐỔI THƯỞNG MOMO 💰💰💰" +
                                                            "\n- Số tiền chuyển 💵" + Lib.General.FormatMoneyVND(p.Amount) +
                                                            "\n- AccountID 😀" + myAccount.AccountID +
                                                            "\n- AccountName: " + myAccount.AccountName +
                                                            "\n- Tài khoản momo: " + p.PhoneNumber + " - " + p.ReceiverName +
                                                            "\n- Trạng thái: chờ duyệt" +
                                                            "\n- 🕜 " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");

                            TeleNotify(myAccount.TelegramID.ToString(), msgTele2);
                        }
                    }
                    return new
                    {
                        ResponseCode = Response,
                        Msg = "Hệ thống đã ghi nhận và đang xử lý, kết quả sẽ được thông báo sau ít phút."
                    };

                }
                else if (Response == -504)
                {
                    return new
                    {
                        ResponseCode = Response,
                        Msg = "Số tiền không đủ"
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = Response,
                        Msg = "Hệ thống bận"
                    };
                }
            }
            catch (Exception ex)
            {
                l.AppendLine("Error: " + ex);
            }
            finally
            {
                l.AppendLine("************** end ***************\r\n");
                NLogManager.MomoLog(l.ToString());
            }

            return new
            {
                Status = -99,
                Msg = "Hệ thống bận!"
            };
        }

    }

    public class CashoutMomoParam
    {
        public string PhoneNumber { get; set; }//số tk momo (sdt)
        public string ReceiverName { get; set; }//tên tk momo
        public int Amount { get; set; }
        public string Otp { get; set; }

    }
}
