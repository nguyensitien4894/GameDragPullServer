
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Warnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Controllers
{
    public class WarningController : BaseController
    {
        private int VPLimit = 500;
        private int LimitAmount = 500000;
        // GET: Warning
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult WarningShot(int type=1)
        {
            if (type == 1)
            {
                ViewBag.GridName = "card";
                ViewBag.WarmType = 1;
                ViewBag.VPLimit = VPLimit;
                ViewBag.QuotaDay = 0;
                ViewBag.LimitAmount = LimitAmount;

            }
            if (type == 2)
            {
                ViewBag.GridName = "bank";
                ViewBag.WarmType = 2;
                ViewBag.VPLimit = VPLimit;
                ViewBag.QuotaDay = 0;
                ViewBag.LimitAmount = LimitAmount;

            }
            if (type == 3)
            {
                ViewBag.GridName = "giftcode";
                ViewBag.WarmType = 3;
                ViewBag.VPLimit = VPLimit;
                ViewBag.QuotaDay = 0;
                ViewBag.LimitAmount = LimitAmount;

            }
            if (type == 5)
            {
                ViewBag.GridName = "momo";
                ViewBag.WarmType = 5;
                ViewBag.VPLimit = VPLimit;
                ViewBag.QuotaDay = 0;
                ViewBag.LimitAmount = LimitAmount;

            }
            return View();
        }
      
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, int WarningType, int VPLimit, int QuotaDay, long LimitAmount)
        {
            try
            {
                long totalRecord;
                var list = GameDAO.Instance.WarningUserRecharge(WarningType,VPLimit, QuotaDay, LimitAmount, command.Page - 1 <= 0 ? 1 : command.Page, command.PageSize, out totalRecord);
                var gridModel = new GridModel<WarningModel>
                {
                    Data = list.Select(x =>
                    {
                        var m = new WarningModel();
                        m.ServiceID = x.ServiceID;
                        m.TotalAmount = x.TotalAmount;
                        m.UserID = x.UserID;
                        m.DisplayName = x.DisplayName;
                        return m;
                    }),
                    Total = ConvertUtil.ToInt(totalRecord)
                };
                return new JsonResult { Data = gridModel };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }



        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult Details(int type)
        {
            var model = new WarningRequestModel();
            model.WarningType = type;
            model.LimitAmount = LimitAmount;
            model.QuotaDay = 0;
            model.VPLimit = VPLimit;
            model.LstType = GetTypes();
            model.LstVP = GetLstVP();
            model.LstQuata = GetLstQuata();
            model.LstLimitAmount = GetLstLimitAmount();
            return View(model);
        }

        private List<SelectListItem> GetTypes()
        {

            List<SelectListItem> LstType = new List<SelectListItem>();
            LstType.Add(new SelectListItem() { Value = "1", Text="Thẻ cào" });
            LstType.Add(new SelectListItem() { Value = "2", Text = "Bank" });
            LstType.Add(new SelectListItem() { Value = "3", Text = "Gift Code" });
            LstType.Add(new SelectListItem() { Value = "5", Text = "Momo" });
            return LstType;

        }
        private List<SelectListItem> GetLstLimitAmount()
        {
            

            List<SelectListItem> LstType = new List<SelectListItem>();
            LstType.Add(new SelectListItem() { Value = "500000", Text = "500K" });
            LstType.Add(new SelectListItem() { Value = "1000000", Text = "1M" });
            LstType.Add(new SelectListItem() { Value = "2000000", Text = "2M" });
            LstType.Add(new SelectListItem() { Value = "5000000", Text = "5M" });
            LstType.Add(new SelectListItem() { Value = "10000000", Text = "10M" });
            return LstType;

        }
        private List<SelectListItem> GetLstQuata()
        {


            List<SelectListItem> LstType = new List<SelectListItem>();
            LstType.Add(new SelectListItem() { Value = "0", Text = "0 ngày" });
            LstType.Add(new SelectListItem() { Value = "1", Text = "1 ngày" });
            LstType.Add(new SelectListItem() { Value = "2", Text = "2 ngày" });
            LstType.Add(new SelectListItem() { Value = "5", Text = "5 ngày" });
            LstType.Add(new SelectListItem() { Value = "10", Text = "10 ngày" });
            return LstType;

        }
        private List<SelectListItem> GetLstVP()
        {
        

            List<SelectListItem> LstType = new List<SelectListItem>();


            LstType.Add(new SelectListItem() { Value = "5000", Text = "5000 VP" });
            LstType.Add(new SelectListItem() { Value = "500", Text = "500 VP" });
            LstType.Add(new SelectListItem() { Value = "100", Text = "100 VP" });
            LstType.Add(new SelectListItem() { Value = "50", Text = "50 VP" });
            LstType.Add(new SelectListItem() { Value = "20", Text = "20 VP" });
            LstType.Add(new SelectListItem() { Value = "10", Text = "10 VP" });
           
            return LstType;

        }

    }
}