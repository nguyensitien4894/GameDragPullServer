using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.Telecoms;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class TelecomController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, int serviceId = 1)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
            //lay danh sách chăm sóc khách hàng
            var list = TelecomOperatorsDAO.Instance.GetList(0, null, serviceId);

            var gridModel = new GridModel<TelecomOperatorModel>
            {
                Data = list.Select(x =>
                {
                    var m = new TelecomOperatorModel();
                    m = Mapper.Map<TelecomOperatorModel>(x);
                    m.serviceId = x.ServiceID;
                    return m;
                }),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #region thêm mới telecom
        /// <summary>
        /// GET :thêm mới telecom
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create()
        {
            if ((string)Session["UserName"] != "admin" && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
           
            var model = new TelecomOperatorModel();
            model.Status = true;
            PrepareModel(model);

            return View(model);
        }
        /// <summary>
        /// Post thêm mới telecom
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create(TelecomOperatorModel model)
        {
            if ((string)Session["UserName"] != "admin" && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
            PrepareModel(model);
            if (!model.Rate.HasValue)
            {
                ModelState.AddModelError("Rate", Message.OperatorRateRequired);
            }
            if (!model.ExchangeRate.HasValue)
            {
                ModelState.AddModelError("ExchangeRate", Message.OperatorExChangeRateRequired);
            }

            var list = TelecomOperatorsDAO.Instance.GetList(0, null,model.serviceId);

            if (list != null && list.Any())
            {
                if (list.Any(c => c.OperatorName.ToLower() == model.OperatorName.ToLower()))
                {
                    ModelState.AddModelError("OperatorName", Message.OperatorNameExist);
                    return View(model);
                }
                if (list.Any(c => c.OperatorName.ToLower() == model.OperatorCode.ToLower()))
                {
                    ModelState.AddModelError("OperatorCode", Message.OperatoerCodeExist);
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                TelecomOperatorsDAO.Instance.TelecomOperatorsHandle(model.OperatorCode, model.OperatorName, model.Rate.Value, model.ExchangeRate.Value, model.Status,model.ExchangeStatus, 0, AdminID,model.serviceId, out outResponse);

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

            return View(model);
        }
        #endregion

        private void PrepareModel(TelecomOperatorModel model)
        {
            model.listStatus = new List<SelectListItem>()
            {
                new SelectListItem() {Value="True",Text="Hoạt động" },
                 new SelectListItem() {Value="False",Text="Tạm dừng" },
            };
            ViewBag.ServiceBox = GetServices();
        }

        #region  edit telecom
        /// <summary>
        /// GET :edit telecom
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(long Id)
        {
            if ((string)Session["UserName"] != "admin" && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
            if (Id <= 0)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            var entity = TelecomOperatorsDAO.Instance.TelecomGetByID(Id);
            if (entity == null)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            var model = new TelecomOperatorModel();
            model = Mapper.Map<TelecomOperatorModel>(entity);
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
        public ActionResult Edit(TelecomOperatorModel model)
        {
            if ((string)Session["UserName"] != "admin" && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }

            PrepareModel(model);
            if (!model.Rate.HasValue)
            {
                ModelState.AddModelError("Rate", Message.OperatorRateRequired);
            }
            if (!model.ExchangeRate.HasValue)
            {
                ModelState.AddModelError("ExchangeRate", Message.OperatorExChangeRateRequired);
            }

            var list = TelecomOperatorsDAO.Instance.GetList(0, null,model.serviceId).Where(c => c.ID != model.ID);

            if (list != null && list.Any())
            {
                if (list.Any(c => c.OperatorName.ToLower() == model.OperatorName.ToLower()))
                {
                    ModelState.AddModelError("OperatorName", Message.OperatorNameExist);
                    return View(model);
                }
                if (list.Any(c => c.OperatorName.ToLower() == model.OperatorCode.ToLower()))
                {
                    ModelState.AddModelError("OperatorCode", Message.OperatoerCodeExist);
                    return View(model);
                }
            }
            if (!model.Rate.HasValue)
            {

                ModelState.AddModelError("Rate","Tỉ lệ đổi thưởng không thể trống");
                return View(model);
            }
            if (model.Rate <= 0 && model.Rate > 1)
            {
                ModelState.AddModelError("Rate", "Tỉ lệ đổi thưởng từ 0-1");
                return View(model);
            }

            if (!model.ExchangeRate.HasValue)
            {
                ModelState.AddModelError("Rate", "Tỉ lệ nạp thẻ từ 1-1.5");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                TelecomOperatorsDAO.Instance.TelecomOperatorsHandle(model.OperatorCode, model.OperatorName, model.Rate.Value, model.ExchangeRate.Value, model.Status,model.ExchangeStatus, model.ID, AdminID,model.serviceId, out outResponse);

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

            return View(model);
        }
        #endregion
    }
}