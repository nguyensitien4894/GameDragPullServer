using System;
using System.Collections.Generic;
using System.Web.Http;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Handlers;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Controllers
{
    public class RankRacingController : BaseApiController
    {
        [HttpGet]
        [ActionName("GetTopVpToDate")]
        public TopVpToDateList GetTopVpToDate(DateTime raceDate)
        {
            try
            {
                int start = 1;
                int quantity = 100;
                long accountId = AccountSession.AccountID;
                int userPos = 0;

                List<TopVpToDate> lstTop = null;
                if (DateTime.Now.Day == raceDate.Day && DateTime.Now.Month == raceDate.Month)
                {
                    DateTime fromDate = DateTime.Today;
                    DateTime toDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    lstTop = RankRacingDAO.Instance.GetBetaForecast(fromDate, toDate, start, quantity, accountId, out userPos);
                }
                else
                {
                    lstTop = RankRacingDAO.Instance.GetTopVpToDate(null, null, start, quantity, null, null, true, raceDate);
                }

                if (lstTop == null)
                    return new TopVpToDateList();

                var l = lstTop.Count;
                if (DateTime.Now.Day == raceDate.Day && DateTime.Now.Month == raceDate.Month)
                {
                    for (int i = 0; i < l; i++)
                    {
                        if (i == 0)
                        {
                            lstTop[i].PrizeValue = 5000000;
                            lstTop[i].PrizeValueStr = MoneyExtension.IntToMoneyFormat(5000000);
                        }
                        else if (i == 1)
                        {
                            lstTop[i].PrizeValue = 3000000;
                            lstTop[i].PrizeValueStr = MoneyExtension.IntToMoneyFormat(3000000);
                        }
                        else if (i >= 2 && i < 5)
                        {
                            lstTop[i].PrizeValue = 1000000;
                            lstTop[i].PrizeValueStr = MoneyExtension.IntToMoneyFormat(1000000);
                        }
                        else if (i >= 5 && i < 10)
                        {
                            lstTop[i].PrizeValue = 200000;
                            lstTop[i].PrizeValueStr = MoneyExtension.IntToMoneyFormat(200000);
                        }
                        else if (i >= 10 && i < 20)
                        {
                            lstTop[i].PrizeValue = 100000;
                            lstTop[i].PrizeValueStr = MoneyExtension.IntToMoneyFormat(100000);
                        }
                        else if (i >= 20 && i < 50)
                        {
                            lstTop[i].PrizeValue = 50000;
                            lstTop[i].PrizeValueStr = MoneyExtension.IntToMoneyFormat(50000);
                        }
                        else if (i >= 50 && i < 100)
                        {
                            lstTop[i].PrizeValue = 20000;
                            lstTop[i].PrizeValueStr = MoneyExtension.IntToMoneyFormat(20000);
                        }
                    }
                }

                foreach (var item in lstTop)
                {
                    item.RankName = ExtensionConvert.IntToRankFormat(item.RankID);
                }

                TopVpToDateList lstRs = new TopVpToDateList();
                lstRs.LstTopVpToDate = lstTop;
                lstRs.UserRank = userPos;

                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [HttpGet]
        [ActionName("GetTopVpToMonth")]
        public List<TopVpToDate> GetTopVpToMonth(int quantity = 10)
        {
            try
            {
                //handler cache data
                var lstRs = CachingHandler.GetListCache<TopVpToDate>("TopVpMonthCa",ServiceID);
                if (lstRs != null)
                    return lstRs;

                lstRs = RankRacingDAO.Instance.GetVpTopMonth(quantity);
                if (lstRs == null)
                    lstRs = new List<TopVpToDate>();

                if (lstRs != null)
                {
                    foreach (var item in lstRs)
                    {
                        item.RankName = ExtensionConvert.IntToRankFormat(item.RankID);
                    }
                    lstRs[0].PrizeValueStr = "01 iPhone XS Max";
                }
                
                CachingHandler.AddListCache<TopVpToDate>("TopVpMonthCa",ServiceID,lstRs, 5);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }
        /// <summary>
        /// Top win
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetGameTopBalance")]
        public List<TopBalance> GetGameTopBalance()
        {
            try
            {
                //handler cache data
                var lstRs = CachingHandler.GetListCache<TopBalance>("TopBalanceCa",ServiceID);
                if (lstRs != null)
                    return lstRs;

                lstRs = RankRacingDAO.Instance.GetGameTopBalance(10);
                if (lstRs == null)
                    lstRs = new List<TopBalance>();

                CachingHandler.AddListCache<TopBalance>("TopBalanceCa",ServiceID, lstRs, 2);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }
    }
}
