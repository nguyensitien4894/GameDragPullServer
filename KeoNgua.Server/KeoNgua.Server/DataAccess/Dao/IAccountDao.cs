using KeoNgua.Server.DataAccess.Dto;

namespace KeoNgua.Server.DataAccess.Dao
{
    public interface IAccountDao
    {
        AccountDB GetAccount(long accountId, string nickName, int deviceId, int serviceId, int avatar, int vip);
    }
}
