using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.BanksGateTheNhanh
{
    public class UserRequest

    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature
        {
            get; set;
        }
    }
}
