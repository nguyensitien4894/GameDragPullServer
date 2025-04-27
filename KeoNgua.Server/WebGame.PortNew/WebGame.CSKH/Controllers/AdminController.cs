using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MsTraditionGame.Utilities.Utils;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Security;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.HistoryTranfers;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Models.Transactions;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities.OneSignal;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class AdminController : BaseController
    {
        #region chuyển khoản cho đại lý
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToAgency()
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN &&
                (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST &&
                (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF)
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            ViewBag.ServiceBox = GetServices();
            model.balance = GetAmountAdmin();
            ViewBag.Message = Session["TranferMoneyToAgency"] != null ? Session["TranferMoneyToAgency"].ToString() : string.Empty;
            Session["TranferMoneyToAgency"] = null;
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToAgency(ParsTransfer input)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF 
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            {
                return RedirectToAction("Permission", "Account");
            }
            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;

            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount)
                || (input.walletType != 1 && input.walletType != 3) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }

            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var agencyInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);

            if (agencyInfo == null || agencyInfo.AccountID <= 0 || agencyInfo.AccountType != 2)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (agencyInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            long walletrecive = 0;
            int res = AdminDAO.Instance.AdminTransferToAgency(AdminID, agencyInfo.AccountID, input.walletType, iamount, input.note, input.ServiceID, out transId, out wallet,out walletrecive);
            NLogManager.LogMessage(string.Format("AdminTransferToAgency - AdminID:{0} | AgencyID:{1} | walletType:{2} | iamount:{3} | note:{4} | TransId:{5} | Response:{6}",
                AdminID, agencyInfo.AccountID, input.walletType, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại";
                //Update trạng thái giao dịch = 2;
                int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                return View(input);
            }
            else
            {
                string content = string.Format("Bạn vừa nhận {0} {2} từ nickname {1}. Số dư hiện tại là : {3}",
                        input.amount, AdminDisplayName, input.walletType == 3 ? " giftcode" : string.Empty, walletrecive.LongToMoneyFormat());
                if (agencyInfo.TelegramID > 0)
                {

                    GunNotifyChangeBalance(agencyInfo.TelegramID, content);
                }
                int outResponse;
                long msgID;
                if (agencyInfo != null)
                {
                    if (!String.IsNullOrEmpty(agencyInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, agencyInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(agencyInfo.SignalID) && !String.IsNullOrEmpty(agencyInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { agencyInfo.SignalID }, content);
                    }
                }
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Chuyển khoản thành công";
                input.amount = string.Empty;
                input.orgAmount = string.Empty;
                input.receiverName = string.Empty;
                input.note = string.Empty;
                return   View(input);
            }
        }

        private static readonly string _ChangeBalanceUrl = System.Configuration.ConfigurationManager.AppSettings["CHANGE_BALANCE_URL"].ToString();
        private static readonly string _ApiChangeBalance = System.Configuration.ConfigurationManager.AppSettings["API_CHANGE_BALANCE"].ToString();
        private void GunNotifyChangeBalance(long tele_id, string content)
        {
            try
            {
                var api = new Helpers.ApiUtil<dynamic>();
                api.ApiAddress = _ChangeBalanceUrl;
                api.URI = _ApiChangeBalance;
                var result = api.Send(new { ChatID = tele_id, Content = content });
                NLogManager.LogMessage(string.Format("GunNotifyChangeBalance-Res:{0}", result.ResponseCode));
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
        #endregion



        private List<string> accpetList = new List<string> { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01 };

        public ActionResult TranferMoneyBankMiss()
        {
            //if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            Session["TranferMoneyToUser"] = null;
            return View(model);
        }
        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyBankMiss(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) )
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }


            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName,1);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            var chargeId = 2;
            if (input.walletType == 0)
            {
                //double iamountnew = iamount * 1.2;
                //iamount = Convert.ToInt32(iamountnew);
                chargeId = 1;
            }

            var res = JsonConvert.DeserializeObject<dynamic>(Get("https://callback01052023.sicbet.net/api/Momo/MomoCallBackResultAction?chargeId=" + chargeId + "&chargeType=bank&chargeCode=" + input.receiverName + "&regAmount=0&status=success&chargeAmount=" + iamount + "&requestId=" + input.receiverName + "&signature=0&momoTransId=0"));
            if (res.errorCode != 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại|" + res.errorDescription;
                return View(input);
            }
            

            balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            ViewBag.Message = "Chuyển khoản thành công";
            input.note = string.Empty;
            input.receiverName = string.Empty;
            input.orgAmount = string.Empty;
            input.amount = string.Empty;

            input.transfee = 0;
            return View(input);

            
        }
        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #region chuyển khoản cho người chơi
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToUser()
        {
            //if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            if(!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            Session["TranferMoneyToUser"] = null;
            return View(model);
        }

        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToUser(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }
            
            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }
            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferToUser2(AdminID, accInfo.AccountID, iamount, input.note, input.ServiceID, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTransferToUser - AdminID:{0} | UserID:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, accInfo.AccountID, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại|"+ res;
                return View(input);
            }
            else
            {
                int outResponse;
                long msgID;
                if (accInfo != null)
                {
                    string content = string.Format("Bạn vừa nhận {0} từ nickname {1}",
                       input.amount, AdminDisplayName);
                    if (!String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(accInfo.SignalID) && !String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accInfo.SignalID }, content);
                    }
                }
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Chuyển khoản thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;
                
                input.transfee = 0;
                return View(input);
                ////Cộng tiền và ghi log  User
                //int resUser = AdminDAO.Instance.UserTransferReceive(AdminID, AdminAccountName, accInfo.AccountID, iamount, input.note, transId, wallet);
                //NLogManager.LogMessage(string.Format("UserTransferReceive - Wallet:{0} | Response:{1}", wallet, resUser));
                //if (resUser == 1)
                //{
                //    Session["TranferMoneyToUser"] = "Chuyển khoản thành công";
                //    return RedirectToAction("TranferMoneyToUser");
                //}
                //else
                //{
                //    ViewBag.Message = "Cộng tiền cho người chơi không thành công. Báo lại cho admin";
                //    //Update trạng thái giao dịch = 2;
                //    int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                //    NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                //    return View(input);
                //}
            }
        }

         
        public ActionResult TranferMoneyToUserSubtraction()
        {
            //if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            Session["TranferMoneyToUser"] = null;
            return View(model);
        }

        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToUserSubtraction(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }
            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferToUserSub(AdminID, accInfo.AccountID, iamount, input.note, input.ServiceID, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTransferToUser - AdminID:{0} | UserID:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, accInfo.AccountID, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại|" + res;
                return View(input);
            }
            else
            {
                int outResponse;
                long msgID;
                if (accInfo != null)
                {
                    string content = string.Format("Bạn vừa nhận {0} từ nickname {1}",
                       input.amount, AdminDisplayName);
                    if (!String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(accInfo.SignalID) && !String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accInfo.SignalID }, content);
                    }
                }
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Chuyển khoản thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;

                input.transfee = 0;
                return View(input);
                ////Cộng tiền và ghi log  User
                //int resUser = AdminDAO.Instance.UserTransferReceive(AdminID, AdminAccountName, accInfo.AccountID, iamount, input.note, transId, wallet);
                //NLogManager.LogMessage(string.Format("UserTransferReceive - Wallet:{0} | Response:{1}", wallet, resUser));
                //if (resUser == 1)
                //{
                //    Session["TranferMoneyToUser"] = "Chuyển khoản thành công";
                //    return RedirectToAction("TranferMoneyToUser");
                //}
                //else
                //{
                //    ViewBag.Message = "Cộng tiền cho người chơi không thành công. Báo lại cho admin";
                //    //Update trạng thái giao dịch = 2;
                //    int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                //    NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                //    return View(input);
                //}
            }
        }


        public ActionResult TranferMoneyToUserSafebalance()
        {
            //if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            Session["TranferMoneyToUser"] = null;
            return View(model);
        }

        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToUserSafebalance(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }
            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferToUserSafeBalance(AdminID, accInfo.AccountID, iamount, input.note, input.ServiceID, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTransferToUserSafeBalance - AdminID:{0} | UserID:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, accInfo.AccountID, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại|" + res;
                return View(input);
            }
            else
            {
                int outResponse;
                long msgID;
                if (accInfo != null)
                {
                    string content = string.Format("Bạn vừa nhận {0} từ nickname {1}",
                       input.amount, AdminDisplayName);
                    if (!String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(accInfo.SignalID) && !String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accInfo.SignalID }, content);
                    }
                }
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Chuyển khoản thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;

                input.transfee = 0;
                return View(input);
                ////Cộng tiền và ghi log  User
                //int resUser = AdminDAO.Instance.UserTransferReceive(AdminID, AdminAccountName, accInfo.AccountID, iamount, input.note, transId, wallet);
                //NLogManager.LogMessage(string.Format("UserTransferReceive - Wallet:{0} | Response:{1}", wallet, resUser));
                //if (resUser == 1)
                //{
                //    Session["TranferMoneyToUser"] = "Chuyển khoản thành công";
                //    return RedirectToAction("TranferMoneyToUser");
                //}
                //else
                //{
                //    ViewBag.Message = "Cộng tiền cho người chơi không thành công. Báo lại cho admin";
                //    //Update trạng thái giao dịch = 2;
                //    int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                //    NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                //    return View(input);
                //}
            }
        }

        public ActionResult TranferMoneyToUserSafebalanceSub()
        {
            //if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            Session["TranferMoneyToUser"] = null;
            return View(model);
        }

        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToUserSafebalanceSub(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }
            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferToUserSafeBalanceSub(AdminID, accInfo.AccountID, iamount, input.note, input.ServiceID, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTransferToUserSafeBalance - AdminID:{0} | UserID:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, accInfo.AccountID, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Trừ tiền két user thất bại|" + res;
                return View(input);
            }
            else
            {
                int outResponse;
                long msgID;
                if (accInfo != null)
                {
                    string content = string.Format("Bạn vừa nhận {0} từ nickname {1}",
                       input.amount, AdminDisplayName);
                    if (!String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(accInfo.SignalID) && !String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accInfo.SignalID }, content);
                    }
                }
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Trừ tiền thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;

                input.transfee = 0;
                return View(input);
                ////Cộng tiền và ghi log  User
                //int resUser = AdminDAO.Instance.UserTransferReceive(AdminID, AdminAccountName, accInfo.AccountID, iamount, input.note, transId, wallet);
                //NLogManager.LogMessage(string.Format("UserTransferReceive - Wallet:{0} | Response:{1}", wallet, resUser));
                //if (resUser == 1)
                //{
                //    Session["TranferMoneyToUser"] = "Chuyển khoản thành công";
                //    return RedirectToAction("TranferMoneyToUser");
                //}
                //else
                //{
                //    ViewBag.Message = "Cộng tiền cho người chơi không thành công. Báo lại cho admin";
                //    //Update trạng thái giao dịch = 2;
                //    int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                //    NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                //    return View(input);
                //}
            }
        }


        public ActionResult TranferMoneyToUserFish()
        {
            //if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            Session["TranferMoneyToUser"] = null;
            return View(model);
        }

        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToUserFish(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }
            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferUserFish(AdminID,1, accInfo.AccountID, iamount, input.note, input.ServiceID, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTranferMoneyToUserFish - AdminID:{0} | User:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, accInfo.AccountID, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại|" + res;
                return View(input);
            }
            else
            {
                int outResponse;
                long msgID;
                if (accInfo != null)
                {
                    string content = string.Format("Bạn vừa nhận {0} từ nickname {1}",
                       input.amount, AdminDisplayName);
                    if (!String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(accInfo.SignalID) && !String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accInfo.SignalID }, content);
                    }
                }
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Chuyển khoản thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;

                input.transfee = 0;
                return View(input);
                ////Cộng tiền và ghi log  User
                //int resUser = AdminDAO.Instance.UserTransferReceive(AdminID, AdminAccountName, accInfo.AccountID, iamount, input.note, transId, wallet);
                //NLogManager.LogMessage(string.Format("UserTransferReceive - Wallet:{0} | Response:{1}", wallet, resUser));
                //if (resUser == 1)
                //{
                //    Session["TranferMoneyToUser"] = "Chuyển khoản thành công";
                //    return RedirectToAction("TranferMoneyToUser");
                //}
                //else
                //{
                //    ViewBag.Message = "Cộng tiền cho người chơi không thành công. Báo lại cho admin";
                //    //Update trạng thái giao dịch = 2;
                //    int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                //    NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                //    return View(input);
                //}
            }
        }

        public ActionResult TranferMoneyToUserFishSub()
        {
            //if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            model.transfee = GetTransFee("ADMIN_TO_USER");
            ViewBag.ServiceBox = GetServices();
            ViewBag.Message = Session["TranferMoneyToUser"] != null ? Session["TranferMoneyToUser"].ToString() : string.Empty;
            Session["TranferMoneyToUser"] = null;
            return View(model);
        }

        [HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToUserFishSub(ParsTransfer input)
        {
            if (!accpetList.Contains((string)Session["UserName"]))
            {
                return RedirectToAction("Permission", "Account");
            }

            ViewBag.ServiceBox = GetServices();
            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }
            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            var accInfo = UserDAO.Instance.GetAccountByNickName(input.receiverName, input.ServiceID);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            if (accInfo.AccountStatus != 1)
            {
                ViewBag.Message = "Tài khoản không hợp lệ";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferUserFish(AdminID, 2, accInfo.AccountID, iamount, input.note, input.ServiceID, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTranferMoneyToUserFishSub - AdminID:{0} | User:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, accInfo.AccountID, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại|" + res;
                return View(input);
            }
            else
            {
                int outResponse;
                long msgID;
                if (accInfo != null)
                {
                    string content = string.Format("Bạn vừa nhận {0} từ nickname {1}",
                       input.amount, AdminDisplayName);
                    if (!String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, accInfo.PhoneSafeNo, content, out outResponse, out msgID);
                    }
                    if (!String.IsNullOrEmpty(accInfo.SignalID) && !String.IsNullOrEmpty(accInfo.PhoneSafeNo))
                    {
                        OneSignalApi.SendByPlayerID(new List<string>() { accInfo.SignalID }, content);
                    }
                }
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Trừ tiền thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;

                input.transfee = 0;
                return View(input);
                ////Cộng tiền và ghi log  User
                //int resUser = AdminDAO.Instance.UserTransferReceive(AdminID, AdminAccountName, accInfo.AccountID, iamount, input.note, transId, wallet);
                //NLogManager.LogMessage(string.Format("UserTransferReceive - Wallet:{0} | Response:{1}", wallet, resUser));
                //if (resUser == 1)
                //{
                //    Session["TranferMoneyToUser"] = "Chuyển khoản thành công";
                //    return RedirectToAction("TranferMoneyToUser");
                //}
                //else
                //{
                //    ViewBag.Message = "Cộng tiền cho người chơi không thành công. Báo lại cho admin";
                //    //Update trạng thái giao dịch = 2;
                //    int resTran = AdminDAO.Instance.TransactionUpdate(transId, 2);
                //    NLogManager.LogMessage(string.Format("TransactionUpdate - TransId:{0} | Response:{1}", transId, resTran));
                //    return View(input);
                //}
            }
        }

        #endregion

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToAdmin()
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF 
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            ViewBag.Message = Session["TranferMoneyToAdmin"] != null ? Session["TranferMoneyToAdmin"].ToString() : string.Empty;
            Session["TranferMoneyToAdmin"] = null;
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToAdmin(ParsTransfer input)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN 
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF 
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            {
                return RedirectToAction("Permission", "Account");
            }

            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }

            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            long receiverId = GetAdminIDByNickName(input.receiverName);
            if (receiverId == 0)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferToAdmin(AdminID, receiverId, iamount, input.note, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTransferToUser - AdminID:{0} | AgencyID:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, receiverId, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại";
                return View(input);
            }
            else
            {
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Chuyển khoản thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;
                input.transfee = 0;
                return View(input);
            }
        }


        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToAdminTest()
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            {
                return RedirectToAction("Permission", "Account");
            }

            ParsTransfer model = new ParsTransfer();
            model.balance = GetAmountAdmin();
            ViewBag.Message = Session["TranferMoneyToAdmin"] != null ? Session["TranferMoneyToAdmin"].ToString() : string.Empty;
            Session["TranferMoneyToAdmin"] = null;
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TranferMoneyToAdminTest(ParsTransfer input)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF
                && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST)
            {
                return RedirectToAction("Permission", "Account");
            }

            long balanceAdmin = GetAmountAdmin();
            input.balance = balanceAdmin;
            if (string.IsNullOrEmpty(input.receiverName) || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000";
                return View(input);
            }

            long iorgAmount = Int64.Parse(input.orgAmount.Replace(".", ""));
            if (iorgAmount > balanceAdmin)
            {
                ViewBag.Message = "Số tiền nhận tối lớn hơn số dư hiện có";
                return View(input);
            }

            long receiverId = GetAdminTestIDByNickName(input.receiverName);
            if (receiverId == 0)
            {
                ViewBag.Message = "Tài khoản không tồn tại";
                return View(input);
            }

            long transId = 0;
            long wallet = 0;
            int res = AdminDAO.Instance.AdminTransferToAdmin(AdminID, receiverId, iamount, input.note, out transId, out wallet);
            NLogManager.LogMessage(string.Format("AdminTransferToUser - AdminID:{0} | AgencyID:{1} | iamount:{2} | note:{3} | TransId:{4} | Response:{5}",
                AdminID, receiverId, iamount, input.note, transId, res));
            if (res <= 0 || wallet < 0 || transId <= 0)
            {
                ViewBag.Message = "Chuyển khoản thất bại";
                return View(input);
            }
            else
            {
                balanceAdmin = GetAmountAdmin();
                input.balance = balanceAdmin;
                ViewBag.Message = "Chuyển khoản thành công";
                input.note = string.Empty;
                input.receiverName = string.Empty;
                input.orgAmount = string.Empty;
                input.amount = string.Empty;
                input.transfee = 0;
                return View(input);
            }
        }
        #region log tranfer money
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult LogTranferMoney()
        {
            ListHistoryTransferModel model = new ListHistoryTransferModel();
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult LogTranferMoneyList(GridCommand command, ListHistoryTransferModel model)
        {
            int totalRecord = 0;
            var list = AdminDAO.Instance.GetListHistoryTranfers(AdminID, 3, model.TransType,null, null, null, model.ReceiverName, model.StartDate,
                model.EndDate, model.ServiceID, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);




            var gridModel = new GridModel<HistoryTransferModel>
            {
                Data = list.Select(x =>
                {
                    var m = new HistoryTransferModel();
                    m = Mapper.Map<HistoryTransferModel>(x);
                    return m;
                }),
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        #endregion

        #region helper
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public dynamic GetReceicerAgencyInfo(string nickName,int ServiceID)
        {
            try
            {
                var agencyInfo = UserDAO.Instance.GetAccountByNickName(nickName, ServiceID);
                if (agencyInfo == null || agencyInfo.AccountID <= 0 || agencyInfo.AccountType != 2)
                {
                    return Json(new
                    {
                        Response = -103,
                        AccountLevel = -1,
                        Fee = 0,
                        FeePercent = "Phí chuyển 0%"
                    });
                }

                if (agencyInfo.AccountStatus != 1)
                {
                    return Json(new
                    {
                        Response = -105,
                        AccountLevel = agencyInfo.AccountLevel,
                        Fee = 0,
                        FeePercent = "Phí chuyển 0%"
                    });
                }

                if (agencyInfo.AccountID == AdminID)
                {
                    return Json(new
                    {
                        Response = -101,
                        AccountLevel = agencyInfo.AccountLevel,
                        Fee = 0,
                        FeePercent = "Phí chuyển 0%"
                    });
                }

                //get phí chuyển khoản của đại lý
                string feeCode = "ADMIN_TO_AGENCY_LEVEL" + agencyInfo.AccountLevel.ToString();
                double transFee = GetTransFee(feeCode);
                return Json(new
                {
                    Response = 1,
                    AccountLevel = agencyInfo.AccountLevel,
                    Fee = transFee,
                    FeePercent = "Phí chuyển " + (transFee * 100) + "%"
                });
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return Json(new
                {
                    Response = -99,
                    AccountLevel = -1,
                    Fee = 0,
                    FeePercent = "Phí chuyển 0%"
                });
            }
        }

        private long GetAmountAdmin()
        {
            try
            {
                var admin = AdminDAO.Instance.GetById(AdminID);
                return Int64.Parse(admin.Wallet.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
        }
        #endregion


        #region GameBank
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GameBank()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.RoomBox = InfoHandler.Instance.GetRoomBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GameBankSearch(GridCommand command, int gameId, int roomId, DateTime startDate, DateTime endDate)
        {
            if (gameId == 0 || roomId == 0)
                return new JsonResult { Data = null };

            var list = AdminDAO.Instance.GetGameBankInfo(gameId, roomId, startDate, endDate);
            if (list == null || list.Count == 0)
            {
                int res = TrackingSpin(gameId, roomId, startDate, endDate);
                if (res > 0)
                {
                    list = AdminDAO.Instance.GetGameBankInfo(gameId, roomId, startDate, endDate);
                }
            }

            if (list == null)
                list = new List<GameBankInfo>();

            var model = new GridModel<GameBankInfo>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GameBankExpertise()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.RoomBox = InfoHandler.Instance.GetRoomBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetGameBankExpertise(GridCommand command, int gameId, int roomId)
        {
            var list = AdminDAO.Instance.GameBankExamine(gameId, roomId);
            if (list == null)
                list = new List<GameBankExpertise>();

            var model = new GridModel<GameBankExpertise>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GameBankExpertiseProceed()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.RoomBox = InfoHandler.Instance.GetRoomBox();
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GameBankExpertiseProceed(GameBankExpertiseProceed data)
        {
            long res = AdminDAO.Instance.TrackingExamine(data.GameID, data.RoomID, data.StartDate, data.EndDate);
            if (res == AppConstants.DBS.SUCCESS)
                SuccessNotification("Thẩm định thành công");
            else
                ErrorNotification("Thẩm định thất bại");

            return View();
        }

        private int TrackingSpin(int gameId, int roomId, DateTime startDate, DateTime endDate)
        {
            int res = AdminDAO.Instance.TrackingSpin(gameId, roomId, startDate, endDate);
            return res;
        }
        #endregion

        #region Player Manager
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public dynamic UserManager(ParUserManager  pars)
        {
            string nickName = pars.NickName;
            int status = pars.Status;
            int serviceId = pars.ServiceID;
            int outResponse = 0;
            var accInfo = UserDAO.Instance.GetAccountInfo(2, nickName, serviceId);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
                return Json(new { Response = -1, Msg = "Tài khoản không tồn tại" });

            int res = 0;
            if (status == 1 || status == 2)
            {
                res = UserDAO.Instance.UserUpdateStatus(accInfo.AccountID, status);
               
                if (status==1)
                {
                    UserComplainDAO.Instance.UserComplainCreate(accInfo.AccountID, 4, serviceId, pars.Msg, pars.Msg,true, AdminID, out outResponse);
                }

                if (status ==2)
                {
                    UserComplainDAO.Instance.UserComplainCreate(accInfo.AccountID, 3, serviceId, pars.Msg, pars.Msg, true, AdminID, out outResponse);
                }



            }
            else if (status == 3)
            {
                string password = Security.SHA256Encrypt("123456a");
                res = UserDAO.Instance.UserResetPassword(accInfo.AccountID, password);
                UserComplainDAO.Instance.UserComplainCreate(accInfo.AccountID, 5, serviceId, pars.Msg, pars.Msg, true, AdminID, out outResponse);
            }

            var msg = string.Format("{0} user {1}", status == 1 ? "Mở khóa" : status == 2 ? "Khóa" : "Reset password",
                res == AppConstants.DBS.SUCCESS ? "thành công" : "thất bại");
            return Json(new { Response = res, Msg = msg });
        }


        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public dynamic UserGameBaiLock(ParUserManager pars)
        {
            string nickName = pars.NickName;
            int status = pars.Status;
            int serviceId = pars.ServiceID;
          
            var accInfo = UserDAO.Instance.GetAccountInfo(2, nickName, serviceId);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
                return Json(new { Response = -1, Msg = "Tài khoản không tồn tại" });

            int res = 0;
           
                UserDAO.Instance.UserWrongCreate(serviceId, accInfo.AccountID, nickName,pars.Msg,out res);
            string msg = string.Empty;
            if (res >= 0)
            {
                msg = "Khóa người dùng Game Bài thành công";
            }else
            {
                msg = "Khóa người dùng Game Bài thất bại| "+res;
            }


            return Json(new { Response = res, Msg = msg });
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public dynamic UserLockTrans(ParUserLockTrans pars)
        {
            string nickName = pars.NickName;
            bool status = pars.Status;
            int serviceId = pars.ServiceID;

            var accInfo = UserDAO.Instance.GetAccountInfo(2, nickName, serviceId);
            if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != 1)
                return Json(new { Response = -1, Msg = "Tài khoản không tồn tại" });

            string msg = string.Empty;
            //lock core
            int res = UserDAO.Instance.UserLockTrans(accInfo.AccountID, status);
            NLogManager.LogMessage(string.Format("UserLockTransCore - AccountID:{0} | NickName:{1} | LockStatus:{2} | Response:{3}",
                accInfo.AccountID, nickName, status, res));

            msg = string.Format("{0} giao dịch tài khoản {1} {2}", status ? "Khóa" : "Mở khóa", nickName,
                res == AppConstants.DBS.SUCCESS ? "thành công" : "thất bại");
            return Json(new { Response = res, Msg = msg });
        }

        #endregion

        #region dong bang
        [HttpGet]
        [AdminAuthorize(Roles =ADMIN_CALLCENTER_ROLE)]
        public ActionResult TransactionFree()
        {
            
            if (!_acceptListAdminTransactionFee.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }
            TransactionFreeModel model = new TransactionFreeModel();
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }
        private List<string> _acceptListAdminTransactionFee = new List<string>() { "admin","adminref","admin_test","cskh_01"};
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult TransactionFree(GridCommand command,string nickName,string partnerName, DateTime? fromDate, DateTime? toDate,int ServiceID=1)
        {
            
            if (!_acceptListAdminTransactionFee.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }
             UserInfo user=null;
            if (!String.IsNullOrEmpty(nickName))
            {
                user = UserDAO.Instance.GetAccountByNickName(nickName, ServiceID);
            }
            long userID = 0;
            if (user != null)
            {
                userID = user.AccountID;
            }
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            int totalRecord = 0;
            if (userID <= 0)
                userID = -1;
            var lstRs = TransactionDAO.Instance.TransactionSearch(1,1,-1,1, partnerName,fromDate, toDate, userID,null, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord,ServiceID);
            var gridModel = new GridModel<TransactionRefund>
            {
                Data = lstRs,
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        /// <summary>
        /// thu hồi giao dịch
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public dynamic TransactionFreeExecute(long Id)
        {
            
            if (!_acceptListAdminTransactionFee.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }

            var lstRs = TransactionDAO.Instance.TransactionByID(Id);
            if (lstRs != null)
            {
                var sendID = lstRs.CreateUserID;//thằng chuyển tiền
                var receiverID = lstRs.ReceiverID;//thằng nhận tiền
                var amount = lstRs.Amount;
                var userReceiver = AccountProfileDAO.Instance.GetAccountInfor(receiverID.Value, null, null, lstRs.ServiceID);
                var userSend = AccountProfileDAO.Instance.GetAccountInfor(sendID, null, null, lstRs.ServiceID);
                //lấy lại amout thành nhận tiền


                string note = String.Format("Admin thu hồi giao dịch mã {0} do {1} chuyển sai", lstRs.TransID, userSend.AccountName);
                long TransID;
                int Response = 0;
                long RemainWallet = 0;
                TransactionDAO.Instance.UserTransferRetrieve(receiverID.Value, sendID, amount, note, Id, lstRs.ServiceID, out TransID, out RemainWallet, out Response);
                if (Response == 1)
                {
                    return Json(new
                    {
                        Code = 1,
                        Message = "Thu hồi giao dịch thành công|" + Response,
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (Response == -507)
                {
                    return Json(new
                    {
                        Code = -1,
                        Message = "Người dùng không đủ số dư" + Response,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Code = -1,
                        Message = "Có lỗi khi thu hồi giao dịch|"+ Response,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        #endregion

        #region thuhoidaily
        [HttpGet]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TransactionFreeAgency()
        {
            TransactionFreeModel model = new TransactionFreeModel();
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult TransactionFreeAgency(GridCommand command, string nickName, string partnerName, DateTime? fromDate, DateTime? toDate, int ServiceID = 1)
        {
            UserInfo user = null;
            if (!String.IsNullOrEmpty(nickName))
            {
                user = UserDAO.Instance.GetAccountByNickName(nickName, ServiceID);
            }
            long userID = 0;
            if (user != null)
            {
                userID = user.AccountID;
            }
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            int totalRecord = 0;
            if (userID <= 0)
                userID = -1;
            var lstRs = TransactionDAO.Instance.TransactionSearch(1, 1, -1, 2, partnerName, fromDate, toDate, userID, null, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord, ServiceID);
            var gridModel = new GridModel<TransactionRefund>
            {
                Data = lstRs,
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public dynamic TransactionFreeExecuteAgency(long Id)
        {

            var lstRs = TransactionDAO.Instance.TransactionByID(Id);
            if (lstRs != null)
            {
                var sendID = lstRs.CreateUserID;//thằng chuyển tiền
                var receiverID = lstRs.ReceiverID;//thằng nhận tiền
                var amount = lstRs.Amount;
                var userReceiver = AccountProfileDAO.Instance.GetAccountInfor(receiverID.Value, null, null, lstRs.ServiceID);
                var userSend = AccountProfileDAO.Instance.GetAccountInfor(sendID, null, null,lstRs.ServiceID);
                //lấy lại amout thành nhận tiền


                string note = String.Format("Admin thu hồi giao dịch mã {0} do {1} chuyển sai", lstRs.TransID, userSend.AccountName);
                long TransID;
                int Response = 0;
                long RemainWallet = 0;
                long RemainBalance = 0;
                TransactionDAO.Instance.UserRollbackTransAgency(sendID, amount,lstRs.TransID,note,lstRs.ServiceID,Convert.ToInt32(AdminID), receiverID.Value, out TransID,out RemainBalance, out RemainWallet, out Response);
                if (Response == 1)
                {
                    return Json(new
                    {
                        Code = 1,
                        Message = "Thu hồi giao dịch thành công|" + Response,
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new
                    {
                        Code = -1,
                        Message = "Có lỗi khi thu hồi giao dịch|" + Response,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
        #endregion
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public dynamic BanUserChat(string nickName, int rdoStatus, int serviceId)
        {
            string adminName = Session["UserName"].ToString();
            NLogManager.LogMessage("adminName: " + "admintest");
            bool res = false;
            if (rdoStatus == 0)
            {
                string uri = "api/chat/AddBanUser";
                var resLock = SendRequestApi.Instance.SendLockChat("admintest", nickName, uri, serviceId);
                res = resLock;
            }
            else if (rdoStatus == 1)
            {
                string uri = "api/chat/RemoveBanUser";
                var resUnlock = SendRequestApi.Instance.SendUnLockChat("admintest", nickName, uri, serviceId);
                res = resUnlock == string.Format("[{0}]{1}", SendRequestApi.Instance.GetServiceName(serviceId), nickName);
            }

            var msg = string.Format("{0} chat user {1}", rdoStatus == 0 ? "Khóa" : rdoStatus == 1 ? "Mở" : string.Empty, res ? "thành công" : "thất bại");
            return Json(new { Response = res ? 1 : -1, Msg = msg });
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CashFlow()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCashFlow(GridCommand command, ParsAdminTrans input)
        {
            long accountId = AdminID;
            if (!string.IsNullOrEmpty(input.nicknameAdmin))
            {
                accountId = GetAdminIDByNickName(input.nicknameAdmin);
                if (accountId <= 0)
                {
                    var grid = new GridModel<AdminTrans> { Data = null, Total = 0 };
                    return new JsonResult { Data = grid };
                }
            }
            int totalRecord = 0;
            var lstRs = AdminDAO.Instance.GetAdminTrans(accountId, input, command.Page <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<AdminTrans> { Data = lstRs, Total = totalRecord };
            return new JsonResult { Data = gridModel };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult SearchAdminInfo()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult SearchAdminInfo(ParsAdminInfo input)
        {
            if (string.IsNullOrEmpty(input.nickName))
                return View();

            var lstRs = AdminDAO.Instance.GetList(input.roleId, input.nickName, input.phoneNo, ServiceID);
            if (lstRs == null)
                return View();

            Admin rs = new Admin();
            rs = lstRs.FirstOrDefault();
            return View(rs);
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult ChangePassword()
        {
            ViewBag.Message = string.Empty;
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult ChangePassword(ParsChangePassword input)
        {
            if (string.IsNullOrEmpty(input.passwordOld) || string.IsNullOrEmpty(input.passwordNew) || string.IsNullOrEmpty(input.rePasswordNew))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                return View();
            }

            if (input.passwordOld == input.passwordNew)
            {
                ViewBag.Message = "Mật khẩu mới không được trùng với mật khẩu cũ!";
                return View();
            }

            if (input.passwordNew != input.rePasswordNew)
            {
                ViewBag.Message = "Nhập lại mật khẩu mới không khớp!";
                return View();
            }

            if (!ValidateInput.IsValidatePassSpecial(input.passwordNew))
            {
                ViewBag.Message = "Mật khẩu mới không hợp lệ! Mật khẩu yêu cầu 8-16 ký tự. Bao gồm ký tự hoa, thường, số và ký tự đặc biệt!";
                return View();
            }

            int resLogin = 0;
            string passwordEncry = Security.SHA256Encrypt(input.passwordOld);
            var rs = AccountDAO.Instance.AdminLogin(AdminAccountName, passwordEncry, out resLogin);
            if (rs == null)
            {
                ViewBag.Message = "Mật khẩu cũ không đúng!";
                return View();
            }

            string newPasswordEncry = Security.SHA256Encrypt(input.passwordNew);
            int resChange = AdminDAO.Instance.AdminChangePassword(AdminAccountName, newPasswordEncry);
            if (resChange == 1)
                ViewBag.Message = "Đổi mật khẩu thành công!";
            else
                ViewBag.Message = "Đổi mật khẩu thất bại!";

            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CallCenter()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCallCenter(GridCommand command, string nickname, string phoneno, int serviceId = 1)
        {
            var lstRs = AdminDAO.Instance.GetCallCenterList(nickname, phoneno, serviceId);
            if (lstRs == null)
                lstRs = new List<CallCenterInfo>();

            var model = new GridModel<CallCenterInfo>
            {
                Data = lstRs,
                Total = lstRs.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult LockCallCenter(string username, bool lockStatus)
        {
            if (string.IsNullOrEmpty(username))
            {
                ErrorNotification("Khóa thất bại");
                return RedirectToAction("CallCenter");
            }

            int response = AdminDAO.Instance.AdminLockCallcenter(username, lockStatus);
            string msg = string.Format("{0} tài khoản cskh({2}) {1}", lockStatus ? "Mở khóa" : "Khóa", response == 1 ? "thành công" : "thất bại", username);
            if (response != 1)
                ErrorNotification(msg);
            else
                SuccessNotification(msg);

            return RedirectToAction("CallCenter");
        }

        [HttpPost]
        public long GetAdminIDByNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName))
                return 0;

            var lstRs = AdminDAO.Instance.GetList(1, nickName, null, ServiceID);
            if (lstRs != null && lstRs.Any())
                return lstRs.FirstOrDefault().AccountID;

            return 0;
        }
        [HttpPost]
        public long GetAdminTestIDByNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName))
                return 0;

            var lstRs = AdminDAO.Instance.GetList(2, nickName, null, ServiceID);
            if (lstRs != null && lstRs.Any())
                return lstRs.FirstOrDefault().AccountID;

            return 0;
        }

        [HttpPost]
        public int ValidateUserName(string UserName, int Type)
        {
            try
            {
                long userId = 0;
                UserDAO.Instance.GetUserIDFromUserName(Type, UserName, out userId);
                if (userId <= 0)
                    return -103;

                return 1;
            }
            catch (Exception)
            {
                return -99;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="type">1-User, 2-Agency</param>
        /// <returns></returns>
        [HttpPost]
        public int ValidateNickName(string nickName, int accountType,int ServiceID)
        {
            try
            {
                var accInfo = UserDAO.Instance.GetAccountByNickName(nickName, ServiceID);
                if (accInfo == null || accInfo.AccountID <= 0 || accInfo.AccountType != accountType)
                    return -103;

                if (accInfo.AccountStatus != 1)
                    return -105;

                return 1;
            }
            catch (Exception)
            {
                return -99;
            }
        }

        public int ValidateNickNameSpecial(string nickName)
        {
            try
            {
                var accInfo = PhatLocDAO.Instance.GetUsersCheckBot(nickName);
                if (accInfo == null || accInfo.AccountID >= 0)
                    return -103;

                return 1;
            }
            catch (Exception)
            {
                return -99;
            }
        }

        private void PrapareUserTranferModel(TranferMoneyToUserModel model)
        {
            var admin = AdminDAO.Instance.GetById(AdminID);
            model.AdminAccountName = AdminAccountName;
            model.AdminAmount = admin.Wallet ?? 0;
        }

        private double GetTransFee(string feeCode)
        {
            if (string.IsNullOrEmpty(feeCode))
                return 0;

            string paramType = "TRANSFEE";
            int totalRecord = 0;
            var lstFee = ParamConfigDAO.Instance.GetList(paramType, feeCode, null, 1, Int16.MaxValue, out totalRecord);
            if (lstFee != null && lstFee.Any())
                return double.Parse(lstFee.FirstOrDefault().Value);

            return 0;
        }


    }
}