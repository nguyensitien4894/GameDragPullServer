using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MsWebGame.CSKH.Models.HistoryTranfers
{
    public class ListHistoryTransferModel
    {
        [DisplayName("Từ ngày")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }
        [DisplayName("Đến ngày")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }
        [DisplayName("Nickname nhận")]
        public string ReceiverName { get; set; }
        public int ServiceID { get; set; }
        public int TransType { get; set; }
    }
}