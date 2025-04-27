using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;

namespace MsWebGame.Portal.Helpers.Chargings.HaDongPho
{

    public class HaDongPhoResponse
    {
        public string errorcode { get; set; }
        public string description { get; set; }
        public string requestid { get; set; }
    }


    //public class HaDongPhoResponse
    //{

    //    public HaDongPhoResponse()
    //    {
    //        data = new HaDongPhoDataResponse();
    //    }
    //    public int status { get; set; }
    //    public string message { get; set; }
    //    public HaDongPhoDataResponse data { get; set; }
    //    public string requestId { get; set; }
    //}

    //public class HaDongPhoDataResponse
    //{
    //    public string transid { get; set; }
    //}
}