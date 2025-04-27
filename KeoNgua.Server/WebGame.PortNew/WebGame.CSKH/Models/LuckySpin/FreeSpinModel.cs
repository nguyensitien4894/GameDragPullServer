using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Models.LuckySpin
{
    public class FreeSpinModel
    {
        public int FreeSpinID { get; set; }
        public string Name { get; set; }
        //[Range(1, 3, ErrorMessage = "Chọn phòng 1, 2 hoặc 3")]
        public int RoomID { get; set; }
        public string RoomIDFormat
        {
            get { return RoomID.IntToRoomFormat(); }
        }

        //[Range(1, int.MaxValue, ErrorMessage = "Lượt quay nhỏ nhất là {1}")]
        public int SpinQuantity { get; set; }
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