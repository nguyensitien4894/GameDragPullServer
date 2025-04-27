using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Helpers.SeDice
{
    public class Helper
    {
        public static string GenerateKey(long SessionID ,int type, long accountId, int betSide)
        {
            string value = string.Empty;
            if (type == (int)KeyType.Exist)
            {
                value = string.Format("xd.{0}:{1}.{2}", SessionID, accountId, betSide);
            }
            else if (type == (int)KeyType.TotalBet)
            {
                value = string.Format("xd.{0}:totalbet.{1}", SessionID, betSide);
            }
            else if (type == (int)KeyType.Turn)
            {
                value = string.Format("xd.{0}:total.{1}", SessionID, betSide);
            }
            else if (type == (int)KeyType.Result)
            {
                value = string.Format("xd.{0}:result", SessionID);
            }
            else if (type == (int)KeyType.Bet)
            {
                value = string.Format("xd.{0}:{1}.{2}", SessionID, accountId, betSide);
            }
            else if (type == (int)KeyType.Summon)
            {
                value = string.Format("xd.{0}:summon", SessionID);
            }

            return value;
        }
    }
}