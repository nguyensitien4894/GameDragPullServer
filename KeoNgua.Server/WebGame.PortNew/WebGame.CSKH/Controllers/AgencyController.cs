using AutoMapper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Security;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.Agencies;
using MsWebGame.CSKH.Utils;
using Newtonsoft.Json;
using TraditionGame.Utilities;
using Telerik.Web.Mvc.UI;


namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class AgencyController : BaseController
    {
        #region danh sách dại lý 
        /// <summary>
        /// danh sách đại lý
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpGet, GridAction(EnableCustomBinding = true)]
        public ActionResult Index(GridCommand command, string AccountName, string PhoneOTP, short? AccountLevel, int? Status, int serviceId = 1)
        {
            var model = new ListAgencyModel();
            int totalRecord = 0;

            model.listAgencyOne = new List<SelectListItem>()
            {
                new SelectListItem() {Value="1",Text="Đại lý cấp 1" },
                 new SelectListItem() {Value="2",Text="Đại lý cấp 2" },
            };
            model.listStatus = new List<SelectListItem>()
            {
                new SelectListItem() {Value="-1",Text="Tất cả " },
                 new SelectListItem() {Value="1",Text="Hoạt động" },
                  new SelectListItem() {Value="2",Text="Bị khóa" },
                  new SelectListItem() {Value="0",Text=" Ngừng hoạt động" },
            };
            if (!Status.HasValue) Status = -1;
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetAgency(GridCommand command, string AccountName, string PhoneOTP, short? AccountLevel, int? Status, int serviceId = 1)
        {
            int totalrecord = 0;
            var lstRs = AgencyDAO.Instance.GetList(AccountName, PhoneOTP.PhoneFormat(), AccountLevel, Status, null, serviceId,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (lstRs == null)
                lstRs = new List<Agency>();

            var model = new GridModel<Agency> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }
        #endregion

        #region thêm mới đại lý
        /// <summary>
        /// GET :thêm mới đại lí
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create()
        {
            var model = new AgencyModel();
            ViewBag.AgencyOne = PrepareDropdownListAgencyOne();
            ViewBag.AgencyStatus = PrepareDropdownListAgencyStatus();
            ViewBag.ServiceBox = GetServices();
            model.ServiceID = 1;
            return View(model);
        }
        /// <summary>
        /// Post thêm mới đại lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create(AgencyModel model)
        {
            model.AccountName = model.AccountName.Trim();
            model.DisplayName = model.DisplayName.Trim();

            if (model.AccountName.Length < 4 || model.AccountName.Length > 20)
            {
                ModelState.AddModelError("AccountName", ErrorMsg.UserNameLength);
            }

            if (!ValidateInput.ValidateUserName(model.AccountName))
            {
                ModelState.AddModelError("AccountName", ErrorMsg.UsernameIncorrect);
            }

            //kiểm tra mật khẩu
            if (model.Password.Length < 6 || model.Password.Length > 20)
            {
                ModelState.AddModelError("Password", ErrorMsg.PasswordLength);
            }
            if (ValidateInput.IsContainSpace(model.Password))
            {
                ModelState.AddModelError("Password", ErrorMsg.UserNameContainSpace);
            }

            if (!ValidateInput.IsValidatePass(model.Password))
            {
                ModelState.AddModelError("Password", ErrorMsg.PasswordIncorrect);
            }

            if (model.DisplayName.Length < 4 || model.DisplayName.Length > 25)
            {
                ModelState.AddModelError("DisplayName", ErrorMsg.DisplayNameLength);
            }

            //kiểm tra số điện thoại 
            if (string.IsNullOrEmpty(model.PhoneOTP))
            {
                ModelState.AddModelError("PhoneNo", ErrorMsg.PhoneEmpty);
            }
            if (!ValidateInput.ValidatePhoneNumber(model.PhoneOTP))
            {
                ModelState.AddModelError("PhoneNo", ErrorMsg.PhoneIncorrect);
            }

            //kiểm tra user name đại lý có trùng hay không
            int isExist = 0;
            if (ModelState.IsValid)
            {
                //kiem tra ten dang nhap co trung ko
                AccountProfileDAO.Instance.CheckAccountCheckExist(1, model.AccountName, model.ServiceID, out isExist);
                if (isExist != 1)
                {
                    ModelState.AddModelError("AccountName", ErrorMsg.UserNameInUse);
                }

                //kiểm tra xem tên hiển thị có trùng ko
                AccountProfileDAO.Instance.CheckAccountCheckExist(2, model.DisplayName, model.ServiceID, out isExist);
                if (isExist != 1)
                {
                    ModelState.AddModelError("DisplayName", ErrorMsg.NickNameInUse);
                }

                //kiểm tra phone trên profile 
                AccountProfileDAO.Instance.CheckAccountCheckExist(3, model.PhoneOTP.PhoneFormat(), model.ServiceID, out isExist);
                if (isExist != 1)
                {
                    ModelState.AddModelError("PhoneNo", ErrorMsg.PhoneInUse);
                }
            }

            ViewBag.AgencyOne = PrepareDropdownListAgencyOne();
            ViewBag.AgencyStatus = PrepareDropdownListAgencyStatus();
            ViewBag.ServiceBox = GetServices();

            if (ModelState.IsValid)
            {
                long UserId = 0;
                UserDAO.Instance.UserGetSequence(out UserId);
                if (UserId <= 0)
                {
                    string msg = ErrorMsg.InProccessException;
                    ErrorNotification(msg);
                    return View(model);
                }

                //create agency
                int resAgency = AgencyDAO.Instance.AgencyCreate(model.AccountName, model.DisplayName, model.FullName, model.ParentID.HasValue ? 2 : 1, model.ParentID ?? 0,
                model.Status.Value, model.AreaName, model.PhoneOTP.PhoneFormat(), model.PhoneDisplay, model.FBLink, Security.SHA256Encrypt(model.Password),
                UserId, model.OrderNum, model.ServiceID);
                //tạo agency thành công
                if (resAgency == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = MessageConvetor.MsgAgencyCreate.GetMessage(resAgency);
                    ErrorNotification(msg);
                }
            }

            return View(model);
        }
        #endregion

        #region cập nhật đại lý
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(long Id, int serviceId)
        {
            if (Id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);
           
            var entity = AgencyDAO.Instance.AdminGetById(Id, serviceId);
            if (entity == null)
                throw new ArgumentException(Message.ParamaterInvalid);
           
            var model = new AgencyModel();
            //sử dụng autor mapper để mapp object (nhanh hơn )
            model = Mapper.Map<AgencyModel>(entity);
          
            model.PhoneOTP = entity.PhoneOTP.PhoneDisplayFormat();
            ViewBag.AgencyOne = PrepareDropdownListAgencyOne();
            ViewBag.AgencyStatus = PrepareDropdownListAgencyStatus();
            return View(model);
        }
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit([Bind(Exclude = "AccountName")]AgencyModel model)
        {
            ModelState.Remove("AccountName");
            ModelState.Remove("DisplayName");
            ModelState.Remove("Password");
            ModelState.Remove("AccountCode");

            //kiểm tra số điện thoại 
            if (string.IsNullOrEmpty(model.PhoneOTP))
            {
                ModelState.AddModelError("PhoneNo", ErrorMsg.PhoneEmpty);
            }
            if (!ValidateInput.ValidatePhoneNumber(model.PhoneOTP))
            {
                ModelState.AddModelError("PhoneNo", ErrorMsg.PhoneIncorrect);
            }

            //kiểm tra số  điện thoại trên profile
            int IsUsed = AccountProfileDAO.Instance.CheckAccountPhone(model.AccountId, model.PhoneOTP.PhoneFormat(), model.ServiceID);
            if (IsUsed != 1)
            {
                ModelState.AddModelError("PhoneNo", ErrorMsg.PhoneInUse);
            }

            if (ModelState.IsValid)
            {
                var entityAgency = AgencyDAO.Instance.AdminGetById(model.AccountId, model.ServiceID);
                bool isTong = false;
                int lvl = 0;
                if (entityAgency != null)
                {
                    isTong = entityAgency.AccountLevel == 0;
                }
                if (isTong)
                {
                    lvl = -1;
                }
                else
                {
                    lvl = model.ParentID.HasValue ? 2 : 1;
                }
                int resUpdate = AgencyDAO.Instance.AgencyUpdate(model.AccountId, model.FullName,lvl , model.ParentID ?? 0, model.Status,
                    model.AreaName, model.PhoneOTP.PhoneFormat(), model.PhoneDisplay, model.FBLink, model.OrderNum,model.TelegramLink,model.ZaloLink);
                int resPro = 0;
                AccountProfileDAO.Instance.UpdateProfile(model.AccountId, null, null, model.PhoneOTP.PhoneFormat(), -1, -1, null, null, out resPro);
                if (resUpdate == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = MessageConvetor.MsgAgencyEdit.GetMessage(resUpdate);
                    ErrorNotification(msg);
                }
            }
            else
            {
                ErrorNotification(ErrorMsg.PhoneInUse.ToString());
            }

            ViewBag.AgencyOne = PrepareDropdownListAgencyOne();
            ViewBag.AgencyStatus = PrepareDropdownListAgencyStatus();

            return View(model);
        }
        #endregion



        #region xóa đại lý
        /// <summary>
        /// xóa bản ghi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpGet]
        public ActionResult Delete(long Id)
        {
            if (Id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);

            if (ModelState.IsValid)
            {
                int outResponse;

                AgencyDAO.Instance.Delete(Id, out outResponse);
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.DeleteSuccess, true);
                }
                else if (outResponse == -6)
                {
                    SuccessNotification(Message.AgencyHasTransaction, true);
                }
                else
                {
                    ErrorNotification(Message.DeleteFail);
                }
            }

            return RedirectToAction("Index");
        }
        #endregion
        #region import đại lý cấp 2
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ImportC2()
        {
            var model = new ListImportC2Model();
            model.listAgencyOne = PrepareDropdownListAgencyOne();
            return View(model);
        }
        /// <summary>
        /// Import excel 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ParrentID"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ImportC2(HttpPostedFileBase file, long? ParrentID)
        {
            ListImportC2Model model = new ListImportC2Model();
            model.listAgencyOne = PrepareDropdownListAgencyOne();
            if (!ParrentID.HasValue || ParrentID <= 0)
            {
                ErrorNotification("Chưa chọn đại lý cha");
                return View(model);
            }
            if (file == null || file.ContentLength <= 0)
            {
                ErrorNotification("Chưa nhập danh sách đại lý");
                return View(model);
            }

            if ((file != null) && (file.ContentLength > 0 && !string.IsNullOrEmpty(file.FileName)))
            {
                string extension = Path.GetExtension(file.FileName);
                if (file == null || file.ContentLength <= 0)
                {
                    ErrorNotification("Định dạng file chấp nhận là .xlsx");
                    return RedirectToAction("Index");
                }

                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                List<TmpAgencyC2> listAgencyModels = new List<TmpAgencyC2>();
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();

                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    for (int i = 2; i < noOfRow + 1; i++)
                    {
                        TmpAgencyC2 agency = new TmpAgencyC2();
                        agency.ID = Int32.Parse(workSheet.Cells[i, 1].Value.ToString());
                        agency.AccountName = workSheet.Cells[i, 2].Value.ToString().Trim();
                        agency.DisplayName = workSheet.Cells[i, 3].Value.ToString().Trim();
                        agency.FullName = workSheet.Cells[i, 4].Value.ToString();
                        agency.AccountLevel = 2;
                        agency.ParentID = ParrentID.Value;
                        agency.Status = 1;
                        agency.AreaName = workSheet.Cells[i, 7].Value.ToString();
                        agency.PhoneNo = workSheet.Cells[i, 5].Value.ToString();
                        agency.PhoneDisplay = workSheet.Cells[i, 6].Value.ToString();
                        agency.FBLink = workSheet.Cells[i, 8].Value.ToString();
                        agency.Password = Security.SHA256Encrypt(workSheet.Cells[i, 9].Value.ToString().Trim());

                        listAgencyModels.Add(agency);
                    }
                }

                List<string> listErrorID = new List<string>();
                DataTable dt = DatatableConvertor.ToDataTable(listAgencyModels);
                int Response = 0;
                var listResult = AgencyDAO.Instance.ImportAgencyC2(dt, out Response);
                model.listSuccess = listResult.Where(c => c.Status == 1).ToList();
                model.ListError = listResult.Where(c => c.Status != 1).ToList();
            }

            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CashFlow()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCashFlow(GridCommand command, string nickName, DateTime fromDate, DateTime toDate, int serviceId)
        {
            List<CashFlow> lstRs = new List<CashFlow>();
            if (string.IsNullOrEmpty(nickName))
            {
                var model = new GridModel<CashFlow> { Data = lstRs, Total = 0 };
                return new JsonResult { Data = model };
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(nickName, serviceId);
            if (accInfo == null)
            {
                var model = new GridModel<CashFlow> { Data = lstRs, Total = 0 };
                return new JsonResult { Data = model };
            }

            toDate = toDate.AddDays(1).AddMilliseconds(-1);
            long accountId = accInfo.AccountID;
            var rs = AgencyDAO.Instance.GetAgencyCashFlow(accountId, 2, fromDate, toDate, serviceId);
            if (rs == null)
            {
                var model = new GridModel<CashFlow> { Data = lstRs, Total = 0 };
                return new JsonResult { Data = model };
            }

            lstRs.Add(rs);
            var gridModel = new GridModel<CashFlow> { Data = lstRs, Total = 1 };
            return new JsonResult { Data = gridModel };
        }

        #endregion
        #region báo cáo doanh thu đại lý cấp 1
        [HttpGet]
        public ActionResult AgencyRevenue(ListAgencyRevenueModel model)
        {
            //chuẩn bị dropdown list cho đại lý cha
            model.listAgencyOne = PrepareDropdownListAgencyOne();
            //lấy báo cao theo ngày tháng, parrent và cấp độ
            var listReport = AgencyDAO.Instance.GetAgencyRevenue(model.ParrentID ?? 0, model.Level ?? 0, model.StartDate, model.EndDate);
            model.listReport = listReport;
            return View(model);
        }
        #endregion
        #region thu hồi tiền
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult AgencyRefund(int ServiceID)
        {


            if (ServiceID <= 0)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Mã service phải >0"
                });
            }
            //chuẩn bị dropdown list cho đại lý cha
            int outResponse = 0;
            AgencyDAO.Instance.AgencyRefund(ServiceID, out outResponse);
            if (outResponse == 1)
            {
                return Json(new
                {
                    ResponseCode = 1,
                    Message = "Thu hồi thành công"
                });
            }
            else
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Thu hồi  thất bại|" + outResponse
                });
            }

        }
        #endregion

        #region thuhoitiencuatungdaily
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult AgencyRefundEachOther(long AgencyID,string Amount,int Type,string Note,int ServiceId)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            {
                return RedirectToAction("Permission", "Account");
            }


            if (AgencyID <= 0)
            {
                return Json(new { ResponseCode = -1005, Message = "Không tồn tại đại lý " },JsonRequestBehavior.AllowGet);
            }
            var acceptTypes = new List<int> { 1, 3 };
            if (!acceptTypes.Contains(Type))
            {
                return Json(new { ResponseCode = -1005, Message = "Ví đại lý không hợp lệ " }, JsonRequestBehavior.AllowGet);
            }
            if (String.IsNullOrEmpty(Amount))
            {
                return Json(new { ResponseCode = -1005, Message = "Số tiền thu hồi phải nhập " }, JsonRequestBehavior.AllowGet);
            }
            

            long iamount = Int64.Parse(Amount.Replace(".", ""));
            if (iamount <=0)
            {
                return Json(new { ResponseCode = -1005, Message = "Số tiền thu hồi phải nhập " }, JsonRequestBehavior.AllowGet);
            }
            var entityAgency = AgencyDAO.Instance.AdminGetById(AgencyID, ServiceId);
            if (entityAgency == null)
            {
                return Json(new { ResponseCode = -1005, Message = "Không tồn tại đại lý " }, JsonRequestBehavior.AllowGet);
            }
            //so sánh ví chính
            if (Type == 1)
            {
                if (entityAgency.Wallet < iamount)
                {
                    return Json(new { ResponseCode = -1005, Message = "Số tiền ví chính không đủ để thu hồi" }, JsonRequestBehavior.AllowGet);
                }

            }
            if (Type == 3)
            {
                if (entityAgency.GiftcodeWallet < iamount)
                {
                    return Json(new { ResponseCode = -1005, Message = "Số tiền ví gift code không đủ để thu hồi" }, JsonRequestBehavior.AllowGet);
                }

            }

            long TransId = 0;
            long Wallet = 0;
            int Response = 0;

            AgencyDAO.Instance.AdminWithdrawAgencyWallet(AdminID,AgencyID, ServiceId, Type,iamount,Note,out TransId,out Wallet,out Response);
            if (Response == 1)
            {
                return Json(new { ResponseCode = 1, Message = "Thu hồi tiền đại lý thành công" }, JsonRequestBehavior.AllowGet);
            }else
            {
                return Json(new { ResponseCode = -1, Message = "Thu hồi tiền đại lý thất bại |"+ Response }, JsonRequestBehavior.AllowGet);
            }

           
        }

        #endregion

        #region helper
        private List<SelectListItem> PrepareDropdownListAgencyOne()
        {
            int totalRecord = 0;
            var tmpList = AgencyDAO.Instance.GetList(null, null, null, 1, null, 1, 1, Int16.MaxValue, out totalRecord);
            var tmp = tmpList.Select(c => new SelectListItem
            {
                Value = c.AccountId.ToString(),
                Text = c.DisplayName,
            }).ToList();
            return tmp;
        }

        private List<SelectListItem> PrepareDropdownListAgencyStatus()
        {
            var tmp = new List<SelectListItem>()
            {
                new SelectListItem() {Value="1",Text="Hoạt động" },
                new SelectListItem() {Value="0",Text="Ngừng hoạt động" },
                 new SelectListItem() {Value="2",Text="Bị khóa" },
            };

            return tmp;
        }
        #endregion

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult RaceTop(string raceDate = null, DateTime? fromDate = null, DateTime? toDate = null, int serviceId = 1)
        {
            DateTime? race = null;
            if (!string.IsNullOrEmpty(raceDate))
                race = DateTime.ParseExact(raceDate, "dd/MM/yyyy", null);

            if (fromDate == null)
                fromDate = DateTime.Today.AddDays(-7);

            if (toDate == null)
                toDate = DateTime.Today;
            toDate = toDate.Value.AddDays(1).AddMinutes(-1);
            bool isClosed = false;
            if (race != null)
                isClosed = true;

            var lstRd = AgencyDAO.Instance.GetAgencyRaceTopListRaceDate(serviceId);
            var lstDropDown = lstRd.Select(c => new DropDownItem()
            {
                Value = c.RaceDate.ToString("dd/MM/yyyy"),
                Text = string.Format("Ngày: {0}", c.RaceDate.ToString("dd/MM/yyyy"))
            }).ToList();
            var first = new DropDownItem() { Value = "", Text = "Chọn kỳ chốt" };
            lstDropDown.Insert(0, first);

            ViewBag.RaceDateBox = lstDropDown;
            ViewBag.ServiceBox = GetServices();
            ViewBag.IsClosed = isClosed;
            List<AgencyRaceTop> lstRs = AgencyDAO.Instance.GetListAgencyRaceTop(race, fromDate, toDate, serviceId, isClosed);
            return View(lstRs);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Transaction()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTransaction(GridCommand command, TransactionModel input)
        {
            List<AgencyTransaction> lstRs = new List<AgencyTransaction>();
            string nickName = input.NickName;
            //if (string.IsNullOrEmpty(nickName))
            //{
            //    var model = new GridModel<AgencyTransaction> { Data = lstRs, Total = 1 };
            //    return new JsonResult { Data = model };
            //}

            var aginfo = UserDAO.Instance.GetAccountByNickName(nickName, input.ServiceID);
            //if (aginfo == null)
            //{
            //    var model = new GridModel<AgencyTransaction> { Data = lstRs, Total = 1 };
            //    return new JsonResult { Data = model };
            //}

            string partnername = input.PartnerName;
            if (string.IsNullOrEmpty(input.PartnerName))
                input.PartnerName = null;

            DateTime to = input.ToDate.Value.AddDays(1).AddMilliseconds(-1);
            long totalRecord;
            lstRs = AgencyDAO.Instance.GetAgencyTransaction(aginfo != null ? aginfo.AccountID : 0, input.UserType, input.PartnerName, input.Type, input.Status, input.FromDate, to, input.ServiceID, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<AgencyTransaction> { Data = lstRs, Total = Convert.ToInt32(totalRecord) };
            return new JsonResult { Data = gridModel};
        }


        #region thưởng đại lý cuối kì 
        private List<string> acceptList = new List<string>() { AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_ADMIN };
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult RaceTopAward(string raceDate = null, int serviceId = 1)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
           
            {
                return RedirectToAction("Permission", "Account");

            }
         
            DateTime? race = null;
            if (!string.IsNullOrEmpty(raceDate))
                race = DateTime.ParseExact(raceDate, "dd/MM/yyyy", null);


            bool isClosed = false;
            if (race != null)
                isClosed = true;

            var lstRd = AgencyDAO.Instance.GetAgencyRaceTopListRaceDate(serviceId).OrderByDescending(c => c.RaceDate);
            var lstDropDown = lstRd.Select(c => new DropDownItem()
            {
                Value = c.RaceDate.ToString(),
                Text = string.Format("Ngày: {0}", c.RaceDate.ToString("dd/MM/yyyy"))
            }).ToList();
            var first = new DropDownItem() { Value = "", Text = "Chọn kỳ chốt" };
            lstDropDown.Insert(0, first);

            ViewBag.RaceDateBox = lstDropDown;
            ViewBag.ServiceBox = GetServices();
            ViewBag.IsClosed = isClosed;

            return View();
        }
        [HttpPost]
        public ActionResult GetAgencyRaceTopListRaceDate(int ServiceID) {
            var lstRd = AgencyDAO.Instance.GetAgencyRaceTopListRaceDate(ServiceID).OrderByDescending(c=>c.RaceDate);
            var lstDropDown = lstRd.Select(c => new DropDownItem()
            {
                Value = c.RaceDate.ToString(),
                Text = string.Format("Ngày: {0}", c.RaceDate.ToString("dd/MM/yyyy"))
            }).ToList();
            return Json(new { data = lstDropDown }, JsonRequestBehavior.AllowGet);

        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetPrepareRaceTop(GridCommand command, DateTime? raceDate = null, DateTime? fromDate = null, DateTime? toDate = null, int serviceId = 1)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
          
            {
                return RedirectToAction("Permission", "Account");

            }
           
            DateTime? race = null;


            if (fromDate == null)
                fromDate = DateTime.Today.AddDays(-7);

            if (toDate == null)
                toDate = DateTime.Today;
            toDate = toDate.Value.AddDays(1).AddMinutes(-1);
            bool isClosed = false;
           


            List<AgencyRaceTop> lstRs = AgencyDAO.Instance.GetListAgencyRaceTop(null, fromDate, toDate, serviceId, isClosed);


            List<AgencyRaceTopAward> lstTemp = AgencyDAO.Instance.GetPrepareListAgencyRaceTop(raceDate, serviceId, null);
            foreach (var item in lstRs)
            {
                if (lstTemp != null && lstTemp.Any())
                {
                    var matchItem = lstTemp.FirstOrDefault(c => c.AccountID == item.AccountID && c.PrizeID == item.PrizeID);
                    if (matchItem != null)
                    {
                        item.PrizeValue = matchItem.PrizeValue;
                        item.RaceDate = matchItem.RaceDate;
                        item.IsClosed = matchItem.IsClosed;
                    }
                }
                
                item.ServiceID = serviceId;
            }

            var gridModel = new GridModel<AgencyRaceTop> { Data = lstRs, Total = lstRs.Count() };
            return new JsonResult { Data = gridModel };
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateRaceTopAward(AgencyRaceTop model, GridCommand command)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
           
            {
                return RedirectToAction("Permission", "Account");

            }
            
            if (model.AccountID <= 0)
            {
                return Content(String.Format("Account ID không thể trống"));
            }
            long AccountID = model.AccountID;
            int ServiceID = model.ServiceID;

            if (ServiceID <= 0)
            {
                return Content(String.Format("ServiceID không thể trống"));
            }
            var agencyInfo = AgencyDAO.Instance.AdminGetById(AccountID, ServiceID);
            if (ServiceID <= 0)
            {
                return Content(String.Format("Không thể tìm thấy tông tin đại lý"));
            }
            if (model.PrizeValue <= 0)
            {
                return Content(String.Format("Chưa cập nhật tiền thưởng"));
            }
            if (model.RaceDate==null)
            {
                return Content(String.Format("Chưa chọn ngày chốt"));
            }

            int AccountLevel = (int)agencyInfo.AccountLevel;


            string DisplayName = model.DisplayName;
            long ParentID = ConvertUtil.ToLong(agencyInfo.ParentID);

            string ParentName = agencyInfo.ParrentDisplayName ?? "TDL";
            int PrizeID = ConvertUtil.ToInt(model.PrizeID);
            var AccountName = agencyInfo.AccountName;
            DateTime RaceDate = model.RaceDate.Value;
            DateTime CreateDate = DateTime.Now;
            bool IsClosed = false;
            long TotalAmount = model.TotalTransfer;
            decimal TotalVP = model.TotalVP;
             string  FullName = agencyInfo.FullName;
            long PrizeValue = model.PrizeValue;
            int Response = 0;


            AgencyDAO.Instance.AgencyRaceTopAwardCreate(AccountID, AccountLevel, AccountName, DisplayName, FullName, ParentID, ParentName, PrizeID, RaceDate, CreateDate, IsClosed, TotalAmount, TotalVP, PrizeValue, ServiceID, out Response);
            if (Response == 1)
            {

                return Content(String.Format("Chốt tạm thời thành công"));
            }
            else
            {

            }
            return Content(String.Format("Chốt tạm thời thất bại"));



        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateWeekClose(DateTime raceDate, int serviceId = 1)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            
            {
                return RedirectToAction("Permission", "Account");

            }
           

            int Response = 0;


            AgencyDAO.Instance.AgencyRaceTopWeekClose(serviceId, raceDate, out Response);
            if (Response == 1)
            {
                return Json(new
                {
                    ResponseCode = 1,
                    Message = "Cập nhật thành công"
                });
            }
            else
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Chốt danh sách   thất bại|" + Response
                });
            }




        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetPrepare2RaceTop(GridCommand command, DateTime? raceDate = null, DateTime? fromDate = null, DateTime? toDate = null, int serviceId = 1)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
          
            {
                return RedirectToAction("Permission", "Account");

            }
          
            bool isClosed = false;
            List<AgencyRaceTopAward> lstRs = AgencyDAO.Instance.GetAgencyRaceTopCloseList(raceDate, serviceId, isClosed);
            var gridModel = new GridModel<AgencyRaceTopAward> { Data = lstRs, Total = lstRs.Count() };
            return new JsonResult { Data = gridModel };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetResultRaceTop(GridCommand command, DateTime? raceDate = null, DateTime? fromDate = null, DateTime? toDate = null, int serviceId = 1)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            
            {
                return RedirectToAction("Permission", "Account");

            }
           

            bool isClosed = true;
            List<AgencyRaceTopAward> lstRs = AgencyDAO.Instance.GetAgencyRaceTopCloseList(raceDate, serviceId, isClosed);
            var gridModel = new GridModel<AgencyRaceTopAward> { Data = lstRs, Total = lstRs.Count() };
            return new JsonResult { Data = gridModel };
        }
        private List<string> _acceptListAdminTransactionFee = new List<string>() { "admin", "adminref", "admin_test", "cskh_01", "monitor_01" };
        [HttpGet]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult ResetPasswordAgency()
        {

            if (!_acceptListAdminTransactionFee.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }
            

            return View();
        }
        
        [HttpPost]
        public ActionResult ResetPasswordAgency(string Username)
        {
            try
            {
                if (!_acceptListAdminTransactionFee.Contains(AdminAccountName))
                {
                    return RedirectToAction("Permission", "Account");
                }

                if (String.IsNullOrEmpty(Username))
                {
                    ViewBag.Message = "Không để trống tên hiển thị ";
                    return View();
                }
                string newPass = Security.SHA256Encrypt("123456a");
                int response = AgencyDAO.Instance.ChangePasswordCore(Username, newPass, base.ServiceID);
                NLogManager.LogMessage(string.Format("UserName:{0} | Response:{1}", Username, response));
                if (response == 1)
                {
                    ViewBag.Message = "Cập nhật thành công";
                    return View();
                }
                else
                {
                    ViewBag.Message = "Nickname đại lý không hợp lệ";
                    return View();
                }
                
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            ViewBag.Message = ErrorMsg.InProccessException;
            return View();
        }

        #endregion
    }
}