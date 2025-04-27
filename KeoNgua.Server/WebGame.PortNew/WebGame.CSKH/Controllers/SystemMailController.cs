using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.SystemMails;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class SystemMailController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command)
        {
            int totalRecord = 0;
            var list = SystemMailDAO.Instance.GetSystemMailList(0, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);

            var gridModel = new GridModel<SystemMailModel>
            {
                Data = list.Select(x =>
                {
                    var m = new SystemMailModel();
                    m = Mapper.Map<SystemMailModel>(x);
                    return m;
                }),
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #region thêm mới maill
        /// <summary>
        /// GET :thêm mới đại lí
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create()
        {
            var model = new SystemMailModel();
            PrepareModel(model);
            return View(model);
        }
        /// <summary>
        /// Post thêm mới đại lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create(SystemMailModel model)
        {
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                SystemMailDAO.Instance.SendMail(model.Title, model.Content, out outResponse);
                //tạo agency thành công
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = MessageConvetor.MsgAgencyCreate.GetMessage(outResponse);
                    ErrorNotification(msg);
                }
            }
            PrepareModel(model);
            return View(model);
        }
        #endregion

        #region cập nhật mail
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(long Id)
        {
            if (Id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);
            int totalRecord = 0;
            var list = SystemMailDAO.Instance.GetSystemMailList(0, 1,Int16.MaxValue, out totalRecord);
            var entity = list.FirstOrDefault();
            if (entity == null)
            {
                throw new ArgumentException(Message.ParamaterInvalid);
            }
          
            var model = new SystemMailModel();
            //sử dụng autor mapper để mapp object (nhanh hơn )
            model = Mapper.Map<SystemMailModel>(entity);
        
            PrepareModel(model);
            return View(model);
        }
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit([Bind(Exclude = "AccountName")]SystemMailModel model)
        {
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                SystemMailDAO.Instance.SystemMailUpdate(model.ID,model.Title,model.Content,model.Status.Value, out outResponse);

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
        private void PrepareModel(SystemMailModel model)
        {
            model.listStatus = new List<SelectListItem>()
            {
                new SelectListItem() {Value="True",Text="Hoạt động" },
                new SelectListItem() {Value="False",Text="Tạm dừng" },
            };
        }
        #endregion
    }
}