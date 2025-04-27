using System;
using System.Configuration;
using System.Threading;
using System.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Security;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    public class PhatLocController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TransferToUser()
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != "kinhdoanh_123A")
                return RedirectToAction("Permission", "Account");

            ParsPhatLocTransfer model = new ParsPhatLocTransfer();
            ViewBag.Message = Session["BotTransferToUser"] != null ? Session["BotTransferToUser"].ToString() : string.Empty;
            Session["BotTransferToUser"] = null;
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TransferToUser(ParsPhatLocTransfer input)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != "kinhdoanh_123A")
                return RedirectToAction("Permission", "Account");

            ViewBag.ServiceBox = GetServices();
            if (string.IsNullOrEmpty(input.phatLocName) || string.IsNullOrEmpty(input.receiverNameList) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000!";
                return View(input);
            }

            if (iamount > 20000)
            {
                ViewBag.Message = "Số tiền chuyển không hợp lệ!";
                return View(input);
            }
            input.amountNum = iamount;

            //Get PhatLocId
            var plInfo = PhatLocDAO.Instance.GetUsersCheckBot(input.phatLocName);
            if (plInfo == null || plInfo.AccountID >= 0)
            {
                ViewBag.Message = "Tài khoản phát lộc không tồn tại";
                return View(input);
            }
            input.phatLocId = plInfo.AccountID;

            //cut receiverNameList
            string receiverNameLst = StringUtil.RemoveLastStr(input.receiverNameList);
            string[] ArrReceiverName = receiverNameLst.Split(',');

            string nickNameSuccess = string.Empty;
            string nickNameError = string.Empty;
            int response = 0;
            long transId = 0;
            long wallet = 0;

            var len = ArrReceiverName.Length;
            if (len > 10)
                len = 10;

            for (int i = 0; i < len; i++)
            {
                Thread.Sleep(500);
                //Get ReceiverId
                var accInfo = UserDAO.Instance.GetAccountByNickName(ArrReceiverName[i], input.serviceId);
                if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
                {
                    ViewBag.Message = "Tài khoản không tồn tại!";
                    return View(input);
                }

                if (accInfo.AccountStatus != 1)
                {
                    ViewBag.Message = "Tài khoản không hợp lệ!";
                    return View(input);
                }
                input.receiverId = accInfo.AccountID;

                //bot transfer to user
                transId = 0;
                wallet = 0;
                response = 0;
                response = PhatLocDAO.Instance.TransferToUser(input, out transId, out wallet);
                NLogManager.LogMessage(string.Format("BotTransferToUser - PhatLocName:{0} | ReceiverName:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                    input.phatLocName, input.receiverName, iamount, input.note, transId, response));
                if (response > 0)
                    nickNameSuccess += ArrReceiverName[i] + ",";
                else
                    nickNameError += ArrReceiverName[i] + ",";
            }

            string msgSuccess = string.Empty;
            if (!string.IsNullOrEmpty(nickNameSuccess))
                msgSuccess = string.Format("Chuyển khoản cho tài khoản {0} thành công.", StringUtil.RemoveLastStr(nickNameSuccess));

            string msgError = string.Empty;
            if (!string.IsNullOrEmpty(nickNameError))
                msgError = string.Format("Chuyển khoản cho tài khoản {0} thất bại!", StringUtil.RemoveLastStr(nickNameError));

            string msg = string.Format("{0} {1}", msgSuccess, msgError);
            Session["BotTransferToUser"] = msg;
            return RedirectToAction("BotTransferToUser");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TestTransferToUser()
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != "kinhdoanh_123A")
                return RedirectToAction("Permission", "Account");

            ParsPhatLocTransfer model = new ParsPhatLocTransfer();
            ViewBag.Message = Session["TestTransferToUser"] != null ? Session["TestTransferToUser"].ToString() : string.Empty;
            Session["TestTransferToUser"] = null;
            var isSuccess = ViewBag.Message.IndexOf("thành công") > -1;
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TestTransferToUser(ParsPhatLocTransfer input)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != "kinhdoanh_123A")
                return RedirectToAction("Permission", "Account");

            ViewBag.ServiceBox = GetServices();
            if (string.IsNullOrEmpty(input.phatLocName) || string.IsNullOrEmpty(input.receiverNameList) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000!";
                return View(input);
            }

            if (iamount > 20000)
            {
                ViewBag.Message = "Số tiền chuyển không hợp lệ!";
                return View(input);
            }
            input.amountNum = iamount;
            //Get PhatLocId
            var plInfo = PhatLocDAO.Instance.GetUsersCheckTest(input.phatLocName, input.serviceId);
            if (plInfo == null)
            {
                ViewBag.Message = "Tài khoản phát lộc không tồn tại";
                return View(input);
            }
            input.phatLocId = plInfo.AccountID;
            //cut receiverNameList
            string receiverNameLst = StringUtil.RemoveLastStr(input.receiverNameList);
            string[] ArrReceiverName = receiverNameLst.Split(',');

            string nickNameSuccess = string.Empty;
            string nickNameError = string.Empty;
            int response = 0;
            long transId = 0;
            long wallet = 0;

            var len = ArrReceiverName.Length;
            if (len > 10)
                len = 10;

            for (int i = 0; i < len; i++)
            {
                Thread.Sleep(500);
                //Get ReceiverId
                var accInfo = UserDAO.Instance.GetAccountByNickName(ArrReceiverName[i], input.serviceId);
                if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
                {
                    NLogManager.LogMessage(string.Format("TestTransferToUser-NotExist:{0}", ArrReceiverName[i]));
                    continue;
                }
                if (accInfo.AccountStatus != 1)
                {
                    NLogManager.LogMessage(string.Format("TestTransferToUser-Invalid:{0}", ArrReceiverName[i]));
                    continue;
                }
                if (plInfo.AccountID == accInfo.AccountID)
                {
                    NLogManager.LogMessage(string.Format("TestTransferToUser-Không thể chuyển cho chính mình:{0}", ArrReceiverName[i]));
                    continue;
                }

                input.receiverId = accInfo.AccountID;
                //bot transfer to user
                transId = 0;
                wallet = 0;
                response = 0;
                response = PhatLocDAO.Instance.TestTransferToUser(input, out transId, out wallet);
                NLogManager.LogMessage(string.Format("TestTransferToUser - PhatLocName:{0} | ReceiverName:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                    input.phatLocName, input.receiverName, iamount, input.note, transId, response));
                if (response > 0)
                    nickNameSuccess += ArrReceiverName[i] + ",";
                else
                    nickNameError += ArrReceiverName[i] + ",";
            }

            string msgSuccess = string.Empty;
            if (!string.IsNullOrEmpty(nickNameSuccess))
                msgSuccess = string.Format("Chuyển khoản cho tài khoản {0} thành công.", StringUtil.RemoveLastStr(nickNameSuccess));

            string msgError = string.Empty;
            if (!string.IsNullOrEmpty(nickNameError))
                msgError = string.Format("Chuyển khoản cho tài khoản {0} thất bại!", StringUtil.RemoveLastStr(nickNameError));

            string msg = string.Format("{0} {1}", msgSuccess, msgError);
            Session["TestTransferToUser"] = msg;
            return RedirectToAction("TestTransferToUser");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult NormalTransferToUser()
        {
            if (!CheckPermissionUser(new string[] { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, "kinhdoanh_123A" }, AdminAccountName))
                return RedirectToAction("Permission", "Account");

            ParsNormalTransfer model = new ParsNormalTransfer();
            ViewBag.Message = Session["NormalTransferToUser"] != null ? Session["NormalTransferToUser"].ToString() : string.Empty;
            Session["NormalTransferToUser"] = null;

            //fee user
            string strFee = ParamConfigDAO.Instance.GetConfigStrValue("TRANSFEE", "TRANSFEE");
            double feeUser = 0;
            if (!string.IsNullOrEmpty(strFee))
                feeUser = Convert.ToDouble(strFee);

            model.fee = feeUser;
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult NormalTransferToUser(ParsNormalTransfer input)
        {
            if (!CheckPermissionUser(new string[] { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, "kinhdoanh_123A" }, AdminAccountName))
                return RedirectToAction("Permission", "Account");

            ViewBag.ServiceBox = GetServices();
            if (string.IsNullOrEmpty(input.username) || string.IsNullOrEmpty(input.password)
                || string.IsNullOrEmpty(input.receiverNameList) || string.IsNullOrEmpty(input.amount)
                || string.IsNullOrEmpty(input.orgAmount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000!";
                return View(input);
            }

            int limit_feedback = Int32.Parse(ConfigurationManager.AppSettings["Limit_Feedback"]);
            if (iamount > limit_feedback)
            {
                ViewBag.Message = "Số tiền chuyển không hợp lệ!";
                return View(input);
            }

            //Get PhatLocId
            int responseLogin = 0;
            var plInfo = PhatLocDAO.Instance.UserLogin(input.username, Security.SHA256Encrypt(input.password), 1, 1, input.serviceId, out responseLogin);
            if (plInfo == null)
            {
                ViewBag.Message = "Tài khoản phát lộc không tồn tại";
                return View(input);
            }

            //cut receiverNameList
            string lstName = StringUtil.RemoveLastStr(input.receiverNameList);
            string[] ArrReceiverName = lstName.Split(',');

            string nickNameSuccess = string.Empty;
            string nickNameError = string.Empty;
            var len = ArrReceiverName.Length;
            if (len > 10)
                len = 10;

            for (int i = 0; i < len; i++)
            {
                Thread.Sleep(500);
                //Get ReceiverId
                var accInfo = UserDAO.Instance.GetAccountByNickName(ArrReceiverName[i], input.serviceId);
                if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
                {
                    NLogManager.LogMessage(string.Format("NormalTransferToUser-NotExist:{0}", ArrReceiverName[i]));
                    continue;
                }

                if (accInfo.AccountStatus != 1)
                {
                    NLogManager.LogMessage(string.Format("NormalTransferToUser-Invalid:{0}", ArrReceiverName[i]));
                    continue;
                }

                long transId = 0;
                long wallet = 0;
                int response = PhatLocDAO.Instance.UserTransferToUser(plInfo.AccountID, accInfo.AccountID, iamount, input.note, input.serviceId, out transId, out wallet);
                NLogManager.LogMessage(string.Format("NormalTransferToUser-PhatLocName:{0}| ReceiverName:{1}| iamount:{2}| note:{3}| TransId:{4}| Response:{5}",
                    plInfo.AccountName, ArrReceiverName[i], iamount, input.note, transId, response));
                if (response > 0)
                    nickNameSuccess += ArrReceiverName[i] + ",";
                else
                    nickNameError += ArrReceiverName[i] + ",";
            }

            string msgSuccess = string.Empty;
            if (!string.IsNullOrEmpty(nickNameSuccess))
                msgSuccess = string.Format("Chuyển khoản cho tài khoản {0} thành công.", StringUtil.RemoveLastStr(nickNameSuccess));

            string msgError = string.Empty;
            if (!string.IsNullOrEmpty(nickNameError))
                msgError = string.Format("Chuyển khoản cho tài khoản {0} thất bại!", StringUtil.RemoveLastStr(nickNameError));

            string msg = string.Format("{0} {1}", msgSuccess, msgError);
            Session["NormalTransferToUser"] = msg;
            return RedirectToAction("NormalTransferToUser");
        }
    }
}