using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Games
{
    public class GameIndexListModel
    {
        [DisplayName("Chọn ngày")]
        public DateTime ?searchDate { get; set; }
    }
}