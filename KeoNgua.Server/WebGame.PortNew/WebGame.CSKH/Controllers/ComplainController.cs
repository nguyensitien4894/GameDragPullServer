using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using TraditionGame.Utilities.Messages;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.Complains;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class ComplainController : BaseController
    {
        private string strProcessItemFormat = "*--@@--*";
        private string strFormat = "*---;;---*";
        #region Quản lý Khiếu nại
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ComplainManager()
        {
            ViewBag.PartialSearchType = SEARCHTYPE;
            ViewBag.PartialComplainType = COMPLAIN_TYPE;
            ViewBag.PartialComplainStatus = COMPLAIN_STATUS;
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListComplain(GridCommand command, Complain data)
        {
            int totalRecord = 0;
            var list = AdminDAO.Instance.GetComplainList(data.SearchType, data.ComplainType, 0, data.UserName, data.ComplainStatus,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord)
                .Select(x =>
                {
                    var m = new ComplainModel();
                    m = Mapper.Map<ComplainModel>(x);
                    return m;
                });

            var model = new GridModel<ComplainModel>
            {
                Data = list,
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = model
            };
        }


        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ComplainEdit(long id, int? active)
        {
            if (id <= 0)
                throw new ArgumentException(Message.ParamaterInvalid);
            int totalRecord = 0;
            var model = new ComplainModel();

            var rs = AdminDAO.Instance.GetComplainList(0, 0, id, null, -1, 1, Int16.MaxValue, out totalRecord);
            if (rs == null)
                throw new ArgumentException(Message.ParamaterInvalid);
            var entity = rs.FirstOrDefault();
            string processCall = entity.ProcessCall;
            List<ComplainProcessModel> list = new List<ComplainProcessModel>();
            if (!String.IsNullOrEmpty(processCall))
            {
                var arrTmp = processCall.Split(new string[] { strFormat }, StringSplitOptions.None);
                foreach (var item in arrTmp)
                {
                    var arrItem = item.Split(new string[] { strProcessItemFormat }, StringSplitOptions.None);

                    var itemProcess = new ComplainProcessModel();
                    itemProcess.No = Convert.ToInt32(arrItem[0]);
                    itemProcess.CreateResult = arrItem[1].ToString();
                    itemProcess.ReceiveResult = arrItem[2].ToString();
                    itemProcess.AdminID = Convert.ToInt64(arrItem[3].ToString());
                    itemProcess.AdminUserName = (arrItem[4].ToString());
                    itemProcess.Create = (arrItem[5].ToString());
                    list.Add(itemProcess);
                }
            }

            model = Mapper.Map<ComplainModel>(entity);
            model.list = list;
            model.Active = active ?? 0;
            ViewBag.PartialComplainStatus = COMPLAIN_STATUS;
            ViewBag.PartialRequestStatus = REQUEST_STATUS;
            model.ComplainID = entity.ComplainID;
            ViewBag.Url = System.Configuration.ConfigurationManager.AppSettings["CHOSAOURL_IMG_URL"];
            return View(model);
        }

        [HttpPost]
        public ActionResult ComplainProcess(ComplainProcessModel model)
        {
            if (model.No != 1 && model.No != 2 && model.No != 3)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                });
            }
            if (String.IsNullOrEmpty(model.CreateResult) || String.IsNullOrEmpty(model.ReceiveResult))
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.ParamaterInvalid
                });
            }
            int totalRecord = 0;
            var rs = AdminDAO.Instance.GetComplainList(0, 0, model.ComplainID, null, -1, 1, Int16.MaxValue, out totalRecord);
            var entity = rs.FirstOrDefault();
            var processCall = entity.ProcessCall;
            string template = "{0}" + strProcessItemFormat + "{1}" + strProcessItemFormat + "{2}" + strProcessItemFormat + "{3}" + strProcessItemFormat + "{4}" + strProcessItemFormat + "{5}";
            string strProcess = string.Empty;
            if (String.IsNullOrEmpty(processCall))
            {
                strProcess = String.Format(template, model.No, model.CreateResult, model.ReceiveResult, AdminID, AdminAccountName, DateTime.Now.ToString());
            }
            else
            {
                List<ComplainProcessModel> list = new List<ComplainProcessModel>();
                var arrTmp = processCall.Split(new string[] { strFormat }, StringSplitOptions.None);
                foreach (var item in arrTmp)
                {
                    var arrItem = item.Split(new string[] { strProcessItemFormat }, StringSplitOptions.None);

                    var itemProcess = new ComplainProcessModel();
                    itemProcess.No = Convert.ToInt32(arrItem[0]);
                    itemProcess.CreateResult = arrItem[1].ToString();
                    itemProcess.ReceiveResult = arrItem[2].ToString();
                    itemProcess.AdminID = Convert.ToInt64(arrItem[3].ToString());
                    itemProcess.AdminUserName = (arrItem[4].ToString());
                    itemProcess.Create = (arrItem[5].ToString());
                    list.Add(itemProcess);
                }
                if (list.Any(c => c.No == model.No))
                {
                    return Json(new
                    {
                        ResponseCode = -1005,
                        Message = "Đã gọi điện xử lý lần " + model.No
                    });
                }
                var tmp = String.Format(template, model.No, model.CreateResult, model.ReceiveResult, AdminID, AdminAccountName, DateTime.Now.ToString());
                strProcess = String.Format("{0}" + strFormat + "{1}", entity.ProcessCall, tmp);

            }
            if (!String.IsNullOrEmpty(strProcess))
            {
                int res = AdminDAO.Instance.ComplainVerify(model.ComplainID, 0, null, null,
                                  0, 0, null, null, null, null, 0, null,
                                 null, null, AdminID, null, -1, null, null, null, strProcess);
                return Json(new
                {
                    ResponseCode = 1,
                    Message = ErrorMsg.UpdateSuccess
                });
            }

            return Json(new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            });
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult UserMarketUpdate(Complain model)
        {
            if (model == null)
                throw new ArgumentException(Message.ParamaterInvalid);

            if (ModelState.IsValid)
            {
                int res = 0;
                if (model.RequestStatus == 1)
                {
                    res = AdminDAO.Instance.UserMarketUpdate(model.TransID, AdminID, model.RequestStatus);
                }
                else
                {
                    res = AdminDAO.Instance.UserMarketCancel(model.TransID, AdminID, model.RequestStatus);
                }

                if (res == AppConstants.DBS.SUCCESS)
                {
                    int UpdateComplain = AdminDAO.Instance.ComplainVerify(model.ComplainID, 0, null, null,
                                 0, 0, null, null, null, null, 0, null,
                                null, null, AdminID, null, 5, null, null, null, null);
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.UpdateSuccess
                    });
                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    });
                }
            }

            return Json(new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            });
        }

        private readonly List<ConfigSelect> COMPLAIN_TYPE = new List<ConfigSelect>()
        {
            new ConfigSelect { Value = 0, Name = "Tất cả"},
            new ConfigSelect { Value = 1, Name = "Khiếu nại trong nội bộ chợ"},
            new ConfigSelect { Value = 2, Name = "Khiếu nại bên ngoài chợ"},
            new ConfigSelect { Value = 9, Name = "Khiếu nại khác"}
        };
        private readonly List<ConfigSelect> SEARCHTYPE = new List<ConfigSelect>()
        {
             new ConfigSelect { Value = 0, Name = "Tất cả"},
            new ConfigSelect { Value = 1, Name = "Theo người bán"},
            new ConfigSelect { Value =2, Name = "Theo người mua"},
        };

        private readonly List<ConfigSelect> COMPLAIN_STATUS = new List<ConfigSelect>()
        {
            new ConfigSelect { Value = -1, Name = "Tất cả"},
              new ConfigSelect { Value = 0, Name = "Khiếu nại chờ xử lý"},
            new ConfigSelect { Value = 5, Name = "Khiếu nại đã được xử lý"}
        };

        private readonly List<ConfigSelect> REQUEST_STATUS = new List<ConfigSelect>()
        {
            new ConfigSelect { Value = 1, Name = "Thành công"},
            new ConfigSelect { Value =4, Name = "Hủy giao dịch"}
        };
        #endregion
        public ActionResult Index()
        {
            return View();
        }
    }
}