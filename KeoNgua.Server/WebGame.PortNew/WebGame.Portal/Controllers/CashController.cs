using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities.Messages;
using MsWebGame.Portal.Database.DAO;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Cash")]
    public class CashController : ApiController
    {
        //[Route("CahInMarket")]
        //[AllowAnonymous]
        //[HttpOptions, HttpPost]
        //public dynamic CahInMarket([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        string userID = input.UserID;
        //        string amount = input.Amount;
        //        long lngUserID = Convert.ToInt64(userID);
        //        long lngAmount = Convert.ToInt64(amount);
        //        long fromTranID = Convert.ToInt64(input.FromTranID);
        //        string note = input.Note;
                
        //        long balance;
        //        long tranID;
        //        int response;
        //        CashDAO.Instance.CashInMarket(lngUserID, lngAmount, note, fromTranID, out balance, out tranID, out response);

        //    }
        //    catch
        //    {

        //    }
        //    return new
        //    {
        //        ResponseCode = -99,
        //        Message = ErrorMsg.InProccessException
        //    };
        //}

        //[Route("CashOutMarket")]
        //[AllowAnonymous]
        //[HttpOptions, HttpPost]
        //public dynamic CashOutMarket([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        string userID = input.UserID;
        //        string amount = input.OrgAmount;
        //        long lngUserID = Convert.ToInt64(userID);
        //        long lngAmount = Convert.ToInt64(amount);
        //        long fromTranID = Convert.ToInt64(input.FromTranID);
        //        string note = input.Note;

        //        long balance;
        //        long BalanceHang;
        //        int response;
        //        long TranID;
        //        CashDAO.Instance.CashOutMarket(lngUserID, lngAmount, note, out balance, out BalanceHang,out TranID, out response);

        //    }
        //    catch
        //    {

        //    }
        //    return new
        //    {
        //        ResponseCode = -99,
        //        Message = ErrorMsg.InProccessException
        //    };
        //}

        //[Route("CashUpdate")]
        //[AllowAnonymous]
        //[HttpOptions, HttpPost]
        //public dynamic CashUpdate([FromBody] dynamic input)
        //{
        //    try
        //    {
        //        string userID = input.UserID;
        //        string tranID = input.TranID;
        //        string tranStatus = input.TranStatus;
        //        long lngUserID = Convert.ToInt64(userID);
        //        long lngtranID = Convert.ToInt64(tranID);
        //        long balance;
        //        int response;
        //        CashDAO.Instance.CashUpdate(lngUserID, lngtranID, tranStatus, out balance, out response);

        //    }
        //    catch
        //    {

        //    }
        //    return new
        //    {
        //        ResponseCode = -99,
        //        Message = ErrorMsg.InProccessException
        //    };
        //}
    }



}
