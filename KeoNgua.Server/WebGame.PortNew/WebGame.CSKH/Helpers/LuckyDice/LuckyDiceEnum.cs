using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Helpers.LuckyDice
{
    public enum BetSide
    {
        Tai = 0,
        Xiu = 1,
        Unknow = -99
    }

    public enum KeyType
    {
        Exist = 0,
        TotalBet = 1,
        Turn = 2,
        Result = 3,
        Info = 4,
        Bet = 5,
        Summon = 6
    }

    public enum GameState
    {
        /// <summary>
        /// 51s betting
        /// </summary>
        Betting = 0,

        /// <summary>
        /// 12s finish session + can cua + tra thuong
        /// </summary>
        ShowResult = 1,

        /// <summary>
        /// 3s chuan bi cho session moi
        /// </summary>
        PrepareNewRound = 2,

        /// <summary>
        /// 3s chot phien
        /// </summary>
        EndBetting = 3,

        CreateDiceTime = 500,

        #region Bot Config
        //SessionTime = 80000,
        BetFirtTime = 80,
        BetSecondTimes = BetFirtTime + 15,
        BetThirdTimes = BetSecondTimes + 5,

        BetTime = 54000,
        DelayTime = 1000,
        //Bet kết thúc sớm do độ trễ giữa db và webservice
        LoopCount = BetTime / DelayTime,
        #endregion
    }
}