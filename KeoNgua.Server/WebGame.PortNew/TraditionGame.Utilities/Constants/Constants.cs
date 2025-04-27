namespace MsTraditionGame.Utilities.Constants
{
    public class Constants
    {
        public static string MAIL_KEY = "b2bca895371900c9067a08621240a008";
        public static string GO_URL = "http://www.go.vn/";

        public static string SERVER_VTC_ADDR = "http://localhost:8084/";//"http://sandbox.vtcgame.vn/";

        public static string SSO_URL_CONTINUE = "http://localhost:5706/login/default.aspx";

        public static string SERVICE_PROVIDER_SITE_ID = "100000";

        public static string SessionErrorMessage = "Phiên đăng nhập của bạn đã hết hạn. Xin vui lòng đăng nhập lại!";
        public static string GameSessionErrorMessage = "Không tìm thấy bàn chơi. Xin vui lòng thử lại!";
        public static string RoomErrorMessage = "Bàn chơi chưa sẵn sàng. Mời bạn chọn bàn khác!";
        public static string BlockAccountErrorMessage = "Tài khoản của bạn tạm thời bị khóa! Vui lòng quay trở lại sau {0} phút";
        public static string SessionEndErrorMessage = "Phiên chơi {0} đã kết thúc, mời bạn tham gia vào bàn khác";
        public static string ChatErrorMessage = "Bạn chat quá nhanh! ";
        public static string SystemErrorMessage = "Hệ thống bận.Xin vui lòng thử lại!";
        public static string NotFoundRoomErrorMessage = "Không có bàn chơi nào phù hợp, mời bạn thử lại!";
        public static string FastEnterLobbyErrorMessage = "Bạn ra vào phòng quá nhanh! Vui lòng thử lại!";
        public static string MultipleDeviceMessage = "Tài khoản của bạn đang chơi game ở thiết bị khác!";
        public static string SessionNotFound = "Phiên chơi đã kết thúc. Xin vui lòng thử lại!";
        public static string NotInTurn = "Chưa đến lượt chơi của bạn. Vui lòng chờ!";
        public static string BuyChipNotFound = "Nạp chip không thành công. Xin vui lòng thử lại!";

        //Mật khẩu mới không hợp lệ
        public const int NEW_PASSWORD_ERROR_CODE = -2512;
        public const string NEW_PASSWORD_ERROR_MESSAGE = "Mật khẩu mới không hợp lệ. Vui lòng thử lại!";

        public const int ODP_EXPRIED_CODE = -2513;
        public const string ODP_EXPRIED_MESSAGE = "Bạn đã nhập sai quá số lần cho phép!";

        //Tên tài khoản không hợp lệ
        public const int ACCOUNTNAME_INPUT_ERROR_CODE = -101;
        public const string ACCOUNTNAME_INPUT_ERROR_MESSAGE = "Tên tài khoản không hợp lệ. Vui lòng thử lại!";

        //Mật khẩu không hợp lệ
        public const int PASSWORD_INPUT_ERROR_CODE = -102;
        public const string PASSWORD_INPUT_ERROR_MESSAGE = "Mật khẩu không hợp lệ. Vui lòng thử lại!";

        public const string DEVICETYPE = "devicetype";

        public const string DEVICEID = "deviceid";
        public const string USERNAME = "username";
        public const string TOKEN = "token";
        public const string LOGINTYPE = "logintype";
        public const string CAPTCHA = "captcha";
        public const string VERIFICAPTCHA = "verifycaptcha";

        public const int UNAUTHORIZED = -2;
        
        public enum enmDeviceType
        {
            Web = 1, IOS = 2, Android = 3, WindowPhone = 4
        }

        public enum enmAuthenType
        {
            AUTHEN_ID = 1,
            AUTHEN_FB = 2,
            AUTHEN_DEVICEID = 3,
            REGISTER_VTCID = 4,
            LINK_EXISTS_VTCID = 5,
            LINK_CREATE_VTCID = 6
        }

        public enum Paging
        {
            RECORD_PER_PAGE = 10
        }
    }
}