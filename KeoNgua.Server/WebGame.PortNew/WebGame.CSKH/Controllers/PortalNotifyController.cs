using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;

namespace MsWebGame.CSKH.Controllers
{
    public class PortalNotifyController : BaseController
    {
        // GET: PortalNotify
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetPortalNotify(GridCommand command, bool? status, DateTime? fromDate, DateTime? toDate, int serviceId = 1)
        {
            var lstRs = NotifyDAO.Instance.GetPortalGameNotifyList(status, fromDate, toDate, serviceId);
            if (lstRs == null)
                lstRs = new List<PortalNotify>();

            var model = new GridModel<PortalNotify>
            {
                Data = lstRs,
                Total = lstRs.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        #region thêm mới - sửa notify
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult CreateNotify()
        {
            PortalNotify model = new PortalNotify();
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult CreateNotify(PortalNotify input)
        {
            ViewBag.ServiceBox = GetServices();
            if (string.IsNullOrEmpty(input.Content))
                return View(input);

            int response = NotifyDAO.Instance.CreatePortalGameNotify(input.Content,input.ServiceID);
            if (response == 1)
            {
                SuccessNotification("Thêm mới thành công");
                input = new PortalNotify();
                return View(input);
            }
            else
            {
                ErrorNotification("Thêm mới thất bại");
            }

            return View(input);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdatePortalNotify(GridCommand command, PortalNotify input)
        {
            if (input.ID < 1)
                return Content("Fail");

            int response = NotifyDAO.Instance.UpdatePortalGameNotify(input);
            return GetPortalNotify(command, null, null, null);
        }
        #endregion
    }
}