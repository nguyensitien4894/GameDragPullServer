using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Models.Complains
{
    public class ComplainModel
    {
        public ComplainModel()
        {

            list = new List<ComplainProcessModel>();
        }
        [DisplayName("Loại tìm kiếm")]
        public int SearchType { get; set; }
        [DisplayName("Khiếu nại")]
        public long ComplainID { get; set; }
        [DisplayName("Loại khiếu nại")]
        public int ComplainType { get; set; }
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }
        [DisplayName("Nội dung")]
        public string Content { get; set; }
        [DisplayName("Tài Khoản")]
        public long UserID { get; set; }
        [DisplayName("Tài khoản")]
        public string UserName { get; set; }
        [DisplayName("Hình ảnh")]
        public string UserImage { get; set; }
        [DisplayName("Giải thích")]
        public string UserExplain { get; set; }
        [DisplayName("Kết quả khiếu nại")]
        public string UserProcessResult { get; set; }
        [DisplayName("Tài Khoản bị khiếu nại")]
        public long DefendantID { get; set; }
        [DisplayName("Tài Khoản bị khiếu nại")]
        public string DefendantName { get; set; }
        [DisplayName("Hình ảnh bị khiếu nại")]
        public string DefendantImage { get; set; }
        [DisplayName("Bị khiếu nại giải thích")]
        public string DefendantExplain { get; set; }
        [DisplayName("Kết quả bị khiếu nại")]
        public string DefendantProcessResult { get; set; }
        [DisplayName("Kết quả")]
        public string Result { get; set; }
        [DisplayName("Trạng thái khiếu nại")]
        public int ComplainStatus { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime CreateDate { get; set; }
        [DisplayName("Ngày cập nhật")]
        public DateTime UpdateDate { get; set; }
        [DisplayName("Giao Dịch")]
        public long TransID { get; set; }
        [DisplayName("Mã giao dịch")]
        public string TransCode { get; set; }
        [DisplayName("Ngày giao dịch")]
        public DateTime TransDate { get; set; }

        public int TranStatus { get; set; }
        [DisplayName("Trạng thái xử lý")]
        public int RequestStatus { get; set; }

        [DisplayName("Người bán")]
        public string CreateUserName { get; set; }
        public string CreatePhoneContact { get; set; }
        public string CreatePhoneOtp { get; set; }
        public int CreateUserTotalComplainCnt { get; set; }
        public int CreateUserTranBuyCnt { get; set; }
        public int CreateUserTranSellCnt { get; set; }
        public string CreateUserDisplayName { get; set; }
        [DisplayName("Người bán giải thích")]
        public string CreateExplain
        {
            get
            {
                if (UserID == CreateUserID)
                {
                    return UserExplain;
                }else if (UserID == DefendantID)
                {
                    return DefendantExplain;
                }else
                {
                    return String.Empty;
                }

            }
        }
        [DisplayName("Người mua giải thích")]
        public string ReceiveExplain
        {
            get
            {
                if (DefendantID == CreateUserID)
                {
                    return UserExplain;
                }
                else if (DefendantID == DefendantID)
                {
                    return DefendantExplain;
                }
                else
                {
                    return String.Empty;
                }

            }
        }

        [DisplayName("Người mua")]
        public string ReceiveUserName { get; set; }
        public string ReceiveUserDisplayName { get; set; }
        public string ReceivePhoneContact { get; set; }
        public string ReceivePhoneOtp { get; set; }
        public int ReceiverTotalComplainCnt { get; set; }
        public int ReceiverTranBuyCnt { get; set; }
        public int ReceiverTranSellCnt { get; set; }
        public long ReceiveUserID { get; set; }
        public long CreateUserID { get; set; }
        [DisplayName("Trạng thái khiếu nại")]
        public string ComplainStatusStr
        {
            get
            {
                switch (ComplainStatus)
                {
                    case 0:
                        return AppConstants.Complain.Status.NEW;
                    case 1:
                        return AppConstants.Complain.Status.PROCESSING;

                    case 5:
                        return AppConstants.Transactions.Status.SUCCESS;
                    default:
                        return string.Empty;
                };

            }
        }
        [DisplayName("Trạng thái giao dịch")]
        public string TransStatusStr
        {
            get
            {
                switch (TranStatus)
                {
                    case 0:
                        return AppConstants.Transactions.Status.PENDING;
                    case 1:
                        return AppConstants.Transactions.Status.SUCCESS;
                    case 2:
                        return AppConstants.Transactions.Status.COMPLAIN;
                    case 3:
                        return AppConstants.Transactions.Status.APPROVED;
                    case 4:
                        return AppConstants.Transactions.Status.CANCEL;
                    case 5:
                        return AppConstants.Transactions.Status.CANCEL;
                    default:
                        return string.Empty;
                };

            }
        }
        public int Active {get;set;}

      public   List<ComplainProcessModel> list { get; set; }

    }
}