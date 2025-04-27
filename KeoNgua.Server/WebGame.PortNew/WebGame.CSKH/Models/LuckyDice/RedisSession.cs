using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace MsWebGame.CSKH.Models.LuckyDice
{
    public class RedisSession
    {
        public long SessionID { get; set; }
        public MsWebGame.CSKH.Helpers.LuckyDice.GameState CurrentState { get; set; }
        public int Ellapsed { get; set; }
        public int Dice1 { get; set; }
        public int Dice2 { get; set; }
        public int Dice3 { get; set; }

        public RedisSession() { }

        public RedisSession(long sessionId, MsWebGame.CSKH.Helpers.LuckyDice.GameState currState, int ellapsed, int dice1, int dice2, int dice3)
        {
            this.SessionID = sessionId;
            this.CurrentState = currState;
            this.Ellapsed = ellapsed;
            this.Dice1 = dice1;
            this.Dice2 = dice2;
            this.Dice3 = dice3;
        }
    }
}