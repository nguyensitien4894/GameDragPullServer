namespace MsTraditionGame.Utilities.Utils
{
    public static class MoneyExtension
    {
        /// <summary>
        /// 100000=>1.000.000
        /// </summary>
        /// <returns></returns>
        public static string LongToMoneyFormat(this long number) {
            if (number==0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }

        public static string IntToMoneyFormat(this int number)
        {
            if (number == 0) return "0";
            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
        public static string IntToMoneyFormat(this int? number)
        {
            if (!number.HasValue) return string.Empty;
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

        public static string LongToMoneyTrans(this long number)
        {
            if (number == 0) return "0";
            if(number > 0)
                return string.Format("+{0:0,0}", number).Replace(',', '.');

            return string.Format("{0:0,0}", number).Replace(',', '.');
        }
    }
}
