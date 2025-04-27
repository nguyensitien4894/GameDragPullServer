using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsTraditionGame.Utilities.Log;

namespace MsTraditionGame.Utilities.Google
{
    public class GoogleUtil
    {
        public static GoogleAccount GetGoogleAccount(string UserInformation)
        {
            try
            {
                var GoogleResponseData = JObject.Parse(UserInformation.ToString());
                if (GoogleResponseData["email"] != null || GoogleResponseData["id"] != null)
                {
                    var GoogleId = (string)GoogleResponseData["id"];
                    var Name = (string)GoogleResponseData["email"];
                    var Email = (string)GoogleResponseData["email"];
                    var GoogleAccount = new GoogleAccount(GoogleId, Name, Email);
                    return GoogleAccount;
                }

                return new GoogleAccount();
            }
            catch (Exception exx)
            {
                NLogManager.PublishException(exx);
                return new GoogleAccount();
            }
        }
    }
}
