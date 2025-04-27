
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.SMSOTPS;
using MsWebGame.CSKH.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Controllers
{
    public class OtpController :BaseController
    {
        // GET: CustomerSupport
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult Index()
        {
            SmsOtpListModel model = new SmsOtpListModel();
            model.FromDate = DateTime.Now.AddDays(-7);
            model.ToDate = DateTime.Now.AddDays(1);
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        /// <summary>
        /// search hiển thị danh sách chăm sóc khách hàng
        /// </summary>
        /// <param name="command"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command, SmsOtpListModel model)
        {
            //lay danh sách chăm sóc khách hàng
           long totalRecord = 0;
            var list = SMSOTPDAO.Instance.GetList(model.NickName,model.Msisdn.PhoneFormat(),null,model.ServiceID,model.FromDate,model.ToDate.Date.AddDays(1).AddSeconds(-1), command.Page - 1 <= 0 ? 1 : command.Page,
                AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<SmsOtpModel>
            {
                Data = list.Select(x =>
                {
                    var m = new SmsOtpModel();
                    m.Msisdn = x.Msisdn.PhoneDisplayFormat();
                    m.Status = x.Status;
                    m.Type = x.Type;
                    m.LoginAppSafeStatus=x.Otp.Length==7?1:0;
                    m.Active = x.Active;
                    m.DisplayName = x.DisplayName;
                    m.CreateDate = x.CreateDate;
                    return m;
                }),
                Total = ConvertUtil.ToInt(totalRecord)
            };
            return new JsonResult { Data = gridModel };
        }
    }
}