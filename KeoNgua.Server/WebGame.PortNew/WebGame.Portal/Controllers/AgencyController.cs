using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Handlers;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Agency")]
    public class AgencyController : BaseApiController
    {
        [ActionName("GetList")]
        [HttpGet]
        //[Authorize]
        public dynamic GetList()
        {
            try
            {
                //handler cache data
                //var lstRs = CachingHandler.GetListCache<Agency>("AgencyListCa",ServiceID);
                //if (lstRs != null && lstRs.Any())
                //{
                //    return new
                //    {
                //        ResponseCode = 1,
                //        List = lstRs
                //    };
                //}

                int TotalRecord = 0;
                var lstRs = AgencyDAO.Instance.GetList(null, null, null, 1,-1,1,ServiceID, 1, 50, out TotalRecord);
                if(lstRs == null)
                    lstRs = new List<Agency>();


                List<Agency> lstRsNew = new List<Agency>();
                int Index = 0;
                foreach (var item in lstRs)
                {
                    item.PhoneNo = string.IsNullOrEmpty(item.PhoneDisplay) ? string.Empty : item.PhoneDisplay.Replace(",", Environment.NewLine);

                    if (item.DisplayName == "dieulinh24h")
                    {
                        item.Index = lstRs.Count * -5;
                        lstRsNew.Add(item);
                    }else if (item.DisplayName == "Daiphat888") 
                    {
                        item.Index = lstRs.Count * -10;
                        lstRsNew.Add(item);
                    }else if (item.DisplayName == "ShopKyDuyen")
                    {
                        item.Index = lstRs.Count * -15;
                        lstRsNew.Add(item);
                    }else if (item.DisplayName == "uytin9999")
                    {
                        item.Index = lstRs.Count * -14;
                        lstRsNew.Add(item);
                    }
                    else
                    {
                        item.Index = Index++; ;
                        lstRsNew.Add(item);
                    }
                };
                lstRsNew = lstRsNew.OrderBy(s => s.Index).ToList();
                CachingHandler.AddListCache<Agency>("AgencyListCa", ServiceID, lstRsNew, 600);
                return new
                {
                    ResponseCode = 1,
                    List = lstRsNew
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
    }
}