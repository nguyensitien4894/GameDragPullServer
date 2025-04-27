using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.GiftCodes;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{

    [AllowedIP]
    public class GiftCodeController : BaseController
    {

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GiftCodeCheck()
        {


            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListGiftListCodeCheck(GridCommand command, string giftCode)
        {

            if (string.IsNullOrEmpty(giftCode))
                giftCode = null;


            var list = GiftCodeDAO.Instance.GetListGameGiftCodeCheck(giftCode);
            var model = new GridModel<GameGiftCode>
            {
                Data = list,
                Total = 1
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Index()
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != "kinhdoanh")
            {
                return RedirectToAction("Permission", "Account");
            }
            ViewBag.RankBox = InfoHandler.Instance.GetRankBox();
            //ViewBag.CampaignBox = InfoHandler.Instance.GetCampaignBox(1);

            ViewBag.ServiceBox = GetServices();
            ViewBag.RoleCode = Session["RoleCode"].ToString();
            return View();
        }

        public ActionResult GetCampainByServiceId(int ServiceID)
        {
            var list = InfoHandler.Instance.GetCampaignBox(ServiceID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult GetCampaignAutoComplete(string name,int ServiceID)
        {
            var list = GiftCodeDAO.Instance.GetCampaignAutoComplete(name,ServiceID);
            var lstData = list.Select(c => new
            {
                c.CampaignID,
                c.CampaignName,
            });
            return Json(lstData, JsonRequestBehavior.AllowGet);
        }
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListGiftCode(GridCommand command, string campaignName, int giftCodeType, string giftCode, bool? status, string createdUser,
            string nickName, DateTime? fromDate, DateTime? toDate, int accountType, int serviceId = 1)
        {
            if ((string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMIN && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINREF && (string)Session["UserName"] != AppConstants.ADMINUSER.USER_ADMINTEST && (string)Session["UserName"] != "kinhdoanh")
            {
                return RedirectToAction("Permission", "Account");
            }

            if (string.IsNullOrEmpty(giftCode))
                giftCode = null;

            int totalRecord = 0;
            long accountId = 0;
            if (!string.IsNullOrEmpty(createdUser))
            {
                if (accountType == 1)
                {
                    var lstAdmin = AdminDAO.Instance.GetList(1, createdUser, null, serviceId);
                    if (lstAdmin != null && lstAdmin.Count > 0)
                        accountId = lstAdmin[0].AccountID;
                }
                else
                {
                    var accInfo = UserDAO.Instance.GetAccountByNickName(createdUser, serviceId);
                    if (accInfo != null)
                        accountId = accInfo.AccountID;
                }
            }


            var list = GiftCodeDAO.Instance.GetListGameGiftCode2(accountId, accountType, giftCodeType, campaignName, giftCode, status, nickName, serviceId, fromDate, toDate,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var model = new GridModel<GameGiftCode>
            {
                Data = list,
                Total = totalRecord
            };

            return new JsonResult
            {
                Data = model
            };
        }


        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GetGiftcodeProgress(string nickName = null, int accountType = 0, int ServiceID = 1)
        {
            long accountId = 0;
            if (!string.IsNullOrEmpty(nickName))
            {
                if (accountType == 1)
                {
                    var lstAdmin = AdminDAO.Instance.GetList(1, nickName, null, ServiceID);
                    if (lstAdmin != null && lstAdmin.Any())
                        accountId = lstAdmin[0].AccountID;
                }
                else
                {
                    var accInfo = UserDAO.Instance.GetAccountByNickName(nickName, ServiceID);
                    if (accInfo != null)
                        accountId = accInfo.AccountID;
                }
            }

            var rs = GiftCodeDAO.Instance.GetGiftcodeProgress(accountId, accountType, ServiceID);
            return PartialView(rs);
        }


        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult GiftCodeAdd()
        {
            GiftCodeAdMode model = new GiftCodeAdMode();
            PrepareGiftCodeAd(model);
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult GiftCodeAdd(GiftCodeAdMode model)
        {
            var Admin = AdminDAO.Instance.GetById(AdminID);
            if (model.ExpireDate < DateTime.Now)
            {
                ModelState.AddModelError("ExpireDate", Message.GiftCodeExpireDateMin);
            }
            if (model.Quantity < 0 || model.Quantity > 500001)
            {
                ModelState.AddModelError("Quantity", "Số lượng gift code từ 1-500000");
            }


            var lngMoneyReward = Convert.ToInt64(model.MoneyReward.Replace(".", ""));
            var lostMoney = lngMoneyReward * model.Quantity;

            PrepareGiftCodeAd(model);
            if (Admin.Wallet < lostMoney)
            {
                ModelState.AddModelError("Quantity", Message.AdminNotEnoughMoneyToCreateGiftCode);
            }

            if (ModelState.IsValid)
            {
                int Repsonse = 0;
                long TotalMoneyUsed = 0;
                long CampaignID = 0;
                long Wallet = -1;
                //AdminDAO.Instance.AdminWalletExchange(AdminID, lostMoney * -1, out Wallet);
                //if (Wallet <= 0)
                //{
                //    ModelState.AddModelError("Quantity", Message.AdminNotEnoughMoneyToCreateGiftCode);
                //    return View(model);
                //}

                //ghi log trừ tiền
                //string paramType = "REASON";
                //string feeCode = "ADMIN_CREATE_GIFT_CODE";
                //int reasonId = ParamConfigDAO.Instance.GetConfigValue(paramType, feeCode);
                //string des = "Trừ tiền admin khi tạo gift code";
                //int resLog = AdminDAO.Instance.WalletLogsCreate(AdminID, 1, 1, Admin.Wallet.Value, lostMoney * -1, Wallet, reasonId, 1, 0, des, DateTime.Now, AdminID, model.serviceId);

                //Tạo giftcode
                //GiftCodeDAO.Instance.GiftcodeCreate(1, model.GiftcodeType, model.Quantity, model.CampaignName, model.Description, model.ExpireDate,
                //    lngMoneyReward, AdminID, model.serviceId, out Repsonse, out TotalMoneyUsed, out CampaignID);

                var res = GiftCodeDAO.Instance.GiftcodeGenerate(AdminID, 1, 1, model.CampaignName, model.GiftcodeType, lngMoneyReward, model.ExpireDate, model.Description, model.Quantity, model.serviceId,
                    out TotalMoneyUsed, out CampaignID, out Wallet);
                if (res == AppConstants.DBS.SUCCESS)
                {
                    string paramType = "REASON";
                    string feeCode = "ADMIN_CREATE_GIFT_CODE";
                    int reasonId = ParamConfigDAO.Instance.GetConfigValue(paramType, feeCode);
                    string des = "Trừ tiền admin khi tạo gift code";
                    int resLog = AdminDAO.Instance.WalletLogsCreate(AdminID, 1, 1, Admin.Wallet.Value, lostMoney * -1, Wallet, reasonId, 1, 0, des, DateTime.Now, AdminID, model.serviceId);

                    SuccessNotification(Message.InsertSuccess);
                    return RedirectToAction("GiftCodeAdd");
                }
                else
                {
                    //long walletRefund = -1;
                    //AdminDAO.Instance.AdminWalletExchange(AdminID, lostMoney, out walletRefund);
                    //ghi log trừ tiền
                    //string paramType2 = "REASON";
                    //string feeCode2 = "ADMIN_CREATE_GIFTCODE_FAIL";
                    //int reasonId2 = ParamConfigDAO.Instance.GetConfigValue(paramType2, feeCode2);
                    //string des2 = "Hoàn tiền khi không tạo được gift code";
                    //int resLog2 = AdminDAO.Instance.WalletLogsCreate(AdminID, 1, 1, Wallet, lostMoney, walletRefund, reasonId2, -1, 0, des2, DateTime.Now, AdminID, model.serviceId);
                    string message = MessageConvetor.MsgGiftCode.GetMessageGiftCodeAdd(res);
                    ErrorNotification(message);
                }
            }

            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult GiftCodeEdit(long id, long campaignId, string giftcode, bool status)
        {
            if (id <= 0 || campaignId <= 0 || string.IsNullOrEmpty(giftcode))
                throw new ArgumentException(Message.ParamaterInvalid);
            int totalRecord = 0;
            var rs = GiftCodeDAO.Instance.GetListGameGiftCode(0, 0, 0, campaignId, giftcode, status, 1, 50, out totalRecord);
            if (rs == null)
                throw new ArgumentException(Message.ParamaterInvalid);

            var model = new GameGiftCode();
            model = rs[0];
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult GiftCodeEdit(GameGiftCode model)
        {
            if (model == null)
                throw new ArgumentException(Message.ParamaterInvalid);

            if (ModelState.IsValid)
            {
                int res = GiftCodeDAO.Instance.AddOrUpdateGameGiftCode(model.ID, model.CampaignID, model.GiftCode, model.MoneyReward,
                    model.GiftCodeType, model.TotalUsed, model.LimitQuota, AdminID, model.Status, model.ExpiredDate);
                if (res == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("GiftCode");
                }
                else if (res == -1)
                {
                    ErrorNotification(Message.CSKHExist);
                    return View(model);
                }
                else
                {
                    ErrorNotification(Message.SystemProccessing);
                    return View(model);
                }
            }

            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ExportExcel(string campaignName, int giftCodeType, string giftCode, bool? status, string createdUser, string nickName, DateTime? fromDate, DateTime? toDate, int accountType, int serviceId = 1)
        {
            string _fileName = "ExportExcel_";
            string _sheetName = "Sheet";
            IWorkbook _workbook;
            ISheet _sheet;

            _workbook = new XSSFWorkbook();
            _sheet = _workbook.CreateSheet(_sheetName);

            if (string.IsNullOrEmpty(giftCode))
                giftCode = null;

            int totalRecord = 0;

            long accountId = 0;
            if (!string.IsNullOrEmpty(createdUser))
            {
                if (accountType == 1)
                {
                    var lstAdmin = AdminDAO.Instance.GetList(1, createdUser, null, ServiceID);
                    if (lstAdmin != null && lstAdmin.Count > 0)
                        accountId = lstAdmin[0].AccountID;
                }
                else
                {
                    var accInfo = UserDAO.Instance.GetAccountByNickName(createdUser, ServiceID);
                    if (accInfo != null)
                        accountId = accInfo.AccountID;
                }
            }
            else
            {
                accountType = 0;
            }
            // var lstRs = GiftCodeDAO.Instance.GetGiftCodeExport(0, 0, giftCodeType, campaignId, giftCode, status, 1, 5000, out totalRecord);
            var lstRs = GiftCodeDAO.Instance.GetListGameGiftCode2(accountId, accountType, giftCodeType, campaignName, giftCode, status, nickName, serviceId, fromDate, toDate,
                1, 500000, out totalRecord);

            DataTable table = new DataTable();
            table = ExcelHelpers.ListToDataTable(lstRs);

            ExcelHelpers.WriteExcelWithNPOI(_workbook, _sheet, table);

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                _workbook.Write(exportData);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", _fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx"));
                Response.BinaryWrite(exportData.ToArray());
                Response.End();
            }

            return null;
        }

        private void PrepareGiftCodeAd(GiftCodeAdMode model)
        {
            ViewBag.ServiceBox = GetServices();
            model.listGiftTypes = new List<SelectListItem>
            {
                new SelectListItem() {Value="1",Text="Gift code(1 tài khoản dùng 1 gift code" },
                new SelectListItem() {Value="2",Text="Gift code(1 tài khoản dùng nhiều gift code" }
            };
        }
    }
}