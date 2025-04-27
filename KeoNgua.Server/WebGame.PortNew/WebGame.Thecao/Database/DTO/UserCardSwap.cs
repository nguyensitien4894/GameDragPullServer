using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class UserCardSwap
    {
        public long? AccountID { get; set; }
        public long UserCardSwapID { get; set; }
        public long? OrgAmount { get; set; }

        //public string CardNumber { get; set; }

        //public string CardSerial { get; set; }

        public long? CardValue { get; set; }

        //public int? TelOperatorID { get; set; }

        public string TelOperatorCode { get; set; }

        //public string ExpiryDate { get; set; }

        public int Status { get; set; }

        //public DateTime? ImportDate { get; set; }

        public DateTime? BuyDate { get; set; }

        //public string CardIndex { get; set; }

        //public int? DeviceType { get; set; }
        public long BitExchange { get; set; }

        //miêu tả statusStr
        public string  StatusStr {
            get
            {
                
                if (Status == 1)
                {
                    return "Hợp lệ";
                }
                else if (Status == 2)
                {
                    return "Chờ duyệt";
                }else if (Status == 3)
                {
                    return "Hủy thẻ";
                }
                else
                {
                    return "Thất bại";
                }
            }
        } 
    }
}