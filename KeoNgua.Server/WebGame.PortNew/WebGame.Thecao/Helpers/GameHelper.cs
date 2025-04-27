using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Helpers
{
    public class GameHelper
    {

        private static string _gameConfig
        {
            get
            {
                var config = ConfigurationManager.AppSettings["GAMECONFIG"].ToString();
                return config;
            }
        }
        private static Dictionary<string, string> _GameConfigList
        {
            get
            {
                Dictionary<string, string> _list = new Dictionary<string, string>();
                var arrList = _gameConfig.Split(';');
                foreach (var item in arrList)
                {
                    var gameArr = item.Split(':');

                    _list.Add(gameArr[0], gameArr[1]);
                }
                return _list;
            }

        }

        public static string GetGameName(string gameID)
        {
            try
            {
                string gameName = string.Empty;
                _GameConfigList.TryGetValue(gameID, out gameName);
                return gameName;
            }
            catch
            {
                return string.Empty;
            }


        }
    }
}