using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{

    public class CasoutBank
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Nickname { get; set; }
        public string SoTK { get; set; }
        public string TenTK { get; set; }
        public long Amount { get; set; }
        public long Price { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? ActionTime { get; set; }
        public int Service { get; set; }
    }
    public class CasoutMomo
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public long Amount { get; set; }
        public long Price { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? ActionTime { get; set; }
        public int Service { get; set; }
    }
}