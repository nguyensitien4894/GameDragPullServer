using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.BankSecondary;
using MsWebGame.CSKH.Models.Telecoms;
using MsWebGame.CSKH.Utils;
using Telerik.Web.Mvc;

namespace MsWebGame.CSKH.Controllers
{
    public class BankOeratorSecondaryController : BaseController
    {
        private List<string> acceptList = new List<string>() { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_CSKH_01, AppConstants.ADMINUSER.USER_CSKH_09 };
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        // GET: BankOeratorSecondary
        public ActionResult Index()
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, int serviceId = 1)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            //lay danh sách chăm sóc khách hàng
            var list = BankOeratorSecondaryDAO.Instance.GetList(0, null, serviceId);
            var gridModel = new GridModel<BankOperatorsSecondaryModel>
            {
                Data = list.Select(x =>
                {
                    var m = new BankOperatorsSecondaryModel();
                    m = Mapper.Map<BankOperatorsSecondaryModel>(x);
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

        private void PrepareModel(BankOperatorsSecondaryModel model)
        {
            //model.listStatus = new List<SelectListItem>()
            //{
            //    new SelectListItem() {Value="True",Text="Hoạt động" },
            //    new SelectListItem() {Value="False",Text="Tạm dừng" },
            //};
            ViewBag.ListStatus = new List<SelectListItem>()
            {
                new SelectListItem() {Value = "True", Text = "Hoạt động"},
                new SelectListItem() {Value = "False", Text = "Tạm dừng"},
            };
            ViewBag.ServiceBox = GetServices();
        }

        #region thêm mới telecom
        /// <summary>
        /// GET :thêm mới telecom
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create()
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }

            var model = new BankOperatorsSecondaryModel();
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
        public ActionResult Create(BankOperatorsSecondaryModel model)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            PrepareModel(model);

            var list = BankOeratorSecondaryDAO.Instance.GetList(0, null, model.serviceId);

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
                BankOeratorSecondaryDAO.Instance.TelecomOperatorsHandle(model.OperatorCode, model.OperatorName, model.Rate, model.Status, 0, AdminID, model.serviceId, out outResponse);

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


        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(long Id)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }
            if (Id <= 0)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            var entity = BankOeratorSecondaryDAO.Instance.BankOperatorsSecondaryGetByID(Id);
            if (entity == null)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            var model = new BankOperatorsSecondaryModel();
            model = Mapper.Map<BankOperatorsSecondaryModel>(entity);
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
        public ActionResult Edit(BankOperatorsSecondaryModel model)
        {
            if (!acceptList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");

            }

            PrepareModel(model);
            

            var list = BankOeratorSecondaryDAO.Instance.GetList(0, null, model.serviceId).Where(c => c.ID != model.ID);

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
            
            if (model.Rate <= 0 && model.Rate > 1)
            {
                ModelState.AddModelError("Rate", "Tỉ lệ đổi thưởng từ 0-1");
                return View(model);
            }

            
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                
                BankOeratorSecondaryDAO.Instance.TelecomOperatorsHandle(model.OperatorCode, model.OperatorName, model.Rate, model.Status, model.ID, AdminID, model.serviceId, out outResponse);
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
    }
}