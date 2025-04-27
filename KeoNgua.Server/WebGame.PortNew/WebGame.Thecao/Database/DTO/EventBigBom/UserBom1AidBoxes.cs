using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO.EventBigBom
{
    public class UserBom1AidBoxes
    {

        public long ID { get; set; }

        public long UserID { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public int B1P { get; set; }

        public int DegreeID { get; set; }

        public int? GiftID { get; set; }

        public int? GiftType { get; set; }

        public int? GiftValue { get; set; }

        public int? TimeFrameID { get; set; }

        public bool Status { get; set; }

        public DateTime? AidDate { get; set; }

        public DateTime? ReceiveDate { get; set; }

        public string Description { get; set; }
    }
}