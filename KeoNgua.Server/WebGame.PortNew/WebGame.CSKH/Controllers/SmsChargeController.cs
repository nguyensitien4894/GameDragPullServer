using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Controllers
{
    public class SmsChargeController : BaseController
    {
        #region lich sử nạp thẻ cào
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult Index(string nickName = null)
        {

            ViewBag.ServiceBox = GetServices();
            ViewBag.GetStatus = GetStatus();
         
            ViewBag.NickName = nickName;
            ViewBag.Partners = GetPartner(1);
            return View();
        }
        private List<SelectListItem> GetStatus()
        {
            return new List<SelectListItem>()
            {

                new SelectListItem() {Text="Thành công",Value="1" },
              

            };
        }
        private List<SelectListItem> GetPartner(int SerViceID)
        {
            return new List<SelectListItem>()
            {

                new SelectListItem() {Text="SHOP THE NHANH",Value="1" },
             

            };
        }
      

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetSmsList(long? RequestID, string NickName, string RefKey, string Phone,
             DateTime? FromRequestDate, DateTime? ToRequestDate, int? Status, int? ServiceID, int? partnerID, int currentPage = 1)
        {



            if (string.IsNullOrEmpty(NickName))
                NickName = null;

           
            if (string.IsNullOrEmpty(RefKey))
                RefKey = null;
            if (string.IsNullOrEmpty(Phone))
                Phone = null;
            
            ViewBag.GetStatus = GetStatus();

            int totalRecord = 0;
            int customerPageSize = 25;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;

            var lstCard = SmsChargeDAO.Instance.UserSmsRequestList(RequestID, null, RefKey, Phone, NickName,
             partnerID, ServiceID, FromRequestDate, ToRequestDate
            , currentPage, customerPageSize, out  totalRecord);
           

            var pager = new Pager(totalRecord, (currentPage), customerPageSize, 10);
            //int totalPage = totalRecord / AppConstants.CONFIG.GRID_SIZE + 1;
            ViewBag.CurrentPage = pager.CurrentPage;
            ViewBag.TotalPage = pager.TotalPages;

            ViewBag.TotalRecord = pager.TotalItems;
            ViewBag.Prev = pager.Pre;
            ViewBag.Next = pager.Next;

            ViewBag.Start = pager.StartPage;
            ViewBag.End = pager.EndPage;
            ViewBag.StartIndex = pager.StartIndex + 1;
            ViewBag.EndIndex = pager.EndIndex + 1;
            ViewBag.TotalPage = pager.TotalPages;
            ViewBag.IsAdmin = Session["RoleCode"].ToString() == ADMIN_ROLE ? true : false;

            
            return PartialView(lstCard);
        }





        #endregion lịch sử nạp thẻ cào
    }
}