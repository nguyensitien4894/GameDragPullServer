using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Models;
using MsWebGame.CSKH.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Utils;
using static NPOI.HSSF.Util.HSSFColor;


namespace MsWebGame.CSKH.Controllers
{
    public class MopayInfoResponse
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public MopayData data { get; set; }
    }
    public class MopayData
    {
        public int id { get; set; }
    }
    //public class CasoutMomo
    //{

    //    public long Id { get; set; }
    //    public long UserId { get; set; }
    //    public string Nickname { get; set; }
    //    public string Name { get; set; }
    //    public string Phone { get; set; }
    //    public long Amount { get; set; }
    //    public long Price { get; set; }
    //    public string BankCode { get; set; }
    //    public string BankName { get; set; }
    //    public int Status { get; set; }
    //    public DateTime CreateTime { get; set; }
    //    public DateTime? UpdateTime { get; set; }
    //    public DateTime? ActionTime { get; set; }
    //    public int Service { get; set; }
    //    public long RequestId { get; set; }
    //}

    public class MomoController : BaseController
    {
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
        
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult MomoExchange(string nickName = null)
        {
           ViewBag.ServiceBox = GetServices();
            ViewBag.GetStatus = GetStatus();
            ViewBag.BankTypes = GetBankType();
            ViewBag.NickName = nickName;
            ViewBag.Partners = GetPartner(1);
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult MomoExchangeCharge(string nickName = null)
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.GetStatus = GetStatus();
            ViewBag.BankTypes = GetBankType();
            ViewBag.NickName = nickName;
            ViewBag.Partners = GetPartner(1);
            return View();
        }
        


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserMomoChargeExamine(long requestID, long userId, int checkStatus)
        {
            int Response = 0;
            var usdt = MOMODAO.Instance.UserMomoRequestGetByID(requestID);
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
                    Message = "Trạng thái phê duyệt của Ngân hàng không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            MOMODAO.Instance.UserMomoRequestUpdate(requestID, userId, checkStatus, AdminID, out Response);

            NLogManager.LogMessage(string.Format("userId:{0} | checkStatus:{1} | response:{2} | AdminID:{3} ",
                userId, checkStatus, Response, AdminID));

            if (Response == 1 && checkStatus == 1)
            {
                //MopayInfoResponse mopayInfo = RequestMomo(requestID.ToString(), usdt.Amount, usdt.RequestCode, usdt.UserID.ToString(), usdt.DisplayName);
                //MopayInfoResponse mopayInfo = RequestMomo(requestID.ToString(), usdt.Amount, usdt.RequestCode, null, usdt.DisplayName);

                //if (mopayInfo.stt == 1)

                int PartnerID = 1;
                string PartnerErrorCode = "200";
                long ReceivedMoney = 0;
                long RemainBalance = 0;
                long RequestID = 0;
                double RequestRate = 0;
                int RequestType = 1;
                int outServiceID = 0;
                long intReceivedMoney = usdt.ReceivedMoney.HasValue ? (long)usdt.ReceivedMoney.Value : 0;


                //MOMODAO.Instance.UserMomoRequestPartnerCheck(
                //        userId, 1, intReceivedMoney, 1, usdt.MomoNumber,
                //        "1", "200",
                //        null, null,
                //        null, null, null, null, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
                //    );

                //MOMODAO.Instance.UserMomoRequestPartnerCheck(
                //        userId, RequestType, intReceivedMoney, PartnerID, usdt.MomoNumber,
                //        "1", PartnerErrorCode,
                //usdt.DisplayName, null,
                //        null, usdt.RequestID.ToString(), usdt.RequestID.ToString(), null, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
                //    );

                //NLogManager.LogMessage(userId + "|" + 1 + "|");


                var api = new ApiUtil<bool>();
                api.ApiAddress = ConfigurationManager.AppSettings["PortalUrl"].ToString();
                api.URI = "/api/CardCharging/SendChargeResult";
                var res = api.Send(new { AccountId = userId, Balance = usdt.ReceivedMoney, Msg = "Nạp Momo thành công " + usdt.ReceivedMoney, Status = 1, ServiceID = ServiceID });
                NLogManager.LogMessage("SendChargingNap" + res);

                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            if (Response == 1 && checkStatus == -2) {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                ResponseCode = Response,
                Message = "Có lỗi xảy ra"
            }, JsonRequestBehavior.AllowGet);


        }
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserMomoSwapManual(long requestID, long userId, int checkStatus)
        {
            int Response = 0;
            var usdt = MOMODAO.Instance.UserMomoRequestGetByID(requestID);
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
                    Message = "Trạng thái phê duyệt của Ngân hàng không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            MOMODAO.Instance.UserMomoRequestUpdate(requestID, userId, checkStatus, AdminID, out Response);

            NLogManager.LogMessage(string.Format("userId:{0} | checkStatus:{1} | response:{2} | AdminID:{3} ",
                userId, checkStatus, Response, AdminID));

            if (Response == 1)
            {
                //MopayInfoResponse mopayInfo = RequestMomo(requestID.ToString(), usdt.Amount, usdt.RequestCode, usdt.UserID.ToString(), usdt.DisplayName);
                //MopayInfoResponse mopayInfo = RequestMomo(requestID.ToString(), usdt.Amount, usdt.RequestCode, null, usdt.DisplayName);

                //if (mopayInfo.stt == 1)

                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Thành công: Manual"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                ResponseCode = Response,
                Message = "Có lỗi xảy ra"
            }, JsonRequestBehavior.AllowGet);


        }
        private List<SelectListItem> GetStatus()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() {Text="Chờ xử lý (auto)",Value="2" },
                new SelectListItem() {Text="Thành công",Value="1" },
                new SelectListItem() {Text="Chờ xử lý",Value="0" },
                new SelectListItem() {Text="Gửi yêu cầu thất bại",Value="-1" },
                new SelectListItem() {Text="Thất bại",Value="-2" },

            };
        }
        private List<SelectListItem> GetPartner(int SerViceID)
        {
            return new List<SelectListItem>()
            {

                new SelectListItem() {Text="HAPPY",Value="1" },
                new SelectListItem() {Text="SHOP THE NHANH",Value="2" },
             
            };
        }
        private List<SelectListItem> GetBankType()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() {Text="Nạp tiền",Value="1" },
                //new SelectListItem() {Text="Rút tiền",Value="2" }
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
        
        static MopayInfoResponse RequestMomo(string requestId, long amount, string account, string account_name, string displayName)
        {
            NLogManager.LogMessage("DEeeeeeeeBbbbbbbUuuuuuuuG: CSKH.RequestMomo");

            NLogManager.LogMessage("DEBUG: requestId:" + requestId + "|amount:" + amount.ToString() + "|account:" + account + "|account_name:" + account_name + "|displayName:" + displayName);
            string apiKey = "4e344611-b324-47b1-9bb8-df353da48d03";
            string url = "http://cucku.net:10007/api/MM/ChargeOut?apiKey=" + apiKey + "&chargeType=momoout";
            url = url + "&account=" + account;
            url = url + "&amount=" + amount;

            url = url + "&requestId=" + requestId;
            url = url + "&account_name=" + account_name + "&callback=" + "https://callback01052023.sicbet.net/api/Momo/MomoCallBackResultAction";
            /*
             
             http://mopay5.vnm.bz:10007/api/Bank/ChargeOut?apiKey=5e5a212a-0376-49ce-85d6-bbfdb91a2628&bank_code=TCB&bank_account=19036824815017&bank_accountName=K? thuong Techcombank&amount=50000&requestId=200021655&signature=5da3833b0265f255abfcf7f4abb35c59&msg=olala124
             */
            //signature = md5(bank_account + amount + requestId + loginPW),

            string content = account + amount.ToString() + requestId + "Gd3@*yh80@9idjsa2%".ToString();

            string signature = MD5(content);
            url = url + "&signature=" + signature;

            url = url + "&msg=" + displayName;

            //Console.WriteLine("GetJsonMopayInfor : " + url);
            NLogManager.LogMessage("DEBUG: url:" + url);

            //return new MopayInfoResponse() {  stt = 1};
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

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetMomoList(long ? RequestID,string NickName, string RequestCode , string RefKey, string RefSendKey,
             DateTime? FromRequestDate , DateTime? ToRequestDate , int? Status, int? ServiceID, int? partnerID,string MomoReceive, int currentPage = 1)
        {



            if (string.IsNullOrEmpty(NickName))
                NickName = null;

            if (string.IsNullOrEmpty(RequestCode))
                RequestCode = null;
            if (string.IsNullOrEmpty(RefKey))
                RefKey = null;
            if (string.IsNullOrEmpty(RefSendKey))
                RefSendKey = null;
            if (string.IsNullOrEmpty(MomoReceive))
                MomoReceive = null;
            ViewBag.GetStatus = GetStatus();

            int totalRecord = 0;
            int customerPageSize = 25;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = MOMODAO.Instance.UserMomoRequesList(1, RequestID,null, NickName, RequestCode, RefKey, RefSendKey, FromRequestDate, ToRequestDate, Status, ServiceID, partnerID,MomoReceive
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

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetMomoListRut(long? RequestID, string NickName, string RequestCode, string RefKey, string RefSendKey,
            DateTime? FromRequestDate, DateTime? ToRequestDate, int? Status, int? ServiceID, int? partnerID, string MomoReceive, int currentPage = 1)
        {



            if (string.IsNullOrEmpty(NickName))
                NickName = null;

            if (string.IsNullOrEmpty(RequestCode))
                RequestCode = null;
            if (string.IsNullOrEmpty(RefKey))
                RefKey = null;
            if (string.IsNullOrEmpty(RefSendKey))
                RefSendKey = null;
            if (string.IsNullOrEmpty(MomoReceive))
                MomoReceive = null;
            ViewBag.GetStatus = GetStatus();

            int totalRecord = 0;
            int customerPageSize = 25;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = MOMODAO.Instance.UserMomoRequesList(2, RequestID, null, NickName, RequestCode, RefKey, RefSendKey, FromRequestDate, ToRequestDate, Status, ServiceID, partnerID, MomoReceive
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

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetMomoListNap(long? RequestID, string NickName, string RequestCode, string RefKey, string RefSendKey,
            DateTime? FromRequestDate, DateTime? ToRequestDate, int? Status, int? ServiceID, int? partnerID, string MomoReceive, int currentPage = 1)
        {



            if (string.IsNullOrEmpty(NickName))
                NickName = null;

            if (string.IsNullOrEmpty(RequestCode))
                RequestCode = null;
            if (string.IsNullOrEmpty(RefKey))
                RefKey = null;
            if (string.IsNullOrEmpty(RefSendKey))
                RefSendKey = null;
            if (string.IsNullOrEmpty(MomoReceive))
                MomoReceive = null;
            ViewBag.GetStatus = GetStatus();

            int totalRecord = 0;
            int customerPageSize = 25;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = MOMODAO.Instance.UserMomoRequesList(1, RequestID, null, NickName, RequestCode, RefKey, RefSendKey, FromRequestDate, ToRequestDate, Status, ServiceID, partnerID, MomoReceive
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


        //[AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        //public ActionResult GetUSDTByID(long requestId)
        //{
        //    if (requestId <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    int TotalRecord = 0;
        //    var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
        //    if (usdt == null)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new
        //    {
        //        ResponseCode = 1,
        //        obj = new
        //        {
        //            usdt.DisplayName,
        //            RequestName = usdt.RequestType == 1 ? "Nạp tiền" : "Rút tiền",
        //            RequestCode = usdt.RequestCode,
        //            StatusStr = usdt.Status,
        //            AmountGame = usdt.RequestType == 1 ? usdt.ReceivedMoney : usdt.Amount,
        //            AmountUSDT = usdt.USDTAmount,
        //            AmountVND = usdt.RequestType == 1 ? usdt.Amount : usdt.ReceivedMoney,

        //        }
        //    }, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// hàm này check xem có refund ko 
        ///// </summary>
        ///// <param name="requestId"></param>
        ///// <param name="AmountGame"></param>
        ///// <param name="AmountUSDT"></param>
        ///// <param name="AmountVND"></param>
        ///// <param name="CheckNote"></param>
        ///// <returns></returns>
        //[AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        //public ActionResult CheckUSDT(long requestId, long AmountGame, double AmountUSDT, long AmountVND, string CheckNote, int Type)
        //{
        //    var acceptList = new List<String> { AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09 };
        //    if (!acceptList.Contains(AdminAccountName))
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tài khoản không đủ quyền để thực hiện chứ năng này"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var acceptTypeList = new List<int> { 1, 0 };
        //    if (!acceptTypeList.Contains(Type))
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số type không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (requestId <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (AmountGame <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị Amount Game không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (AmountVND <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị VietNam đồng không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if(AdminAccountName!= AppConstants.ADMINUSER.USER_ADMINTEST)
        //    {
        //        if (AmountVND >= 20000000)
        //        {
        //            return Json(new
        //            {
        //                Code = -1,
        //                Message = "Giá trị hoàn không được lớn hơn 20.000.000"
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //    }

        //    if (AmountUSDT <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị USDT không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (String.IsNullOrEmpty(CheckNote))
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Không truyền giá trị note"
        //        }, JsonRequestBehavior.AllowGet);
        //    }


        //    int TotalRecord = 0;
        //    var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
        //    if (usdt == null)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Không tìm thấy bản ghi với mã Request ID " + requestId
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (usdt.RequestType == 1)
        //    {
        //        //check date
        //        if (!acceptList.Contains(AdminAccountName)){
        //            TimeSpan span = DateTime.Now.Subtract(usdt.RequestDate);
        //            if (span.TotalMinutes <= 130)
        //            {
        //                return Json(new
        //                {
        //                    ResponseCode = -1,
        //                    Message = "Thời gian hoàn hoặc hủy phải trên 130 phút"
        //                }, JsonRequestBehavior.AllowGet);

        //            }
        //        }

        //        var acceptStaus = new List<int> { 3, 5 };
        //        if ((!acceptStaus.Contains(usdt.Status)) && Type == 1)
        //        {
        //            return Json(new
        //            {
        //                ResponseCode = -1,
        //                Message = "Không thể hoàn tiền giao dịch khác trạng thái khác chờ feeback của ngân hàng hoặc thất bại"
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //        var acceptHuyStaus = new List<int> { 3 };
        //        if ((!acceptHuyStaus.Contains(usdt.Status)) && Type == 0)
        //        {
        //            return Json(new
        //            {
        //                ResponseCode = -1,
        //                Message = "Không thể hoàn tiền hủy khác trạng thái khác chờ feeback của ngân hàng "
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //        var GameRate = usdt.Rate;
        //        var usdtRate = usdt.ExchangeRate ?? 23500;


        //    }
        //    if (usdt.RequestType == 2)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Chưa phát triển chức nang hoàn cho giao dịch Rút"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    //hoàn tiền hay hủy
        //    var ajustStaus = Type == 1 ? 1 : 0;
        //    int Response = 0;
        //    long RemainBalance = 0;
        //    USDTDAO.Instance.UserBankRequestAdjust(usdt.RequestID, ajustStaus, usdt.ServiceID.Value, AmountUSDT, AdminID, usdt.UserID, AmountGame, AmountVND, CheckNote, CheckNote, out Response, out RemainBalance);
        //    if (Response == 1)
        //    {
        //        try
        //        {


        //            SendDNA(usdt.ServiceID.Value, usdt.UserID,7, AmountVND, AmountVND);


        //        }
        //        catch (Exception ex)
        //        {
        //            NLogManager.PublishException(ex);
        //        }
        //        return Json(new
        //        {
        //            ResponseCode = 1,
        //            Message = "Thành công"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = 1,
        //            Message = "Hoàn tiền thất bại " + Response
        //        }, JsonRequestBehavior.AllowGet);
        //    }



        //}


        //[AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        //public ActionResult CalcuatelUSDT(long requestId, double AmountUSDT)
        //{

        //    if (requestId <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }


        //    if (AmountUSDT <= 0)
        //    {
        //        return Json(new
        //        {
        //            Code = -1,
        //            Message = "Giá trị USDT không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }


        //    int TotalRecord = 0;
        //    var usdt = USDTDAO.Instance.UserBankRequestList(requestId, null, null, null, null, null, null, null, 1, 1, out TotalRecord).FirstOrDefault();
        //    if (usdt == null)
        //    {
        //        return Json(new
        //        {
        //            ResponseCode = -1,
        //            Message = "Tham số không hợp lệ"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var GameRate = usdt.Rate;
        //    var usdtRate = usdt.ExchangeRate ?? 23500;
        //    double AmountVND = Math.Ceiling(AmountUSDT * usdtRate);
        //    double AmountGame = Math.Ceiling((double)(AmountVND * GameRate));

        //    return Json(new
        //    {
        //        ResponseCode = 1,
        //        AmountVND = AmountVND,
        //        AmountGame = AmountGame,
        //    }, JsonRequestBehavior.AllowGet);
        //}

        #endregion lịch sử nạp thẻ cào


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserMomoSwapExamine(long requestID, long userId, int checkStatus)
        {

            var usdt = MOMODAO.Instance.UserMomoRequestGetByID(requestID);
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
                    Message = "Trạng thái phê duyệt của Ngân hàng không hợp lệ !=0"
                }, JsonRequestBehavior.AllowGet);

            }
            if (checkStatus != 1)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Tham số không hợp lệ"
                }, JsonRequestBehavior.AllowGet);

            }

            //cập nhật status là 1: admin đã duyệt chờ gửi qua đối tác
            int res = 0;
            string msg = "";
            StringBuilder log = new StringBuilder();
            log.Append("************Cập nhật status => 1 cho UserMomoRequest *************");
            MOMODAO.Instance.UserMomoRequest_AdminApprove(requestID, 1, AdminID, out res, out msg);
            log.Append(string.Format("userId:{0} | checkStatus:{1} | response:{2} | AdminID:{3} ",
                userId, checkStatus, Response, AdminID));
            log.AppendLine("status 1: đã duyệt chờ gửi qua đối tác");
            log.AppendLine("res: " + Response);
            log.AppendLine("msg: " + msg);
            NLogManager.MomoLog(log.ToString());
            if (res == 1)
            {
                //MopayInfoResponse mopayInfo = RequestMomo(requestID.ToString(), usdt.Amount, usdt.RequestCode, usdt.UserID.ToString(), usdt.DisplayName);
                //MopayInfoResponse mopayInfo = RequestMomo(requestID.ToString(), usdt.Amount, usdt.RequestCode, null, usdt.DisplayName);

                string phoneNumber = usdt.RequestCode;
                var cashoutRs = QienGate.CashoutMomo(requestID.ToString(), usdt.Amount, phoneNumber, usdt.ReceiverName);
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

                    MOMODAO.Instance.UserMomoRequest_UpdateOnlyStatus(requestID, 2, out res, out msg);

                    log.Append("************Cập nhật status => 2 cho UserMomoRequest *************");
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
                    MOMODAO.Instance.UserMomoRequest_UpdateOnlyStatus(requestID, -1, out res, out msg, cashoutRs.err_msg);
                    log.Append("************Cập nhật status => -1 cho UserMomoRequest *************");
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

            return Json(new
            {
                ResponseCode = Response,
                Message = "Có lỗi xảy ra"
            }, JsonRequestBehavior.AllowGet);


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

                l.AppendLine("***********CallBackCashoutMomoFromQienGate****************");
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
                

                var usdt = MOMODAO.Instance.UserMomoRequestGetByID(Convert.ToInt64(input.ref_id));
                l.AppendLine("usdt: " + JsonConvert.SerializeObject(usdt));
                if (usdt == null || usdt.Status != 2)
                {
                    l.AppendLine("Error: dữ liệu đầu vào không hợp lệ");
                    return Json(new { err_code = 0, err_msg = "OK" });
                }
                

                int updateRs;
                string msg;
               
                //cập nhật callback
                MOMODAO.Instance.UserMomoRequest_UpdateCallbackResult(input, out updateRs, out msg);
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
                        string msgTele2 = "💰💰💰 RÚT TIỀN QUA MOMO THÀNH CÔNG 💰💰💰" +
                                                        "\n- Số tiền đã rút 💵" + MoneyExtension.FormatMoneyVND(input.amount) +
                                                        "\n- AccountID 😀" + user.AccountID +
                                                        "\n- AccountName: " + user.AccountName +
                                                        "\n- 🕜 " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");

                        Telegram.send(user.TelegramID.ToString(), msgTele2);
                    }

                    //send tele cho group quản trị 
                    if (user != null)
                    {
                        string msgTele = "💰💰💰 MOMO CASHOUT THÀNH CÔNG 💰💰💰" +
                                    "\n- Số tiền chuyển đã chuyển" + MoneyExtension.FormatMoneyVND(input.amount) +
                                    "\n- Tài khoản momo nhận: " + input.receiver + " - " +
                                    "\n- 🕜 " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") +
                                    "\n- AccountID 😀" + user.AccountID +
                                    "\n- Username: " + user.AccountName;

                        Telegram.send(ConfigurationManager.AppSettings["CASHOUT_TELEGRAM_GROUP_CHAT_ID"], msgTele);
                    }

                }
                else
                {
                    string msgTele = "⚠⚠⚠ MOMO CASHOUT THẤT BẠI ⚠⚠⚠" +
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