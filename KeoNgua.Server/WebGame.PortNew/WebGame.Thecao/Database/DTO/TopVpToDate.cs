using System.Collections.Generic;
using Newtonsoft.Json;

namespace MsWebGame.Thecao.Database.DTO
{
    public class TopVpToDate
    {
        public string GameAccountName { get; set; }
       
        public int RankID { get; set; }
        public string RankName { get; set; }
        public long CurrentAccVP { get; set; }
        public long PrizeValue { get; set; }
        public string PrizeValueStr { get; set; }
    }

    public class TopVpToDateList
    {
        public int UserRank { get; set; }
        public List<TopVpToDate> LstTopVpToDate { get; set; }
    }
}