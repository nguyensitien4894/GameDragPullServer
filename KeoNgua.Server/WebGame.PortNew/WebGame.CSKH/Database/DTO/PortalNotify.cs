using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class PortalNotify
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public int ServiceID { get; set; }
    }
}