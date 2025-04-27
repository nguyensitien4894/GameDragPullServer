using System.Web;
using System.Web.Http;

namespace MsWebGame.SafeOtp.Controllers
{
    public class BaseApiController : ApiController
    {
        protected int PageSize = 30;
        protected int OTPSAFE_LENGTH = 5;
        protected int OTPMSG_LENGTH = 7;
        protected HttpContext _Context;
        public BaseApiController()
        {

        }
        protected string AddZeroToPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return string.Empty;
            phone = phone.Trim();
            if (!phone.StartsWith("0")) return string.Format("0{0}", phone);
            return phone;
        }

    }
}
