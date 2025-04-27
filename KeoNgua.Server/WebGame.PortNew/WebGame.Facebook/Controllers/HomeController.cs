
using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MsTraditionGame.Utilities.Cookies;
using MsTraditionGame.Utilities.Log;
using MsTraditionGame.Utilities.Session;
using MsWebGame.Facebook.Helpers;
using static MsWebGame.Facebook.Helpers.LoginApi;

using System.Web.Http;
using TraditionGame.Utilities.Security;

namespace MsWebGame.Facebook.Controllers
{
    public class HomeController : Controller
    {
        private string KeyRefer = "QWWRWRhahajfhgkdk00029292828282jdjdueieugd,k@#$%%";
        //public ActionResult Index()
        //{
        //    var facebookAppId = ConfigurationManager.AppSettings["FacebookAppId"].ToString();
        //    var facebookAppSecret = ConfigurationManager.AppSettings["FacebookAppSecret"].ToString();
        //    var facebookRedirectUrl = ConfigurationManager.AppSettings["FacebookRedirectUrl"].ToString();
        //    var facebookScope = ConfigurationManager.AppSettings["FacebookScope"].ToString();
        //    if (facebookAppId != null && facebookAppSecret != null && facebookRedirectUrl != null && facebookScope != null)
        //    {
        //        var url = string.Format(@"https://www.facebook.com/dialog/oauth/?client_id={0}&redirect_uri={1}&scope={2}", facebookAppId, facebookRedirectUrl, facebookScope);
        //        Response.Redirect(url, true);
        //    }
        //    return this.RedirectToAction("Index");

        //}
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Index3(string code)
        {
           
            //user denied permissions on Facebook.   
            if (Request["error_reason"] == "user_denied")
            {
                //this is not implemented. For reference only.  
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Error = "There was an error while loggin into Facebook. Please try again later.";
                return RedirectToAction("Login", "Account");
            }
            var state = Request["state"];
            var Url = string.Empty;
            if (!String.IsNullOrEmpty(state))
            {
                state = state.Replace("{\"{ ", "");
                state = state.Replace("}\"}", "");
                state = state.Trim();
                var arr = state.Split(',');
               
                if (arr.Any() && arr.Length >= 3)
                {
                    Url = arr[2].ToString();
                    Url = Url.Replace("sgR=", "").Trim();
                }
            }
            ViewBag.UrlRegister = Url;
            
            var client = new FacebookClient();  // Make the Get request to the facebook
            dynamic result = client.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FacebookAppId"].ToString(),
                client_secret = ConfigurationManager.AppSettings["FacebookAppSecret"].ToString(),
                redirect_uri = ConfigurationManager.AppSettings["FacebookRedirectUrl"].ToString(),
                code = code
            });
           var token = result.access_token as string;
            ViewBag.Redirect = ConfigurationManager.AppSettings["RedirectUrl"].ToString();
            ViewBag.PortalURl = ConfigurationManager.AppSettings["PORTAL_URL"].ToString();
            
            ViewBag.Token = token;
            return View();
        }
      
        public ActionResult FbLogin(string refer)
        {
            var url = ConfigurationManager.AppSettings["FacebookRedirectUrl"].ToString();
          
            var singRefer = Security.TripleDESEncrypt(refer, KeyRefer);
            var client_id = ConfigurationManager.AppSettings["FacebookAppId"].ToString();
            var guid = Guid.NewGuid().ToString();
            var redirectUrl=String.Format("{0}", "https://www.facebook.com/v3.2/dialog/oauth?client_id="+ client_id + "&redirect_uri="+ url + "&state={\"{st=ew0oejslhklekehshsh,ds=353xxe93h3938hkhiqwe92n53,sgR=" + refer + "}\"}");
            return RedirectPermanent(redirectUrl);
        }

        public ActionResult Index(string code)
        {

            //user denied permissions on Facebook.   
            if (Request["error_reason"] == "user_denied")
            {
                //this is not implemented. For reference only.  
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Error = "There was an error while loggin into Facebook. Please try again later.";
                return RedirectToAction("Login", "Account");
            }
            var state = Request["state"];
            var Url = string.Empty;
            if (!String.IsNullOrEmpty(state))
            {
                state = state.Replace("{\"{ ", "");
                state = state.Replace("}\"}", "");
                state = state.Trim();
                var arr = state.Split(',');

                if (arr.Any() && arr.Length >= 3)
                {
                    Url = arr[2].ToString();
                    Url = Url.Replace("sgR=", "").Trim();
                }
            }
            ViewBag.UrlRegister = Url;

            var client = new FacebookClient();  // Make the Get request to the facebook
            dynamic result = client.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FacebookAppId"].ToString(),
                client_secret = ConfigurationManager.AppSettings["FacebookAppSecret"].ToString(),
                redirect_uri = ConfigurationManager.AppSettings["FacebookRedirectUrl"].ToString(),
                code = code
            });
            var token = result.access_token as string;
           
            ViewBag.Redirect = ConfigurationManager.AppSettings["RedirectUrl"].ToString();
            ViewBag.PortalURl = ConfigurationManager.AppSettings["PORTAL_URL"].ToString();
            
            ViewBag.OtpLoginUrl = ConfigurationManager.AppSettings["GET_LOGIN_OTP"].ToString();
            ViewBag.Token = token;
            return View();
        }


    }
}