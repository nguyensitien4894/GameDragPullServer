using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Helpers.Chargings.ShopMuaThe
{
    public class CallBackRequest
    {

    public string  RefCode { get; set; }
    public int  Status { get; set; }
    public int Amount { get; set; }
    public string Signature { get; set; }
    }
}