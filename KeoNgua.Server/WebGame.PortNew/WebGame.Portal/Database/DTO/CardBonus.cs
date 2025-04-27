using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Security;

namespace MsWebGame.Portal.Database.DTO
{
    public class CardBonus
    {
        [JsonIgnore]
        private string KeyGen = "bc5820dd3f9a8bfc2988c7224119e378970896b84282b272258528ff5d77ac63#@j93";
        public long UserID { get; set; }
        public int RankID { get; set; }
        public long  VP { get; set; }
        [JsonIgnore]
        public DateTime EffectDate { get; set; }
        
        public long PointAcc { get; set; }
        [JsonIgnore]
        public double VipQuaterCoeff { get; set; }
        [JsonIgnore]
        public double LoanLimit { get; set; }
        [JsonIgnore]
        public double LoanRate { get; set; }
        public int CardLimit { get; set; }
        public double CardRate { get; set; }
        public int CardVpPeriod { get; set; }
        public int  CardVpPeriodBonus { get; set; }
       
        public int  CardBonusNo { get; set; }
        [JsonIgnore]
        public string  CardBonusStatus { get; set; }

        public string Key
        {
            get
            {
                return Security.SHA256Encrypt(String.Format("{0}{1}{2}{3}{4}{5}{6}", UserID, RankID, VP, CardBonusNo, CardRate, CardLimit, KeyGen));
            }
        }

    }
}