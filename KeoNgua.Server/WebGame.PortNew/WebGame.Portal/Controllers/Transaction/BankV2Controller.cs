using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models.USDTBanks;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Lib;
using System.Text;
using System.Web;
using System.Windows.Documents;
using System.Collections.Generic;
using WebGame.Payment.Database.DAO;

namespace MsWebGame.Portal.Controllers.Transaction
{
    public class BankV2Controller : BaseApiController
    {
        

        /// <summary>
        /// Lấy danh sách banks
        /// </summary>
        /// <returns></returns>
        [ActionName("ChargeConfigs")]
        [HttpGet]
        public dynamic ChargeConfigs()
        {
            try
            {
                var accountId = AccountSession.AccountID;

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

                string APPROVE = Lib.Constant.BANK_APPROVED;
                if (APPROVE != "1")
                {
                    return new
                    {
                        ResponseCode = -1,
                        Description = "Hệ thống đang tạm ngưng để bảo trì!"
                    };
                }


                //Lấy tỉ lệ quy đổi 
                double rate = 0;
                var bankOperator = USDTDAO.Instance.BankOperatorsSecondaryList(ServiceID);
                
                if (bankOperator == null || !bankOperator.Any())
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Hệ thống đang bảo trì. Liên hệ chăm sóc khách hàng để được trợ giúp",
                    };
                }
                var firstBanks = bankOperator.FirstOrDefault();
                if (firstBanks == null)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Hệ thống đang bảo trì. Liên hệ chăm sóc khách hàng để được trợ giúp",
                    };
                }
                rate = firstBanks.Rate;
                var validBankCodes = bankOperator.Select(b => b.ShortOperatorCode).ToArray();


                //get banks from qien.org gateways
                var url = "https://ezconnectdgp.com/deposit/banks";
                HttpWebRequest webRequest = null;
                webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
                webRequest.Method = WebHelper.Method.GET.ToString();
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.UserAgent = "";
                webRequest.Timeout = 30000;
                webRequest.Headers["APIKEY"] = Lib.Constant.QUIEN_API_KEY;
                string res = Lib.WebHelper.WebResponseGet(webRequest);
                NLogManager.MomoLog("GetListBank from qien.org gateways: " + res);
                var rs = JsonConvert.DeserializeObject<BankListResponse>(res);
                if (rs.banks is null || rs.banks.Count == 0)
                {
                    return new
                    {
                        ResponseCode = -1007,
                        Message = "Hệ thống đang bảo trì. Liên hệ chăm sóc khách hàng để được trợ giúp",
                    };
                }

                // min max
                string minValue, maxValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
                return new
                {
                    ResponseCode = 1,
                    Banks = rs.banks.Where(bank => Array.IndexOf(validBankCodes, bank.bank_type) > -1),
                    Rate = rate,
                    Min = ConvertUtil.ToLong(minValue),
                    Max = ConvertUtil.ToLong(maxValue)
                   
                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = "The system is busy, please come back later"
                };
            }
        }



        /// <summary>
        /// Nạp tiền vào tk qua banking
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="Amount"></param>
        /// <param name="Captcha"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        [HttpPost]
        public dynamic RequestTopup([FromBody] BankRequestParams input)
        {
            TransferInfo transferBankInfo = new TransferInfo();
            StringBuilder l = new StringBuilder();
            try
            {
                string bankCode = input.BankCode;
                int amount = input.Amount;
                string captcha = input.Captcha;
                string captchaKey = input.PrivateKey;
                l.AppendLine("************ RequestTopupBank **************");
                l.AppendLine("bankCode: " + bankCode);
                l.AppendLine("amount: " + amount);
                l.AppendLine("captcha: " + captcha);
                l.AppendLine("captchaKey: " + captchaKey);
            
                long accountID = AccountSession.AccountID;
                string accountName = AccountSession.AccountName;
                var myAccount = AccountDAO.Instance.GetProfile(accountID, 1);
                l.AppendLine("accountID: " + accountID);
                if (accountID <= 0 || String.IsNullOrEmpty(accountName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                string APPROVE = Lib.Constant.BANK_APPROVED;
                if (APPROVE != "1")
                {
                    return new
                    {
                        ResponseCode = -1,
                        Description = "Hệ thống đang tạm ngưng để bảo trì!"
                    };
                }

                int captchaVeriryStatus = CaptchaCache.Instance.VerifyCaptcha(captcha, captchaKey);
                if (captchaVeriryStatus < 0)
                {
                    return new
                    {
                        ResponseCode = 0,
                        Description = "Mã captcha không chính xác!"
                    };
                }

                var rs1 = TransactionDAO.GetRate("bank");
                if (amount < rs1.MinTopup || amount > rs1.MaxTopup)
                {
                    l.AppendLine("Số tiền không hợp lệ!");
                    return new
                    {
                        ResponseCode = -3,
                        Description =  "Số tiền nạp tối thiểu là " + Lib.General.FormatMoneyVND(rs1.MinTopup) + ", tối đa là " + Lib.General.FormatMoneyVND(rs1.MaxTopup)
                    };
                }

               
                if (accountID <= 0)
                {
                    return new
                    {
                        ResponseCode = -4,
                        Description = "Tài khoản không hợp lệ"
                    };
                    
                }
                transferBankInfo = QienGate.TopupBank(amount, accountID, accountName, bankCode);
            }
            catch (Exception ex)
            {
                l.AppendLine("Error RequestTopupBank " + ex);
                transferBankInfo.status = -99;
                transferBankInfo.description = "Hệ thống bận!";
            }
            finally
            {
                l.AppendLine(JsonConvert.SerializeObject(transferBankInfo));
                l.AppendLine("***************************");
                NLogManager.MomoLog(l.ToString());
            }
            return new
            {
                ResponseCode = transferBankInfo.status,
                Description = transferBankInfo.description, 
                Data = transferBankInfo.data
            };
        }


        [HttpPost]
        public dynamic CallBackForQienGate([FromBody] QienCallbackParams input)
        {
            StringBuilder l = new StringBuilder();
            try
            {
                var checksum1 = HttpContext.Current.Request.Headers["Checksum"];
                var requestBody = WebClass.GetRequestBody();
                string checksum2 = QienGate.Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);
                
                l.AppendLine("***********CallBackBankFromQienGate****************");
                l.AppendLine("requestBody: " + requestBody);
                l.AppendLine("checksum1: " + checksum1);
                l.AppendLine("checksum2: " + checksum2);
                if (checksum1 != checksum2)
                {
                    l.AppendLine("Error: Checksum is Wrong");
                    return new { err_code = 0, err_msg = "OK" };
                }

                //get charge code
                var chargeCode = Database.DAO.TransactionDAO.GetChargeCode(input.ticket, input.IntergerChargeType(), input.ref_id);
                if (chargeCode is null || string.IsNullOrEmpty(chargeCode.Data))
                {
                    l.AppendLine("Chargecode không hợp lệ");
                    return new { err_code = 0, err_msg = "OK" };
                }
                if (chargeCode.Status != (int)Lib.Constant.CHARGE_CODE_STATUS.Processing)
                {
                    l.AppendLine("Status không hợp lệ");
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
                    int rs = Database.DAO.TransactionDAO.AddTopupBank(chargeCode, requestBody, input.bank_type);
                    if (rs == 1) //cộng tiền user thành công
                    {
                        var myAccount = AccountDAO.Instance.GetProfile(chargeCode.UserId, 1);
                        if (myAccount.TelegramID != null && myAccount.TelegramID > 0)
                        {
                            string msgTele2 = "💰💰💰 NẠP TIỀN QUA BANK THÀNH CÔNG 💰💰💰" +
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
            return new{err_code = 0,err_msg = "OK"};

        }



        [HttpGet]
        public dynamic CashoutBankList()
        {
            try
            {
                var accountId = AccountSession.AccountID;

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

                string APPROVE = Lib.Constant.CASHOUT_BANK_APPROVED;
                if (APPROVE != "1")
                {
                    return new
                    {
                        ResponseCode = -1,
                        Description = "Hệ thống đang tạm ngưng để bảo trì!"
                    };
                }


                //Lấy tỉ lệ quy đổi 
                var rs = TransactionDAO.GetRate("bank");
                return new
                {
                    ResponseCode = 1,
                    Description = "OK",
                    Banks = CashoutBanks,
                    Rate = rs.CashoutRate,
                    Min = rs.MinCashout,
                    Max = rs.MaxCashout

                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = "The system is busy, please come back later"
                };
            }
        }


        [HttpPost]
        public dynamic CashoutBank(CashoutBankParam p)
        {
            StringBuilder l = new StringBuilder();
            l.AppendLine("************ Cashout_bank **************\r\n");
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

                if (string.IsNullOrEmpty(p.AccountBankNumber))
                {
                    return new 
                    {
                        ResponseCode = -1,
                        Msg = "Số tài ngân hàng khoản không được bỏ trống!"
                    };
                }

                if (string.IsNullOrEmpty(p.AccountBankName))
                {
                    return new 
                    {
                        ResponseCode = -1,
                        Msg = "Tên chủ tài khoản không được bỏ trống!"
                    };
                }
                if (string.IsNullOrEmpty(p.BankCode))
                {
                    return new 
                    {
                        ResponseCode = -1,
                        Msg = "Vui lòng chọn ngân hàng!"
                    };
                }

                var bankSelected = CashoutBanks.Where(b => b.code == p.BankCode).FirstOrDefault();
                if (bankSelected == null)
                {
                    return new
                    {
                        ResponseCode = -1,
                        Msg = "Ngân hàng không hợp lệ!"
                    };
                }

                var rs = TransactionDAO.GetRate("bank");

                if (p.Amount < rs.MinCashout || p.Amount > rs.MaxCashout)
                {
                    return new 
                    {
                        Status = -1,
                        Msg = $"Số tiền rút tối thiểu từ {Lib.General.FormatMoneyVND(rs.MinCashout)}, tối đa là {Lib.General.FormatMoneyVND(rs.MaxCashout)}"
                    };
                }



 
                var deductAmount = Convert.ToInt64(p.Amount*1.0 * rs.CashoutRate);

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
                CasoutDAO.Instance.Casout_Bank(myAccount.AccountID, 0, myAccount.AccountName, p.AccountBankNumber, p.AccountBankName, p.Amount, p.BankCode, bankSelected.name, 1, out Balance, out Response);

                //l.AppendLine("Cashout_Bank: " + JsonConvert.SerializeObject(r));
                if (Response == 1)
                {
                    var balance1 = myAccount.Balance;
                    myAccount = AccountDAO.Instance.GetProfile(accountID, 1);
                    if (myAccount != null)
                    {

                        string msgTele = "💰💰💰 ĐỔI THƯỞNG BANK 💰💰💰" +
                                    "\n- Số tiền chuyển 💵" + Lib.General.FormatMoneyVND(p.Amount) +
                                    "\n- Tài khoản bank: " + p.AccountBankNumber + " - " + p.AccountBankName +
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
                            string msgTele2 = "💰💰💰 ĐỔI THƯỞNG BANK 💰💰💰" +
                                                            "\n- Số tiền chuyển 💵" + Lib.General.FormatMoneyVND(p.Amount) +
                                                            "\n- AccountID 😀" + myAccount.AccountID +
                                                            "\n- AccountName: " + myAccount.AccountName +
                                                            "\n- Tài khoản bank: " + p.AccountBankNumber + " - " + p.AccountBankName +
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
                        Msg = "An error occurred, please try again"
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

        private List<BankCode> CashoutBanks = new List<BankCode>() {
            new BankCode(){name= "ABBANK", code="ABB" },
            new BankCode(){name= "ACB", code="ACB" },
            new BankCode(){name= "Agribank", code="AGR" },
            new BankCode(){name= "Bac A Bank", code="BAB" },
            new BankCode(){name= "BanVietBank", code="VCCB" },
            new BankCode(){name= "Bao Viet Bank", code="BVB" },
            new BankCode(){name= "BIDV", code="BIDV" },
            new BankCode(){name= "Dong A Bank", code="DAB" },
            new BankCode(){name= "Eximbank", code="EIB" },
            new BankCode(){name= "HD Bank", code="HDB" },
            new BankCode(){name= "KienLong Bank", code="KLB" },
            new BankCode(){name= "LienVietPostBank", code="LPB" },
            new BankCode(){name= "Maritime Bank", code="MSB" },
            new BankCode(){name= "MB Bank", code="MBB" },
            new BankCode(){name= "Orient Commercial Bank", code="OCB" },
            new BankCode(){name= "PG Bank", code="PGB" },
            new BankCode(){name= "PVComBank", code="PVC" },
            new BankCode(){name= "Sacombank", code="STB" },
            new BankCode(){name= "Sai Gon Commercial Bank", code="SCB" },
            new BankCode(){name= "Saigonbank", code="SGB" },
            new BankCode(){name= "SeaBank", code="SEAB" },
            new BankCode(){name= "SHB bank", code="SHB" },
            new BankCode(){name= "Shinhan bank Việt Nam", code="SHBVN" },
            new BankCode(){name= "Techcombank", code="TCB" },
            new BankCode(){name= "TienPhong Bank", code="TPB" },
            new BankCode(){name= "VIB", code="VIB" },
            new BankCode(){name= "VietBank", code="VBB" },
            new BankCode(){name= "Vietcombank", code="VCB" },
            new BankCode(){name= "Vietinbank", code="VTB" },
            new BankCode(){name= "VP Bank", code="VPB" }
        };

    }

    public class CashoutBankParam
    {
        public string BankCode { get; set; }//tên ngân hàng
        public string AccountBankNumber { get; set; }//số tk ngân hàng
        public string AccountBankName { get; set; }//tên chủ tk ngân hàng
        public int Amount { get; set; }
        public string Otp { get; set; }

    }


    public class BankCode
    {
        public string code { get; set; }
        public string name { get; set; }

    }




}
