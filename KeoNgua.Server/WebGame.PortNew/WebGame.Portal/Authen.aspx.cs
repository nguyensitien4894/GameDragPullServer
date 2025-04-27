using System;
using System.Web;
using System.Web.Security;

using TraditionGame.Utilities.Cookies;
using TraditionGame.Utilities;

namespace MsWebGame.Portal
{
    public partial class Authen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request.Params["logout"] != null)
                {
                    FormsAuthentication.SignOut();
                    CookieManager.RemoveAllCookies(true);
                    Session.RemoveAll();
                    Response.Redirect(HttpContext.Current.Request.Url.Scheme + "://" +  CookieManager.GetDomain(HttpContext.Current.Request.Url.Host), false);
                    return;
                }
            }
            catch (Exception exception)
            {
                NLogManager.PublishException(exception);
                Response.Redirect("~/");
            }
        }
    }
}