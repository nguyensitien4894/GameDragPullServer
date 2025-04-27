using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class ChartController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult BalanceFlow(int serviceId = 1)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != "kinhdoanh")
                return RedirectToAction("Permission", "Account");

            var lstBalance = UserDAO.Instance.GetBalanceFlow(serviceId);
            string[] lstTypeName = { "Tổng đại lý", "Giftcode TĐL", "Đại lý cấp 1", "Giftcode ĐLC1", "Giftcode ĐLC1 còn hiệu lực", "Người chơi", "Giftcode NPH còn hiệu lực", "" };
            List<BalanceFlow> lstRs = new List<BalanceFlow>();
            for (int i = 0; i < lstBalance.Count; i++)
            {
                BalanceFlow item = new BalanceFlow();
                item.BalanceName = lstTypeName[i];
                item.TotalBalance = lstBalance[i]; 
                lstRs.Add(item);
            }

            ViewBag.ServiceBox = GetServices();
            ViewBag.Data = JsonConvert.SerializeObject(lstRs, _jsonSetting);
            return View(lstRs);
        }

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
    }
}