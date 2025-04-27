using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Cookies;
using TraditionGame.Utilities.Facebook;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.IpAddress;

using TraditionGame.Utilities.Security;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models;
using System.Configuration;
using TraditionGame.Utilities.Messages;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/History")]
    public class HistoryController : BaseApiController
    {


        /// <summary>
        /// get user history
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBalanceHistory")]
        [AllowAnonymous]
        public dynamic GetBalanceHistory([FromBody] dynamic input)
        {
            //kiểm  tra account id
            var accountId = AccountSession.AccountID;
            var displayName = AccountSession.AccountName;
            if (accountId <= 0 || String.IsNullOrEmpty(displayName))
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            var tkServiceID = AccountSession.ServiceID;
            if (tkServiceID != ServiceID)
            {
                return new
                {
                    ResponseCode = Constants.NOT_IN_SERVICE,
                    Message = ErrorMsg.NOTINSERVICE
                };
            }

            try
            {
                string ExchangeType = input.ExchangeType;
                int intExchangeType = String.IsNullOrEmpty(ExchangeType) ? -1 : Convert.ToInt32(ExchangeType);
                string DateTransaction = input.DateTransaction;
                DateTime dtDateTransaction = DateTransaction == null ? DateTime.Now : Convert.ToDateTime(DateTransaction);
                string CurrentPage = input.CurrentPage;
                int intCurrentPage = String.IsNullOrEmpty(CurrentPage) ? 0 : Convert.ToInt32(CurrentPage);
                string RecordPerpage = input.RecordPerpage;
                int intRecordPerpage = String.IsNullOrEmpty(RecordPerpage) ? 0 : Convert.ToInt32(RecordPerpage);
                int totalPage;
                var list = HistoryDAO.Instance.GetBalanceHistory(accountId, intExchangeType, intCurrentPage, intRecordPerpage, dtDateTransaction, out totalPage);

                return new
                {
                    ResponseCode = 1,
                    list = list,
                    totalPage

                };


            }
            catch
            {
                return new
                {
                    ResponseCode = -99,
                    list = new List<UserHistory>(),
                    totalPage = 0
                };
            }

        }


        /// <summary>
        /// get game history
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetGameHistory")]
        [AllowAnonymous]
        public dynamic GetGameHistory([FromBody] dynamic input)
        {
            //kiểm  tra account id
            var accountId = AccountSession.AccountID;

            var displayName = AccountSession.AccountName;
            if (accountId <= 0 || String.IsNullOrEmpty(displayName))
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            var tkServiceID = AccountSession.ServiceID;
            if (tkServiceID != ServiceID)
            {
                return new
                {
                    ResponseCode = Constants.NOT_IN_SERVICE,
                    Message = ErrorMsg.NOTINSERVICE
                };
            }

            try
            {
                string GameType = input.GameType;
                int intGameType = String.IsNullOrEmpty(GameType) ? -1 : Convert.ToInt32(GameType);
                string DateTransaction = input.DateTransaction;
                DateTime dtDateTransaction = DateTransaction == null ? DateTime.Now : Convert.ToDateTime(DateTransaction);
                string CurrentPage = input.CurrentPage;
                int intCurrentPage = String.IsNullOrEmpty(CurrentPage) ? 0 : Convert.ToInt32(CurrentPage);
                string RecordPerpage = input.RecordPerpage;
                int intRecordPerpage = String.IsNullOrEmpty(RecordPerpage) ? 0 : Convert.ToInt32(RecordPerpage);
                int totalPage;

                var list = HistoryDAO.Instance.GetGameHistory(accountId, intGameType, intCurrentPage, intRecordPerpage, dtDateTransaction, out totalPage);

                return new
                {
                    ResponseCode = 1,
                    list = list,
                    totalPage

                };


            }
            catch
            {
                return new
                {
                    ResponseCode = -99,
                    list = new List<GameHistory>(),
                    totalPage = 0
                };
            }

        }
    }
}