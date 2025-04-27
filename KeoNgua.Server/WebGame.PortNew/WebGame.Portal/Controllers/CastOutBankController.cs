using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities.MyUSDT.Models.Exchanges;
using System.IO;
using MsWebGame.Portal.Database.DAO;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Messages;
using WebGame.Payment.Database.DAO;
using TraditionGame.Utilities.Constants;
using System.Text;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/CastOutBank")]
    public class CastOutBankController : BaseApiController
    {
        //public int Rate = 25;
        public int Rate = 0;
        [ActionName("GetListBankCode")]
        [HttpPost]
        public dynamic GetListBankCode()
        {
            // string apiKey = "4e344611-b324-47b1-9bb8-df353da48d03";
            string apiKey = "-";
            string url = "http://cucku.net:10007/api/Bank/getListBankCode?apiKey=" + apiKey;
            try
            {
                
                return new
                {
                    ResponseCode = 1,
                    Description = "",
                    Data = new BankDataAccount[] {

                        //new BankDataAccount { code= "MB", name= "Quân đội MBBank"},
                        //new BankDataAccount { code= "TCB", name= "Kỹ thương Techcombank"},
                        //new BankDataAccount { code= "VCB", name= "Vietcombank"},
                        //new BankDataAccount { code= "BIDV", name= "Đầu tư và phát triển BIDV"},
                        //new BankDataAccount { code= "VIETINBANK", name= "Công thương VN - VietinBank"},
                        //new BankDataAccount { code= "VBA", name= "Nông nghiệp và phát triển nông thôn - Agri Banh"},
                        //new BankDataAccount { code= "VPB", name= "Việt nam thịnh vượng VPBank"},
                        //new BankDataAccount { code= "VIB", name= "Ngân hàng Quốc tế"},
                        //new BankDataAccount { code= "SHB", name= "Sài gòn hà nội"},
                        //new BankDataAccount { code= "TPB", name= "Tiên phong TPBank"},
                        //new BankDataAccount { code= "MSB", name= "Hàng hải Msb"},
                        //new BankDataAccount { code= "SCB", name= "Sài gòn"},
                        //new BankDataAccount { code= "STB", name= "Sacombank"},
                        //new BankDataAccount { code= "ACB", name= "Á châu"},
                        //new BankDataAccount { code= "DAB", name= "Đông á"},
                        //new BankDataAccount { code= "LPB", name= "Bưu điện liên việt"},
                        //new BankDataAccount { code= "SHBVN", name= "Shinhan việt nam"},
                        //new BankDataAccount { code= "CITIBANK", name= "CItiBank việt nam"},
                        //new BankDataAccount { code= "HSBC", name= "HSBC việt nam"},
                        //new BankDataAccount { code= "SEABANK", name= "TMCP Dong nam á - SeaBank"},
                        //new BankDataAccount { code= "HDB", name= "TMCP Phát triển TP.HCM - HDB"},
                        //new BankDataAccount { code= "NCB", name= "TMCP Quốc dân - NCB"},
                        //new BankDataAccount { code= "PBVN", name= "TNHH MTV Public VietNam - PBVN"},
                        //new BankDataAccount { code= "OCB", name= "TMCP PHUONG DONG - OCB"},

                        new BankDataAccount { code= "ABA", name= "ABA Bank - ABA"},
                        new BankDataAccount { code= "ACLEDA", name= "ACLEDA Bank - ACLEDA"},
                        new BankDataAccount { code= "WING", name= "Wing Bank - Wing"},
                        new BankDataAccount { code= "EMONEY", name= "eMoney"},
                        new BankDataAccount { code= "TRUE", name= "True Money"},
                    },
                    Rate = this.Rate
                };


            }
            catch (Exception ex)
            {
                MopayListBankResponse objReturn = new MopayListBankResponse();
                objReturn.stt = -99;

                return new
                {
                    ResponseCode = -99,
                    Description = "",
                    Data = objReturn.data,
                    Rate = this.Rate
                };
            }
        }

        [ActionName("ChargeOut")]
        [HttpOptions, HttpPost]
        public dynamic ChargeOut([FromBody] dynamic input)
        {
            long accountId = AccountSession.AccountID;
            string Nickname = AccountSession.AccountName;
            string SoTk = input.SoTk ?? string.Empty;
            string NameTk = input.NameTk ?? string.Empty;
            string Amount = input.Amount ?? string.Empty;
            string BankName = input.BankName ?? string.Empty;
            string BankCode = input.BankCode ?? string.Empty;
            string Otp = input.Otp ?? string.Empty;
            if (string.IsNullOrEmpty(SoTk) || string.IsNullOrEmpty(NameTk) || string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(BankName) || string.IsNullOrEmpty(BankCode))
            {
                return new
                {
                    ResponseCode = -99,
                    Message = "Uploaded data is incorrect"
                };
            }
            Otp = Otp.Trim();
            if (Otp.Length != OTPAPP_LENGTH && Otp.Length != OTPSMS_LENGTH && Otp.Length != OTPSAFE_LENGTH)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.OtpLengthInValid
                };
            }
            var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
            if (account == null)
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            if (account.Status != 1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountLock
                };
            }
            int resOtp = 0;
            long otpID;
            string otmsg;
            SMSDAO.Instance.ValidOtp(account.AccountID, Otp.Length == OTPSAFE_LENGTH ? account.PhoneSafeNo : account.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
            if (resOtp != 1)
            {
                return new
                {
                    ResponseCode = -5,
                    Message = Otp.Trim().Length == 6 ? "Otp is not correct" : ErrorMsg.OtpIncorrect
                };
            }
            long Balance;
            int Response;

            long AmountNumber = long.Parse(Amount);
          
            if (AmountNumber < 10)
            {
                return new
                {
                    ResponseCode = -99,
                    Message = "Minimum withdrawal amount 10"
                };
            }
            CasoutDAO.Instance.Casout_Bank(accountId, this.Rate, Nickname, SoTk, utf8Convert(NameTk).ToUpper(), AmountNumber, BankCode, utf8Convert( BankName), 1,out Balance,out Response);
            if (Response == 1)
            {
                SendTelePushTelegramID(12, "Nickname: " + account.AccountName+". Withdraw the amount: "+ AmountNumber + " from the Bank: " + utf8Convert(BankName) + ", Bank number: "+ SoTk + " (" + NameTk + ")", 0, false, account.AccountName);
                return new
                {
                    ResponseCode = 1,
                    Message = "Change Bank successfully!",
                    Balance = Balance,
                };
            }else if (Response == -504)
            {
                return new
                {
                    ResponseCode = Response,
                    Message = "Your amount is not enough"
                };
            }
            else
            {
                return new
                {
                    ResponseCode = Response,
                    Message = "An error occurred, please try again"
                };
            }
           
        }
        [ActionName("Momo")]
        [HttpOptions, HttpPost]
        public dynamic Momo([FromBody] dynamic input)
        {
            long accountId = AccountSession.AccountID;
            string Nickname = AccountSession.AccountName;
            string Phone = input.Phone ?? string.Empty;
            string Amount = input.Amount ?? string.Empty;
            string Name = input.Name ?? string.Empty;
            string Otp = input.Otp ?? string.Empty;
            if (string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(Name))
            {
                return new
                {
                    ResponseCode = -99,
                    Message = "Uploaded data is incorrect"
                };
            }
            Otp = Otp.Trim();
            if (Otp.Length != OTPAPP_LENGTH && Otp.Length != OTPSMS_LENGTH && Otp.Length != OTPSAFE_LENGTH)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.OtpLengthInValid
                };
            }
            var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
            if (account == null)
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            if (account.Status != 1)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AccountLock
                };
            }
            int resOtp = 0;
            long otpID;
            string otmsg;
            SMSDAO.Instance.ValidOtp(account.AccountID, Otp.Length == OTPSAFE_LENGTH ? account.PhoneSafeNo : account.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
            if (resOtp != 1)
            {
                return new
                {
                    ResponseCode = -5,
                    Message = "Otp is not correct"//Otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                };
            }
            long Balance;
            int Response;

            long AmountNumber = long.Parse(Amount);
            if (AmountNumber < 10)
            {
                return new
                {
                    ResponseCode = -99,
                    Message = "Minimum withdrawal amount 10"
                };
            }
            CasoutDAO.Instance.Casout_Momo(accountId, Nickname, this.Rate , Phone, utf8Convert(Name).ToUpper(), AmountNumber, 1, out Balance, out Response);
            if (Response == 1)
            {
                SendTelePushTelegramID(11, "Nickname : " + account.AccountName + " Create a request to withdraw Momo amount :" + AmountNumber + " to Momo:"+ Phone + "(" + Name + ")", 0, false, account.AccountName);
                return new
                {
                    ResponseCode = 1,
                    Message = "Exchange Momo successfully!",
                    Balance = Balance,
                };
            }
            else if (Response == -504)
            {
                return new
                {
                    ResponseCode = Response,
                    Message = "Your amount is not enough"
                };
            }
            else
            {
                return new
                {
                    ResponseCode = Response,
                    Message = "An error occurred, please try again"
                };
            }

        }
        [ActionName("GetMomo")]
        [HttpPost]
        public dynamic GetMomo()
        {
            return new
            {
                ResponseCode = 1,
                Description = "",
                Rate = this.Rate
            };
        }

        public string utf8Convert(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }
    }
 
}