using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;


namespace MsWebGame.Portal.Models
{
    public class ReturnData
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string Extend { get; set; }


        #region Trả về dữ liệu

        /// <summary>
        /// Tổng hợp dữ liệu trả về client
        /// </summary>
        /// <param name="responseCode">ResponseCode trả về</param>
        /// <returns></returns>
        public static ReturnData GetReturnData(int responseCode)
        {
            return GetReturnData(responseCode, string.Empty);
        }


        /// <summary>
        /// Tổng hợp dữ liệu trả về client
        /// </summary>
        /// <param name="responseCode">ResponseCode trả về</param>
        /// <param name="extend">Option mở rộng</param>
        /// <returns></returns>
        public static ReturnData GetReturnData(int responseCode, string extend)
        {
            try
            {
                var returnData = new ReturnData { ResponseCode = responseCode, Extend = extend };
                if (responseCode > 0)
                {
                    returnData.Description = "Bạn đã giao dịch thành công.";
                    return returnData;
                }
                switch (responseCode)
                {
                    #region Danh sách mã lỗi chung -80xx
                    case -7000: // Chưa login
                        returnData.Description = "Bạn vui lòng đăng nhập hệ thống để sử dụng dịch vụ";
                        break;
                    case -7001: // Không đủ số dư
                        returnData.Description = "Tài khoản không đủ số dư để thực hiện giao dịch";
                        returnData.Extend = extend;
                        break;

                    case -7004: // Có sử dụng dịch vụ bảo mật ODP, OTP
                        returnData.Description = "Có sử dụng dịch vụ bảo mật ODP, OTP";
                        break;
                    case -7005: // Mã odp, otp ko hợp lệ
                        returnData.Description = "Mã bảo mật không hợp lệ";
                        break;
                    case -7006: // Mã  otp hết hạn cần reload lại vị trí mới
                        returnData.Description = "Mã OTP đã hết hạn, bạn vui lòng nhập lại theo vị trí mới";
                        break;
                    case -7007: // Số dư trong tài khoản của bạn không đủ để thực hiện thao tác này
                        returnData.Description = "Số dư trong tài khoản của bạn không đủ để thực hiện thao tác này";
                        break;
                    case -7008: // Bạn đã nhận mã ODP quá 3 lần
                        returnData.Description = "Bạn đã nhận mã ODP quá 3 lần";
                        break;
                    case -7009: // ODP đang được gửi, bạn vui lòng chờ
                        returnData.Description = "ODP đang được gửi, bạn vui lòng chờ";
                        break;
                    case -7010:
                        returnData.Description = "Mã xác thực ODP đã được gửi lại vào số điện thoại của bạn. Vui lòng kiểm tra lại.";
                        break;
                    case -7011:
                        returnData.Description = string.Format("Mã xác thực ODP đã được gửi lại vào email {0} và số điện thoại của bạn.", extend);
                        break;
                    case -7012: //chưa đăng nhập thì redirect sang cổng thanh toán
                        returnData.Description = "Chưa đăng nhập thì redirect sang cổng thanh toán";
                        break;


                    case -7014: //Số di động không hợp lệ
                        returnData.Description = "Số di động không hợp lệ";
                        break;
                    case -7015: //Gửi mã thẻ không thành công
                        returnData.Description = "Gửi mã thẻ không thành công";
                        break;
                    case -8999: // Exception
                        returnData.Description = "Hệ thống đang bận, bạn vui lòng quay lại sau";
                        break;
                    #endregion Danh sách mã lỗi chung 70xx

                    #region Danh sách thông báo lỗi Đăng ký tài khoản -81xx
                    case -8100:
                        returnData.Description = "Vui lòng nhập số tài khoản";
                        break;
                    case -8101:
                        returnData.Description = "Tài khoản không hợp lệ";
                        break;
                    case -8102:
                        returnData.Description = "Vui lòng nhập mật khẩu";
                        break;
                    case -8103:
                        returnData.Description = "Mật khẩu từ 6-18 ký tự bao gồm cả chữ cái và chữ số";
                        break;
                    case -8108:
                        returnData.Description = "Vui lòng nhập lại MK";
                        break;
                    case -8104:
                        returnData.Description = "Mật khẩu nhập lại không đúng";
                        break;
                    case -8105:
                        returnData.Description = "Vui lòng nhập địa chỉ email";
                        break;
                    case -8106:
                        returnData.Description = "Email không hợp lệ";
                        break;
                    case -8107:
                        returnData.Description = "Email đã được sử dụng để đăng ký trên hệ thống";
                        break;
                    case -8109:
                        returnData.Description = "Vui lòng nhập họ và tên";
                        break;
                    case -8110:
                        returnData.Description = "Vui lòng nhập số CMTND, Hộ chiếu";
                        break;
                    case -8111:
                        returnData.Description = "Vui lòng nhập mã xác thực";
                        break;
                    case -8112:
                        returnData.Description = "Vui lòng nhập tên doanh nghiệp";
                        break;
                    case -8113:
                        returnData.Description = "Vui lòng nhập số giấy ĐKKD";
                        break;
                    case -8114:
                        returnData.Description = "Vui lòng nhập mã số thuế";
                        break;
                    case -8115:
                        returnData.Description = "Vui lòng nhập địa chỉ trụ sở chính";
                        break;
                    case -8116:
                        returnData.Description = "Vui lòng nhập điện thoại liên hệ";
                        break;
                    case -8117:
                        returnData.Description = "Vui lòng chọn tình thành phố";
                        break;
                    case -8118:
                        returnData.Description = "Vui lòng chọn quận huyện";
                        break;
                    case -8119:
                        returnData.Description = "Vui lòng nhập địa chỉ đăng ký kinh doanh";
                        break;
                    case -8120:
                        returnData.Description = "Tài khoản đã tồn tại";
                        break;
                    case -8121:
                        returnData.Description = "Tài khoản đã tồn tại trên hệ thống, nhưng chưa được kích hoạt";
                        break;
                    case -8122:
                        returnData.Description = "Mã kích hoạt không hợp lệ";
                        break;
                    case -8123:
                        returnData.Description = "Tài khoản kích hợp không tồn tại";
                        break;
                    case -8124:
                        returnData.Description = "Mã xác thực không hợp lệ";
                        break;
                    case -8125:
                        returnData.Description = "Hiện tại VTC Pay không hỗ trợ đăng ký nhanh cho các số điện thoại không thuộc các mạng Viettel, Vinaphone, Mobifone, Vietnamobile. Để tiếp tục đăng ký vui lòng click <a href=\"https://pay.vtc.vn/#dang-ky-440\" target=\"_blank\" >tại đây</a>";
                        break;
                    #endregion Danh sách thông báo lỗi mua mã thẻ 71xx

                    #region Danh sách thông báo lỗi Nạp tiền -82xx
                    case -8201:
                        returnData.Description = "Bạn cần nhập tối thiểu 10,000 VNĐ";
                        break;

                    case -8205:
                        returnData.Description = "Ngân hàng không tồn tại";
                        break;

                    case -8206:
                        returnData.Description = "Mã kiểm tra không hợp lệ";
                        break;

                    case -8207:
                        returnData.Description = "Mã kiểm tra đã hết hạn. Vui lòng đổi mã kiểm tra.";
                        break;

                    #endregion Danh sách thông báo lỗi Nạp tiền -82xx

                    #region Danh sách thông báo lỗi nạp điện thoại trả trước, trả sau, dt cố định, nạp nhiều số 72xx
                    case -7200: // số điện thoại không hợp lệ
                        returnData.Description = "Số điện thoại không hợp lệ";
                        break;
                    case -7201: // Mệnh giá không hợp lệ
                        returnData.Description = "Mệnh giá không hợp lệ";
                        break;
                    case -7202: // Giao dịch không thành công
                        returnData.Description = "Giao dịch không thành công. Vui lòng quay lại sau.";
                        break;
                    case -7203: // Số tiền nạp phải từ 6000 đến 1 triệu (trả sau)
                        returnData.Description = "Số tiền nạp phải từ 6.000 đến 1 triệu";
                        break;
                    case -7204: // Số tiền nạp phải chẵn nghìn (trả sau)
                        returnData.Description = "Số tiền nạp phải chẵn nghìn";
                        break;
                    case -7205: // Số tiền thanh toán phải từ 10.000 và chẵn nghìn (điện thoại cố định)
                        returnData.Description = "Số tiền thanh toán phải từ 10.000 và chẵn nghìn";
                        break;
                    case -7206: // Cần chọn file danh sách điện thoại để thanh toán (nạp nhiều số)
                        returnData.Description = "Cần chọn file danh sách điện thoại để thanh toán";
                        break;
                    case -7207: // Danh sách số điện thoại không vượt quá 50 số (nạp nhiều số)
                        returnData.Description = "Danh sách số điện thoại không vượt quá 50 số";
                        break;
                    #endregion Danh sách thông báo lỗi nạp điện thoại trả trước, trả sau, dt cố định, nạp nhiều số 72xx

                    #region Danh sách thông báo lỗi nạp Partner 74xx
                    case -7400: // chưa nhập tài khoản partner cần nạp
                        returnData.Description = "Vui lòng nhập tài khoản cần nạp";
                        break;
                    case -7401: // tài khoản không hợp lệ
                        returnData.Description = "Tài khoản nạp không hợp lệ";
                        break;
                    case 7401: // tài khoản  hợp lệ
                        returnData.Description = "Tài khoản nạp hợp lệ";
                        break;
                    #endregion Danh sách thông báo lỗi nạp Partner 74xx

                    #region Danh sách thông báo lỗi Gia hạn truyền hình VTC 75xx
                    case -7500: // Mã dịch vụ không hợp lệ
                        returnData.Description = "Mã dịch vụ không hợp lệ";
                        break;
                    case -7501: // Mã IC không hợp lệ
                        returnData.Description = "Mã IC không hợp lệ";
                        break;
                    case -7502: //Lỗi ngoài khoảng giá trị gói
                        returnData.Description = "Lỗi ngoài khoảng giá trị gói";
                        break;
                    case -7503: // Loại thẻ gia hạn không hợp đầu thu
                        returnData.Description = "Loại thẻ gia hạn không hợp đầu thu";
                        break;
                    case -7504: // Sai gói kênh
                        returnData.Description = "Sai gói kênh";
                        break;
                    case -7505: // Mã sản phẩm không đúng
                        returnData.Description = "Mã sản phẩm không đúng";
                        break;
                    case -7506: // Giao dịch đang xử lý
                        returnData.Description = "Giao dịch đang xử lý";
                        break;
                    case -7507: //Số lượng hàng không đủ
                        returnData.Description = "Số lượng hàng không đủ";
                        break;
                    case -7508: // Lỗi trong quá trình tạo giao dịch
                        returnData.Description = "Lỗi trong quá trình tạo giao dịch";
                        break;
                    case -7509: // Lỗi trong quá trình gọi service topup
                        returnData.Description = "Lỗi trong quá trình gọi service topup";
                        break;
                    case -7510: // Giao dịch nghi vấn
                        returnData.Description = "Giao dịch nghi vấn";
                        break;
                    #endregion Danh sách thông báo lỗi Gia hạn truyền hình VTC 75xx


                    default:
                        //NLogLogger.LogInfo("Mã lỗi chưa định nghĩa:" + responseCode);
                        return GetReturnData(-8999, string.Empty);
                }

                return returnData;
            }
            catch (Exception exception)
            {
                NLogManager.PublishException(exception);
                return GetReturnData(-8999, string.Empty);
            }
        }

        #endregion Trả về dữ liệu
    }
}