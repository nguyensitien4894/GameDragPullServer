using System;
using System.Globalization;

namespace TraditionGame.Utilities.Utils
{
    public static class MoneyExtension
    {
        public static long  MoneyCelling(this double number)
        {
            return ConvertUtil.ToLong(Math.Ceiling(number));
        }
        public static long MoneyFloor(this double number)
        {
            return ConvertUtil.ToLong(Math.Floor(number));
        }
        public static string DoubleMoneyFormat(this double number)
        {
            
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static string DoubleMoneyFormat(this double? number)
        {
            if (!number.HasValue) return "0";
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static string LongToMoneyFormat(this long number)
        {
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static string LongToMoneyFormatNew(this long number)
        {
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number);
        }

        public static string IntToMoneyFormat(this int number)
        {
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static string IntToMoneyFormat(this int? number)
        {
            if (!number.HasValue||number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static string LongToMoneyFormat(this long? number)
        {
            if (number == null) return string.Empty;
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }

        public static string DecimalToMoneyFormat(this decimal? number)
        {
            if (number == null) return string.Empty;
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static string DecimalToMoneyFormat(this decimal number)
        {
           
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static long MoneyFormatRound(this long number)
        {
            if (number <=0) return 0;
            if (number > 0 && number < 1000) return number;
            var n = (int)number / 1000;
            return n * 1000;

        }

        public static string DecimalToFormat(this decimal? number)
        {
            if (number == null) return string.Empty;
            if (number == 0) return "0";
            var newNum = string.Format("{0:0.000}", number);
            string[] arrNum = newNum.Split('.');
            var first = string.Format("{0:0,0}", long.Parse(arrNum[0])).Replace(',', '.');
            var second = arrNum[1];
            return first + "," + second;
        }

        public static string FormatMoneyVND(long price)
        {
            return price.ToString("0,0", CultureInfo.CreateSpecificCulture("el-GR"));
        }
    }
}
