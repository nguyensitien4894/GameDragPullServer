using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MsWebGame.Portal.Helpers;

namespace MsWebGame.Portal.Database.DTO
{
    public class JackpotInfo
    {
        public int GameID { get; set; }
        public int RoomID { get; set; }
        
        public long JackpotFund { get; set; }
        public string GameName
        {
            get
            {
                return GameHelper.GetGameName(GameID.ToString());
            }
        }


    }
}