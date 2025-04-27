using System.Collections.Generic;
using HorseHunter.Server.DataAccess.DTO;

namespace HorseHunter.Server.DataAccess.DAO
{
    public interface IHorseHunterDao
    {
        RoomFunds GetListRoomFunds(int roomid);
        AccountInfo GetAccountInfo(long accountId, string username, int roomId);

        SpinData Spin(long accountId, string username, int roomId, string lines, int sourceId, int merchantId, int serviceId, string clientIp, long prizein, long jacpotin, ref long prizeout, ref long jackpotout);

        List<JackpotInfo> GetJackpot();

        List<History> GetHistory(long accountId, int top);

        List<BigWinner> GetBigWinner(int type, int top);

        int InsertHistory(int serviceId, int deviceId, long spinId, long accountId, string username, int roomId,
            int totalLine, string lines, int betValue, bool isjp, bool iseventjp, int totalBet, long totalPrize,
            long totalJpVal, long orgBalance, long balance, string slotsData, string prizesData, string des, long jackpot);
    }
}
