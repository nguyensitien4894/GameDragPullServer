using System.Collections.Generic;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Controllers;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Utils
{
    public static class ExtensionConvert
    {
        public static string DoubleToMoneyFormat(this double inputValue)
        {
            if (inputValue == 0) return inputValue.ToString();
            return string.Format("{0:0,0}", inputValue).Replace(',', '.');
        }

        public static string BoolToConfigGameFormat(this bool inputValue)
        {
            string val = string.Empty;
            val = inputValue ? "Hiệu lực" : "Đã dùng";
            return val;
        }

        public static string BoolToStatusFormat(this bool inputValue)
        {
            string val = inputValue ? "Thành công" : "Thất bại";
            return val;
        }

        public static string BoolToUserLockFormat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 0)
                val = "Khóa tất cả";
            else if (inputValue == 1)
                val = "Khóa quảng cáo";
            else if (inputValue == 2)
                val = "Khóa mua";
            else if (inputValue == 3)
                val = "Khóa bán";
            else if (inputValue == 4)
                val = "Khóa rút";
            else if (inputValue == 5)
                val = "Khóa nạp";
            else
                val = inputValue.ToString();

            return val;
        }

        public static string IntToGameFormat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 0)
                val = "Tất cả";
            else
                val = InfoHandler.Instance.GetGameName(inputValue);

            return val;
        }

        public static string IntToRoomFormat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 0)
                val = "Tất cả";
            //else if (inputValue == 1)
            //    val = "Phòng 100";
            //else if (inputValue == 2)
            //    val = "Phòng 1K";
            //else if (inputValue == 3)
            //    val = "phòng 10K";
            else
                val = inputValue.ToString();

            return val;
        }

        public static string IntToRankFormat(this double inputValue)
        {
            string val = string.Empty;
            if (inputValue == 1)
                val = "Kim Cương";
            else if (inputValue == 2)
                val = "Vàng";
            else if (inputValue == 3)
                val = "Bạc";
            else if (inputValue == 4)
                val = "Đồng";
            else if (inputValue == 5)
                val = "Đá";
            else
                val = inputValue.ToString();

            return val;
        }

        public static string IntToRankFormat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 0)
                inputValue = 1;

            val = InfoHandler.Instance.GetPrivilegeName(inputValue);
            return val;
        }

        public static string IntToLockAndOpenFormat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 0)
                val = "Mở";
            else if (inputValue == 1)
                val = "Khóa";
            else
                val = inputValue.ToString();

            return val;
        }

        public static string ConvertStatus(this int inputValue)
        {
            string val = string.Empty;
            val = inputValue == 1 ? "Hoạt động" : inputValue == 0 ? "Khóa" : string.Empty;
            return val;
        }

        public static string PhoneNumberFormat(this string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                return string.Empty;

            string val = string.Empty;
            //if (inputValue.Substring(0, 2).Equals("84"))
                val = "0" + inputValue.Substring(2, inputValue.Length - 2);

            return val;
        }

        public static string IntToGiftCodeTypeFomat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 1)
                val = "Gift Code(1 tài khoản dùng 1 code)";
            else if (inputValue == 2)
                val = "Gift Code(1 tài khoản dùng nhiều code)";
            else
                val = inputValue.ToString();

            return val;
        }

        public static string IntToAgencyStatusFormat(this int? inputValue)
        {
            string val = string.Empty;
            if (inputValue == 0)
                val = "Ngừng hoạt động";
            else if (inputValue == 1)
                val = "Hoạt động";
            else if (inputValue == 2)
                val = "Bị khóa";
            else
                val = inputValue.ToString();

            return val;
        }

        public static string StrToTransStatusFormat(this string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                return string.Empty;

            string val = string.Empty;
            if (inputValue.Equals("SUCCESS"))
                val = "Thành công";
            else if (inputValue.Equals("CONFIRMED"))
                val = "Xác nhận giao dịch";
            else if (inputValue.Equals("PENDING"))
                val = "Khởi tạo giao dịch";
            else if (inputValue.Equals("CANCELLED"))
                val = "Hủy giao dịch";
            else if (inputValue.Equals("REFUSED"))
                val = "Từ chối giao dịch";
            else if (inputValue.Equals("RETRIEVAL"))
                val = "Đóng băng";
            else
                val = inputValue;

            return val;
        }
        public static string StrToAccountStatusFormat(this int inputValue)
        {
            if (inputValue==0)
                return string.Empty;

            string val = string.Empty;
            if (inputValue==1)
                val = "Người dùng";
            else if (inputValue==2)
                val = "Đại lý";
            else if (inputValue == 2)
                val = "Admin";
            
            else
                val = inputValue.ToString();

            return val;
        }
        public static string DoubleToFeeFormat(this double inputValue)
        {
            string val = string.Empty;
            if (inputValue <= 0)
                return "0%";

            val = inputValue * 100 + "%";
            return val;
        }

        public static string IntToReasonIDFormat(this int inputValue)
        {
            string val = string.Empty;
            switch (inputValue)
            {
                case 1:
                    val = "Chuyển tiền từ chợ sang game";
                    break;
                case 2:
                    val = "Nhận tiền từ game sang chợ";
                    break;
                case 3:
                    val = "Đại lý chuyển bit cho user";
                    break;
                case 4:
                    val = "Tổng đại lý chuyển tiền cho đại lý";
                    break;
                case 5:
                    val = "User bán bit cho user trong chợ";
                    break;
                case 6:
                    val = "Đại lý chuyển bit cho đại lý";
                    break;
                case 7:
                    val = "Admin chuyển bit cho đại lý";
                    break;
                case 8:
                    val = "Admin chuyển bit cho user";
                    break;
                case 9:
                    val = "Đại lý chuyển bit cho admin";
                    break;
                case 10:
                    val = "Tạo bản tin";
                    break;
                case 11:
                    val = "Xóa bản tin";
                    break;
                case 12:
                    val = "Người chơi hủy giao dịch";
                    break;
                case 13:
                    val = "Admin hủy giao dịch";
                    break;
                case 14:
                    val = "Admin chấp nhận giao dịch";
                    break;
                case 15:
                    val = "Rút tiền từ quảng cáo";
                    break;
                case 16:
                    val = "Chuyển ví";
                    break;
                case 17:
                    val = "Trừ tiền Otp";
                    break;
                case 18:
                    val = "Hoàn tiền Otp";
                    break;
                case 20:
                    val = "Trừ tiền admin khi tạo giftcode";
                    break;
                case 21:
                    val = "Hoàn tiền khi không tạo được giftcode";
                    break;
                default:
                    val = inputValue.ToString();
                    break;
            }

            return val;
        }

        public static string WeekDayFormat(this string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                return string.Empty;

            string val = string.Empty;
            string[] ArrInput = inputValue.Split(',');
            List<ConfigSelect> lstConfig = InfoHandler.Instance.GetWeekDayBox();
            for (int i = 0; i < ArrInput.Length; i++)
            {
                int index = int.Parse(ArrInput[i]) - 1;
                val += lstConfig[index].Name + ",";
            }
            val = StringUtil.RemoveLastStr(val);
            return val;
        }

        public static string TelOperatorFormat(this int inputValue)
        {
            string[] ArrData = { "", "Viettel", "Vinaphone", "Mobifone", "Vietnamobile" };
            if (inputValue <= 0 || inputValue > ArrData.Length)
                return string.Empty;

            return ArrData[inputValue];
        }

        public static string ConvertServiceID(this int inputValue)
        {
            string[] ArrData = { "Tất cả", "Halloween", "Tây du ký", "Thất truyền", "Pokemon", "Zombie", "Cao thấp", "Mini 777", "Tài xỉu","Minipoker",
            "Vòng quay", "Nạp thẻ", "Đổi thẻ", "Chuyển khoản", "Giftcode", "Thưởng event", "Rút két", "Nạp két"};

            if (inputValue < 0 || inputValue > ArrData.Length)
                return string.Empty;

            return ArrData[inputValue];
        }
    }
}