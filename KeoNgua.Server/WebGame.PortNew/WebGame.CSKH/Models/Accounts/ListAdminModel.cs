using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Database.DTO;

namespace MsWebGame.CSKH.Models.Accounts
{
    public class ListAdminModel
    {
        public ListAdminModel()
        {
            list = new List<Admin>();
        }
        public List<Admin> list { get; set; }
        public string UserName { get; set; }
        public string PhoneContact { get; set; }
    }
}