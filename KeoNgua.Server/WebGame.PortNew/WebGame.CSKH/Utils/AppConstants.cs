namespace MsWebGame.CSKH.Utils
{
    public class AppConstants
    {
        public static class ADMINROLES
        {
            public static readonly string ROLE_ADMIN = "ADMIN";
            public static readonly string ROLE_CALLCENTER = "CALLCENTER ";
        }
        public static class ADMINUSER
        {
            public const string USER_ADMIN = "admin";
            public const string USER_ADMINTEST = "admin_test";
            public const string USER_ADMINREF = "adminref";
            public const string USER_CSKH_01 = "cskh_01";
            public const string USER_CSKH_08 = "cskh_08";
            public const  string USER_CSKH_09 = "cskh_09";
            public const string USER_CSKH_02 = "cskh_02";
            public const string MONIOTR_01 = "monitor_01";
            
        }

        public static class CONFIG
        {
            public static readonly string EDIT = "Sửa ";
            public static readonly string Send = "Gửi ";
            public static readonly string ADD_NEW = "Thêm mới ";
            public static readonly string SEARCH = "Tìm kiếm ";
            public static readonly string DELETE = "Xóa ";
            public static readonly int GRID_SIZE = 20;
            public static readonly string BACK = "Quay lại danh sách ";
            public static readonly string SAVE = "Lưu ";
            public static readonly string TRANFER_MONEY = "Chuyển bit ";
            public static readonly string IMPORTEXCEL = "Nhập file excel ";
            public static readonly string CONFIRM = "Xác nhận";
            public static readonly string BTNTRANSUPDATE = "Cập nhật giao dịch";
        }

        public static class DBS
        {
            public static readonly int SUCCESS =1;
        }

        public static class STATUS
        {
            public static readonly int ACTIVE = 1;
            public static readonly int  DEACTIVE = 0;
        }

        public static class Transactions
        {
            public static class Status
            {
                public static readonly string PENDING = "Chờ xử lý";
                public static readonly string SUCCESS = "Đã xử lý";
                public static readonly string CANCEL = "Hủy giao dịch";
                public static readonly string APPROVED = "Chấp nhận giao dịch";
                public static readonly string COMPLAIN = "Khiếu nại";
            }
        }

        public static class Complain
        {
            public static class Status
            {
                public static readonly string NEW = "Khiếu nại chờ xử lý";
                public static readonly string PROCESSING = "Khiếu nại được tiếp nhận";
                public static readonly string SUCCESS = "Khiếu nại xử lý thành công";
            }
        }

        public enum NotifyType
        {
            Success,
            Error
        }
    }
}