using KeoNgua.Server.DataAccess.Dao;
using KeoNgua.Server.DataAccess.DaoImpl;

namespace KeoNgua.Server.DataAccess.Factory
{
    public class Factory : AbstractFactory
    {
        public override IAccountDao CreateAccountDao()
        {
            return new AccountDaoImpl();
        }

        public override IKeoNguaDao CreateGameDao()
        {
            return new KeoNguaDaoImpl();
        }
    }
}