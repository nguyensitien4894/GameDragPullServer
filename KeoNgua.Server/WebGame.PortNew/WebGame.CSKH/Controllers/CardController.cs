using AutoMapper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Helpers;
using MsWebGame.CSKH.Models.Cards;
using MsWebGame.CSKH.Utils;
using TraditionGame.Utilities.Utils;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.DNA;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class CardController : BaseController
    {

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]

        public ActionResult CardReport()
        {
            var fromDate = DateTime.Now.AbsoluteStart();
            var toDate = DateTime.Now.AbsoluteEnd();
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            int totalRecord = 0;
            ViewBag.ServiceBox = GetServices();
            var lstRs = CardDAO.Instance.FnCardRechargeTopList(fromDate, toDate, 1, 1, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            ViewBag.TotalRecord = totalRecord;


            return View(lstRs);
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CardReportList(GridCommand command, DateTime? fromDate, DateTime? toDate, int serviceId = 1)
        {
            if (fromDate == null)
                fromDate = DateTime.Now.AbsoluteStart();

            if (toDate == null)
                toDate = DateTime.Now.AbsoluteEnd();
            ViewBag.ServiceBox = GetServices();
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            int totalRecord = 0;
            var lstRs = CardDAO.Instance.FnCardRechargeTopList(fromDate, toDate, serviceId, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);
            var gridModel = new GridModel<CardRechargeTopList>
            {
                Data = lstRs,
                Total = totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #region danh sách thẻ cào
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult Index()
        {
            CardListModel model = new CardListModel();

            PrepareListModel(model);
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetList(GridCommand command, CardListModel model)
        {
            //lay danh sách chăm sóc khách hàng
            long totalRecord = 0;
            var list = CardDAO.Instance.GetList(0, model.TeleId, model.CardCode, model.CardName, model.Status, command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, model.ServiceID, out totalRecord);
            var listPartners = GetPartners(model.ServiceID);
            var gridModel = new GridModel<CardModel>
            {
                Data = list.Select(x =>
                {
                    var m = new CardModel();
                    m = Mapper.Map<CardModel>(x);

                    return m;
                }),
                Total = (int)totalRecord
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        #endregion

        #region thêm mới thẻ cào
        /// <summary>
        /// GET :thêm mới telecom
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create()
        {
            var model = new CardModel();
            model.Status = true;
            PrepareModel(model);
            return View(model);
        }
        /// <summary>
        /// Post thêm mới telecom
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Create(CardModel model)
        {
            PrepareModel(model);
            if (String.IsNullOrEmpty(model.CardValueStr))
            {
                ModelState.AddModelError("CardValueStr", Message.CardValueRequired);
            }
            try
            {
                model.CardValue = Convert.ToInt32(model.CardValueStr.Replace(".", ""));
            }
            catch
            {
                ModelState.AddModelError("CardValueStr", Message.CardValueInvalid);
            }

            long totalRecored = 0;
            var list = CardDAO.Instance.GetList(0, 0, null, null, null, 1, Int32.MaxValue, model.ServiceID, out totalRecored);

            if (list != null && list.Any())
            {
                if (list.Any(c => c.CardCode.ToLower() == model.CardCode.ToLower()))
                {
                    ModelState.AddModelError("CardCode", Message.CardCodeExist);
                }
            }

            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                CardDAO.Instance.Cards_Handle(model.CardCode, model.CardName, (long)model.TelecomOperatorsID, model.CardValue.Value, 0, 0, model.Status, model.ExchangeStatus, AdminID, model.PartnerId, model.ServiceID, out outResponse);

                //tạo agency thành công
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = MessageConvetor.MsgAgencyCreate.GetMessage(outResponse);
                    ErrorNotification(msg);
                }
            }

            return View(model);
        }

        public ActionResult GetPartnersAndTeleByServiceID(int ServiceID)
        {
            var listTelecom = GetListTelecoms(ServiceID);
            var listPartners = GetPartners(ServiceID);

            return Json(new { listTelecom, listPartners }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region  edit  thẻo cào
        /// <summary>
        /// GET :edit telecom
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(long Id)
        {
            if (Id <= 0)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            long totalRecord = 0;
            var entity = CardDAO.Instance.CardsGetByID(Id);
            if (entity == null)
            {
                throw new ArgumentException("Paramater invalid ");
            }
            var model = new CardModel();
            model = Mapper.Map<CardModel>(entity);
            model.CardValueStr = entity.CardValue.IntToMoneyFormat();
            PrepareModel(model);
            return View(model);
        }
        /// <summary>
        /// Post thêm mới đại lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult Edit(CardModel model)
        {
            PrepareModel(model);
            if (String.IsNullOrEmpty(model.CardValueStr))
            {
                ModelState.AddModelError("CardValueStr", Message.CardValueRequired);
            }
            try
            {
                model.CardValue = Convert.ToInt32(model.CardValueStr.Replace(".", ""));
            }
            catch
            {
                ModelState.AddModelError("CardValueStr", Message.CardValueInvalid);
            }

            long totalRecord = 0;

            var list = CardDAO.Instance.GetList(0, 0, null, null, null, 1, Int16.MaxValue, model.ServiceID, out totalRecord).Where(c => c.ID != model.ID);

            if (list != null && list.Any())
            {
                if (list.Any(c => c.CardCode.ToLower() == model.CardCode.ToLower()))
                {
                    ModelState.AddModelError("CardCode", Message.CardCodeExist);
                }
            }

            if (ModelState.IsValid)
            {
                int outResponse = 0;
                //create agency
                CardDAO.Instance.Cards_Handle(model.CardCode, model.CardName, (long)model.TelecomOperatorsID, model.CardValue.Value, 0, 0, model.Status, model.ExchangeStatus, AdminID, model.PartnerId, model.ServiceID, out outResponse);

                //tạo agency thành công
                if (outResponse == AppConstants.DBS.SUCCESS)
                {
                    SuccessNotification(Message.Updatesuccess);
                    return RedirectToAction("Index");
                }
                else
                {
                    string msg = MessageConvetor.MsgAgencyCreate.GetMessage(outResponse);
                    ErrorNotification(msg);
                }
            }

            return View(model);
        }
        #endregion

        #region lich sử nạp thẻ cào
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult CardRechard(string nickName = null)
        {
            ViewBag.NickName = nickName;
            ViewBag.IsAdmin = Session["RoleCode"].ToString() == ADMIN_ROLE ? true : false;
            var list = GetServices();
            ViewBag.ServiceBox = list;
            UserCardRechardListModel model = new UserCardRechardListModel();

            PrepareUserCardRechargeListModel(model, ConvertUtil.ToInt(list.FirstOrDefault().Value));
            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetCardRechardList(string cardNumber = null, string cardSerial = null, int cardValue = 0, string nickName = null,
            int telOperatorId = 0, DateTime? fromDate = null, DateTime? toDate = null, string PartnerErrorCode = null, int currentPage = 1, int? status = null, int? PartnerID = 0, int? smg = 0, int serviceId = 1)
        {
            if (string.IsNullOrEmpty(nickName))
                nickName = null;

            if (string.IsNullOrEmpty(cardNumber))
                cardNumber = null;

            if (string.IsNullOrEmpty(cardSerial))
                cardSerial = null;
            int customerPageSize = 20;
            int totalRecord = 0;
            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = CardDAO.Instance.UserCardRechargeList(null, telOperatorId, nickName, cardValue, cardNumber, cardSerial, fromDate,
                toDate, status, PartnerID, smg, serviceId, PartnerErrorCode, currentPage, customerPageSize, out totalRecord);

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


            ViewBag.IsDisplay = IsDisplayFunction(ADMIN_CALLCENTER_ROLE);

            //ViewBag.CurrentPage = currentPage;
            //ViewBag.TotalPage = totalPage;
            //ViewBag.TotalRecord = totalRecord;
            //ViewBag.Prev = currentPage != 1 ? currentPage - 1 : currentPage;
            //ViewBag.Next = currentPage == totalPage ? currentPage : currentPage + 1;

            //ViewBag.Start = (currentPage - 1) * 10 + 1;
            //ViewBag.End = currentPage == totalPage ? totalRecord : currentPage * 10;
            ViewBag.IsAdmin = Session["RoleCode"].ToString() == ADMIN_ROLE ? true : false;
            return PartialView(lstCard);
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult RefundToUser(long requestId)
        {
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }

            var userCharge = CardDAO.Instance.UserCardRechargeGetByID(requestId);
            if (userCharge == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }
            if (userCharge.Status != 0 && userCharge.Status != 3 && userCharge.Status != 1)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = ErrorMsg.CardIsUpdateStatus
                }, JsonRequestBehavior.AllowGet);
            }
            int Response = 0;
            CardDAO.Instance.UserCardRechargeAdminUpdate(requestId, userCharge.UserID, AdminID, null, null, 2, "Cập nhật thành công phiên" + requestId, null, null, out Response);
            if (Response == 1)
            {
                //string note =string.Format( "Admin hoàn tiền thẻ phiên {0}", requestId);
                //long wallet = 0;
                //int resUser = AdminDAO.Instance.UserCardRechargeRefund(AdminID, AdminAccountName, userCharge.UserID, userCharge.ReceivedMoney.Value, note, wallet, userCharge.ServiceID);

                return Json(new
                {
                    ResponseCode = 1,
                    Message = "Cập nhật thành công",
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    ResponseCode = "-1",
                    Message = "Có lỗi khi cập nhật trạng thái .Liên hệ quản trị"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult RejectToUser(long requestId)
        {
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }

            var userCharge = CardDAO.Instance.UserCardRechargeGetByID(requestId);
            if (userCharge == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }
            if (userCharge.Status != 0)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = ErrorMsg.CardIsUpdateStatus
                }, JsonRequestBehavior.AllowGet);
            }
            int Response = 0;
            CardDAO.Instance.UserCardRechargeAdminUpdate(requestId, userCharge.UserID, AdminID, null, null, -1, "Admin từ chối thẻ nạp đã tra cứu trên cms", null, null, out Response);
            if (Response == 1)
            {
                return Json(new
                {
                    ResponseCode = 1,
                    Message = "Từ chối thẻ nạp thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ResponseCode = "-1",
                    Message = "Có lỗi khi cập nhật trạng thái .Liên hệ quản trị"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult GetCardRechardById(long requestId)
        {
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }

            var userCharge = CardDAO.Instance.UserCardRechargeGetByID(requestId);
            if (userCharge == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                ResponseCode = 1,
                obj = userCharge
            }, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult RefundMoneyInValidCard(long requestId, int Amount)
        {
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }

            var configPara = new List<int>() { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };

            if (!configPara.Contains(Amount))
            {
                return Json(new
                {
                    Code = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }

            var userCharge = CardDAO.Instance.UserCardRechargeGetByID(requestId);
            if (userCharge == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }
            if (!new List<String>() { AppConstants.ADMINUSER.USER_ADMINTEST, AppConstants.ADMINUSER.USER_CSKH_01, "cskh_08", "cskh_09" }.Contains(AdminAccountName))

            {
                if (userCharge.Status != 0 && userCharge.Status != -1 && userCharge.Status != -2)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Hoàn tiền chỉ chấp nhận trạng thái Đăng chờ xử lý và Nạp thất bại và gửi yêu cầu thất bại"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            var notAcceptStatus = new List<int> { 2, 4 };
            if (notAcceptStatus.Contains(userCharge.Status))
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Thẻ đã ở trạng thái thành công hoặc đã hoàn tiền.Không thể hoàn tiền"
                }, JsonRequestBehavior.AllowGet);
            }

            if (!new List<String>() { AppConstants.ADMINUSER.USER_ADMINTEST, "cskh_01", "cskh_09", "cskh_08", "cskh_02" }.Contains(AdminAccountName))
            {



                //if (userCharge.PartnerID == 1 && userCharge.FeedbackErrorCode != "10")
                //{
                //    return Json(new
                //    {
                //        ResponseCode = -1,
                //        Message = "Với đối tác 1.Chỉ cập nhật được trạng thái sai mệnh giá"
                //    }, JsonRequestBehavior.AllowGet);
                //}
                if (userCharge.PartnerID == 2 && userCharge.PartnerErrorCode != "-372")
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Với đối tác 2.Chỉ cập nhật được trạng thái sai mệnh giá"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (userCharge.PartnerID == 3 && (userCharge.FeedbackErrorCode != "0" || userCharge.ValidAmount == 0))
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Với đối tác 3.Chỉ cập nhật được trạng thái sai mệnh giá khi Mệnh giá thực lớn hơn 0 khác Mệnh giá thẻ "
                    }, JsonRequestBehavior.AllowGet);
                }
                if (userCharge.PartnerID == 5 && (userCharge.FeedbackErrorCode != "0" || userCharge.ValidAmount == 0))
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Với đối tác 5.Chỉ cập nhật được trạng thái sai mệnh giá khi Mệnh giá thực lớn hơn 0 khác Mệnh giá thẻ "
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            if (Amount > userCharge.CardValue)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = "Giá trị hoàn trả phải nhỏ hơn giá trị yêu cầu"
                }, JsonRequestBehavior.AllowGet);
            }

            var Rate = userCharge.TeleRate; ;
            var ReceiveMoney = (long)Math.Ceiling(userCharge.TeleRate.Value * Amount);
            int Response = 0;
            string note = string.Format("Admin hoàn trả  phiên {0}", requestId);
            CardDAO.Instance.UserCardRechargeAdminUpdate(requestId, userCharge.UserID, AdminID, null, null, 4, "", Amount, ReceiveMoney, out Response);
            if (Response == 1)
            {
                long wallet = 0;
                int resUser = AdminDAO.Instance.UserCardRechargeRefund(AdminID, AdminAccountName, userCharge.UserID, ReceiveMoney, note, wallet, userCharge.ServiceID);
                if (resUser == 1)
                {

                    try
                    {

                        var cardIndex = GetCardCodeIndex(userCharge.OperatorCode);
                        SendDNA(userCharge.ServiceID, userCharge.UserID, cardIndex, Amount, ReceiveMoney);

                        double KMRate = userCharge.Rate - userCharge.TeleRate.Value;
                        //Gửi thông tin khuyến mại value
                        if (KMRate <= 0)
                        {
                            KMRate = 0;

                        }
                        if (KMRate > 0)
                        {
                            var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                            var KMValue = ConvertUtil.ToLong(userCharge.CardValue * KMRate);
                            dnaHelper.SendTransactionBounus(userCharge.UserID, cardIndex, null, KMValue);
                        }



                    }
                    catch (Exception ex)
                    {
                        NLogManager.PublishException(ex);
                    }
                    return Json(new
                    {
                        ResponseCode = 1,
                        Message = "Hoàn tiền thành công",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        ResponseCode = resUser,
                        Message = "User không nhận được tiền.Admin đã chuyển tiền .Liên hệ quản trị",
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    ResponseCode = "-1",
                    Message = "Có lỗi khi cập nhật trạng thái .Liên hệ quản trị"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        public ActionResult NotDoAnyThing(long requestId, int Amount)
        {
            if (requestId <= 0)
            {
                return Json(new
                {
                    Code = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }

            if (Amount > 50000)
            {
                return Json(new
                {
                    Code = -1,
                    Message = "Số tiền cập nhật không hợp lệ"
                }, JsonRequestBehavior.AllowGet);
            }

            var configPara = new List<int>() { 10000, 20000, 50000, 100000, 200000, 300000, 500000, 1000000 };
            var userCharge = CardDAO.Instance.UserCardRechargeGetByID(requestId);
            if (userCharge == null)
            {
                return Json(new
                {
                    ResponseCode = -1,
                    Message = ErrorMsg.ParamaterInvalid
                }, JsonRequestBehavior.AllowGet);
            }
            if (AdminAccountName != AppConstants.ADMINUSER.USER_ADMINTEST)
            {
                if (userCharge.Status != 0 && userCharge.Status != -1 && userCharge.Status != -2)
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Xử lý chỉ chấp nhận trạng thái Đăng chờ xử lý và Nạp thất bại và gửi yêu cầu thất bại"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (userCharge.PartnerID == 1 && userCharge.FeedbackErrorCode != "10")
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Với đối tác 1.Chỉ cập nhật được trạng thái sai mệnh giá"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (userCharge.PartnerID == 2 && userCharge.PartnerErrorCode != "-372")
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Với đối tác 2.Chỉ cập nhật được trạng thái sai mệnh giá"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (userCharge.PartnerID == 3 && (userCharge.FeedbackErrorCode != "0" || userCharge.ValidAmount == 0))
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Với đối tác 3.Chỉ cập nhật được trạng thái sai mệnh giá khi Mệnh giá thực lớn hơn 0 khác Mệnh giá thẻ "
                    }, JsonRequestBehavior.AllowGet);
                }
                if (userCharge.PartnerID == 5 && (userCharge.FeedbackErrorCode != "0" || userCharge.ValidAmount == 0))
                {
                    return Json(new
                    {
                        ResponseCode = -1,
                        Message = "Với đối tác 5.Chỉ cập nhật được trạng thái sai mệnh giá khi Mệnh giá thực lớn hơn 0 khác Mệnh giá thẻ "
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            var Rate = userCharge.TeleRate; ;

            int Response = 0;
            string note = "Admin cập nhật trạng thái-3 .Không xử lý gì";

            var ReceiveMoney = (long)Math.Ceiling(userCharge.TeleRate.Value * Amount);
            CardDAO.Instance.UserCardRechargeAdminUpdate(requestId, userCharge.UserID, AdminID, null, null, -3, note, Amount, ReceiveMoney, out Response);
            if (Response == 1)
            {
                return Json(new
                {
                    ResponseCode = 1,
                    Message = "Cập nhật thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ResponseCode = "-1",
                    Message = "Có lỗi khi cập nhật trạng thái .Liên hệ quản trị"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion lịch sử nạp thẻ cào
        private bool IsDisplayFunction(string UserRoles)
        {
            string RoleCode = Session["RoleCode"].ToString();
            if (UserRoles != "*")
            {
                var arrRoles = UserRoles.Split(',');
                var curRoles = RoleCode.Split(',');
                var listCommon = arrRoles.Intersect(curRoles).ToList();
                if (listCommon != null && listCommon.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        #region lich sử nạp thẻ cào
        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult CardExChange(string nickName = null)
        {
            ViewBag.TelecomStatusBox = InfoHandler.Instance.GetTelecomStatus();
            ViewBag.ServiceBox = GetServices();
            ViewBag.NickName = nickName;
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult GetCardExChangeList(string userName = null, string nickName = null, string cardNumber = null, string cardSerial = null,
            int? cardValue = null, DateTime? buyDate = null, int status = -1, int currentPage = 1, int serviceId = 0)
        {
            if (string.IsNullOrEmpty(userName))
                userName = null;

            if (string.IsNullOrEmpty(nickName))
                nickName = null;

            if (string.IsNullOrEmpty(cardNumber))
                cardNumber = null;

            if (string.IsNullOrEmpty(cardSerial))
                cardSerial = null;

            int totalRecord = 0;
            int customerPageSize = 25;

            currentPage = currentPage - 1 <= 0 ? 1 : currentPage;
            var lstCard = CardDAO.Instance.UserCardSwapList(userName, nickName, cardNumber, cardSerial, cardValue, buyDate, null,
                status, null, serviceId, currentPage, customerPageSize, out totalRecord);

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

            //int totalPage = totalRecord / AppConstants.CONFIG.GRID_SIZE + 1;
            //ViewBag.CurrentPage = currentPage;
            //ViewBag.TotalPage = totalPage;
            //ViewBag.TotalRecord = totalRecord;
            //ViewBag.Prev = currentPage != 1 ? currentPage - 1 : currentPage;
            //ViewBag.Next = currentPage == totalPage ? currentPage : currentPage + 1;
            //ViewBag.Start = (currentPage - 1) * 10 + 1;
            //ViewBag.End = currentPage == totalPage ? totalRecord : currentPage * 10;
            //ViewBag.IsAdmin = Session["RoleCode"].ToString() == "ADMIN" ? true : false;
            return PartialView(lstCard);
        }
        #endregion lịch sử nạp thẻ cào

        [AdminAuthorize(Roles = ADMIN_CALLCENTER_ROLE)]
        [HttpPost]
        public ActionResult UserCardSwapExamine(long userCardSwapId, long userId, int checkStatus)
        {
            long balance = 0;
            var card = CardDAO.Instance.UserCardSwapGetByID(userCardSwapId);
            if (card == null)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapNotFound
                }, JsonRequestBehavior.AllowGet);

            }
            if (card.Status != 2)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapStatusInValid
                }, JsonRequestBehavior.AllowGet);

            }
            int res = CardDAO.Instance.UserCardSwapExamine(AdminID, userCardSwapId, userId, checkStatus, out balance);
            NLogManager.LogMessage(string.Format("AdminID:{5} | CardSwapId:{0} | UserId:{1} | Balance:{2} | Status: {3} | Response:{4}",
                userCardSwapId, userId, balance, checkStatus, res, AdminID));
            if (res == 1)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UpdateSuccess
                }, JsonRequestBehavior.AllowGet);

            }
            if (res == -35)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.CardSwapNotFound
                }, JsonRequestBehavior.AllowGet);

            }
            if (res == -36)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.UserInValid
                }, JsonRequestBehavior.AllowGet);

            }
            if (res == -504)
            {
                return Json(new
                {
                    ResponseCode = -1005,
                    Message = ErrorMsg.AmountNotValid
                }, JsonRequestBehavior.AllowGet);

            }
            return Json(new
            {
                ResponseCode = res,
                Message = ErrorMsg.InProccessException
            }, JsonRequestBehavior.AllowGet);



        }

        #region hellper method
        private List<SelectListItem> GetListStatus()
        {
            return new List<SelectListItem>
            {
                 new SelectListItem() {Value="True",Text="Hoạt động" },
                new SelectListItem() {Value="False",Text="Tạm dừng" },
            };
        }

        private List<SelectListItem> GetListTelecoms(int serviceID)
        {
            var list = TelecomOperatorsDAO.Instance.GetList(0, null, serviceID).Select(c => new SelectListItem()
            {
                Value = c.ID.ToString(),
                Text = c.OperatorName,
            }).ToList();
            return list;
        }

        private List<SelectListItem> GetListCards(long teleID, int ServiceID)
        {
            long totalRecord;
            var list = CardDAO.Instance.GetList(0, teleID, null, null, null, 1, Int16.MaxValue, ServiceID, out totalRecord).Select(c => new SelectListItem()
            {
                Value = c.ID.ToString(),
                Text = c.OperatorName,
            }).ToList();
            return list;
        }

        private void PrepareListModel(CardListModel model)
        {
            var list = GetServices();
            ViewBag.ServiceBox = list;
            model.ServiceID = ConvertUtil.ToInt(list.FirstOrDefault().Value);
            model.listStatus = GetListStatus();
            model.listTelecom = GetListTelecoms(model.ServiceID);


        }

        private void PrepareUserCardRechargeListModel(UserCardRechardListModel model, int SeriviceID)
        {

            model.listTelecom = GetListTelecoms(SeriviceID);
            model.listCard = GetListCards(0, SeriviceID);
            model.listPartner = GetPartners(SeriviceID);
            model.listSMG = new List<SelectListItem>()
            {

                new SelectListItem() {Value="1",Text="Sai mệnh giá chưa xử lý" },
                new SelectListItem() {Value="2",Text="Sai mệnh giá đã xử lý" },
            };
            model.listValue = new List<SelectListItem>()
            {
                new SelectListItem() {Value="10000",Text="10.000" },
                new SelectListItem() {Value="20000",Text="20.000" },
                new SelectListItem() {Value="30000",Text="30.000" },
                new SelectListItem() {Value="50000",Text="50.000" },
                new SelectListItem() {Value="100000",Text="100.000" },
                new SelectListItem() {Value="200000",Text="200.000" },
                new SelectListItem() {Value="300000",Text="300.000" },
                new SelectListItem() {Value="500000",Text="500.000" },
                new SelectListItem() {Value="1000000",Text="1.000.000" }
            };
            model.listStatus = new List<SelectListItem>()
            {
                new SelectListItem() {Value="2",Text="Nạp thành công" },
                new SelectListItem() {Value="0",Text="Chờ xử lý" },
                new SelectListItem() {Value="-1",Text="Nạp thất bại" },
                new SelectListItem() {Value="-2",Text="Gửi yêu cầu thất bại" },
                new SelectListItem() {Value="1",Text="Đối soát bên thứ 3" },
                new SelectListItem() {Value="3",Text="Cộng tiền không thành công" },
                new SelectListItem() {Value="4",Text="Đã  hoàn tiền bởi admin" }
             };

        }
        private List<SelectListItem> GetPartners(int ServiceID)
        {

            return CardPartnersDAO.Instance.GetList(ServiceID).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.PartnerName.ToString()
            }).ToList();
        }
        private void PrepareModel(CardModel model)
        {
            var lst = GetServices();
            model.ServiceID = model.ServiceID == 0 ? ConvertUtil.ToInt(lst.FirstOrDefault().Value) : model.ServiceID;
            ViewBag.ServiceBox = lst;
            model.listStatus = GetListStatus();
            model.listTelecom = GetListTelecoms(model.ServiceID);
            model.listPartners = GetPartners(model.ServiceID);

        }
        #endregion


        #region KenKa code new
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        public ActionResult ImportCard()
        {
            ResultImportCardList model = new ResultImportCardList();
            return View(model);
        }

        [HttpPost]
        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult ImportCard(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
            {
                ErrorNotification("Chưa nhập danh sách đại lý");
                return RedirectToAction("ImportCard");
            }
            string filename = Path.GetFileName(Server.MapPath(file.FileName));
            string extension = Path.GetExtension(file.FileName);
            if (!extension.Equals(".xlsx"))
            {
                ErrorNotification("Định dạng file chấp nhận là .xlsx");
                return RedirectToAction("ImportCard");
            }

            byte[] fileBytes = new byte[file.ContentLength];
            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

            List<ImportCardInfo> lstImportCard = new List<ImportCardInfo>();
            using (var package = new ExcelPackage(file.InputStream))
            {
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;

                for (int i = 2; i < noOfRow + 1; i++)
                {
                    ImportCardInfo cardInfo = new ImportCardInfo();
                    cardInfo.ID = i;
                    cardInfo.CardValue = Int32.Parse(workSheet.Cells[i, 1].Value.ToString());
                    cardInfo.CardNumber = workSheet.Cells[i, 2].Value.ToString();
                    cardInfo.CardSerial = workSheet.Cells[i, 3].Value.ToString();
                    cardInfo.ExperiedDate = DateTime.ParseExact(workSheet.Cells[i, 4].Value.ToString(),
                        "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    cardInfo.TelOperatorID = Int32.Parse(workSheet.Cells[i, 5].Value.ToString());
                    cardInfo.Status = Int32.Parse(workSheet.Cells[i, 6].Value.ToString());
                    cardInfo.ImportDate = DateTime.ParseExact(workSheet.Cells[i, 7].Value.ToString(),
                        "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    lstImportCard.Add(cardInfo);
                }
            }

            DataTable dt = ExcelHelpers.ListToDataTable(lstImportCard);
            int response = 0;
            var lstResult = CardDAO.Instance.ImportCard(dt, out response);
            ResultImportCardList model = new ResultImportCardList();
            if (lstResult != null && lstResult.Any())
            {
                model.LstSuccess = lstResult.Where(c => c.Result == true).ToList();
                model.LstError = lstResult.Where(c => c.Result == false).ToList();
            }

            return View(model);
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CardCatalog()
        {
            //if ((string)Session["UserName"] != "admin_123A")
            //    return RedirectToAction("Permission", "Account");

            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetListCardCatalog(GridCommand command, string cardNumber, string cardSerial, int? cardValue, int telOperatorId, int status)
        {
            //if ((string)Session["UserName"] != "admin_123A")
            //    return RedirectToAction("Permission", "Account");

            if (string.IsNullOrEmpty(cardNumber))
                cardNumber = null;

            if (string.IsNullOrEmpty(cardSerial))
                cardSerial = null;

            int totalRecord = 0;
            var lstRs = CardDAO.Instance.GetInventoryCardBankList(telOperatorId, null, null, cardValue, cardNumber, cardSerial, status,
                command.Page - 1 <= 0 ? 1 : command.Page, AppConstants.CONFIG.GRID_SIZE, out totalRecord);

            if (lstRs == null)
                lstRs = new List<CardBankInfo>();

            var model = new GridModel<CardBankInfo>
            {
                Data = lstRs,
                Total = totalRecord
            };
            return new JsonResult { Data = model };
        }

        [AdminAuthorize(Roles = ADMIN_ROLE)]
        public ActionResult CardBankCheck()
        {
            var lstRs = CardDAO.Instance.GetInventoryCardBankCheck();
            if (lstRs == null)
                lstRs = new List<CardBankCheck>();

            return View(lstRs);
        }
        #endregion

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        public ActionResult UserRechargeProgress()
        {
            ViewBag.ServiceBox = GetServices();
            return View();
        }

        [AdminAuthorize(Roles = ADMIN_ALL_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetUserRechargeProgress(GridCommand command, UserRechargeProgressModel input, int serviceId = 1)
        {
            DateTime from = input.ToDate.AddDays(1).AddMilliseconds(-1);
            var lstRs = CardDAO.Instance.GetUserRechargeProgress(input.NickName, input.FromDate, from, serviceId);
            if (lstRs == null)
                lstRs = new List<UserRechargeProgress>();

            var model = new GridModel<UserRechargeProgress>
            {
                Data = lstRs,
                Total = lstRs.Count
            };
            return new JsonResult { Data = model };
        }
    }
}