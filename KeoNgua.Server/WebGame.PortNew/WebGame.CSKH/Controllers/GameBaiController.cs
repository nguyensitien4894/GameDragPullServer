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

namespace MsWebGame.CSKH.Controllers
{
    public class GameBaiController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult Index()
        {
            ViewBag.ServiceBox = GetServices();
            ViewBag.GameBox = InfoHandler.Instance.GetGameBai();
            ViewBag.SearchTypes = InfoHandler.Instance.GetGameBaiSearch();
            return View();
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, string TextSearch, int GameID, int? ServiceID, int SearchBy, DateTime FromDate, DateTime ToDate)
        {
           

            int totalrecord = 0;
            var lstRs = GameBaiDAO.Instance.GetList(TextSearch,  GameID,  ServiceID, SearchBy,  FromDate,  ToDate, command.Page - 1 <= 0 ? 1 : command.Page,
                AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (lstRs == null)
                lstRs = new List<GameBai>();

            var model = new GridModel<GameBai> { Data = lstRs, Total = totalrecord };
            return new JsonResult { Data = model };
        }
    }
}