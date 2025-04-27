using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MsWebGame.CSKH.App_Start
{
    public class AdminAuthorize : AuthorizeAttribute
    {

        private int isAuthen { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(Roles))
            {
                isAuthen = 0;
                return false;
            }
            var sessionId = httpContext.Session.SessionID;
            if (httpContext.Session != null && httpContext.Session["UserName"] != null && httpContext.Session["RoleCode"] != null)
            {
                string userName = httpContext.Session["UserName"].ToString();
                string RoleCode = httpContext.Session["RoleCode"].ToString();
                if (Roles != "*")
                {
                    var arrRoles = Roles.Split(',');
                    var curRoles = RoleCode.Split(',');
                    var listCommon = arrRoles.Intersect(curRoles).ToList();
                    if (listCommon != null && listCommon.Any())
                    {
                        isAuthen = 2;//đủ quyền truy cập
                        return true;
                    }
                    else
                    {
                        isAuthen = 1;//không đủ quyền truy cập
                        return false;
                    }
                }
                else
                {
                    return true; //được gán quyền = * và chỉ cần loin là được vào
                }
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (isAuthen == 0)
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }

                filterContext.Result = new JsonResult();
            }
            else
            {
                if (isAuthen == 0)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Account" }, { "Action", "Login" } });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Account" }, { "Action", "Permission" } });
                }
            }
        }
    }
}