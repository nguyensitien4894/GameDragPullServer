using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.LuckySpin
{
    public class DBitModel
    {
        public int PrizeID { get; set; }
        public string Name { get; set; }
        public long PrizeValue { get; set; }
        // Số lượng giải cho user mới
        [Range(0, int.MaxValue, ErrorMessage = "Số giải thưởng phải từ {1} trở lên")]
        public int New { get; set; }
        // Số lượng giải cho user có rank ĐÁ
        [Range(0, int.MaxValue, ErrorMessage = "Số giải thưởng phải từ {1} trở lên")]
        public int Stone { get; set; }
        // Số lượng giải cho user có rank ĐỒNG hoặc Bạc
        [Range(0, int.MaxValue, ErrorMessage = "Số giải thưởng phải từ {1} trở lên")]
        public int BronzeSilver { get; set; }
        // Số lượng giải cho user có rank VÀNG hoặc KIM CƯƠNG
        [Range(0, int.MaxValue, ErrorMessage = "Số giải thưởng phải từ {1} trở lên")]
        public int GoldDiamond { get; set; }

        public int ServiceID { get; set; }
    }
}