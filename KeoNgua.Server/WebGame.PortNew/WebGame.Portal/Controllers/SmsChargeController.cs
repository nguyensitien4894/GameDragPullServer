using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Handlers;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Helpers.Chargings.Cards;
using MsWebGame.Portal.Helpers.Chargings.FconnClub;
using MsWebGame.Portal.Helpers.Chargings.MobileSMS;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using MsWebGame.Portal.ShopMuaThe;
using TraditionGame.Utilities.DNA;
using System.Threading;
using TraditionGame.Utilities.XBomms;
using MsWebGame.Portal.Models;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/SmsCharge")]
    public class SmsChargeController : BaseApiController
    {

        [ActionName("GetChargeConfigs")]
        [HttpGet]
        public dynamic GetChargeConfigs()
        {
            try
            {
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
                //Lấy tỉ lệ quy đổi 
                var lstBanks = SmsChargeDAO.Instance.SmsOperatorList(ServiceID, null, null, null).Where(c=>c.Status==true);
                if (lstBanks == null || !lstBanks.Any())
                {
                    return new
                    {
                        ResponseCode = Constants.MAINTAIN,
                        Message = "Functional maintenance system"
                    };

                }
                var countVTT=lstBanks.Count(c => c.Telecom == "VTT");
                if (countVTT >= 2)
                {
                    return new
                    {
                        ResponseCode =-1005,
                        Message = "Maintenance system |2 VTT"
                    };
                }
                var countVNP = lstBanks.Count(c => c.Telecom == "VNP");
                if (countVNP >= 2)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Maintenance system |2 VNP"
                    };
                }

                var lstCards = SmsChargeDAO.Instance.SmsValuesList(0, 0,0, ServiceID);
                if (lstCards == null || !lstCards.Any())
                {
                    return new
                    {
                        ResponseCode = Constants.MAINTAIN,
                        Message = "Functional maintenance system"
                    };

                }

               var  model = new List<SmsChargeConfigModel>();
                foreach(var item in lstBanks)
                {
                    SmsChargeConfigModel modelAdd = new SmsChargeConfigModel();
                    //chỗ này đang được hiểu 1 VTT của 1 partner chỉ được active 1 lần )
                    if (item.Telecom=="VTT")
                    {
                        modelAdd.Telecom = item.Telecom;
                        var listValuesByPartners = lstCards.Where(c => c.PartnerID==item.PartnerID).OrderBy(c=>c.DisplayValue).ToList();
                        List<SmsCardModel> listCards = new List<SmsCardModel>();
                        foreach (var item2 in listValuesByPartners)
                        {
                            SmsCardModel value = new SmsCardModel();
                            value.Value = item2.Value;
                            value.Systax = String.Format(item.Sysntax, item2.DisplayValue.ToString(), accountId.ToString());
                            value.Amount = ConvertUtil.ToLong(item2.Value * item2.Rate);
                            listCards.Add(value);

                        }
                        modelAdd.cards = listCards;


                        model.Add(modelAdd);


                    }
                    if (item.Telecom == "VNP")
                    {
                        modelAdd.Telecom = item.Telecom;
                        var listValuesByPartners = lstCards.Where(c => c.PartnerID == item.PartnerID).OrderBy(c => c.DisplayValue).ToList();
                        List<SmsCardModel> listCards = new List<SmsCardModel>();
                        foreach (var item2 in listValuesByPartners)
                        {
                            SmsCardModel value = new SmsCardModel();
                            value.Value = item2.Value;
                            value.Systax = String.Format(item.Sysntax, item2.DisplayValue.ToString(), accountId.ToString());
                            value.Amount = ConvertUtil.ToLong(item2.Value * item2.Rate);
                            listCards.Add(value);

                        }
                        modelAdd.cards = listCards;


                        model.Add(modelAdd);


                    }


                }
                return new
                {
                    ResponseCode =1,
                    Data = model,
                    To= "9029"
                };
                

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = "The system is busy, please come back later"
                };
            }
        }



        
    }
  
   
}
