using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models;
using MsWebGame.CSKH.Models.Bot;
using MsWebGame.CSKH.Models.Param;
using MsWebGame.CSKH.Utils;
using MsWebGame.RedisCache.Cache;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class BotController : BaseController
    {
        // GET: Bot
        public ActionResult Index()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult BotSpecialTimeSet()
        {
            TimeSetModel model = new TimeSetModel();
            model.StartTime = DateTime.Now.TimeOfDay;
            model.FinishTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetBotSpecialTimeSetList(GridCommand command, TimeSetModel model)
        {
            int totalRecord = 0;
            var list = BotDAO.Instance.GetBotSpecialTimeSetList(model, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<BotModel>
            {
                Data = list,
                Total = totalRecord
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Permanent()
        {
            ViewBag.PartialBotType = GetBotTypeSelectBox();
            BotModel model = new BotModel();
            model.BotType = 1;
            model.StartTime = DateTime.Now.TimeOfDay;
            model.FinishTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TimeSet()
        {
            ViewBag.PartialBotType = GetBotTypeSelectBox();
            TimeSetModel model = new TimeSetModel();
            model.StartTime = DateTime.Now.TimeOfDay;
            model.FinishTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PermanentSearch(GridCommand command, BotModel model)
        {
            int totalRecord = 0;
            var list = BotDAO.Instance.GetPermanentList(model, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<BotModel>
            {
                Data = list,
                Total = totalRecord
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult PermanentUpdate(BotModel model, GridCommand command)
        {
            int response = BotDAO.Instance.PermanentUpdate(model);
            if (response < 0)
            {
                return Content("Fail");
            }
            return PermanentSearch(command, model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TimeSetSearch(GridCommand command, TimeSetModel model)
        {
            int totalRecord = 0;
            var list = BotDAO.Instance.GetTimeSetList(model, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<TimeSetModel>
            {
                Data = list,
                Total = totalRecord
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult TimeSetUpdate(TimeSetModel model, GridCommand command)
        {
            //int response = BotDAO.Instance.TimeSetUpdate(model);
            //if (response < 0)
            //{
            //    return Content("Fail");
            //}
            return TimeSetSearch(command, model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult BotManager()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListBot(GridCommand command, BotInfo data)
        {
            var list = BotDAO.Instance.GetListBot(data.BotName, data.Type)
                .Select(x => new BotInfo
                {
                    BotId = x.BotId,
                    BotName = x.BotName,
                    DisplayName = x.DisplayName,
                    Balance = x.Balance,
                    BetNumber = x.BetNumber,
                    TotalBet = x.TotalBet,
                    TotalPrize = x.TotalPrize,
                    Delta = x.Delta,
                    Status = x.Status,
                    BotGroupId = x.BotGroupId,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                }).ForCommand(command).ToList();

            var model = new GridModel<BotInfo>
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
        public ActionResult BotInjectMoney()
        {
            BotFundInfo result = new BotFundInfo();
            List<BotFund> lstRs = new List<BotFund>();
            BotFundSets tx = GetBotFundSets();
            //lstRs = BotDAO.Instance.GetListBotFundSet();
            lstRs.Add(new BotFund()
            {
                FundSetID = 1,
                Balance = tx.Balance,
                TotalAddBalance = tx.TotalAddBalance,
                PrizeValue = tx.PrizeValue,
                PrizeMaxWin = tx.PrizeMaxWin,
                PrizeMaxLose = tx.PrizeMaxLose
            });
            result.LstBotFund = lstRs;
            ViewBag.Message = "";
            return View(result);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult BotInjectMoney(BotFundInfo model)
        {
            List<BotFund> lstRs = BotDAO.Instance.GetListBotFundSet();
            model.LstBotFund = lstRs;
            int typeFund = model.TypeFund;
            string PrizeMaxWin = model.PrizeMaxWin;
            string PrizeMaxLose = model.PrizeMaxLose;
            if ((typeFund != 1 && typeFund != 2) || string.IsNullOrEmpty(PrizeMaxWin) || string.IsNullOrEmpty(PrizeMaxLose))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                return View(model);
            }

            long iamountwin = Int64.Parse(PrizeMaxWin.Replace(".", ""));
            if (iamountwin < 1000000)
            {
                ViewBag.Message = "Số tiền tối thiểu là 1.000.000!";
                return View(model);
            }
            long iamountlose = Int64.Parse(PrizeMaxLose.Replace(".", ""));
            if (iamountlose < 1000000)
            {
                ViewBag.Message = "Số tiền tối thiểu là 1.000.000!";
                return View(model);
            }

            AddFundSets(iamountwin , iamountlose);
            //int res = BotDAO.Instance.AllInjectMoney(model.TypeFund, iamount);
            string msg = string.Format("{0}", typeFund == 1 ? "tài xỉu" : "slot");
            SuccessNotification(string.Format("Thay đổi hạn mức {0} thành công!", msg));
            //if (res == 0)
            //    SuccessNotification(string.Format("Bơm tiền {0} thành công!", msg));
            //else
            //    ErrorNotification(string.Format("Bơm tiền {0} thất bại!", msg));

            return RedirectToAction("BotInjectMoney");
        }

        public BotFundSets GetBotFundSets()
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "FundSets");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            if (_cachePvd.Exists(keyHu))
            {
                BotFundSets botFundSets = _cachePvd.Get<BotFundSets>(keyHu);
                if (botFundSets == null)
                {
                    botFundSets = new BotFundSets();
                    botFundSets.TotalAddBalance = 100000000;
                    botFundSets.Balance = 100000000;
                    _cachePvd.Set(keyHu, botFundSets);
                }
                return botFundSets;
            }
            else
            {
                BotFundSets botFundSets = new BotFundSets();
                botFundSets.TotalAddBalance = 100000000;
                botFundSets.Balance = 100000000;
                _cachePvd.Set(keyHu, botFundSets);
                return botFundSets;
            }
        }

        public BotFundSets AddFundSets(long moneywin, long moneylose)
        {
            string keyHu = CachingHandler.Instance.GeneralRedisKey("TaiXiu", "FundSets");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            if (_cachePvd.Exists(keyHu))
            {
                BotFundSets botFundSets = _cachePvd.Get<BotFundSets>(keyHu);
                if (botFundSets == null)
                {
                    botFundSets = new BotFundSets();
                    botFundSets.TotalAddBalance = 0;
                    botFundSets.Balance = 0;
                    botFundSets.PrizeMaxWin = 100000000;
                    botFundSets.PrizeMaxLose = 100000000;
                    _cachePvd.Set(keyHu, botFundSets);
                }
                else
                {
                    botFundSets.PrizeMaxWin = moneywin;
                    botFundSets.PrizeMaxLose = moneylose;
                    _cachePvd.Set(keyHu, botFundSets);
                }
                return botFundSets;
            }
            else
            {
                BotFundSets botFundSets = new BotFundSets();
                botFundSets.TotalAddBalance = 100000000;
                botFundSets.Balance = 100000000;
                _cachePvd.Set(keyHu, botFundSets);
                return botFundSets;
            }
        }

        protected List<ConfigSelect> GetBotTypeSelectBox()
        {
            var lstRs = new List<ConfigSelect>();
            lstRs.Add(new ConfigSelect() { Value = 1, Name = "Bot thường" });
            lstRs.Add(new ConfigSelect() { Value = 2, Name = "Bot đặc biệt" });
            lstRs.Add(new ConfigSelect() { Value = 3, Name = "Bot lên top" });
            return lstRs;
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult TimelineBotSpecial()
        {

            ViewBag.PartialByResult = GetByResultSelectBox();
            BotModel model = new BotModel();
            model.ByResult = 1;
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now.Add(TimeSpan.FromHours(1));
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTimelineBotSpecialList(GridCommand command, BotModel model)
        {
            int totalRecord = 0;
            var list = BotDAO.Instance.GetTimelineSpecialList(model, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<BotBet>
            {
                Data = list,
                Total = totalRecord
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult BotTransferToBot()
        {
            if (!CheckPermissionUser(AdminAccountName))
                return RedirectToAction("Permission", "Account");

            ParsPhatLocTransfer model = new ParsPhatLocTransfer();
            ViewBag.Message = Session["BotTransferToBot"] != null ? Session["BotTransferToBot"].ToString() : string.Empty;
            Session["BotTransferToBot"] = null;
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult BotTransferToBot(ParsPhatLocTransfer input)
        {
            if (!CheckPermissionUser(AdminAccountName))
                return RedirectToAction("Permission", "Account");

            if (string.IsNullOrEmpty(input.phatLocName) || string.IsNullOrEmpty(input.receiverNameList)
                || string.IsNullOrEmpty(input.amount) || string.IsNullOrEmpty(input.note))
            {
                ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                return View(input);
            }

            long iamount = Int64.Parse(input.amount.Replace(".", ""));
            if (iamount < 10000)
            {
                ViewBag.Message = "Số tiền nhận tối thiểu là 10.000!";
                return View(input);
            }

            //Get PhatLocId
            var plInfo = PhatLocDAO.Instance.GetUsersCheckBot(input.phatLocName);
            if (plInfo == null || plInfo.AccountID >= 0)
            {
                ViewBag.Message = "Tài khoản phát lộc không tồn tại";
                return View(input);
            }

            //cut receiverNameList
            string lstName = StringUtil.RemoveLastStr(input.receiverNameList);
            string[] ArrReceiverName = lstName.Split(',');

            string nickNameSuccess = string.Empty;
            string nickNameError = string.Empty;
            var len = ArrReceiverName.Length;
            if (len > 10)
                len = 10;

            for (int i = 0; i < len; i++)
            {
                Thread.Sleep(500);
                //Get ReceiverId
                var accInfo = PhatLocDAO.Instance.GetUsersCheckBot(ArrReceiverName[i]);
                if (accInfo == null || plInfo.AccountID >= 0)
                {
                    NLogManager.LogMessage(string.Format("BotTransferToBot-NotExist:{0}", ArrReceiverName[i]));
                    continue;
                }
                if (plInfo.AccountID == accInfo.AccountID)
                {
                    NLogManager.LogMessage(string.Format("BotTransferToBot-Không thể chuyển cho chính mình:{0}", ArrReceiverName[i]));
                    continue;
                }

                long transId = 0;
                long wallet = 0;
                int response = BotDAO.Instance.BotTransferToBot(plInfo.AccountID, accInfo.AccountID, iamount, input.note, out transId, out wallet);
                NLogManager.LogMessage(string.Format("BotTransferToBot-PhatLocName:{0}| ReceiverName:{1}| iamount:{2}| note:{3}| TransId:{4}| Response:{5}",
                    plInfo.AccountName, ArrReceiverName[i], iamount, input.note, transId, response));
                if (response > 0)
                    nickNameSuccess += ArrReceiverName[i] + ",";
                else
                    nickNameError += ArrReceiverName[i] + ",";
            }

            string msgSuccess = string.Empty;
            if (!string.IsNullOrEmpty(nickNameSuccess))
                msgSuccess = string.Format("Chuyển khoản cho tài khoản {0} thành công.", StringUtil.RemoveLastStr(nickNameSuccess));

            string msgError = string.Empty;
            if (!string.IsNullOrEmpty(nickNameError))
                msgError = string.Format("Chuyển khoản cho tài khoản {0} thất bại!", StringUtil.RemoveLastStr(nickNameError));

            string msg = string.Format("{0} {1}", msgSuccess, msgError);
            Session["BotTransferToBot"] = msg;
            return RedirectToAction("BotTransferToBot");
        }

        protected List<ConfigSelect> GetByResultSelectBox()
        {
            var lstRs = new List<ConfigSelect>();
            lstRs.Add(new ConfigSelect() { Value = 1, Name = "Được nhiều" });
            lstRs.Add(new ConfigSelect() { Value = 2, Name = "Mất nhiều" });
            return lstRs;
        }
    }
}