using System;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Cookies;
using TraditionGame.Utilities.Facebook;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Security;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models;
using System.Configuration;
using TraditionGame.Utilities.Messages;
using MsWebGame.Portal.Server.Hubs;
using WebGame.Payment.Database.DAO;
using TraditionGame.Utilities.Utils;
using System.Diagnostics;
using MsWebGame.Portal.Helper;
using TraditionGame.Utilities;
using MsWebGame.Portal.Database;
using TraditionGame.Utilities.OneSignal;
using System.Collections.Generic;
using TraditionGame.Utilities.DNA;
using System.Linq;
using MsWebGame.RedisCache.Cache;
using Newtonsoft.Json;
using QRCoder;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System.Drawing.Imaging;
using System.Drawing;
using System.Web.Services.Description;
using System.Text;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : BaseApiController
    {
        private int TokenExpired = 5000;
        private string FB_CHANGE_LOG = "FB_Change";
        private string NORMAL_LOG = "NORMAL";
        [ActionName("CheckAuthenticated")]
        [HttpGet]
        public dynamic CheckAuthenticated()
        {
            try
            {
                var userData = new UserData();
                if (!CheckAuthenticated(ref userData))
                {
                    return new
                    {
                        ResponseCode = -1,
                        Description = "Please login to perform this function"
                    };
                }
                return new
                {
                    ResponseCode = 1,
                    Description = "Logged"
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = "The system is busy, please come back later"
                };
            }
        }
        private bool CheckAuthenticated(ref UserData userData)
        {
            userData = new UserData();
            try
            {
                //chưa đăng nhập
                var data = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    data = HttpContext.Current.User.Identity.Name;
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        string[] roles = data.Split(new char[] { '|' });
                        userData.AccountID = int.Parse(roles[0]);
                        userData.AccountName = roles[1];
                        userData.ClientIP = roles[2];
                    }
                }
                NLogManager.LogMessage("CheckAuthenticated : " + data);
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }
        /// <summary>
        /// check authentication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBalance")]
        public dynamic GetBalance( string username )
        {
            return AccountDAO.Instance.GetAccountInfo(0,"69live" + username, null, ServiceID);
        }
        /// <summary>
        /// check authentication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Captcha")]
        public dynamic Captcha()
        {
            return CaptchaCache.Instance.GetCaptcha();
        }

        [HttpGet]
        [Route("QrCode")]
        //public async Task<dynamic>QrCode(string inputUrl, string t, string access_token)
        //public async Task<dynamic>QrCode(string inputUrl, string access_token)
        public async Task<dynamic>QrCode()
        {
            Uri requestUri = Request.RequestUri; 
            string queryString = requestUri.Query; // ?inputUrl=somevalue
            string inputUrl = HttpUtility.ParseQueryString(queryString).Get("inputUrl"); // 

            inputUrl = System.Text.Encoding.Default.GetString(Convert.FromBase64String(inputUrl));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(inputUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                HttpRequestMessage request = response.RequestMessage;
                Uri requestUri2 = request.RequestUri;
                string url = requestUri2.ToString().Replace("DepositQR.aspx", "Images/QRCode.svg");

                HttpResponseMessage response2 = await client.GetAsync(url);
                response2.EnsureSuccessStatusCode();

                Bitmap bitmap;

                var svgString = await response2.Content.ReadAsStringAsync();

                var svg = new SkiaSharp.Extended.Svg.SKSvg();
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(svgString)))
                {
                    svg.Load(stream);
                }

                // Create a new bitmap
                bitmap = new Bitmap(605, 605, PixelFormat.Format32bppArgb);

                using (var skBitmap = new SKBitmap(bitmap.Width, bitmap.Height))
                {
                    // Render the SVG onto the SKBitmap
                    using (var skCanvas = new SKCanvas(skBitmap))
                    {
                        skCanvas.Clear(SKColors.White); // Set the background color

                        // Adjust the scale and translate as needed to fit the SVG onto the SKBitmap
                        var bounds = svg.ViewBox;
                        var scale = Math.Min(skBitmap.Width / bounds.Width, skBitmap.Height / bounds.Height);
                        skCanvas.Scale(scale);
                        skCanvas.Translate(-bounds.Left, -bounds.Top);

                        // Draw the SVG onto the SKCanvas
                        using (var picture = svg.Picture)
                        {
                            skCanvas.DrawPicture(picture);
                        }
                    }

                    // Convert the SKBitmap to a System.Drawing.Bitmap
                    using (var image = SKImage.FromBitmap(skBitmap))
                    {
                        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                        {
                            using (var stream = new MemoryStream(data.ToArray()))
                            {
                                bitmap = new Bitmap(stream);
                            }
                        }
                    }
                }

                //var svgDoc = SvgDocument.FromSvg<SvgDocument>(svgString);

                //NLogManager.LogMessage("SVG: " + svgString);
                //bitmap = new Bitmap(605, 605, PixelFormat.Format32bppArgb);
                ////svgDoc.Draw(bitmap);

                //using (Graphics graphics = Graphics.FromImage(bitmap))
                //{
                //    graphics.Clear(Color.White);
                //    svgDoc.ViewBox = new SvgViewBox(0, 0, 605, 605);
                //    svgDoc.Draw(graphics);
                //}

                //using (Graphics graphics = Graphics.FromImage(bitmap))
                //{
                //    // Clear the bitmap with a white background
                //    graphics.Clear(Color.White);

                //    // Draw a red rectangle
                //    using (Pen pen = new Pen(Color.Red, 2))
                //    {
                //        Rectangle rect = new Rectangle(50, 50, 200, 100);
                //        graphics.DrawRectangle(pen, rect);
                //    }

                //    // Draw a blue ellipse
                //    using (Pen pen = new Pen(Color.Blue, 2))
                //    {
                //        Rectangle rect = new Rectangle(250, 250, 200, 100);
                //        graphics.DrawEllipse(pen, rect);
                //    }

                //    // Draw text
                //    using (Font font = new Font("Arial", 12))
                //    {
                //        string text = "Hello, Bitmap!";
                //        PointF location = new PointF(50, 400);
                //        graphics.DrawString(text, font, Brushes.Black, location);
                //    }
                //}

                byte[] bytes;
                using (var ms = new MemoryStream())
                { 
                    bitmap.Save(ms, ImageFormat.Png);
                    bytes = ms.ToArray();
                }
                string base64String = Convert.ToBase64String(bytes);

                return base64String;
            }
            catch (HttpRequestException e)
            {
                return "Exception :{0} " + e.Message;
            }
        }


        /// <summary>
        /// tạo tài khoản từ form đăng nhập hoặc từ facebook 
        /// </summary>
        /// <param name="input"></param>
        /// <returns>object Account Infor</returns>
        [Route("CreateAccount")]
        [AllowAnonymous]
        [HttpOptions, HttpPost]
        public dynamic CreateAccount([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                //if (ServiceID == 2)
                //{
                //    return new
                //    {
                //        ResponseCode = -10009,
                //        Message =ErrorMsg.STOPREGISTER
                //    };
                //}
                //lấy login type 1 0r 2
                int loginType = input.LoginType ?? -1;
                //  bool isRegistering = input.IsRegister != null ? Convert.ToBoolean(input.IsRegister.Value) : false;
                // string phoneNumber = input.PhoneNumber;
                //lấy user name
                string username = input.UserName ?? string.Empty;
                username = username != null ? username.ToLower() : string.Empty;
                //lấy nick name
                string nickname = input.NickName ?? string.Empty;
                if (!String.IsNullOrEmpty(username)) username = username.Trim();
                if (!String.IsNullOrEmpty(nickname)) nickname = nickname.Trim();
                //lấy password
                string password = input.Password ?? string.Empty;
                if (!String.IsNullOrEmpty(password)) password = password.Trim();
                //lấy device id mã của phone ,chorme ,fire fox
                string deviceId = input.DeviceId ?? string.Empty;
                //lấy device type (web,phone,ios,win phone 
                int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;
                //lấy mã private captca
                string privateKey = input.PrivateKey ?? string.Empty;
                //lấy mã capcha user nhập
                string captcha = input.Captcha ?? string.Empty;
                //lấy client ip
                string clientIp = IPAddressHelper.GetClientIP();
                string referUrl = input.ReferUrl ?? GetReferUrlByService();
                string InviteCode = input.InviteCode != null ? input.InviteCode : string.Empty;
                InviteCode = InviteCode.ToLower();
                int response = -99;
                Account registerAccount = new Account();
                //NLogManager.LogMessage("DEBUG: CreateAccount | " + InviteCode);
                //kiểm tra từng loại login
                switch (loginType)
                {
                    case (int)Constants.enmAuthenType.AUTHEN_ID://login từ form =1
                                                                //kiểm tra user name
                        if (username.Length < 6 || username.Length > 18)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.UserNameLength
                            };
                        }
                        if (username.Contains(" "))
                        {
                            return new
                            {
                                ResponseCode = -101,
                                Message = ErrorMsg.UserNameContainSpace
                            };
                        }
                        if (ValidateInput.IsNickNameContainNotAllowString(username))
                        {
                            return new
                            {
                                ResponseCode = -102,
                                Message = ErrorMsg.UserNameContainsNotAllowString
                            };
                        }
                        if (!ValidateInput.ValidateUserName(username))
                        {
                            return new
                            {
                                ResponseCode = -102,
                                Message = ErrorMsg.UsernameIncorrect
                            };
                        }
                        //kiểm tra mật khẩu 
                        if (String.IsNullOrEmpty(password))
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.PasswordEmpty
                            };
                        }
                        if (password.Length < 6 || password.Length > 16)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.PasswordLength
                            };
                        }
                        if (!ValidateInput.IsValidatePass(password))
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.PasswordIncorrect
                            };
                        }
                        // kiểm tra capcha
                        if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CapchaRequired
                            };
                        }
                        else
                        {
                            if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.InValidCaptCha
                                };
                            }
                        }
                        //kiểm tra số lần tạo acount
                        int IpResponse = 0;
                        AccountDAO.Instance.IPCheck(clientIp, out IpResponse);
                        if (IpResponse != 1)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.CreateAccountManyTime + clientIp + " " + IpResponse
                            };
                        }
                        //kiểm tra xem có tồn tại trong bảng user 
                        // int isExist;
                        //UserDAO.Instance.CheckUserExist(username, 1, out isExist);
                        //if (isExist != 1)
                        //{
                        //    return new
                        //    {
                        //        ResponseCode = -1005,
                        //        Message = ErrorMsg.UserNameInUse
                        //    };
                        //}
                        //kiểm tra trong bảng profile xem có tồn tại không 
                        int outResponse = 0;
                        AccountDAO.Instance.CheckAccountCheckExist(1, username, ServiceID, out outResponse);
                        if (outResponse != 1)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.UserNameAndPasswordExist
                            };
                        }
                        long UserId = 0;
                        ///get current user id
                        UserDAO.Instance.UserGetSequence(out UserId);
                        if (UserId <= 0)
                        {
                            return new
                            {
                                ResponseCode = -99,
                                Message = ErrorMsg.InProccessException
                            };
                        }
                        //đồng bộ sang chợ sao
                        //ChoSaoDAO.Instance.UserChoSaoInsert(username, null, null, null, "", Security.SHA256Encrypt(password), null, out resChoSao, UserId);
                        //if (resChoSao == 1)
                        //{
                        //tạo account nếu response =1 thì success
                        bool isLanding = input.IsLanding ?? false;
                        
                        registerAccount = AccountDAO.Instance.CreateAccount(UserId, loginType, deviceType, username, Security.SHA256Encrypt(password), clientIp, 1, 1, null, null, ServiceID, password, out response, isLanding);
                        if (response == -102)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.UserNameAndPasswordExist
                            };
                        }
                        else if (response == 1 && registerAccount != null)
                        {
                            NLogManager.LogMessage("DEBUG: " + response + " | " + registerAccount.AccountID.ToString());
                            if (InviteCode.Length > 0)
                            //if (true)
                            {
                                //var httpclient = new HttpClient();
                                //httpclient.GetStringAsync("https://ag.sicbet.net/api/UpdateInviteCode?inviteCode=" + InviteCode + "&userId=" + registerAccount.AccountID.ToString()).Wait();
                                var request = (HttpWebRequest)WebRequest.Create("https://ag.sicbet.net/api/UpdateInviteCode?inviteCode=" + InviteCode + "&userId=" + registerAccount.AccountID.ToString());
                                request.Method = "Get";
                                var response1 = (HttpWebResponse)request.GetResponse();
                                var responseString = new StreamReader(response1.GetResponseStream()).ReadToEnd();
                                NLogManager.LogMessage("https://ag.sicbet.net/api/UpdateInviteCode?inviteCode=" + InviteCode + "&userId=" + registerAccount.AccountID.ToString() + " | REG : " + responseString);
                            }
                            registerAccount.PhoneNumber = registerAccount.PhoneNumber.PhoneDisplayFormat();
                            int responseLog = 0;
                            //log lai ip khi  khi đăng kí
                            AccountDAO.Instance.IPLog(2, registerAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, null, out responseLog);
                            //add refer

                            AddRefer(referUrl, loginType, registerAccount.AccountID, clientIp, deviceType);
                            
                            
                            
                            //end simple http get request
                            //if (responseString.Contains("true"))
                            //{
                            //    return new
                            //    {
                            //        ResponseCode = 1,
                            //        Token = TokenHashprovider.GenerateToken(registerAccount.AccountID, registerAccount.AccountName, ServiceID, clientIp, TokenExpired, registerAccount.AvatarID, deviceType, deviceId),
                            //        AccountInfo = registerAccount
                            //    };
                            //}
                            //else
                            //{
                            //    return new
                            //    {
                            //        ResponseCode = -100,
                            //        Message = ErrorMsg.InProccessException
                            //    };
                            //}
                            return new
                            {
                                ResponseCode = 1,
                                Token = TokenHashprovider.GenerateToken(registerAccount.AccountID, registerAccount.AccountName, ServiceID, clientIp, TokenExpired, registerAccount.AvatarID, deviceType, deviceId),
                                AccountInfo = registerAccount
                            };
                        }
                        else if (response == -102)
                        {
                            return new
                            {
                                ResponseCode = -100,
                                Message = ErrorMsg.UserNameAndPasswordExist
                            };
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -100,
                                Message = ErrorMsg.InProccessException
                            };
                        }
                        break;


                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        /// <summary>
        /// tạo tài khoản từ form đăng nhập hoặc từ facebook 
        /// </summary>
        /// <param name="input"></param>
        /// <returns>object Account Infor</returns>
        [Route("CreateAccountAUTO")]
        [AllowAnonymous]
        [HttpOptions, HttpPost]
        public dynamic CreateAccountAUTO([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                //if (ServiceID == 2)
                //{
                //    return new
                //    {
                //        ResponseCode = -10009,
                //        Message =ErrorMsg.STOPREGISTER
                //    };
                //}
                //lấy login type 1 0r 2
                int loginType = input.LoginType ?? -1;
                //  bool isRegistering = input.IsRegister != null ? Convert.ToBoolean(input.IsRegister.Value) : false;
                // string phoneNumber = input.PhoneNumber;
                //lấy user name
                string username = input.UserName ?? string.Empty;
                username = username != null ? username.ToLower() : string.Empty;
                //lấy nick name
                string nickname = input.NickName ?? string.Empty;
                if (!String.IsNullOrEmpty(username)) username = username.Trim();
                if (!String.IsNullOrEmpty(nickname)) nickname = nickname.Trim();
                //lấy password
                string password = input.Password ?? string.Empty;
                if (!String.IsNullOrEmpty(password)) password = password.Trim();
                //lấy device id mã của phone ,chorme ,fire fox
                string deviceId = input.DeviceId ?? string.Empty;
                //lấy device type (web,phone,ios,win phone 
                int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;
                //lấy mã private captca
                string privateKey = input.PrivateKey ?? string.Empty;
                //lấy mã capcha user nhập
                string captcha = input.Captcha ?? string.Empty;
                //lấy client ip
                string clientIp = IPAddressHelper.GetClientIP();
                string referUrl = input.ReferUrl ?? GetReferUrlByService();
                int response = -99;
                Account registerAccount = new Account();
                //kiểm tra từng loại login
                switch (loginType)
                {
                    case (int)Constants.enmAuthenType.AUTHEN_ID://login từ form =1
                                                                //kiểm tra user name
                        if (username.Length < 6 || username.Length > 18)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.UserNameLength
                            };
                        }
                        if (username.Contains(" "))
                        {
                            return new
                            {
                                ResponseCode = -101,
                                Message = ErrorMsg.UserNameContainSpace
                            };
                        }
                        if (ValidateInput.IsNickNameContainNotAllowString(username))
                        {
                            return new
                            {
                                ResponseCode = -102,
                                Message = ErrorMsg.UserNameContainsNotAllowString
                            };
                        }
                        if (!ValidateInput.ValidateUserName(username))
                        {
                            return new
                            {
                                ResponseCode = -102,
                                Message = ErrorMsg.UsernameIncorrect
                            };
                        }
                        //kiểm tra mật khẩu 
                        if (String.IsNullOrEmpty(password))
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.PasswordEmpty
                            };
                        }
                        if (password.Length < 6 || password.Length > 16)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.PasswordLength
                            };
                        }
                        if (!ValidateInput.IsValidatePass(password))
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.PasswordIncorrect
                            };
                        }
                        //Disable capcha, privateKey, etc for AUTO creator
                        // kiểm tra capcha
                        //if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                        //{
                        //    return new
                        //    {
                        //        ResponseCode = -1005,
                        //        Message = ErrorMsg.CapchaRequired
                        //    };
                        //}
                        //else
                        //{
                        //    if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
                        //    {
                        //        return new
                        //        {
                        //            ResponseCode = -1005,
                        //            Message = ErrorMsg.InValidCaptCha
                        //        };
                        //    }
                        //}
                        ////kiểm tra số lần tạo acount
                        //int IpResponse = 0;
                        ////disable IPCHECK for AUTO Creator
                        ////AccountDAO.Instance.IPCheck(clientIp, out IpResponse);
                        //if (IpResponse != 1)
                        //{
                        //    return new
                        //    {
                        //        ResponseCode = -1005,
                        //        Message = ErrorMsg.CreateAccountManyTime + clientIp + " " + IpResponse
                        //    };
                        //}
                        //kiểm tra xem có tồn tại trong bảng user 
                        // int isExist;
                        //UserDAO.Instance.CheckUserExist(username, 1, out isExist);
                        //if (isExist != 1)
                        //{
                        //    return new
                        //    {
                        //        ResponseCode = -1005,
                        //        Message = ErrorMsg.UserNameInUse
                        //    };
                        //}
                        //kiểm tra trong bảng profile xem có tồn tại không 
                        int outResponse = 0;
                        AccountDAO.Instance.CheckAccountCheckExist(1, username, ServiceID, out outResponse);
                        if (outResponse != 1)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.UserNameAndPasswordExist
                            };
                        }
                        long UserId = 0;
                        ///get current user id
                        UserDAO.Instance.UserGetSequence(out UserId);
                        if (UserId <= 0)
                        {
                            return new
                            {
                                ResponseCode = -99,
                                Message = ErrorMsg.InProccessException
                            };
                        }
                        //đồng bộ sang chợ sao
                        //ChoSaoDAO.Instance.UserChoSaoInsert(username, null, null, null, "", Security.SHA256Encrypt(password), null, out resChoSao, UserId);
                        //if (resChoSao == 1)
                        //{
                        //tạo account nếu response =1 thì success
                        bool isLanding = input.IsLanding ?? false;
                        registerAccount = AccountDAO.Instance.CreateAccountAUTO(UserId, loginType, deviceType, username, Security.SHA256Encrypt(password), clientIp, 1, 1, null, null, ServiceID, password, out response, isLanding);
                        if (response == -102)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.UserNameAndPasswordExist
                            };
                        }
                        else if (response == 1 && registerAccount != null)
                        {
                            registerAccount.PhoneNumber = registerAccount.PhoneNumber.PhoneDisplayFormat();
                            int responseLog = 0;
                            //log lai ip khi  khi đăng kí
                            AccountDAO.Instance.IPLog(2, registerAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, null, out responseLog);
                            //add refer

                            AddRefer(referUrl, loginType, registerAccount.AccountID, clientIp, deviceType);

                            return new
                            {
                                ResponseCode = 1,
                                Token = TokenHashprovider.GenerateToken(registerAccount.AccountID, registerAccount.AccountName, ServiceID, clientIp, TokenExpired, registerAccount.AvatarID, deviceType, deviceId),
                                AccountInfo = registerAccount
                            };
                        }
                        else if (response == -102)
                        {
                            return new
                            {
                                ResponseCode = -100,
                                Message = ErrorMsg.UserNameAndPasswordExist
                            };
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -100,
                                Message = ErrorMsg.InProccessException
                            };
                        }
                        break;


                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        /// <summary>
        /// đăng nhập tài khoản
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("Login")]
        [Route("Login")]
        [HttpOptions, HttpPost]
        public dynamic Login([FromBody] dynamic input)
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                int loginType = input.LoginType ?? -1;
                int response = -99;
                int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : 1;
                Account loginAccount = new Account();
                string clientIp = IPAddressHelper.GetClientIP();
                string deviceId = input.DeviceId ?? string.Empty;//lấy ra deviceId
                string otp = input.Otp ?? String.Empty;
                switch (loginType)
                {

                    case (int)Constants.enmAuthenType.AUTHEN_ID:
                        string msg;
                        int resCode;
                        string username = input.UserName ?? string.Empty;
                        username = username != null ? username.ToLower() : string.Empty;//lấy ra userme
                        if (!String.IsNullOrEmpty(username)) username = username.Trim();
                        string password = input.Password ?? string.Empty;//lấy ra password
                        if (!String.IsNullOrEmpty(password)) password = password.Trim();

                        string privateKey = input.PrivateKey ?? string.Empty;//lấy ra privte key
                        string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
                        if (!ValidateInput.IsValidated(username, password, out msg, out resCode))
                        {
                            return new
                            {
                                ResponseCode = resCode,
                                Message = msg
                            };
                        }
                        int numberLogin = CacheLogin.CheckStatusFrequency(username, ServiceID, "login");
                        if (numberLogin >= 3)
                        {
                            if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                            {
                                return new
                                {
                                    ResponseCode = -4,
                                    Message = ErrorMsg.CapchaRequired
                                };
                            }
                            else
                            {
                                if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
                                {
                                    return new
                                    {
                                        ResponseCode = -100,
                                        Message = ErrorMsg.InValidCaptCha
                                    };
                                }
                                //clear cache khi login thành công
                                CacheLogin.ClearCache(username, ServiceID, "login");
                            }
                        }

                        loginAccount = AccountDAO.Instance.UserLogin(username, Security.SHA256Encrypt(password), loginType, deviceType, ServiceID, out response);
                        if ((loginAccount != null && (loginAccount.Status != 1 && loginAccount.Status != 3)) || response == -109)//tài khoản bị khóa
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.AccountLock
                            };
                        }
                        if (loginAccount == null)
                        {
                            if (numberLogin < 3)//nếu login sai thì add vào cache
                            {
                                CacheLogin.AddCache(120, username, ServiceID, "login");
                            }
                        }
                        if (response == 0)//mã lỗi user name or pass incorrect
                        {
                            return new
                            {
                                ResponseCode = -1,
                                Message = ErrorMsg.UserNameOrPassInCorrect
                            };
                        }
                        else if (loginAccount == null)
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.UserNameOrPassInCorrect
                            };
                        }
                        else if (response == 1 && loginAccount != null)//nếu login thành công
                        {
                            if (loginAccount.AuthenType == 1 && String.IsNullOrEmpty(otp))//kiểm tra authen type và opt (lần đầu login yêu cầu otp)
                            {
                                return new
                                {
                                    ResponseCode = -3,
                                    Message = ErrorMsg.OtpEmpty
                                };
                            }
                            else if ((loginAccount.AuthenType == 1 && !String.IsNullOrEmpty(otp)))//truyền lại otp
                            {
                                if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
                                {
                                    return new
                                    {
                                        ResponseCode = -1005,
                                        Message = ErrorMsg.OtpLengthInValid
                                    };
                                }
                                int resOtp = 0;
                                long otpID = 0;
                                //SMSDAO.Instance.ValidOtp(loginAccount.AccountID, otp.Length == OTPSAFE_LENGTH ? loginAccount.PhoneSafeNo : loginAccount.PhoneNumber, otp, ServiceID, out resOtp, out otpID, out msg);//otp valid 
                                SMSDAO.Instance.ValidOtpFistTele(loginAccount.AccountID, otp.Length == OTPSAFE_LENGTH ? loginAccount.PhoneSafeNo : loginAccount.PhoneNumber, otp, ServiceID, out resOtp, out otpID, out msg);//otp valid 
                                if (resOtp != 1)//otp not success
                                {
                                    return new
                                    {
                                        ResponseCode = -5,
                                        Message = ErrorMsg.OtpIncorrect
                                    };
                                }

                            }
                            else if (response == -99)//mã lỗi hệ thống bận
                            {
                                return new
                                {
                                    ResponseCode = -99,
                                    Message = ErrorMsg.InProccessException
                                };
                            }
                            CacheLogin.ClearCache(username, ServiceID, "login");
                            int responseLog = 0;
                            AccountDAO.Instance.IPLog(1, loginAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, loginAccount.IsFB == 1 ? FB_CHANGE_LOG : null, out responseLog);
                            var userRank = AccountDAO.Instance.GetUserRank(loginAccount.AccountID, out responseLog);
                            loginAccount.RankID = userRank == null ? VipIndex : userRank.RankID;
                            loginAccount.VP = userRank == null ? 0 : userRank.VP;
                            loginAccount.PhoneNumber = loginAccount.PhoneNumber.PhoneDisplayFormat();
                            loginAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
                            //Lấy ra next Vip
                            var nextVP = GetNextVIPInfor(loginAccount.RankID);

                            // SetAuthCookie(loginAccount);
                            return new
                            {
                                ResponseCode = 1,
                                Token = TokenHashprovider.GenerateToken(loginAccount.AccountID, loginAccount.AccountName, ServiceID, clientIp, TokenExpired, loginAccount.AvatarID, deviceType, deviceId),
                                AccountInfo = loginAccount,
                                NextVIP = nextVP
                            };
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -99,
                                Message = ErrorMsg.InProccessException
                            };
                        }

                    case (int)Constants.enmAuthenType.AUTHEN_FB://trường hợp login facebok
                        string accessToken = input.AccessToken ?? string.Empty;
                        if (accessToken != null)
                        {
                            string fbid = string.Empty;
                            var acceptBusinessIDS = new List<int> { 1, 2, 3 };
                            if (acceptBusinessIDS.Contains(ServiceID))
                            {
                                var list = FacebookUtil.GetIDsForBusiness(accessToken);
                                if (list == null || !list.Any())
                                {
                                    return new
                                    {
                                        ResponseCode = -103,
                                        Message = ErrorMsg.FacebookGetFail
                                    };
                                }
                                string appID = ConfigurationManager.AppSettings["APP_FB_ID"].ToString().Trim();
                                var userFBInfo = list.FirstOrDefault(c => c.app.id == appID);
                                if (userFBInfo == null || userFBInfo.app == null)
                                {
                                    return new
                                    {
                                        ResponseCode = -103,
                                        Message = ErrorMsg.FacebookGetFail
                                    };
                                }
                                fbid = userFBInfo.id.ToString();
                            }
                            else
                            {
                                FacebookUtil.FbUserInfo userFBInfo = FacebookUtil.GetFbUserInfo(accessToken);//lấy thông tin facebook
                                if (userFBInfo == null || userFBInfo.ResponeCode < 0)//không thể lấy được facebook
                                {
                                    return new
                                    {
                                        ResponseCode = -103,
                                        Message = ErrorMsg.FacebookGetFail
                                    };
                                }
                                fbid = userFBInfo.UserId.ToString();//lấy facebook id
                            }
                            if (string.IsNullOrEmpty(fbid))
                            {
                                return new
                                {
                                    ResponseCode = -104,
                                    Message = ErrorMsg.FacebookGetFail
                                };
                            }

                            loginAccount = AccountDAO.Instance.UserLoginFB(fbid, string.Empty, loginType, deviceType, ServiceID, out response);//login facebook
                            if ((loginAccount != null && (loginAccount.Status != 1 && loginAccount.Status != 3)) || response == -109)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.AccountLock
                                };
                            }
                            if (loginAccount == null)//không tồn tại user facebook
                            {
                                //if (ServiceID == 2)
                                //{
                                //    return new
                                //    {
                                //        ResponseCode = -10009,
                                //        Message = ErrorMsg.STOPREGISTER
                                //    };
                                //}
                                //kiểm tra số lần tạo acount
                                int IpResponse = 0;
                                AccountDAO.Instance.IPCheck(clientIp, out IpResponse);
                                if (IpResponse != 1)
                                {
                                    return new
                                    {
                                        ResponseCode = -1005,
                                        Message = ErrorMsg.CreateAccountManyTime
                                    };
                                }
                                ///get current user id
                                long UserId = 0;
                                UserDAO.Instance.UserGetSequence(out UserId);
                                if (UserId <= 0)
                                {
                                    return new
                                    {
                                        ResponseCode = -99,
                                        Message = ErrorMsg.InProccessException
                                    };
                                }
                                //tạo mới facebook account
                                string referUrl = input.ReferUrl ?? GetReferUrlByService();
                                //int res = 0;
                                //ChoSaoDAO.Instance.UserChoSaoInsert(null, null, null, null, "", string.Empty, fbid, out res, UserId);
                                //if (res == 1)
                                //{
                                int outResponse = 0;
                                var registerAccount = AccountDAO.Instance.CreateAccount(UserId, loginType, deviceType, fbid, string.Empty, clientIp, 1, 1, null, null, ServiceID, null, out outResponse, false);
                                //đồng bộ sang chợ sao
                                if (outResponse == 1 && registerAccount != null)
                                {
                                    int responseLog = 0;
                                    AccountDAO.Instance.IPLog(2, registerAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, null, out responseLog);
                                    var userRank = AccountDAO.Instance.GetUserRank(registerAccount.AccountID, out outResponse);
                                    registerAccount.RankID = userRank == null ? VipIndex : userRank.RankID;
                                    registerAccount.PhoneNumber = registerAccount.PhoneNumber.PhoneDisplayFormat();
                                    registerAccount.VP = userRank == null ? 0 : userRank.VP;
                                    registerAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
                                    registerAccount.IsUpdatedFB = 1;
                                    //set cookies
                                    // SetAuthCookie(registerAccount);
                                    var nextVP = GetNextVIPInfor(registerAccount.RankID);
                                    AddRefer(referUrl, loginType, registerAccount.AccountID, clientIp, deviceType);
                                    return new
                                    {
                                        ResponseCode = 1,
                                        Token = TokenHashprovider.GenerateToken(registerAccount.AccountID, registerAccount.AccountName ?? string.Empty, ServiceID, clientIp, TokenExpired, registerAccount.AvatarID, deviceType, deviceId),
                                        AccountInfo = registerAccount,
                                        NextVIP = nextVP
                                    };
                                }
                                else if (outResponse == -102)
                                {
                                    return new
                                    {
                                        ResponseCode = -1005,
                                        Message = ErrorMsg.FBRegister
                                    };
                                }
                                else
                                {
                                    //tạo không được báo lỗi hệ thống
                                    NLogManager.LogMessage("Msg " + outResponse);
                                    return new
                                    {
                                        ResponseCode = -99,
                                        Message = ErrorMsg.InProccessException
                                    };
                                }
                                //}
                                //else if (res == -102)
                                //{
                                //    return new
                                //    {
                                //        ResponseCode = -1005,
                                //        Message = ErrorMsg.FBRegister
                                //    };
                                //}
                                //else
                                //{
                                //    return new
                                //    {
                                //        ResponseCode = -99,
                                //        Message = ErrorMsg.InProccessException
                                //    };
                                //}
                            }
                            //login facebook thành công
                            else if (loginAccount != null)//login facebook thành công
                            {

                                if (loginAccount.AuthenType == 1 && String.IsNullOrEmpty(otp))//otp nếu authen type
                                {
                                    return new
                                    {
                                        ResponseCode = -3,
                                        Message = ErrorMsg.OtpEmpty
                                    };
                                }
                                else if ((loginAccount.AuthenType == 1 && !String.IsNullOrEmpty(otp)))//yêu cầu otp
                                {
                                    if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
                                    {
                                        return new
                                        {
                                            ResponseCode = -1005,
                                            Message = ErrorMsg.OtpLengthInValid
                                        };
                                    }
                                    int resOtp = 0;
                                    long otpID;
                                    string otmsg;
                                    SMSDAO.Instance.ValidOtp(loginAccount.AccountID, otp.Length == OTPSAFE_LENGTH ? loginAccount.PhoneSafeNo : loginAccount.PhoneNumber, otp.Trim(), ServiceID, out resOtp, out otpID, out otmsg);
                                    if (resOtp != 1)
                                    {
                                        return new
                                        {
                                            ResponseCode = -5,
                                            Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                                        };
                                    }

                                }

                                //set cookies
                                int responseLog = 0;
                                AccountDAO.Instance.IPLog(1, loginAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, NORMAL_LOG, out responseLog);
                                var userRank = AccountDAO.Instance.GetUserRank(loginAccount.AccountID, out responseLog);
                                loginAccount.RankID = userRank == null ? VipIndex : userRank.RankID;
                                loginAccount.VP = userRank == null ? 0 : userRank.VP;
                                loginAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
                                loginAccount.IsUpdatedFB = 1;
                                var nextVP = GetNextVIPInfor(loginAccount.RankID);
                                //SetAuthCookie(loginAccount);
                                return new
                                {
                                    ResponseCode = 1,
                                    Token = TokenHashprovider.GenerateToken(loginAccount.AccountID, loginAccount.AccountName, ServiceID, clientIp, TokenExpired, loginAccount.AvatarID, deviceType, deviceId),
                                    AccountInfo = loginAccount,
                                    NextVIP = nextVP,
                                };
                            }
                            else //lỗi hệ thống khi login 
                            {
                                NLogManager.LogMessage("Msg" + 2);
                                return new
                                {
                                    ResponseCode = -99,
                                    Message = ErrorMsg.InProccessException
                                };
                            }
                        }
                        break;
                    case (int)Constants.enmAuthenType.LIVE69: //  hợp login live69
                        username = input.UserName ?? string.Empty;
                        username = username != null ? username.ToLower() : string.Empty;//lấy ra userme
                        if (!String.IsNullOrEmpty(username)) username = username.Trim();
                        password = input.Password ?? string.Empty;//lấy ra password
                        if (!String.IsNullOrEmpty(password)) password = password.Trim();

                        privateKey = input.PrivateKey ?? string.Empty;//lấy ra privte key
                        captcha = input.Captcha ?? string.Empty;//lấy ra capcha
                        //khoa cong game cu~
                        //var res = JsonConvert.DeserializeObject<dynamic>(Get("https://oknha.vip/api/public/?service=User.getBaseInfo&token=" + username + "&uid=" + password));
                        var res = JsonConvert.DeserializeObject<dynamic>("");
                        username = "69live" +res["data"]["info"][0]["id"];
                        string displayname =  res["data"]["info"][0]["user_nicename"];

                        loginAccount = AccountDAO.Instance.UserLogin(username, Security.SHA256Encrypt(password), loginType, deviceType, ServiceID, out response);
                        if ((loginAccount != null && (loginAccount.Status != 1 && loginAccount.Status != 3)) || response == -109)//tài khoản bị khóa
                        {
                            return new
                            {
                                ResponseCode = -1005,
                                Message = ErrorMsg.AccountLock
                            };
                        }
                        if (response == 0)//mã lỗi user name or pass incorrect
                        {
                            return new
                            {
                                ResponseCode = -1,
                                Message = ErrorMsg.UserNameOrPassInCorrect
                            };
                        }
                        else if (loginAccount == null)
                        {
                            int IpResponse = 0;
                            AccountDAO.Instance.IPCheck(clientIp, out IpResponse);
                            if (IpResponse != 1)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.CreateAccountManyTime
                                };
                            }
                            ///get current user id
                            long UserId = 0;
                            UserDAO.Instance.UserGetSequence(out UserId);
                            if (UserId <= 0)
                            {
                                return new
                                {
                                    ResponseCode = -99,
                                    Message = ErrorMsg.InProccessException
                                };
                            }
                            //tạo mới facebook account
                            string referUrl = input.ReferUrl ?? GetReferUrlByService();
                            //int res = 0;
                            //ChoSaoDAO.Instance.UserChoSaoInsert(null, null, null, null, "", string.Empty, fbid, out res, UserId);
                            //if (res == 1)
                            //{
                            int outResponse = 0;
                            var registerAccount = AccountDAO.Instance.CreateAccount(UserId, loginType, deviceType, username, string.Empty, clientIp, 1, 1, null, username, ServiceID, null, out outResponse, false);
                            //đồng bộ sang chợ sao
                            if (outResponse == 1 && registerAccount != null)
                            {
                                int responseLog = 0;
                                AccountDAO.Instance.IPLog(2, registerAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, null, out responseLog);
                                var userRank = AccountDAO.Instance.GetUserRank(registerAccount.AccountID, out outResponse);
                                registerAccount.RankID = userRank == null ? VipIndex : userRank.RankID;
                                registerAccount.PhoneNumber = registerAccount.PhoneNumber.PhoneDisplayFormat();
                                registerAccount.VP = userRank == null ? 0 : userRank.VP;
                                registerAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
                                registerAccount.IsUpdatedFB = 1;
                                //set cookies
                                // SetAuthCookie(registerAccount);
                                var nextVP = GetNextVIPInfor(registerAccount.RankID);
                                AddRefer(referUrl, loginType, registerAccount.AccountID, clientIp, deviceType);
                                return new
                                {
                                    ResponseCode = 1,
                                    Token = TokenHashprovider.GenerateToken(registerAccount.AccountID, registerAccount.AccountName ?? string.Empty, ServiceID, clientIp, TokenExpired, registerAccount.AvatarID, deviceType, deviceId),
                                    AccountInfo = registerAccount,
                                    NextVIP = nextVP
                                };
                            }
                            else if (outResponse == -102)
                            {
                                return new
                                {
                                    ResponseCode = -1005,
                                    Message = ErrorMsg.FBRegister
                                };
                            }
                            else
                            {
                                //tạo không được báo lỗi hệ thống
                                NLogManager.LogMessage("Msg " + outResponse);
                                return new
                                {
                                    ResponseCode = -99,
                                    Message = ErrorMsg.InProccessException
                                };
                            }
                        }
                        else if (response == 1 && loginAccount != null)//nếu login thành công
                        {
                            if (loginAccount.AuthenType == 1 && String.IsNullOrEmpty(otp))//kiểm tra authen type và opt (lần đầu login yêu cầu otp)
                            {
                                return new
                                {
                                    ResponseCode = -3,
                                    Message = ErrorMsg.OtpEmpty
                                };
                            }
                            else if ((loginAccount.AuthenType == 1 && !String.IsNullOrEmpty(otp)))//truyền lại otp
                            {
                                if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
                                {
                                    return new
                                    {
                                        ResponseCode = -1005,
                                        Message = ErrorMsg.OtpLengthInValid
                                    };
                                }
                                int resOtp = 0;
                                long otpID = 0;
                                SMSDAO.Instance.ValidOtp(loginAccount.AccountID, otp.Length == OTPSAFE_LENGTH ? loginAccount.PhoneSafeNo : loginAccount.PhoneNumber, otp, ServiceID, out resOtp, out otpID, out msg);//otp valid 
                                if (resOtp != 1)//otp not success
                                {
                                    return new
                                    {
                                        ResponseCode = -5,
                                        Message = ErrorMsg.OtpIncorrect
                                    };
                                }

                            }
                            else if (response == -99)//mã lỗi hệ thống bận
                            {
                                return new
                                {
                                    ResponseCode = -99,
                                    Message = ErrorMsg.InProccessException
                                };
                            }
                            CacheLogin.ClearCache(username, ServiceID, "login");
                            int responseLog = 0;
                            AccountDAO.Instance.IPLog(1, loginAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, loginAccount.IsFB == 1 ? FB_CHANGE_LOG : null, out responseLog);
                            var userRank = AccountDAO.Instance.GetUserRank(loginAccount.AccountID, out responseLog);
                            loginAccount.RankID = userRank == null ? VipIndex : userRank.RankID;
                            loginAccount.VP = userRank == null ? 0 : userRank.VP;
                            loginAccount.PhoneNumber = loginAccount.PhoneNumber.PhoneDisplayFormat();
                            loginAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
                            //Lấy ra next Vip
                            var nextVP = GetNextVIPInfor(loginAccount.RankID);

                            // SetAuthCookie(loginAccount);
                            return new
                            {
                                ResponseCode = 1,
                                Token = TokenHashprovider.GenerateToken(loginAccount.AccountID, loginAccount.AccountName, ServiceID, clientIp, TokenExpired, loginAccount.AvatarID, deviceType, deviceId),
                                AccountInfo = loginAccount,
                                NextVIP = nextVP
                            };
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -99,
                                Message = ErrorMsg.InProccessException
                            };
                        }

                        break;

                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                NLogManager.LogError("Line" + line);
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        /// <summary>
        /// POST :thay đổi mật khẩu user hiện tại
        /// </summary>
        /// <param name="oldpass">password cũ </param>
        /// <param name="newpass">password mới</param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("ChangePassword")]
        public dynamic ChangePassword([FromBody] dynamic input)
        {
            try
            {
                //kiểm tra lại cách lấy accountId
                var accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                var tkServiceID = AccountSession.ServiceID;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (String.IsNullOrEmpty(user.PhoneNumber))
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.PhoneNotRegister
                    };
                }
                string oldpass = input.OldPass ?? string.Empty;
                string newpass = input.NewPass ?? string.Empty;
                string privateKey = input.PrivateKey ?? string.Empty;//lấy ra privte key
                string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
                //old pass độ dài không thích hợp
                if (oldpass.Length < 6 || oldpass.Length > 16)
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.OldPasswordLength
                    };
                }
                //new pass độ dài không thích hợp
                if (newpass.Length < 6 || newpass.Length > 16)
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.NewPasswordLength
                    };
                }
                if (!ValidateInput.IsValidatePass(newpass))
                {
                    return new
                    {
                        ResponseCode = -101,
                        Message = ErrorMsg.NewPasswordFormat,
                    };
                }
                oldpass = oldpass.Trim();
                newpass = newpass.Trim();
                // mật khẩu trùng
                if (oldpass == newpass)
                {
                    return new
                    {
                        ResponseCode = -101,
                        Message = ErrorMsg.OldPasswordEqualNewPassword,
                    };
                }
                if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                {
                    return new
                    {
                        ResponseCode = -4,
                        Message = ErrorMsg.CapchaRequired
                    };
                }
                else
                {
                    if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
                    {
                        return new
                        {
                            ResponseCode = -100,
                            Message = ErrorMsg.InValidCaptCha
                        };
                    }
                }
                //lấy ra mã otp
                // cập nhật dbs
                string otp = input.Otp;
                if (string.IsNullOrEmpty(otp))
                {
                    return new
                    {
                        ResponseCode = -203,
                        Message = ErrorMsg.OtpEmpty
                    };
                }
                otp = otp.Trim();
                if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpLengthInValid
                    };
                }
                int resOtp;
                long otpID;
                var newPhone = user.PhoneNumber.PhoneFormat();
                string otmsg;
                SMSDAO.Instance.ValidOtp(accountId, otp.Length == OTPSAFE_LENGTH ? user.PhoneSafeNo : newPhone, otp, ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
                int response = AccountDAO.Instance.ChangePassword(accountId, Security.SHA256Encrypt(oldpass), Security.SHA256Encrypt(newpass), null, newpass, ServiceID);
                if (response == 0)//trả về 0 nếu mật khẩu cũ không đúng
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OldPassWorldNotCorrect,
                    };
                }
                else if (response == -100)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneEmpty,
                    };
                }
                else if (response <= 0)//lỗi exception
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    };
                }
                else if (response > 0)//cập nhật thành công
                {
                    int resChoSao = 0;
                    // ChoSaoDAO.Instance.UpdateProfile(accountId, null, null, null, null, Security.SHA256Encrypt(newpass), out resChoSao);
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.PasswordChangeSuccess,
                    };
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        /// <summary>
        /// POST :thay đổi mật khẩu user hiện tại
        /// </summary>
        /// <param name="oldpass">password cũ </param>
        /// <param name="newpass">password mới</param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("ChangeAgent")]
        public dynamic ChangeAgent([FromBody] dynamic input)
        {

            try
            {
                //kiểm tra lại cách lấy accountId
                var accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                var tkServiceID = AccountSession.ServiceID;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);

                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                string InviteCode = input.InviteCode ?? string.Empty;
                string privateKey = input.PrivateKey ?? string.Empty;//lấy ra privte key
                string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
                //old pass độ dài không thích hợp
                if (InviteCode.Length < 1 || InviteCode.Length > 20)
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = "Inappropriate invite code length"
                    };
                }

                InviteCode = InviteCode.Trim();
                
                if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                {
                    return new
                    {
                        ResponseCode = -4,
                        Message = ErrorMsg.CapchaRequired
                    };
                }
                else
                {
                    if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
                    {
                        return new
                        {
                            ResponseCode = -100,
                            Message = ErrorMsg.InValidCaptCha
                        };
                    }
                }

                var request = (HttpWebRequest)WebRequest.Create("https://ag.sicbet.net/api/UpdateInviteCode?inviteCode=" + InviteCode + "&userId=" + accountId.ToString());
                request.Method = "Get";
                var response1 = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response1.GetResponseStream()).ReadToEnd();
                response1.Close();

                if (responseString.Contains("false"))
                {
                    var errmsg = "Update failed";

                    if (responseString.Contains("Mã giới thiệu không tồn tại")) errmsg = "Referral code does not exist";
                    if (responseString.Contains("Người dùng không tồn tại")) errmsg = "User does not exist";
                    if (responseString.Contains("Người dùng đã tham gia đại lý")) errmsg = "User already joined agent";

                    return new
                    {
                        ResponseCode = -1005,
                        Message = errmsg,
                    };
                } else
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Update invite code successfully",
                    };
                }

                //int response = -99;

                //AccountDAO.Instance.UpdateAgent(accountId, 0, ServiceID, out response);
                //if (response == 0)//trả về 0 nếu mật khẩu cũ không đúng
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.OldPassWorldNotCorrect,
                //    };
                //}
                //else if (response == -100)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.PhoneEmpty,
                //    };
                //}
                //else if (response <= 0)//lỗi exception
                //{
                //    return new
                //    {
                //        ResponseCode = -99,
                //        Message = ErrorMsg.InProccessException
                //    };
                //}
                //else if (response > 0)//cập nhật thành công
                //{
                //    return new
                //    {
                //        ResponseCode = 1,
                //        Message = "Cập nhật invite code thành công",
                //    };
                //}
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }

        [HttpOptions, HttpPost]
        [Route("ChangeFBInfor")]
        public dynamic ChangeFBInfor([FromBody] dynamic input)
        {
            try
            {
                //kiểm tra lại cách lấy accountId
                var accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                var tkServiceID = AccountSession.ServiceID;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                if (user.IsFB != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountNotFB
                    };
                }
                if (user.IsUpdatedFB == 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountUpdateFBOneTime
                    };
                }
                string username = input.UserName ?? string.Empty;
                username = username != null ? username.ToLower() : string.Empty;
                if (username.Length < 6 || username.Length > 18)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserNameLength
                    };
                }
                if (username.Contains(" "))
                {
                    return new
                    {
                        ResponseCode = -101,
                        Message = ErrorMsg.UserNameContainSpace
                    };
                }
                if (ValidateInput.IsNickNameContainNotAllowString(username))
                {
                    return new
                    {
                        ResponseCode = -102,
                        Message = ErrorMsg.UserNameContainsNotAllowString
                    };
                }
                if (!ValidateInput.ValidateUserName(username))
                {
                    return new
                    {
                        ResponseCode = -102,
                        Message = ErrorMsg.UsernameIncorrect
                    };
                }
                int outResponse = 0;
                AccountDAO.Instance.CheckAccountCheckExist(1, username, ServiceID, out outResponse);
                if (outResponse != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserNameAndPasswordExist
                    };
                }
                string Password = input.Password ?? string.Empty;

                //old pass độ dài không thích hợp
                if (Password.Length < 6 || Password.Length > 16)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PasswordLength
                    };
                }
                //new pass độ dài không thích hợp

                if (!ValidateInput.IsValidatePass(Password))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PasswordIncorrect,
                    };
                }
                Password = Password.Trim();


                //lấy ra mã otp
                // cập nhật dbs
                string otp = input.Otp;
                if (string.IsNullOrEmpty(otp))
                {
                    return new
                    {
                        ResponseCode = -203,
                        Message = ErrorMsg.OtpEmpty
                    };
                }
                otp = otp.Trim();
                if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpLengthInValid
                    };
                }
                int resOtp;
                long otpID;
                var newPhone = user.PhoneNumber.PhoneFormat();
                string otmsg;
                SMSDAO.Instance.ValidOtp(accountId, otp.Length == OTPSAFE_LENGTH ? user.PhoneSafeNo : newPhone, otp, ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
                int response = AccountDAO.Instance.ChangeFbInfor(accountId, username, Security.SHA256Encrypt(Password), Password, ServiceID);
                if (response == -1)//trả về 0 nếu mật khẩu cũ không đúng
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserNameInUse,
                    };
                }
                else if (response == -4)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UsernameContainDisplayname,
                    };
                }
                else if (response == -5)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountFBNotExist,
                    };
                }
                else if (response == -246)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountNotFB,
                    };
                }
                else if (response == 1)//cập nhật thành công
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateSuccess,
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException + "|" + response
                    };
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }


        //[HttpOptions, HttpPost]
        //[Route("ChangeFBInforPopup")]
        //public dynamic ChangeFBInforPopup([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        //kiểm tra lại cách lấy accountId
        //        var accountId = AccountSession.AccountID;
        //        var displayName = AccountSession.AccountName;
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
        //        if (user == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (user.Status != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountLock
        //            };
        //        }
        //        if (user.IsFB != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountNotFB
        //            };
        //        }
        //        if (user.IsUpdatedFB == 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountUpdateFBOneTime
        //            };
        //        }
        //        string username = input.UserName ?? string.Empty;
        //        username = username != null ? username.ToLower() : string.Empty;
        //        if (username.Length < 6 || username.Length > 18)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.UserNameLength
        //            };
        //        }
        //        if (username.Contains(" "))
        //        {
        //            return new
        //            {
        //                ResponseCode = -101,
        //                Message = ErrorMsg.UserNameContainSpace
        //            };
        //        }
        //        if (ValidateInput.IsNickNameContainNotAllowString(username))
        //        {
        //            return new
        //            {
        //                ResponseCode = -102,
        //                Message = ErrorMsg.UserNameContainsNotAllowString
        //            };
        //        }
        //        if (!ValidateInput.ValidateUserName(username))
        //        {
        //            return new
        //            {
        //                ResponseCode = -102,
        //                Message = ErrorMsg.UsernameIncorrect
        //            };
        //        }
        //        int outResponse = 0;
        //        AccountDAO.Instance.CheckAccountCheckExist(1, username, ServiceID, out outResponse);
        //        if (outResponse != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.UserNameAndPasswordExist
        //            };
        //        }
        //        string Password = input.Password ?? string.Empty;

        //        //old pass độ dài không thích hợp
        //        if (Password.Length < 6 || Password.Length > 16)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.PasswordLength
        //            };
        //        }
        //        //new pass độ dài không thích hợp

        //        if (!ValidateInput.IsValidatePass(Password))
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.PasswordIncorrect,
        //            };
        //        }
        //        Password = Password.Trim();
        //        //lấy mã private captca
        //        string privateKey = input.PrivateKey ?? string.Empty;
        //        //lấy mã capcha user nhập
        //        string captcha = input.Captcha ?? string.Empty;
        //        // kiểm tra mã capcha có thích hợp hay ko
        //        if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
        //        {
        //            return new
        //            {
        //                ResponseCode = -4,
        //                Message = ErrorMsg.CapchaRequired
        //            };
        //        }
        //        else
        //        {
        //            if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
        //            {
        //                return new
        //                {
        //                    ResponseCode = -100,
        //                    Message = ErrorMsg.InValidCaptCha
        //                };
        //            }
        //        }

        //        int response = AccountDAO.Instance.ChangeFbInfor(accountId, username, Security.SHA256Encrypt(Password), Password, ServiceID);
        //        if (response == -1)//trả về 0 nếu mật khẩu cũ không đúng
        //        {
        //            //cộng tiền cho user
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.UserNameInUse,
        //            };
        //        }
        //        else if (response == -4)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.UsernameContainDisplayname,
        //            };
        //        }
        //        else if (response == -5)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountFBNotExist,
        //            };
        //        }
        //        else if (response == -246)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountNotFB,
        //            };
        //        }
        //        else if (response == 1)//cập nhật thành công
        //        {
        //            if (user.IsFBReward == 1&&ServiceID==1)
        //            {
        //                long Amount = 0;
        //                int rsRes = 0;
        //                AccountDAO.Instance.SPUserFBUpdateInfoAward(accountId, out Amount, out rsRes);
        //                NLogManager.LogMessage(String.Format("SPUserFBUpdateInfoAward -AccountID {0}-Res {1}", accountId, rsRes));
        //            }
        //            return new
        //            {
        //                ResponseCode = 1,
        //                Message = ErrorMsg.UpdateSuccess,
        //            };
        //        }
        //        else
        //        {
        //            return new
        //            {
        //                ResponseCode = -99,
        //                Message = ErrorMsg.InProccessException + "|" + response
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //    }
        //    return new
        //    {
        //        ResponseCode = -99,
        //        Message = ErrorMsg.InProccessException
        //    };
        //}
        /// <summary>
        /// Get account infor
        /// </summary>
        /// <returns>obj json account infor else msg</returns>
        [HttpGet]
        [Route("GetAccountInfo")]
        public dynamic GetAccountInfo()
        {
            try
            {
                //kiểm tra lại
                var accountId = AccountSession.AccountID;
                //var displayName = AccountSession.AccountName;
                if (accountId <= 0)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                // lấy account inform by id
                var response = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (response == null)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    };
                }
                else
                {
                    int responseLog = 0;
                    int eventVp = 0;
                    var userRank = AccountDAO.Instance.GetUserRankNew(response.AccountID, out responseLog, out eventVp);
                    response.RankID = userRank == null ? VipIndex : userRank.RankID;
                    response.VP = userRank == null ? 0 : userRank.VP;
                    response.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
                    response.PhoneNumber = response.PhoneNumber.PhoneDisplayFormat();
                    response.PhoneSafeNo = response.PhoneSafeNo.PhoneDisplayFormat();
                    var nextVP = GetNextVIPInfor(response.RankID);
                    response.IsFBReward = ServiceID == 1 ? response.IsFBReward : 0;
                    //  response.IsUpdatedFB= ServiceID == 1 ? response.IsUpdatedFB : 1;
                    response.IsUpdatedFB = 1;
                    return new
                    {
                        ResponseCode = 1,
                        AccountInfo = response,
                        NextVIP = nextVP,
                        TopVP = eventVp
                    };
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        /// <summary>
        /// lout out for all device
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public dynamic Logout()
        {
            try
            {
                FormsAuthentication.SignOut();
                CookieManager.RemoveAllCookies(true);
                ////kiểm  tra account id
                //var accountId = AccountSession.AccountID;
                //if (accountId <= 0)
                //{
                //    return new
                //    {
                //        ResponseCode = -2,
                //        Message = ErrorMsg.UnAuthorized,
                //    };
                //}
                //// lấy account inform by id
                //FormsAuthentication.SignOut();
                //CookieManager.RemoveAllCookies(true);

                //try
                //{
                //    HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                //    cookie1.Expires = DateTime.Now.AddYears(-1);
                //    HttpContext.Current.Response.Cookies.Set(cookie1);
                //    //lấy cookie có sẵn
                //    var cookieName = String.Format(".{0}", ConfigurationManager.AppSettings["COOKIE_DOMAIN"].ToString());
                //    HttpCookie httpCookie = HttpContext.Current.Request.Cookies[".ibom.cc"];
                //    NLogManager.LogMessage("http cookies" + httpCookie);
                //    if (httpCookie != null)
                //    {
                //        //set expires cho cookie
                //        httpCookie.Domain = HttpContext.Current.Request.Url.Host.Contains("localhost") ? null : ".ibom.cc";
                //        httpCookie.Expires = DateTime.Now.AddYears(-1);
                //        //update cookie
                //        HttpContext.Current.Response.Cookies.Set(httpCookie);
                //    }
                //    HttpContext.Current.Request.Cookies.Remove(".ibom.cc");
                //}
                //catch (Exception ex)
                //{
                //    NLogManager.PublishException(ex);
                //    NLogManager.LogMessage("http cookies" + ex);
                //}
                return new
                {
                    ResponseCode = 1,
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        /// <summary>
        /// cập nhật thông tin người dùng
        /// </summary>
        /// <param name="accountName">tên cập nhật</param>
        /// <param name="dateofbirth">ngày sinh</param>
        /// <param name="phonenumber">số điện thoại (không cho cập nhật)</param>
        /// <param name="gender">giới tính</param>
        /// <param name="avatar">ảnh</param>
        /// <returns></returns>
        //[HttpOptions, HttpPost]
        //[Route("UpdateAvatar")]
        //public dynamic UpdateAvatar([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        string accountName = input.AccountName;


        //        int avatar = Convert.ToInt32(input.Avatar ?? -1);



        //        if (avatar != -1 && (avatar < 1 && avatar > 20))
        //        {
        //            return new
        //            {
        //                ResponseCode = -104,
        //                Message = ErrorMsg.AvatarIncorrect
        //            };
        //        }
        //        var accountId = AccountSession.AccountID;
        //        if (accountId <= 0)
        //        {
        //            return new
        //            {
        //                ResponseCode = -2,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        int? authen = null;


        //        int response = -99;
        //        var res = AccountDAO.Instance.UpdateProfile(accountId, accountName,null, null,0, avatar, authen, otp, out response);
        //        //cập nhật thành công
        //        if (res != null && response == 1)
        //        {
        //            int resChoSao = 0;
        //            ChoSaoDAO.Instance.UpdateProfile(accountId, accountName, avatar.ToString(), null, null, null, out resChoSao);
        //            int responseLog = 0;
        //            var userRank = AccountDAO.Instance.GetUserRank(res.AccountID, out responseLog);
        //            res.RankID = userRank == null ? 5 : userRank.RankID;
        //            res.VP = userRank == null ? 0 : userRank.VP;
        //            res.RankName = userRank == null ? "Đá" : userRank.RankName;
        //            SetAuthCookie(res);
        //            return new
        //            {
        //                ResponseCode = 1,
        //                Message = ErrorMsg.UpdateSuccess,
        //                AccountInfo = res
        //            };
        //        }
        //        else if (response == -1)
        //        {
        //            return new
        //            {
        //                ResponseCode = response,
        //                Message = ErrorMsg.NameInUse
        //            };
        //        }
        //        else if (response == -2)
        //        {
        //            return new
        //            {
        //                ResponseCode = response,
        //                Message = ErrorMsg.UpdateFail
        //            };
        //        }
        //        else if (response == -3)
        //        {
        //            return new
        //            {
        //                ResponseCode = response,
        //                Message = ErrorMsg.OtpIncorrect
        //            };
        //        }
        //        else if (response == -4)
        //        {
        //            return new
        //            {
        //                ResponseCode = response,
        //                Message = ErrorMsg.NickNameContainUsername
        //            };
        //        }
        //        else
        //        {
        //            return new
        //            {
        //                ResponseCode = -99,
        //                Message = ErrorMsg.InProccessException
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.InProccessException
        //        };
        //    }
        //}
        /// <summary>
        /// cập nhật update phone
        /// </summary>
        /// <param name="phoneNumber">số điện thoại</param>
        /// <param name="otp">otp</param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        public dynamic UpdatePhone([FromBody] dynamic input)
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                var tkServiceID = AccountSession.ServiceID;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                string phoneNumber = input.PhoneNumber ?? string.Empty;
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneEmpty
                    };
                }
                phoneNumber = phoneNumber.Trim();
                if (!ValidateInput.ValidatePhoneNumberWithRegion(phoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneIncorrect
                    };
                }
                //if (IsVieNamMobilePhone(phoneNumber))
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.PhoneVietnammobileNotSupport
                //    };
                //}
                string otp = input.Otp;
                if (string.IsNullOrEmpty(otp))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpEmpty
                    };
                }
                otp = otp.Trim();
                //if (otp.Length != OTPSMS_LENGTH)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.SmsOtpOnlyAccept
                //    };
                //}

                //cập nhật phone thì phone phải trống
                if (!String.IsNullOrEmpty(user.PhoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneRegister
                    };
                }

                var newPhone = phoneNumber;
                int resultPhone = AccountDAO.Instance.CheckAccountPhone(accountId, newPhone, ServiceID);
                if (resultPhone != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneInUse
                    };
                }
                int resOtp = 0;
                long otpID = 0;
                string otmsg = string.Empty;
                //--Otp temp--
                //SMSDAO.Instance.ValidOtp(accountId, newPhone, otp, ServiceID, out resOtp, out otpID, out otmsg);
                //if ("123456".Equals(otp))
                //{
                //    resOtp = 1;
                //}
                //else
                //{
                if (otp.Length == 7)
                {
                    SMSDAO.Instance.ValidOtpFistTele(accountId, newPhone, otp, ServiceID, out resOtp, out otpID, out otmsg);
                    //SMSDAO.Instance.ValidOtp(accountId, newPhone, otp, ServiceID, out resOtp, out otpID, out otmsg);
                }
                else
                {
                    SMSDAO.Instance.ValidOtp(accountId, newPhone, otp, ServiceID, out resOtp, out otpID, out otmsg);
                }
                //}
                NLogManager.LogMessage("resOtp: " + resOtp);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
                //-- ENd Otp temp--
                //int resChoSao = ChoSaoDAO.Instance.UpdatePhone(accountId, newPhone);
                //if (resChoSao == 1)
                //{
                int rs = AccountDAO.Instance.UpdatePhone(accountId, newPhone, ServiceID, otp.Length == 7 ? (otpID > 0 ? otpID : 0) : 0);
                if (rs > 0)
                {
                    return new
                    {
                        ResponseCode = rs,
                        Phone = newPhone,
                        Message = ErrorMsg.UpdateSuccess
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = rs,
                        Message = ErrorMsg.UpdateFail
                    };
                }
                //}
                //else if (resChoSao == -104)
                //{
                //    return new
                //    {
                //        ResponseCode = resChoSao,
                //        Message = ErrorMsg.PhoneRegister
                //    };
                //}
                //else
                //{
                //    return new
                //    {
                //        ResponseCode = resChoSao,
                //        Message = ErrorMsg.UpdateFail
                //    };
                //}

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        [HttpOptions, HttpPost]
        [Route("DeletePhone")]
        public dynamic DeletePhone([FromBody] dynamic input)
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                //xóa phone thì phone không được trống
                if (String.IsNullOrEmpty(user.PhoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneEmpty
                    };
                }
                string otp = input.Otp;
                if (string.IsNullOrEmpty(otp))
                {
                    return new
                    {
                        ResponseCode = -203,
                        Message = ErrorMsg.OtpEmpty
                    };
                }
                otp = otp.Trim();
                //if (otp.Length != OTPSMS_LENGTH)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.SmsOtpOnlyAccept
                //    };
                //}
                if (user.AuthenType == 1 && !String.IsNullOrEmpty(user.PhoneNumber) && String.IsNullOrEmpty(user.PhoneSafeNo))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AuthenTypeNeedRemove
                    };
                }
                int resOtp;
                long otpID;
                string otmsg;
                SMSDAO.Instance.ValidOtp(accountId, otp.Length == OTPSAFE_LENGTH ? user.PhoneSafeNo : user.PhoneNumber, otp.Trim(), ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
                //int resChoSao = ChoSaoDAO.Instance.DeletePhone(accountId);
                //if (resChoSao == 1)
                //{
                int rs = AccountDAO.Instance.DeletePhone(accountId, ServiceID);
                if (rs > 0)
                {
                    return new
                    {
                        ResponseCode = rs,
                        Message = ErrorMsg.UpdateSuccess
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = rs,
                        Message = ErrorMsg.UpdateFail
                    };
                }
                //}
                //else
                //{
                //    return new
                //    {
                //        ResponseCode = -99,
                //        Message = ErrorMsg.UpdateFail
                //    };
                //}

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        /// <summary>
        /// add tiền vào két sắt
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("AddToSafe")]
        public dynamic AddToSafe([FromBody] dynamic input)
        {
            try
            {
                string APPROVE = ConfigurationManager.AppSettings["AddToSafe_APPROVED"].ToString();
                if (APPROVE != "1")
                {
                    return AnphaHelper.Close();
                }
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var acountInfor = AccountDAO.Instance.GetAccountInfo(accountId, null, null, ServiceID);
                if (acountInfor == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                long amount = ConvertUtil.ToLong(input.Amount);
                string strMinTranferAmount = "1000";
                ParaConfigDAO.Instance.GetCoreConfig("SAFE_TRANSFER", "MIN_AMOUNT", out strMinTranferAmount);
                long minTranferAmount = ConvertUtil.ToLong(strMinTranferAmount);
                if (amount <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.MinAmountTranfer, minTranferAmount.LongToMoneyFormat())
                    };
                }

                if (amount < minTranferAmount)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.MinAmountTranfer, minTranferAmount.LongToMoneyFormat())
                    };
                }
                if (acountInfor.Balance < amount)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotEnoughToSafe
                    };
                }
                string clientIp = IPAddressHelper.GetClientIP();
                //lấy device type (web,phone,ios,win phone 
                int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;
                var accountSafeInfo = AccountDAO.Instance.AddToSafe(accountId, deviceType, clientIp, amount, ServiceID);
                if (accountSafeInfo == null)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.InProccessException
                    };
                }
                if (accountSafeInfo.responseStatus == 1)
                {
                    if (!String.IsNullOrEmpty(acountInfor.PhoneSafeNo))
                    {
                        int outResponse;
                        long msgID;
                        string msg = String.Format("You just made a transaction freeze {0} at time {1}", amount.LongToMoneyFormat(), DateTime.Now);
                        if (acountInfor != null)
                        {
                            if (!String.IsNullOrEmpty(acountInfor.PhoneSafeNo))
                            {
                                SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, acountInfor.PhoneSafeNo, msg, out outResponse, out msgID);
                            }
                            if (!String.IsNullOrEmpty(acountInfor.SignalID) && !String.IsNullOrEmpty(acountInfor.PhoneSafeNo))
                            {
                                OneSignalApi.SendByPlayerID(new List<string>() { acountInfor.SignalID }, msg);
                            }
                        }
                    }
                    return new
                    {
                        ResponseCode = accountSafeInfo.responseStatus,
                        AccountSafeInfo = accountSafeInfo
                    };
                }
                else if (accountSafeInfo.responseStatus == -2)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotEnoughToSafe
                    };
                }
                else if (accountSafeInfo.responseStatus == -504)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotEnoughToSafe
                    };
                }
                else if (accountSafeInfo.responseStatus == -515)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.SaveLogError
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.InProccessException
                    };
                }


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        /// <summary>
        /// rút tiền ra khỏi két sắt
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("WithDrawFromSafe")]
        public dynamic WithDrawFromSafe([FromBody] dynamic input)
        {
            try
            {
                string APPROVE = ConfigurationManager.AppSettings["WithDraw_APPROVED"].ToString();
                if (APPROVE != "1")
                {
                    return AnphaHelper.Close();
                }
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                long amount = ConvertUtil.ToLong(input.Amount);
                string strMinTranferAmount = "1000";
                ParaConfigDAO.Instance.GetCoreConfig("SAFE_TRANSFER", "MIN_AMOUNT", out strMinTranferAmount);
                long minTranferAmount = ConvertUtil.ToLong(strMinTranferAmount);
                if (amount <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.MinAmountTranfer, minTranferAmount.LongToMoneyFormat())
                    };
                }

                if (amount < minTranferAmount)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.MinAmountTranfer, minTranferAmount.LongToMoneyFormat())
                    };
                }
                string clientIp = IPAddressHelper.GetClientIP();
                string otp = input.OTP ?? String.Empty;
                if (!ValidateInput.IsValidOtp(otp))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpIncorrect
                    };
                }
                otp = otp.Trim();
                int resOtp;
                long otpID;
                string otmsg;
                SMSDAO.Instance.ValidOtp(accountId, otp.Length == OTPSAFE_LENGTH ? user.PhoneSafeNo : user.PhoneNumber, otp, ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
                //lấy device type (web,phone,ios,win phone 
                int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;
                var accountSafeInfo = AccountDAO.Instance.WithDrawFromSafe(accountId, deviceType, clientIp, amount, null, ServiceID);
                if (accountSafeInfo == null)
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    };
                }
                if (accountSafeInfo.responseStatus == 1)
                {
                    // String Description = "Rút   " + amount.LongToMoneyFormat() + " Bom  ra khỏi két";
                    // int outResponse;
                    //  HistoryDAO.Instance.GameInsert(0, amount, 0, 0, 2, accountId, accountSafeInfo.SessionID, 0, accountSafeInfo.Balance, 0, accountSafeInfo.SafeBalance, IPAddressHelper.GetClientIP(), Description, out outResponse);
                    if (!String.IsNullOrEmpty(user.PhoneSafeNo))
                    {
                        int outResponse;
                        long msgID;
                        string msg = String.Format("You have just made a withdrawal transaction {0} at time {1}", amount.LongToMoneyFormat(), DateTime.Now);
                        if (user != null)
                        {
                            if (!String.IsNullOrEmpty(user.PhoneSafeNo))
                            {
                                SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, user.PhoneSafeNo, msg, out outResponse, out msgID);
                            }
                            if (!String.IsNullOrEmpty(user.SignalID) && !String.IsNullOrEmpty(user.PhoneSafeNo))
                            {
                                OneSignalApi.SendByPlayerID(new List<string>() { user.SignalID }, msg);
                            }
                        }
                    }
                    return new
                    {
                        ResponseCode = accountSafeInfo.responseStatus,
                        AccountSafeInfo = accountSafeInfo
                    };
                }
                else if (accountSafeInfo.responseStatus == -1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                else if (accountSafeInfo.responseStatus == -504)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AmountNotEnoughToOpenSafe
                    };
                }
                else if (accountSafeInfo.responseStatus == -515)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.SaveLogError
                    };
                }
                else if (accountSafeInfo.responseStatus == -3 || accountSafeInfo.responseStatus == -202)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneNotRegister
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    };
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        [HttpOptions, HttpPost]
        [Route("GiftCode")]
        public dynamic GiftCode([FromBody] dynamic input)
        {
            try
            {
                //kiểm tra lại cách lấy accountId
                var accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                string deviceId = input.DeviceId;//lấy ra deviceId
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (account == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (account.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                if (string.IsNullOrEmpty(account.PhoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = ErrorMsg.SecurityInValid
                    };
                }
                string GiftCode = input.GiftCode;

                //new pass độ dài không thích hợp
                if (String.IsNullOrEmpty(GiftCode))
                {
                    return new
                    {
                        ResponseCode = -100,
                        Message = ErrorMsg.GiftCodeRequired
                    };
                }
                GiftCode = GiftCode.Trim();
                //lấy mã private captca
                string privateKey = input.PrivateKey ?? string.Empty;
                //lấy mã capcha user nhập
                string captcha = input.Captcha ?? string.Empty;
                // kiểm tra mã capcha có thích hợp hay ko
                if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
                {
                    return new
                    {
                        ResponseCode = -4,
                        Message = ErrorMsg.CapchaRequired
                    };
                }
                else
                {
                    if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
                    {
                        return new
                        {
                            ResponseCode = -100,
                            Message = ErrorMsg.InValidCaptCha
                        };
                    }
                }
                //string keyHu = CachingHandler.Instance.GeneralRedisKey("GiftCode", "" + accountId);
                //RedisCacheProvider _cachePvd = new RedisCacheProvider();
                //string gift = _cachePvd.Get<string>(keyHu);
                //bool isddos = false;
                //if (!string.IsNullOrEmpty(gift))
                //{
                //    if (gift.Contains(GiftCode.ToLower()))
                //    {
                //        isddos = true;
                //        NLogManager.LogMessage(string.Format("AccountID:{0}-GiftCode{1}", accountId, GiftCode.ToUpper()));
                //    }
                //    else
                //    {
                //        gift += ";" + GiftCode.ToLower();
                //    }
                //}
                //else
                //{
                //    gift = GiftCode.ToLower();
                //    _cachePvd.Set<string>(keyHu, gift, 1);
                //}
                //if (isddos)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Balance = 0,
                //        Message = ErrorMsg.GiftCodeInUse,
                //    };
                //}
                long balance;
                long giftCodeID;
                long GiftCodeAmount;
                int response = GiftCodeDAO.Instance.ReceiveGiftCode(accountId, GiftCode, deviceId, ServiceID, out balance, out giftCodeID, out GiftCodeAmount);
                NLogManager.LogMessage(String.Format("GiftCode -{0}-AccountID{1}-Content:{2}", response, accountId, GiftCode.ToUpper()));
                if (response == 1)
                {
                    // int outResponse = 0;
                    String Description = String.Format("Account {2} Enter gift code {0} with G number {1}", GiftCode, GiftCodeAmount, displayName);
                    //   HistoryDAO.Instance.GameInsert(0, GiftCodeAmount, 0, 0, 2, accountId, giftCodeID, 0, balance, 0, 0, IPAddressHelper.GetClientIP(), Description, out outResponse);
                    SendTelePush(Description, 6);
                    return new
                    {
                        ResponseCode = 1,
                        Balance = balance,
                        GiftCodeAmount = GiftCodeAmount,
                        Message = ErrorMsg.GiftCodeSuccess,
                    };
                }
                else if (response == -1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Balance = 0,
                        Message = ErrorMsg.GiftCodeInCorrect,
                    };
                }
                else if (response == -2)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Balance = 0,
                        Message = ErrorMsg.GiftCodeInCorrect,
                    };
                }
                else if (response == -3)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Balance = 0,
                        Message = ErrorMsg.GiftCodeExpired,
                    };
                }
                else if (response == -4)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Balance = 0,
                        Message = ErrorMsg.GiftCodeInUse,
                    };
                }
                else if (response == -6)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Balance = 0,
                        Message = ErrorMsg.GiftCodeExpired,
                    };
                }
                else if (response == -7)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Balance = 0,
                        Message = ErrorMsg.CampaignOutMoney,
                    };
                }
                else if (response == -29)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Balance = 0,
                        Message = ErrorMsg.GiftCodeUseOnlyOne,
                    };
                }
                else if (response == -520)
                {
                    return new
                    {
                        ResponseCode = -520,
                        Balance = 0,
                        Message = ErrorMsg.GiftCodeAmountValid,
                    };
                }
                else if (response == -208)
                {
                    return new
                    {
                        ResponseCode = -208,
                        Balance = 0,
                        Message = ErrorMsg.UserNotValid,
                    };
                }
                else if (response == -42)
                {
                    return new
                    {
                        ResponseCode = -42,
                        Balance = 0,
                        Message = ErrorMsg.ERR_EXEED_WRONG_GIFTCODE_QUOTA,
                    };
                }
                else if (response == -43)
                {
                    return new
                    {
                        ResponseCode = -43,
                        Balance = 0,
                        Message = ErrorMsg.ERR_LOCK_EXEED_WRONG_GIFTCODE_QUOTA,
                    };
                }
                else if (response == -44)
                {
                    return new
                    {
                        ResponseCode = -44,
                        Balance = 0,
                        Message = ErrorMsg.ERR_WRONG_GIFTCODE_10,
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Balance = 0,
                        Message = ErrorMsg.InProccessException,
                    };
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }

        //[HttpOptions, HttpGet]
        //[Route("GetAvatar")]
        //public dynamic GetAvatar([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        string dirPath = Path.Combine(ConfigurationManager.AppSettings["AVATARDIR"].ToString());
        //        List<string> files = new List<string>();
        //        DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
        //        foreach (FileInfo fInfo in dirInfo.GetFiles())
        //        {
        //            files.Add(string.Format("{0}/Contents/Avatars/{1}", Request.RequestUri.GetLeftPart(UriPartial.Authority), fInfo.Name));
        //        }
        //        return new
        //        {
        //            ResponseCode = 1,
        //            Files = files,

        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //    }
        //    return new
        //    {
        //        ResponseCode = -99,
        //        Message = ErrorMsg.InProccessException
        //    };
        //}
        /// <summary>
        /// get current account balance
        /// </summary>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("GetBalance")]
        public dynamic GetBalance()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                long balance = 0;
                long safebalance = 0;
                AccountDAO.Instance.GetBalance(accountId, out balance, out safebalance);
                return new
                {
                    ResponseCode = 1,
                    balance = balance,
                    safebalance = safebalance,
                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        /// <summary>
        /// cập nhật mật khẩu theo số điện thoại ,username ,otp ,password
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ForgotPassword")]
        public dynamic ForgotPassword(dynamic input)
        {
            //para
            string username = input.UserName;
            username = username != null ? username.ToLower() : string.Empty;

            string phoneNumber = input.PhoneNumber ?? String.Empty;

            if (!ValidateInput.ValidatePhoneNumber(phoneNumber))
            {
                return new
                {
                    ResponseCode = -203,
                    Message = ErrorMsg.PhoneIncorrect
                };
            }
            string password = input.Password ?? string.Empty;
            if (String.IsNullOrEmpty(password))
            {
                return new
                {
                    ResponseCode = -102,
                    Message = ErrorMsg.PasswordEmpty
                };
            }
            if (password.Length < 6 || password.Length > 16)
            {
                return new
                {
                    ResponseCode = -102,
                    Message = ErrorMsg.PasswordLength
                };
            }
            if (!ValidateInput.IsValidatePass(password))
            {
                return new
                {
                    ResponseCode = -103,
                    Message = ErrorMsg.PasswordIncorrect
                };
            }
            string otp = input.Otp ?? string.Empty;
            if (!ValidateInput.IsValidOtp(otp))
            {
                return new
                {
                    ResponseCode = -104,
                    Message = ErrorMsg.OtpIncorrect
                };
            }
            otp = otp.Trim();
            if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.OtpLengthInValid
                };
            }
            //lấy mã private captca
            string privateKey = input.PrivateKey;
            //lấy mã capcha user nhập
            string captcha = input.Captcha;
            // kiểm tra mã capcha có thích hợp hay ko
            if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
            {
                return new
                {
                    ResponseCode = -100,
                    Message = ErrorMsg.InValidCaptCha
                };
            }

            string newPhone = phoneNumber.Substring(1);
            var loginAccount = AccountDAO.Instance.GetAccountInfo(0, username, null, ServiceID);
            if (loginAccount == null)
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserNotExist
                };
            }
            if (String.IsNullOrEmpty(loginAccount.PhoneNumber) && String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AcountNotConfigOtp
                };
            }
            //if (loginAccount.PhoneNumber != newPhone && loginAccount.PhoneSafeNo != newPhone)
            //{
            //    return new
            //    {
            //        ResponseCode = -1005,
            //        Message = ErrorMsg.PhoneRegisterDiffPhonePass
            //    };
            //}
            if ("85" + newPhone != loginAccount.PhoneNumber && "1" + newPhone == loginAccount.PhoneNumber && "0" == loginAccount.PhoneNumber)//check phone number with area/country code
            {
                return new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.PhoneRegisterDiffPhonePass
                };
            }
            if (loginAccount != null)
            {
                //kiểm tra lại mã otp khi forgot pasword
                int resOtp = 0;
                long otpID;
                string otmsg;
                SMSDAO.Instance.ValidOtp(loginAccount.AccountID, otp.Length == OTPSAFE_LENGTH ? loginAccount.PhoneSafeNo : loginAccount.PhoneNumber, otp, ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
                //kết thúc kiểm tra opt forgot password
                int response;
                AccountDAO.Instance.ForgotPassword(loginAccount.AccountID, Security.SHA256Encrypt(password), newPhone, otp, password, out response);
                if (response == 1)
                {
                    if (!String.IsNullOrEmpty(loginAccount.PhoneSafeNo))
                    {
                        int outResponse;
                        long msgID;
                        string msg = String.Format("You have just changed the password for account {0} at time {1}", loginAccount.AccountName, DateTime.Now);
                        SafeOtpDAO.Instance.OTPSafeMessageSend(ServiceID, loginAccount.PhoneSafeNo, msg, out outResponse, out msgID);
                    }
                    //int resChoSao = 0;
                    //ChoSaoDAO.Instance.UpdateProfile(loginAccount.AccountID, null, null, null, null, Security.SHA256Encrypt(password), out resChoSao);
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.PasswordChangeSuccess
                    };
                }
            }


            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.UpdateFail
            };

        }
        /// <summary>
        /// validate phonenumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private dynamic ValidatePhoneNumber(string phoneNumber, int status)
        {
            status = 0;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return new
                {
                    ResponseCode = -203,
                    Message = ErrorMsg.PhoneEmpty
                };
            }
            if (!phoneNumber.StartsWith("0"))
            {
                return new
                {
                    ResponseCode = -203,
                    Message = ErrorMsg.PhoneIncorrect
                };
            }
            if (!ValidateInput.ValidatePhoneNumber(phoneNumber))
            {
                return new
                {
                    ResponseCode = -203,
                    Message = ErrorMsg.PhoneIncorrect
                };
            }
            status = 1;
            return null;
        }
        private dynamic ValidateCapCha(string captcha, string privateKey, out int status)
        {
            status = 0;
            if (String.IsNullOrEmpty(captcha) || String.IsNullOrEmpty(privateKey))
            {
                return new
                {
                    ResponseCode = -100,
                    Message = ErrorMsg.InValidCaptCha
                };
            }
            // kiểm tra mã capcha có thích hợp hay ko
            if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
            {
                return new
                {
                    ResponseCode = -100,
                    Message = ErrorMsg.InValidCaptCha
                };
            }
            status = 1;
            return null;
        }
        /// <summary>
        /// validate passowrd
        /// </summary>
        /// <param name="password"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        //private dynamic ValidatePassword(string password, out int status)
        //{
        //    status = 0;
        //    if (String.IsNullOrEmpty(password))base: ec2-3-0-92-215.ap-southeast-1.compute.amazonaws.com
        //    {
        //        return new
        //        {
        //            ResponseCode = -402,
        //            Message = ErrorMsg.PasswordEmpty
        //        };
        //    }
        //    if (password.Length < 6 || password.Length > 18)
        //    {
        //        return new
        //        {
        //            ResponseCode = -403,
        //            Message = ErrorMsg.PasswordLength
        //        };
        //    }
        //    if (!ValidateInput.IsValidatePass(password))
        //    {
        //        return new
        //        {
        //            ResponseCode = -404,
        //            Message = ErrorMsg.PasswordIncorrect
        //        };
        //    }
        //    status = 1;
        //    return null;
        //}
        /// <summary>
        /// validate user name
        /// </summary>
        /// <param name="username"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private dynamic ValidateUserName(string username, out int status)
        {
            status = 0;
            if (string.IsNullOrEmpty(username))
            {
                return new
                {
                    ResponseCode = -301,
                    Message = ErrorMsg.UsernameEmpty
                };
            }
            if (username.Length < 6 || username.Length > 18)
            {
                return new
                {
                    ResponseCode = -302,
                    Message = ErrorMsg.UserNameLength
                };
            }
            if (!ValidateInput.IsValidNickName(username))
            {
                return new
                {
                    ResponseCode = -303,
                    Message = ErrorMsg.UsernameIncorrect
                };
            }
            status = 1;
            return null;
        }
        /// <summary>
        /// cập nhật authen type
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("UpdateAuthenType")]
        public dynamic UpdateAuthenType([FromBody] dynamic input)
        {
            string APPROVE = ConfigurationManager.AppSettings["UserTranfer_APPROVED"].ToString();
            if (APPROVE != "1")
            {
                return AnphaHelper.Close();
            }
            try
            {

                var accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                int? authen = null;
                string authenType = input.AuthenType;
                if (String.IsNullOrEmpty(authenType))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                if (authenType != "0" && authenType != "1")
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                if (!String.IsNullOrEmpty(authenType)) authen = Convert.ToInt32(authenType);
                string otp = input.Otp;
                if (authen.HasValue && authen.Value == 1 && String.IsNullOrEmpty(user.PhoneNumber) && String.IsNullOrEmpty(user.PhoneSafeNo))
                {
                    return new
                    {

                        ResponseCode = -1005,
                        Message = ErrorMsg.PhoneNotRegister
                    };
                }
                if (authen.HasValue && authen.Value == 0 && String.IsNullOrEmpty(otp))
                {
                    return new
                    {
                        ResponseCode = -2,
                        Message = ErrorMsg.OtpEmpty
                    };
                }
                if (authen.HasValue && authen.Value == 0 && !String.IsNullOrEmpty(otp))
                {
                    if (String.IsNullOrEmpty(user.PhoneNumber) && String.IsNullOrEmpty(user.PhoneSafeNo))
                    {
                        return new
                        {

                            ResponseCode = -1005,
                            Message = ErrorMsg.PhoneNotRegister
                        };
                    }
                    otp = otp.Trim();
                    if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
                    {
                        return new
                        {
                            ResponseCode = -1005,
                            Message = ErrorMsg.OtpLengthInValid
                        };
                    }
                    int resOtp = 0;
                    long otpID = 0;
                    string otmsg;
                    SMSDAO.Instance.ValidOtp(accountId, otp.Length == OTPSAFE_LENGTH ? user.PhoneSafeNo : user.PhoneNumber, otp, ServiceID, out resOtp, out otpID, out otmsg);//otp valid 
                    if (resOtp != 1)
                    {
                        return new
                        {
                            ResponseCode = -5,
                            Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                        };
                    }
                }

                int response = -99;

                var res = AccountDAO.Instance.UpdateAuthen(accountId, authen.Value, ServiceID, out response);
                //cập nhật thành công
                if (response == 1)
                {

                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateSecuritySucess,
                    };
                }
                else if (response == -2)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.PhoneEmpty
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.UpdateFail
                    };
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        /// <summary>
        /// cập nhật avatar
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpOptions, HttpPost]
        [Route("UpdateAvatar")]
        public dynamic UpdateAvatar([FromBody] dynamic input)
        {
            try
            {
                var accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                string stravatar = input.Avatar ?? string.Empty;
                if (String.IsNullOrEmpty(stravatar))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }
                int avatar = Convert.ToInt32(input.Avatar ?? -1);
                if (avatar != -1 && (avatar < 1 && avatar > 20))
                {
                    return new
                    {
                        ResponseCode = -104,
                        Message = ErrorMsg.AvatarIncorrect
                    };
                }
                int response = -99;
                var res = AccountDAO.Instance.UpdateAvatar(accountId, avatar, ServiceID, out response);
                //cập nhật thành công
                if (response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.AvatarUpdateSuccess,
                    };
                }
                else if (response == -1)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.NameInUse
                    };
                }
                else if (response == -2)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.UpdateFail
                    };
                }
                else if (response == -3)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.OtpIncorrect
                    };
                }
                else if (response == -4)
                {
                    return new
                    {
                        ResponseCode = response,
                        Message = ErrorMsg.NickNameContainUsername
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    };
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        [HttpOptions, HttpPost]
        [Route("UpdateNickName")]
        public dynamic UpdateNickName([FromBody] dynamic input)
        {
            try
            {
                var accountId = AccountSession.AccountID;
                if (accountId <= 0)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                string accountName = input.AccountName ?? string.Empty;
                //nikc name trong
                if (String.IsNullOrEmpty(accountName))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.NickNameEmpty
                    };
                }
                accountName = accountName.Trim();
                //độ dài không phù hợp
                if (accountName.Length < 6 || accountName.Length > 16)
                {
                    return new
                    {
                        ResponseCode = -102,
                        Message = ErrorMsg.NickNameLength
                    };
                }
                //chứa khoảng trắng
                if (accountName.Contains(" "))
                {
                    return new
                    {
                        ResponseCode = -101,
                        Message = ErrorMsg.NickNameNotContainSpace
                    };
                }
                //chứa kí tự ko cho phép
                if (ValidateInput.IsNickNameContainNotAllowString(accountName))
                {
                    return new
                    {
                        ResponseCode = -102,
                        Message = ErrorMsg.NickNameContainNotAlowString
                    };
                }

                if (!ValidateInput.ValidateUserName(accountName))
                {
                    return new
                    {
                        ResponseCode = -103,
                        Message = ErrorMsg.DisplayNameIncorrect
                    };
                }
                //kiểm tra nick nam đã được xử dụng
                //int outResponse = 0;
                //AccountDAO.Instance.CheckAccountCheckExist(2, accountName, out outResponse);
                //if (outResponse != 1)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.NickNameInUse
                //    };
                //}
                //UserDAO.Instance.CheckUserExist(accountName, 2, out outResponse);
                //if (outResponse != 1)
                //{
                //    return new
                //    {
                //        ResponseCode = -1005,
                //        Message = ErrorMsg.NickNameInUse
                //    };
                //}
                //cập nhật trên 109
                int response = 0;
                var res = AccountDAO.Instance.AccountUpdateNickName(accountId, accountName, ServiceID, out response);
                //cập nhật thành công
                if (res != null && response == 1)
                {
                    string clientIp = IPAddressHelper.GetClientIP();
                    int responseLog = 0;
                    var userRank = AccountDAO.Instance.GetUserRank(res.AccountID, out responseLog);
                    res.RankID = userRank == null ? VipIndex : userRank.RankID;
                    res.VP = userRank == null ? 0 : userRank.VP;
                    res.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
                    var nextVP = GetNextVIPInfor(res.RankID);
                    // SetAuthCookie(res);
                    //  string APPROVE = ConfigurationManager.AppSettings["IS_ANPHA"].ToString();
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateNickNameSuccess,
                        NextVIP = nextVP,
                        Token = TokenHashprovider.GenerateToken(res.AccountID, res.AccountName, ServiceID, clientIp, TokenExpired, res.AvatarID, AccountSession.DeviceType, AccountSession.DeviceId),
                        AccountInfo = res
                    };
                }
                else
                {
                    return MessageConvetor.AccountUpdateNickName.GetMsg(response);
                }

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        /// <summary>
        /// set cookies
        /// </summary>
        /// <param name="account"></param>
        private void SetAuthCookie(Account account)
        {
            //string cookieUsername = AccountSession.SessionName(account.AccountID, account.AccountName, 0, 0);
            //string cookieDomain = FormsAuthentication.CookieDomain;
            //NLogManager.LogMessage("host" + HttpContext.Current.Request.Url.Host);
            //if (string.IsNullOrEmpty(cookieDomain))
            //{
            //    cookieDomain = ("." + HttpContext.Current.Request.Url.Host);
            //    string cookiesdomain = ConfigurationManager.AppSettings["COOKIE_DOMAIN"].ToString();
            //    cookieDomain = cookiesdomain;
            //}
            //HttpCookie cookie = FormsAuthentication.GetAuthCookie(cookieUsername, false, FormsAuthentication.FormsCookiePath);
            //if (HttpContext.Current.Request.Url.Host.Contains("localhost"))
            //{
            //    FormsAuthentication.SetAuthCookie(cookieUsername, false);
            //}
            //cookie.Domain = cookieDomain;
            //NLogManager.LogMessage("cookieDomain: " + cookieDomain);
            //HttpContext.Current.Response.Cookies.Add(cookie);
            string cookieUsername = AccountSession.SessionName(account.AccountID, account.AccountName, 0, 0);
            string cookieDomain = FormsAuthentication.CookieDomain;
            NLogManager.LogMessage(HttpContext.Current.Request.Url.Host);
            if (string.IsNullOrEmpty(cookieDomain))
            {
                cookieDomain = ("." + CookieManager.GetDomain(HttpContext.Current.Request.Url.Host));
                //cookieDomain =   ConfigurationManager.AppSettings["COOKIE_DOMAIN"].ToString();
            }


            if (HttpContext.Current.Request.Url.Host.Contains("localhost"))
                FormsAuthentication.SetAuthCookie(cookieUsername, false);
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(cookieUsername, false, FormsAuthentication.FormsCookiePath);
            cookie.Domain = cookieDomain;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        private void AddRefer(string referUrl, int loginType, long AccountID, string clientIP, int platforType)
        {
            try
            {
                string UtmMedium = string.Empty;
                string Utmsource = string.Empty;
                string Utmcampaign = string.Empty;
                string gclid = string.Empty;
                string Utmcontent = string.Empty;
                string path = string.Empty;
                if (!String.IsNullOrEmpty(referUrl))
                {
                    int res;
                    var model = UrlParseHelper.UrlParse(referUrl);
                    if (model != null)
                    {
                        var querys = model.queryStrings;
                        if (!String.IsNullOrEmpty(model.path) && !model.path.EndsWith("/")) model.path = String.Format("{0}/", model.path);
                        UtmMedium = querys.ContainsKey("utm_medium") ? querys["utm_medium"].ToString() : null;
                        Utmsource = querys.ContainsKey("utm_source") ? querys["utm_source"].ToString() : null;
                        Utmcampaign = querys.ContainsKey("utm_campaign") ? querys["utm_campaign"].ToString() : null;
                        Utmcontent = querys.ContainsKey("utm_content") ? querys["utm_content"].ToString() : null;
                        gclid = querys.ContainsKey("gclid") ? querys["gclid"].ToString() : null;
                        path = model.path;
                        UserDAO.Instance.UserTracking(loginType, ServiceID, AccountID, referUrl, clientIP, UtmMedium, Utmsource, Utmcampaign, Utmcontent, gclid, model.path, out res);
                        //Gửi Thông Tin lên DNA
                    }
                }
                var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                dnaHelper.SenDNACreateAccount(AccountID, loginType, platforType, Utmsource, Utmcampaign, UtmMedium, gclid, path, Utmcontent, clientIP);

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
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
        /// <summary>
        /// lấy thông tin next VP
        /// </summary>
        /// <param name="VIPID"></param>
        /// <returns></returns>
        private NextVP GetNextVIPInfor(long VIPID)
        {
            var nextVIP = new NextVP();
            try
            {
                if (VIPID >= 10)
                {
                    nextVIP.RankID = 10;
                    nextVIP.VP = 300000;
                    nextVIP.RankName = "VIP 10";
                    return nextVIP;
                }
                var listVip = UserPrivilegeDAO.Instance.PrivilegeTypeList(null);
                if (listVip == null) return nextVIP;

                if (VIPID < 10 && listVip != null)
                {
                    var firsObj = listVip.FirstOrDefault(c => c.ID == VIPID + 1);
                    if (firsObj != null)
                    {
                        nextVIP.RankID = firsObj.ID;
                        nextVIP.RankName = firsObj.PrivilegeName;
                        nextVIP.VP = firsObj.VP;
                    }
                }

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return nextVIP;
        }

        //[ActionName("LoginStore")]
        //[Route("LoginStore")]
        //[HttpOptions, HttpPost]
        //public dynamic LoginStore([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
        //        if (isOption)
        //        {
        //            return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
        //        }
        //        int loginType = input.LoginType ?? -1;
        //        int response = -99;
        //        int deviceType = input.DeviceType != null ? Convert.ToInt32(input.DeviceType.Value) : -1;
        //        Account loginAccount = new Account();
        //        string clientIp = IPAddressHelper.GetClientIP();
        //        string deviceId = input.DeviceId ?? string.Empty;//lấy ra deviceId
        //        string otp = input.Otp ?? String.Empty;
        //        switch (loginType)
        //        {

        //            case (int)Constants.enmAuthenType.AUTHEN_ID:
        //                string msg;
        //                int resCode;
        //                string username = input.UserName ?? string.Empty;
        //                username = username != null ? username.ToLower() : string.Empty;//lấy ra userme
        //                if (!String.IsNullOrEmpty(username)) username = username.Trim();
        //                string password = input.Password ?? string.Empty;//lấy ra password
        //                if (!String.IsNullOrEmpty(password)) password = password.Trim();

        //                string privateKey = input.PrivateKey ?? string.Empty;//lấy ra privte key
        //                string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
        //                if (!ValidateInput.IsValidated(username, password, out msg, out resCode))
        //                {
        //                    return new
        //                    {
        //                        ResponseCode = resCode,
        //                        Message = msg
        //                    };
        //                }
        //                int numberLogin = CacheLogin.CheckStatusFrequency(username, ServiceID, "login");
        //                if (numberLogin >= 3)
        //                {
        //                    if ((String.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(captcha)))
        //                    {
        //                        return new
        //                        {
        //                            ResponseCode = -4,
        //                            Message = ErrorMsg.CapchaRequired
        //                        };
        //                    }
        //                    else
        //                    {
        //                        if (CaptchaCache.Instance.VerifyCaptcha(captcha, privateKey) < 0)//kiểm tra mã cap cha <0 error
        //                        {
        //                            return new
        //                            {
        //                                ResponseCode = -100,
        //                                Message = ErrorMsg.InValidCaptCha
        //                            };
        //                        }
        //                        //clear cache khi login thành công
        //                        CacheLogin.ClearCache(username, ServiceID, "login");
        //                    }
        //                }

        //                loginAccount = AccountDAO.Instance.UserLogin(username, Security.SHA256Encrypt(password), loginType, deviceType, ServiceID, out response);
        //                if ((loginAccount != null && (loginAccount.Status != 1 && loginAccount.Status != 3)) || response == -109)//tài khoản bị khóa
        //                {
        //                    return new
        //                    {
        //                        ResponseCode = -1005,
        //                        Message = ErrorMsg.AccountLock
        //                    };
        //                }
        //                if (loginAccount == null)
        //                {
        //                    if (numberLogin < 3)//nếu login sai thì add vào cache
        //                    {
        //                        CacheLogin.AddCache(120, username, ServiceID, "login");
        //                    }
        //                }
        //                if (response == 0)//mã lỗi user name or pass incorrect
        //                {
        //                    return new
        //                    {
        //                        ResponseCode = -1,
        //                        Message = ErrorMsg.UserNameOrPassInCorrect
        //                    };
        //                }
        //                else if (loginAccount == null)
        //                {
        //                    return new
        //                    {
        //                        ResponseCode = -1005,
        //                        Message = ErrorMsg.UserNameOrPassInCorrect
        //                    };
        //                }
        //                else if (response == 1 && loginAccount != null)//nếu login thành công
        //                {
        //                    if (loginAccount.AuthenType == 1 && String.IsNullOrEmpty(otp))//kiểm tra authen type và opt (lần đầu login yêu cầu otp)
        //                    {
        //                        return new
        //                        {
        //                            ResponseCode = -3,
        //                            Message = ErrorMsg.OtpEmpty
        //                        };
        //                    }
        //                    else if ((loginAccount.AuthenType == 1 && !String.IsNullOrEmpty(otp)))//truyền lại otp
        //                    {
        //                        if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
        //                        {
        //                            return new
        //                            {
        //                                ResponseCode = -1005,
        //                                Message = ErrorMsg.OtpLengthInValid
        //                            };
        //                        }
        //                        int resOtp = 0;
        //                        long otpID = 0;
        //                        SMSDAO.Instance.ValidOtp(loginAccount.AccountID, otp.Length == OTPSAFE_LENGTH ? loginAccount.PhoneSafeNo : loginAccount.PhoneNumber, otp, ServiceID, out resOtp, out otpID, out msg);//otp valid 
        //                        if (resOtp != 1)//otp not success
        //                        {
        //                            return new
        //                            {
        //                                ResponseCode = -5,
        //                                Message = ErrorMsg.OtpIncorrect
        //                            };
        //                        }

        //                    }
        //                    else if (response == -99)//mã lỗi hệ thống bận
        //                    {
        //                        return new
        //                        {
        //                            ResponseCode = -99,
        //                            Message = ErrorMsg.InProccessException
        //                        };
        //                    }
        //                    CacheLogin.ClearCache(username, ServiceID, "login");
        //                    int responseLog = 0;
        //                    AccountDAO.Instance.IPLog(1, loginAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, out responseLog);
        //                    var userRank = AccountDAO.Instance.GetUserRank(loginAccount.AccountID, out responseLog);
        //                    loginAccount.RankID = userRank == null ? 5 : userRank.RankID;
        //                    loginAccount.VP = userRank == null ? 0 : userRank.VP;
        //                    loginAccount.PhoneNumber = loginAccount.PhoneNumber.PhoneDisplayFormat();
        //                    loginAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
        //                    // SetAuthCookie(loginAccount);
        //                    return new
        //                    {
        //                        ResponseCode = 1,
        //                        Token = TokenHashprovider.GenerateToken(loginAccount.AccountID, loginAccount.AccountName, ServiceID, clientIp, TokenExpired, loginAccount.AvatarID),
        //                        AccountInfo = loginAccount
        //                    };
        //                }
        //                else
        //                {
        //                    return new
        //                    {
        //                        ResponseCode = -99,
        //                        Message = ErrorMsg.InProccessException
        //                    };
        //                }
        //                break;
        //            case (int)Constants.enmAuthenType.AUTHEN_FB://trường hợp login facebok
        //                string accessToken = input.AccessToken ?? string.Empty;
        //                if (accessToken != null)
        //                {
        //                    var list = FacebookUtil.GetIDsForBusiness(accessToken);
        //                    if (list == null || !list.Any())
        //                    {
        //                        return new
        //                        {
        //                            ResponseCode = -103,
        //                            Message = ErrorMsg.FacebookGetFail
        //                        };
        //                    }
        //                    string appID = ConfigurationManager.AppSettings["APP_FB_ID"].ToString().Trim();
        //                    var userFBInfo = list.FirstOrDefault(c => c.app.id == appID);
        //                    if (userFBInfo == null || userFBInfo.app == null)
        //                    {
        //                        return new
        //                        {
        //                            ResponseCode = -103,
        //                            Message = ErrorMsg.FacebookGetFail
        //                        };
        //                    }
        //                    string fbid = userFBInfo.id.ToString();
        //                    //FacebookUtil.FbUserInfo userFBInfo = FacebookUtil.GetFbUserInfo(accessToken);//lấy thông tin facebook
        //                    //if (userFBInfo == null || userFBInfo.ResponeCode < 0)//không thể lấy được facebook
        //                    //{
        //                    //    return new
        //                    //    {
        //                    //        ResponseCode = -103,
        //                    //        Message = ErrorMsg.FacebookGetFail
        //                    //    };
        //                    //}
        //                    // string fbid = userFBInfo.UserId.ToString();//lấy facebook id
        //                    loginAccount = AccountDAO.Instance.UserLoginFB(fbid, string.Empty, loginType, deviceType, ServiceID, out response);//login facebook
        //                    if ((loginAccount != null && (loginAccount.Status != 1 && loginAccount.Status != 3)) || response == -109)
        //                    {
        //                        return new
        //                        {
        //                            ResponseCode = -1005,
        //                            Message = ErrorMsg.AccountLock
        //                        };
        //                    }
        //                    if (loginAccount == null)//không tồn tại user facebook
        //                    {
        //                        //kiểm tra số lần tạo acount
        //                        int IpResponse = 0;
        //                        AccountDAO.Instance.IPCheck(clientIp, out IpResponse);
        //                        if (IpResponse != 1)
        //                        {
        //                            return new
        //                            {
        //                                ResponseCode = -1005,
        //                                Message = ErrorMsg.CreateAccountManyTime
        //                            };
        //                        }
        //                        ///get current user id
        //                        long UserId = 0;
        //                        UserDAO.Instance.UserGetSequence(out UserId);
        //                        if (UserId <= 0)
        //                        {
        //                            return new
        //                            {
        //                                ResponseCode = -99,
        //                                Message = ErrorMsg.InProccessException
        //                            };
        //                        }
        //                        //tạo mới facebook account
        //                        string referUrl = input.ReferUrl ?? GetReferUrlByService();
        //                        //int res = 0;
        //                        //ChoSaoDAO.Instance.UserChoSaoInsert(null, null, null, null, "", string.Empty, fbid, out res, UserId);
        //                        //if (res == 1)
        //                        //{
        //                        int outResponse = 0;
        //                        var registerAccount = AccountDAO.Instance.CreateAccount(UserId, loginType, deviceType, fbid, string.Empty, clientIp, 1, 1, null, null, ServiceID, null, out outResponse);
        //                        //đồng bộ sang chợ sao
        //                        if (outResponse == 1 && registerAccount != null)
        //                        {
        //                            int responseLog = 0;
        //                            AccountDAO.Instance.IPLog(2, registerAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, out responseLog);
        //                            var userRank = AccountDAO.Instance.GetUserRank(registerAccount.AccountID, out outResponse);
        //                            registerAccount.RankID = userRank == null ? 5 : userRank.RankID;
        //                            registerAccount.PhoneNumber = registerAccount.PhoneNumber.PhoneDisplayFormat();
        //                            registerAccount.VP = userRank == null ? 0 : userRank.VP;
        //                            registerAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
        //                            //set cookies
        //                            // SetAuthCookie(registerAccount);
        //                           AddRefer(referUrl, loginType, registerAccount.AccountID, clientIp, deviceType);
        //                            return new
        //                            {
        //                                ResponseCode = 1,
        //                                Token = TokenHashprovider.GenerateToken(registerAccount.AccountID, registerAccount.AccountName ?? string.Empty, ServiceID, clientIp, TokenExpired, registerAccount.AvatarID),
        //                                AccountInfo = registerAccount
        //                            };
        //                        }
        //                        else if (outResponse == -102)
        //                        {
        //                            return new
        //                            {
        //                                ResponseCode = -1005,
        //                                Message = ErrorMsg.FBRegister
        //                            };
        //                        }
        //                        else
        //                        {
        //                            //tạo không được báo lỗi hệ thống
        //                            NLogManager.LogMessage("Msg " + outResponse);
        //                            return new
        //                            {
        //                                ResponseCode = -99,
        //                                Message = ErrorMsg.InProccessException
        //                            };
        //                        }
        //                        //}
        //                        //else if (res == -102)
        //                        //{
        //                        //    return new
        //                        //    {
        //                        //        ResponseCode = -1005,
        //                        //        Message = ErrorMsg.FBRegister
        //                        //    };
        //                        //}
        //                        //else
        //                        //{
        //                        //    return new
        //                        //    {
        //                        //        ResponseCode = -99,
        //                        //        Message = ErrorMsg.InProccessException
        //                        //    };
        //                        //}
        //                    }
        //                    //login facebook thành công
        //                    else if (loginAccount != null)//login facebook thành công
        //                    {
        //                        if (loginAccount.AuthenType == 1 && String.IsNullOrEmpty(otp))//otp nếu authen type
        //                        {
        //                            return new
        //                            {
        //                                ResponseCode = -3,
        //                                Message = ErrorMsg.OtpEmpty
        //                            };
        //                        }
        //                        else if ((loginAccount.AuthenType == 1 && !String.IsNullOrEmpty(otp)))//yêu cầu otp
        //                        {
        //                            if (otp.Length != OTPAPP_LENGTH && otp.Length != OTPSMS_LENGTH && otp.Length != OTPSAFE_LENGTH)
        //                            {
        //                                return new
        //                                {
        //                                    ResponseCode = -1005,
        //                                    Message = ErrorMsg.OtpLengthInValid
        //                                };
        //                            }
        //                            int resOtp = 0;
        //                            long otpID;
        //                            string otmsg;
        //                            SMSDAO.Instance.ValidOtp(loginAccount.AccountID, otp.Length == OTPSAFE_LENGTH ? loginAccount.PhoneSafeNo : loginAccount.PhoneNumber, otp.Trim(), ServiceID, out resOtp, out otpID, out otmsg);
        //                            if (resOtp != 1)
        //                            {
        //                                return new
        //                                {
        //                                    ResponseCode = -5,
        //                                    Message = otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
        //                                };
        //                            }

        //                        }
        //                        //set cookies
        //                        int responseLog = 0;
        //                        AccountDAO.Instance.IPLog(1, loginAccount.AccountID, deviceType, clientIp, ServiceID, deviceId, out responseLog);
        //                        var userRank = AccountDAO.Instance.GetUserRank(loginAccount.AccountID, out responseLog);
        //                        loginAccount.RankID = userRank == null ? 5 : userRank.RankID;
        //                        loginAccount.VP = userRank == null ? 0 : userRank.VP;
        //                        loginAccount.RankName = userRank == null ? LowestVips : userRank.RankName ?? LowestVips;
        //                        //SetAuthCookie(loginAccount);
        //                        return new
        //                        {
        //                            ResponseCode = 1,
        //                            Token = TokenHashprovider.GenerateToken(loginAccount.AccountID, loginAccount.AccountName, ServiceID, clientIp, TokenExpired, loginAccount.AvatarID),
        //                            AccountInfo = loginAccount
        //                        };
        //                    }
        //                    else //lỗi hệ thống khi login 
        //                    {
        //                        NLogManager.LogMessage("Msg" + 2);
        //                        return new
        //                        {
        //                            ResponseCode = -99,
        //                            Message = ErrorMsg.InProccessException
        //                        };
        //                    }
        //                }
        //                break;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var st = new StackTrace(ex, true);
        //        // Get the top stack frame
        //        var frame = st.GetFrame(0);
        //        // Get the line number from the stack frame
        //        var line = frame.GetFileLineNumber();
        //        NLogManager.LogError("Line" + line);
        //        NLogManager.PublishException(ex);
        //    }
        //    return new
        //    {
        //        ResponseCode = -99,
        //        Message = ErrorMsg.InProccessException
        //    };
        //}
    }
}
