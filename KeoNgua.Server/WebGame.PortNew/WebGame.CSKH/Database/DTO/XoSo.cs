using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class XoSo
    {
        public string SpecialPrizeData {get;set; }
        public string FirstPrizeData  { get; set; }
        public string SecondPrizeData  { get; set; }
        public string ThirdPrizeData { get; set; }
        public string FourthPrizeData  { get; set; }
        public string FifthPrizeData { get; set; }
        public string SixthPrizeData { get; set; }
        public string SeventhPrizeData  { get; set; }
        public string EighthPrizeData  { get; set; }
        public long SessionID { get; set; }
    }
}