using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserCardRecharge
    {
        public long UserCardRechargeID { get; set; }
        public long RequestID { get; set; }
        public long UserID { get; set; }
        public int TelOperatorID { get; set; }
        public int CardID { get; set; }
        public string CardNumber { get; set; }
        public string SerialNumber { get; set; }
        public int CardValue { get; set; }
        public string OperatorName { get; set; }
        public string OperatorCode { get; set; }
        public double Rate { get; set; }
        public long? ReceivedMoney { get; set; }
        public int? ValidAmount { get; set; }
        public int Status { get; set; }
        public string Username { get; set; }
        public string PartnerErrorCode { get; set; }
        public string PartnerMessage { get; set; }
        public string FeedbackMessage { get; set; }
        public long RefundCardValude { get; set; }
        public long RefundReceivedMoney { get; set; }
        public string DisplayName { get; set; }
        public int ServiceID { get; set; }
        public int BonusStatus { get; set; }
        public string PartnerMessageFormat
        {
            get
            {
                if (PartnerID == 1 || PartnerID == 4 || PartnerID == 6 || PartnerID == 8)
                {
                    if (PartnerErrorCode == "00")
                    {
                        return "Gửi yêu cầu Thành công";
                    }
                    else if (PartnerErrorCode == "01")
                    {
                        return "Hệ thống bận, vui lòng thử lại";
                    }
                    else if (PartnerErrorCode == "04")
                    {
                        return "Truy cập bị từ chối ";
                    }
                    else if (PartnerErrorCode == "05")
                    {
                        return "Sai mã NCC Game ";
                    }
                    else if (PartnerErrorCode == "06")
                    {
                        return "Sai mã Game";
                    }
                    else if (PartnerErrorCode == "07")
                    {
                        return "Sai AccessKey";
                    }
                    else if (PartnerErrorCode == "08")
                    {
                        return "Sai Chữ ký ";
                    }
                    else if (PartnerErrorCode == "14")
                    {
                        return "Thẻ đã tồn tại trong hệ thống ";
                    }
                    else if (PartnerErrorCode == "09")
                    {
                        return "Sai mã nhà mạng (Mã hợp lệ là: VTT, VNP, VMS) ";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }

                if (PartnerID == 2||PartnerID == 9)
                {



                    if (PartnerErrorCode == "0")
                    {
                        return "Thành công";
                    }
                    else if (PartnerErrorCode == "6")
                    {
                        return "Lỗi phía nhà mạng";
                    }
                    else if (PartnerErrorCode == " 9")
                    {
                        return "Sai chữ ký";
                    }

                    else if (PartnerErrorCode == "11")
                    {
                        return "Chưa khai báo kết nối gạch thẻ";
                    }

                    else if (PartnerErrorCode == "99")
                    {
                        return "Lỗi hệ thống";
                    }

                    else
                    {
                        return string.Empty;
                    }

                }
                if (PartnerID == 3|| PartnerID==7)
                {
                    return PartnerMessage;

                }
                if (PartnerID == 5|| PartnerID == 10 || PartnerID == 11)
                {
                    return PartnerMessage;
                }
                return "Đối tác khác ";


            }
        }
        public string FeedbackMessageFormat
        {
            get
            {

                if (PartnerID == 1 || PartnerID == 4 || PartnerID == 6 || PartnerID == 8)
                {
                    if (FeedbackErrorCode == "00")
                    {
                        return "Thành công";
                    }
                    else if (FeedbackErrorCode == "01")
                    {
                        return "Hệ thống bận, vui lòng thử lại";
                    }
                    else if (FeedbackErrorCode == "02")
                    {
                        return "Yêu cầu không được thực hiện, vui lòng cào thẻ ở nơi khác ";
                    }
                    else if (FeedbackErrorCode == "03")
                    {
                        return " Thông tin thẻ không đúng hoặc đã được sử dụng";
                    }
                    else if (FeedbackErrorCode == "04")
                    {
                        return " Truy cập bị từ chối";
                    }
                    else if (FeedbackErrorCode == "10")
                    {
                        return "Sai mệnh giá";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else if (PartnerID == 2|| PartnerID == 9)
                {

                    if (FeedbackErrorCode == "1")
                    {
                        return "Giao dịch thành công";
                    }
                    else if (FeedbackErrorCode == "-1")
                    {
                        return "Thẻ cào không hợp lệ hoặc đã sử dụng";
                    }
                    else if (FeedbackErrorCode == "0")
                    {
                        return "Giao dịch thất bại";
                    }

                    else if (FeedbackErrorCode == "2")
                    {
                        return "Mã số thẻ nạp không đúng";
                    }
                    else if (FeedbackErrorCode == "3")
                    {
                        return "Giao dịch bị từ chối";
                    }
                    else if (PartnerErrorCode == "99")
                    {
                        return "Lỗi hệ thống";
                    }



                }
                else if (PartnerID == 3|| PartnerID == 7)
                {
                    return FeedbackMessage;
                }
                else if (PartnerID == 5|| PartnerID == 10 || PartnerID == 11)
                {
                    return FeedbackMessage;
                }
                return "Đối tác khác ";


            }
        }

        public string FeedbackErrorCode { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int PartnerID { get; set; }
        public double? TeleRate { get; set; }
        public string PartnerStr
        {
            get
            {
                if (PartnerID == 1)
                {
                    return "P1-Happy";
                };
                if (PartnerID == 2)
                {
                    return "P2-Mobile Sms";
                };
                if (PartnerID == 3)
                {
                    return "P3-FcconnClub";
                };
                if (PartnerID == 4)
                {
                    return "P4-Smile";
                };
                if (PartnerID == 5)
                {
                    return "P5-Thenhanh";
                };
                if (PartnerID == 7)
                {
                    return "Gate_2_P7-FcconnClub";
                };
                if (PartnerID == 6)
                {
                    return "Gate_2_P6-Funny";
                };
                if (PartnerID == 8)
                {
                    return "Gate_3_P8-Funny";
                };
                if (PartnerID == 9)
                {
                    return "Gate_3_P9-Mobile Sms";
                };
                if (PartnerID == 10)
                {
                    return "Gate_3_P10-Thenhanh";
                };
                if (PartnerID == 11)
                {
                    return "Gate_2_P11-Thenhanh";
                };
                return string.Empty;
            }
        }


        public string ColorStr
        {
            get
            {
                if (Status == 0)
                {
                    return "label label-warning";
                }
                else if (Status == -1)
                {
                    return "label label-grey";
                }
                else if (Status == -2)
                {
                    return "label label-grey";
                }
                else if (Status == 2)
                {
                    return "label label-success";
                }
                else if (Status == 1)
                {
                    return "label label-important";
                }
                else if (Status == 3)
                {
                    return "label label-important";
                }
                else if (Status == 4)
                {
                    return "label label-success";
                }
                else
                {
                    return "label label-grey";
                }
            }
        }



        public string StatusStr
        {
            get
            {
                if (Status == 0)
                {
                    return "Chờ xử lý";
                }
                else if (Status == -1)
                {
                    return "Nạp thất bại";
                }
                else if (Status == -2)
                {
                    return "Gửi yêu cầu thất bại";
                }
                else if (Status == 2)
                {
                    return "Nạp thành công";
                }
                else if (Status == 1)
                {
                    return "Đối soát bên thứ 3";
                }
                else if (Status == 3)
                {
                    return "Cộng tiền không thành công";
                }
                else if (Status == 4)
                {
                    return "Đã  hoàn tiền bởi admin";
                }
                else if (Status == -3)
                {
                    return "Thất bại-SMG . Ko xử lý";
                }
                else
                {
                    return "Nạp thất bại";
                }
            }
        }
    }
}