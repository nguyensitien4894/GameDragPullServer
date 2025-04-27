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
using Telerik.Web.Mvc.UI;

namespace MsWebGame.CSKH.Controllers
{
    public class CampaignController : BaseController
    {
        private string GiftAll = "Tạo chiến dịch cho event";
        private List<DropDownItem> GetCustomerGate()
        {
            var dropItems = new List<DropDownItem>();
            dropItems.Add(new DropDownItem() { Value = "0", Text = GiftAll });
            dropItems.AddRange(GetServices());
            return dropItems;
        }

        // GET: Compaing
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Campaign()
        {

            ViewBag.ServiceBox = GetCustomerGate();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCampaign(GridCommand command, string campaignName, string campaignCode, bool? status, string effectDate, string expiredDate, int serviceId = -1)
        {
            if (string.IsNullOrEmpty(campaignName))
                campaignName = null;

            if (!string.IsNullOrEmpty(campaignCode))
                campaignCode = null;

            DateTime? effect = null;
            if (!string.IsNullOrEmpty(effectDate))
                effect = DateTime.Parse(effectDate);

            DateTime? expired = null;
            if (!string.IsNullOrEmpty(expiredDate))
                expired = DateTime.Parse(expiredDate);

            var list = ParamConfigDAO.Instance.GetListGameCampaign(0, campaignName, campaignCode, status, effect, expired, serviceId);
            if (list == null)
                list = new List<GameCampaign>();
            var listGate = GetServices();
            var model = new GridModel<GameCampaign>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CampaignAdd()
        {
            ViewBag.ServiceBox = GetCustomerGate();
            GameCampaign model = new GameCampaign();
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CampaignAdd(string campaignName, string campaignCode, DateTime effectDate, DateTime expiredDate, bool status, string description, int ServiceID)
        {
            if (string.IsNullOrEmpty(campaignName) || string.IsNullOrEmpty(campaignCode) || string.IsNullOrEmpty(description))
                return View();
            ViewBag.ServiceBox = GetCustomerGate();
            GameCampaign model = new GameCampaign();
            int res = ParamConfigDAO.Instance.AddOrUpdateGameCampaign(0, campaignName, campaignCode, 0,
                0, effectDate, expiredDate, status, description, AdminID, ServiceID);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.InsertSuccess);
                return View();
            }
            else if (res == -17)
            {
                ErrorNotification(Message.ERR_CAMPAIGN_EXISTED);
            }
            else if (res == -18)
            {
                ErrorNotification(Message.ERR_CAMPAIGN_INVALID);
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
            }

            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CampaignEdit(long id)
        {
            ViewBag.ServiceBox = GetCustomerGate();
            var model = ParamConfigDAO.Instance.CampaignGetByID(id);

            if (model == null)
                RedirectToAction("Campaign");



            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CampaignEdit(GameCampaign model)
        {
            if (model == null)
                throw new ArgumentException(Message.ParamaterInvalid);
            ViewBag.ServiceBox = GetCustomerGate();
            if (ModelState.IsValid)
            {
                int res = ParamConfigDAO.Instance.AddOrUpdateGameCampaign(model.CampaignID, model.CampaignName, model.CampaignCode, 0,
                    0, model.EffectDate, model.ExpiredDate, model.Status, model.Description, AdminID, model.ServiceID);
                if (res == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Campaign");
                }
                else if (res == -17)
                {
                    ErrorNotification(Message.ERR_CAMPAIGN_EXISTED);
                }
                else if (res == -18)
                {
                    ErrorNotification(Message.ERR_CAMPAIGN_INVALID);
                }
                else
                {
                    ErrorNotification(Message.SystemProccessing);
                }
            }

            return View(model);
        }
    }
}