using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Database.DTO
{
    public class CuuListModel
    {
        public string Time { set; get; }
        public int Android { set; get; } = 0;
        public int Ios { set; get; } = 0;
        public int Web { set; get; } = 0;
        public int Total { set; get; } = 0;

    }
}