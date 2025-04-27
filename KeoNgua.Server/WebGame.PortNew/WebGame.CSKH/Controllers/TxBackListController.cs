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
    public class TxBackListController : BaseController
    {
        private List<string>  _acceptList =new List<string>{"admin","adminref","admin_test","cskh_01","monitor_01","cskh_04_tq"};
    
        public ActionResult Index()
        {
            if (!_acceptList.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }
            ViewBag.ServiceBox = GetServices();
            return View();
        }
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, string DisplayName ,int? ServiceID)
        {
            if (!_acceptList.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }

            int totalrecord = 0;
            var lstRs = TxBackListDAO.Instance.GetList(DisplayName, ServiceID, command.Page - 1 <= 0 ? 1 : command.Page,
                AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (lstRs == null)
                lstRs = new List<TxBackList>();

            var model = new GridModel<TxBackList> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }

        [HttpPost]
    
        public dynamic Create(int ServiceID,string DisplayName)
        {
            if (!_acceptList.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }
            var accpetServiceID = new List<int> { 1, 3 };
            if (!accpetServiceID.Contains(ServiceID))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Cổng game không thể trống",
                }, JsonRequestBehavior.AllowGet);
            }
            if (String.IsNullOrEmpty(DisplayName))
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tên nhân vật không thể trống",
                }, JsonRequestBehavior.AllowGet);
            }
            //int Response = TxBackListDAO.Instance.AddBackList(DisplayName, ServiceID);
            int Response = -99;
            if (Response == 1)
            {
                return Json(new
                {
                    Code = 1,
                    Message = "Thêm mới vào backlist thành công" + Response,
                }, JsonRequestBehavior.AllowGet);

            }
            else if (Response == -1)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Tham số không hơp lệ " + Response,
                }, JsonRequestBehavior.AllowGet);
            }
            else if (Response == -2)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "KHông tồn tại user " + Response,
                }, JsonRequestBehavior.AllowGet);
            }
            else if (Response == -3)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Đã tồn tại user trong danh sách back list " + Response,
                }, JsonRequestBehavior.AllowGet);
            }else
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Có lỗi khi thu add user |" + Response,
                }, JsonRequestBehavior.AllowGet);
            }

          
        }


        [HttpPost]
      
        public dynamic Delete(long AccountID)
        {
            if (!_acceptList.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }
            var accpetServiceID = new List<int> { 1, 3 };
            if (AccountID<=0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "AccountID không thể trống",
                }, JsonRequestBehavior.AllowGet);
            }
           
            int Response = TxBackListDAO.Instance.DeleteBackList(AccountID);
            if (Response == 1)
            {
                return Json(new
                {
                    Code = 1,
                    Message = "Xóa khỏi backlist thành công" + Response,
                }, JsonRequestBehavior.AllowGet);

            }
            
            else
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Có lỗi khi thu add user |" + Response,
                }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}