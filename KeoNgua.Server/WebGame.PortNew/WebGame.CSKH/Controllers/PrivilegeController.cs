using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class PrivilegeController : BaseController
    {
        // GET: Privilege
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index(DateTime? startDate, DateTime? endDate, int serviceId = 1)
        {
            if (serviceId < 1)
                serviceId = 1;

            DateTime date = DateTime.Today;
            var firstDayOfMonth = new DateTime(date.Year, date.Month - 1, 1);
            ViewBag.FirstDay = firstDayOfMonth;
            if (startDate == null)
                startDate = firstDayOfMonth;

            if (endDate == null)
                endDate = DateTime.Now;

            var lstRs = PrivilegeDAO.Instance.GetReportAccountVIP(startDate, endDate, serviceId);
            ViewBag.Data = JsonConvert.SerializeObject(lstRs, _jsonSetting);
            ViewBag.ServiceBox = GetServices();
            return View(lstRs);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Lookup()
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetLookup(GridCommand command, ParsLookup input)
        {
            int totalRecord = 0;
            var lstRs = PrivilegeDAO.Instance.GetAccountVipPoint(input, command.Page <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            if (lstRs == null)
                lstRs = new List<PrivilegeLookup>();

            var model = new GridModel<PrivilegeLookup> { Data = lstRs, Total = totalRecord };
            return new JsonResult { Data = model };
        }

        //[AdminAuthorize(Roles = ADMIN_MONITOR_MARKETING_VIEW_BB_ROLE)]
        public ActionResult ReportVpToDate()
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.MKTServiceID = GetServiceIDBRole;
            return View();
        }

        //[AdminAuthorize(Roles = ADMIN_MONITOR_MARKETING_VIEW_BB_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetReportVpToDate(GridCommand command, DateTime? fromDate, DateTime? toDate, int serviceId)
        {
            if (fromDate == null)
                fromDate = DateTime.Today;

            if (toDate == null)
                toDate = DateTime.Now;

            if (serviceId < 1)
                serviceId = 1;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            ViewBag.MKTServiceID = serviceId;

            int totalRecord = 0;
            var lstRs = PrivilegeDAO.Instance.GetReportVP( serviceId);
            if (lstRs == null)
                lstRs = new List<ReportVP>();

            var model = new GridModel<ReportVP> { Data = lstRs, Total = totalRecord };
            return new JsonResult { Data = model };
        }


        //[AdminAuthorize(Roles = ADMIN_MONITOR_MARKETING_VIEW_BB_ROLE)]
        public ActionResult ReportPUToDate()
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.MKTServiceID = GetServiceIDBRole;
            return View();
        }

        //[AdminAuthorize(Roles = ADMIN_MONITOR_MARKETING_VIEW_BB_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetReportPUToDate(GridCommand command, DateTime? fromDate, DateTime? toDate, int serviceId)
        {
            if (fromDate == null)
                fromDate = DateTime.Today;

            if (toDate == null)
                toDate = DateTime.Now;

            if (serviceId < 1)
                serviceId = 1;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            ViewBag.MKTServiceID = serviceId;

            int totalRecord = 0;
            var lstRs = PrivilegeDAO.Instance.GetReportPU(serviceId);
            if (lstRs == null)
                lstRs = new List<ReportVP>();

            var model = new GridModel<ReportVP> { Data = lstRs, Total = totalRecord };
            return new JsonResult { Data = model };
        }

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
    }


}