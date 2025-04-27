using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace MsWebGame.CSKH.Controllers
{
    public class BankOperatorController : BaseController
    {
        private List<string> acceptList = new List<string>() { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09 };
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index(string nickName = null)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            ViewBag.ServiceBox = GetServices();
         
            ViewBag.NickName = nickName;
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, int? serviceId = null, int currentPage = 1)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }


            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var list = BankOperatorDAO.Instance.BankOperatorsList(serviceId, null,null);
            var model = new GridModel<BankOperators>
            {
                Data = list,
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult Update(BankOperators model, GridCommand command)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }

            if (model.ID<=0)
            {
                return Content("Không thể để trống giá trị viettel ");
            }
            if(model.Rate>2||model.Rate<=0)
            {
                return Content("Tỉ lệ rate chỉ nằm trong khoảng 0-2");
            }
            if (model.ExchangeRate > 2 || model.ExchangeRate <= 0)
            {
                return Content("Tỉ lệ rate chỉ nằm trong khoảng 0-2");
            }
            if (String.IsNullOrEmpty(model.OperatorName))
            {
                return Content("Tên đối tác không thể để trống");
            }
            var item  = BankOperatorDAO.Instance.BankOperatorsList(null,null,model.ID);
            if (item == null|| !item.Any())
            {
                return Content("Không tìm thấy đối tác");
            }
            var firstBank = item.FirstOrDefault();
            if (firstBank == null )
            {
                return Content("Không tìm thấy đối tác");
            }
          
            
           
            int response = 0;
            BankOperatorDAO.Instance.BankOperatorsUpdate(model.ID,model.OperatorName,model.Rate,model.ExchangeRate.Value,model.Status,model.ExchangeStatus,AdminID, out response);
            if (response == 1)
            {

                return GetList(command);
            }
            else
            {

            }
            return Content(String.Format("Cập nhật thất bại"+ response));



        }


    }
}