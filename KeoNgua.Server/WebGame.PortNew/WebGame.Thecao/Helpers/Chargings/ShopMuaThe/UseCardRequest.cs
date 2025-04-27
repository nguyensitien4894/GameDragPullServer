using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsWebGame.Thecao.ShopMuaThe
{
   public  class UseCardRequest
    {
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public string CardType { get; set; }
        public string AccountName { get; set; }
        public string AppCode { get; set; }
        public string RefCode { get; set; }
        public int AmountUser { get; set; }
        public string CallbackUrl { get; set; }

    }
}
