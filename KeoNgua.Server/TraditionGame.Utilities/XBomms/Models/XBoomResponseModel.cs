using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.XBomms.Models
{
   public  class XBoomResponseModel
    {
        public XBoomResponseModel()
        {
            data = new DataModel();
        }

        public int code { get; set; }
        public string message { get; set; }
        public DataModel data { get; set; }
    }

    public class DataModel
    {
        public string id { get; set; }
        public string status { get; set; }
    }
}
