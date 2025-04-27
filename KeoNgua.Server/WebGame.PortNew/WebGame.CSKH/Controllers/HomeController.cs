using System.Web.Mvc;
using MsWebGame.CSKH.App_Start;

namespace MsWebGame.CSKH.Controllers
{
    public class HomeController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index()
        {
            return View();
        }
    }
}