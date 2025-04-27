namespace TraditionGame.Utilities.Constants
{
    public class Constants
    {
        public static string MAIL_KEY = "b2bca895371900c9067a08621240a008";
        public static string GO_URL = "http://www.go.vn/";

        public static string SERVER_VTC_ADDR = "http://localhost:8084/";//"http://sandbox.vtcgame.vn/";

        public static string SSO_URL_CONTINUE = "http://localhost:5706/login/default.aspx";

        public static string SERVICE_PROVIDER_SITE_ID = "100000";

        public static string SessionErrorMessage = "Your login session has expired. Please log in again!";
        public static string GameSessionErrorMessage = "Table not found. Please try again!";
        public static string RoomErrorMessage = "The table is not ready. Please choose another table!";
        public static string BlockAccountErrorMessage = "Your account is temporarily locked! Please come back in {0} minutes";
        public static string SessionEndErrorMessage = "Session {0} has ended, please join another table";
        public static string ChatErrorMessage = "You chat too fast!";
        public static string SystemErrorMessage = "The system is busy. Please try again!";
        public static string NotFoundRoomErrorMessage = "No matching tables, please try again!";
        public static string FastEnterLobbyErrorMessage = "You go in and out of the room too fast! Please try again!";
        public static string MultipleDeviceMessage = "Your account is playing games on another device!";
        public static string SessionNotFound = "The game session has ended. Please try again!";
        public static string NotInTurn = "It's not your turn yet. Please wait!";
        public static string BuyChipNotFound = "Chip load failed. Please try again!";

        //Mật khẩu mới không hợp lệ
        public const int NEW_PASSWORD_ERROR_CODE = -2512;
        public const string NEW_PASSWORD_ERROR_MESSAGE = "The new password is not valid. Please try again!";

        public const int ODP_EXPRIED_CODE = -2513;
        public const string ODP_EXPRIED_MESSAGE = "You have entered the wrong input more than the allowed number of times!";

        //Tên tài khoản không hợp lệ
        public const int ACCOUNTNAME_INPUT_ERROR_CODE = -101;
        public const string ACCOUNTNAME_INPUT_ERROR_MESSAGE = "Invalid account name. Please try again!";

        //Mật khẩu không hợp lệ
        public const int PASSWORD_INPUT_ERROR_CODE = -102;
        public const string PASSWORD_INPUT_ERROR_MESSAGE = "Invalid password. Please try again!";

        public const string DEVICETYPE = "devicetype";

        public const string DEVICEID = "deviceid";
        public const string USERNAME = "username";
        public const string TOKEN = "token";
        public const string LOGINTYPE = "logintype";
        public const string CAPTCHA = "captcha";
        public const string VERIFICAPTCHA = "verifycaptcha";
        public const int MAINTAIN = -1010101;

        public const int UNAUTHORIZED = -1001;
        public const int COMMON_ERR = -1005;
        public const int EXCEPTION_ERR = -99;
        public const int NOT_IN_SERVICE = -1009;
        public const int OtherDevice = -98;
        public const int SUCCESS = 1;
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
            LINK_CREATE_VTCID = 6,
            LIVE69 = 7
        }

        public enum Paging
        {
            RECORD_PER_PAGE = 10
        }
    }
}