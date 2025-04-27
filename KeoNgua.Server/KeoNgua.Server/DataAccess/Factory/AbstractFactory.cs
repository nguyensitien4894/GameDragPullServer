using System;
using KeoNgua.Server.DataAccess.Dao;

namespace KeoNgua.Server.DataAccess.Factory
{
    public abstract class AbstractFactory
    {
        public static AbstractFactory Instance()
        {
            try
            {
                return new Factory();
            }
            catch (Exception)
            {
                throw new Exception("Couldn't create AbstractDAOFactory: ");
            }
        }

        public abstract IAccountDao CreateAccountDao();

        public abstract IKeoNguaDao CreateGameDao();
    }
}