using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Helpers.Chargings.Cards
{
    public class CardResponse
    {
        public bool IsError { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Signature { get; set; }
    }



    public class VinaResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public string Signature { get; set; }
    }
}