using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models
{
    public class ParConfigLive
    {
        public long UserId { get; set; }
        public List<string> Tranfer { get; set; } = new List<string>();
        public List<string> Game { get; set; } = new List<string>();
    }
}