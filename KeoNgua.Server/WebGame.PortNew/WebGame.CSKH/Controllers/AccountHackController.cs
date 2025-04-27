using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.AccountHack;
using MsWebGame.CSKH.Models.Cards;
using MsWebGame.CSKH.Models.Telecoms;
using MsWebGame.CSKH.Utils;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using Telerik.Web.Mvc;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.CSKH.Controllers
{
    public class AccountHackController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        // GET: AccountHack
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
        public ActionResult GetList(GridCommand command, AccountHackModel model)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
            //lay danh sách chăm sóc khách hàng
            int totalRecord = 0;
            var list = AccountHackDAO.Instance.GetList(model.AccountBankName, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<AccountHackModel>
            {
                Data = list.Select(x =>
                {
                    var m = new AccountHackModel();
                    m = Mapper.Map<AccountHackModel>(x);

                    return m;
                }),
                Total = (int)totalRecord
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
            var model = new AccountHackModel();
            model.Status = true;
            return View(model);
        }
        /// <summary>
        /// Post thêm mới telecom
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create(AccountHackModel model)
        {
            if ((string)Session["UserName"] != "admin" && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
            //if (!model.Rate.HasValue)
            //{
            //    ModelState.AddModelError("Rate", Message.OperatorRateRequired);
            //}
            //if (!model.ExchangeRate.HasValue)
            //{
            //    ModelState.AddModelError("ExchangeRate", Message.OperatorExChangeRateRequired);
            //}

            //var list = TelecomOperatorsDAO.Instance.GetList(0, null, model.serviceId);

            //if (list != null && list.Any())
            //{
            //    if (list.Any(c => c.OperatorName.ToLower() == model.OperatorName.ToLower()))
            //    {
            //        ModelState.AddModelError("OperatorName", Message.OperatorNameExist);
            //        return View(model);
            //    }
            //    if (list.Any(c => c.OperatorName.ToLower() == model.OperatorCode.ToLower()))
            //    {
            //        ModelState.AddModelError("OperatorCode", Message.OperatoerCodeExist);
            //        return View(model);
            //    }
            //}
            if (string.IsNullOrEmpty(model.AccountBankName))
            {
                ModelState.AddModelError("AccountBankName", "Chủ tài khoản không được trống");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.AccountBankNumber))
            {
                ModelState.AddModelError("AccountBankNumber", "Số tài khoản không được trống");
                return View(model);
            }

            if (!ValidateInput.ValidateStringNumber(model.AccountBankNumber))
            {
                ModelState.AddModelError("AccountBankNumber", "Số tài khoản sai định dạng");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.BankName))
            {
                ModelState.AddModelError("BankName", "Tên ngân hàng không được trống");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create AccountHack
                AccountHackDAO.Instance.AccountHackHandle(model.AccountBankName, model.AccountBankNumber, model.BankName, model.Reason, out outResponse);
                //tạo AccountHack thành công
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
            var entity = AccountHackDAO.Instance.AccountHackGetByID(Id);
            if (entity == null)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            var model = new AccountHackModel();
            model = Mapper.Map<AccountHackModel>(entity);
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpGet]
        public ActionResult Delete(long Id)
        {
            if (Id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);
            //var entity = AdminDAO.Instance.GetById(Id);
            //if (entity == null)
            //{
            //    throw new ArgumentException(Message.ParamaterInvalid);
            //}

            if (ModelState.IsValid)
            {
                int outResponse;
                AccountHackDAO.Instance.Delete(Id, out outResponse);
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.DeleteSuccess);
                }
                else
                {
                    ErrorNotification(Message.DeleteFail);
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Post thêm mới đại lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(AccountHackModel model)
        {
            if ((string)Session["UserName"] != "admin" && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");

            }
            //if (!model.Rate.HasValue)
            //{
            //    ModelState.AddModelError("Rate", Message.OperatorRateRequired);
            //}
            //if (!model.ExchangeRate.HasValue)
            //{
            //    ModelState.AddModelError("ExchangeRate", Message.OperatorExChangeRateRequired);
            //}

            //var list = TelecomOperatorsDAO.Instance.GetList(0, null, model.serviceId).Where(c => c.ID != model.ID);

            //if (list != null && list.Any())
            //{
            //    if (list.Any(c => c.OperatorName.ToLower() == model.OperatorName.ToLower()))
            //    {
            //        ModelState.AddModelError("OperatorName", Message.OperatorNameExist);
            //        return View(model);
            //    }
            //    if (list.Any(c => c.OperatorName.ToLower() == model.OperatorCode.ToLower()))
            //    {
            //        ModelState.AddModelError("OperatorCode", Message.OperatoerCodeExist);
            //        return View(model);
            //    }
            //}
            //if (!model.Rate.HasValue)
            //{

            //    ModelState.AddModelError("Rate", "Tỉ lệ đổi thưởng không thể trống");
            //    return View(model);
            //}
            //if (model.Rate <= 0 && model.Rate > 1)
            //{
            //    ModelState.AddModelError("Rate", "Tỉ lệ đổi thưởng từ 0-1");
            //    return View(model);
            //}

            if (string.IsNullOrEmpty(model.AccountBankName))
            {
                ModelState.AddModelError("AccountBankName", "Chủ tài khoản không được trống");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.AccountBankNumber))
            {
                ModelState.AddModelError("AccountBankNumber", "Số tài khoản không được trống");
                return View(model);
            }

            if (!ValidateInput.ValidateStringNumber(model.AccountBankNumber))
            {
                ModelState.AddModelError("AccountBankNumber", "Số tài khoản sai định dạng");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.BankName))
            {
                ModelState.AddModelError("BankName", "Tên ngân hàng không được trống");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create Account hack
                AccountHackDAO.Instance.AccountHackEdit(model.ID,model.AccountBankName, model.AccountBankNumber, model.BankName, model.Reason, out outResponse);

                //tạo account hack thành công
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = "Edit lỗi";
                    ErrorNotification(msg);
                }
            }

            return View(model);
        }
        #endregion
    }
}