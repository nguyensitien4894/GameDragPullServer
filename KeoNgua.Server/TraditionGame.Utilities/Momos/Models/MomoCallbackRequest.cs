using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TraditionGame.Utilities.Momos.Models
{
    public class MomoCallbackRequest
    {
        public long amount { get; set; }
        public string method { get; set; }
        public string content { get; set; }
        public string hash { get; set; }
        public string transaction_id { get; set; }
        public int status { get; set; }
        public string phone { get; set; }
    }
}
