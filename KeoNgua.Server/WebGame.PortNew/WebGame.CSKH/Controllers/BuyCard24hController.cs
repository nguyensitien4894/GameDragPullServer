using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using TraditionGame.Utilities;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Utils;
using MsWebGame.CSKH.Helpers.Muathe24;
using MsWebGame.CSKH.Helpers.MuaTheHDP;

namespace MsWebGame.CSKH.Controllers
{
    public class BuyCard24hController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Index()
        {
            if (!CheckPermissionUser(AdminAccountName))
                return RedirectToAction("Permission", "Account");

            ResultImportCardList model = new ResultImportCardList();
            model.ParsBuyCard = new ParsBuyCard24H();
            ViewBag.Message = Session["BuyCard24h"] != null ? Session["BuyCard24h"].ToString() : string.Empty;
            Session["BuyCard24h"] = null;
            model.ParsBuyCard.trace = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Index(ParsBuyCard24H input)
        {
            if (!CheckPermissionUser(AdminAccountName))
                return RedirectToAction("Permission", "Account");

            ResultImportCardList model = new ResultImportCardList();
            model.ParsBuyCard = input;
            string msg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(input.trace) || string.IsNullOrEmpty(input.telco) || input.amount < 10000
                || input.amount > 500000 || input.quantity < 1)
                {
                    ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
                    model.ParsBuyCard.trace = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    return View(model);
                }

                //string username = Config.GetAppSettings("USERNAME24H", true, "");
                //string password = Config.GetAppSettings("PASSWORD24H", true, "");
                //vn.muathe24h.UserCredentials user = new vn.muathe24h.UserCredentials();
                //user.userName = username;
                //user.pass = password;
                //vn.muathe24h.MechantServices hulft = new vn.muathe24h.MechantServices();
                //hulft.UserCredentialsValue = user;
                //var result = hulft.BuyCards(input.trace, input.telco, input.amount, input.quantity);
                
                var result = MuaTheHDPApi.BuyCard(input.telco, input.amount, input.quantity);
                NLogManager.LogMessage(string.Format("BuyCardHDP -Tran:{4}| Telco:{0}| Amount:{1}| Quantity:{2}| Response:{3}",
                    input.telco, input.amount, input.quantity, result.status, input.trace));
                msg = result.msg;

                if (result.status != 1)
                {
                    ViewBag.Message = msg;
                    return View(model);
                }

                List<ImportCardInfo> lstImportCard = new List<ImportCardInfo>();
                if (result.cards != null)
                {
                  
                    var i = 0;
                    foreach (var item in result.cards)
                    {
                        ImportCardInfo cardInfo = new ImportCardInfo();
                        i++;
                        cardInfo.ID = i;
                        cardInfo.CardValue = item.cardvalue;
                        cardInfo.CardNumber = StringUtil.FilterInjectionChars(item.cardcode);
                        cardInfo.CardSerial = item.cardseri;
                        cardInfo.ExperiedDate = DateTime.ParseExact(item.cardvexpdate, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        cardInfo.TelOperatorID = input.telco == "VTT" ? 1 : input.telco == "VNP" ? 2 : input.telco == "VMS" ? 3 : 0;
                        cardInfo.Status = 1;
                        cardInfo.ImportDate = DateTime.Now;
                        lstImportCard.Add(cardInfo);
                    }

                    DataTable dt = ExcelHelpers.ListToDataTable(lstImportCard);
                    int response = 0;
                    var lstResult = CardDAO.Instance.ImportCard(dt, out response);
                    if (lstResult != null && lstResult.Any())
                    {
                        model.LstSuccess = lstResult.Where(c => c.Result == true).ToList();
                        model.LstError = lstResult.Where(c => c.Result == false).ToList();
                    }
                }

                Session["BuyCard24h"] = msg;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                msg = ex.Message;
            }

            ViewBag.Message = msg;
            return View(model);
        }


        //[HttpPost]
        //[AdminAuthorize(Roles = ADMIN_ROLE)]
        //public ActionResult Index(ParsBuyCard24H input)
        //{
        //    if (!CheckPermissionUser(AdminAccountName))
        //        return RedirectToAction("Permission", "Account");

        //    ResultImportCardList model = new ResultImportCardList();
        //    model.ParsBuyCard = input;
        //    string msg = string.Empty;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(input.trace) || string.IsNullOrEmpty(input.telco) || input.amount < 10000
        //        || input.amount > 500000 || input.quantity < 1)
        //        {
        //            ViewBag.Message = "Dữ liệu đầu vào không hợp lệ!";
        //            model.ParsBuyCard.trace = DateTime.Now.ToString("yyyyMMddHHmmssffff");
        //            return View(model);
        //        }

        //        string username = Config.GetAppSettings("USERNAME24H", true, "");
        //        string password = Config.GetAppSettings("PASSWORD24H", true, "");
        //        vn.muathe24h.UserCredentials user = new vn.muathe24h.UserCredentials();
        //        user.userName = username;
        //        user.pass = password;
        //        vn.muathe24h.MechantServices hulft = new vn.muathe24h.MechantServices();
        //        hulft.UserCredentialsValue = user;
        //        var result = hulft.BuyCards(input.trace, input.telco, input.amount, input.quantity);
        //        NLogManager.LogMessage(string.Format("BuyCard24h-Trace:{4}| Telco:{0}| Amount:{1}| Quantity:{2}| Response:{3}",
        //            input.telco, input.amount, input.quantity, result.RepCode, input.trace));
        //        msg = MessageMuaThe24h.Instance.GetMessage(result.RepCode) ?? "undefined";

        //        if (result.RepCode != 0)
        //        {
        //            ViewBag.Message = msg;
        //            return View(model);
        //        }

        //        List<ImportCardInfo> lstImportCard = new List<ImportCardInfo>();
        //        if (result.Data != null)
        //        {
        //            List<BuyCard24hData> resultData = JsonConvert.DeserializeObject<List<BuyCard24hData>>(result.Data.ToString());
        //            var i = 0;
        //            foreach (var item in resultData)
        //            {
        //                ImportCardInfo cardInfo = new ImportCardInfo();
        //                i++;
        //                cardInfo.ID = i;
        //                cardInfo.CardValue = input.amount;
        //                cardInfo.CardNumber = StringUtil.FilterInjectionChars(item.PinCode);
        //                cardInfo.CardSerial = item.Serial;
        //                cardInfo.ExperiedDate = DateTime.Now.AddYears(1);
        //                cardInfo.TelOperatorID = input.telco == "VTT" ? 1 : input.telco == "VNP" ? 2 : input.telco == "VMS" ? 3 : 0;
        //                cardInfo.Status = 1;
        //                cardInfo.ImportDate = DateTime.Now;
        //                lstImportCard.Add(cardInfo);
        //            }

        //            DataTable dt = ExcelHelpers.ListToDataTable(lstImportCard);
        //            int response = 0;
        //            var lstResult = CardDAO.Instance.ImportCard(dt, out response);
        //            if (lstResult != null && lstResult.Any())
        //            {
        //                model.LstSuccess = lstResult.Where(c => c.Result == true).ToList();
        //                model.LstError = lstResult.Where(c => c.Result == false).ToList();
        //            }
        //        }

        //        Session["BuyCard24h"] = msg;
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        msg = ex.Message;
        //    }

        //    ViewBag.Message = msg;
        //    return View(model);
        //}
    }
}