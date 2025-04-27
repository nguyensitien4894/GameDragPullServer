using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraditionGame.Utilities.Api;
using TraditionGame.Utilities.DNA.Event;
using TraditionGame.Utilities.DNA.Game;
using TraditionGame.Utilities.DNA.VP;

namespace TraditionGame.Utilities.DNA
{
    public class DNAHelpers
    {
        private string _ApiAddress = string.Empty;
        private string _UrI = string.Empty;
        private List<int> AccpetServiceID = new List<int>() {1,2,3};
        private int _ServiceID = 0;
        #region ctor
        /// <summary>
        /// service ID
        /// </summary>
        /// <param name="ServiceID">gate</param>
        /// <param name="type">1 môi trường dev,2 môi trường deploy</param>
        public DNAHelpers(int ServiceID, int type)
        {
            _ServiceID = ServiceID;
            if (ServiceID == 1)
            {
                if (type == 1)//môi trường develop
                {
                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api/";
                }
                if (type == 2)//môi trường deploye
                {

                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api/";
                }

            }
            if (ServiceID == 2)
            {

                if (type == 1)//môi trường develop
                {
                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api//";
                }
                if (type == 2)//môi trường deploye
                {

                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api//";
                }
            }
            if (ServiceID == 3)
            {

                if (type == 1)//môi trường develop
                {
                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api//";
                }
                if (type == 2)//môi trường deploye
                {

                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api//";
                }
            }

        }

        /// <summary>
        /// service ID
        /// </summary>
        /// <param name="ServiceID">gate</param>
        /// <param name="type">1 môi trường dev,2 môi trường deploy</param>
        public DNAHelpers(int ServiceID, int type,bool bulk)
        {
            _ServiceID = ServiceID;
            if (ServiceID == 1)
            {
                if (type == 1)//môi trường develop
                {
                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api/bulk";
                }
                if (type == 2)//môi trường deploye
                {

                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api/bulk";
                }

            }
            if (ServiceID == 2)
            {

                if (type == 1)//môi trường develop
                {
                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api/bulk";
                }
                if (type == 2)//môi trường deploye
                {

                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api//bulk";
                }
            }
            if (ServiceID == 3)
            {

                if (type == 1)//môi trường develop
                {
                    _ApiAddress = "https:/deltadna.net/collect/";
                    _UrI = "api//bulk";
                }
                if (type == 2)//môi trường deploye
                {

                    _ApiAddress = "https://deltadna.net/collect/";
                    _UrI = "api//bulk";
                }
            }
        }

        #endregion
        #region method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userType">1 System,2 FB</param>
        /// <param name="platformType">1:web,2:ios,3:android</param>
        /// <param name="utm_source"></param>
        /// <param name="utm_campaign"></param>
        /// <param name="utm_Medium"></param>
        /// <param name="gclId"></param>
        /// <param name="referrer"></param>
        /// <param name="utm_Content"></param>
        /// <returns></returns>
        public dynamic SenDNACreateAccount(long userID, int userType, int platformType, string utm_source, string utm_campaign, string utm_Medium, string gclId, string referrer, string utm_Content, string ipAddress)
        {
            try
            {

                if (!AccpetServiceID.Contains(_ServiceID))
                {
                    return null;
                }
                String timeStamp = GetTimestamp(DateTime.Now);
                dynamic objSend = new ExpandoObject();
                objSend.eventName = "newPlayer";
                objSend.userID = userID.ToString();
                objSend.eventUUID = Guid.NewGuid().ToString();
                objSend.eventTimestamp = timeStamp;
                objSend.eventParams = new ExpandoObject();
                objSend.eventParams.platform = GetPlatform(platformType);
                objSend.eventParams.userType = UserType(userType);
                objSend.eventParams.ipAddress = ipAddress;
                objSend.eventParams.userScore = 0;
                objSend.eventParams.authenType = 0;
                objSend.eventParams.phoneNumber = "null";
                objSend.eventParams.phoneSafeNo = "null";
                objSend.eventParams.displayName = "null";


                if (!String.IsNullOrEmpty(utm_source))
                {
                    objSend.eventParams.acquisitionChannel = utm_source;
                }
                if (!String.IsNullOrEmpty(utm_campaign))
                {
                    objSend.eventParams.campaign = utm_campaign;
                }
                if (!String.IsNullOrEmpty(utm_Medium))
                {
                    objSend.eventParams.utmMedium = utm_Medium;
                }
                //if (!String.IsNullOrEmpty(gclId))
                //{
                //    objSend.eventParams.uniqueTracking = gclId;
                //}
                if (!String.IsNullOrEmpty(referrer))
                {
                    objSend.eventParams.referrer = referrer;
                }
                if (!String.IsNullOrEmpty(utm_Content))
                {
                    objSend.eventParams.utmContent = utm_Content;
                }
                //kênh marketing để hướng lượng truy cập đên game (vd: Facebook, Google Ad Campaign)

                var result = Send(objSend);
                return result;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }


        /// <summary>
        /// Đổi thưởng(thẻ ,người dùng chuyển tiền  cho đại lý-portal)
        /// </summary>
        /// <param name="userID">user ID</param>
        /// <param name="TransType">1 "VIETTEL"; 2 "VINA"; 3 "MOBI"; 4 "AGENCY"; 5 "TRADE"; 6 "GIFTCODE"</param>
        /// <param name="platformType"></param>
        /// <param name="displayName">displayName cua thang mua Q dai ly</param>
        /// <param name="amount">so tien VND</param>
        /// <param name="currencyName">Q hoac B</param>
        /// <param name="amountGame">So tien Q hoac B</param>
        /// <returns></returns>
        public dynamic SendTransactionSALE(long userID, int TransType, string displayName, long amount, long amountGame)
        {
            //try
            //{
            //    if (!AccpetServiceID.Contains(_ServiceID))
            //    {
            //        return null;
            //    }
            //    String timeStamp = GetTimestamp(DateTime.Now);
            //    dynamic objSend = new ExpandoObject();
            //    objSend.eventName = "transaction";
            //    objSend.userID = userID.ToString();
            //    objSend.eventUUID = Guid.NewGuid().ToString();
            //    objSend.eventTimestamp = timeStamp;
            //    objSend.eventParams = new ExpandoObject();
            //    objSend.eventParams.transactionType = "SALE";
            //    objSend.eventParams.transactionName = GetTransactionName(TransType);
            //    objSend.eventParams.displayName = displayName;
            //    objSend.eventParams.productsReceived = new
            //    {
            //        realCurrency = new
            //        {
            //            realCurrencyType = "VND",
            //            realCurrencyAmount = amount,
            //        }

            //    };
            //    objSend.eventParams.productsSpent = new ExpandoObject();
            //    objSend.eventParams.productsSpent.virtualCurrencies = new dynamic[1];
            //    objSend.eventParams.productsSpent.virtualCurrencies[0] = new ExpandoObject();
            //    objSend.eventParams.productsSpent.virtualCurrencies[0].virtualCurrency = new
            //    {
            //        virtualCurrencyName = GetCurrencyName(_ServiceID),
            //        virtualCurrencyType = "PREMIUM",
            //        virtualCurrencyAmount = amountGame
            //    };

            //    //kênh marketing để hướng lượng truy cập đên game (vd: Facebook, Google Ad Campaign)
            //    var result = Send(objSend);
            //    return result;

            //}
            //catch (Exception ex)
            //{
            //    NLogManager.PublishException(ex);
            //}
            return null;

        }


        /// <summary>
        /// Người dùng mua Q (tool dl dùng hàm này),Nạp thẻ dùng hàm này
        /// </summary>
        /// <param name="userID">user ID(ko phai ID dai ly)</param>
        /// <param name="TransType">1 "VIETTEL"; 2 "VINA"; 3 "MOBI"; 4 "AGENCY"; 5 "TRADE"; 6 "GIFTCODE";7 "USDT",8 "ZING",9 "VCOIN"</param>
        /// <param name="platformType"></param>
        /// <param name="displayName">displayName cua thang mua Q dai ly</param>
        /// <param name="amount">so tien VND</param>
        /// <param name="currencyName">Q hoac B</param>
        /// <param name="amountGame">So tien Q hoac B</param>
        /// <returns></returns>
        public dynamic SendTransactionPURCHASE(long userID, int TransType, string displayName, long amount, long amountGame)
        {
            //try
            //{
            //    if (!AccpetServiceID.Contains(_ServiceID))
            //    {
            //        return null;
            //    }
            //    String timeStamp = GetTimestamp(DateTime.Now);
            //    dynamic objSend = new ExpandoObject();
            //    objSend.eventName = "transaction";
            //    objSend.userID = userID.ToString();
            //    objSend.eventUUID = Guid.NewGuid().ToString();
            //    objSend.eventTimestamp = timeStamp;
            //    objSend.eventParams = new ExpandoObject();
            //    objSend.eventParams.transactionType = "PURCHASE";
            //    objSend.eventParams.transactionName = GetTransactionName(TransType);
            //    if (!String.IsNullOrEmpty(displayName))
            //    {
            //        objSend.eventParams.displayName = displayName;
            //    }

            //    objSend.eventParams.productsSpent = new
            //    {
            //        realCurrency = new
            //        {
            //            realCurrencyType = "VND",
            //            realCurrencyAmount = amount,
            //        }

            //    };

            //    objSend.eventParams.productsReceived = new ExpandoObject();
            //    objSend.eventParams.productsReceived.virtualCurrencies = new dynamic[1];
            //    objSend.eventParams.productsReceived.virtualCurrencies[0] = new ExpandoObject();
            //    objSend.eventParams.productsReceived.virtualCurrencies[0].virtualCurrency = new
            //    {
            //        virtualCurrencyName = GetCurrencyName(_ServiceID),
            //        virtualCurrencyType = "PREMIUM",
            //        virtualCurrencyAmount = amountGame
            //    };


            //    var result = Send(objSend);
            //    return result;

            //}
            //catch (Exception ex)
            //{
            //    NLogManager.PublishException(ex);
            //}
            return null;

        }
        /// <summary>
        /// gui len danh sach gia tri khuyen mai cho user id the 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="TransType"></param>
        /// <param name="displayName"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public dynamic SendTransactionBounus(long userID, int TransType, string displayName, long amount)
        {
            try
            {

                if (!AccpetServiceID.Contains(_ServiceID))
                {
                    return null;
                }
                String timeStamp = GetTimestamp(DateTime.Now);
                dynamic objSend = new ExpandoObject();
                objSend.eventName = "transaction";
                objSend.userID = userID.ToString();
                objSend.eventUUID = Guid.NewGuid().ToString();
                objSend.eventTimestamp = timeStamp;
                objSend.eventParams = new ExpandoObject();
                objSend.eventParams.transactionType = "PURCHASE";
                objSend.eventParams.transactionName = GetTransactionNameBONUS(TransType);
                if (!String.IsNullOrEmpty(displayName))
                {
                    objSend.eventParams.displayName = displayName;
                }

               

                objSend.eventParams.productsReceived = new ExpandoObject();
                objSend.eventParams.productsReceived.virtualCurrencies = new dynamic[1];
                objSend.eventParams.productsReceived.virtualCurrencies[0] = new ExpandoObject();
                objSend.eventParams.productsReceived.virtualCurrencies[0].virtualCurrency = new
                {
                    virtualCurrencyName = GetCurrencyName(_ServiceID),
                    virtualCurrencyType = "PREMIUM",
                    virtualCurrencyAmount = amount
                };


                var result = Send(objSend);
                return result;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

        /// <summary>
        /// Hàm dùng gửi lên DNA thông tin bet của TX, 3 Cây, Poker, Xóc Đĩa, Rồng Hổ, Đua Khỉ
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Balance">Balance sau khi kết thúc phiên</param>
        /// <param name="Game">Game dùng EnumGameDAN</param>
        /// <param name="BetAmount">Giá trị bet</param>
        /// <param name="RefundAmount">Giá trị hoàn lại</param>
        /// <param name="WinAmount">Giá trị thắng</param>
        /// <param name="sID">Phiên</param>
        /// <returns></returns>
        public dynamic SendBetSummary(long UserID, long Balance, string Game, long BetAmount, long RefundAmount, long WinAmount, long sID)
        {
            try

            {
                if (!AccpetServiceID.Contains(_ServiceID))
                {
                    return null;
                }
                String timeStamp = GetTimestamp(DateTime.Now);
                dynamic objSend = new ExpandoObject();
                objSend.eventName = "betSummary";
                objSend.userID = UserID.ToString();
                objSend.eventUUID = Guid.NewGuid().ToString();
                objSend.eventTimestamp = timeStamp;

                objSend.eventParams = new ExpandoObject();
                objSend.eventParams.userScore = Balance;
                objSend.eventParams.game = Game;
                objSend.eventParams.betAmount = BetAmount;
                objSend.eventParams.refundAmount = RefundAmount;
                objSend.eventParams.winAmount = WinAmount;
                objSend.eventParams.sID = sID;


                var result = Send(objSend);
                return result;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

        /// <summary>
        /// gửi đồng bộ Event send
        /// </summary>
        /// <param name="paramter"></param>
        /// <returns></returns>
        public dynamic SendEvent(EventParamaters paramter)
        {
            try

            {
                if (!AccpetServiceID.Contains(_ServiceID))
                {
                    return null;
                }
                var result = Send(paramter);
                return result;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }
        /// <summary>
        /// gửi đồng bộ Game send
        /// </summary>
        /// <param name="paramter"></param>
        /// <returns></returns>
        public dynamic SendGame(GameParamaters paramter)
        {
            try

            {
                if (!AccpetServiceID.Contains(_ServiceID))
                {
                    return null;
                }
                var result = Send(paramter);
                return result;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }
        public dynamic SendVP(VPParamaters paramter)
        {
            try

            {
                if (!AccpetServiceID.Contains(_ServiceID))
                {
                    return null;
                }
                var result = Send(paramter);
                return result;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

        public dynamic SendTreasure(CarrotParamaters paramter)
        {
            try

            {
                if (!AccpetServiceID.Contains(_ServiceID))
                {
                    return null;
                }
                var result = Send(paramter);
                return result;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

        #endregion
        #region DNA Helper function
        private dynamic Send(dynamic input)
        {
            try
            {
                var api = new ApiUtil<dynamic>();
                api.ApiAddress = _ApiAddress;
                api.URI = _UrI;
                var result = api.Send(input);
                return result;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        private string GetCurrencyName(int serviceID)
        {
            if (serviceID == 1) return "Q";
            if (serviceID == 2) return "B";
            else return "NO";
        }
        private string GetPlatform(int pType)
        {

            if (pType == 1) return "WEB";
            else if (pType == 3) return "IOS";
            else if (pType == 2) return "ANDROID";
            else return "WEB";

        }
        /// <summary>
        /// 1:VIETTELL,2 VINA,3 MOBI,4 AGENCY,5 TRADE,6 GIFTCODE.7 USDT.8 ZING.9 VCOIN
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        private string GetTransactionName(int pType)
        {

            if (pType == 1) return "VIETTEL";
            else if (pType == 2) return "VINA";
            else if (pType == 3) return "MOBI";
            else if (pType == 4) return "AGENCY";
            else if (pType == 5) return "TRADE";
            else if (pType == 6) return "GIFTCODE";
            else if (pType == 7) return "USDT";
            else if (pType == 8) return "ZING";
            else if (pType == 9) return "VCOIN";
            else if (pType == 10) return "MOMO";
            else return "UNKNOWN";

        }

        private string GetTransactionNameBONUS(int pType)
        {

            if (pType == 1) return "VIETTEL_BONUS";
            else if (pType == 2) return "VINA_BONUS";
            else if (pType == 3) return "MOBI_BONUS";
            //else if (pType == 4) return "AGENCY";
            //else if (pType == 5) return "TRADE";
            //else if (pType == 6) return "GIFTCODE";
            //else if (pType == 7) return "USDT";
            else if (pType == 8) return "ZING_BONUS";
            else if (pType == 9) return "VCOIN_BONUS";
            else return "UNKNOWN";

        }


        private string UserType(int userType)
        {

            return userType == 1 ? "SYSTEM" : "FACEBOOK";

        }
        #endregion
    }
}
