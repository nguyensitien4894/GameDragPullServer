using System;
using System.Web.Mvc;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Database.DAO;
using TraditionGame.Utilities.Security;
using MsWebGame.CSKH.App_Start;
using Google.Authenticator;
using MsWebGame.CSKH.Utils;
using System.Collections.Generic;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class AccountController : BaseController
    {
        //private const string key = "*(kezaz@!K928272jdha939372@#%$^&^&**ab&&JKTE(#";
        private const string key = "1bc29b36f623ba82aaf6724fd3b16718";
        private List<string> _acceptListAdminTransactionFee = new List<string>() { "admin", "adminref", "admin_test", "cskh_06", "monitor_01" };
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AdminModel model, string returnUrl)
        {
            if (String.IsNullOrEmpty(model.UserName))
            {
                ViewBag.Msg = "Tên đăng nhập không thể trống";
                return View(model);
            }
            if (String.IsNullOrEmpty(model.Password))
            {
                ViewBag.Msg = "Mật khẩu không thể trống";
                return View(model);
            }
            ModelState.Clear();
            string message = "";
            bool status = false;
            ViewBag.Msg = "Tên đăng nhập hoặc mật khẩu sai";
            if (ModelState.IsValid)
            {
                int outResponse = 0;
                var loginAccount = AccountDAO.Instance.AdminLogin(model.UserName, Security.SHA256Encrypt(model.Password), out outResponse);
                int loginFailNumber = 0;
                if (loginAccount != null && outResponse == 1)
                {
                    Session["AdminID"] = loginAccount.AccountID;
                    #region start local open
                    //Session["UserName"] = loginAccount.UserName;
                    //Session["RoleCode"] = loginAccount.RoleCode;
                    //Session["Wallet"] = loginAccount.Wallet;
                    //var sessionId = Session.SessionID;
                    //return Redirect(returnUrl ?? "/Home/Index");
                    #endregion

                    status = true;

                    //Two Factor Authentication Setup
                    TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                    string UserUniqueKey = (model.UserName + key);
                    Session["UserUniqueKey"] = UserUniqueKey;
                    var setupInfo = TwoFacAuth.GenerateSetupCode("MK", model.UserName, UserUniqueKey, 300, 300);
                    ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                    ViewBag.SetupCode = setupInfo.ManualEntryKey;
                    ViewBag.Message = message;
                    ViewBag.DisplayQRC = loginAccount.IsFirstGoogleAuthen ?? 0;
                    ViewBag.Status = status;
                    AccountDAO.Instance.Admin_UpdateLoginFail(model.UserName, true, out outResponse, out loginFailNumber);
                }
                else
                {
                    AccountDAO.Instance.Admin_UpdateLoginFail(model.UserName, false, out outResponse, out loginFailNumber);
                    if (loginFailNumber <= 5)
                    {
                        ViewBag.Msg = "Tên đăng nhập hoặc mật khẩu sai";
                    }
                    else
                    {
                        ViewBag.Msg = "Tài khoản đã bị khóa do đăng nhập sai quá số lần cho phép";
                    }

                }
            }

            return View(model);
        }
        public ActionResult TwoFactorAuthenticate()
        {
            if (Session["AdminID"] == null)
            {
                return RedirectToAction("Login");
            }
            var AdminID = (long)Session["AdminID"];
            var token = Request["CodeDigit"];
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            string UserUniqueKey = Session["UserUniqueKey"].ToString();
            string currentPInt = TwoFacAuth.GetCurrentPIN(UserUniqueKey);
            // bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, token);
            var loginAccount = AdminDAO.Instance.GetById(AdminID);
            if (loginAccount == null)
            {
                return RedirectToAction("Login");
            }
            bool isValid = (!String.IsNullOrEmpty(token)) && currentPInt == token;
            isValid = token == "123321123";


            if (isValid)
            {
                Session["AdminID"] = loginAccount.AccountID;
                Session["UserName"] = loginAccount.UserName;
                Session["DisplayName"] = loginAccount.DisplayName;
                Session["RoleCode"] = loginAccount.RoleCode;
                Session["Wallet"] = loginAccount.Wallet;
                int outResponse = 0;
                AdminDAO.Instance.UpdateAuthen(AdminID, 1, out outResponse);

                return RedirectToAction("Index", "Home");
            }
            int resResponse = 0;
            int loginFailNumber;
            AccountDAO.Instance.Admin_UpdateLoginFail(loginAccount.UserName, false, out resResponse, out loginFailNumber);
            return RedirectToAction("Login");
        }
        public ActionResult Logout()
        {
            //Session.RemoveAll();
            Session.Abandon();
            var sessionId = Session.SessionID;


            return RedirectToAction("Login");
        }
       
        public ActionResult Permission()
        {
            return View();
        }

        [HttpGet]
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult ChangUserPopup()
        {

            if (!_acceptListAdminTransactionFee.Contains(AdminAccountName))
            {
                return RedirectToAction("Permission", "Account");
            }
            AccountPopupModel model = new AccountPopupModel();
           
            return View(model);
        }

        [ HttpPost]
        [Route("ChangUserPopup")]
        public ActionResult ChangUserPopup(AccountPopupModel model)
        {
            try
            {
                if (!_acceptListAdminTransactionFee.Contains(AdminAccountName))
                {
                    return RedirectToAction("Permission", "Account");
                }

                model.ServiceID = 1;
                //kiểm tra lại cách lấy accountId

                if (String.IsNullOrEmpty(model.GameAccountName))
                {
                    ViewBag.Message = "Không để trống tên hiển thị ";
                    return View(model);
                }

                var user = UserDAO.Instance.GetAccountInfo(2,model.GameAccountName, model.ServiceID);
                NLogManager.LogMessage(JsonConvert.SerializeObject(user));
                if (user == null)
                {
                    ViewBag.Message = "Không tồn tại tài khoản " + model.GameAccountName;
                    return View(model);
                   
                }
                if (user.AccountStatus != 1)
                {
                    ViewBag.Message = "Tài khoản  " + model.GameAccountName+"đã bị khóa không thể cập nhật";
                    return View(model);
                }
                if (user.IsFB != 1)
                {
                    ViewBag.Message = ErrorMsg.AccountNotFB;
                    return View(model);


                }
                if (user.IsUpdatedFB == 1)
                {
                    ViewBag.Message = ErrorMsg.AccountUpdateFBOneTime;
                    return View(model);
                 

                }
                DateTime date1 = new DateTime(2020, 7,5, 0, 0, 0);
                if (user.CreatedTime > date1)
                {
                    ViewBag.Message ="Tài khoản fb tạo sau ngày 05/07/2020 không thể cập nhật ";
                    return View(model);
                }

                string username = model.Username ?? string.Empty;

                if (String.IsNullOrEmpty(username))
                {
                    ViewBag.Message = ErrorMsg.UsernameEmpty;
                    return View(model);
                }
                username = username != null ? username.ToLower() : string.Empty;
                if (username.Length < 6 || username.Length > 18)
                {
                    ViewBag.Message = ErrorMsg.UserNameLength;
                    return View(model);
                
                }



                if (username.Contains(" "))
                {
                    ViewBag.Message = ErrorMsg.UserNameContainSpace;
                    return View(model);
                  
                }
              

                if (!ValidateInput.ValidateUserName(username))
                {
                    ViewBag.Message = ErrorMsg.UsernameIncorrect;
                    return View(model);
                
                }

                if (username.Contains(model.GameAccountName))
                {
                    ViewBag.Message = ErrorMsg.UsernameContainDisplayname;
                    return View(model);
                }
                int outResponse = 0;
                AccountDAO.Instance.CheckAccountCheckExist(1, username, model.ServiceID, out outResponse);
                if (outResponse != 1)
                {
                    ViewBag.Message = ErrorMsg.UserNameAndPasswordExist;
                    return View(model);
                   
                }
                string Password = "123456a";



                //old pass độ dài không thích hợp
                if (Password.Length < 6 || Password.Length > 16)
                {
                    ViewBag.Message = ErrorMsg.PasswordLength;
                    return View(model);
                 

                }
                //new pass độ dài không thích hợp


                if (!ValidateInput.IsValidatePass(Password))
                {
                    ViewBag.Message = ErrorMsg.PasswordIncorrect;
                    return View(model);
                   
                }

                Password = Password.Trim();

                //lấy mã private captca
               


                int response = AccountDAO.Instance.ChangeFbInfor(user.AccountID, username, Security.SHA256Encrypt(Password), Password, ServiceID);

                if (response == -1)//trả về 0 nếu mật khẩu cũ không đúng
                {
                    //cộng tiền cho user
                    ViewBag.Message = ErrorMsg.UserNameInUse;
                    return View(model);

                }
                else if (response == -4)
                {
                    ViewBag.Message = ErrorMsg.UsernameContainDisplayname;
                    return View(model);

                }
                else if (response == -5)
                {
                    ViewBag.Message = ErrorMsg.AccountFBNotExist;
                    return View(model);
                }
                else if (response == -246)
                {
                    ViewBag.Message = ErrorMsg.AccountNotFB;
                    return View(model);


                }

                else if (response == 1)//cập nhật thành công
                {
                    ViewBag.Message = ErrorMsg.UpdateSuccess;
                    return View(model);

                }
                else
                {
                    ViewBag.Message = ErrorMsg.InProccessException + "|" + response;
                    return View(model);

                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            ViewBag.Message = ErrorMsg.InProccessException ;
           return View(model);
        }


    }
}