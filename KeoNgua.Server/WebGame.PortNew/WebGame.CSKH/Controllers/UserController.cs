using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Controllers
{
    public class UserController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TransfigureAgency()
        {
            if (!CheckPermissionUser(AdminAccountName))
                return RedirectToAction("Permission", "Account");

            ViewBag.Message = ViewBag.Message = Session["TransfigureAgency"] != null ? Session["TransfigureAgency"].ToString() : string.Empty;
            Session["TransfigureAgency"] = null;
            ViewBag.ServiceBox = GetServices();
            ParsTransfigureAgency model = new ParsTransfigureAgency();
            model.serviceId = 1;
            return View(model);
        }
        public ActionResult Balances()
        {
            //Session.RemoveAll();



            var adminID = AdminID;
            string balance = string.Empty;
            var admin = AdminDAO.Instance.GetById(adminID);
            if (admin != null) balance = admin.Wallet.HasValue?admin.Wallet.Value.LongToMoneyFormat():string.Empty;
            return PartialView("Balances", balance);
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult TransfigureAgency(ParsTransfigureAgency input)
        {
            if (!CheckPermissionUser(AdminAccountName))
                return RedirectToAction("Permission", "Account");

            ViewBag.ServiceBox = GetServices();
            if (string.IsNullOrEmpty(input.nickName) || string.IsNullOrEmpty(input.fullName) || string.IsNullOrEmpty(input.areaName)
                || string.IsNullOrEmpty(input.phoneDisplay) || input.orderNum < 1 || input.orderNum > 200)
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                return View(input);
            }

            //get user by nickname
            var accInfo = UserDAO.Instance.GetAccountByNickName(input.nickName, input.serviceId);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            input.userId = accInfo.AccountID;
            //User transfigure to agency
            var response = UserDAO.Instance.TransfigureAgency(input);
            NLogManager.LogMessage(string.Format("NickName:{0} | Response:{1}", input.nickName, response));
            string msgCore = string.Format("Chuyển đổi {0} {1}!", input.nickName, response == 1 ? "thành công" : "thất bại");
            Session["TransfigureAgency"] = msgCore;
            return RedirectToAction("TransfigureAgency");
        }

        [AdminAuthorize(Roles = ADMIN_MARKETTING_BB_ROLE)]
        public ActionResult GenerateNewUser()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MARKETTING_BB_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetGenerateNewUser(GridCommand command, string nickName, DateTime? fromDate, DateTime? toDate, int serviceId)
        {
            if (string.IsNullOrEmpty(nickName))
                nickName = null;

            if (fromDate == null)
                fromDate = DateTime.Today;

            if (toDate == null)
                toDate = DateTime.Now;

            if(serviceId < 1)
                serviceId = 1;

            int totalRecord = 0;
            var lstRs = PrivilegeDAO.Instance.GetReportVP( serviceId);
            if (lstRs == null)
                lstRs = new List<ReportVP>();

            var model = new GridModel<ReportVP> { Data = lstRs, Total = totalRecord };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TopTransferToTotalCharge()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTopTransferToTotalCharge(GridCommand command, DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null)
                fromDate = DateTime.Today;

            if (toDate == null)
                toDate = DateTime.Now;

            int totalRecord = 0;
            var lstRs = PrivilegeDAO.Instance.GetReportVP( ServiceID);
            if (lstRs == null)
                lstRs = new List<ReportVP>();

            var model = new GridModel<ReportVP> { Data = lstRs, Total = totalRecord };
            return new JsonResult { Data = model };
        }

        [HttpPost]
        public dynamic UpdateUserToBlacklist(long userId, int status)
        {
            

            if (userId < 1 || (status < 0 && status > 1))
                return Json(new { Response = -1, Msg = "Dữ liệu đầu vào không hợp lệ!" });

            string description = string.Format("{0} tài khoản {1} {2} blacklist", status == 1 ? "Cập nhật" : "Xóa", userId, status == 1 ? "vào" : "khỏi");
            int response = UserDAO.Instance.UpdateUserBlackList(userId, status, description, AdminID);
            string msg = string.Empty;
            msg = string.Format("{0} tài khoản {1} {2} blacklist {3}", status == 1 ? "Cập nhật" : "Xóa",
                userId, status == 1 ? "vào" : "khỏi", response == 1 ? "thành công" : "thất bại");
            return Json(new { Response = response, Msg = msg });
        }


        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public dynamic RemovePhoneOtp(long userId,int ServiceID)
        {
            //if (!CheckPermissionUser(new string[] { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, "kinhdoanh_123A", AppConstants.ADMINUSER.USER_ADMINTEST }, AdminAccountName))
            //    return RedirectToAction("Permission", "Account");

            var account = AccountProfileDAO.Instance.GetAccountInfor(userId, null, null, ServiceID);
            if (account == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Msg = "Tài khoản không tồn tại",
                });
            }
            if (account == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Msg = ErrorMsg.PhoneNotRegister,
                });
            }
           
                int rs = AccountDAO.Instance.DeletePhone(userId,account.ServiceID);
                if (rs > 0)
                {

                    return Json(new
                    {
                        ResponseCode = rs,
                        Msg = ErrorMsg.UpdateSuccess
                    });
                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = rs,
                        Msg = ErrorMsg.UpdateFail
                    });
                }
           



        }
    }
}