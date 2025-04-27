namespace MsWebGame.CSKH.Database.DTO
{
    public enum AgencyKeyType
    {
        UserName = 1,
        Phone = 2,
        Email = 3,
        ID = 4
    }

    public enum SearchType
    {
        DisplayName = 1,
        Phone = 2,
        ID = 3,
        UserName = 4
    }

    public enum TransType
    {
        All = -1,
        Received = 0,
        Forward = 1
    }
}