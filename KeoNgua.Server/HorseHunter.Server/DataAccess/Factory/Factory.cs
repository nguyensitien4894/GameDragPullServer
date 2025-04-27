using HorseHunter.Server.DataAccess.DAO;
using HorseHunter.Server.DataAccess.DAOImpl;

namespace HorseHunter.Server.DataAccess.Factory
{
    public class Factory : AbstractFactory
    {
        public override IHorseHunterDao CreateHorseHunterDao()
        {
            return new HorseHunterDaoImpl();
        }
    }
}