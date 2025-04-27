using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models.USDTBanks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;

namespace MsWebGame.Portal.Controllers.Transaction
{
    public class TransactionController : BaseApiController
    {
        [ActionName("GetTopupRate")]
        [HttpGet]
        public dynamic GetTopupRate()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                //momo rate
                var momo = new RateResponse();
                
                if (Lib.Constant.MOMO_APPROVED != "1")
                {
                    momo.Status = -1;
                    momo.Message = "Hệ thống đang tạm ngưng để bảo trì!";
                }
                else
                {
                    var rs1 = TransactionDAO.GetRate("momo");
                    momo.Status = 1;
                    momo.Code = "OK";
                    momo.TopupRate = rs1.TopupRate;
                    momo.MinTopup = rs1.MinTopup;
                    momo.MaxTopup = rs1.MaxTopup;
                    momo.CashoutRate = rs1.CashoutRate;
                    momo.MinCashout = rs1.MinCashout;
                    momo.MaxCashout = rs1.MaxCashout;
                }

                //bank rate
                var bank = new RateResponse();
                if (Lib.Constant.BANK_APPROVED != "1")
                {
                    bank.Status = -1;
                    bank.Message = "Hệ thống đang tạm ngưng để bảo trì!";
                }
                else
                {
                    var rs2 = TransactionDAO.GetRate("bank");
                    bank.Status = 1;
                    bank.Code = "OK";
                    bank.TopupRate = rs2.TopupRate;
                    bank.MinTopup = rs2.MinTopup;
                    bank.MaxTopup = rs2.MaxTopup; 
                    bank.CashoutRate = rs2.CashoutRate;
                    bank.MinCashout = rs2.MinCashout;
                    bank.MaxCashout = rs2.MaxCashout;
                }
                //viettel pay
                var viettelpay = new RateResponse();
                if (Lib.Constant.VIETTELPAY_APPROVED != "1")
                {
                    viettelpay.Status = -1;
                    viettelpay.Message = "Hệ thống đang tạm ngưng để bảo trì!";
                }
                else
                {
                    var rs = TransactionDAO.GetRate("viettelpay");
                    viettelpay.Status = 1;
                    viettelpay.Code = "OK";
                    viettelpay.TopupRate = rs.TopupRate;
                    viettelpay.MinTopup = rs.MinTopup;
                    viettelpay.MaxTopup = rs.MaxTopup;
                    viettelpay.CashoutRate = rs.CashoutRate;
                    viettelpay.MinCashout = rs.MinCashout;
                    viettelpay.MaxCashout = rs.MaxCashout;
                }

                //zalo pay
                var zalo = new RateResponse();
                if (Lib.Constant.VIETTELPAY_APPROVED != "1")
                {
                    zalo.Status = -1;
                    zalo.Message = "Hệ thống đang tạm ngưng để bảo trì!";
                }
                else
                {
                    var rs = TransactionDAO.GetRate("zalo");
                    zalo.Status = 1;
                    zalo.Code = "OK";
                    zalo.TopupRate = rs.TopupRate;
                    zalo.MinTopup = rs.MinTopup;
                    zalo.MaxTopup = rs.MaxTopup;
                    zalo.CashoutRate = rs.CashoutRate;
                    zalo.MinCashout = rs.MinCashout;
                    zalo.MaxCashout = rs.MaxCashout;
                }

                //the cao
                var vt = new RateResponse();
                var vn = new RateResponse();
                var mb = new RateResponse();
                int cardStatus = 1;
                string cardMes = "OK";
                if (Lib.Constant.CARD_APPROVED != "1")
                {
                    cardStatus = -1;
                    cardMes = "Hệ thống đang tạm ngưng để bảo trì!";
                }
                else
                {
                    //Viettel
                    var rs3 = CardDAO.Instance.GetTeleCom("VTT", ServiceID).FirstOrDefault();
                    vt.Status = rs3.Status ? 1 : 0;
                    vt.TopupRate = rs3.Rate;
                    vt.CashoutRate = rs3.ExchangeRate;
                    vt.Code = "VTT";
                    //Vinaphone

                    var rs4 = CardDAO.Instance.GetTeleCom("VNP", ServiceID).FirstOrDefault();
                    vn.Status = rs4.Status ? 1 : 0;
                    vn.TopupRate = rs4.Rate;
                    vn.CashoutRate = rs4.ExchangeRate;
                    vn.Code = "VNP";

                    //mobi

                    var rs5 = CardDAO.Instance.GetTeleCom("VMS", ServiceID).FirstOrDefault();
                    mb.Status = rs5.Status ? 1 : 0;
                    mb.TopupRate = rs5.Rate;
                    mb.CashoutRate = rs5.ExchangeRate;
                    mb.Code = "VMS";
                }

                var card = new RateResponse();

                return new
                {
                    ResponseCode = 1,
                    Bank = bank,
                    Momo = momo,
                    ViettelPay = viettelpay,
                    ZaloPay = zalo,
                    Card = new
                    {
                        Status = cardStatus,
                        Message = cardMes,
                        vt = vt,
                        vn = vn,
                        mb = mb
                    }
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new {
                    Status = -99,
                    Description = "Hệ thống bận!"
                };
            }
        }


    }

    public class RateResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }


        public double TopupRate { get; set; }
        public int MinTopup { get; set; }
        public int MaxTopup { get; set; }
        
        public double CashoutRate { get; set; }
        public int MinCashout { get; set; }
        public int MaxCashout { get; set; }
    }

    public class CashoutParam
    {
        public long Amount { get; set; }
        public string Type { get; set; }

        public string Captcha { get; set; }
        public string PrivateKey { get; set; }

    }

}
