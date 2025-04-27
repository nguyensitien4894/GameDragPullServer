using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsWebGame.Portal.Helpers.Chargings.NewBoc
{
   public  class UseCardRequest
    {
        public string CardSeri { get; set; }
        public string CardCode { get; set; }
        public string CardType { get; set; }
        public string AccountName { get; set; }
        //public string AppCode { get; set; }
        public string TransID { get; set; }
        public int Amount { get; set; }
        public string CallbackUrl { get; set; }
        public string Signature { get; set; }
        public string ApiToken { get; set; }

    }
}
