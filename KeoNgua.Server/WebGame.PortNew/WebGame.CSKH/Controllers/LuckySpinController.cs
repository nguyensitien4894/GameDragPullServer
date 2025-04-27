using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.LuckySpin;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class LuckySpinController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult PresentSpins()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult DBit()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult FreeSpin()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult FreeSpinUpdate(FreeSpinModel model, GridCommand command)
        {
            int response = LuckySpinDAO.Instance.UpdateFreeSpin(model);
            if (response < 0)
            {
                return Content("Fail");
            }
            else
            {
                return GetFreeSpinList(command);
            }
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetFreeSpinList(GridCommand command,int ServiceID=1)
        {
            //lay danh sách chăm sóc khách hàng
            var list = LuckySpinDAO.Instance.GetFreeSpinList(ServiceID);
            var gridModel = new GridModel<FreeSpinModel>
            {
                Data = list.Select(x =>
                {
                    var m = new FreeSpinModel();
                    m.FreeSpinID = x.FreeSpinID;
                    m.Name = x.Name;
                    m.RoomID = x.RoomID;
                    m.SpinQuantity = x.SpinQuantity;
                    m.New = x.New;
                    m.Stone = x.Stone;
                    m.BronzeSilver = x.BronzeSilver;
                    m.GoldDiamond = x.GoldDiamond;
                    m.ServiceID = x.ServiceID;
                    return m;
                }),
                Total = list.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        /// <summary>
        /// Danh sách giải vòng quay lớn
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetDBitList(GridCommand command,int serviceid=1)
        {
            //lay danh sách chăm sóc khách hàng
            var list = LuckySpinDAO.Instance.GetDBitList(serviceid);
            var gridModel = new GridModel<DBitModel>
            {
                Data = list.Select(x =>
                {
                    var m = new DBitModel();
                    m.PrizeID = x.PrizeID;
                    m.Name = x.Name;
                    m.PrizeValue = x.PrizeValue;
                    m.New = x.New;
                    m.Stone = x.Stone;
                    m.BronzeSilver = x.BronzeSilver;
                    m.GoldDiamond = x.GoldDiamond;
                    m.ServiceID = x.ServiceID;
                    return m;
                }),
                Total = list.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult DBitUpdate(DBitModel model, GridCommand command)
        {
            int response = LuckySpinDAO.Instance.UpdateDBit(model);
           
            if (response < 0)
            {
                return Content("Fail");
            }
            else
            {
                return GetDBitList(command,model.ServiceID);
            }
        }

        /// <summary>
        /// thêm mới cấu hình số lượt quay 
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CreatePresentSpins()
        {
            PresentSpinsModel model = new PresentSpinsModel();
            ViewBag.ServiceBox = GetServices();
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost]
        public ActionResult InsertPresentSpins(PresentSpinsModel model, int serviceId = 1)
        {

            int response = LuckySpinDAO.Instance.InsertPresentSpins(model, serviceId);
            ViewBag.ServiceBox = GetServices();

            if (response < 0)
            {
                string message = MessageConvetor.MsgLuckySpin.GetMessage(response);
                ErrorNotification(message);
                return View("CreatePresentSpins", model);
            }
            SuccessNotification("Thêm mới thành công");
            return RedirectToAction("PresentSpins", "LuckySpin");
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpGet]
        public ActionResult DeleteConfigPresentSpins(PresentSpinsModel model)
        {
            int response = LuckySpinDAO.Instance.DeletePresentSpins(model);
            if (response < 0)
            {
                string message = MessageConvetor.MsgLuckySpin.GetMessage(response);
                ErrorNotification(message);
            }
            SuccessNotification("Xóa thành công");
            return RedirectToAction("PresentSpins", "LuckySpin");
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetPresentSpinsList(GridCommand command,int serviceId=1)
        {
            //lay danh sách chăm sóc khách hàng
            var list = LuckySpinDAO.Instance.GetPresentSpinsList(serviceId);
            var gridModel = new GridModel<PresentSpinsModel>
            {
                Data = list.Select(x =>
                {
                    var m = new PresentSpinsModel();
                    m.ID = x.ID;
                    m.Quantity = x.Quantity;
                    m.StartDate = x.StartDate;
                    m.EndDate = x.EndDate;
                    m.CreatedDate = x.CreatedDate;
                    return m;
                }),
                Total = list.Count()
            };
            return new JsonResult { Data = gridModel };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult DBitReport(GridCommand command)
        {
            ViewBag.PartialRanks = GetRankList();
            ViewBag.PartialPrizes = GetPrizesList();
            ReportModel model = new ReportModel();
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            model.Rank = -1;
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult DBitReportSearch(GridCommand command, ReportModel data)
        {
            int totalRecord = 0;
            var list = LuckySpinDAO.Instance.GetDBitReport(data, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            if(list == null)
                return new JsonResult { Data = null };

            var gridModel = new GridModel<ReportModel>
            {
                Data = list.Select(x =>
                {
                    var m = new ReportModel();
                    m.PrizeID = x.PrizeID;
                    m.Name = x.Name;
                    m.PrizeValue = x.PrizeValue;
                    m.DateSpin = x.DateSpin;
                    m.RankName = x.RankName;
                    m.ReciveAward = x.ReciveAward;
                    m.AwardLimit = x.AwardLimit;
                    m.AwardRemain = x.AwardLimit - x.ReciveAward;
                    return m;
                }),
                Total = totalRecord
            };

            return new JsonResult { Data = gridModel };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult FreeSpinReport(GridCommand command)
        {
            ViewBag.PartialRanks = GetRankList();
            ViewBag.PartialFreeSpins = GetFreeSpinsList();
            ReportModel model = new ReportModel();
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            model.Rank = -1;
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult FreeSpinReportSearch(GridCommand command, ReportModel data)
        {
            int totalRecord = 0;
            var list = LuckySpinDAO.Instance.GetFreeSpinReport(data, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            if (list == null)
                return new JsonResult { Data = null };

            var gridModel = new GridModel<ReportModel>
            {
                Data = list.Select(x =>
                {
                    var m = new ReportModel();
                    m.FreeSpinID = x.FreeSpinID;
                    m.Name = x.Name;
                    m.DateSpin = x.DateSpin;
                    m.RankName = x.RankName;
                    m.ReciveAward = x.ReciveAward;
                    m.AwardLimit = x.AwardLimit;
                    m.AwardRemain = x.AwardLimit - x.ReciveAward;
                    return m;
                }),
                Total = totalRecord
            };

            return new JsonResult { Data = gridModel };
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult AccountReport(GridCommand command)
        {
            ViewBag.PartialRanks = GetRankList();
            ViewBag.PartialFreeSpins = GetFreeSpinsList();
            ViewBag.PartialPrizes = GetPrizesList();
            ViewBag.ServiceBox = GetServices();
            ReportModel model = new ReportModel();
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            model.Rank = -1;
            return View(model);
        }

        [AdminAuthorize(Roles =ADMIN_ALL_ROLE)]
        public ActionResult AccountReportSearch(GridCommand command, ReportModel data)
        {
            int totalRecord = 0;
            var list = LuckySpinDAO.Instance.GetAccountReport(data, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<ReportModel>
            {
                Data = list.Select(x =>
                {
                    var m = new ReportModel();
                    m.SpinID = x.SpinID;
                    m.DateSpin = x.CreatedTime;
                    m.Username = x.Username;
                    m.RankName = x.RankName;
                    m.PrizeName = x.PrizeName;
                    m.FreeSpinName = x.FreeSpinName;
                    return m;
                }),
                Total = totalRecord
            };

            return new JsonResult { Data = gridModel };
        }


        private List<RankUser> GetRankList()
        {
            var lstRs = new List<RankUser>();
            var list = LuckySpinDAO.Instance.GetRankUserList();
            lstRs.Add(new RankUser()
            {
                Rank = -1,
                RankName = "Tất cả"
            });
            lstRs.AddRange(list);
            return lstRs;
        }

        private List<DBit> GetPrizesList()
        {
            var lstRs = new List<DBit>();
            var list = LuckySpinDAO.Instance.GetPrizeList();
            lstRs.Add(new DBit()
            {
                PrizeID = 0,
                Name = "Tất cả"
            });
            lstRs.AddRange(list);
            return lstRs;
        }

        private List<FreeSpin> GetFreeSpinsList()
        {
            var lstRs = new List<FreeSpin>();
            var list = LuckySpinDAO.Instance.GetFreeSpinsList();
            lstRs.Add(new FreeSpin()
            {
                FreeSpinID = 0,
                Name = "Tất cả"
            });
            lstRs.AddRange(list);
            return lstRs;
        }
    }
}