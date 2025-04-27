using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeoNgua.Server.DataAccess.Dto;
namespace KeoNgua.Server.Models
{
    public class ResultDice
    {
        public int[] Dice;
        public Dictionary<BetSide, double> Rate;
        public int Percent = 0;
        public ResultDice(int[] _Dice, Dictionary<BetSide, double> _Rate,int _Percent)
        {
            Dice = _Dice;
            Rate = _Rate;
            Percent = _Percent;
        }
        public int[] GetDice()
        {
            int[] _Dice = new int[3] { Dice[0], Dice[1], Dice[2] };
            Random random = new Random();
            _Dice.OrderBy(x => random.Next()).ToArray();
            return _Dice;
        }
        public string StringDice(int[] _Dice)
        {
            //    Gourd = 1, //bau
            //Crab = 2, //cua
            //Fish = 3, //ca
            //Chicken = 4, //ga
            //Shrimp = 5, //tom
            //Deer = 6 //huou
            int i = 0;
            string str = "";
            while (_Dice.Length > i)
            {
                if (_Dice[i] == 1) str += "Bau";
                if (_Dice[i] == 2) str += "Cua";
                if (_Dice[i] == 3) str += "Ca";
                if (_Dice[i] == 4) str += "Ga";
                if (_Dice[i] == 5) str += "Tom";
                if (_Dice[i] == 6) str += "huou";
                str += "-";
                i++;
            }
            return str.TrimEnd('-');
        }
    }
    public class ResultIndex
    {
        public double Win { set; get; } = 0;
        public double Lost { set; get; } = 0;
        public int Index { set; get; } = -1; 
    }
}