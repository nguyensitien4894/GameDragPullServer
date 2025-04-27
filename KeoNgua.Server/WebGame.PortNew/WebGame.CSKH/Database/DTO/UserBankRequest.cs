using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserBankRequest
    {
        public long RequestID { get; set; }

        public int RequestType { get; set; }

        public string RequestCode { get; set; }

        public long UserID { get; set; }

        public long Amount { get; set; }

        public double? Rate { get; set; }

        public long? ReceivedMoney { get; set; }

        public long? RefundReceivedMoney { get; set; }

        public int Status { get; set; }

        public string PartnerStatus { get; set; }

        public string PartnerErrorCode { get; set; }

        public string PartnerMessage { get; set; }

        public string FeedbackErrorCode { get; set; }

        public string FeedbackMessage { get; set; }

        public string Description { get; set; }

        public double? ExchangeRate { get; set; }

        public double? Fee { get; set; }

        public int? PartnerID { get; set; }

        public double? OrgUSDTmount { get; set; }

        public double? USDTAmount { get; set; }

        public double? RemainUSDTAmount { get; set; }

        public string USDTWalletAddress { get; set; }

        public string Email { get; set; }

        public string BankName { get; set; }

        public string BankCode { get; set; }

        public string AccountBankName { get; set; }
        public string BankAccount { get; set; }

        public string BankNumber { get; set; }

        public int? ServiceID { get; set; }

        public string ServieName
        {
            get
            {
                if(ServiceID.HasValue)
                {
                    int value = ServiceID.Value;
                    if (value == 1) return "B1";
                    if (value == 2) return "B2";
                    if (value == 3) return "B3";
                }
                return string.Empty;
            }
        }

        public DateTime RequestDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// Số tiền VND thực nhận
        /// </summary>
        public double ?RealAmount { get; set; }
        /// <summary>
        /// Số tiền USD giao dịch
        /// </summary>
        public double? RealUSDTAmount { get; set; }
        public long? RealReceivedMoney { get; set; }
       
        public string ColorCss
        {
            get
            {
                var success_List = new List<int> { 1,6};
                if (success_List.Contains(Status))
                {
                    return "label label-success";
                }
                var moneyBackList = new List<int> { 4};
                if (moneyBackList.Contains(Status))
                {
                    return "label label-grey";
                }

                var failList = new List<int> { 5, -1, -2 };
                if (failList.Contains(Status) )
                {
                    if (RequestType == 1)
                    {
                        return "label label-grey";
                    }else
                    {
                        if (Status == -2)
                        {
                            return "label label-red";
                        }
                        return "label label-grey";
                    }
                    
                }
                var wattingList= new List<int> { 0,3 };
                if (wattingList.Contains(Status))
                {
                    return "label label-warning";
                }
                return string.Empty;
            }
            
        }
        
    }
}