namespace HorseHunter.Server.DataAccess.DTO
{
    public class Enums
    {

    }

    public enum ErrorCode
    {
        Success = 1,
        InputInvalid = -10,
        LinesInvalid = -11,
        RoomNotExist = -12,
        DeviceIDNotExist = -13,
        InvalidTime = -14,
        Bet = -15,
        OtherDevice = -98,
        Exception = -99,
        Captcha = -100,
        Undefined = -999,
        NotAuthen = -1001,
        PlayerNull = -1002,
        Duplicate = -1003,
        BlockSpin = -1004,
        BlockPlayNow = -1005,
        PlayBonus = -1006,
        PlayFreeSpin = -1007,
        PlayX2 = -1008
    }
}