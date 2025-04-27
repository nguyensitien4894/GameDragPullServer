using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class PresentSpins
    {
        public int ID { get; set; }

        public int Quantity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class DBit
    {
        public int PrizeID { get; set; }
        public string Name { get; set; }
        public long PrizeValue { get; set; }
        // Số lượng giải cho user mới
        public int New { get; set; } 
        // Số lượng giải cho user có rank ĐÁ
        public int Stone { get; set; }
        // Số lượng giải cho user có rank ĐỒNG hoặc Bạc
        public int BronzeSilver { get; set; }
        // Số lượng giải cho user có rank VÀNG hoặc KIM CƯƠNG
        public int GoldDiamond { get; set; }
        public int ServiceID { get; set; }

    }

    public class FreeSpin
    {
        public int FreeSpinID { get; set; }
        public string Name { get; set; }
        public int RoomID { get; set; }
        public int SpinQuantity { get; set; }
        // Số lượng giải cho user mới
        public int New { get; set; }
        // Số lượng giải cho user có rank ĐÁ
        public int Stone { get; set; }
        // Số lượng giải cho user có rank ĐỒNG hoặc Bạc
        public int BronzeSilver { get; set; }
        // Số lượng giải cho user có rank VÀNG hoặc KIM CƯƠNG
        public int GoldDiamond { get; set; }
        public int ServiceID { get; set; }

    }

    public class DBitReport
    {
        public long SpinID { get; set; }
        public int PrizeID { get; set; }
        public string Username { get; set; }
        public string PrizeName { get; set; }
        public int FreeSpinID { get; set; }
        public int FreeSpinValue { get; set; }
        public string FreeSpinName { get; set; }
        public string Name { get; set; }
        public string RankName { get; set; }
        public long PrizeValue { get; set; }
        public DateTime DateSpin { get; set; }
        public int ReciveAward { get; set; }
        public int AwardLimit { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class LuckySpinSearch
    {
        public string Username { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PrizeID { get; set; }
        public int FreeSpinID { get; set; }
        public int Rank { get; set; }
    }

    public class RankUser
    {
        public int ID { get; set; }
        public int Rank { get; set; }
        public string RankName { get; set; }
    }
}