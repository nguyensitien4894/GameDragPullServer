using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.IpAddress;

namespace MsWebGame.CSKH.App_Start
{
    public class AllowedIPAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Retrieve user's IP
            string usersIpAddress = IPAddressHelper.GetClientIP();
            //NLogManager.LogMessage("UserIP:" + usersIpAddress);
            if (!checkIp(usersIpAddress))
            {
                //return 403 Forbidden HTTP code
                filterContext.Result = new HttpStatusCodeResult(403);
            }

            base.OnActionExecuting(filterContext);
        }

        public static bool checkIp(string usersIpAddress)
        {
            //get allowedIps Setting from Web.Config file and remove whitespaces from int
            if (string.IsNullOrEmpty(usersIpAddress)) return false;
            var arr = usersIpAddress.Split(',');
            if (arr == null&&!arr.Any()) return false;
            string allowedIps = ConfigurationManager.AppSettings["allowedIPs"].Replace(" ", "").Trim();
            if (allowedIps=="*")
            {
                return true;
            }
            var userId = arr[0];

            //convert allowedIPs string to table by exploding string with ';' delimiter
            string[] ips = allowedIps.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //iterate ips table
            foreach (var ip in ips)
            {
                if (ip.Equals(userId))
                    return true; //return true confirming that user's address is allowed
            }

            //if we get to this point, that means that user's address is not allowed, therefore returning false
            return false;

        }
    }
}