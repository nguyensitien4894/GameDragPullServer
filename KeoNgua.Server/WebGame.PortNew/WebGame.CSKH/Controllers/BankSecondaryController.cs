using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.BankSecondary;
using MsWebGame.CSKH.Models.Cards;
using MsWebGame.CSKH.Utils;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Controllers
{
    public class BankSecondaryController : BaseController
    {
        // GET: BankSecondary
        public ActionResult Index()
        {
            BankSecondaryModel model = new BankSecondaryModel();
            PrepareListModel(model);
            return View(model);
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, BankSecondaryModel model)
        {
            //lay danh sách chăm sóc khách hàng
            long totalRecord = 0;
            var list = BankSecondaryDAO.Instance.GetList(0, model.BankOperatorsSecondaryID, model.BankNumber, model.BankName, model.Status, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, model.ServiceID, out totalRecord);
            //var listPartners = GetPartners(model.ServiceID);
            var gridModel = new GridModel<BankSecondaryModel>
            {
                Data = list.Select(x =>
                {
                    var m = new BankSecondaryModel();
                    m = Mapper.Map<BankSecondaryModel>(x);

                    return m;
                }),
                Total = (int)totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        private void PrepareListModel(BankSecondaryModel model)
        {
            var list = GetServices();
            ViewBag.ServiceBox = list;
            ViewBag.listBank = GetListBankOperator(1);
            model.ServiceID = ConvertUtil.ToInt(list.FirstOrDefault().Value);
        }

        private List<SelectListItem> GetListBankOperator(int serviceID)
        {
            var list = BankOeratorSecondaryDAO.Instance.GetList(0, null, serviceID).Select(c => new SelectListItem()
            {
                Value = c.ID.ToString(),
                Text = c.OperatorName,
            }).ToList();
            return list;
        }

        public ActionResult GetPartnersAndTeleByServiceID(int ServiceID)
        {
            var listTelecom = GetListBankOperator(ServiceID);
            return Json(new { listTelecom }, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create()
        {
            var model = new BankSecondaryModel();
            model.Status = true;
            PrepareListModel(model);
            return View(model);
        }
        /// <summary>
        /// Post thêm mới telecom
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create(BankSecondaryModel model)
        {
            PrepareListModel(model);

            long totalRecored = 0;
            var list = BankSecondaryDAO.Instance.GetList(0, 0, null, null, null, 1, Int32.MaxValue, model.ServiceID, out totalRecored);

            if (list != null && list.Any())
            {
                if (list.Any(c => c.BankNumber.ToLower() == model.BankNumber.ToLower()))
                {
                    ModelState.AddModelError("BankNumber", Message.BankCodeExist);
                }
            }

            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                BankSecondaryDAO.Instance.Cards_Handle(model.BankNumber, model.BankName, (long)model.BankOperatorsSecondaryID,  model.Status, AdminID, model.ServiceID, out outResponse);

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
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(long Id)
        {
            if (Id <= 0)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            long totalRecord = 0;
            var entity = BankSecondaryDAO.Instance.BanksByID(Id);
            if (entity == null)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            var model = new BankSecondaryModel();
            model = Mapper.Map<BankSecondaryModel>(entity);
            
            PrepareListModel(model);
            return View(model);
        }
        /// <summary>
        /// Post thêm mới đại lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(BankSecondaryModel model)
        {
            PrepareListModel(model);
            

            long totalRecord = 0;

            var list = BankSecondaryDAO.Instance.GetList(0, 0, null, null, null, 1, Int16.MaxValue, model.ServiceID, out totalRecord).Where(c => c.ID != model.ID);

            if (list != null && list.Any())
            {
                if (list.Any(c => c.BankNumber.ToLower() == model.BankNumber.ToLower()))
                {
                    ModelState.AddModelError("BankNumber", Message.BankCodeExist);
                }
            }

            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                BankSecondaryDAO.Instance.Cards_Handle_Edit(model.BankNumber, model.BankName, (long)model.BankOperatorsSecondaryID, model.Status, AdminID, model.ServiceID, out outResponse,model.ID);
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