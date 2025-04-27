using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.Mails;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class MailController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index()
        {
            MailListModel model = new MailListModel();
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, MailListModel model)
        {
            int totalRecord = 0;
            var list = MailDAO.Instance.GetList(0, model.UserName, model.FromDate, model.ToDate, model.ServiceID, command.Page - 1 <= 0 ? 1 : command.Page,
                AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<MailModel>
            {
                Data = list.Select(x =>
                {
                    var m = new MailModel();
                    m = Mapper.Map<MailModel>(x);
                    return m;
                }),
                Total = totalRecord
            };
            return new JsonResult { Data = gridModel };
        }

        #region thêm mới mail
        /// <summary>
        /// GET :thêm mới mail
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Create()
        {
            ViewBag.ServiceBox = GetServices();
            var model = new MailModel();
            PrepareModel(model);
            model.ServiceID = 1;
            return View(model);
        }

        /// <summary>
        /// Post thêm mới mail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Create(MailModel model)
        {
            var user = UserDAO.Instance.GetAccountByNickName(model.RecevierNickname, model.ServiceID);
            if (user == null)
                ModelState.AddModelError("RecevierNickname", Message.AccountInvalid);

            if (ModelState.IsValid)
            {
                //create agency
                int response = MailDAO.Instance.SystemSendMailToUser(AdminID, user.AccountID, user.AccountName, model.Title, model.Content, 1, DateTime.Now, model.ServiceID);
                //tạo agency thành công
                if (response == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.SendMailSuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = "Gửi mail thất bại";
                    ErrorNotification(msg);
                }
            }
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }
        #endregion

        #region chỉnh sửa mail
        /// <summary>
        /// GET :thêm mới đại lí
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Edit(long Id)
        {
            if (Id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);
            int totalRecord = 0;
            var list = MailDAO.Instance.GetList(Id, null, null, null, 0, 1, 1, out totalRecord);
            var entity = list.FirstOrDefault();
            if (entity == null)
            {
                throw new ArgumentException(Message.ParamaterInvalid);
            }

            var model = new MailModel();
            //sử dụng autor mapper để mapp object (nhanh hơn )
            model = Mapper.Map<MailModel>(entity);
            PrepareModel(model);
            return View(model);
        }

        /// <summary>
        /// Post thêm mới đại lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Edit(MailModel model)
        {
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                MailDAO.Instance.UpdateMail(model.ID, model.ID, model.Title, model.Content, model.Status.Value, out outResponse);

                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = MessageConvetor.MsgAgencyEdit.GetMessage(outResponse);
                    ErrorNotification(msg);
                }
            }
            PrepareModel(model);
            return View(model);
        }
        #endregion

        #region helper
        private void PrepareModel(MailModel model)
        {
            model.listStatus = new List<SelectListItem>()
            {
                new SelectListItem() {Value="1",Text="Chưa đọc" },
                new SelectListItem() {Value="2",Text="Đã đọc" },
                new SelectListItem() {Value="-1",Text="Xóa" },
            };
        }
        #endregion
    }
}