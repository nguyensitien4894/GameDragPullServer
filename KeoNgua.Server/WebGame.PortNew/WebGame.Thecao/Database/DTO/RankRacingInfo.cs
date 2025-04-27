using Newtonsoft.Json;
using System.Collections.Generic;

namespace MsWebGame.Thecao.Database.DTO
{
    public class RankRacingInfo
    {
     
        public long UserID { get; set; }
       
        public string Username { get; set; }
        public string GameAccountName { get; set; }
        public long Balance { get; set; }
        public long PrizeValue { get; set; }
    }

    public class RankRacingInfoList
    {
        public int UserRank { get; set; }
        public List<RankRacingInfo> LstRankRacingInfo { get; set; }
    }
}