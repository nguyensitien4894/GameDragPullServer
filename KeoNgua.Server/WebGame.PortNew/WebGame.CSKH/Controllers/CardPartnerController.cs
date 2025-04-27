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

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class CardPartnerController : BaseController
    {
        private List<string> acceptList = new List<string>() { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09, AppConstants.ADMINUSER.USER_CSKH_08, AppConstants.ADMINUSER.USER_CSKH_02 };
        // GET: CardPartner
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult Index()
        {
            
           if(!acceptList.Contains((string)Session["UserName"]))
           
            {
                return RedirectToAction("Permission", "Account");

            }
            ViewBag.ServiceBox = GetServices();
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult GetList(GridCommand command,int serviceId = 1)
        {
            if (Session["AdminID"] == null)
            {
                return RedirectToAction("Login");
            }
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            var list = CardPartnersDAO.Instance.GetList(serviceId);
           
                 var model = new GridModel<CardPartnersModel>
            {
                Data = list.Select(x =>
                {
                    var m = new CardPartnersModel();
                    m = Mapper.Map<CardPartnersModel>(x);
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
        public ActionResult Update(CardPartnersModel model, GridCommand command)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            
            if (String.IsNullOrEmpty(model.VTT))
            {
                return Content("Không thể để trống giá trị viettel ");
            }
            if (String.IsNullOrEmpty(model.VNP))
            {
                return Content("Không thể để trống giá trị vina phone ");
            }
            if (String.IsNullOrEmpty(model.VMS))
            {
                return Content("Không thể để để trống thẻ mobile phone");
            }
            if (String.IsNullOrEmpty(model.ZING))
            {
                return Content("Không thể để để trống thẻ zing");
            }
            if (String.IsNullOrEmpty(model.VCOIN))
            {
                return Content("Không thể để để trống thẻ vcoin");
            }
            var upPartner = CardPartnersDAO.Instance.CardPartnerGetByID(model.Id);
            if (upPartner == null)
            {
                return Content("Không tìm thấy đối tác");
            }
            if (model.VTT != "0")
            {

               
                var arr = model.VTT.Split(',');
                foreach(var item in arr)
                {
                    if (item != model.Id.ToString())
                    {
                        return Content(String.Format("Giá trị cập nhật viettel theo định dạng {0},{1}",model.Id,model.Id));

                    }
                }
               
            }
            if (model.VMS != "0")
            {
                var arr = model.VMS.Split(',');
                foreach (var item in arr)
                {
                    if (item != model.Id.ToString())
                    {
                        return Content(String.Format("Giá trị cập nhật mobile phone theo định dạng {0},{1}", model.Id, model.Id));
                    }
                }
            }
            if (model.VNP != "0")
            {
                var arr = model.VNP.Split(',');
                foreach (var item in arr)
                {
                    if (item != model.Id.ToString())
                    {
                        return Content(String.Format("Giá trị cập nhật vina theo định dạng {0},{1}", model.Id, model.Id));
                    }
                }
            }
            if (model.ZING != "0")
            {
                var arr = model.ZING.Split(',');
                foreach (var item in arr)
                {
                    if (item != model.Id.ToString())
                    {
                        return Content(String.Format("Giá trị cập nhật zing theo định dạng {0},{1}", model.Id, model.Id));
                    }
                }
            }
            if (model.VCOIN != "0")
            {
                var arr = model.VCOIN.Split(',');
                foreach (var item in arr)
                {
                    if (item != model.Id.ToString())
                    {
                        return Content(String.Format("Giá trị cập nhật zing theo định dạng {0},{1}", model.Id, model.Id));
                    }
                }
            }
            int response = 0;
            CardPartnersDAO.Instance.CardPartnerUpdate(model.Id, model.VTT, model.VMS, model.VNP,model.ZING,model.VCOIN, out response);
            if (response == 1)
            {
             
                return GetList(command,model.ServiceID);
            }
            else
            {

            }
            return Content(String.Format("Cập nhật thất bại"));



        }

    }
}