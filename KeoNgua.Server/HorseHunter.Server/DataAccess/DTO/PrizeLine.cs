using System.Collections.Generic;

namespace HorseHunter.Server.DataAccess.DTO
{
    public class PrizeLine
    {
        public int LineID;
        public int PrizeID;
        public long PrizeValue;
        public List<int> Items { get; set; }

        public PrizeLine(string prizeData, string positionData)
        {
            if (string.IsNullOrEmpty(prizeData) || string.IsNullOrEmpty(positionData))
                return;

            var lstPrizeData = prizeData.Split(',');
            if (lstPrizeData.Length > 2)
            {
                int.TryParse(lstPrizeData[0].Trim(), out LineID);
                int.TryParse(lstPrizeData[1].Trim(), out PrizeID);
                long.TryParse(lstPrizeData[2].Trim(), out PrizeValue);
            }

            var lstPositionData = positionData.Split(',');
            foreach (var pos in lstPositionData)
            {
                if (Items == null)
                    Items = new List<int>();

                Items.Add(int.Parse(pos));
            }
        }
    }
}