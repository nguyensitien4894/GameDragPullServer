using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
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
                    return "Thành công";
                }
                else if (Status == 2)
                {
                    return "Chờ duyệt";
                }else if (Status == 3)
                {
                    return "Hủy đổi thẻ";
                }
                else
                {
                    return "Thất bại";
                }
            }
        } 
    }
    public class UserCardSwap_New
    {
        public long? AccountID { get; set; }


        public long? CardValue { get; set; }


        public string TelOperatorCode { get; set; }


        public int Status { get; set; }


        public DateTime? BuyDate { get; set; }


        public long BitExchange { get; set; }

        //miêu tả statusStr
        public string StatusStr
        {
            get
            {

                if (Status == 0)
                {
                    return "Đã ghi nhận.Chờ duyệt";
                }
                else if (Status == 1 || Status == 2)
                {
                    return "Đã duyệt chờ ngân hàng xử lý";
                }
                else if (Status == 3)
                {
                    return "Thành công";
                }
                else
                {
                    return "Thất bại";
                }
            }
        }
    }
}