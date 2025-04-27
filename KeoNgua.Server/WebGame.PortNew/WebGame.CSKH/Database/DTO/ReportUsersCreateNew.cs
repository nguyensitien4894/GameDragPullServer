using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class ReportUsersCreateNew
    {
        public DateTime DateCreate { get; set; }
        public int NumCreate { get; set; }
    }
    public class ReportProfit
    {
        public long luong { get; set; }
        public long profit { get; set; }
        public string gamename { get; set; }
    }
}