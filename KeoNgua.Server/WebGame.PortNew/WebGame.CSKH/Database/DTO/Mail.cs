using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Mail
    {
        public long ID { get; set; }
        public long Sender { get; set; }
        public long Reciever { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string RecevierUserName { get; set; }
    }
}