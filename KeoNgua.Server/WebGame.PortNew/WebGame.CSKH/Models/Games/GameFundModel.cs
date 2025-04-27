using System.ComponentModel;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Games
{
    public class CCUModel
    {
        [DisplayName("Tên Game")]
        public string gamename { get; set; }

        [DisplayName("Số người đang chơi")]
        public int CCU { get; set; }



    }
    public class GameFundModel
    {
        public int GameID { get; set; }
        [DisplayName("Phòng")]
        public int RoomID { get; set; }
       
        public long? PrizeFund { get; set; }
       
        public long? JackpotFund { get; set; }
        [DisplayName("Quỹ")]
        public string PrizeFundFormat
        {
            get
            {
                return PrizeFund.LongToMoneyFormat();
            }
        }
        [DisplayName("Hũ")]
        public string JackpotFundFormat
        {
            get
            {
                return JackpotFund.LongToMoneyFormat();
            }
        }
        [DisplayName("Tên game")]
        public string GameName { get; set; }
        [DisplayName("Số người đang chơi")]
        public int CCU { get; set; }
        public string Displayname { get; set; }

        [DisplayName("Trừ Quỹ Game")]
        public string TruQuy
        {
            get
            {
                return "<button onclick='GlobalHeader.TruQuy(" + GameID + ", "+ RoomID + ")'> Trừ Quỹ </button><button onclick='GlobalHeader.TruQuy(" + GameID + ", " + RoomID + ", true)'> Cộng Quỹ </button>";
            }
        }

        [DisplayName("Sét Nổ Hũ")]
        public string SetJackpot
        {
            get
            {
                return "<button onclick='GlobalHeader.SetJackpot(" + GameID + ", " + RoomID + ")'> Sét Nổ Hũ </button> " + Displayname;
            }
        }

    }

    public class JpRateModel
    {
 
        public int RoomID { get; set; }
        [DisplayName("Loại súng")]
        public string RoomName
        {
            get
            {
                switch (RoomID) {
                    case 11:
                        return "Nông dân súng 1";
                        break;
                    case 12:
                        return "Nông dân súng 2";
                        break;
                    case 13:
                        return "Nông dân súng 3";
                        break;
                    case 14:
                        return "Nông dân súng 4";
                        break;
                    case 21:
                        return "Địa chủ súng 1";
                        break;
                    case 22:
                        return "Địa chủ súng 2";
                        break;
                    case 23:
                        return "Địa chủ súng 3";
                        break;
                    case 24:
                        return "Địa chủ súng 4";
                        break;
                    case 31:
                        return "Địa chủ súng 1";
                        break;
                    case 32:
                        return "Đại gia súng 2";
                        break;
                    case 33:
                        return "Đại gia súng 3";
                        break;
                    case 34:
                        return "Đại gia súng 4";
                        break;
               
                }

                return RoomID.ToString();
            }
        }

        [DisplayName("Tỷ lệ")]
        public long? JpRate { get; set; }

        [DisplayName("Thay đổi")]
        public string Change
        {
            get
            {
                return "<button onclick='GlobalHeader.UpdateRateJP("+ RoomID +")'> Thay đổi </button>";
            }
        }

    }
}