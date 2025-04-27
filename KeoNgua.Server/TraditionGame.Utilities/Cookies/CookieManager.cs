using System;
using System.Web;
using System.Web.Security;


namespace TraditionGame.Utilities.Cookies
{
    public class CookieManager
    {
        public static void RemoveAllCookies(bool removeSessionCookie)
        {
            HttpContext.Current.Response.Cookies.Clear();

            string[] cookies = HttpContext.Current.Request.Cookies.AllKeys;
            if (cookies == null || cookies.Length == 0)
                return;

            foreach (string cookie in cookies)
            {
                //Thêm cookie đã expire nếu có config Cookie Domain trong FormsAuthentication của web.config
                if (!string.IsNullOrEmpty(FormsAuthentication.CookieDomain) && cookie.Equals(FormsAuthentication.FormsCookieName))
                {
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie(cookie)
                    {
                        Name = FormsAuthentication.FormsCookieName,
                        Domain = FormsAuthentication.CookieDomain,
                        Expires = DateTime.Now.AddYears(-1)
                    });
                }

                try
                {
                    //lấy cookie có sẵn
                    HttpCookie httpCookie = HttpContext.Current.Request.Cookies[cookie];
                    if (httpCookie != null)
                    {
                        //set expires cho cookie
                        httpCookie.Domain = HttpContext.Current.Request.Url.Host.Contains("localhost") ? null : "." + GetDomain(HttpContext.Current.Request.Url.Host);
                        httpCookie.Expires = DateTime.Now.AddYears(-1);
                        //update cookie
                        HttpContext.Current.Response.Cookies.Set(httpCookie);
                    }

                    if (removeSessionCookie)
                    {
                        HttpContext.Current.Request.Cookies.Remove(cookie);
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            }
        }

        public static string GetDomain(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return string.Empty;

            if (inputData.Equals("localhost"))
                return inputData;

            string[] ArrDomain = inputData.Split('.');
            int l = ArrDomain.Length;
            NLogManager.LogMessage("ArrDomain.Length: " + l);
            string domain = string.Empty;
            if (l == 2)
                domain = string.Format("{0}.{1}", ArrDomain[0], ArrDomain[1]);
            else
                domain = string.Format("{0}.{1}", ArrDomain[1], ArrDomain[2]);

            return domain;
        }
    }
}