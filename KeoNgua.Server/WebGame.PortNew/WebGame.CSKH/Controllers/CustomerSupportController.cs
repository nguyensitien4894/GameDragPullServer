using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Security;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.LuckyDice;
using MsWebGame.CSKH.Models.Mails;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Utils;
using Telerik.Web.Mvc.UI;
using TraditionGame.Utilities.OneSignal;
using TraditionGame.Utilities.Utils;
using MsWebGame.RedisCache.Cache;

namespace MsWebGame.CSKH.Controllers
{
    /// <summary>
    /// quản lý chăm sóc khách hàng
    /// </summary>

    public class CustomerSupportController : BaseController
    {
        private List<string> accpetList = new List<string> { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01 };
        // GET: CustomerSupport
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            return View();
        }

       

        /// <summary>
        /// search hiển thị danh sách chăm sóc khách hàng
        /// </summary>
        /// <param name="command"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command, ListAdminModel model)
        {
            //lay danh sách chăm sóc khách hàng
            var list = AdminDAO.Instance.GetList(2, model.UserName, model.PhoneContact.PhoneFormat(), ServiceID);
            var gridModel = new GridModel<AdminModel>
            {
                Data = list.Select(x =>
                {
                    var m = new AdminModel();
                    m.CreatedAt = x.CreatedAt;
                    m.AdminID = x.AccountID;
                    m.UserName = x.UserName;
                    m.PhoneContact = x.PhoneContact.PhoneDisplayFormat();
                    return m;
                }),
                Total = list.Count()
            };
            return new JsonResult { Data = gridModel };
        }
        /// <summary>
        /// thêm mới chăm sóc khách hàng
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Create()
        {
            AdminModel model = new AdminModel();
            return View(model);
        }
        /// <summary>
        /// thêm mới chăm sóc khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create(AdminModel model)
        {
            if (String.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", Message.PasswordRequired);
                return View(model);
            }
            if (ModelState.IsValid)
            {
                int outResponse;
                //roleId =2 là chăm sóc khách hàng
                AdminDAO.Instance.Insert(model.UserName, model.PhoneContact.PhoneFormat(), model.Email, 0, "2", AdminID, Security.SHA256Encrypt(model.Password), 0, out outResponse);
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.InsertSuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string message = MessageConvetor.MsgCustomerSupportCreate.GetMessage(outResponse);
                    ErrorNotification(message);
                }
            }
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(long Id)
        {
            if (Id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);
            var entity = AdminDAO.Instance.GetById(Id);
            if (entity == null)
            {
                throw new ArgumentException(Message.ParamaterInvalid);
            }
            var model = new AdminModel();
            model.AdminID = Id;

            model.UserName = entity.UserName;
            model.PhoneContact = entity.PhoneContact.PhoneDisplayFormat();
            model.Password = entity.Password;
            model.Email = entity.Email;
            model.Level = entity.Level ?? 0;
            return View(model);
        }
        /// <summary>
        /// cập nhật chăm sóc khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult Edit(AdminModel model)
        {
            if (model == null)
                throw new ArgumentException(Message.ParamaterInvalid);
            var entity = AdminDAO.Instance.GetById(model.AdminID);
            if (entity == null)
            {
                throw new ArgumentException(Message.ParamaterInvalid);
            }

            if (String.IsNullOrEmpty(model.Password))
                ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                int outResponse;
                //roleId =2 là chăm sóc khách hàng
                AdminDAO.Instance.Update(entity.UserName, String.IsNullOrEmpty(model.Password) ? entity.Password : Security.SHA256Encrypt(model.Password), 2,
                    model.PhoneContact.PhoneFormat(), model.Email, model.Level, 0, 1, out outResponse);
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else if (outResponse == -1)
                {
                    ErrorNotification(Message.CSKHExist);
                    return View(model);
                }
                else
                {
                    ErrorNotification(Message.SystemProccessing);
                    return View(model);
                }
            }

            return View(model);
        }

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
            var entity = AdminDAO.Instance.GetById(Id);
            if (entity == null)
            {
                throw new ArgumentException(Message.ParamaterInvalid);
            }

            if (ModelState.IsValid)
            {
                int outResponse;
                AdminDAO.Instance.Delete(entity.UserName, out outResponse);
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.DeleteSuccess);
                }
                else
                {
                    ErrorNotification(Message.DeleteFail);
                }
            }

            return RedirectToAction("Index");
        }

        #region CSKH REBBLACK
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult UserProfile()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetUserProfile(GridCommand command, int searchType, string value)
        {
            if (searchType < 1 || string.IsNullOrEmpty(value))
            {
                var rs = new GridModel<UserProfile> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            var list = CustomerSupportDAO.Instance.GetListUserProfile(searchType, value);
            if (list == null)
                list = new List<UserProfile>();

            var model = new GridModel<UserProfile> { Data = list.PagedForCommand(command), Total = list.Count };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Vip()
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetVip(GridCommand command, int searchType, string value, int rankId, DateTime rankedMonth, int serviceId)
        {
            if (searchType < 0 || string.IsNullOrEmpty(value) || (rankId < 0 || rankId > 5))
            {
                var rs = new GridModel<UserPrivilege> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            var list = CustomerSupportDAO.Instance.GetListUserPrivilege(searchType, value, rankId, rankedMonth, serviceId);
            if (list == null)
                list = new List<UserPrivilege>();

            var model = new GridModel<UserPrivilege> { Data = list.PagedForCommand(command), Total = list.Count };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult HistoryChangeVip(string nickName = null)
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.NickName = nickName;
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryChangeVip(GridCommand command, string nickName, int serviceId)
        {
            if (string.IsNullOrEmpty(nickName))
            {
                var rs = new GridModel<UserPrivilegeHistory> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(nickName, serviceId);
            if (accInfo == null || accInfo.AccountID < 1 || accInfo.AccountType != 1)
            {
                var rs = new GridModel<UserPrivilegeHistory> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            var list = CustomerSupportDAO.Instance.GetUserPrivilegeHistory(accInfo.AccountID);
            if (list == null)
                list = new List<UserPrivilegeHistory>();

            var model = new GridModel<UserPrivilegeHistory> { Data = list.PagedForCommand(command), Total = list.Count };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult HistoryVipRedemption()
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryVipRedemption(GridCommand command, string userName, int rankId, DateTime createDate, int serviceId)
        {
            int totalrecord = 0;
            var list = ManagerDAO.Instance.GetListUserRedemption(0, 0, userName, rankId, createDate, serviceId,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<UserRedemption>();

            var model = new GridModel<UserRedemption> { Data = list, Total = totalrecord };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult HistoryPlay()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.ServiceBox = GetServices();
            ViewBag.Orders = InfoHandler.Instance.GetOrderHistoryPlay();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryPlay(GridCommand command, ParCustomerSupport data)
        {
            string nickName = data.NickName;
            int gameId = data.GameID;
            long? spinId = data.SpinID;
            DateTime fromDate = data.FromDate;
            DateTime toDate = data.ToDate;
            int serviceId = data.ServiceID;
            int orderby = data.OrderBy;
            if (string.IsNullOrEmpty(nickName))
            {
                var rs = new GridModel<HistoryPlay> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(nickName, serviceId);
            if (accInfo == null || accInfo.AccountID < 1 || accInfo.AccountType != 1)
            {
                var rs = new GridModel<HistoryPlay> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            if (gameId == 0)
                gameId = -1;

            int totalrecord = 0;
            var lstRs = CustomerSupportDAO.Instance.GetHistoryPlay(accInfo.AccountID, gameId, spinId, fromDate, toDate, orderby, command.Page - 1 <= 0 ? 1 : command.Page,
                AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (lstRs == null)
                lstRs = new List<HistoryPlay>();
            var model = new GridModel<HistoryPlay> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult HistoryPlayTaiXiu()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.ServiceBox = GetServices();
            ViewBag.Orders = InfoHandler.Instance.GetOrderHistoryPlay();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryPlayTaiXiu(GridCommand command, ParCustomerSupport data)
        {
            long? spinId = data.SpinID;
            if (spinId ==null)
            {
                var rs = new GridModel<HistoryPlay> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            int page = command.Page - 1 <= 0 ? 0 : command.Page - 1;
            int totalrecord = 0;
            var lstRs = CustomerSupportDAO.Instance.GetHistoryPlay((long)spinId);
            if (lstRs == null)
                lstRs = new List<HistoryPlay>();
            totalrecord = lstRs.Count;
            lstRs = lstRs.Skip(page*20).Take(20).ToList();
            var model = new GridModel<HistoryPlay> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult CheckCloneByAccount()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.ServiceBox = GetServices();
            ViewBag.Orders = InfoHandler.Instance.GetOrderHistoryPlay();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCheckCloneByAccount(GridCommand command, ParCustomerSupport data)
        {
            string nickName = data.NickName;
            if (string.IsNullOrEmpty(nickName))
            {
                var rs = new GridModel<HistoryPlay> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            int page = command.Page - 1 <= 0 ? 0 : command.Page - 1;
            int totalrecord = 0;
            var lstRs = CustomerSupportDAO.Instance.GetAccountClone(nickName);
            if (lstRs == null)
                lstRs = new List<AccountIP>();
            totalrecord = lstRs.Count;
            lstRs = lstRs.Skip(page * 20).Take(20).ToList();
            var model = new GridModel<AccountIP> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }


        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult HistoryWalletLog()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryWalletLog(GridCommand command, ParCustomerSupport pars)
        {
            string nickName = pars.NickName;
            string partnerName = pars.PartnerName;
            int transType = pars.TransType;
            DateTime from = pars.FromDate;
            DateTime to = pars.ToDate;
            int serviceId = pars.ServiceID;
            if (string.IsNullOrEmpty(nickName))
            {
                var rs = new GridModel<HistoryWalletLog> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(nickName, serviceId);
            if (accInfo == null || accInfo.AccountID < 1)
            {
                var rs = new GridModel<HistoryWalletLog> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            int totalrecord = 0;
            var list = CustomerSupportDAO.Instance.GetListWalletLog(accInfo.AccountID, partnerName, transType, from, to, serviceId,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<HistoryWalletLog>();

            var model = new GridModel<HistoryWalletLog> { Data = list, Total = totalrecord };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult HistoryThankful()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        //[HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult getHistoryThankfulAccountList(DateTime? fromDate = null, DateTime? toDate = null, int currentPage = 1, int gamemode = 0)
        {
            //DateTime from = fromDate;
            //DateTime to = toDate;
            if (fromDate == null)
                fromDate = DateTime.Today.AddDays(-7);

            if (toDate == null)
                toDate = DateTime.Today;

            int totalRecord = 0;
            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var list = CustomerSupportDAO.Instance.GetHistoryThankfulAccountList(fromDate, toDate, currentPage, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            if (list == null)
                list = new List<HistoryThankfulAccountList>();

            var pager = new Pager(totalRecord, (currentPage), AppConstants.CONFIG.GRID_SIZE, 10);
            //int totalPage = totalRecord / AppConstants.CONFIG.GRID_SIZE + 1;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.GameMode = gamemode;

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

            //var model = new GridModel<HistoryThankfulAccountList> { Data = list.PagedForCommand(command), Total = totalRecord };
            //return new JsonResult { Data = model };
            return PartialView(list);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryThankful(GridCommand command, ParCustomerSupportforThankful pars)
        {
            string nickName = pars.NickName;
            string AccountID = pars.AccountID;
            int gameId = pars.GameID;
            int gameMode = pars.GameMode;
            DateTime from = pars.FromDate;
            DateTime to = pars.ToDate;
            int serviceId = pars.ServiceID;
            if (string.IsNullOrWhiteSpace(AccountID) && string.IsNullOrWhiteSpace(nickName))
            {
                var list = CustomerSupportDAO.Instance.GetHistoryThankful(from, to);
                if (list == null)
                    list = new List<HistoryThankful>();
                var model = new GridModel<HistoryThankful> { Data = list.PagedForCommand(command), Total = list.Count };
                return new JsonResult { Data = model };
            }
            else
            {
                var list = CustomerSupportDAO.Instance.GetHistoryThankful(AccountID, from, to);
                if (list == null)
                    list = new List<HistoryThankful>();
                var model = new GridModel<HistoryThankful> { Data = list.PagedForCommand(command), Total = list.Count };
                return new JsonResult { Data = model };
            }
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult HistoryJackpot()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryJackpot(GridCommand command, ParCustomerSupport pars)
        {
            string nickName = pars.NickName;
            int gameId = pars.GameID;
            DateTime from = pars.FromDate;
            DateTime to = pars.ToDate;
            int serviceId = pars.ServiceID;
            if (string.IsNullOrWhiteSpace(nickName))
            {
                 var list = CustomerSupportDAO.Instance.GetHistoryJackpot(from, to, serviceId);
                 if (list == null)
                     list = new List<HistoryJackpot>();

                 var model = new GridModel<HistoryJackpot> { Data = list.PagedForCommand(command), Total = list.Count };
                 return new JsonResult { Data = model };
            }
            else
            {
                var list = CustomerSupportDAO.Instance.GetHistoryJackpot(nickName, gameId, from, to, serviceId);
                if (list == null)
                    list = new List<HistoryJackpot>();

                var model = new GridModel<HistoryJackpot> { Data = list.PagedForCommand(command), Total = list.Count };
                return new JsonResult { Data = model };
            }
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult TransactionHistory()
        {
            ViewBag.ServiceBox = GetServices();
            var list = new List<ConfigSelect>();
            list.AddRange(InfoHandler.Instance.GetAllGameService());
           // list.AddRange(InfoHandler.Instance.GetGameBox());
            ViewBag.GameBox = list;
            ParsTransactionHistory model = new ParsTransactionHistory();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTransactionHistory(GridCommand command, ParsTransactionHistory input)
        {
            int totalrecord = 0;
            var list = CustomerSupportDAO.Instance.GetListTransaction(input, command.Page <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<TransactionHistory>();

            var model = new GridModel<TransactionHistory> { Data = list, Total = totalrecord };
            return new JsonResult { Data = model };
        }
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult CallDragonHis()
        {
            ViewBag.ServiceBox = GetServices();
            var list = new List<ConfigSelect>();
            list.AddRange(InfoHandler.Instance.GetAllGameService());
            // list.AddRange(InfoHandler.Instance.GetGameBox());
            ViewBag.GameBox = list;
            ParsTransactionHistory model = new ParsTransactionHistory();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCallDragonHis(GridCommand command, ParsTransactionHistory input)
        {
            int totalrecord = 0;
            var list = CustomerSupportDAO.Instance.GetCallDragonHis(input, command.Page <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<TransactionHistory>();

            var model = new GridModel<TransactionHistory> { Data = list, Total = totalrecord };
            return new JsonResult { Data = model };
        }
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTransactionHistoryNew(GridCommand command, ParsTransactionHistory input)
        {
            int totalrecord = 0;
            var list = CustomerSupportDAO.Instance.GetListTransaction(input, command.Page <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<TransactionHistory>();

            var model = new GridModel<TransactionHistory> { Data = list, Total = totalrecord };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult AccountOverview()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost]
        public ActionResult AccountOverview(ParCustomerSupport data)
        {
            int searchType = data.SearchType;
            string value = data.Value;
            int serviceId = data.ServiceID;

            ViewBag.SearchType = searchType;
            ViewBag.Value = value;
            ViewBag.ServiceBox = GetServices();
            ViewBag.ServiceID = serviceId;
           
            int totalRecord = 0;
            ViewBag.ComplainTypes = UserComplainDAO.Instance.UserComplainTypeList(null, 1, Int16.MaxValue, out totalRecord).Select(c => new SelectListItem {
                Text=c.Name,
                Value=c.ID.ToString()
            }) ;

            if (searchType < 1 || string.IsNullOrEmpty(value))
                return View();

            AccountOverview account = CustomerSupportDAO.Instance.GetAccountInfoOverview(searchType, value, serviceId);
            if (account == null)
                return View();

            if (account.AuthenType == 1)
                Session["OverviewID"] = account.UserID;

            //check user has in blacklist
            int resBl = UserDAO.Instance.CheckUserBlackList(account.UserID);
            if (resBl == 1)
                account.IsBlackList = resBl;
            //MsWebGame.CSKH.Helpers.EsportsDataBalance esportsDataBalance = MsWebGame.CSKH.Helpers.Esports.GetBalance(account.GameAccountName);
            //if (esportsDataBalance != null)
            //{
            //    ViewBag.EsportsVi = (long) esportsDataBalance.GetSabeCoin();
            //    ViewBag.EsportsGame =(long) esportsDataBalance.GetSabaToGame();
            //}
            //else
            //{
            //    ViewBag.EsportsVi = 0;
            //    ViewBag.EsportsGame = 0;
            //}
            //Lay ra thong tin no
            account.BeforQuater = new AccountQuaterInfor();
            account.CurrentQuater = new AccountQuaterInfor();

            DateTime StartQuaterTime;
            DateTime EndQuaterTime;
            int currentQuater;
            DateUtil.GetQuatarStartEndDate(DateTime.Now, out currentQuater, out StartQuaterTime, out EndQuaterTime);
            int RankID;
            int VPQuaterAcc;//hiển thị số tiền chỗ thưởng quý
            int VipQuaterCoeff;
            int QuaterPrizeStatus;//0: đã nhận; 1: hợp lệ chưa nhận; 2: Chua den thoi gian nhan; 3: Qua thoi gian nhan
            float LoanLimit;
            float LoanRate;
            long QuaterAcc;
            long LoanAmount;//số tiền được vay
            long OldDebt;//số tiền nợ
            UserDAO.Instance.VIPCheckQuaterLoan(account.UserID, StartQuaterTime, EndQuaterTime, out RankID, out VPQuaterAcc, out VipQuaterCoeff, out QuaterPrizeStatus, out LoanLimit, out LoanRate, out QuaterAcc, out LoanAmount, out OldDebt);
            account.CurrentQuater.LoanAmount = LoanAmount;
            account.CurrentQuater.OldDebt = OldDebt;
            account.CurrentQuater.CurrentQuater = currentQuater;
            account.CurrentQuater.StartTime = StartQuaterTime;
            account.CurrentQuater.EndTime = EndQuaterTime;
            account.CurrentQuater.QuaterAcc = QuaterAcc;
            //lấy ra thông tin quý trước đó 
            DateTime BeforeStartQuaterTime;
            DateTime BeforeEndQuaterTime;
            int BeforeCurrentQuater;
            DateUtil.GetQuatarStartEndDate(DateTime.Now.AddMonths(-3), out BeforeCurrentQuater, out BeforeStartQuaterTime, out BeforeEndQuaterTime);

            int BeforeRankID;
            int BeforeVPQuaterAcc;//hiển thị số tiền chỗ thưởng quý
            int BeforeVipQuaterCoeff;
            int BeforeQuaterPrizeStatus;//hiển thị xem đã nhận thưởng hay chưa;
            float BeforeLoanLimit;
            float BeforeLoanRate;
            long BeforeQuaterAcc;
            long BeforeLoanAmount;//số tiền được vay
            long BeforeOldDebt;//số tiền nợ

            UserDAO.Instance.VIPCheckQuaterLoan(account.UserID, BeforeStartQuaterTime, BeforeEndQuaterTime, out BeforeRankID, out BeforeVPQuaterAcc, out BeforeVipQuaterCoeff, out BeforeQuaterPrizeStatus, out BeforeLoanLimit, out BeforeLoanRate, out BeforeQuaterAcc, out BeforeLoanAmount, out BeforeOldDebt);

            account.BeforQuater.LoanAmount = BeforeLoanAmount;
            account.BeforQuater.OldDebt = BeforeOldDebt;
            account.BeforQuater.CurrentQuater = BeforeCurrentQuater;
            account.BeforQuater.StartTime = BeforeStartQuaterTime;
            account.BeforQuater.EndTime = BeforeEndQuaterTime;
            account.BeforQuater.QuaterAcc = BeforeQuaterAcc;



            Session["AccountGameProfit"] = account.UserID;
            return View(account);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult KiemTraHoanTienTaiXiu()
        {
            TaiXiuRefunParam param = new TaiXiuRefunParam();
            ViewBag.ServiceBox = GetServices();
            return View(param);
        }
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost]
        public ActionResult KiemTraHoanTienTaiXiu(TaiXiuRefunParam param)
        {
            AccountOverview account = CustomerSupportDAO.Instance.GetAccountInfoOverview(1, param.NickName, 1);
            TaiXiuRefunParam data = new TaiXiuRefunParam();
            if (account == null)
                return View(data);
            List<Refunds> eventModel = LuckyDiceDAO.Instance.GetRefundsInfo(account.UserID,param.SessionID);
            if (eventModel != null && eventModel.Count>0)
            {

                data = new TaiXiuRefunParam();
                data.NickName = param.NickName;
                data.SessionID = param.SessionID;
                data.AccountID = account.UserID;
                data.Amount = eventModel.Sum(c=>c.Amount);
                ViewBag.Date = eventModel.FirstOrDefault().CreatedDate.ToString("dd/MM/yyyy hh:mm:ss");
                return View(data);
            }


            //check user has in blacklist
            return View(data);
        }


        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpGet]
        public dynamic AccountGameProfit(int gameId)
        {
            if (gameId < 1)
                return null;

            if (Session["AccountGameProfit"] == null)
                return null;

            long accountId = long.Parse(Session["AccountGameProfit"].ToString());
            AccountPlayGame result = CustomerSupportDAO.Instance.GetAccountGameProfit(gameId, accountId);
            if (result == null)
                return null;

            var json = new JavaScriptSerializer().Serialize(result);
            return json;
        }

        [HttpGet]
        public dynamic GetGameInfo()
        {
            try
            {
                var lstRs = InfoHandler.Instance.GetGameBox();
                var json = new JavaScriptSerializer().Serialize(lstRs);
                return json;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [HttpGet]
        public ActionResult AccountLoginIP(long accountId, int top)
        {
            List<AccountLoginIP> lstRs = null;
            if (accountId > 0)
                lstRs = AccountDAO.Instance.GetAccountLoginIP(accountId, top);

            return PartialView(lstRs);
        }
        [HttpGet]
        public ActionResult ConfigLiveUser(long accountId, int top)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + accountId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHu);
            return PartialView(parConfig == null? new Database.DTO.ParConfigLive(): parConfig);
        }
        [HttpPost]
        public ActionResult ConfigLiveUser(Models.Param.ParConfigLive data)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("UsersLive","Config:"+ data.UserId);
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            
            _cachePvd.Set(keyHu, data);
            return new JsonResult { 
                Data = new
                {
                    UserId = data.UserId,
                    Price = data.Tranfer,
                    Game = data.Game,
                    Type = data.Type
                }
            };
        }
        [HttpGet]
        public ActionResult UpdatePhone(long accountId, int top)
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult UpdatePhone(Models.Param.ParUpdatePhone data)
        {
            long TeleId = long.Parse(AccountDAO.Instance.GetTelegramId(data.PhoneNumber));
            int Status = -1;
            if (TeleId > 0)
            {
                if (data.Type == 2)
                {
                    Status = AccountDAO.Instance.DeletePhone(data.AccountId , data.ServiceID);
                }
                else
                {
                    Status = AccountDAO.Instance.UpdatePhone(data.AccountId, data.PhoneNumber, data.ServiceID, TeleId);
                }
            }
            return new JsonResult
            {
                Data = new
                {
                    TeleId  = TeleId,
                    Status = Status
                }
            };
        }
        [HttpPost]
        public int CloseLoginOtp()
        {
            if (Session["OverviewID"] == null)
                return -1;

            long accountId = long.Parse(Session["OverviewID"].ToString());
            int response = UserDAO.Instance.AccountDisableSecure(accountId);
            if (response == 1)
                Session["OverviewID"] = null;

            return response;
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult VpProgress()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetVpProgress(GridCommand command, int serviceId)
        {
            var lstRs = CustomerSupportDAO.Instance.GetVpProgress(serviceId);
            if (lstRs == null)
                lstRs = new List<VpProgress>();

            var model = new GridModel<VpProgress> { Data = lstRs, Total = 1 };
            return new JsonResult { Data = model };
        }
        #endregion

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult BangXepHang()
        {
            ViewBag.ServiceBox = GetServices();
            //var list = new List<ConfigSelect>();
            //list.AddRange(InfoHandler.Instance.GetAllGameService());
            // list.AddRange(InfoHandler.Instance.GetGameBox());
            //ViewBag.Type = list;
            ParsBangXepHang model = new ParsBangXepHang();
            model.Type = 1;
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetBangXepHang(GridCommand command, ParsBangXepHang input)
        {
            int totalrecord = 0;
            var list = CustomerSupportDAO.Instance.GetListBangXepHang(input, command.Page <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<UserProfit>();

            var model = new GridModel<UserProfit> { Data = list, Total = totalrecord };
            return new JsonResult { Data = model };
        }
        
        public ActionResult AddMoneyBXH(long Id,long AccountId,string Nickname)
        {
            ViewBag.Id = Id;
            ViewBag.AccountId = AccountId;
            ViewBag.Nickname = Nickname;
            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            model.receiverName = Nickname;
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            return View(model);
        }

        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult AddMoneyBXH(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }
            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferToUser2(AdminID, accInfo.AccountID, iamount, input.note, input.ServiceID, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTransferToUser - AdminID:{0} | AgencyID:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, accInfo.AccountID, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại|" + res;
                return View(input);
            }
            else
            {
                int outResponse;
                long msgID;
                if (accInfo != null)
                {
                    string content = string.Format("Bạn vừa nhận {0} từ nickname {1}",
                       input.amount, AdminDisplayName);
                    if (!String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(accInfo.SignalID) && !String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accInfo.SignalID }, content);
                    }
                    int response = MailDAO.Instance.SystemSendMailToUser(AdminID, accInfo.AccountID, accInfo.AccountName, "Tri ân khách hàng", "X6 Club gửi tặng bạn "+ MoneyExtension.LongToMoneyFormat(iamount) + " game tri ân khách hàng . Chúc bạn chơi game vui vẻ !", 1, DateTime.Now, 1);
                    
                    if (response == AppConstants.DBS.SUCCESS)
                    {
                        SuccessNotification(Message.SendMailSuccess);
                        return RedirectToAction("BangXepHang");
                    }
                    else
                    {
                        string msg = "Gửi mail thất bại";
                        ErrorNotification(msg);
                    }
                }
                ViewBag.Message = "Chuyển khoản thành công";
                return RedirectToAction("BangXepHang");
                ////Cộng tiền và ghi log  User
                //int resUser = AdminDAO.Instance.UserTransferReceive(AdminID, AdminAccountName, accInfo.AccountID, iamount, input.note, transId, wallet);
                //NLogManager.LogMessage(string.Format("UserTransferReceive - Wallet:{0} | Response:{1}", wallet, resUser));
                //if (resUser == 1)
                //{
                //    Session["TranferMoneyToUser"] = "Chuyển khoản thành công";
                //    return RedirectToAction("TranferMoneyToUser");
                //}
                //else
                //{
                //    ViewBag.Message = "Cộng tiền cho người chơi không thành công. Báo lại cho admin";
                //    //Update trạng thái giao dịch = 2;
                //    int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                //    NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                //    return View(input);
                //}
            }
        }

        protected List<DropDownItem> GetServices()
        {
            try
            {
                var list = GameDAO.Instance.GetSerivces();
                return list.Select(c => new DropDownItem()
                {
                    Value = c.ServiceID.ToString(),
                    Text = c.ServiceName,

                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        private double GetTransFee(string feeCode)
        {
            if (string.IsNullOrEmpty(feeCode))
                return 0;

            string paramType = "TRANSFEE";
            int totalRecord = 0;
            var lstFee = ParamConfigDAO.Instance.GetList(paramType, feeCode, null, 1, Int16.MaxValue, out totalRecord);
            if (lstFee != null && lstFee.Any())
                return double.Parse(lstFee.FirstOrDefault().Value);

            return 0;
        }
        private long GetAmountAdmin()
        {
            try
            {
                var admin = AdminDAO.Instance.GetById(AdminID);
                return Int64.Parse(admin.Wallet.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
        }
    }
}