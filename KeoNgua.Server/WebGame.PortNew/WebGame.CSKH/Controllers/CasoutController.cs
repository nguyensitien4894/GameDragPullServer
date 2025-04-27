using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MsTraditionGame.Utilities.Utils;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Security;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.HistoryTranfers;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Models.Transactions;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities.OneSignal;
 

namespace MsWebGame.CSKH.Controllers
{
    public class CasoutController : BaseController
    {
        // GET: Casout
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            DateTime date = DateTime.Now;
            DateTime dateStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            DateTime dateEnd = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);

            ViewBag.ServiceBox = GetServices();
            ViewBag.DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd");

            

            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryCasoutBank(GridCommand command, ParCustomerSupport data)
        {
            string nickName = data.NickName;
            DateTime date = DateTime.Now;
            DateTime fromDate = data.FromDate==null?  new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0): data.FromDate;
            DateTime toDate = data.ToDate==null?  new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999): data.ToDate;
            int serviceId = data.ServiceID;
            int orderby = data.OrderBy;
            int Status = data.SearchType;
            int totalrecord = 0;
            var lstRs = CasoutDAO.Instance.GetListBank( fromDate, toDate, Status, AppConstants.CONFIG.GRID_SIZE, command.Page - 1 <= 0 ? 1 : command.Page,
                out totalrecord);
            if (lstRs == null)
                lstRs = new List<CasoutBank>();

            var model = new GridModel<CasoutBank> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost]
        public JsonResult UpdateStatus(CasoutBankPar data)
        {
            int repon;
             CasoutDAO.Instance.UpdateStatus(data.UserId,data.Id,data.Status,out repon);
            return new JsonResult { Data = repon };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult IndexMomo()
        {
            DateTime date = DateTime.Now;
            DateTime dateStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            DateTime dateEnd = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);

            ViewBag.ServiceBox = GetServices();
            ViewBag.DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd");

            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetHistoryCasoutMomo(GridCommand command, ParCustomerSupport data)
        {
            string nickName = data.NickName;
            DateTime date = DateTime.Now;
            DateTime fromDate = data.FromDate == null ? new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0) : data.FromDate;
            DateTime toDate = data.ToDate == null ? new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999) : data.ToDate;
            int serviceId = data.ServiceID;
            int orderby = data.OrderBy;
            int Status = data.SearchType;
            int totalrecord = 0;
            var lstRs = CasoutDAO.Instance.GetListMomo(fromDate, toDate, Status, AppConstants.CONFIG.GRID_SIZE, command.Page - 1 <= 0 ? 1 : command.Page,
                out totalrecord);
            if (lstRs == null)
                lstRs = new List<CasoutMomo>();

            var model = new GridModel<CasoutMomo> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost]
        public JsonResult UpdateStatusMomo(CasoutBankPar data)
        {
            int repon;
            CasoutDAO.Instance.UpdateStatusMomo(data.UserId, data.Id, data.Status, out repon);
            return new JsonResult { Data = repon };
        }
    }
}