using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TraditionGame.Utilities.Constants;

namespace MsWebGame.SafeOtp.Helpers
{
    public class ValidateInput
    {
        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            if (IsContainSpace(phoneNumber)) return false;
            if (!phoneNumber.StartsWith("0")) return false;
            if (phoneNumber.StartsWith("00")) return false;
            if (phoneNumber.Length != 10) return false;
            phoneNumber = phoneNumber.Replace(".", "").Trim();

            var IsPhone = new Regex(@"(09|03[2|3|4|5|6|7|8|9]|07[0|9|7|6|8]|08[3|4|5|1|2|6|8|7|8|9])+([0-9]){7,}\b");
            return IsPhone.IsMatch(phoneNumber);
        }

        public static bool ValidatePhoneNumberWithRegion(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            if (IsContainSpace(phoneNumber)) return false;
            //if (!phoneNumber.StartsWith("0")) return false;
            if (phoneNumber.StartsWith("00")) return false;
            //if (phoneNumber.Length != 10) return false;
            phoneNumber = phoneNumber.Replace(".", "").Trim();

            var IsPhone = new Regex(@"(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|
                                        9[8543210]|8[6421]|6[6543210]|5[87654321]|
                                        4[987654310]|3[9643210]|2[70]|7|1)\d{1,14}$");
            return IsPhone.IsMatch(phoneNumber);
        }
        public static  bool IsVieNamMobilePhone(string phoneNumber)
        {
            var listPhone = new List<string>() { "092", "056", "058", "0186", "0188" };
            var splitPhone = phoneNumber.Substring(0, 3);
            return listPhone.Contains(splitPhone);


        }
        public static bool IsContainSpace(string str)
        {
            return str.Contains(" ");
        }
        public static bool IsValidatePass(string password)
        {
            if (String.IsNullOrEmpty(password)) return false;
            if (password.Length < 6 || password.Length > 16) return false;
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{6,16}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            //if ((!hasLowerChar.IsMatch(password) && !hasUpperChar.IsMatch(password)) || !hasMiniMaxChars.IsMatch(password) || !hasNumber.IsMatch(password) || hasSymbols.IsMatch(password))
            //{

            //    return false;
            //}

            return true;
        }
        public static bool IsValidOtp(string otp)
        {
            if (String.IsNullOrEmpty(otp)) return false;
            return true;
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

        public static bool ValidateUserName(string username)
        {
           
            if (String.IsNullOrEmpty(username) )
                return false;

            Regex rUserName = new Regex("^[a-zA-Z0-9]{6,16}$");
            if (!rUserName.IsMatch(username))
            {
               
               
                return false;
            }
            return true;
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

        public static bool IsNickNameContainNotAllowString(string nickname)
        {

            
            var strNotAllow = ConfigurationManager.AppSettings["NOTALOWSTRING"].ToString();
            if (String.IsNullOrEmpty(strNotAllow)) return false;
            var arrStr = strNotAllow.Split(',');
            if (arrStr == null || !arrStr.Any()) return false;
            foreach(var item in arrStr)
            {
                if (nickname.ToLower().Contains(item.ToLower())) {
                    return true;
                }
            }

           return false;
        }
    }
}