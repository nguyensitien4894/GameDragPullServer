using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Utils;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.Database.DTO;
using System.Collections.Generic;
using System.Linq;
using MsWebGame.CSKH.Models.Param;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class ReportController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_MONITOR_VIEW_ROLE)]
        public ActionResult GetCashflowOverview(DateTime? fromDate, DateTime? toDate, int? serviceId = 0)
        {
            DateTime date = DateTime.Today;
            var firstDayOfMonth = DateTime.Now.AddDays(-7);
            ViewBag.FirstDay = firstDayOfMonth;
            if (fromDate == null)
                fromDate = firstDayOfMonth;

            if (toDate == null)
                toDate = DateTime.Now;
            ViewBag.ServiceBox = GetServices();
            var lstRs = Session["RoleCode"].ToString() == VIEW_ROLE ? CashflowOverviewDAO.Instance.GetOtherList(fromDate, toDate, serviceId.Value):
                CashflowOverviewDAO.Instance.GetList(fromDate, toDate, serviceId.Value);
            ViewBag.Data = JsonConvert.SerializeObject(lstRs, _jsonSetting);
            return View(lstRs);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GetCardRechargeProgress(int serviceId = 1)
        {
            ViewBag.ServiceBox = GetServices();
            var lst = CardDAO.Instance.GetCardRechargeProgress(serviceId);
            return View(lst);
        }

        #region thống kê giao dịch tống với c1
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GetAgencyC1Report(AgencyGeneralRevenueInfo data)
        {
            DateTime fromDate = data.FromDate != DateTime.MinValue ? data.FromDate : DateTime.Now.AddDays(-7);
            DateTime toDate = data.ToDate != DateTime.MinValue ? data.ToDate : DateTime.Now.AddDays(1);
            if (data.ServiceID == 0)
                data.ServiceID = 1;

            int serviceId = data.ServiceID;
            ViewBag.ServiceBox = GetServices();

            // get list agency total
            List<SelectListItem> lstDrop = InfoHandler.Instance.GetAgencyTotal(serviceId);
            ViewBag.AgencyTotalBox = lstDrop;

            long agencyId = data.AgencyIDTotal;
            if (data.AgencyIDTotal <= 0 && lstDrop != null && lstDrop.Any())
                agencyId = Int64.Parse(lstDrop[0].Value);

            //get balance of agency total select
            var agencyInfo = AgencyDAO.Instance.GetAgencyInfo((int)AgencyKeyType.ID, agencyId.ToString(), serviceId);
            if (agencyInfo != null)
            {
                data.Balance = agencyInfo.Wallet;
            }
            else
            {
                agencyId = Int64.Parse(lstDrop[0].Value);
                agencyInfo = AgencyDAO.Instance.GetAgencyInfo((int)AgencyKeyType.ID, agencyId.ToString(), serviceId);
                if (agencyInfo != null)
                    data.Balance = agencyInfo.Wallet;
            }

            var lstRs = AgencyDAO.Instance.AgencyC1GeneralRevenue(agencyId, fromDate, toDate, serviceId);
            data.LstReport = lstRs;
            return View(data);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult AgencyC1ReportDetails(ParReport data)
        {
            ViewBag.ServiceBox = GetServices();
            if (data.FromDate == DateTime.MinValue)
                data.FromDate = DateTime.Now;

            ViewBag.AgencyTotalBox = InfoHandler.Instance.GetAgencyTotal(data.ServiceID);
            return View(data);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetAgencyC1ReportDetails(ParReport data)
        {
            int transType = data.TransType;
            string partnerName = data.PartnerName;
            DateTime fromDate = data.FromDate;
            DateTime toDate = fromDate.AddDays(1).AddTicks(-1);
            int serviceId = data.ServiceID;

            if (string.IsNullOrEmpty(partnerName))
            {
                var rs = new GridModel<ReportAgencyGeneralTrans> { Data = null, Total = 0 };
                return new JsonResult { Data = rs };
            }

            long agencyId = data.AgencyIDTotal;
            var lstRs = AgencyDAO.Instance.AgencyGeneralTrans(agencyId, transType, partnerName, fromDate, toDate, serviceId);
            var model = new GridModel<ReportAgencyGeneralTrans> { Data = lstRs, Total = 0 };
            return new JsonResult { Data = model };
        }

        #endregion

        #region thống kê đại lý c1 cho user
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult AgencyL1CashFlowUsers(AgencyL1CashFlowUsersInfo data)
        {
            ViewBag.ServiceBox = GetServices();

            DateTime fromDate = data.FromDate != DateTime.MinValue ? data.FromDate : DateTime.Now.AddDays(-7);
            DateTime toDate = data.ToDate != DateTime.MinValue ? data.ToDate : DateTime.Now;
            if (data.ServiceID == 0)
                data.ServiceID = 1;

            int serviceId = data.ServiceID;
            string nickName = data.NickName;
            long agencyId = 0;
            if (!string.IsNullOrEmpty(nickName))
            {
                var agencyInfo = UserDAO.Instance.GetAccountByNickName(nickName, serviceId);
                if (agencyInfo != null && agencyInfo.AccountType == 2)
                    agencyId = agencyInfo.AccountID;
            }

            var lstRs = AgencyDAO.Instance.AgencyL1CashFlowUsers(agencyId, fromDate, toDate, serviceId);
            data.LstResult = lstRs;
            return View(data);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult AgencyL1CashFlowUsersDetails(ParReport data)
        {
            if (data.FromDate == DateTime.MinValue)
                data.FromDate = DateTime.Now;

            ViewBag.ServiceBox = GetServices();
            return View(data);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetAgencyL1CashFlowUsersDetails(GridCommand command, ParReport data)
        {
            string nickName = data.NickName;
            string partnerName = data.PartnerName;
            int partnerType = 1;
            int transType = data.TransType;
            DateTime fromDate = data.FromDate;
            DateTime toDate = fromDate.AddDays(1).AddTicks(-1);
            int serviceId = data.ServiceID;

            long accountId = 0;
            if (!string.IsNullOrEmpty(nickName))
            {
                UserInfo agInfo = UserDAO.Instance.GetAccountByNickName(nickName, serviceId);
                if (agInfo != null)
                    accountId = agInfo.AccountID;
            }

            int totalRecord = 0;
            List<WalletLogs> lstRs = AgencyDAO.Instance.WalletLogsAgencyList(accountId, partnerType, transType, partnerName, -1, null, fromDate,
                toDate, serviceId, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);

            if (lstRs == null)
                lstRs = new List<WalletLogs>();

            var model = new GridModel<WalletLogs> { Data = lstRs, Total = totalRecord };
            return new JsonResult { Data = model };
        }

        #endregion

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CashFlowOfEachAgency()
        {
          

            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCashFlowOfEachAgency(string nickName, DateTime fromDate, DateTime toDate, int status = -1, int serviceId = 1)
        {
            

            if (string.IsNullOrEmpty(nickName))
                nickName = null;

            toDate = toDate.AddDays(1).AddMilliseconds(-1);
            var lstRs = AgencyDAO.Instance.GetCashFlowOfEachAgency(null, nickName, null, status, fromDate, toDate, serviceId);
            if (lstRs == null)
            {
                var model = new GridModel<CashFlowOfEachAgency> { Data = lstRs, Total = 0 };
                return new JsonResult { Data = model };
            }

            var gridModel = new GridModel<CashFlowOfEachAgency> { Data = lstRs, Total = 1 };
            return new JsonResult { Data = gridModel };
        }

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
    }
}