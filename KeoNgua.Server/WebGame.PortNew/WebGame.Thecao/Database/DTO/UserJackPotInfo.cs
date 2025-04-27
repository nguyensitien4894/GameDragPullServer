using System;
using MsWebGame.Thecao.Helpers;

namespace MsWebGame.Thecao.Database.DTO
{
    public class UserJackPotInfo
    {
        public string Username { get; set; }
        public long PrizeValue { get; set; }
        public DateTime CreatedTime { get; set; }
        public int GameID { get; set; }
        public int RoomID { get; set; }
        public string GameName
        {
            get
            {
                return GameHelper.GetGameName(GameID.ToString());
            }
        }

    }
}