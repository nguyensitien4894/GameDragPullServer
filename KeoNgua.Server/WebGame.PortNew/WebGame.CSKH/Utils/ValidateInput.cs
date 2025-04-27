using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TraditionGame.Utilities.Constants;

namespace MsWebGame.CSKH.Utils
{
    public class ValidateInput
    {
        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            if (phoneNumber.Length != 10)
                return false;

            Regex reg = new Regex(@"^[0]?[9|8|7|5|3]{1}?[0-9]{8}$");
            return reg.IsMatch(phoneNumber);
        }

        public static bool ValidateUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            Regex rUserName = new Regex("^[a-zA-Z0-9_.-]{4,16}$");
            if (!rUserName.IsMatch(username))
            {
                return false;
            }

            return true;
        }

        public static bool ValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static bool ValidateStringNumber(string str)
        {

            if (String.IsNullOrEmpty(str))
                return false;

            Regex rUserName = new Regex("^[0-9]*$");
            if (!rUserName.IsMatch(str))
            {


                return false;
            }
            return true;
        }
        public static bool IsValidatePass(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (password.Length < 6 || password.Length > 16) return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{6,16}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            //if ((!hasLowerChar.IsMatch(password) && !hasUpperChar.IsMatch(password)) || !hasMiniMaxChars.IsMatch(password)
            //    || !hasNumber.IsMatch(password) || hasSymbols.IsMatch(password))
            //{
            //    return false;
            //}

            return true;
        }

        public static bool IsValidatePassSpecial(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (password.Length < 8 || password.Length > 16) return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,16}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[$&+,:;=?@#|'<>.^*()%!-]");

            //if ((!hasLowerChar.IsMatch(password) && !hasUpperChar.IsMatch(password)) || !hasMiniMaxChars.IsMatch(password)
            //    || !hasNumber.IsMatch(password) || !hasSymbols.IsMatch(password))
            //{
            //    return false;
            //}

            return true;
        }
        public static bool IsNumber(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
           

            var hasNumber = new Regex(@"[0-9]+");
         
         

            if ( !hasNumber.IsMatch(input) )
            {
                return false;
            }

            return true;
        }
        public static bool IsValidOtp(string otp)
        {
            if (String.IsNullOrEmpty(otp)) return false;
            
            return true;
        }

        public static bool IsContainSpace(string str)
        {
            return str.Contains(" ");
        }
      
        public static bool IsValidated(string username, string password, out string msg, out int resCode)
        {
            msg = string.Empty;
            resCode = 0;
            if (username == null || password == null)
                return false;

            Regex rUserName = new Regex("^[a-zA-Z0-9_.-]{6,20}$");
            Regex rPassword = new Regex("^[a-zA-Z0-9_.-]{6,18}$");

            if (!rUserName.IsMatch(username))
            {
                msg = Constants.ACCOUNTNAME_INPUT_ERROR_MESSAGE;
                resCode = Constants.ACCOUNTNAME_INPUT_ERROR_CODE;
                return false;
            }

            if (!rPassword.IsMatch(password))
            {
                msg = Constants.PASSWORD_INPUT_ERROR_MESSAGE;
                resCode = Constants.PASSWORD_INPUT_ERROR_CODE;
                return false;
            }
            return true;
        }

        public static bool IsValidDevice(string deviceId, int? deviceType)
        {
            return ((deviceType == (int)Constants.enmDeviceType.Android
                || deviceType == (int)Constants.enmDeviceType.IOS
                || deviceType == (int)Constants.enmDeviceType.WindowPhone
                || deviceType == (int)Constants.enmDeviceType.Web)
                && deviceId != null);
        }

        public static bool IsValidLoginType(int loginType)
        {
            return (loginType == (int)Constants.enmAuthenType.AUTHEN_ID
                || loginType == (int)Constants.enmAuthenType.AUTHEN_FB);
        }

        public static bool IsValidNickName(string nickname)
        {
            if (nickname == null) return false;
            nickname = nickname.Trim();
            Regex rNickname = new Regex("^[0-9a-zA-Z_ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ ]{6,20}$");
            if (!rNickname.IsMatch(nickname)) return false;
            return true;
        }
    }
}