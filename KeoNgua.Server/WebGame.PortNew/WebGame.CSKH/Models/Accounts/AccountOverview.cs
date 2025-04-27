using System;
using System.Collections.Generic;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Accounts
{
    public class AccountOverview
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string GameAccountName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? PhoneUpdateDate { get; set; }
        public long Balance { get; set; }
        public long SafeBalance { get; set; }
        public long TotalBalance { get; set; }
        public long TotalRecharge { get; set; }
        public long TotalSwap { get; set; }
        public int? AuthenType { get; set; }
        public int Status { get; set; }
        public int IsBlackList { get; set; }
        public int ServiceID { get; set; }
        public int RankID { get; set; }
        public long VipPoint { get; set; }
        public long RefundAmount { get; set; }
        public long TotalRefundAmount { get; set; }
        public long EventValue { get; set; }
        public long GratefulValue { get; set; }
        public long TotalAward { get { return TotalRefundAmount + EventValue + GratefulValue; } }
        public long TotalValueOutUser { get; set; } // Giao dịch user - user
        public long TotalValueInUser { get; set; } // Giao dịch user - user
        public long TotalValueOutAgency { get; set; } // Giao dịch agency, admin - user
        public long TotalValueInAgency { get; set; }// Giao dịch agency, admin - user
        public long TotalValueOutAdmin { get; set; }
        public long TotalValueInAdmin { get; set; }
        public long TotalGiftcode { get; set; }
        public long TotalRechargeBank { get; set; }
        public long TotalValueOut { get { return TotalValueOutUser + TotalValueOutAgency + TotalValueOutAdmin; } }
        public long TotalValueIn { get { return TotalValueInUser + TotalValueInAgency + TotalValueInAdmin; } }
        public string PhoneOTPSafe { get; set; }
        public int HasNote { get; set; }
        public bool IsWrong { get; set; }
        public long TotalSwapBank { get; set; }
        public long TotalSwapMomo { get; set; }

        public long TotalQuaterRedemptionValue { get; set; }
        public long TotalSMSCharge { get; set; }

        public long FishCash { get; set; }

        public long TotalVipLoanValue { get; set; }
        public AccountQuaterInfor BeforQuater { get; set; }
        public AccountQuaterInfor CurrentQuater { get; set; }
        public long TotalMomo { get; set; }

        public decimal TotalElapse
        {
            get
            {
                return TotalIn - TotalOut;
            }
        }
        public decimal TotalOut {


            get
            {
                return TotalSwapMomo + TotalSwap + TotalSwapBank + TotalValueOutUser + TotalValueOutAgency + TotalValueOutAdmin;
            }
        }
        public decimal TotalIn
        {

            get
            {
                return TotalRefundAmount + TotalVipLoanValue + TotalQuaterRedemptionValue + EventValue + GratefulValue + TotalRecharge + TotalGiftcode + TotalRechargeBank + TotalMomo + TotalValueInUser + TotalValueInAgency + TotalValueInAdmin;
            }
        }

        public string StatusStr
        {
            get
            {
                if (Status == 0)
                {
                    return "Đã xóa";
                }
                if (Status == 1)
                {
                    return "Hoạt động";
                }
                if (Status == 2)
                {
                    return "Khóa toàn bộ";
                }
                if (Status == 3)
                {
                    return "Khóa giao dịch";
                }
                if (Status == 4)
                {
                    return "Khóa hack";
                }
                return string.Empty;




            }
        }
    }

    public class AccountQuaterInfor
    {
        
        public long LoanAmount { get; set; }
        public long OldDebt { get; set; }
        public long QuaterAcc { get; set; }
        public int CurrentQuater { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LoanAmountFormat { get { return LoanAmount.LongToMoneyFormat(); } }
        public string OldDebtFormat { get { return OldDebt.LongToMoneyFormat(); } }
        public string QuaterAccFormat { get { return QuaterAcc.LongToMoneyFormat(); } }

    }
    public class AccountPlayGame
    {
        public int GameID { get; set; }
        public string GameName { get { return ExtensionConvert.IntToGameFormat(GameID); } }
        public long TotalBetValue { get; set; }
        public long TotalPrizeValue { get; set; }
        public long RateBetPrize { get { return TotalPrizeValue - TotalBetValue; } }
    }
}