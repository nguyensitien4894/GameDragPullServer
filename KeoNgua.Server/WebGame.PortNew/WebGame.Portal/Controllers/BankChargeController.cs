using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models.USDTBanks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.DynamicData;
using System.Web.Http;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Models;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.MyUSDT.Charges;
using TraditionGame.Utilities.MyUSDT.Models.Charges;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.SmartBanks.API.Charges;
using TraditionGame.Utilities.Utils;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Hubs;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/BankCharge")]
    public class BankChargeController : BaseApiController
    {
        protected int RequestType = 1;
        private int USDTPartnerID = 1;
        private int MOMO_MAINTAIN = -8;
        /// <summary>
        /// Lấy danh sách banks
        /// </summary>
        /// <returns></returns>
        [ActionName("GetChargeConfigs")]
        [HttpGet]
        public dynamic GetChargeConfigs()
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
                //Lấy tỉ lệ quy đổi 
                double rate = 0;
                //var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
                var bankOperator = USDTDAO.Instance.BankOperatorsSecondaryList(ServiceID);
                if (bankOperator == null || !bankOperator.Any())
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Can't find rate configuration",
                    };
                }
                var firstBanks = bankOperator.FirstOrDefault();
                if (firstBanks == null)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Can't find rate configuration",
                    };
                }
                rate = firstBanks.Rate;
                long totalRecord;
                var list = USDTDAO.Instance.GetListBanks(0, 0, null, null, null, 1, Int16.MaxValue, tkServiceID, out totalRecord);
                if (list == null || !list.Any())
                {
                    return new
                    {
                        ResponseCode = -1007,
                        Message = "Can't find rate configuration",
                    };
                }

                List<BankObject> listBankObject = new List<BankObject>();
                foreach (var bankOperatorsSecondary in bankOperator)
                {
                    List<Bank> tempBank = list.Where(c => c.BankOperatorsSecondaryID == bankOperatorsSecondary.ID && c.Status)
                        .ToList();
                    if (tempBank != null && tempBank.Count>0)
                    {
                        BankObject bankobject = new BankObject();
                        int dem = 0;
                        foreach (var bank in tempBank)
                        {
                            if(dem==0){
                                bankobject.ServiceID = bank.ServiceID;
                                bankobject.BankOperatorsSecondaryID = bank.BankOperatorsSecondaryID;
                                bankobject.OperatorName = bank.OperatorName;
                            }
                            bankobject.BankItems.Add(new BankInfo()
                            {
                                BankNumber = bank.BankNumber,
                                BankName = bank.BankName
                            });
                            dem++;
                        }
                        listBankObject.Add(bankobject);
                    }
                }
                //Lấy tỉ giá min max
                string minValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
                var min = ConvertUtil.ToLong(minValue);
                string maxValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
                var max = ConvertUtil.ToLong(maxValue);
                return new
                {
                    ResponseCode = 1,
                    Banks = listBankObject,
                    Rate = rate,
                    Min = 50000,
                    Max = max,
                    Content = displayName
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
        //public dynamic GetChargeConfigs()
        //{
        //    try
        //    {
        //        var accountId = AccountSession.AccountID;

        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        //Lấy tỉ lệ quy đổi 
        //        double rate = 0;
        //        var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
        //        if (bankOperator == null || !bankOperator.Any())
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Không tìm được cấu hình tỉ giá",
        //            };
        //        }
        //        var firstBanks = bankOperator.FirstOrDefault();
        //        if (firstBanks == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1006,
        //                Message = "Không tìm được cấu hình tỉ giá",
        //            };
        //        }
        //        rate = firstBanks.Rate;

        //        long lngValue;
        //        try
        //        {
        //            var listP = new List<string>();
        //            var listPartners = BankPartnersDAO.Instance.USDTPartnerList(ServiceID);
        //            if (listPartners == null || !listPartners.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = MOMO_MAINTAIN,
        //                    Message = ErrorMsg.MOMOLOCK + "-1"
        //                };
        //            }


        //            listP = listPartners.Where(c => c.Bank != "0").Select(c => c.Bank).ToList();


        //            if (listP == null || !listP.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = MOMO_MAINTAIN,
        //                    Message = ErrorMsg.MOMOLOCK + "-2"
        //                };
        //            }


        //            var strRandom = String.Join(",", listP);
        //            var listAcitve = strRandom.Split(',');
        //            var ListIntActive = listAcitve.Select(long.Parse).ToList().Where(c => c > 0).ToList();
        //            if (ListIntActive == null || !ListIntActive.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = MOMO_MAINTAIN,
        //                    Message = ErrorMsg.MOMOLOCK + "-3"
        //                };
        //            }

        //            Random rndActives = new Random();
        //            var intRandom = rndActives.Next(0, ListIntActive.Count);
        //            lngValue = ListIntActive[intRandom];


        //        }
        //        catch
        //        {
        //            return new
        //            {
        //                ResponseCode = MOMO_MAINTAIN,
        //                Message = ErrorMsg.MOMOLOCK + "-4"
        //            };
        //        }

        //        var usdtNormal = new List<long> { 1, 2 };
        //        var usdtSmartBank = new List<long> { 3, 4 };
        //        int Type = 0;
        //        var objReturn = new List<USDTChargeConfig>();
        //        if (usdtNormal.Contains(lngValue))
        //        {
        //            Type = 1;
        //            var banks = BankChargeApiApi.GetBankCharge();
        //            objReturn = banks.Select(c => new USDTChargeConfig
        //            {
        //                BankName = c.BankName,



        //            }).ToList();
        //        }
        //        else if (usdtSmartBank.Contains(lngValue))
        //        {
        //            Type = 2;
        //            var banks = SmartBankChargeApi.GetBankCharge();
        //            objReturn = banks.Banks.Select(c => new USDTChargeConfig
        //            {
        //                BankName = c.BankName,



        //            }).ToList();


        //        }





        //        //Lấy tỉ giá min max
        //        string minValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
        //        var min = ConvertUtil.ToLong(minValue);
        //        string maxValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
        //        var max = ConvertUtil.ToLong(maxValue);
        //        return new
        //        {
        //            ResponseCode = 1,
        //            Type = Type,
        //            Banks = objReturn,
        //            Rate = rate,
        //            Min = min,
        //            Max = max,
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Description = "Hệ thống đang bận, vui lòng quay lại sau"
        //        };
        //    }
        //}
        /// <summary>
        /// Lấy danh sách banks
        /// </summary>
        /// <returns></returns>
        //[ActionName("CreateBuyOrders")]
        //[HttpPost]
        //public dynamic CreateBuyOrders([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
        //        if (isOption)
        //        {
        //            return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
        //        }
        //        var accountId = AccountSession.AccountID;

        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
        //        if (account == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (account.Status != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountLock
        //            };
        //        }
        //        string privateKey = input.PrivateKey ?? string.Empty;//lấy ra privte key
        //        string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
        //        //if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
        //        //{

        //        //    return new
        //        //    {
        //        //        ResponseCode = -4,
        //        //        Message = ErrorMsg.CapchaRequired
        //        //    };
        //        //}
        //        //else
        //        //{
        //        //    if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
        //        //    {
        //        //        return new
        //        //        {
        //        //            ResponseCode = -100,
        //        //            Message = ErrorMsg.InValidCaptCha
        //        //        };
        //        //    }
        //        // }
        //        string minValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
        //        var min = ConvertUtil.ToLong(minValue);
        //        string maxValue;
        //        ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
        //        var max = ConvertUtil.ToLong(maxValue);
        //        string Amount = input.Amount ?? string.Empty; //Tiền VND
        //        var lngAmount = ConvertUtil.ToLong(Amount);
        //        if (lngAmount <= 0)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format("Số tiền không hợp lệ {0}", lngAmount),
        //            };
        //        }
        //        if (lngAmount < min)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format(ErrorMsg.MinAmountTranfer, min.LongToMoneyFormat()),
        //            };
        //        }
        //        if (lngAmount > max)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = String.Format("Số tiền tối đa được giao dịch {0}", max.LongToMoneyFormat()),
        //            };
        //        }
        //        string BankName = input.BankName ?? string.Empty;
        //        if (String.IsNullOrEmpty(BankName))
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = "Bạn chưa chọn ngân hàng",
        //            };
        //        }

        //        //xu y ly bat dau ty day
        //        //lay ra Type
        //        string Type = input.Type ?? string.Empty;
        //        if (String.IsNullOrEmpty(Type))
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = "Bạn chưa chọn Loại ngân hàng",
        //            };
        //        }
        //        double AmountRt = 0;
        //        string CodeRt = "";
        //        double RemainRt = 0;
        //        long TimeOutRt = 0;
        //        double AmountReceivedRt = 0;
        //        string BankNameRt = string.Empty;
        //        string MasterBankAccountRt = string.Empty;
        //        string MasterBankNameRt = string.Empty;
        //        if (Type == "1")
        //        {

        //            //lấy ra rate  trong dbs 
        //            double rate = 0;
        //            var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
        //            if (bankOperator == null || !bankOperator.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //                };
        //            }

        //            var firstBanks = bankOperator.FirstOrDefault();
        //            if (firstBanks == null)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //                };
        //            }
        //            if (!firstBanks.Status)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //                };
        //            }
        //            rate = firstBanks.Rate;
        //            if (rate <= 0)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Không tìm được cấu hình tỉ giá",
        //                };
        //            }




        //            ///Lấy ra phí cấu hình
        //            var feeObjet = BankChargeApiApi.GetRateConfig();
        //            if (feeObjet == null)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Hệ thống kết nối ngân hàng lỗi",
        //                };

        //            }
        //            double fee = feeObjet.DepositFeeFlat + feeObjet.DepositFeePct;
        //            var dbMoney = Math.Round(lngAmount * rate);
        //            var AmountReceived = Convert.ToInt64(dbMoney);//Tiền Game
        //            var description = String.Format("User:{0} nạp {1}", displayName, lngAmount);

        //            int Response;
        //            long RemainBalance;
        //            long RequestID;
        //            long TransID;
        //            //Khởi tạo order trong dbs 
        //            int PartnerID = 1;
        //            USDTDAO.Instance.UserBankRequestCreate(RequestType, accountId, lngAmount, 0, AmountReceived, PENDING_STATUS, PartnerID
        //                , ServiceID, description, null, null, null, rate, out Response, out RemainBalance, out RequestID, out TransID);
        //            if (Response != 1 || RequestID <= 0)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Không khởi tạo được order .Xin quay lại sau ",
        //                };
        //            }
        //            var ordersApi = BankChargeApiApi.SendBuyOrder(lngAmount);

        //            LoggerHelper.LogUSDTMessage(String.Format("BankChargeApiApi.SendBuyOrder Amount:{0}|AccountID {1}|Result:{2}", lngAmount, accountId, JsonConvert.SerializeObject(ordersApi, Formatting.None)));
        //            if (ordersApi == null || String.IsNullOrEmpty(ordersApi.Code))
        //            {
        //                USDTDAO.Instance.UserBankRequestUpdate(RequestID, null, accountId, PartnerID, ServiceID, FAIL_API1, PENDING, null, null, null, null, null, null, null, null, null, null, null, null, null, description, null, null, null, null, null, out Response);
        //                return new
        //                {
        //                    ResponseCode = -99,
        //                    Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau",
        //                };
        //            }
        //            if (String.IsNullOrEmpty(ordersApi.Code) || ordersApi.Status != PENDING)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -99,
        //                    Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau",
        //                };
        //            }

        //            //cập nhật tiếp các thông tin cơ bản  order trong dbs

        //            //trả về thông tin cho người dùng
        //            var acceptBanks = ordersApi.Note.Banks.FirstOrDefault(c => c.BankName == BankName);
        //            if (acceptBanks == null)
        //            {
        //                acceptBanks = ordersApi.Note.Banks.FirstOrDefault();
        //                if (acceptBanks == null)
        //                {
        //                    return new
        //                    {
        //                        ResponseCode = -99,
        //                        Message = "Hệ thống kết nối ngân hàng lỗi.Không tìm thấy tài khoản bank thích hợp tại " + BankName,
        //                    };
        //                }

        //            }

        //            //cập nhật lại thông tin order 

        //            USDTDAO.Instance.UserBankRequestUpdate(RequestID, ordersApi.Code, accountId, PartnerID, ServiceID, APPROVED_STATUS, PENDING, null, ordersApi.Rate, null, ordersApi.Amount, null, null, acceptBanks.BankName, acceptBanks.MasterBankName, acceptBanks.MasterBankAccount, ((int)ordersApi.HttpStatusCode).ToString(), null, ordersApi.HttpMsg, null, description, null, null, null, ordersApi.Note.ReceiveUsdtAddress, fee, out Response);
        //            if (Response != 1)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -99,
        //                    Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau",
        //                };
        //            }

        //            AmountRt = ordersApi.AmountVcc;
        //            CodeRt = ordersApi.Code;
        //            TimeOutRt = ordersApi.Timeout;
        //            RemainRt = ordersApi.Remaining;
        //            AmountReceivedRt = AmountReceived;
        //            BankNameRt = acceptBanks.BankName;
        //            MasterBankAccountRt = acceptBanks.MasterBankAccount;
        //            MasterBankNameRt = acceptBanks.MasterBankName;

        //        }
        //        else if (Type == "2")//logic smart bank
        //        {
        //            double rate = 0;
        //            var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, BankOperatorConfig);
        //            if (bankOperator == null || !bankOperator.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //                };
        //            }

        //            var firstBanks = bankOperator.FirstOrDefault();
        //            if (firstBanks == null)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //                };
        //            }
        //            if (!firstBanks.Status)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
        //                };
        //            }
        //            rate = firstBanks.Rate;
        //            if (rate <= 0)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Không tìm được cấu hình tỉ giá",
        //                };
        //            }





        //            var dbMoney = Math.Round(lngAmount * rate);
        //            var AmountReceived = Convert.ToInt64(dbMoney);//Tiền Game
        //            var description = String.Format("User:{0} nạp {1}", displayName, lngAmount);

        //            int Response;
        //            long RemainBalance;
        //            long RequestID;
        //            long TransID;
        //            //Khởi tạo order trong dbs 
        //            int PartnerID = 2;
        //            USDTDAO.Instance.UserBankRequestCreate(RequestType, accountId, lngAmount,0, AmountReceived, PENDING_STATUS, PartnerID
        //                , ServiceID, description, null, null, null, rate, out Response, out RemainBalance, out RequestID, out TransID);
        //            if (Response != 1 || RequestID <= 0)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -1006,
        //                    Message = "Không khởi tạo được order .Xin quay lại sau ",
        //                };
        //            }
        //            var ordersApi = SmartBankChargeApi.SendBuyOrder(lngAmount, BankName);

        //            NLogManager.LogMessage(JsonConvert.SerializeObject(ordersApi));
        //            if (ordersApi == null || String.IsNullOrEmpty(ordersApi.RequestCode) || ordersApi.ResponseCode != 1)
        //            {
        //                USDTDAO.Instance.UserBankRequestUpdate(RequestID, null, accountId, PartnerID, ServiceID, FAIL_API1, PENDING, null, null, null, null, null, null, null, null, null, null, null, null, null, description, null, null, null, null, null, out Response);
        //                return new
        //                {
        //                    ResponseCode = -99,
        //                    Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau 1",
        //                };
        //            }




        //            //cập nhật lại thông tin order 

        //            USDTDAO.Instance.UserBankRequestUpdate(RequestID, ordersApi.RequestCode, accountId, PartnerID, ServiceID, APPROVED_STATUS, PENDING, null,

        //                0, null, ordersApi.Amount, null, null, ordersApi.Bankname, ordersApi.BankAccount,

        //                ordersApi.BankNumber, ((int)ordersApi.HttpStatusCode).ToString(), null, ordersApi.HttpMsg, null, description, null, null, null, null, 0, out Response);
        //            if (Response != 1)
        //            {
        //                return new
        //                {
        //                    ResponseCode = -99,
        //                    Message = "Hệ thống kết nối ngân hàng lỗi. Đề nghị thử lại sau 2"+ Response,
        //                };
        //            }

        //            AmountRt = ordersApi.Amount;
        //            CodeRt = ordersApi.RequestCode;
        //            TimeOutRt = 120;
        //            RemainRt = 0;
        //            AmountReceivedRt = AmountReceived;
        //            BankNameRt = ordersApi.Bankname;
        //            MasterBankAccountRt = ordersApi.BankAccount;
        //            MasterBankNameRt = ordersApi.BankNumber;

        //        }




        //        return new
        //        {
        //            ResponseCode = 1,
        //            Orders = new
        //            {
        //                Amount = AmountRt,//Số tiền
        //                Code = CodeRt,//mã order
        //                Timeout = TimeOutRt,//Thời gian hết hạn của order
        //                Remain = RemainRt,
        //                AmountReceived = AmountReceivedRt,
        //                Banks = new
        //                {
        //                    BankName = BankNameRt,//Tên ngân hàng như vietcombank,bidv
        //                    MasterBankAccount = MasterBankAccountRt,//Số tài khoản cần chuyển vào
        //                    MasterBankName = MasterBankNameRt//Tên chủ tài khoản cần chuyển tiền vào
        //                }
        //            }
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Description = "Hệ thống đang bận, vui lòng quay lại sau 3"
        //        };
        //    }
        //}


        [ActionName("CreateBuyOrders")]
        [HttpPost]
        public dynamic CreateBuyOrders([FromBody] Bank1s input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }

                string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
                if ((String.IsNullOrEmpty(input.PrivateKey) || string.IsNullOrEmpty(captcha)))
                {

                    return new
                    {
                        ResponseCode = -4,
                        Message = ErrorMsg.CapchaRequired
                    };
                }
                else
                {
                    if (CaptchaCache.Instance.VerifyCaptcha(captcha, input.PrivateKey) < 0)//kiểm tra mã cap cha <0 error
                    {
                        return new
                        {
                            ResponseCode = -100,
                            Message = ErrorMsg.InValidCaptCha
                        };
                    }
                }
                var lngAmount = ConvertUtil.ToLong(input.Amount);

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
                if (input == null)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "The system pauses this function. Please come back later",
                    };
                }
                
                string NoiDung = input.NoiDung ?? string.Empty;//lấy ra noi dung ck
                if (string.IsNullOrEmpty(NoiDung))
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "The system pauses this function. Please come back later",
                    };
                } else
                {
                    //displayName + DateTime.Now.ToString("yyMMddh") // nickname2301133
                    if (String.Compare(NoiDung.ToLower(), (displayName + DateTime.Now.ToString("yyMMddh")).ToLower()) != 0)
                    {
                        return new
                        {
                            ResponseCode = -100,
                            Message = ErrorMsg.InvalidNote
                        };
                    }
                }

                string minValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
                var min = ConvertUtil.ToLong(minValue);
                string maxValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
                var max = ConvertUtil.ToLong(maxValue);
                //string Amount = input.Amount ?? string.Empty; //Tiền VND
                
                if (lngAmount <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Invalid amount {0}", lngAmount),
                    };
                }
                if (lngAmount < min)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.MinAmountTranfer, min.LongToMoneyFormat()),
                    };
                }
                if (lngAmount > max)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Maximum amount to be traded {0}", max.LongToMoneyFormat()),
                    };
                }
                //string opID = input.OperatorID ?? string.Empty;
                //int operatorID = ConvertUtil.ToInt(input.OperatorID);
                if (String.IsNullOrEmpty(input.BankName))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "You have not selected a bank",
                    };
                }
                //if (operatorID<1)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = "Bạn chưa chọn ngân hàng",
                //    };
                //}
                //xu y ly bat dau ty day
                //lay ra Type
                //string Type = input.Type ?? string.Empty;
                //if (String.IsNullOrEmpty(Type))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = "Bạn chưa chọn Loại ngân hàng",
                //    };
                //}
                double AmountRt = 0;
                string CodeRt = "";
                double RemainRt = 0;
                long TimeOutRt = 0;
                double AmountReceivedRt = 0;
                string BankNameRt = string.Empty;
                string MasterBankAccountRt = string.Empty;
                string MasterBankNameRt = string.Empty;
                if (input.Type == 1)
                {

                    //lấy ra rate  trong dbs 
                    double rate = 0;

                    var bankOperator = USDTDAO.Instance.BankOperatorsSecondaryList(ServiceID);
                    if (bankOperator == null || !bankOperator.Any())
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "The system pauses this function. Please come back later",
                        };
                    }
                    //var firstBanks = bankOperator.FirstOrDefault();
                    var isExistBank = bankOperator.Find(x => x.OperatorName == input.BankName);

                    if (isExistBank is null)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "The system pauses this function. Please come back later",
                        };
                    }
                    rate = isExistBank.Rate;
                    if (rate <= 0)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Can't find rate configuration",
                        };
                    }
                    ///Lấy ra phí cấu hình
                    var dbMoney = Math.Round(lngAmount * rate);
                    var AmountReceived = Convert.ToInt64(dbMoney);//Tiền Game
                    var description = String.Format("User:{0} deposit {1}", displayName, lngAmount);
                    int Response;
                    long RemainBalance;
                    long RequestID;
                    long TransID;
                    //Khởi tạo order trong dbs 
                    int PartnerID = 1;
                    USDTDAO.Instance.UserBankRequestChargeCreate(RequestType, accountId, lngAmount, 0, AmountReceived, PENDING_STATUS, PartnerID
                        , ServiceID, description, null, null, null, rate, out Response, out RemainBalance, out RequestID, out TransID);
                    if (Response != 1 || RequestID <= 0)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Unable to initialize order. Please come back later " + ". Code: 936" + ". Response:" + Response,
                        };
                    }
                    long totalRecord;
                    //cập nhật tiếp các thông tin cơ bản  order trong dbs
                    var list = USDTDAO.Instance.GetListBanksSecondary(0, isExistBank.ID, null, null, true, 1);
                    //var list = USDTDAO.Instance.GetListBanksSecondary(0, input.OperatorID, input.BankName, null, true, 1);
                    //trả về thông tin cho người dùng
                    if (list == null)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Unable to initialize order. Please come back later" + ". Code: 948",
                        };
                    }
                    var acceptBanks = list.OrderBy(c => Guid.NewGuid()).FirstOrDefault();
                    if (acceptBanks == null)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Unable to initialize order. Please come back later" + ". Code: 957",
                        };
                    }
                    

                    //cập nhật lại thông tin order 

                    USDTDAO.Instance.UserBankSecondaryRequestUpdate(RequestID, null, accountId, PartnerID, ServiceID, PENDING_STATUS, PENDING, null, rate, null, 0, null, null, acceptBanks.OperatorName, acceptBanks.BankName, acceptBanks.BankNumber, "1", null, NoiDung, null, description, null, null, null, "", 0, out Response,acceptBanks.ID,acceptBanks.BankOperatorsSecondaryID);
                    if (Response != 1)
                    {
                        return new
                        {
                            ResponseCode = -99,
                            Message = "Bank connection system error. Recommend to try again later",
                        };
                    } else
                    {
                        SendTelePushTelegramID(2, "**BANK** == " + ", ID: " + RequestID + ". Nickname: " + account.AccountName + ". Deposit: " + AmountReceived + " (" + lngAmount + " USD)" + ". Bank: " + acceptBanks.OperatorName + ", Account: " +  acceptBanks.BankName + ", Bank number: " + acceptBanks.BankNumber + ", Note: " + NoiDung, 0, false, account.AccountName);
                    }
                    AmountReceivedRt = AmountReceived;
                    BankNameRt = acceptBanks.OperatorName;
                    MasterBankAccountRt = acceptBanks.BankNumber;
                    MasterBankNameRt = acceptBanks.BankName;

                } else
                {
                    //lấy ra rate  trong dbs 
                    double rate = 0;

                    var bankOperator = USDTDAO.Instance.BankOperatorsSecondaryList(ServiceID);
                    if (bankOperator == null || !bankOperator.Any())
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "The system pauses this function. Please come back later",
                        };
                    }
                    //var firstBanks = bankOperator.FirstOrDefault();
                    var isExistBank = bankOperator.Find(x => x.OperatorCode.ToLower() == input.BankName.ToLower());

                    if (isExistBank is null)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "The system pauses this function. Please come back later",
                        };
                    }
                    rate = isExistBank.Rate;
                    if (rate <= 0)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Can't find rate configuration",
                        };
                    }
                    ///Lấy ra phí cấu hình
                    ///
                    var dbMoney = Math.Round(lngAmount * rate);
                    var AmountReceived = Convert.ToInt64(dbMoney);//Tiền Game
                    var description = String.Format("User:{0} deposit {1}", displayName, lngAmount);
                    int Response;
                    long RemainBalance;
                    long RequestID;
                    long TransID;
                    //Khởi tạo order trong dbs 
                    int PartnerID = 1;

                    USDTDAO.Instance.UserBankRequestChargeCreate(RequestType, accountId, lngAmount, 0, AmountReceived, PENDING_STATUS, PartnerID
                    , ServiceID, description, input.Receive_BankHolderName, input.Receive_BankNumber, input.BankName, rate, out Response, out RemainBalance, out RequestID, out TransID);
                    if (Response != 1 || RequestID <= 0)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Unable to initialize order. Please come back later " + ". Code: 936" + ". Response:" + Response,
                        };
                    }
                    else
                    {
                        var list = USDTDAO.Instance.GetListBanksSecondary(0, isExistBank.ID, null, null, true, 1);
                        //var list = USDTDAO.Instance.GetListBanksSecondary(0, input.OperatorID, input.BankName, null, true, 1);
                        //trả về thông tin cho người dùng
                        if (list == null)
                        {
                            return new
                            {
                                ResponseCode = -1006,
                                Message = "Unable to initialize order. Please come back later" + ". Code: 948",
                            };
                        }
                        var acceptBanks = list.OrderBy(c => Guid.NewGuid()).FirstOrDefault();
                        if (acceptBanks == null)
                        {
                            return new
                            {
                                ResponseCode = -1006,
                                Message = "Unable to initialize order. Please come back later" + ". Code: 957",
                            };
                        }

                        USDTDAO.Instance.UserBankSecondaryRequestUpdate(RequestID, null, accountId, PartnerID, ServiceID, PENDING_STATUS, PENDING, null, rate, null, 0, null, null, isExistBank.OperatorName, input.Receive_BankHolderName, input.Receive_BankNumber, "1", null, NoiDung, null, description, null, null, null, "", 0, out Response, acceptBanks.ID, acceptBanks.BankOperatorsSecondaryID);

                        SendTelePushTelegramID(2, "##AUTOBANK## == " + ", ID: " + RequestID + ". Nickname: " + account.AccountName + ". Deposit: " + AmountReceived + " (" + lngAmount + " USD)" + ". Bank: " + isExistBank.OperatorName + ", Account: " + input.Receive_BankHolderName + ", Bank number: " + input.Receive_BankNumber + ", Note: " + NoiDung, 0, false, account.AccountName);
                    }
                }
                
                return new
                {
                    ResponseCode = 1,
                    Orders = new
                    {
                        Amount = lngAmount,//Số tiền
                        AmountReceived = AmountReceivedRt,
                        Content = displayName,
                        Banks = new
                        {
                            BankName = BankNameRt,//Tên ngân hàng như vietcombank,bidv
                            MasterBankAccount = MasterBankAccountRt,//Số tài khoản cần chuyển vào
                            MasterBankName = MasterBankNameRt//Tên chủ tài khoản cần chuyển tiền vào
                        }
                    }
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
        /// call back khi xử lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ActionName("CallBackResult")]
        [HttpPost]
        public dynamic CallBackResult([FromBody] CallBackBuyOrderRequest model)
        {

            if (model == null)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                };
            }
            LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBackResult CallBackModel:{0}", JsonConvert.SerializeObject(model, Formatting.None)));
            var code = model.Code;
            if (String.IsNullOrEmpty(code))
            {
                return new
                {
                    ResponseCode = -1006,
                    Message = "Code cannot be empty"
                };
            }

            var accepStatus = new List<string>() { "completed", "canceled", "failed" };
            if (!accepStatus.Contains(model.Status.ToLower()))
            {
                var obj = new
                {
                    ResponseCode = -1007,
                    Message = "The code is not in the sequence ( completed, canceled,failed)"
                };
                HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
                return response;

            }
            if (model.Method.ToLower() != "buy")
            {
                return new
                {
                    ResponseCode = -1008,
                    Message = "Send method is not buy"
                };
            }
            int TotalRecord = 0;
            //1.lấy thông tin order theo code
            var orderByCode = USDTDAO.Instance.UserBankRequestList(null, null, code, null, null, null, ServiceID, 1, 1, out TotalRecord);
            if (orderByCode == null || !orderByCode.Any())
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = String.Format("No transaction with code {0}", code),
                };
            }
            var firstOrderCode = orderByCode.FirstOrDefault();
            if (firstOrderCode == null)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = String.Format("No transaction by code{0}", code),
                };
            }
            if (firstOrderCode.PartnerStatus != PENDING)
            {
                return new
                {
                    ResponseCode = -1006,
                    Message = String.Format("Order {0} status updated {1}", code, firstOrderCode.PartnerStatus),
                };
            }
            if (firstOrderCode.Status != APPROVED_STATUS)
            {
                return new
                {
                    ResponseCode = -1006,
                    Message = String.Format("Order {0} status updated {1}", code, firstOrderCode.Status),
                };
            }

            int Response;
            long RemainBalance;
            var Status = MappingStatus(model.Status.ToLower());
            if (model.Status.ToLower() == COMPLETED)
            {
                var dbMoney = Math.Round((double)(model.AmountVcc * firstOrderCode.Rate));
                var realAmountReceive = Convert.ToInt64(dbMoney);
                //1.Cộng tiền cho khách và cập nhật order
                USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Success, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, realAmountReceive, model.AmountVcc, realAmountReceive, model.Status, out RemainBalance, out Response);

                LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBack.Completed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount:{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus:{10}|RemainBalance:{11}| Response:{12}",
                    firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Success, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, realAmountReceive, model.AmountVcc, realAmountReceive, model.Status, RemainBalance, Response
                    ));
                if (Response == 1)
                {
                    try
                    {
                        var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                        dnaHelper.SendTransactionPURCHASE(firstOrderCode.UserID, 7, null, model.AmountVcc, model.AmountVcc);
                        LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", firstOrderCode.UserID, 7, model.AmountVcc, model.AmountVcc, firstOrderCode.RequestID));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        NLogManager.PublishException(ex);
                        return false;

                    }
                }

                var obj = new
                {
                    ResponseCode = Response,
                    Message = String.Format("Order {0} has been updated to Success {1}", code, Response),
                };
                HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
                return response;
            }
            else
            {
                var dbMoney = model.AmountVcc * firstOrderCode.Rate;
                var realAmountReceive = Convert.ToInt64(dbMoney);
                USDTDAO.Instance.UserBankRequestPartnerCheck(firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Fail, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, 0, model.AmountVcc, realAmountReceive, model.Status, out RemainBalance, out Response);
                //cập nhật lại trạng thái của order 
                LoggerHelper.LogUSDTMessage(String.Format("BankCharge.CallBack.Failed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount:{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus:{10}|RemainBalance:{11}| Response:{12}",
                   firstOrderCode.RequestID, firstOrderCode.UserID, Bank_Fail, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, 0, model.AmountVcc, realAmountReceive, model.Status, RemainBalance, Response
                   ));

                var obj = new
                {
                    ResponseCode = Response,
                    Message = String.Format("Order {0} updated to Fail {1}", code, Response),
                };
                HttpResponseMessage response = Request.CreateResponse<Object>(HttpStatusCode.OK, obj);
                return response;
            }

        }

        /// <summary>
        /// Lấy thông tin chi tiếp đơn hàng thông qua code
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("OrderDetails")]
        [HttpPost]
        public dynamic OrderDetails([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
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

                string Code = input.Code ?? string.Empty;
                int TotalRecord = 0;
                //1.Lấy ra order details từ dbs (accountId,orderCode)
                var orderByCode = USDTDAO.Instance.UserBankRequestList(null, accountId, Code, null, null, null, ServiceID, 1, 1, out TotalRecord);
                if (orderByCode == null || !orderByCode.Any())
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("No transaction by code {0}", Code),
                    };
                }
                var firstOrderCode = orderByCode.FirstOrDefault();
                if (firstOrderCode == null)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("No transaction by code {0}", Code),
                    };
                }
                if (firstOrderCode.UserID != accountId)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("The account is not authorized to view the transaction code {0}", Code),
                    };
                }
                //khởi tạo order trong api
                var ordersApi = BankChargeApiApi.GetOrderDeails(Code);
                if (ordersApi == null)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = "Bank connection system error. Recommend to try again later",
                    };
                }
                var acceptBanks = ordersApi.Note.Banks.FirstOrDefault();
                return new
                {
                    ResponseCode = 1,
                    Orders = new
                    {
                        Amount = ordersApi.AmountVcc,//Số tiền

                        Code = ordersApi.Code,//mã order
                        Timeout = ordersApi.Timeout,//Thời gian hết hạn của order
                        AmountReceived = firstOrderCode.ReceivedMoney,
                        Status = firstOrderCode.Status,
                        StatusStr = MappingStatuStr(firstOrderCode.Status, firstOrderCode.RequestType),
                        Banks = new
                        {
                            BankName = acceptBanks.BankName,//Tên ngân hàng như vietcombank,bidv
                            MasterBankAccount = acceptBanks.MasterBankAccount,//Số tài khoản cần chuyển vào
                            MasterBankName = acceptBanks.MasterBankName//Tên chủ tài khoản cần chuyển tiền vào
                        }

                    }
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


        [ActionName("GetHistory")]
        [HttpGet]
        public dynamic GetHistory()
        {
            try
            {

                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
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
                int TotalRecord = 0;
                var list = USDTDAO.Instance.UserBankRequestList(null, accountId, null, null, null, null, ServiceID, 1, 50, out TotalRecord);
                var objReturn = list.Select(c => new
                {
                    RequestID = c.RequestID,
                    StatusStr = MappingStatuStr(c.Status, c.RequestType),

                    RequestType = c.RequestType,
                    Type = c.RequestType == 1 ? "Recharge" : "Withdraw ",
                    Code = c.RequestCode,
                    RequestDate = c.RequestDate,
                    Status = c.Status,
                    AmountGame = c.RequestType == 1 ? ((c.Status == SUCCESS_STATUS) ? c.RealReceivedMoney : c.ReceivedMoney) : c.Amount,


                });
                return new
                {
                    ResponseCode = 1,
                    List = objReturn
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
    }
}
