using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.Ccu
{
    public class CuuListModel
    {
        public TimeSpan Datetime { set; get; }
        public int Android { set; get; } = 0;
        public int Ios { set; get; } = 0;
        public int Web { set; get; } = 0;
        
    }
}