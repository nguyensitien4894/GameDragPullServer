using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using MsTraditionGame.Utilities.Utils;
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
    public class CcuController : BaseController
    {
        private List<string> accpetList = new List<string> { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01 };
        // GET: CustomerSupport
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null)
                fromDate = DateTime.Today.AddDays(-59);

            if (toDate == null)
                toDate = DateTime.Today;

            DateTime date = DateTime.Now;
            date = date.AddDays(-59);
            DateTime dateStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            DateTime dEnd = DateTime.Now;
            DateTime dateEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59, 999);

            long totalAndroid = 0;
            long totalIos = 0;
            long totalWeb = 0;

            List<CuuListModel> Lists = CcuDAO.Instance.GetLists(fromDate, toDate, out totalAndroid, out totalIos, out totalWeb);
            List<object[]> list = new List<object[]>();
            foreach (CuuListModel val in Lists)
            {
                list.Add(new object[] { val.Time, val.Web, val.Android, val.Ios, val.Total });
            }

            ViewBag.DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.CCU = Lists;

            ViewBag.TotalAndroid = totalAndroid;
            ViewBag.TotalIos = totalIos;
            ViewBag.TotalWeb = totalWeb;

            return View(list);
        }
    }
}