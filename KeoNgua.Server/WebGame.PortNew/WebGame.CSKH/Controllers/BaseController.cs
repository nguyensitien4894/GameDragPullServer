using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using static MsWebGame.CSKH.Utils.AppConstants;
using System.Configuration;
using MsWebGame.CSKH.Database.DAO;
using System.Linq;
using Telerik.Web.Mvc.UI;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.DNA;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{

    [AllowedIP]
    public class BaseController : Controller
    {
        protected const string ADMIN_ALL_ROLE = "ADMIN,MONITOR,CALLCENTER,VIEW,MARKETING,MARKETING_BB";
        protected const string ADMIN_ROLE = "ADMIN";
        protected const string ADMIN_VIEW_ROLE = "ADMIN,VIEW";
        protected const string ADMIN_MARKETING_VIEW_ROLE = "ADMIN,VIEW,MARKETING";
        protected const string VIEW_ROLE = "VIEW";
        protected const string ADMIN_MONITOR_ROLE = "ADMIN,MONITOR";
        protected const string ADMIN_MONITOR_VIEW_ROLE = "ADMIN,MONITOR,VIEW";
        protected const string ADMIN_CALLCENTER_ROLE = "ADMIN,CALLCENTER";
        protected const string ADMIN_MARKETTING_BB_ROLE = "ADMIN,MARKETING_BB";
   
        protected int DNA_PLATFORM = ConvertUtil.ToInt(ConfigurationManager.AppSettings["DNA_PLATFORM"].ToString());//1 dev,2 deploy
        protected const string ADMIN_MARKETING_VIEW_BB_ROLE = "ADMIN,VIEW,MARKETING,MARKETING_BB";
        protected const string ADMIN_MONITOR_MARKETING_VIEW_BB_ROLE = "ADMIN,VIEW,MARKETING,MARKETING_BB,MONITOR";
        protected int? GetServiceIDBRole
        {
            get
            {
                var gate2Roles = new List<string> { "MARKETING_BB" };
                var gate3Roles = new List<string> { "MARKETING_NH" };
                var gate1Roles = new List<string> { "MARKETING" };
                if (gate2Roles.Contains(AdminRoleName)) return 2;
                if (gate1Roles.Contains(AdminRoleName)) return 1;
                if (gate3Roles.Contains(AdminRoleName)) return 3;
                return null;

            }

        }
        protected string AdminDisplayName
        {
            get
            {
                try
                {
                    return Session["DisplayName"].ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        protected int GetCardCodeIndex(String cardOperatorCode)
        {
            try
            {
                if (cardOperatorCode.Contains("VTT")) return 1;
                else if (cardOperatorCode.Contains("VNP")) return 2;
                else if (cardOperatorCode.Contains("ZING")) return 8;
                else if (cardOperatorCode.Contains("VCOIN")) return 9;
                else return 3;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        protected bool SendDNA(int ServiceID,long accountId, int transType, long amount, long amountGame)
        {
            try
            {
                var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                dnaHelper.SendTransactionPURCHASE(accountId, transType, null, amount, amountGame);
                ProfileLogger.LogMessage(String.Format("accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}", accountId, transType, amount, amountGame));
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;

            }

        }
        protected long AdminID
        {
            get
            {
                try
                {
                    return Convert.ToInt64(Session["AdminID"].ToString());
                }
                catch
                {
                    return 0;
                }
            }
        }
        protected string AdminRoleName
        {
            get
            {
                try
                {
                    return (Session["RoleCode"].ToString());
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        protected string AdminAccountName
        {
            get
            {
                try
                {
                    return Session["UserName"].ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        protected int ServiceID
        {
            get
            {
                try
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["ServiceID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        protected bool CheckPermissionUser(string currUser)
        {
            try
            {
                string[] _arrPermission = { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_ADMINTEST };
                bool isExist = Array.Exists(_arrPermission, x => x == currUser);
                return isExist;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected List<DropDownItem> GetServices()
        {
            try
            {
                var list = GameDAO.Instance.GetSerivces();
                return list.Select(c => new DropDownItem()
                {
                    Value = c.ServiceID.ToString(),
                    Text = c.ServiceName,

                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected bool CheckPermissionUser(string[] _arrPermission, string currUser)
        {
            try
            {
                bool isExist = Array.Exists(_arrPermission, x => x == currUser);
                return isExist;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected virtual void SuccessNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Success, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void ErrorNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Error, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        protected virtual void ErrorNotification(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
        {
            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("webgame.notifications.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }
    }
}