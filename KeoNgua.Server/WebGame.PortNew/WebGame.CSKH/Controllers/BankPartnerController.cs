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

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class BankPartnerController : BaseController
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
            var list = BankPartnersDAO.Instance.GetList(serviceId);
           
            var model = new GridModel<BankPartnersModel>
            {
                Data = list.Select(x =>
                {
                    var m = new BankPartnersModel();
                    m = Mapper.Map<BankPartnersModel>(x);
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
        public ActionResult Update(BankPartnersModel model, GridCommand command)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }

            if (String.IsNullOrEmpty(model.Momo))
            {
                return Content("Không thể để trống giá trị Momo ");
            }
           
            var upPartner = CardPartnersDAO.Instance.CardPartnerGetByID(model.Id);
            if (upPartner == null)
            {
                return Content("Không tìm thấy đối tác");
            }
            if (model.Momo != "0")
            {


                var arr = model.Momo.Split(',');
                foreach (var item in arr)
                {
                    if (item != model.Id.ToString())
                    {
                        return Content(String.Format("Giá trị cập nhật momo theo định dạng {0},{1}", model.Id, model.Id));

                    }
                }

            }
            
          
          
           
           
            int response = 0;
            BankPartnersDAO.Instance.BankPartnerUpdate(model.Id, model.Momo,null, out response);
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