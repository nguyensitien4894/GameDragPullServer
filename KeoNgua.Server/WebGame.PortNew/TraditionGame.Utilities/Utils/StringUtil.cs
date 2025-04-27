using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MsTraditionGame.Utilities.Utils
{
    public class StringUtil
    {



        public static string MaskUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return "";
            }

            int length = username.Length;
            //neu dai it nhat 11 ky tu -> lay 8 ky tu dau và ***
            if (length > 10)
            {
                username = string.Format("{0}***", username.Substring(0, 8));
            }
            //neu dai it nhat 7 ky tu -> thi *** 3 ky tu cuoi
            else if (length > 6)
            {
                username = string.Format("{0}***", username.Substring(0, length - 3));
            }
            //neu dai it nhat 4 ky tu -> lay 3 ky tu dau va ***
            else if (length > 3)
            {
                username = string.Format("{0}***", username.Substring(0, 3));
            }
            else
            {
                username = string.Format("{0}***", username.Substring(0, 1));
            }

            return username;
        }

        public static string RemoveLastStr(string inputStr)
        {
            if (string.IsNullOrEmpty(inputStr))
                return string.Empty;

            inputStr = inputStr.Substring(0, inputStr.Length - 1);
            return inputStr;
        }

        public static string FilterInjectionChars(string input)
        {
            input = input.Replace('\u0001', ' ');
            input = input.Replace('\u0002', ' ');
            input = input.Replace('\u0003', ' ');
            input = input.Replace('\u0004', ' ');
            input = input.Replace('\t', ' ');
            return input.Trim();
        }
    }

    public static class ConvertUtil
    {
        public static int ToInt(object obj)
        {
            if (obj == null) return -1;
            int value = 0;
            int.TryParse(obj.ToString(), out value);
            return value;
        }

        public static long ToLong(object obj)
        {
            if (obj == null) return -1;
            long value = 0;
            long.TryParse(obj.ToString(), out value);
            return value;
        }

        public static string ToString(object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        public static bool ToBool(object obj)
        {
            if (obj == null) return false;
            bool value = false;
            bool.TryParse(obj.ToString(), out value);
            return value;
        }

        public static decimal ToDecimal(object obj)
        {
            if (obj == null) return 0;
            decimal value = 0;
            decimal.TryParse(obj.ToString(), out value);
            return value;
        }

        public static float ToFloat(object obj)
        {
            if (obj == null) return -1.0f;
            float value = 0.0f;
            float.TryParse(obj.ToString(), out value);
            return value;
        }
    }

    public static class StringExt
    {
        public static string CusSubString(this string s, int length)
        {
            try
            {
                if (String.IsNullOrEmpty(s)) return string.Empty;
                if (s.Length <= length) return s;
                string result = s.Substring(0, length).ToString();
                return String.Format("{0}...", result);
            }
            catch
            {
                return string.Empty;
            }


        }
        public static string PhoneFormat(this string s)
        {
            return String.IsNullOrEmpty(s) ? null : s.StartsWith("0") ? "84" + s.Substring(1) : s;
        }
        public static string PhoneDisplayFormat(this string s)
        {
            return String.IsNullOrEmpty(s) ? null : s.StartsWith("84") ? "0" + s.Substring(2) : s;
        }
        public static string Replace(this string s, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (s == null)
                return null;

            if (String.IsNullOrEmpty(oldValue))
                return s;

            StringBuilder result = new StringBuilder(Math.Min(4096, s.Length));
            int pos = 0;

            while (true)
            {
                int i = s.IndexOf(oldValue, pos, comparisonType);
                if (i < 0)
                    break;

                result.Append(s, pos, i - pos);
                result.Append(newValue);

                pos = i + oldValue.Length;
            }
            result.Append(s, pos, s.Length - pos);

            return result.ToString();
        }
    }
}
