using MsWebGame.CSKH.App_Start;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Utils;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class ManagerController : BaseController
    {
        #region Artifacts
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Artifacts()
        {
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListArtifacts(GridCommand command, string artifactCode, string artifactName, bool? status, DateTime createDate)
        {
            int totalrecord = 0;
            var list = ManagerDAO.Instance.GetListArtifacts(0, artifactCode, artifactName, status, createDate,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<Artifacts>();

            var model = new GridModel<Artifacts>
            {
                Data = list,
                Total = totalrecord
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ArtifactsAdd()
        {
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ArtifactsAdd(string artifactsCode, string artifactsName, string price, bool status, string description)
        {
            var model = new Artifacts();
            model.ArtifactCode = artifactsCode;
            model.ArtifactName = artifactsName;
            model.PriceStr = price;
            model.Status = status;
            model.Description = description;

            if (string.IsNullOrEmpty(artifactsCode) || string.IsNullOrEmpty(artifactsName) || string.IsNullOrEmpty(price) || string.IsNullOrEmpty(description))
                return View(model);

            int iprice = Int32.Parse(price.Replace(".", ""));
            int res = ManagerDAO.Instance.AddOrUpdateArtifacts(0, artifactsCode, artifactsName, iprice, status, description, AdminID, DateTime.Now);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.InsertSuccess);
                return RedirectToAction("Artifacts");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
            }

            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ArtifactsEdit(int id)
        {
            int totalrecord = 0;
            var rs = ManagerDAO.Instance.GetListArtifacts(id, null, null, true, null, 1, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (rs == null)
                return RedirectToAction("Artifacts");

            var model = new Artifacts();
            model = rs[0];
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ArtifactsEdit(Artifacts model)
        {
            model.Price = Int64.Parse(model.PriceStr.Replace(".", ""));
            int res = ManagerDAO.Instance.AddOrUpdateArtifacts(model.ArtifactID, model.ArtifactCode, model.ArtifactName,
                model.Price, model.Status, model.Description, model.CreateUser, model.CreateDate);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.Updatesuccess);
                return RedirectToAction("Artifacts");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
                return View(model);
            }
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult PrivilegeArtifacts()
        {
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListPrivilegeArtifacts(GridCommand command, int privilegeId, string artifactName, DateTime createDate)
        {
            int totalrecord = 0;
            var list = ManagerDAO.Instance.GetListPrivilegeArtifacts(0, privilegeId, artifactName, createDate,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<PrivilegeArtifacts>();

            var model = new GridModel<PrivilegeArtifacts>
            {
                Data = list,
                Total = totalrecord
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegeArtifactsAdd()
        {
            ViewBag.ArtifactsBox = InfoHandler.Instance.GetArtifactsBox();
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegeArtifactsAdd(int privilegeId, int artifactId, string price, string quantity, string remainQuantity,
            string totalPrize, bool status, string description)
        {
            ViewBag.ArtifactsBox = InfoHandler.Instance.GetArtifactsBox();
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            ViewBag.Price = price;

            var model = new PrivilegeArtifacts();
            model.PrivilegeID = privilegeId;
            model.ArtifactID = artifactId;
            model.QuantityStr = quantity;
            model.RemainQuantityStr = remainQuantity;
            model.TotalPrizeStr = totalPrize;
            model.Status = status;
            model.Description = description;

            if (privilegeId < 1 || privilegeId > 5 || artifactId < 1 || string.IsNullOrEmpty(quantity) || string.IsNullOrEmpty(totalPrize) || string.IsNullOrEmpty(description))
                return View(model);

            int quan = Int32.Parse(quantity.Replace(".", ""));
            long total = Int64.Parse(totalPrize.Replace(".", ""));
            int res = ManagerDAO.Instance.AddOrUpdatePrivilegeArtifacts(0, privilegeId, artifactId, quan, quan, total, status, description, AdminID);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.InsertSuccess);
                return RedirectToAction("PrivilegeArtifacts");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
            }

            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult PrivilegeArtifactsEdit(long id)
        {
            int totalrecord = 0;
            var rs = ManagerDAO.Instance.GetListPrivilegeArtifacts(id, 0, null, null, 1, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (rs == null)
                return RedirectToAction("PrivilegeArtifacts");

            var model = new PrivilegeArtifacts();
            model = rs[0];
            ViewBag.Price = model.TotalPrize / model.Quantity;
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = "ADMIN")]
        public ActionResult PrivilegeArtifactsEdit(PrivilegeArtifacts model)
        {
            model.Quantity = Int32.Parse(model.QuantityStr.Replace(".", ""));
            model.RemainQuantity = Int32.Parse(model.RemainQuantityStr.Replace(".", ""));
            model.TotalPrize = Int64.Parse(model.TotalPrizeStr.Replace(".", ""));

            int res = ManagerDAO.Instance.AddOrUpdatePrivilegeArtifacts(model.PriArtID, model.PrivilegeID, model.ArtifactID, model.Quantity,
                model.RemainQuantity, model.TotalPrize, model.Status, model.Description, model.CreateUser);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.Updatesuccess);
                return RedirectToAction("PrivilegeArtifacts");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
                return View(model);
            }
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult UserRedemption()
        {
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListUserRedemption(GridCommand command, string userName, int privilegeId, DateTime createDate, int serviceId)
        {
            int totalrecord = 0;
            var list = ManagerDAO.Instance.GetListUserRedemption(0, 0, userName, privilegeId, createDate, serviceId,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (list == null)
                list = new List<UserRedemption>();

            var model = new GridModel<UserRedemption> { Data = list, Total = totalrecord };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult UserRedemptionEdit(long id)
        {
            if (id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);

            int totalrecord = 0;
            var rs = ManagerDAO.Instance.GetListUserRedemption(id, 0, null, 0, null, 0, 1, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (rs == null)
                throw new ArgumentException(Message.ParamaterInvalid);

            var model = new UserRedemption();
            model = rs[0];
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = "ADMIN")]
        public ActionResult UserRedemptionEdit(UserRedemption model)
        {
            if (model == null)
                throw new ArgumentException(Message.ParamaterInvalid);

            int res = ManagerDAO.Instance.AddOrUpdateUserRedemption(model.id, model.UserID, model.RefundAmount, model.PriArtID,
                model.Quantity, model.PrivilegeID, model.VP, model.Point, model.RankedMonth, model.Status, model.Description,
                model.RefundReceiveDate, model.GiftReceiveDate, model.UserID);
            if (res == AppConstants.DBS.SUCCESS)
            {
                SuccessNotification(Message.Updatesuccess);
                return RedirectToAction("UserRedemption");
            }
            else
            {
                ErrorNotification(Message.SystemProccessing);
                return View(model);
            }
        }
        #endregion

        public string GetArtifactsPrice(int id)
        {
            int totalrecord = 0;
            string val = string.Empty;
            var lstData = ManagerDAO.Instance.GetListArtifacts(id, null, null, true, null, 1, AppConstants.CONFIG.GRID_SIZE, out totalrecord);
            if (lstData != null && lstData.Any())
                val = lstData[0].PriceFormat;

            return val;
        }

        private DateTime ConvertToDateTime(string inputvalue)
        {
            if (string.IsNullOrEmpty(inputvalue))
                return new DateTime();

            string date = inputvalue.Substring(4, 11);
            var rs = DateTime.ParseExact(date, "MMM dd yyyy", CultureInfo.InvariantCulture);
            return rs;
        }
    }
}