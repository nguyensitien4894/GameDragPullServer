using System.Collections.Generic;
using KeoNgua.Server.DataAccess.Dto;

namespace KeoNgua.Server.DataAccess.Dao
{
    public interface IKeoNguaDao
    {
        long CreateSession();
        int Bet(long sessionId, long accountId, int serviceId, int deviceId, int betSide, long betValue, out long balance);
        void CreateManualDice(long sessionId, out int dice1, out int dice2, out int dice3);
        int FinishSession(long sessionId, DiceResult result);
        List<SessionResult> GetAwardSession(long sessionId);
        List<SoiCau> GetSoiCau();
        List<BigWinner> GetBigWinner();
        List<History> GetHistory(long accountId, int top);
        int BotBet(long sessionId, long accountId, int serviceId, int deviceId, int betSide, long amount);
        List<BotDB> GetListCardBot(int gameId, out string betValues);
    }
}
