using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Utils;
using Telerik.Web.Mvc.UI;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class StatisticsController : BaseController
    {
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };


        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult NRUStatistic()
        {
            ViewBag.ServiceBox = GetServices();
            
              var  from = DateTime.Today.AddDays(-7);
         
           var  to = DateTime.Now.AddDays(1).AddMinutes(-1);
            int serviceId = 1;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            ViewBag.MKTServiceID = serviceId;
            var list = StatisticsDAO.Instance.NruDeviceList(serviceId, from, to);
            ViewBag.Data = JsonConvert.SerializeObject(list, _jsonSetting);
            return View(list);
           
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult NRUStatistic(GridCommand command,DateTime from, DateTime to,int serviceId)
        {
            

          
            ViewBag.ServiceBox = GetServices();
            ViewBag.ServiceID = serviceId;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            ViewBag.MKTServiceID = serviceId;
            if (from == null)
            {
                from = DateTime.Today.AddDays(-7);
            }
            if (to == null)
            {
                to = DateTime.Now;
            }
            to = to.AddDays(1).AddMinutes(-1);

            var list = StatisticsDAO.Instance.NruDeviceList(serviceId, from, to);
            ViewBag.Data = JsonConvert.SerializeObject(list, _jsonSetting);
            return View(list);
        }

        /// <summary>
        /// Thống kê download
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MARKETTING_BB_ROLE)]
        public ActionResult TrackingDownload()
        {

            return View();
        }
        /// <summary>
        /// Thống kê download
        /// </summary>
        /// <param name="command"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MARKETTING_BB_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult TrackingDownload(GridCommand command, DateTime from, DateTime to, string urlPage,int? typeDownLoad= 0)
        {
            if (from == null)
            {
                from = DateTime.Today.AddDays(-7);
            }
            if (to == null)
            {
                to = DateTime.Now;
            }
            to = to.AddDays(1).AddMinutes(-1);
            if (typeDownLoad.HasValue && typeDownLoad <= 0) typeDownLoad = null;
            if (String.IsNullOrEmpty(urlPage)) urlPage = null;
            var list = StatisticsDAO.Instance.PageTrackingInfoStatistic(typeDownLoad, 2, from, to, urlPage);
            if (list == null)
                list = new List<PageTrackingInfo>();
            var TotalDownLoad = list.Sum(c => c.DownloadCount);
            var model = new GridModel<PageTrackingInfo>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count,
                
                
            };
            return new JsonResult { Data = model };
        }

        /// <summary>
        /// Thống kê đăng kí người dùng
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MARKETTING_BB_ROLE)]
        public ActionResult TrackingUser()
        {
            int serviceId = 1;
            ViewBag.ServiceBox = GetServices();
            ViewBag.ServiceID = serviceId;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            @ViewBag.MKTServiceID = serviceId;
            return View();
        }
        /// <summary>
        /// Thống kê đăng kí người dùng
        /// </summary>
        /// <param name="command"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MARKETTING_BB_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTrackingUser(GridCommand command, DateTime from, DateTime to,string urlPage,int serviceId, string UrlPath,string UtmMedium,string UtmSource,string UtmCampaign,string UtmContent,int? LoginType)
        {
            
            ViewBag.ServiceBox = GetServices();
            ViewBag.ServiceID = serviceId;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            if (from == null)
            {
                from = DateTime.Today.AddDays(-7);
            }
            if (to == null)
            {
                to = DateTime.Now;
            }
            to = to.AddDays(1).AddMinutes(-1);
            if (String.IsNullOrEmpty(urlPage)) urlPage = null;
            if (String.IsNullOrEmpty(UtmMedium)) UtmMedium = null;
            if (String.IsNullOrEmpty(UrlPath)) UrlPath = null;
            if (String.IsNullOrEmpty(UtmSource)) UtmSource = null;
            if (String.IsNullOrEmpty(UtmCampaign)) UtmCampaign = null;
            if (String.IsNullOrEmpty(UtmContent)) UtmContent = null;
            if (LoginType.HasValue && LoginType == 0) LoginType = null;
            var list = StatisticsDAO.Instance.UserTrackingPageStatistic(serviceId, from, to,null,UrlPath,UtmMedium,UtmSource,UtmCampaign,UtmContent, LoginType);
            if (list == null)
                list = new List<UserTrackingPage>();

            var model = new GridModel<UserTrackingPage>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult StatisticsJackpot()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetStatisticsJackpot(GridCommand command, int gameId, DateTime from, DateTime to, int serviceId)
        {
            var list = StatisticsDAO.Instance.GetTrackingJackpot(gameId, from, to, serviceId);
            if (list == null)
                list = new List<TrackingJackpot>();

            var model = new GridModel<TrackingJackpot>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult StatisticsUser(DateTime? playDate = null, int serviceId = 1)
        {
            if (playDate == null)
                playDate = DateTime.Today;

            serviceId = !GetServiceIDBRole.HasValue? serviceId: GetServiceIDBRole.Value;
            ViewBag.MKTServiceID = serviceId;
            var lstRs = UserDAO.Instance.GamePlayTrackingHour(playDate, serviceId);
            ViewBag.PlayDate = string.Format("Dữ liệu ngày {0}", playDate.Value.ToString("dd/MM/yyyy"));
            ViewBag.Data = JsonConvert.SerializeObject(lstRs, _jsonSetting);
            ViewBag.ServiceBox = GetServices();
            return View(lstRs);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CashFlowAdmin(string nickName = null, int partnerType = 0, DateTime? fromDate = null, DateTime? toDate = null, int serviceId = 0)
        {
            List<DropDownItem> lst = GetServices();
            lst.Insert(0, new DropDownItem() { Text = "All", Value = "0" });
            ViewBag.ServiceBox = lst;
            ViewBag.NickName = nickName;
            long adminid = 0;
            if (!string.IsNullOrEmpty(nickName) && serviceId > 0)
            {
                var lstAd = AdminDAO.Instance.GetList(1, nickName, null, serviceId);
                if (lstAd == null || !lstAd.Any())
                    return View();

                adminid = lstAd.First().AccountID;
            }

            if (fromDate == null)
                fromDate = DateTime.Today.AddDays(-7);

            if (toDate == null)
                toDate = DateTime.Today;

            var rs = AdminDAO.Instance.GetAdminRevenue(adminid, partnerType, fromDate, toDate, serviceId);
            return View(rs);
        }


        //[AdminAuthorize(Roles = ADMIN_MONITOR_MARKETING_VIEW_BB_ROLE)]
        public ActionResult NewUserStatistics(DateTime? fromDate = null, DateTime? toDate = null, int serviceId = 1)
        {
            if (fromDate == null)
                fromDate = DateTime.Today.AddDays(-7);

            if (toDate == null)
                toDate = DateTime.Today;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            ViewBag.MKTServiceID = serviceId;

            //if (serviceId < 1)
            //    serviceId = 1;

            var lstRs = UserDAO.Instance.ReportUsersCreateNew(fromDate, toDate, serviceId);
            ViewBag.Data = JsonConvert.SerializeObject(lstRs, _jsonSetting);
            ViewBag.ServiceBox = GetServices();
            return View(lstRs);
        }
        public ActionResult Profit (DateTime? fromDate = null, DateTime? toDate = null, int serviceId = 1)
        {
            if (fromDate == null)
                fromDate = DateTime.Today.AddDays(-1);

            if (toDate == null)
                toDate = DateTime.Today;
            serviceId = !GetServiceIDBRole.HasValue ? serviceId : GetServiceIDBRole.Value;
            ViewBag.MKTServiceID = serviceId;

            //if (serviceId < 1)
            //    serviceId = 1;

            var lstRs = UserDAO.Instance.ReportProfit(fromDate, toDate);
            ViewBag.Data = JsonConvert.SerializeObject(lstRs, _jsonSetting);
            ViewBag.ServiceBox = GetServices();
            return View(lstRs);
        }
    }
}