using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities;
using MsWebGame.CSKH.Models.Games;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;  // For class Encoding

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class SettingController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            return View();
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult FishJpRate()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult SettingFishList(GridCommand command)
        {
            try
            {

                int totalRecord = 50;
                var list = new List<JPRates>();

                var rq1 = JsonConvert.DeserializeObject<dynamic>(Get("http://127.0.0.1:81/bancaapi/getconfig/5f67cff4ff68ad17a534d8c1f1ec6cdd"));
                var dataRate = rq1["data"]["JpThreshold"];

                list.Add(new JPRates {
                    RoomID = 11,
                    JpRate = dataRate["11"]
                });
                list.Add(new JPRates
                {
                    RoomID = 12,
                    JpRate = dataRate["12"]
                });
                list.Add(new JPRates
                {
                    RoomID = 13,
                    JpRate = dataRate["13"]
                });
                list.Add(new JPRates
                {
                    RoomID = 14,
                    JpRate = dataRate["14"]
                });
                list.Add(new JPRates
                {
                    RoomID = 21,
                    JpRate = dataRate["21"]
                });
                list.Add(new JPRates
                {
                    RoomID = 22,
                    JpRate = dataRate["22"]
                });
                list.Add(new JPRates
                {
                    RoomID = 23,
                    JpRate = dataRate["23"]
                });
                list.Add(new JPRates
                {
                    RoomID = 24,
                    JpRate = dataRate["24"]
                });
                list.Add(new JPRates
                {
                    RoomID = 31,
                    JpRate = dataRate["31"]
                });
                list.Add(new JPRates
                {
                    RoomID = 32,
                    JpRate = dataRate["32"]
                });
                list.Add(new JPRates
                {
                    RoomID = 33,
                    JpRate = dataRate["33"]
                });
                list.Add(new JPRates
                {
                    RoomID = 34,
                    JpRate = dataRate["34"]
                });

                //list = list.OrderBy(c => c.GameID).ToList();
                var gridModel = new GridModel<JpRateModel>
                {
                    Data = list.Select(x =>
                    {
                        var m = new JpRateModel();
                        m = Mapper.Map<JpRateModel>(x);
                        return m;
                    }),
                    Total = totalRecord
                };
                return new JsonResult { Data = gridModel };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public void UpdateRateJP(RateJp model)
        {
            try
            {
                var rq1 = JsonConvert.DeserializeObject<dynamic>(Get("http://127.0.0.1:81/bancaapi/getconfig/5f67cff4ff68ad17a534d8c1f1ec6cdd"));



                var JpThreshold = rq1["data"]["JpThreshold"];
                JpThreshold[model.Roomid+""] = model.Rate;
                string json = JsonConvert.SerializeObject(new { JpThreshold = JpThreshold });
                var rq2 = JsonConvert.DeserializeObject<dynamic>(Get("http://127.0.0.1:81/bancaapi/setconfig/5f67cff4ff68ad17a534d8c1f1ec6cdd", "POST", json));

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

        }


        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult AllSettings(GridCommand command, string paramType, string value)
        {
            Session["ParamType"] = paramType;
            Session["Value"] = value;
            //lay danh sách chăm sóc khách hàng
            int TotalRecord = 0;
            if (string.IsNullOrEmpty(paramType))
                paramType = null;

            if (string.IsNullOrEmpty(value))
                value = null;

            var list = ParamConfigDAO.Instance.GetList(paramType, null, value, 1, Int16.MaxValue, out TotalRecord);

            var model = new GridModel<ParamConfig>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult SettingUpdate(ParamConfig model, GridCommand command)
        {
            if (model.Value != null)
                model.Value = model.Value.Trim();
            if (model.Description != null)
                model.Description = model.Description.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }
            int outResponse = 0;
            //lay ra ban ghi can update
            var config = ParamConfigDAO.Instance.ParamConfigGetByID(model.ID);
            if (config != null)
            {
                var paramType = Session["ParamType"].ToString();
                var value = Session["Value"].ToString();
                ParamConfigDAO.Instance.Update(model.ID, model.Value, model.Description, out outResponse);


                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    var gameConfig = GameParamConfigDAO.Instance.ParamConfigGetByKey(config.ParamType, config.Code);
                    if (gameConfig != null)
                    {
                        int gameResponse;
                        GameParamConfigDAO.Instance.Update(gameConfig.ID, model.Value, model.Description, out gameResponse);
                        if (gameResponse != 1)
                        {
                            return Content("Liên hệ quản trị.Đã cấu hình được 1 bảng");
                        }

                    }
                    return AllSettings(command, paramType, value);
                }
                else
                {
                    return Content("Fail");
                }
            }
            else
            {

                return Content("Not find record");

            }

        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult PrivilegeGameInfo()
        {
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListPrivilegeGameInfo(GridCommand command, int gameId)
        {
            if (gameId == 0)
                gameId = -1;

            var list = ParamConfigDAO.Instance.GetListPrivilegeGameInfo(gameId);
            if (list == null)
                list = new List<PrivilegeGameInfo>();

            var model = new GridModel<PrivilegeGameInfo>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegeGameInfoEdit(int id)
        {
            var rs = ParamConfigDAO.Instance.GetListPrivilegeGameInfo(id);
            if (rs == null || rs.Count == 0)
                return RedirectToAction("PrivilegeGameInfo");

            var model = new PrivilegeGameInfo();
            model = rs[0];
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegeGameInfoEdit(PrivilegeGameInfo model)
        {
            model.ConversionCoefficient = Int64.Parse(model.ConversionCoefficientStr.Replace(".", ""));
            int res = ParamConfigDAO.Instance.UpdatePrivilegeGameInfo(model.GameID, model.GameName, model.GameWeight,
                model.ProfitMargin, model.ConversionCoefficient, model.Status);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.Updatesuccess);
                return RedirectToAction("PrivilegeGameInfo");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
                return View(model);
            }
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult PrivilegeLevel()
        {
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GetListPrivilegeLevel(GridCommand command, string privilegeCode)
        {
            if (string.IsNullOrEmpty(privilegeCode))
                privilegeCode = null;

            var list = ParamConfigDAO.Instance.GetListPrivilegeLevel(privilegeCode);
            if (list == null)
                list = new List<PrivilegeLevel>();

            var model = new GridModel<PrivilegeLevel>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegeLevelEdit(string privilegeCode)
        {
            var rs = ParamConfigDAO.Instance.GetListPrivilegeLevel(privilegeCode);
            if (rs == null || rs.Count == 0)
                return RedirectToAction("PrivilegeLevel");

            return View(rs[0]);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegeLevelEdit(PrivilegeLevel model)
        {
            model.VP = Int64.Parse(model.VPStr.Replace(".", ""));
            int res = ParamConfigDAO.Instance.UpdatePrivilegeLevel(model.ID, model.PrivilegeName, model.VP, model.Status);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.Updatesuccess);
                return RedirectToAction("PrivilegeLevel");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
                return View(model);
            }
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult PrivilegePrize()
        {
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GetListPrivilegePrize(GridCommand command, int rankId)
        {
            if (rankId == 0)
                rankId = -1;

            var list = ParamConfigDAO.Instance.GetListPrivilegePrize(rankId);
            if (list == null)
                list = new List<PrivilegePrize>();

            var model = new GridModel<PrivilegePrize>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegePrizeEdit(int id)
        {
            var rs = ParamConfigDAO.Instance.GetListPrivilegePrize(id);
            if (rs == null)
                return RedirectToAction("PrivilegePrize");

            var model = new PrivilegePrize();
            model = rs[0];
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegePrizeEdit(PrivilegePrize model)
        {
            model.MoneyExchangeRate = Int64.Parse(model.MoneyExchangeRateStr.Replace(".", ""));
            int res = ParamConfigDAO.Instance.UpdatePrivilegePrize(model.RankID, model.RefundRate, model.PointExchangeRate,
                model.GiftRate, model.MoneyExchangeRate, model.Status);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.Updatesuccess);
                return RedirectToAction("PrivilegePrize");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
            }

            return View(model);
        }



        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Event()
        {
            ViewBag.CampaignBox = InfoHandler.Instance.GetCampaignBox(0);
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.RoomBox = InfoHandler.Instance.GetRoomBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListEvent(GridCommand command, int campaignId, int gameId, int roomId)
        {
            var list = ParamConfigDAO.Instance.GetListGameEvent(campaignId, gameId, roomId);
            if (list == null)
                list = new List<GameEvent>();

            var model = new GridModel<GameEvent>
            {
                Data = list.PagedForCommand(command),
                Total = list.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult EventAdd()
        {
            ViewBag.CampaignBox = InfoHandler.Instance.GetCampaignBox(0);
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.RoomBox = InfoHandler.Instance.GetRoomBox();
            ViewBag.WeekDayBox = InfoHandler.Instance.GetWeekDayBox();
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult EventAdd(long campaignId, int gameId, int roomId, string jackpotEventInit, string jackpotEventQuota, string jackpotStepJump,
            string eventDay1, string eventDay2, string eventDay3, string eventDay4, string eventDay5, string eventDay6, string eventDay7,
            string eventTime, DateTime effectDate, DateTime expiredDate, bool status, string description)
        {
            ViewBag.CampaignBox = InfoHandler.Instance.GetCampaignBox(0);
            ViewBag.GameBox = InfoHandler.Instance.GetGameBox();
            ViewBag.RoomBox = InfoHandler.Instance.GetRoomBox();
            ViewBag.WeekDayBox = InfoHandler.Instance.GetWeekDayBox();

            string eventDay = string.Empty;
            if (!string.IsNullOrEmpty(eventDay1))
                eventDay += eventDay1 + ",";

            if (!string.IsNullOrEmpty(eventDay2))
                eventDay += eventDay2 + ",";

            if (!string.IsNullOrEmpty(eventDay3))
                eventDay += eventDay3 + ",";

            if (!string.IsNullOrEmpty(eventDay4))
                eventDay += eventDay4 + ",";

            if (!string.IsNullOrEmpty(eventDay5))
                eventDay += eventDay5 + ",";

            if (!string.IsNullOrEmpty(eventDay6))
                eventDay += eventDay6 + ",";

            if (!string.IsNullOrEmpty(eventDay7))
                eventDay += eventDay7 + ",";

            if (!string.IsNullOrEmpty(eventDay))
                eventDay = eventDay.Substring(0, eventDay.Length - 1);

            var model = new GameEvent();
            model.CampaignID = campaignId;
            model.GameID = gameId;
            model.RoomID = roomId;
            model.JackpotEventInitStr = jackpotEventInit;
            model.JackpotEventQuota = Int32.Parse(jackpotEventQuota);
            model.JackpotStepJump = Int32.Parse(jackpotStepJump);
            model.EventDay = eventDay;
            model.EventTime = eventTime;
            model.EffectDate = effectDate;
            model.ExpiredDate = expiredDate;
            model.Status = status;
            model.Description = description;

            if (campaignId < 1 || gameId < 1 || roomId < 1 || roomId > 5 || string.IsNullOrEmpty(jackpotEventInit) || string.IsNullOrEmpty(jackpotEventQuota)
                || string.IsNullOrEmpty(jackpotStepJump) || string.IsNullOrEmpty(eventDay) || string.IsNullOrEmpty(eventTime) || string.IsNullOrEmpty(description))
            {
                return View(model);
            }

            long ijpEventInit = Int64.Parse(jackpotEventInit.Replace(".", ""));
            int ijpEventQuota = Int32.Parse(jackpotEventQuota.Replace(".", ""));
            int ijpStepJump = Int32.Parse(jackpotStepJump.Replace(".", ""));

            int res = ParamConfigDAO.Instance.AddOrUpdateGameEvent(campaignId, gameId, roomId, ijpEventInit, ijpEventQuota, ijpStepJump,
                eventDay, eventTime, effectDate, expiredDate, status, description, AdminID);

            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.InsertSuccess);
                return View();
            }
            else if (res == -30)
            {
                ErrorNotification(Message.PeriodTimeOverlap);
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
            }
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult EventEdit(long campaignId, int gameId, int roomId)
        {
            var rs = ParamConfigDAO.Instance.GetListGameEvent(campaignId, gameId, roomId);
            if (rs == null || rs.Count == 0)
                RedirectToAction("Event");

            var model = new GameEvent();
            model = rs[0];
            ViewBag.WeekDayBox = InfoHandler.Instance.GetWeekDayBox();
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult EventEdit(GameEvent model, string eventDay1, string eventDay2, string eventDay3, string eventDay4, string eventDay5, string eventDay6, string eventDay7)
        {
            if (model == null)
                throw new ArgumentException(Message.ParamaterInvalid);

            string eventDay = string.Empty;
            if (!string.IsNullOrEmpty(eventDay1))
                eventDay += eventDay1 + ",";

            if (!string.IsNullOrEmpty(eventDay2))
                eventDay += eventDay2 + ",";

            if (!string.IsNullOrEmpty(eventDay3))
                eventDay += eventDay3 + ",";

            if (!string.IsNullOrEmpty(eventDay4))
                eventDay += eventDay4 + ",";

            if (!string.IsNullOrEmpty(eventDay5))
                eventDay += eventDay5 + ",";

            if (!string.IsNullOrEmpty(eventDay6))
                eventDay += eventDay6 + ",";

            if (!string.IsNullOrEmpty(eventDay7))
                eventDay += eventDay7 + ",";

            if (!string.IsNullOrEmpty(eventDay))
                eventDay = eventDay.Substring(0, eventDay.Length - 1);

            model.EventDay = eventDay;
            model.JackpotEventInit = Int64.Parse(model.JackpotEventInitStr.Replace(".", ""));
            int res = ParamConfigDAO.Instance.AddOrUpdateGameEvent(model.CampaignID, model.GameID, model.RoomID, model.JackpotEventInit, model.JackpotEventQuota,
                model.JackpotStepJump, model.EventDay, model.EventTime, model.EffectDate, model.ExpiredDate, model.Status, model.Description, AdminID);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.Updatesuccess);
                return RedirectToAction("Event");
            }
            else if (res == -30)
            {
                ErrorNotification(Message.PeriodTimeOverlap);
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
            }

            ViewBag.WeekDayBox = InfoHandler.Instance.GetWeekDayBox();
            return View(model);
        }


        public string Get(string uri, string Method = "GET", string data = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = Method;
            

            if (Method == "POST")
            {
      
                var encodedata = Encoding.ASCII.GetBytes(data);
                request.ContentLength = encodedata.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(encodedata, 0, data.Length);
                }
            }
         

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}