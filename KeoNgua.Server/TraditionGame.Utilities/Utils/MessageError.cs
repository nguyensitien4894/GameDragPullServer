using System;
using System.Collections.Generic;

namespace TraditionGame.Utilities.Utils
{
    public class MessageError
    {
        private static readonly Lazy<MessageError> _instance = new Lazy<MessageError>(() => new MessageError());

        private readonly Dictionary<int, string> Data_Array = new Dictionary<int, string>();

        public static MessageError Instance
        {
            get { return _instance.Value; }
        }

        private MessageError()
        {
            Data_Array.Add(1, "Thành công!");
            Data_Array.Add(-10, "Tham số đầu vào không hợp lệ!");
            Data_Array.Add(-11, "Line không hợp lệ!");
            Data_Array.Add(-12, "Room không tồn tại!");
            Data_Array.Add(-13, "Game không hợp lệ!");
            Data_Array.Add(-15, "Bước chơi free không hợp lệ!");
            Data_Array.Add(-17, "Đã tồn tại chiến dịch");
            Data_Array.Add(-19, "Đã tồn tại giftcode");
            Data_Array.Add(-21, "Hiện vật không hợp lệ");
            Data_Array.Add(-22, "Giá không hợp lệ");
            Data_Array.Add(-23, "Số lượng không hợp lệ");
            Data_Array.Add(-24, "Nhà mạng không hợp lệ");
            Data_Array.Add(-25, "Nhà mạng đã tồn tại");
            Data_Array.Add(-26, "Thẻ cào đã tồn tại");
            Data_Array.Add(-27, "Thẻ không hợp lệ");
            Data_Array.Add(-28, "Thẻ cào đã được nạp");
            Data_Array.Add(-29, "User đã nhận loại giftcode này");
            Data_Array.Add(-30, "Thời gian bị chồng chập");
            Data_Array.Add(-31, "Token khởi tạo không thành công");
            Data_Array.Add(-32, "Cập nhật trạng thái thẻ không thành công");
            Data_Array.Add(-33, "Vượt quá cấu hình cho phép");
            Data_Array.Add(-34, "Vượt quá hạn mức đổi thẻ trong ngày");
            Data_Array.Add(-35, "Không tìm thấy thông tin đổi thẻ");
            Data_Array.Add(-36, "Không phải user đổi thẻ");
            Data_Array.Add(-99, "Rất tiếc hệ thống của chúng tôi đang bận! Mời bạn thử lại sau!");
            Data_Array.Add(-101, "Không thể thêm mới bản ghi tài khoản!");
            Data_Array.Add(-102, "Tài khoản đã tồn tại!");
            Data_Array.Add(-103, "Tài khoản chưa tồn tại!");
            Data_Array.Add(-104, "Tài khoản không thể xóa!");
            Data_Array.Add(-105, "Tài khoản không hợp lệ!");
            Data_Array.Add(-106, "Tài khoản không có quyền khóa!");
            Data_Array.Add(-107, "Tài khoản không thể cập nhật!");
            Data_Array.Add(-108, "Tên tài khoản đã tồn tại!");
            Data_Array.Add(-201, "Tài khoản người dùng không tồn tại!");
            Data_Array.Add(-202, "Tài khoản không hợp lệ!");
            Data_Array.Add(-203, "Số điện thoại không tồn tại!");
            Data_Array.Add(-204, "User không tồn tại!");
            Data_Array.Add(-206, "Không thể tạo mới user!");
            Data_Array.Add(-207, "Mã hoặc điện thoại user bị trùng lặp!");
            Data_Array.Add(-208, "Ví user không hợp lệ!");
            Data_Array.Add(-209, "Không thể cập nhập thông tin user!");
            Data_Array.Add(-210, "Bình luận không hợp lệ!");
            Data_Array.Add(-211, "Không tìm thấy user!");
            Data_Array.Add(-212, "Trạng thái không hợp lệ!");
            Data_Array.Add(-213, "User không thể từ chối!");
            Data_Array.Add(-214, "User không thể hủy!");
            Data_Array.Add(-215, "User không có quyền!");
            Data_Array.Add(-216, "User bị khóa");
            Data_Array.Add(-217, "User không thể phê duyệt!");
            Data_Array.Add(-219, "Loại user không hợp lệ!");
            Data_Array.Add(-222, "User đã hoạt động!");
            Data_Array.Add(-223, "User chưa được xếp hạng!");
            Data_Array.Add(-224, "User đã nhận thưởng với hạng này!");
            Data_Array.Add(-225, "User đã tồn tại!");
            Data_Array.Add(-226, "User đã bị khóa!");
            Data_Array.Add(-301, "Admin không tồn tại!");
            Data_Array.Add(-302, "Đại lý không tồn tại!");
            Data_Array.Add(-303, "Đại lý bị hủy!");
            Data_Array.Add(-304, "Lỗi khi thêm mới đại lý!");
            Data_Array.Add(-305, "Đại lý bị khóa!");
            Data_Array.Add(-306, "Trạng thái đại lý không hợp lệ!");
            Data_Array.Add(-307, "Đại lý không tồn tại trong Market!");
            Data_Array.Add(-308, "Cấp đại lý không hợp lệ!");
            Data_Array.Add(-309, "Đại lý bị khóa hoặc hủy!");
            Data_Array.Add(-310, "Lỗi khi hủy đại lý!");
            Data_Array.Add(-311, "Đại lý đang tồn tại giao dịch!");
            Data_Array.Add(-312, "Lỗi khi cập nhật thông tin đại lý!");
            Data_Array.Add(-313, "Trạng thái khóa không được cấu hình!");
            Data_Array.Add(-314, "Đại lý còn giao dịch treo!");
            Data_Array.Add(-315, "Ví đại lý không hợp lệ!");
            Data_Array.Add(-317, "Cấp đại lý không được chuyển tiền!");
            Data_Array.Add(-318, "Đại lý không tạo giao dịch này!");
            Data_Array.Add(-319, "Trạng thái OTP của đại lý đã trùng cài đặt trước!");
            Data_Array.Add(-320, "Phí đại lý chưa được cấu hình!");
            Data_Array.Add(-501, "Khoản tiền không hợp lệ!");
            Data_Array.Add(-502, "Ví không hợp lệ!");
            Data_Array.Add(-503, "Khoản tiền không đủ!");
            Data_Array.Add(-504, "Số dư của bạn không đủ");
            Data_Array.Add(-505, "Log ví không hợp lệ!");
            Data_Array.Add(-506, "Khoản tiền nhỏ nhất!");
            Data_Array.Add(-507, "Giao dịch không thể tạo!");
            Data_Array.Add(-508, "Giao dịch không hợp lệ!");
            Data_Array.Add(-509, "Giao dịch đã tồn tại!");
            Data_Array.Add(-510, "Giao dịch không thể cập nhật!");
            Data_Array.Add(-511, "Giao dịch không tìm thấy!");
            Data_Array.Add(-512, "Trạng thái giao dịch không hợp lệ!");
            Data_Array.Add(-515, "Log không thể tạo!");
            Data_Array.Add(-516, "Lỗi lưu lịch sử!");
            Data_Array.Add(-517, "Hạng không tồn tại!");
            Data_Array.Add(-521, "Đã tồn tại giao dịch nhập!");
        }

        public string GetMessageError(int key)
        {
            string msg = string.Empty;
            Data_Array.TryGetValue(key, out msg);
            return msg;
        }
    }
}