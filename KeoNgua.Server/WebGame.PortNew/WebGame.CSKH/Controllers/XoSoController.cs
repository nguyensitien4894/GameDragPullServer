using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models;
using MsWebGame.CSKH.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Controllers
{
    public class XoSoController : BaseController
    {
        private List<string> acceptList = new List<string>() { AppConstants.ADMINUSER.USER_ADMIN, AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_ADMINREF, AppConstants.ADMINUSER.USER_CSKH_01 };
        int length1 = 5;
        int length2 = 4;
        int length3 = 3;
        int length4 = 2;

        // GET: XoSo
        public ActionResult Index()
        {
            if (!acceptList.Contains((string)Session["UserName"]))

            {
                return RedirectToAction("Permission", "Account");

            }
            XoSoModel model = new XoSoModel();
            var curDate = DateTime.Now;
            var xoxoEntity = XoSoDAO.Instance.SessionGetInfo(curDate);
            model=ConvertFromEntityToModel(xoxoEntity);
            model.KetQuaDate = curDate;
            ViewBag.Length1 = length1;
            ViewBag.Length2 = length2;
            ViewBag.Length3 = length3;
            ViewBag.Length4 = length4;
            return View(model);
        }

        

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetResultInfor(DateTime KetQuaDate)
        {
            if (!acceptList.Contains((string)Session["UserName"]))

            {
                return RedirectToAction("Permission", "Account");

            }
            XoSoModel model = new XoSoModel();
            var xoxoEntity = XoSoDAO.Instance.SessionGetInfo(KetQuaDate);
            
            model = ConvertFromEntityToModel(xoxoEntity);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        public ActionResult Index(XoSoModel model)
        {
            if (!acceptList.Contains((string)Session["UserName"]))

            {
                return RedirectToAction("Permission", "Account");

            }
            ViewBag.Length1 = length1;
            ViewBag.Length2 = length2;
            ViewBag.Length3 = length3;
            ViewBag.Length4 = length4;

            var xoxoEntity = XoSoDAO.Instance.SessionGetInfo(model.KetQuaDate);
            //kiểm tra 1 giải bất kỳ xem được cập nhật kết quả hay chưa
            //if(xoxoEntity!=null && !String.IsNullOrEmpty(xoxoEntity.FirstPrizeData))
            //{
            //    ModelState.AddModelError("GiaiDB", "Kết quả  đã được cập nhật ");
            //    return View(model);

            //}
            //trim data
            model.GiaiDB = model.GiaiDB.TrimIfNotNull();
            model.GiaiNhat = model.GiaiNhat.TrimIfNotNull();
            model.GiaiNhiMot = model.GiaiNhiMot.TrimIfNotNull();
            model.GiaiNhiHai = model.GiaiNhiHai.TrimIfNotNull();
            model.GiaiBaMot = model.GiaiBaMot.TrimIfNotNull();
            model.GiaiBaHai = model.GiaiBaHai.TrimIfNotNull();
            model.GiaiBaBa = model.GiaiBaBa.TrimIfNotNull();
            model.GiaiBaBon = model.GiaiBaBon.TrimIfNotNull();
            model.GiaiBaNam = model.GiaiBaNam.TrimIfNotNull();
            model.GiaiBaSau= model.GiaiBaSau.TrimIfNotNull();
            model.GiaiBonMot = model.GiaiBonMot.TrimIfNotNull();
            model.GiaiBonHai = model.GiaiBonHai.TrimIfNotNull();
            model.GiaiBonBa = model.GiaiBonBa.TrimIfNotNull();
            model.GiaiBonBon = model.GiaiBonBon.TrimIfNotNull();

            model.GiaiNamMot = model.GiaiNamMot.TrimIfNotNull();
            model.GiaiNamHai = model.GiaiNamHai.TrimIfNotNull();
            model.GiaiNamBa = model.GiaiNamBa.TrimIfNotNull();
            model.GiaiNamBon = model.GiaiNamBon.TrimIfNotNull();
            model.GiaiNamNam = model.GiaiNamNam.TrimIfNotNull();
            model.GiaiNamSau = model.GiaiNamSau.TrimIfNotNull();

            model.GiaiSauMot = model.GiaiSauMot.TrimIfNotNull();
            model.GiaiSauHai = model.GiaiSauHai.TrimIfNotNull();
            model.GiaiSauBa = model.GiaiSauBa.TrimIfNotNull();

            model.GiaiBayMot = model.GiaiBayMot.TrimIfNotNull();
            model.GiaiBayHai = model.GiaiBayHai.TrimIfNotNull();
            model.GiaiBayBa = model.GiaiBayBa.TrimIfNotNull();
            model.GiaiBayBon = model.GiaiBayBon.TrimIfNotNull();
         

            //kiểm tra giải nhất
            bool isValid = false;
            string msg = string.Empty;
            msg=ValidateKetQua(model.GiaiDB, length1, "Giải đặc biệt", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiDB", msg);
                return View(model);
            }
            //kiểm tra giải  thứ nhất
            msg= ValidateKetQua(model.GiaiNhat, length1, "Giải nhất", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNhat", msg);
                return View(model);
            }
            //kiểm tra giải  thứ nhì
            msg = ValidateKetQua(model.GiaiNhiMot, length1, "Giải nhì 1", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNhiMot", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiNhiHai, length1, "Giải nhì 2", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNhiHai", msg);
                return View(model);
            }
            //kiểm tra giải  thứ ba
            msg = ValidateKetQua(model.GiaiBaMot, length1, "Giải ba 1", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBaMot", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBaHai, length1, "Giải ba 2", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBaHai", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBaBa, length1, "Giải ba 3", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBaBa", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBaBon, length1, "Giải ba 4", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBaBon", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBaNam, length1, "Giải ba 5", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBaNam", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBaSau, length1, "Giải ba 6", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBaSau", msg);
                return View(model);
            }

            //validate giải bốn

            msg = ValidateKetQua(model.GiaiBonMot, length2, "Giải bốn 1", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBonMot", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBonHai, length2, "Giải bốn 2", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBonHai", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBonBa, length2, "Giải bốn 3", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBonBa", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBonBon, length2, "Giải bốn 4", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBonBon", msg);
                return View(model);
            }
            //validate giải năm
            msg = ValidateKetQua(model.GiaiNamMot, length2, "Giải năm 1", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNamMot", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiNamHai, length2, "Giải năm 2", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNamHai", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiNamBa, length2, "Giải năm 3", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNamBa", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiNamBon, length2, "Giải năm 4", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNamBon", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiNamNam, length2, "Giải năm 5", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNamNam", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiNamSau, length2, "Giải năm 6", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiNamSau", msg);
                return View(model);
            }
            //validate giải 6
            msg = ValidateKetQua(model.GiaiSauMot, length3, "Giải sáu 1", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiSauMot", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiSauHai, length3, "Giải sáu 2", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiSauHai", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiSauBa, length3, "Giải sáu 3", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiSauBa", msg);
                return View(model);
            }
            //validate giải 7
            msg = ValidateKetQua(model.GiaiBayMot, length4, "Giải bảy 1", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBayMot", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBayHai, length4, "Giải bảy 2", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBayHai", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBayBa, length4, "Giải bảy 3", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBayBa", msg);
                return View(model);
            }
            msg = ValidateKetQua(model.GiaiBayBon, length4, "Giải bảy 4", out isValid);
            if (!isValid)
            {
                ModelState.AddModelError("GiaiBayBon", msg);
                return View(model);
            }

            long SessionID = xoxoEntity.SessionID;
            string SpecialPrizeData = model.GiaiDB;
            string FirstPrizeData = model.GiaiNhat;

            //trim data
            



            string SecondPrizeData = String.Format("{0},{1}", model.GiaiNhiMot, model.GiaiNhiHai);
            string ThirdPrizeData = String.Format("{0},{1},{2},{3},{4},{5}", model.GiaiBaMot, model.GiaiBaHai,model.GiaiBaBa,model.GiaiBaBon,model.GiaiBaNam,model.GiaiBaSau);
            string FourthPrizeData = String.Format("{0},{1},{2},{3}", model.GiaiBonMot, model.GiaiBonHai, model.GiaiBonBa, model.GiaiBonBon);
            string FifthPrizeData = String.Format("{0},{1},{2},{3},{4},{5}", model.GiaiNamMot, model.GiaiNamHai, model.GiaiNamBa, model.GiaiNamBon, model.GiaiNamNam, model.GiaiNamSau);
            string SixthPrizeData = String.Format("{0},{1},{2}", model.GiaiSauMot, model.GiaiSauHai, model.GiaiSauBa);
            string SeventhPrizeData = String.Format("{0},{1},{2},{3}", model.GiaiBayMot, model.GiaiBayHai, model.GiaiBayBa, model.GiaiBayBon);
            string EighthPrizeData = string.Empty;
            var response=XoSoDAO.Instance.ResultSessionUpdate(SessionID, SpecialPrizeData, FirstPrizeData,
            SecondPrizeData, ThirdPrizeData, FourthPrizeData, FifthPrizeData, SixthPrizeData, SeventhPrizeData, EighthPrizeData);
            if (response >=0)
            {
                SuccessNotification(Message.Updatesuccess);

            }
            else
            {
                ErrorNotification("Cập nhật thất bại "+ response);
            }


            return View(model);
        }
        private string ValidateKetQua(string input,int length,string inputName,out bool isValid)
        {
            if (String.IsNullOrEmpty(input))
            {
                isValid = false;
                return String.Format("{0} không thể trống", inputName);
            }
          
            if (!ValidateInput.IsNumber(input))
            {
                isValid = false;
                return String.Format("{0} chỉ chứa kí tự số", inputName);
            }
            if (input.Length != length)
            {
                isValid = false;
                return String.Format("{0} chỉ chứa {1} ký tự", inputName, length);
            }
           
            isValid = true;
            return string.Empty;

        }

        private XoSoModel ConvertFromEntityToModel(XoSo entity)
        {
            XoSoModel model = new XoSoModel();
            if (entity == null) return model;
            model.GiaiDB = entity.SpecialPrizeData;
            model.GiaiNhat = entity.FirstPrizeData;
            if (!String.IsNullOrEmpty(entity.SecondPrizeData))
            {
                var arr = entity.SecondPrizeData.Split(',');
                model.GiaiNhiMot = arr[0];
                model.GiaiNhiHai = arr[1];
            }
            if (!String.IsNullOrEmpty(entity.ThirdPrizeData))
            {
                var arr = entity.ThirdPrizeData.Split(',');
                model.GiaiBaMot = arr[0];
                model.GiaiBaHai = arr[1];
                model.GiaiBaBa = arr[2];
                model.GiaiBaBon = arr[3];
                model.GiaiBaNam = arr[4];
                model.GiaiBaSau = arr[5];
            }
            if (!String.IsNullOrEmpty(entity.FourthPrizeData))
            {
                var arr = entity.FourthPrizeData.Split(',');
                model.GiaiBonMot = arr[0];
                model.GiaiBonHai = arr[1];
                model.GiaiBonBa = arr[2];
                model.GiaiBonBon = arr[3];
                
            }
            if (!String.IsNullOrEmpty(entity.FifthPrizeData))
            {
                var arr = entity.FifthPrizeData.Split(',');
                model.GiaiNamMot = arr[0];
                model.GiaiNamHai = arr[1];
                model.GiaiNamBa = arr[2];
                model.GiaiNamBon = arr[3];
                model.GiaiNamNam = arr[4];
                model.GiaiNamSau = arr[5];

            }
            if (!String.IsNullOrEmpty(entity.SixthPrizeData))
            {
                var arr = entity.SixthPrizeData.Split(',');
                model.GiaiSauMot = arr[0];
                model.GiaiSauHai = arr[1];
                model.GiaiSauBa = arr[2];
                

            }
            if (!String.IsNullOrEmpty(entity.SeventhPrizeData))
            {
                var arr = entity.SeventhPrizeData.Split(',');
                model.GiaiBayMot = arr[0];
                model.GiaiBayHai = arr[1];
                model.GiaiBayBa = arr[2];
                model.GiaiBayBon = arr[3];
                

            }
            return model;

        }
    }
}