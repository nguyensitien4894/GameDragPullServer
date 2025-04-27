using AutoMapper;
using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.CardPartnerss;
using MsWebGame.CSKH.Utils;
using System.Collections.Generic;
using MsWebGame.CSKH.Models;
using MsWebGame.CSKH.Database;
using MsWebGame.CSKH.Models.SmsOperators;

namespace MsWebGame.CSKH.Controllers
{
    public class SmsOperatorController : BaseController
    {
        private List<string> acceptList = new List<string>() { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09, AppConstants.ADMINUSER.USER_CSKH_08, AppConstants.ADMINUSER.USER_CSKH_02 };
        // GET: CardPartner
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult Index()
        {

            if (!acceptList.Contains((string)Session["UserName"]))

            {
                return RedirectToAction("Permission", "Account");

            }
            ViewBag.ServiceBox = GetServices();
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult GetList(GridCommand command, int serviceId = 1)
        {
            if (Session["AdminID"] == null)
            {
                return RedirectToAction("Login");
            }
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            var list = SmsChargeDAO.Instance.SmsOperatorList(serviceId, null, null, null);

            var model = new GridModel<SmsOperatorModel>
            {
                Data = list.Select(x =>
                {
                    var m = new SmsOperatorModel();
                    m = Mapper.Map<SmsOperatorModel>(x);
                    m.ServiceID = serviceId;
                    return m;
                }).PagedForCommand(command),

                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };

        }


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult Update(SmsOperatorModel model, GridCommand command)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }

           

            var upPartner = SmsChargeDAO.Instance.SmsOperatorrGetByID(model.ID);
            if (upPartner == null)
            {
                return Content("Không tìm thấy đối tác");
            }
          





            int response = 0;
            SmsChargeDAO.Instance.SmsOperatorrUpdate(model.ID,model.Status, out response);
            if (response == 1)
            {

                return GetList(command, model.ServiceID);
            }
            else
            {

            }
            return Content(String.Format("Cập nhật thất bại"));



        }
    }
}