using System;

namespace MsWebGame.CSKH.Models.Param
{
    public class ParsTransactionHistory
    {
        public int ObjectType { get; set; }
        public string ObjectValue { get; set; }
        public int SearchType { get; set; }
        public int PartnerType { get; set; }
        public string TransCode { get; set; }
        public int ServiceID { get; set; }
        public string PartnerName { get; set; }
        public int TransType { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }








    }

    public class ParsBangXepHang
    {
        public int Type { get; set; } // 1 : thang, 2 : thua, 3 vip

    }

    public class ParsLookup
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int searchType { get; set; }
        public string value { get; set; }
        public int rankId { get; set; }
        public int serviceId { get; set; }
    }

    public class ParsAdminTrans
    {
        public string nicknameAdmin { get; set; }
        public int? transferType { get; set; } //1:OUT;0:IN
        public int partnerType { get; set; }  //0:ALL;1:User;2:Agency;3:Admin
        public string partnerName { get; set; }
        public int reasonId { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int serviceId { get; set; }
    }

    public class ParsSendMailToUser
    {
        public long receiverId { get; set; }
        public string receiverName { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int status { get; set; }
        public DateTime createdTime { get; set; }
    }

    public class ParsAdminInfo
    {
        public int roleId { get; set; }
        public string nickName { get; set; }
        public string phoneNo { get; set; }
    }

    public class ParsTransfer
    {
        public string receiverName { get; set; }
        public string amount { get; set; }
        public double transfee { get; set; }
        public string orgAmount { get; set; }
        public string note { get; set; }
        public int walletType { get; set; }
        public long balance { get; set; }
        public int ServiceID { get; set; }
    }



    public class ParsChangePassword
    {
        public string passwordOld { get; set; }
        public string passwordNew { get; set; }
        public string rePasswordNew { get; set; }
    }

    public class ParsTransfigureAgency
    {
        public long userId { get; set; }
        public string nickName { get; set; }
        public string fullName { get; set; }
        public string areaName { get; set; }
        public string phoneDisplay { get; set; }
        public string fbLink { get; set; }
        public string teleLink { get; set; }
        public string zaloLink { get; set; }
        public int orderNum { get; set; }
        public int serviceId { get; set; }
    }

    public class ParsPhatLocTransfer
    {
        public long phatLocId { get; set; }
        public string phatLocName { get; set; }
        public long receiverId { get; set; }
        public string receiverName { get; set; }
        public string receiverNameList { get; set; }
        public string amount { get; set; }
        public long amountNum { get; set; }
        public string note { get; set; }
        public int serviceId { get; set; }
    }

    public class ParsNormalTransfer
    {
        public string username { get; set; }
        public string password { get; set; }
        public long receiverId { get; set; }
        public string receiverName { get; set; }
        public string receiverNameList { get; set; }
        public string amount { get; set; }
        public string orgAmount { get; set; }
        public double fee { get; set; }
        public string note { get; set; }
        public int serviceId { get; set; }
    }
}