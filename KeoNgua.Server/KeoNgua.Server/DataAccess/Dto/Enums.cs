namespace KeoNgua.Server.DataAccess.Dto
{
    public enum BetSide
    {
        //gourd-crab-fish-chicken-shrimp-deer
        Gourd = 1, //bau
        Crab = 2, //cua
        Fish = 3, //ca
        Chicken = 4, //ga
        Shrimp = 5, //tom
        Deer = 6 //huou
    }

    public enum Phrases
    {
        Waiting = 0, //cho
        Shaking = 1, //xoc
        Betting = 2, //dat cua
        EndBetting = 3, //het tg dat cua
        OpenPlate = 4, //mo
        ShowResult = 5 //ket qua
    }

    public enum ErrorCode
    {
        Success = 1,
        InputInvalid = -10,
        DeviceIDNotExist = -13,
        InvalidTime = -14,
        Bet = -15,
        InvalidBetSide = -16,
        InvalidBetTime = -17,
        OtherDevice = -98,
        Exception = -99,
        Undefined = -999,
        NotAuthen = -1001,
        Duplicate = -1003,
    }
}