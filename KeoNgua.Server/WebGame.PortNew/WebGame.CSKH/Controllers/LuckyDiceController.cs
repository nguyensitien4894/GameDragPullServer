using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.LuckyDice;
using MsWebGame.CSKH.Models.SeDice;
using MsWebGame.CSKH.Utils;
using MsWebGame.RedisCache.Cache;
using MsWebGame.CSKH.Models.Accounts;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using TraditionGame.Utilities.Utils;
using System.Windows;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class LuckyDiceController : BaseController
    {
        // GET: LuckyDice
        public ActionResult Index()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Event()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult RaceTop()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult AddOrUpdateEvent()
        {
            EventModel model = new EventModel();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ReportEvent()
        {
            ViewBag.PartialEvents = ComboboxEvents();
            ViewBag.PartialAwards = ComboboxAwards();

            ReportEventModel model = new ReportEventModel();
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            model.RaceTopID = -1;
            model.EventID = 0;
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateEvent(EventModel model)
        {
            //EventModel model = new EventModel();
            EventModel eventModel = LuckyDiceDAO.Instance.GetEventInfo(model.EventID);
            return View("AddOrUpdateEvent", eventModel);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetEventList(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng

            var list = LuckyDiceDAO.Instance.GetEventList();
            var gridModel = new GridModel<EventModel>
            {
                Data = list,
                Total = list.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SaveEvent(EventModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.SaveEvent(model);
            if (response < 0)
            {
                string msg = MessageConvetor.MsgLuckyDice.GetEventMessage(response);
                ErrorNotification(msg);
                return View("AddOrUpdateEvent", model);
            }
            return RedirectToAction("Event", "LuckyDice");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteEvent(EventModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.DeleteEvent(model);
            if (response < 0)
            {
                ErrorNotification(Message.DeleteFail);
            }else
            {
                SuccessNotification(Message.DeleteSuccess);
            }
            return RedirectToAction("Event");
        }


        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult AddOrUpdateRaceTop()
        {
            RaceTopModel model = new RaceTopModel();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Quy()
        {
            //ViewBag.Logs = LuckyDiceDAO.Instance.GetSoiCau(15);
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult QuyMd5()
        {
            //ViewBag.Logs = LuckyDiceDAO.Instance.GetSoiCau(15);
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Xd()
        {
            ViewBag.Logs = LuckyDiceDAO.Instance.GetSoiCau(15);
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult XdLive()
        {
            ViewBag.Logs = LuckyDiceDAO.Instance.GetSoiCau(15);
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateRaceTop(RaceTopModel model)
        {
            //EventModel model = new EventModel();
            RaceTopModel eventModel = LuckyDiceDAO.Instance.GetRaceTopInfo(model.RaceTopID);
            return View("AddOrUpdateRaceTop", eventModel);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SaveRaceTop(RaceTopModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.SaveRaceTop(model);
            if (response < 0)
            {
                string msg = MessageConvetor.MsgLuckyDice.GetEventMessage(response);
                ErrorNotification(msg);
                return View("AddOrUpdateRaceTop", model);
            }
            return RedirectToAction("RaceTop", "LuckyDice");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteRaceTop(RaceTopModel model, GridCommand command)
        {
            int response = LuckyDiceDAO.Instance.DeleteRaceTop(model);
            if (response < 0)
            {
                ErrorNotification(Message.DeleteFail);
            }
            else
            {
                SuccessNotification(Message.DeleteSuccess);
            }
            return RedirectToAction("RaceTop");
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetRaceTopList(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            var list = LuckyDiceDAO.Instance.GetRaceTopList();
            var gridModel = new GridModel<RaceTopModel>
            {
                Data = list,
                Total = list.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetEventAwardList(ReportEventModel model, GridCommand command)
        {
            int totalRecord = 0;
            var list = LuckyDiceDAO.Instance.GetEventAwardList(model, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<ReportEventModel>
            {
                Data = list,
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        private List<EventModel> ComboboxEvents()
        {
            var list = new List<EventModel>();
            var eventList = LuckyDiceDAO.Instance.GetEventList();
            list.AddRange(eventList);
            return list;
        }

        private List<RaceTopModel> ComboboxAwards()
        {
            var list = new List<RaceTopModel>();
            list.Add(new RaceTopModel
            {
                RaceTopID = 0,
                Description = "Triệu hồi"
            });
            var eventList = LuckyDiceDAO.Instance.GetRaceTopList();
            list.AddRange(eventList);

            return list;
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public dynamic InitSessionMd5(GridCommand command)
        {
            return JsonConvert.DeserializeObject<dynamic>(Get("http://127.0.0.1:81/bancaapi/gettxresult/5f67cff4ff68ad17a534d8c1f1ec6cdd"));
        }
        public string Get(string uri, string Method = "GET")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = Method;
            request.ContentLength = 0;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult InitSession(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keySession = "tx.session:info";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            RedisSession result = _cachePvd.Get<RedisSession>(keySession);
            if (result == null)
            {
                result = new RedisSession();
            }
            long TotalBetTai = 0,TotalBetXiu = 0, TotalTai = 0,TotalXiu = 0;
            string key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID, (int)Helpers.LuckyDice.KeyType.TotalBet, 0, (int)Helpers.LuckyDice.BetSide.Tai);

            if (_cachePvd.Exists(key))
                TotalBetTai = _cachePvd.Get<long>(key);
            key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID,(int)Helpers.LuckyDice.KeyType.TotalBet, 0, (int)Helpers.LuckyDice.BetSide.Xiu);
            if (_cachePvd.Exists(key))
                TotalBetXiu = _cachePvd.Get<long>(key);
            key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID,(int)Helpers.LuckyDice.KeyType.Turn, 0, (int)Helpers.LuckyDice.BetSide.Tai);
            if (_cachePvd.Exists(key))
                TotalTai = _cachePvd.Get<long>(key);
           key = Helpers.LuckyDice.Helper.GenerateKey(result.SessionID,(int)Helpers.LuckyDice.KeyType.Turn, 0, (int)Helpers.LuckyDice.BetSide.Xiu);
            if (_cachePvd.Exists(key))
                TotalXiu = _cachePvd.Get<long>(key);





            List<UserBetBalance> BetBalancesTai = new List<UserBetBalance>();
            string keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "UserBetBalance." + 0);
            
            if (_cachePvd.Exists(keyHu))
            {
                BetBalancesTai = _cachePvd.Get<List<UserBetBalance>>(keyHu);
            }

            List<UserBetBalance> BetBalancesXiu = new List<UserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "UserBetBalance." + 1);

            if (_cachePvd.Exists(keyHu))
            {
                BetBalancesXiu = _cachePvd.Get<List<UserBetBalance>>(keyHu);
            }
            keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "Mode");

            List<UserBetBalance> ListBetBalancesTai = BetBalancesTai.FindAll(e => e.AccountID > 0);
            List<UserBetBalance> ListBetBalancesXiu = BetBalancesXiu.FindAll(e => e.AccountID > 0);
            List<UserBetBalance> DataBetBalancesTai = new List<UserBetBalance>();
            foreach (var val in ListBetBalancesTai)
            {
                UserBetBalance userBetBalance = DataBetBalancesTai.Find(x => x.AccountID == val.AccountID);

                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance!=null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History,70);
                    }

                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "HistoryInOut:"+ result.SessionID+":"+ val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;
                     
                       
                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID, 
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);
                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);
                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesTai.Add(val);
                }
            }
            List<UserBetBalance> DataBetBalancesXiu = new List<UserBetBalance>();
            foreach (var val in ListBetBalancesXiu)
            {
                UserBetBalance userBetBalance = DataBetBalancesXiu.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance> 0 ?string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);
                         
                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesXiu.Add(val);
                }
            }

            //AccountPlayGame result = CustomerSupportDAO.Instance.GetAccountGameProfit(gameId, accountId);

            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
                 Data = new
                 {
                     SessionID = result.SessionID,
                     BetBalancesTai = DataBetBalancesTai,
                     BetBalancesXiu = BetBalancesXiu.FindAll(e => e.AccountID > 0),
                     Dices = new List<int>() { result.Dice1, result.Dice2, result.Dice3 },
                     Ellapsed = result.Ellapsed,
                     TotalBetTai = TotalBetTai,
                     TotalBetXiu = TotalBetXiu,
                     TotalTai = TotalTai,
                     TotalXiu = TotalXiu,
                     CurrentState = result.CurrentState,
                     Model = _cachePvd.Get<int>(keyHu),
                     Logs = result.CurrentState == MsWebGame.CSKH.Helpers.LuckyDice.GameState.ShowResult ? LuckyDiceDAO.Instance.GetSoiCau(15) : new List<SoiCau>()
                 },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModel(LuckyDiceModel input)
        {
            //lay danh sách chăm sóc khách hàng
            string keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            int _Model = (input.Model == 1 ? 0 : (input.Model == 2 ? 1 : -1));
            _cachePvd.Set(keyHu, _Model);
             
            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
                Data = new
                {
                    IsOke = _cachePvd.Get<int>(keyHu) == _Model
                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModelMd5(LuckyDiceModel input)
        {
            int Dice1 = 1 + RandomUtil.NextByte(6);
            int Dice2 = 1 + RandomUtil.NextByte(6);
            int Dice3 = 1 + RandomUtil.NextByte(6);
            while (true)
            {
                Dice1 = 1 + RandomUtil.NextByte(6);
                Dice2 = 1 + RandomUtil.NextByte(6);
                Dice3 = 1 + RandomUtil.NextByte(6);
                if(input.Model == 1) // tai
                {
                    if (Dice1 + Dice2 + Dice3 >= 11)
                    {
                        break;
                    }
                }
                else
                {
                    if (Dice1 + Dice2 + Dice3 <= 10)
                    {
                        break;
                    }
                }

            }
            JsonConvert.DeserializeObject<dynamic>(Get(String.Format("http://127.0.0.1:81/bancaapi/settxresult/5f67cff4ff68ad17a534d8c1f1ec6cdd/{0}/{1}/{2}", Dice1, Dice2, Dice3), "POST"));




            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
                Data = new
                {
                    IsOke = true
                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult InitSessionXd(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keySession = "xd.session:info";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            SeDiceRedisSession result = _cachePvd.Get<SeDiceRedisSession>(keySession);
            if (result == null)
            {
                result = new SeDiceRedisSession();
            }
            //long TotalBetChan = 0, TotalBetLe = 0, TotalChan = 0, TotalLe = 0;
            long TotalBetEven = 0, TotalBetOdd = 0, TotalBetFourDown = 0, TotalBetFourUp = 0, TotalBetThreeDown = 0, TotalBetThreeUp = 0;
            long TotalEven = 0, TotalOdd = 0, TotalFourDown = 0, TotalFourUp = 0, TotalThreeDown = 0, TotalThreeUp = 0;

            string key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.TotalBet, 0, (int)Helpers.SeDice.BetSide.Even);
            if (_cachePvd.Exists(key)) TotalBetEven = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.TotalBet, 0, (int)Helpers.SeDice.BetSide.Odd);
            if (_cachePvd.Exists(key)) TotalBetOdd = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.TotalBet, 0, (int)Helpers.SeDice.BetSide.FourDown);
            if (_cachePvd.Exists(key)) TotalBetFourDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.TotalBet, 0, (int)Helpers.SeDice.BetSide.FourUp);
            if (_cachePvd.Exists(key)) TotalBetFourUp = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.TotalBet, 0, (int)Helpers.SeDice.BetSide.ThreeDown);
            if (_cachePvd.Exists(key)) TotalBetThreeDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.TotalBet, 0, (int)Helpers.SeDice.BetSide.ThreeUp);
            if (_cachePvd.Exists(key)) TotalBetThreeUp = _cachePvd.Get<long>(key);

            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.Turn, 0, (int)Helpers.SeDice.BetSide.Even);
            if (_cachePvd.Exists(key)) TotalEven = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.Turn, 0, (int)Helpers.SeDice.BetSide.Odd);
            if (_cachePvd.Exists(key)) TotalOdd = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.Turn, 0, (int)Helpers.SeDice.BetSide.FourDown);
            if (_cachePvd.Exists(key)) TotalFourDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.Turn, 0, (int)Helpers.SeDice.BetSide.FourUp);
            if (_cachePvd.Exists(key)) TotalFourUp = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.Turn, 0, (int)Helpers.SeDice.BetSide.ThreeDown);
            if (_cachePvd.Exists(key)) TotalThreeDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDice.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDice.KeyType.Turn, 0, (int)Helpers.SeDice.BetSide.ThreeUp);
            if (_cachePvd.Exists(key)) TotalThreeUp = _cachePvd.Get<long>(key);


            List<SeDiceUserBetBalance> BetBalancesEven = new List<SeDiceUserBetBalance>();
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "UserBetBalance." + (int)Helpers.SeDice.BetSide.Even);
            if (_cachePvd.Exists(keyHu)) { BetBalancesEven = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesOdd = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "UserBetBalance." + (int)Helpers.SeDice.BetSide.Odd);
            if (_cachePvd.Exists(keyHu)) { BetBalancesOdd = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesFourUp = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "UserBetBalance." + (int)Helpers.SeDice.BetSide.FourUp);
            if (_cachePvd.Exists(keyHu)) { BetBalancesFourUp = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesFourDown = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "UserBetBalance." + (int)Helpers.SeDice.BetSide.FourDown);
            if (_cachePvd.Exists(keyHu)) { BetBalancesFourDown = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesThreeUp = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "UserBetBalance." + (int)Helpers.SeDice.BetSide.ThreeUp);
            if (_cachePvd.Exists(keyHu)) { BetBalancesThreeUp = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesThreeDown = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "UserBetBalance." + (int)Helpers.SeDice.BetSide.ThreeDown);
            if (_cachePvd.Exists(keyHu)) { BetBalancesThreeDown = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }


            keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "Mode");

            List<SeDiceUserBetBalance> ListBetBalancesEven = BetBalancesEven.FindAll(e => e.AccountID > 0);
            List<SeDiceUserBetBalance> ListBetBalancesOdd = BetBalancesOdd.FindAll(e => e.AccountID > 0);
            List<SeDiceUserBetBalance> ListBetBalancesFourDown = BetBalancesFourDown.FindAll(e => e.AccountID > 0);
            List<SeDiceUserBetBalance> ListBetBalancesFourUp = BetBalancesFourUp.FindAll(e => e.AccountID > 0);
            List<SeDiceUserBetBalance> ListBetBalancesThreeDown = BetBalancesThreeDown.FindAll(e => e.AccountID > 0);
            List<SeDiceUserBetBalance> ListBetBalancesThreeUp = BetBalancesThreeUp.FindAll(e => e.AccountID > 0);

            List<SeDiceUserBetBalance> DataBetBalancesEven = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesEven)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesEven.Find(x => x.AccountID == val.AccountID);

                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 70);
                    }

                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("Sedie", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);
                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }



                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);
                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesEven.Add(val);
                }
            }
            List<SeDiceUserBetBalance> DataBetBalancesOdd = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesOdd)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesOdd.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("Sedie", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesOdd.Add(val);
                }
            }
            List<SeDiceUserBetBalance> DataBetBalancesFourDown = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesFourDown)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesFourDown.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("Sedie", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesFourDown.Add(val);
                }
            }
            List<SeDiceUserBetBalance> DataBetBalancesFourUp = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesFourUp)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesFourUp.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("Sedie", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesFourUp.Add(val);
                }
            }
            List<SeDiceUserBetBalance> DataBetBalancesThreeUp = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesThreeUp)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesThreeUp.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("Sedie", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesThreeUp.Add(val);
                }
            }
            List<SeDiceUserBetBalance> DataBetBalancesThreeDown = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesThreeDown)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesThreeDown.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(8, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("Sedie", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesThreeDown.Add(val);
                }
            }

            //AccountPlayGame result = CustomerSupportDAO.Instance.GetAccountGameProfit(gameId, accountId);

            keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "Mode");
            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
                Data = new
                {
                    SessionID = result.SessionID,
                    BetBalancesEven = DataBetBalancesEven,
                    //BetBalancesOdd = BetBalancesOdd.FindAll(e => e.AccountID > 0),
                    BetBalancesOdd = DataBetBalancesOdd,
                    BetBalancesFourUp = DataBetBalancesFourUp,
                    BetBalancesFourDown = DataBetBalancesFourDown,
                    BetBalancesThreeUp = DataBetBalancesThreeUp,
                    BetBalancesThreeDown = DataBetBalancesThreeDown,
                    Dices = new List<int>() { result.Dice1, result.Dice2, result.Dice3, result.Dice4 },
                    Ellapsed = result.Ellapsed,
                    TotalBetEven = TotalBetEven,
                    TotalBetOdd = TotalBetOdd,
                    TotalBetFourDown = TotalBetFourDown,
                    TotalBetFourUp = TotalBetFourUp,
                    TotalBetThreeDown = TotalBetThreeDown,
                    TotalBetThreeUp = TotalBetThreeUp,

                    TotalEven = TotalEven,
                    TotalOdd = TotalOdd,
                    TotalFourDown = TotalFourDown,
                    TotalFourUp = TotalFourUp,
                    TotalThreeDown = TotalThreeDown,
                    TotalThreeUp = TotalThreeUp,

                    CurrentState = result.CurrentState,
                    Model = _cachePvd.Get<int>(keyHu),
                    Logs = result.CurrentState == MsWebGame.CSKH.Helpers.SeDice.GameState.ShowResult ? SeDiceDAO.Instance.GetSoiCau(15) : new List<SeDiceSoiCau>()
                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }



        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModelXd(LuckyDiceModel input)
        {
            //lay danh sách chăm sóc khách hàng
            string keyHu = CachingHandler.Instance.GeneralRedisKey("Sedie", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            int _Model = input.Model;
            _cachePvd.Set(keyHu, _Model);

            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
                Data = new
                {
                    IsOke = _cachePvd.Get<int>(keyHu) == _Model
                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult InitSessionXdLive(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keySession = "xdlive.session:info";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            SeDiceRedisSession result = _cachePvd.Get<SeDiceRedisSession>(keySession);
            if (result == null)
            {
                result = new SeDiceRedisSession();
            }
            //long TotalBetChan = 0, TotalBetLe = 0, TotalChan = 0, TotalLe = 0;
            long TotalBetEven = 0, TotalBetOdd = 0, TotalBetFourDown = 0, TotalBetFourUp = 0, TotalBetThreeDown = 0, TotalBetThreeUp = 0;
            long TotalEven = 0, TotalOdd = 0, TotalFourDown = 0, TotalFourUp = 0, TotalThreeDown = 0, TotalThreeUp = 0;

            string key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.TotalBet, 0, (int)Helpers.SeDiceLive.BetSide.Even);
            if (_cachePvd.Exists(key)) TotalBetEven = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.TotalBet, 0, (int)Helpers.SeDiceLive.BetSide.Odd);
            if (_cachePvd.Exists(key)) TotalBetOdd = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.TotalBet, 0, (int)Helpers.SeDiceLive.BetSide.FourDown);
            if (_cachePvd.Exists(key)) TotalBetFourDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.TotalBet, 0, (int)Helpers.SeDiceLive.BetSide.FourUp);
            if (_cachePvd.Exists(key)) TotalBetFourUp = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.TotalBet, 0, (int)Helpers.SeDiceLive.BetSide.ThreeDown);
            if (_cachePvd.Exists(key)) TotalBetThreeDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.TotalBet, 0, (int)Helpers.SeDiceLive.BetSide.ThreeUp);
            if (_cachePvd.Exists(key)) TotalBetThreeUp = _cachePvd.Get<long>(key);

            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.Turn, 0, (int)Helpers.SeDiceLive.BetSide.Even);
            if (_cachePvd.Exists(key)) TotalEven = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.Turn, 0, (int)Helpers.SeDiceLive.BetSide.Odd);
            if (_cachePvd.Exists(key)) TotalOdd = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.Turn, 0, (int)Helpers.SeDiceLive.BetSide.FourDown);
            if (_cachePvd.Exists(key)) TotalFourDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.Turn, 0, (int)Helpers.SeDiceLive.BetSide.FourUp);
            if (_cachePvd.Exists(key)) TotalFourUp = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.Turn, 0, (int)Helpers.SeDiceLive.BetSide.ThreeDown);
            if (_cachePvd.Exists(key)) TotalThreeDown = _cachePvd.Get<long>(key);
            key = Helpers.SeDiceLive.Helper.GenerateKey(result.SessionID, (int)Helpers.SeDiceLive.KeyType.Turn, 0, (int)Helpers.SeDiceLive.BetSide.ThreeUp);
            if (_cachePvd.Exists(key)) TotalThreeUp = _cachePvd.Get<long>(key);


            List<SeDiceUserBetBalance> BetBalancesEven = new List<SeDiceUserBetBalance>();
            string keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "UserBetBalance." + (int)Helpers.SeDiceLive.BetSide.Even);
            if (_cachePvd.Exists(keyHu)) { BetBalancesEven = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesOdd = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "UserBetBalance." + (int)Helpers.SeDiceLive.BetSide.Odd);
            if (_cachePvd.Exists(keyHu)) { BetBalancesOdd = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesFourUp = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "UserBetBalance." + (int)Helpers.SeDiceLive.BetSide.FourUp);
            if (_cachePvd.Exists(keyHu)) { BetBalancesFourUp = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesFourDown = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "UserBetBalance." + (int)Helpers.SeDiceLive.BetSide.FourDown);
            if (_cachePvd.Exists(keyHu)) { BetBalancesFourDown = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesThreeUp = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "UserBetBalance." + (int)Helpers.SeDiceLive.BetSide.ThreeUp);
            if (_cachePvd.Exists(keyHu)) { BetBalancesThreeUp = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            List<SeDiceUserBetBalance> BetBalancesThreeDown = new List<SeDiceUserBetBalance>();
            keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "UserBetBalance." + (int)Helpers.SeDiceLive.BetSide.ThreeDown);
            if (_cachePvd.Exists(keyHu)) { BetBalancesThreeDown = _cachePvd.Get<List<SeDiceUserBetBalance>>(keyHu); }

            
            keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "Mode");

            List<SeDiceUserBetBalance> ListBetBalancesEven = BetBalancesEven.FindAll(e => e.AccountID > 0);
            
            
            List<SeDiceUserBetBalance> DataBetBalancesEven = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesEven)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesEven.Find(x => x.AccountID == val.AccountID);

                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(68, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 70);
                    }

                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("SedieLive", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);
                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }



                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);
                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesEven.Add(val);
                }
            }

            List<SeDiceUserBetBalance> ListBetBalancesOdd = BetBalancesOdd.FindAll(e => e.AccountID > 0);

            List<SeDiceUserBetBalance> DataBetBalancesOdd = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesOdd)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesOdd.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(68, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("SedieLive", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesOdd.Add(val);
                }
            }

            List<SeDiceUserBetBalance> ListBetBalancesFourUp = BetBalancesFourUp.FindAll(e => e.AccountID > 0);

            List<SeDiceUserBetBalance> DataBetBalancesFourUp = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesFourUp)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesFourUp.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(68, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("SedieLive", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesFourUp.Add(val);
                }
            }

            List<SeDiceUserBetBalance> ListBetBalancesFourDown = BetBalancesFourDown.FindAll(e => e.AccountID > 0);

            List<SeDiceUserBetBalance> DataBetBalancesFourDown = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesFourDown)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesFourDown.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(68, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("SedieLive", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesFourDown.Add(val);
                }
            }

            List<SeDiceUserBetBalance> ListBetBalancesThreeUp = BetBalancesThreeUp.FindAll(e => e.AccountID > 0);

            List<SeDiceUserBetBalance> DataBetBalancesThreeUp = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesThreeUp)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesThreeUp.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(68, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("SedieLive", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesThreeUp.Add(val);
                }
            }

            List<SeDiceUserBetBalance> ListBetBalancesThreeDown = BetBalancesThreeDown.FindAll(e => e.AccountID > 0);

            List<SeDiceUserBetBalance> DataBetBalancesThreeDown = new List<SeDiceUserBetBalance>();
            foreach (var val in ListBetBalancesThreeDown)
            {
                SeDiceUserBetBalance userBetBalance = DataBetBalancesThreeDown.Find(x => x.AccountID == val.AccountID);
                long Balance = CustomerSupportDAO.Instance.GetAccountBalance(val.AccountID);

                val.Balance = Balance > 0 ? string.Format("{0:#,###,###}", Balance) : "0";

                if (userBetBalance != null)
                {
                    userBetBalance.Amount += val.Amount;
                }
                else
                {
                    string _keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "GetAccountGameProfit:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHu))
                    {
                        val.History = _cachePvd.Get<string>(_keyHu);
                    }
                    else
                    {
                        AccountPlayGame _result = CustomerSupportDAO.Instance.GetAccountGameProfit(68, val.AccountID);
                        val.History = _result.TotalBetValue + "|" + _result.TotalPrizeValue + "|" + _result.RateBetPrize;
                        _cachePvd.SetSecond(_keyHu, val.History, 60);
                    }
                    string _keyHuHistoryInOut = CachingHandler.Instance.GeneralRedisKey("SedieLive", "HistoryInOut:" + result.SessionID + ":" + val.AccountID);
                    if (_cachePvd.Exists(_keyHuHistoryInOut))
                    {
                        val.HistoryInOut = _cachePvd.Get<string>(_keyHuHistoryInOut);
                    }
                    else
                    {
                        long TotalRecharge = 0;
                        long TotalRechargeBank = 0;
                        long TotalMomo = 0;
                        long TotalValueInAgency = 0;
                        long TotalValueOutAgency = 0;


                        int status = CustomerSupportDAO.Instance.GetAccountInOut(val.AccountID,
                            out TotalRecharge,
                            out TotalRechargeBank,
                            out TotalMomo,
                            out TotalValueInAgency,
                            out TotalValueOutAgency);

                        long kenh = (TotalRecharge + TotalRechargeBank + TotalMomo);

                        string strKenh = kenh > 0 ? string.Format("{0:#,###,###}", (TotalRecharge + TotalRechargeBank + TotalMomo)) : "0";
                        val.HistoryInOut = strKenh + "|" + ((TotalValueInAgency > 0) ? string.Format("{0:#,###,###}", TotalValueInAgency) : "0") + "|" + ((TotalValueOutAgency > 0) ? string.Format("{0:#,###,###}", TotalValueOutAgency) : "0");
                        _cachePvd.SetSecond(_keyHuHistoryInOut, val.HistoryInOut, 70);
                    }

                    string keyHuLive = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + val.AccountID);
                    if (_cachePvd.Exists(keyHuLive))
                    {
                        Database.DTO.ParConfigLive parConfig = _cachePvd.Get<Database.DTO.ParConfigLive>(keyHuLive);

                        if (parConfig != null)
                        {
                            val.Type = parConfig.Type;
                        }
                    }
                    DataBetBalancesThreeDown.Add(val);
                }
            }

            //AccountPlayGame result = CustomerSupportDAO.Instance.GetAccountGameProfit(gameId, accountId);

            keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "Mode");
            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
            Data = new
                {
                    SessionID = result.SessionID,
                    BetBalancesEven = DataBetBalancesEven,
                    //BetBalancesOdd = BetBalancesOdd.FindAll(e => e.AccountID > 0),
                    BetBalancesOdd = DataBetBalancesOdd,
                //BetBalancesFourUp = BetBalancesFourUp,
                BetBalancesFourUp = DataBetBalancesFourUp,
                //BetBalancesFourDown = BetBalancesFourDown,
                BetBalancesFourDown = DataBetBalancesFourDown,
                //BetBalancesThreeUp = BetBalancesThreeUp,
                BetBalancesThreeUp = DataBetBalancesThreeUp,
                //BetBalancesThreeDown = BetBalancesThreeDown,
                BetBalancesThreeDown = DataBetBalancesThreeDown,

                Dices = new List<int>() { result.Dice1, result.Dice2, result.Dice3, result.Dice4 },
                    Ellapsed = result.Ellapsed,
                    TotalBetEven = TotalBetEven,
                    TotalBetOdd = TotalBetOdd,
                    TotalBetFourDown = TotalBetFourDown,
                    TotalBetFourUp = TotalBetFourUp,
                    TotalBetThreeDown = TotalBetThreeDown,
                    TotalBetThreeUp = TotalBetThreeUp,

                    TotalEven = TotalEven,
                    TotalOdd = TotalOdd,
                    TotalFourDown = TotalFourDown,
                    TotalFourUp = TotalFourUp,
                    TotalThreeDown = TotalThreeDown,
                    TotalThreeUp = TotalThreeUp,
                    
                    CurrentState = result.CurrentState,
                    Model = _cachePvd.Get<int>(keyHu),
                    Logs = result.CurrentState == MsWebGame.CSKH.Helpers.SeDice.GameState.ShowResult ? SeDiceDAO.Instance.GetSoiCau(15) : new List<SeDiceSoiCau>()
                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetModelXdLive(LuckyDiceModel input)
        {
            //lay danh sách chăm sóc khách hàng
            string keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "Mode");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            int _Model = input.Model;
            _cachePvd.Set(keyHu, _Model);

            return new JsonResult
            {
                //SesionId = result.SessionID,
                //CurrentStates = result.CurrentStates,
                Data = new
                {
                    IsOke = _cachePvd.Get<int>(keyHu) == _Model
                },
                //Dice = new List<int>(result.Dice1, result.Dice2, result.Dice3)
            };
        }


        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetPauseXdLive(LuckyDiceModel input)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("SedieLive", "Pause");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            int _Model = input.Model;
            _cachePvd.Set(keyHu, _Model);

            return new JsonResult
            {
                Data = new
                {
                    IsOke = _cachePvd.Get<int>(keyHu) == _Model
                },
            };
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetResumeXdLive(LuckyDiceModel input)
        {
            string keyHuResume = CachingHandler.Instance.GeneralRedisKey("SedieLive", "Resume");
            RedisCacheProvider _cachePvdResume = new RedisCacheProvider();
            int _Model = input.Model;
            _cachePvdResume.Set(keyHuResume, _Model);

            return new JsonResult
            {
                Data = new
                {
                    IsOke = 1 == 1
                },
            };
        }
    }
}