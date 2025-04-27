using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class ChargeCodeInfo
    {
        public long Idx { get; set; }   
        public string ChargeCode { get; set; }
        public int ChargeType { get; set; }
        public string Data { get; set; }
        public long UserId { get; set; }
        public long Amount { get; set; }
        public long ReceivedMoney { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReferId { get; set; }
        public string ExpireAt { get; set; }

        public string GetBankCode()
        {
            string[] data = Data.Split('|');
            return data[1];
        }
    }
}