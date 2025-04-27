using System.Collections.Generic;
using Newtonsoft.Json;

namespace HorseHunter.Server.DataAccess.DTO
{
    public class SpinData
    {
        public long SpinID { get; set; }
        public string LinesData { get; set; }
        public int TotalBet { get; set; }
        public List<int> SlotsData { get; set; }
        [JsonIgnore]
        public string SlosDataStr { get; set; }
        public List<PrizeLine> PrizeLines { get; set; }
        [JsonIgnore]
        public string PrizeLinesStr { get; set; }
        public bool IsJackpot { get; set; }
        public bool IsJackpotEvent { get; set; }
        public long Jackpot { get; set; }
        [JsonIgnore]
        public int TotalJackpot { get; set; }
        public long TotalJackpotValue { get; set; }
        public long PaylinePrize { get; set; }
        public long OrgBalance { get; set; }
        public long Balance { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        public int FreeSpin { get; set; }
        public int EventFreeSpin { get; set; }
        public int Response { get; set; }
        public SpinData() { }

        public SpinData(long spinId, string linesData, int totalBet, string slotsData, string prizeData, string posData, bool isjp, bool isjpevent, 
            int jpNum, long jackpot, long totalJpVal, long payline, long orgbalance, long balance, string des, int freespin, int efreespin, int response)
        {
            this.SpinID = spinId;
            this.LinesData = linesData;
            this.TotalBet = totalBet;
            this.SlotsData = SetSlotsData(slotsData);
            this.SlosDataStr = slotsData;
            this.PrizeLines = SetPrizeLines(prizeData, posData);
            this.PrizeLinesStr = prizeData;
            this.IsJackpot = isjp;
            this.IsJackpotEvent = isjpevent;
            this.TotalJackpot = jpNum;
            this.Jackpot = jackpot;
            this.TotalJackpotValue = totalJpVal;
            this.PaylinePrize = payline;
            this.OrgBalance = orgbalance;
            this.Balance = balance;
            this.Description = des;
            this.FreeSpin = freespin;
            this.EventFreeSpin = efreespin;
            this.Response = response;
        }

        public static List<int> SetSlotsData(string slotsData)
        {
            if (string.IsNullOrEmpty(slotsData))
                return new List<int>();

            var lstSlotsData = slotsData.Split(',');
            if (lstSlotsData.Length < 8)
                return new List<int>();

            var slotItems = new List<int>();
            foreach (var item in lstSlotsData)
            {
                int symBolId = 0;
                int.TryParse(item, out symBolId);
                if (symBolId > 0)
                    slotItems.Add(symBolId);
            }

            return slotItems;
        }

        public static List<PrizeLine> SetPrizeLines(string prizeData, string posData)
        {
            if (string.IsNullOrEmpty(prizeData) || string.IsNullOrEmpty(posData))
                return new List<PrizeLine>();

            string[] PrizeLinesData = prizeData.Split(';');
            string[] PositionLinesData = posData.Split(';');
            int len = PrizeLinesData.Length;

            if (len > 0)
            {
                var prizesData = new List<PrizeLine>();
                for (int i = 0; i < len; i++)
                {
                    string prizeLineData = PrizeLinesData[i];
                    string positionLineData = PositionLinesData[i];
                    if (string.IsNullOrEmpty(prizeLineData) || string.IsNullOrEmpty(positionLineData))
                        continue;

                    PrizeLine prizeLine = new PrizeLine(prizeLineData, positionLineData);
                    prizesData.Add(prizeLine);
                }

                return prizesData;
            }

            return new List<PrizeLine>();
        }

    }
}