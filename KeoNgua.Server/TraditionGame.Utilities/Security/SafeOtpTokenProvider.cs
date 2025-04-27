using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Security
{
    public class SafeAccount
    {
        
        public string PhoneNumber { get; set; }
        public string IPAddress { get; set; }
        long SafeID { get; set; }
        public DateTime ExpiredAt { get; set; }
        //public string DeviceId { get; set; }
        public string MachineName
        {
            get
            {
                return Security.MD5Encrypt(Environment.MachineName);
            }
        }

        public string RandomString
        {
            get
            {
                return Security.RandomString(10);
            }
        }
    }
    public class SafeOtpTokenProvider
    {
        private const string Secret = "jTmVrckYrMmQvMXNGbldHNEhuVjhUWlkzNzM3MzYzamRoZGhkaGRiM09Jc2orQlhFOU5aRHkwdDhXM1Rld2QjFHbE9nSnVRWmRjRjJMdXFtL2hjY013PT0zNTM=";
      
        public static string GenerateToken(long SafeID, string PhoneNumber, string IPAddress)
        {
            var prepareToken = new SafeAccount
            {
              
                PhoneNumber = PhoneNumber,

               
                ExpiredAt = DateTime.Now.AddMinutes(10),
                IPAddress = IPAddress,

            };
            var jsob = JsonConvert.SerializeObject(prepareToken);

            var token = Security.SHA256Encrypt(jsob);

            return token;
        }

       
       
    }
}
