using System;
using TraditionGame.Utilities.Utils;

namespace KeoNgua.Server.DataAccess.Dto
{
    public class DiceResult : ICloneable
    {
        public int Dice1 { get; private set; }
        public int Dice2 { get; private set; }
        public int Dice3 { get; private set; }

        public DiceResult()
        {
            this.ClearResult();
        }

        public void GetNewResult()
        {
            this.Dice1 = 1 + RandomUtil.NextByte(6);
            this.Dice2 = 1 + RandomUtil.NextByte(6);
            this.Dice3 = 1 + RandomUtil.NextByte(6);
        }

        public void GetNewResult(int dice1, int dice2, int dice3)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
            this.Dice3 = dice3;
        }

        public void ClearResult()
        {
            this.Dice1 = this.Dice2 = this.Dice3 = -1;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}