using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class AccountIP
    {
        public long AccountID { get; set; }
        public string DisplayName { get; set; }
        public int Status { get; set; }
        public string StatusFormat { get { return ConvertStatus(Status); } }

        public static string ConvertStatus(int inputValue)
        {
            string val = string.Empty;
            if(inputValue==1)
                val = "Hoạt động";
            if (inputValue == 0)
                val = "Khóa";
            if (inputValue == 2)
                val = "Khóa toàn bộ";
            return val;
        }

    }
}