using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class UserComplainController : BaseController
    {
        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, int? type, bool? status, long UserID)
        {

            int TotalRecord = 0;
            var list = UserComplainDAO.Instance.UserComplainAdminList(type, status, UserID, 1, 10, out TotalRecord);

            var model = new GridModel<UserComplain>
            {
                Data = list.PagedForCommand(command),
                Total = TotalRecord
            };
            return new JsonResult
            {
                Data = model
            };
        }


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult Update(UserComplain model, GridCommand command)
        {
            if (String.IsNullOrEmpty(model.Content))
            {
                return Content("Nội dung không thể trống");
            }
            if (String.IsNullOrEmpty(model.Response))
            {
                return Content("Phản hồi không thể trống");
            }
            if (model.Content != null)
                model.Content = model.Content.Trim();
            if (model.Response != null)
                model.Response = model.Response.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }
            int outResponse = 0;
            //lay ra ban ghi can update
            var item = UserComplainDAO.Instance.UserComplainGetByID(model.ID);
            if (item != null)
            {


                UserComplainDAO.Instance.UserComplainUpdate(model.ID, model.Response, model.Content, model.Status, AdminID, out outResponse);

                if (outResponse == AppConstants.DBS.SUCCESS)
                {


                    return GetList(command, null, null, item.UserID.Value);
                }
                else
                {
                    return Content("Cập nhật thất bại");
                }
            }
            else
            {

                return Content("Not find record");

            }

        }


        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult Insert(UserComplain model, GridCommand command)
        {
            if (String.IsNullOrEmpty(model.Content))
            {
                
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Nội dung không thể trống" 

                }, JsonRequestBehavior.AllowGet);
            }
            if (String.IsNullOrEmpty(model.Response))
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Phản hồi không thể trống"

                }, JsonRequestBehavior.AllowGet);
               
            }
            if (model.Content != null)
                model.Content = model.Content.Trim();
            if (model.Response != null)
                model.Response = model.Response.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }
            int outResponse = 0;
            UserComplainDAO.Instance.UserComplainCreate(model.UserID.Value, model.ComplainTypeID.Value, model.ServiceID, model.Response, model.Content, model.Status, AdminID, out outResponse);
            //lay ra ban ghi can update
            if (outResponse == 1)
            {
                return Json(new
                {
                    ResponseCode = 1,
                    Message="Ghi chú thành công"

                }, JsonRequestBehavior.AllowGet);
            }else
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = "Ghi chú thất bại "+outResponse

                }, JsonRequestBehavior.AllowGet);
            }
            

        }


     
    }
}