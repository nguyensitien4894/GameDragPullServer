using MsTraditionGame.Utilities.Messages;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Models;
using MsWebGame.CSKH.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Exchanges;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.MyUSDT.Charges;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Controllers
{
    public class BankExchagneController : BaseController
    {
        protected static string PENDING = "pending";//Đang chờ xử lý, khi mới tạo order
        protected static string PROCESSING = "processing";//Sau khi tiền/USDT nộp vào tk thành công
        protected static string COMPLETED = "completed";//xử lý thành công
        protected static string CANCELED = "canceled";//order bị cancel sau khi hết thừoi gian xử lý
        protected static string FAILED = "failed";//Order bị lỗi trogn quá tình xử lý
        private int PartnerID = 1;
        protected int WaitBank_Success = 3;
        protected int Bank_Fail = 0;
        protected int PENDING_STATUS = 0;
        protected int CONFIRM_STATUS = 2;
        protected int APPROVED_STATUS = 3;
        protected int FAIL_API1 = -1;
        protected int FAIL_API2 = -2;
        protected int SUCCESS_STATUS = 1;
        protected int CANCELLED_STATUS = 4;
        protected int REFUSED_STATUS = 5;
        protected static int CHECKER_ID = 5;
        #region lich sử nạp thẻ cào
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index(string nickName = null)
        {

            ViewBag.ServiceBox = GetServices();
            ViewBag.GetStatus = GetStatus();
            ViewBag.BankTypes = GetBankType();
            ViewBag.NickName = nickName;
            ViewBag.Partners = GetPartner(1);
            return View();
        }
        private List<SelectListItem> GetPartner(int SerViceID)
        {
            return new List<SelectListItem>()
            {

                new SelectListItem() {Text="HAPPY",Value="1" },
                new SelectListItem() {Text="SHOP THE NHANH",Value="2" },

            };
        }
        private List<SelectListItem> GetStatus()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() {Text="Thất bại-Order (Đã hoàn tiền cho khách Q)",Value="-1" },
                 new SelectListItem() {Text="Thất bại-Chuyển USDT (Admin confirm xử lý)",Value="-2" },
                new SelectListItem() {Text="Chờ xử lý",Value="0" },
                new SelectListItem() {Text="Thành công",Value="1" },
                new SelectListItem() {Text="Chờ phản hồi từ ngân hàng",Value="3" },

                new SelectListItem() {Text="Admin hủy rút (Đã hoàn tiền Q) ",Value="4" },
                new SelectListItem() {Text="Ngân hàng từ chối(Đã hoàn tiền Q)",Value="5" },
           
            };
        }
        private List<SelectListItem> GetBankType()
        {
            return new List<SelectListItem>()
            {
              
                new SelectListItem() {Text="Rút tiền",Value="2" }
            };
        }

        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }

        static MopayInfoResponse RequestBank(string requestId, long amount, string bank_code, string bank_account, string bank_accountName, string displayName)
        {
            NLogManager.LogMessage("DEeeeeeeeBbbbbbbUuuuuuuuG: CSKH.RequestBank");

            NLogManager.LogMessage("DEBUG: requestId:" + requestId + "|amount:" + amount);

            string apiKey = "4e344611-b324-47b1-9bb8-df353da48d03";
            string url = "http://cucku.net:10007/api/Bank/ChargeOut?apiKey=" + apiKey + "&bank_code=" + bank_code.Trim() + "&bank_account=" + bank_account;
            url = url + "&bank_accountName=" + bank_accountName;
            url = url + "&amount=" + amount.ToString();

            url = url + "&requestId=" + requestId+ "&callback="+ "https://callback01052023.sicbet.net/api/MopayBank/BankCallBackResultAction";
            /*
             
             http://mopay5.vnm.bz:10007/api/Bank/ChargeOut?apiKey=870ccddb-c8c0-49c9-9d8f-c4456268fc92&bank_code=TCB&bank_account=19036824815017&bank_accountName=K? thuong Techcombank&amount=50000&requestId=200021655&signature=5da3833b0265f255abfcf7f4abb35c59&msg=olala124
             */
            //signature = md5(bank_account + amount + requestId + loginPW),

            string content = bank_account + amount.ToString() + requestId + "Gd3@*yh80@9idjsa2%";

            string signature = MD5(content);
            url = url + "&signature=" + signature;

            url = url + "&msg=" + displayName;

            //Console.WriteLine("GetJsonMopayInfor : " + url);
            NLogManager.LogMessage("DEBUG: url:" + url);

            // return new MopayInfoResponse() {  stt = 1};
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                Console.WriteLine(result);
                MopayInfoResponse objReturn = JsonConvert.DeserializeObject<MopayInfoResponse>(result);
                return objReturn;
            }
            catch (Exception ex)
            {
                MopayInfoResponse objReturn = new MopayInfoResponse() { stt = -1 };
                objReturn.stt = -99;
                NLogManager.PublishException(ex);
                return objReturn;
            }
        }

        #region UserUSDTExamine_Old 
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserUSDTExamine_Old(long RequestID, long userId, int checkStatus)
        {
            long balance = 0;
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(RequestID);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Không tồn tại bản ghi để xử lý"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.Status != 0)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Trạng thái phê duyệt của USDT không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.UserID != userId)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Tài khoản hoàn tiền không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            //số tiền VND cần rút
            if (checkStatus == 1)
            {
                int res = 0;
                int SUCCESS_STATUS = 1;
                USDTDAO.Instance.UserBankRequestExamine(RequestID, userId, null, SUCCESS_STATUS, 0, 0, 0, AdminID, 0, null, null
                    , null, null, null, null, null
                    , out res, out balance);


                if (res == 1)
                {
                    //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                    //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankCode, usdt.BankNumber, usdt.Description, usdt.DisplayName);

                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UpdateSuccess
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int res = 0;
                int REJECT = 4;
                USDTDAO.Instance.UserBankRequestExamine(RequestID, userId, null, REJECT, 0, 0, 0, AdminID, 0, null, null
                    , null, null, null, null, null
                    , out res, out balance);


                if (res == 1)
                {
                    //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                    //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankAccount, usdt.BankNumber, usdt.BankName, usdt.DisplayName);

                    //if (mopayInfo.stt == 1)

                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UpdateSuccess
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserUSDTExamineWithNote(long RequestID, long userId, int checkStatus,string note)
        {
            long balance = 0;
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(RequestID);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Không tồn tại bản ghi để xử lý"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.Status != 0)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Trạng thái phê duyệt của USDT không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.UserID != userId)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Tài khoản hoàn tiền không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            //số tiền VND cần rút
            int res = 0;
            int REJECT = 4;
            USDTDAO.Instance.UserBankRequestExamine(RequestID, userId, null, REJECT, 0, 0, 0, AdminID, 0, null, null
                , null, null, null, null, note
                , out res, out balance);


            if (res == 1)
            {
                //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankAccount, usdt.BankNumber, usdt.BankName, usdt.DisplayName);

                //if (mopayInfo.stt == 1)

                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UpdateSuccess
                }, JsonRequestBehavior.AllowGet);

            }
            if (res == -35)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapNotFound
                }, JsonRequestBehavior.AllowGet);

            }
            if (res == -36)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserInValid
                }, JsonRequestBehavior.AllowGet);

            }
            if (res == -504)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AmountNotValid
                }, JsonRequestBehavior.AllowGet);

            }
            return Json(new
            {
                ResponseCode = res,
                Message = ErrorMsg.InProccessException
            }, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserUSDTChargeExamine(long RequestID, long userId, int checkStatus)
        {
            long balance = 0;
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(RequestID);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Không tồn tại bản ghi để xử lý"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.Status != 0)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Trạng thái phê duyệt của USDT không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.UserID != userId)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Tài khoản hoàn tiền không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            //số tiền VND cần nạp
            if (checkStatus == 1)
            {
                int res = 0;
                long intReceivedMoney = usdt.ReceivedMoney.HasValue ? (long)usdt.ReceivedMoney.Value : 0;
                int SUCCESS_STATUS = 1;
                USDTDAO.Instance.UserBankRequestChargeExamine(RequestID, userId, null, SUCCESS_STATUS, 0, 0, 0, AdminID, intReceivedMoney, null, null
                    , null, null, null, null, null
                    , out res, out balance);


                if (res == 1)
                {
                    //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                    //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankCode, usdt.BankNumber, usdt.Description, usdt.DisplayName);



                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = "Thành công: duyệt nạp" + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException + ".Code: " + res
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int res = 0;
                int REJECT = 4;
                USDTDAO.Instance.UserBankRequestChargeExamine(RequestID, userId, null, REJECT, 0, 0, 0, AdminID, 0, null, null
                    , null, null, null, null, null
                    , out res, out balance);


                if (res == 1)
                {
                    //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                    //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankAccount, usdt.BankNumber, usdt.BankName, usdt.DisplayName);

                    //if (mopayInfo.stt == 1)

                    //return Json(new
                    //{
                    //    ResponseCode = -1005,
                    //    Message = ErrorMsg.UpdateSuccess
                    //}, JsonRequestBehavior.AllowGet);

                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = "Thành công: hủy nạp" + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid + ".Code: " + res
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException + ".Code: " + res
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserUSDTManual(long RequestID, long userId, int checkStatus)
        {
            long balance = 0;
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(RequestID);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Không tồn tại bản ghi để xử lý"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.Status != 0)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Trạng thái phê duyệt của USDT không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.UserID != userId)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Tài khoản hoàn tiền không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            //số tiền VND cần rút
            if (checkStatus == 1)
            {
                int res = 0;
                int SUCCESS_STATUS = 1;
                USDTDAO.Instance.UserBankRequestExamine(RequestID, userId, null, SUCCESS_STATUS, 0, 0, 0, AdminID, 0, null, null
                    , null, null, null, null, null
                    , out res, out balance);


                if (res == 1)
                {
                    //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                    //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankCode, usdt.BankNumber, usdt.Description, usdt.DisplayName);

                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UpdateSuccess
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int res = 0;
                int REJECT = 4;
                USDTDAO.Instance.UserBankRequestExamine(RequestID, userId, null, REJECT, 0, 0, 0, AdminID, 0, null, null
                    , null, null, null, null, null
                    , out res, out balance);


                if (res == 1)
                {
                    //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                    //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankAccount, usdt.BankNumber, usdt.BankName, usdt.DisplayName);

                    //if (mopayInfo.stt == 1)

                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UpdateSuccess
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException
                }, JsonRequestBehavior.AllowGet);
            }






        }

        private ActionResult CreateSellOrders(long RequestID)
        {
            try
            {



                //Lấy thông tin BankName (đã chọn )



                var feeObjet = BankChargeApiApi.GetRateConfig();
                if (feeObjet == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Hệ thống kết nối ngân hàng lỗi",
                    });

                }
                if (feeObjet.WithdrawFeeFlat<=0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeeFlat<=0",
                    });

                }
                if (feeObjet.WithdrawFeePct <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeePct<=0",
                    });

                }
                if (feeObjet.SellRate <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.SellRate<=0",
                    });

                }

                long lngRequestID = ConvertUtil.ToLong(RequestID);
              

                //1.Lấy ra order details từ dbs (accountId,orderCode)
                var orderByCode = USDTDAO.Instance.UserBankRequestGetByID(lngRequestID);
                if (orderByCode == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Không tồn tại giao dịch theo mã {0}", lngRequestID),
                    });
                }

                //Tiền VND
                if (orderByCode.Amount <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = "Giá trị yêu cập  nạp không tồn tại",
                    });
                }
               
                var realMoney = orderByCode.ReceivedMoney.Value;//lấy ra số tiền VND (user muốn rút )
                var realFee = realMoney * feeObjet.WithdrawFeePct + feeObjet.WithdrawFeeFlat * feeObjet.SellRate;
                long target_amount =Convert.ToInt64(realMoney + realFee);


                var response = BankExchangesApi.SendSellOrder(target_amount, orderByCode.BankNumber, orderByCode.BankAccount, orderByCode.BankName);
                LoggerHelper.LogUSDTMessage(String.Format("BankExchangesApi.SendSellOrder(RequestID:{0})==>TargetAmount:{1}| BankAccount:{2}| BankAccountName:{3}| BankName:{4}|Result:{5}", RequestID, target_amount, orderByCode.BankNumber, orderByCode.BankAccount, orderByCode.BankName, JsonConvert.SerializeObject(response, Formatting.None)));

               
                if (response!=null&&response.Status == PENDING && !String.IsNullOrEmpty(response.Code))
                {
                    int Response = 0;
                 
                    //Gọi tiếp api số 2

                    double real_USDT = Math.Round(target_amount*1.0/ feeObjet.SellRate, 8);//lấy ra số tiền VND;
                    var address = response.Note.ReceiveUsdtAddress;

                    //var transactionRessponse = new MerchantTransactionsResponseModel();
                    //transactionRessponse.Id = 200;
                    var transactionRessponse = BankExchangesApi.MerchantTransactions(real_USDT, address);
                    LoggerHelper.LogUSDTMessage(String.Format("BankExchangesApi.MerchantTransactions(RequestID:{0})==>Amount:{1}| Result:{2}", RequestID, real_USDT, JsonConvert.SerializeObject(transactionRessponse, Formatting.None)));
                    if (transactionRessponse != null && transactionRessponse.Id > 0 )
                    {

                        string description = string.Empty;

                        long RemainBlance = 0;
                      
                        int AdminID = 5;
                        //USDTDAO.Instance.UserBankRequestExamine(orderByCode.RequestID, orderByCode.UserID, response.Code, WaitBank_Success, orderByCode.ServiceID.Value, response.AmountVcc, 
                        //    transactionRessponse.RealAmount, AdminID,
                        //    orderByCode.ReceivedMoney.Value, transactionRessponse.Amount,
                        //    response.Note.ReceiveUsdtAddress
                        //    , response.Status, response.HttpStatusCode.ToString(), response.HttpStatusMsg, null, null
                        //    , out Response, out RemainBlance);


                        USDTDAO.Instance.UserBankRequestUpdate2(orderByCode.RequestID, response.Code, orderByCode.UserID, PartnerID
                            , orderByCode.ServiceID.Value, WaitBank_Success, response.Status, null, null,null, transactionRessponse.Amount, null, null
                            ,null, null, null, response.HttpStatusCode.ToString(), null,response.HttpStatusMsg, null,null
                            , response.AmountVcc,null
                            , orderByCode.ReceivedMoney.Value, response.Note.ReceiveUsdtAddress, out Response);


                        
                        LoggerHelper.LogUSDTMessage(String.Format("USDTDAO.Instance.UserBankRequestExamine.Success==> RequestID:{0}|UserID:{1}|RequestCode:{2}|CheckStatus:{3}|ServiceID:{4}|RealAmount:{5}|RealUSDTAmount:{6}|ExamineUserID:{7}|RealReceivedMoney:{8}|USDTWalletAddress:{9}|Response:{10}|RemainBalance:{11}", orderByCode.RequestID, orderByCode.UserID, response.Code, WaitBank_Success, orderByCode.ServiceID.Value, response.AmountVcc, response.AmountVcc, 1, 0, response.Note.ReceiveUsdtAddress, Response, RemainBlance));
                        if (Response == 1)
                        {
                            return Json(new
                            {
                                ResponseCode = 1,
                                Message = "Duyệt thành công-đã gửi yêu cầu chuyển khoản ngân hàng."
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                ResponseCode = -1005,
                                Message = "Lỗi khi cập nhật thông tin Order" + Response
                            });
                        }
                    }
                    else
                    {
                        //lỗi api số 2 
                        USDTDAO.Instance.UserBankRequestUpdateStatus2(orderByCode.RequestID, response.Code, orderByCode.ServiceID.Value, ((int)transactionRessponse.HttpStatusCode).ToString(), null, (transactionRessponse.HttpMsg).ToString(), null, FAIL_API2,0, orderByCode.UserID, response.Status, null, null, null,null,null, response.Note.ReceiveUsdtAddress, PartnerID, out Response);
                        LoggerHelper.LogUSDTMessage(String.Format("UserBankRequestUpdateStatus.API_2 RequestID:{0}| RequestCode:{1}|ServiceID:{2}| PartnerErrorCode:{3}| FeedbackErrorCode:{4}|PartnerMessage:{5}|FeedbackMessage:{6}|Status:{7}|TransReqStatus:{8}|Response:{9}", orderByCode.RequestID, null, orderByCode.ServiceID.Value, ((int)response.HttpStatusCode).ToString(), null, response.HttpStatusMsg, null, FAIL_API1, CONFIRM_STATUS, Response));
                        //Lỗi chưa xử lý

                        return Json(new
                        {
                            ResponseCode = -10005,
                            Message = "Lỗi kết nối api số 2 -liên hệ USDT để xử lý "
                        });

                    }


                }
                else
                {
                  
                    //lỗi api số 1 ko lấy được thông tin 
                    //USDTDAO.Instance.UserBankRequestUpdateStatus(orderByCode.RequestID, null, orderByCode.ServiceID.Value, ((int)response.HttpStatusCode).ToString(), null, response.HttpStatusMsg, null, FAIL_API1, CONFIRM_STATUS, orderByCode.UserID, null,fee,null,null,null,null,null,PartnerID, out Response);
                    //LoggerHelper.LogUSDTMessage(String.Format("UserBankRequestUpdateStatus.API_1  RequestID:{0}| RequestCode:{1}|ServiceID:{2}| PartnerErrorCode:{3}| FeedbackErrorCode:{4}|PartnerMessage:{5}|FeedbackMessage:{6}|Status:{7}|TransReqStatus:{8}|Response:{9}", orderByCode.RequestID, null, orderByCode.ServiceID.Value, ((int)response.HttpStatusCode).ToString(), null, response.HttpStatusMsg, null, FAIL_API1, CONFIRM_STATUS, Response));

               
                    int res = 0;
                    long balance = 0;
                    int API_1_FAIL = -1;


                    USDTDAO.Instance.UserBankRequestExamine(RequestID, orderByCode.UserID,response.Code,API_1_FAIL, orderByCode.ServiceID.Value,0, 0, AdminID, 0, null,null
                        , response.Status, response.HttpStatusCode.ToString(), response.HttpStatusMsg, null, null,
                        out res, out balance);
                    //cập nhật lại trạng thái của order 
                    //LoggerHelper.LogUSDTMessage(String.Format("BankCharge.Exchange.Failed-RequestID:{0}| UserID:{1}| CheckStatus:{2}|ServiceID:{3}| RealUSDTAmount{4}|RequestRate:{5}|CheckerID:{6}|RequestAmount:{7}| RealAmount:{8}|RealReceivedMoney:{9}| string PartnerStatus{10}|RemainBalance:{11}| Response:{12}",
                    //orderByCode.RequestID, orderByCode.UserID, Bank_Fail, ServiceID, model.AmountUsdt, model.Rate, CHECKER_ID, 0, model.AmountVcc, realAmountReceive, model.Status, RemainBalance, Response
                    //   ));
                    return Json(new
                    {
                        ResponseCode = -10005,
                        Message = "Hệ thống ngân hàng kết nối lỗi-Chúng tôi đã hoàn tiền cho bạn"
                    });

                }






            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return Json(new
                {
                    ResponseCode = -99,
                    Description = "Hệ thống đang bận, vui lòng quay lại sau"
                });
            }
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetUSTDList(string nickName = null, string code = null, int? type = 2,
             DateTime? fromDate = null, DateTime? toDate = null, int? status = null, int? serviceId = null, int currentPage = 1)
        {


            if (string.IsNullOrEmpty(nickName))
                nickName = null;

            if (string.IsNullOrEmpty(code))
                code = null;
            ViewBag.GetStatus = GetStatus();

            int totalRecord = 0;
            int customerPageSize = 25;
            type = 2;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = USDTDAO.Instance.UserBankRequestList(null, nickName, code, fromDate, toDate, status, serviceId, type
               , currentPage, customerPageSize, out totalRecord);

            var pager = new Pager(totalRecord, (currentPage), customerPageSize, 10);
            //int totalPage = totalRecord / AppConstants.CONFIG.GRID_SIZE + 1;
            ViewBag.CurrentPage = pager.CurrentPage;
            ViewBag.TotalPage = pager.TotalPages;

            ViewBag.TotalRecord = pager.TotalItems;
            ViewBag.Prev = pager.Pre;
            ViewBag.Next = pager.Next;

            ViewBag.Start = pager.StartPage;
            ViewBag.End = pager.EndPage;
            ViewBag.StartIndex = pager.StartIndex + 1;
            ViewBag.EndIndex = pager.EndIndex + 1;
            ViewBag.TotalPage = pager.TotalPages;
            ViewBag.IsAdmin = Session["RoleCode"].ToString() == ADMIN_ROLE ? true : false;

            //int totalPage = totalRecord / AppConstants.CONFIG.GRID_SIZE + 1;
            //ViewBag.CurrentPage = currentPage;
            //ViewBag.TotalPage = totalPage;
            //ViewBag.TotalRecord = totalRecord;
            //ViewBag.Prev = currentPage != 1 ? currentPage - 1 : currentPage;
            //ViewBag.Next = currentPage == totalPage ? currentPage : currentPage + 1;
            //ViewBag.Start = (currentPage - 1) * 10 + 1;
            //ViewBag.End = currentPage == totalPage ? totalRecord : currentPage * 10;
            //ViewBag.IsAdmin = Session["RoleCode"].ToString() == "ADMIN" ? true : false;
            return PartialView(lstCard);
        }


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult GetUSDTByID(long requestId)
        {
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }

            int TotalRecord = 0;
            var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                ResponseCode = 1,
                obj = new
                {
                    usdt.DisplayName,
                    RequestName = usdt.RequestType == 1 ? "Nạp tiền" : "Rút tiền",
                    RequestCode = usdt.RequestCode,
                    StatusStr = usdt.Status,
                    AmountGame = usdt.ReceivedMoney,
                    AmountUSDT = usdt.USDTAmount,
                    AmountVND = usdt.Amount,

                }
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// hàm này check xem có refund ko 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="AmountGame"></param>
        /// <param name="AmountUSDT"></param>
        /// <param name="AmountVND"></param>
        /// <param name="CheckNote"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult CheckUSDT(long requestId, string CheckNote, int Type)
        {
            var acceptList = new List<String> { AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09 };
            if (!acceptList.Contains(AdminAccountName))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tài khoản không đủ quyền để thực hiện chứ năng này"
                }, JsonRequestBehavior.AllowGet);
            }
            var acceptTypeList = new List<int> { 1, 0 };
            if (!acceptTypeList.Contains(Type))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số type không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
          
           
            if (String.IsNullOrEmpty(CheckNote))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Không truyền giá trị note"
                }, JsonRequestBehavior.AllowGet);
            }


          
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(requestId);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Không tìm thấy bản ghi với mã Request ID " + requestId
                }, JsonRequestBehavior.AllowGet);
            }
           
            if (usdt.RequestType != 2)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Không được hoàn tiền ngoài giao dịch rút " + requestId
                }, JsonRequestBehavior.AllowGet);
            }
            if (usdt.RequestType == 2)
            {
                //check date
                if (!acceptList.Contains(AdminAccountName))
                {
                    TimeSpan span = DateTime.Now.Subtract(usdt.RequestDate);
                    if (span.TotalMinutes <= 20)
                    {
                        return Json(new
                        {
                            ResponseCode = -1,
                            Message = "Thời gian hoàn hoặc hủy phải trên 20 phút"
                        }, JsonRequestBehavior.AllowGet);

                    }
                }

                var acceptStaus = new List<int> { -2};
                if ((!acceptStaus.Contains(usdt.Status)) && Type == 1)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Không được hoàn tiền với request khác với chờ admin xử lý"
                    }, JsonRequestBehavior.AllowGet);
                }
                var acceptHuyStaus = new List<int> { -2};
                if ((!acceptHuyStaus.Contains(usdt.Status)) && Type == 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Không được hoàn tiền với request khác với chờ admin xử lý"
                    }, JsonRequestBehavior.AllowGet);
                }
               


            }
            //cập nhật trạng thái lại về 1 và không hoàn tiền cho khách,cập nhật lại trạng thái USDT,số USDT
            if (Type == 1)
            {
                var feeObjet = BankChargeApiApi.GetRateConfig();
                if (feeObjet == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Hệ thống kết nối ngân hàng lỗi",
                    });

                }
                if (feeObjet.WithdrawFeeFlat <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeeFlat<=0",
                    });

                }
                if (feeObjet.WithdrawFeePct <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.WithdrawFeePct<=0",
                    });

                }
                if (feeObjet.SellRate <= 0)
                {
                    return Json(new
                    {
                        ResponseCode = -1006,
                        Message = "Lỗi kết nố api.feeObjet.SellRate<=0",
                    });

                }
                var realMoney = usdt.ReceivedMoney.Value;//lấy ra số tiền VND (user muốn rút )
                var realFee = realMoney * feeObjet.WithdrawFeePct + feeObjet.WithdrawFeeFlat * feeObjet.SellRate;
                long target_amount = Convert.ToInt64(realMoney + realFee);
                double real_USDT = Math.Round(target_amount * 1.0 / feeObjet.SellRate, 8);//lấy ra số tiền VND;

                var ordersApi = BankChargeApiApi.GetOrderDeails(usdt.RequestCode);
                if (ordersApi == null)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Không thể lấy được thông tin Order từ đối tác"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (ordersApi.Status != COMPLETED)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Đối tác đang trả về mã khác"+ COMPLETED
                    }, JsonRequestBehavior.AllowGet);
                }
                int Response = 0;
                long RemainBalance = 0;
                string format = String.Format("Admin xủ lý cập nhật thành công {0}",CheckNote);
                USDTDAO.Instance.UserBankRequestExchangeAdjust(usdt.RequestID, 1, usdt.ServiceID.Value,real_USDT, AdminID, usdt.UserID, CheckNote, format, out Response, out RemainBalance);



                if (Response == 1)
                {
                    
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Thành công"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Hoàn tiền thất bại " + Response
                    }, JsonRequestBehavior.AllowGet);
                }

            }else
            {

                int Response = 0;
                long RemainBalance = 0;
                string format = String.Format("Admin xủ lý cập nhật thành công {0}", CheckNote);
                USDTDAO.Instance.UserBankRequestExchangeAdjust(usdt.RequestID, 0, usdt.ServiceID.Value,null, AdminID, usdt.UserID, CheckNote, format, out Response, out RemainBalance);
                if (Response == 1)
                {
                    
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Thành công"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Hoàn tiền thất bại " + Response
                    }, JsonRequestBehavior.AllowGet);
                }



            }

           


        }


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult CalcuatelUSDT(long requestId)
        {

            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }


           


            
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(requestId);
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            var feeObjet = BankChargeApiApi.GetRateConfig();
            if (feeObjet == null)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Hệ thống kết nối ngân hàng lỗi",
                });

            }
            if (feeObjet.WithdrawFeeFlat <= 0)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Lỗi kết nố api.feeObjet.WithdrawFeeFlat<=0",
                });

            }
            if (feeObjet.WithdrawFeePct <= 0)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Lỗi kết nố api.feeObjet.WithdrawFeePct<=0",
                });

            }
            if (feeObjet.SellRate <= 0)
            {
                return Json(new
                {
                    ResponseCode = -1006,
                    Message = "Lỗi kết nố api.feeObjet.SellRate<=0",
                });

            }

        


          
         
            var realMoney = usdt.ReceivedMoney.Value;//lấy ra số tiền VND (user muốn rút )
            var realFee = realMoney * feeObjet.WithdrawFeePct + feeObjet.WithdrawFeeFlat * feeObjet.SellRate;
            long target_amount = Convert.ToInt64(realMoney + realFee);
            double real_USDT = Math.Round(target_amount * 1.0 / feeObjet.SellRate, 8);//lấy ra số tiền VND;

            //var ordersApi = BankChargeApiApi.GetOrderDeails(usdt.RequestCode);
            //if (ordersApi == null)
            //{
            //    return Json(new
            //    {
            //        ResponseCode = -1,
            //        Message = "Không thể lấy được thông tin Order từ đối tác"
            //    }, JsonRequestBehavior.AllowGet);
            //}

            return Json(new
            {
                ResponseCode = 1,
                Amount = real_USDT,
             
            }, JsonRequestBehavior.AllowGet);
        }


        #endregion lịch sử nạp thẻ cào

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserUSDTExamine(long RequestID, long userId, int checkStatus)
        {
            long balance = 0;
            var usdt = USDTDAO.Instance.UserBankRequestGetByID(RequestID);
            #region validaet usdt
            if (usdt == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Không tồn tại bản ghi để xử lý"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.Status != 0)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Trạng thái phê duyệt của USDT không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            if (usdt.UserID != userId)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Tài khoản hoàn tiền không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            

            if (checkStatus == 1)
            {
                int res = 0;
                string msg = "";
                int SUCCESS_STATUS = 1;


  
                #region new code by roy
                //cập nhật status là 1: admin đã duyệt chờ gửi qua đối tác
                StringBuilder log = new StringBuilder(); ;
                log.Append("************Cập nhật status => 1 cho UserBankRequest *************");
                USDTDAO.Instance.UserBankRequest_AdminApprove(RequestID, 1, AdminID, out res, out msg);
                
                log.AppendLine("status 1: đã duyệt chờ gửi qua đối tác");
                log.AppendLine("res: " + res);
                log.AppendLine("msg: " + msg);

                NLogManager.MomoLog(log.ToString());
                if (res == 1)
                {

                    var cashoutRs = QienGate.CashoutBank(usdt.RequestID.ToString(), usdt.Amount, usdt.UserID, usdt.BankCode, usdt.BankNumber, usdt.AccountBankName);
                    /* new logic for status:
                        0: đã ghi nhận chờ duyệt
                        1: đã duyệt chờ gửi qua đối tác
                        -1: đối tác báo lổi, không thực hiện chuyển tiền. Cần refund tiền cho user, hoặc gửi lại yêu cầu cho đối tác
                        2: đã gửi qua đối tác, đối tác đã ghi nhận chờ callback kết quả.
                        -2: callback báo fail. Cần refund tiền cho user, hoặc gửi lại yêu cầu cho đối tác
                        3: callback báo thành công, đã chuyển tiền cho user. Thông báo mail cho user.
                     
                     */

                    if (cashoutRs != null && cashoutRs.err_code == 0)
                    {
                        //cập nhật status là 2: đã gửi qua đối tác, đối tác đã ghi nhận chờ callback kết quả.

                        USDTDAO.Instance.UserBankRequest_UpdateOnlyStatus(RequestID, 2, out res, out msg);

                        log.Append("************Cập nhật status => 2 cho UserBankRequest *************");
                        log.AppendLine("(status 2: đã gửi qua đối tác, đối tác đã ghi nhận chờ callback kết quả.");
                        log.AppendLine("res: " + res);
                        log.AppendLine("msg: " + msg);
                        NLogManager.MomoLog(log.ToString());
                        return Json(new
                        {
                            ResponseCode = -1005,
                            Message = "Đã gửi qua đối tác, đối tác đã ghi nhận chờ callback kết quả."
                        }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        //Cập nhật trại thái đã gửi qua đối tác, bị lổi

                        //cập nhật status là -1: đã gửi yêu cầu sang đối tác, đối tác báo lổi. 
                        USDTDAO.Instance.UserBankRequest_UpdateOnlyStatus(RequestID, -1, out res, out msg);
                        log.Append("************Cập nhật status => -1 cho UserBankRequest *************");
                        log.AppendLine("(status -1: đã gửi yêu cầu sang đối tác, đối tác báo lổi..");
                        log.AppendLine("res: " + res);
                        log.AppendLine("msg: " + msg);
                        return Json(new
                        {
                            ResponseCode = -1005,
                            Message = "Đã đã gửi yêu cầu sang đối tác, đối tác báo lổi."
                        }, JsonRequestBehavior.AllowGet);

                    }


                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = res,
                        Message = ErrorMsg.InProccessException
                    }, JsonRequestBehavior.AllowGet);
                }

                #endregion

            }
            else
            {
                int res = 0;
                int REJECT = 4;
                USDTDAO.Instance.UserBankRequestExamine(RequestID, userId, null, REJECT, 0, 0, 0, AdminID, 0, null, null
                    , null, null, null, null, null
                    , out res, out balance);


                if (res == 1)
                {
                    //MopayInfoResponse mopayInfo = RequestBank((casout.RequestId).ToString(), casout.Amount, casout.BankCode, casout.SoTK, casout.TenTK, casout.Nickname);

                    //MopayInfoResponse mopayInfo = RequestBank((RequestID).ToString(), usdt.Amount, usdt.BankAccount, usdt.BankNumber, usdt.BankName, usdt.DisplayName);

                    //if (mopayInfo.stt == 1)

                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UpdateSuccess
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -35)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.CardSwapNotFound
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -36)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserInValid
                    }, JsonRequestBehavior.AllowGet);

                }
                if (res == -504)
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotValid
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    ResponseCode = res,
                    Message = ErrorMsg.InProccessException
                }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult CashoutCallbackForQienGate()
        {
            StringBuilder l = new StringBuilder();
            try
            {

                var checksum1 = System.Web.HttpContext.Current.Request.Headers["Checksum"];
                var bodyStream = new StreamReader(System.Web.HttpContext.Current.Request.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                var requestBody = bodyStream.ReadToEnd();

                string checksum2 = QienGate.Checksum(ConfigurationManager.AppSettings["QUIEN_PRIVATE_KEY"], requestBody);
                var input = JsonConvert.DeserializeObject<QienCallbackParams>(requestBody);

                l.AppendLine("***********CallBackCashoutBankFromQienGate****************");
                l.AppendLine("requestBody: " + requestBody);
                l.AppendLine("checksum1: " + checksum1);
                l.AppendLine("checksum2: " + checksum2);
                if (checksum1 != checksum2)
                {
                    return Json(new { err_code = 0, err_msg = "OK" });
                }


                /* new logic for status:
                        0: đã ghi nhận chờ duyệt
                        1: đã duyệt chờ gửi qua đối tác
                        -1: đối tác báo lổi, không thực hiện chuyển tiền. Cần refund tiền cho user, hoặc gửi lại yêu cầu cho đối tác
                        2: đã gửi qua đối tác, đối tác đã ghi nhận chờ callback kết quả.
                        -2: callback báo fail. Cần refund tiền cho user, hoặc gửi lại yêu cầu cho đối tác
                        3: callback báo thành công, đã chuyển tiền cho user. Thông báo mail cho user.
                     
                     */

                var usdt = USDTDAO.Instance.UserBankRequestGetByID(Convert.ToInt64(input.ref_id));
                l.AppendLine("usdt: " + JsonConvert.SerializeObject(usdt));
                if (usdt == null || usdt.Status != 2)
                {
                    l.AppendLine("Error: dữ liệu đầu vào không hợp lệ");
                    return Json(new { err_code = 0, err_msg = "OK" });
                }

                int updateRs;
                string msg;

                //cập nhật callback
                USDTDAO.Instance.UserBankRequest_UpdateCallbackResult(input, out updateRs, out msg);
                if (updateRs != 1)
                {
                    l.AppendLine("Error:  updateRs" + updateRs);
                    l.AppendLine("msg" + msg);
                }

                if (input.err_code == 0)
                {
                    //Đối tác đã chuyển thành công
                    //send tele cho user
                    var user = AccountProfileDAO.Instance.GetAccountInfor(usdt.UserID, null, null, 1);
                    if (user != null && user.TelegramID != null && user.TelegramID > 0)
                    {
                        string msgTele2 = "💰💰💰 RÚT TIỀN QUA BANK THÀNH CÔNG 💰💰💰" +
                                                        "\n- Số tiền đã rút 💵" + MoneyExtension.FormatMoneyVND(input.amount) +
                                                        "\n- AccountID 😀" + user.AccountID +
                                                        "\n- AccountName: " + user.AccountName +
                                                        "\n- 🕜 " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");

                        Telegram.send(user.TelegramID.ToString(), msgTele2);
                    }

                    //send tele cho group quản trị 
                    if (user != null)
                    {
                        string msgTele = "💰💰💰 BANK CASHOUT THÀNH CÔNG 💰💰💰" +
                                    "\n- Số tiền chuyển đã chuyển" + MoneyExtension.FormatMoneyVND(input.amount) +
                                    "\n- Tài khoản bank nhận: " + input.bank_number + " - " + input.bank_type +
                                    "\n- 🕜 " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") +
                                    "\n- AccountID 😀" + user.AccountID +
                                    "\n- Username: " + user.AccountName;

                        Telegram.send(ConfigurationManager.AppSettings["CASHOUT_TELEGRAM_GROUP_CHAT_ID"], msgTele);
                    }
                }
                else
                {
                    string msgTele = "⚠⚠⚠ BANK CASHOUT THẤT BẠI ⚠⚠⚠" +
                                    "\n- requestBody :" + requestBody;
                    Telegram.send(ConfigurationManager.AppSettings["CASHOUT_TELEGRAM_GROUP_CHAT_ID"], msgTele);
                }

                if (updateRs != 1)
                {
                    string msgTele = "⚠⚠⚠ KHÔNG CẬP NHẬT ĐƯỢC DỮ LIỆU CALLBACK ⚠⚠⚠" +
                                    "\n- requestBody :" + requestBody;
                    Telegram.send(ConfigurationManager.AppSettings["CASHOUT_TELEGRAM_GROUP_CHAT_ID"], msgTele);
                }


            }
            catch (Exception ex)
            {

                l.AppendLine("" + ex);
                return Json(new { err_code = 0, err_msg = "OK" });
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());

            }

            return Json(new { err_code = 0, err_msg = "OK" });
        }


    }


 

}