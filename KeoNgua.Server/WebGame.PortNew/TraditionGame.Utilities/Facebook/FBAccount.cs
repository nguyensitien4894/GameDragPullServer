using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsTraditionGame.Utilities.Facebook
{
    public class FBAccount
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Link { get; set; }
        public string UserName { get; set; }
        public DateTime BirthDay { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public int Verified { get; set; }
        public DateTime UpdateTime { get; set; }

        public FBAccount() { }

        public FBAccount(long accountID, string accountName, string email)
        {
            this.Id = accountID;
            this.Name = accountName;
            this.Email = email;
        }
    }

    [Serializable]
    public class IDs_Business
    {
        public string id { get; set; }
        public AppInfo app { get; set; }
    }

    [Serializable]
    public class AppInfo
    {
        public string name { get; set; }
        public string name_space { get; set; }
        public string id { get; set; }
    }
}
