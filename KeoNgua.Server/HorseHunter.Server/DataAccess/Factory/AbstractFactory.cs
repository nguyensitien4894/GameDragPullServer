using System;
using HorseHunter.Server.DataAccess.DAO;
using TraditionGame.Utilities;

namespace HorseHunter.Server.DataAccess.Factory
{
    public abstract class AbstractFactory
    {
        public static AbstractFactory Instance()
        {
            try
            {
                return new Factory();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw new Exception("Countn't Created AbstractFactory");
            }
        }

        public abstract IHorseHunterDao CreateHorseHunterDao();
    }
}