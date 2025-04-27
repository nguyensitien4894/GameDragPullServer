using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class UserCardRecharge
    {
        public long RequestID { get; set; }

        public long UserID { get; set; }

        public int TelOperatorID { get; set; }

        public int CardID { get; set; }

        public string CardNumber { get; set; }

        public string SerialNumber { get; set; }

        public int CardValue { get; set; }

        public double TeleRate { get; set; }

        public long? ReceivedMoney { get; set; }

        public int Status { get; set; }
        public double Rate { get; set; }



        public string Description { get; set; }

        public string OperatorCode { get; set; }
        public string PartnerErrorCode { get; set; }


        public DateTime CreateDate { get; set; }



        public string StatusStr
        {
            get
            {
                if (Status ==0)
                {
                    return "Waiting for progressing";
                }else if (Status == -1)
                {
                    return "Failure";
                }
                else if (Status == -2)
                {
                    return "Failure";
                }
                else if (Status == -3)
                {
                    return "Failure-SMG";
                }
                else if (Status == 2)
                {
                    return "Success";
                }
                else if (Status == 1)
                {
                    return "Waiting for progressing";
                }
                else if (Status == 3)
                {
                    return "Waiting for progressing";
                }
                else if (Status == 4)
                {
                    return "Success-SMG";
                }
                else 
                {
                    return "Failure";
                }
            }
        }
    }
    public class UserCardRecharge_New
    {

        public long UserID { get; set; }


        public string Type { get; set; }

        public long CardValue { get; set; }

        public long ReceivedMoney { get; set; }

        public long Status { get; set; }


        public DateTime CreateDate { get; set; }

        public string StatusStr
        {
            get
            {
                if (Status == 1)
                {
                    return "Đang xử lý";
                }
                else if (Status == 2)
                {
                    return "Hết thời gian xử lý";
                }
                else if (Status == 3)
                {
                    return "Không thành công";
                }
                else if (Status == 4)
                {
                    return "Thành công";
                }
                else
                {
                    return "Không thành công";
                }
            }
        }

        //public string StatusStr
        //{
        //    get
        //    {
        //        if (Status == 0)
        //        {
        //            return "Waiting for progressing";
        //        }
        //        else if (Status == -1)
        //        {
        //            return "Failure";
        //        }
        //        else if (Status == -2)
        //        {
        //            return "Failure";
        //        }
        //        else if (Status == -3)
        //        {
        //            return "Failure-SMG";
        //        }
        //        else if (Status == 2)
        //        {
        //            return "Success";
        //        }
        //        else if (Status == 1)
        //        {
        //            return "Waiting for progressing";
        //        }
        //        else if (Status == 3)
        //        {
        //            return "Waiting for progressing";
        //        }
        //        else if (Status == 4)
        //        {
        //            return "Success-SMG";
        //        }
        //        else
        //        {
        //            return "Failure";
        //        }
        //    }
        //}
    }
}