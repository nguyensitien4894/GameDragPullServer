using System;
using System.Web.Http;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using System.Configuration;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Utils;
using WebGame.Payment.Database.DAO;
using MsWebGame.Portal.Database.DTO;
using System.Linq;
using System.Collections.Generic;
using TraditionGame.Utilities.Security;
using MsWebGame.Portal.Handlers;
using MsWebGame.Portal.Models;
using WebGrease.Css.Ast.Selectors;

namespace MsWebGame.Portal.Controllers
{


    [RoutePrefix("api/VIP2")]
    public class VIP2Controller : BaseApiController
    {
        private string KeyCurrentCardFormat = "VIP2CardCurrentV5{0}";
        private string KeyNotYetCardFormat = "VIP2CardNotYetV5{0}";
        private string KeyUserVPFormat = "VIP2UserVPV5{0}";
        private string KeyUserVPRewardFormat = "VIP2UserVPRewardV5{0}";
        private string KeyLoanInfoFormat = "VIP2LoanInfoQuaterV5{0}";
        private int cachetime = 300;
        #region vay tiền
        /// hàm này dùng khi vay tiền -thuog quy (sẽ hiển thị 2 quý liên tiếp )
        /// </summary>
        /// <param name="input"></param>
        /// <returns>ResponseCode int ,LoanInfo obj</returns>
        [ActionName("GetQuaterInfo")]
        [HttpGet]
        public dynamic GetQuaterInfo()
        {
            try
            {

                
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                var cacheKey = String.Format(KeyLoanInfoFormat, accountId);
                //lay thong tin cache
                var cacheQuater = CachingHandler.ExistCache(cacheKey, ServiceID);
                if (cacheQuater)
                {
                    var vip2Model = CachingHandler.GetObjectCache<VIP2QuaterInfo>(cacheKey, ServiceID);
                    if (vip2Model != null)
                    {
                        return new
                        {
                            ResponseCode = 1,
                            QuaterInfo = vip2Model.QuaterInfo,
                            LoanInfor = vip2Model.LoanInfor,
                            BeforeQuaterInfo = vip2Model.BeforeQuaterInfo
                        };

                    }
                }
                //ket thuc thong tin cache

                DateTime StartQuaterTime;
                DateTime EndQuaterTime;
                int currentQuater;
                DateUtil.GetQuatarStartEndDate(DateTime.Now, out currentQuater, out StartQuaterTime, out EndQuaterTime);



                int RankID;
                int VPQuaterAcc;//hiển thị số tiền chỗ thưởng quý
                int VipQuaterCoeff;
                int QuaterPrizeStatus;//0: đã nhận; 1: hợp lệ chưa nhận; 2: Chua den thoi gian nhan; 3: Qua thoi gian nhan
                float LoanLimit;
                float LoanRate;
                long QuaterAcc;
                long LoanAmount;//số tiền được vay
                long OldDebt;//số tiền nợ
                bool Status = false;//hiển thị button 
                Status = DateTime.Now.InRange(StartQuaterTime.AddMonths(3), StartQuaterTime.AddMonths(3).AddDays(7).AddSeconds(-1));

                VIPDAO.Instance.VIPCheckQuaterLoan(accountId, StartQuaterTime, EndQuaterTime, out RankID, out VPQuaterAcc, out VipQuaterCoeff, out QuaterPrizeStatus, out LoanLimit, out LoanRate, out QuaterAcc, out LoanAmount, out OldDebt);

                //lấy ra thông tin quý trước đó 
                DateTime BeforeStartQuaterTime;
                DateTime BeforeEndQuaterTime;
                int BeforeCurrentQuater;
                DateUtil.GetQuatarStartEndDate(DateTime.Now.AddMonths(-3), out BeforeCurrentQuater, out BeforeStartQuaterTime, out BeforeEndQuaterTime);

                int BeforeRankID;
                int BeforeVPQuaterAcc;//hiển thị số tiền chỗ thưởng quý
                int BeforeVipQuaterCoeff;
                int BeforeQuaterPrizeStatus;//hiển thị xem đã nhận thưởng hay chưa;
                float BeforeLoanLimit;
                float BeforeLoanRate;
                long BeforeQuaterAcc;
                long BeforeLoanAmount;//số tiền được vay
                long BeforeOldDebt;//số tiền nợ
                bool BeforeStatus = false;//hiển thị button 
                BeforeStatus = DateTime.Now.InRange(BeforeStartQuaterTime.AddMonths(3), BeforeStartQuaterTime.AddMonths(3).AddDays(7).AddSeconds(-1));

                VIPDAO.Instance.VIPCheckQuaterLoan(accountId, BeforeStartQuaterTime, BeforeEndQuaterTime, out BeforeRankID, out BeforeVPQuaterAcc, out BeforeVipQuaterCoeff, out BeforeQuaterPrizeStatus, out BeforeLoanLimit, out BeforeLoanRate, out BeforeQuaterAcc, out BeforeLoanAmount, out BeforeOldDebt);
                var NotAcceptQuaterPrizeStatus = new List<int> { 0, 3 };
                bool DisplayBefore = (RankID == VipIndex && BeforeVPQuaterAcc == 0) || (NotAcceptQuaterPrizeStatus.Contains(BeforeQuaterPrizeStatus) || BeforeQuaterPrizeStatus == 4);

                //add cache
                var vip2Quater = new VIP2QuaterInfo();
                vip2Quater.BeforeQuaterInfo = DisplayBefore ? null : new QuaterInfo
                {

                    VPQuaterAcc = BeforeVPQuaterAcc,
                    QuaterPrizeStatus = BeforeQuaterPrizeStatus,
                    CurrentQuater = BeforeCurrentQuater,
                    QuaterAcc = BeforeQuaterAcc,
                    IsBefore = true,
                };
               // vip2Quater.QuaterInfo = (RankID == 5 && VPQuaterAcc == 0) || !DisplayBefore ? null : new QuaterInfo
                vip2Quater.QuaterInfo =  !DisplayBefore ? null : new QuaterInfo
                {

                    VPQuaterAcc = VPQuaterAcc,
                    QuaterAcc = QuaterAcc,
                    QuaterPrizeStatus = QuaterPrizeStatus = (RankID == VipIndex && VPQuaterAcc == 0) ? 4 : QuaterPrizeStatus,
                    CurrentQuater = currentQuater,
                    //  QuaterMsg= QuaterMsg,
                    IsBefore = false,
                };
                vip2Quater.LoanInfor = new Loan
                {
                    LoanAmount = LoanAmount,
                    OldDebt = OldDebt,
                };
                CachingHandler.AddObjectCache<VIP2QuaterInfo>(cacheKey, ServiceID, vip2Quater, cachetime);

                return new
                {
                    ResponseCode = 1,

                   // QuaterInfo = (RankID == 5 && VPQuaterAcc == 0) || !DisplayBefore ? null : new
                    QuaterInfo =  !DisplayBefore ? null : new
                    {

                        VPQuaterAcc = VPQuaterAcc,
                        QuaterAcc = QuaterAcc,
                        QuaterPrizeStatus = (RankID == VipIndex && VPQuaterAcc == 0)?4: QuaterPrizeStatus,
                        CurrentQuater = currentQuater,
                        //  QuaterMsg= QuaterMsg,
                        IsBefore = false,
                    },
                    LoanInfor = new
                    {
                        LoanAmount = LoanAmount,
                        OldDebt = OldDebt,
                    },
                    BeforeQuaterInfo = DisplayBefore ? null : new
                    {

                        VPQuaterAcc = BeforeVPQuaterAcc,
                        QuaterPrizeStatus = BeforeQuaterPrizeStatus,
                        CurrentQuater = BeforeCurrentQuater,
                        QuaterAcc = BeforeQuaterAcc,
                        IsBefore = true,
                    }
                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        /// <summary>
        /// hành động vay tiền 
        /// </summary>
        /// <param name="input"> chỉ cần truyền lên Otp</param>
        /// <returns></returns>
        [ActionName("LoanProcess")]
        [HttpPost]
        public dynamic LoanProcess([FromBody]dynamic input)
        {
            try
            {


                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                string Otp = input.Otp ?? string.Empty;
                // kiểm tra otp
                if (String.IsNullOrEmpty(Otp))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpEmpty
                    };
                }
                Otp = Otp.Trim();
                if (Otp.Length != OTPAPP_LENGTH && Otp.Length != OTPSMS_LENGTH && Otp.Length != OTPSAFE_LENGTH)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.OtpLengthInValid
                    };
                }
                var cacheKey = String.Format(KeyLoanInfoFormat, accountId);
                // kiểm tra mã Otp  từ đatabase
                int resOtp = 0;
                long otpID;
                string otmsg;
                SMSDAO.Instance.ValidOtp(user.AccountID, Otp.Length == OTPSAFE_LENGTH ? user.PhoneSafeNo : user.PhoneNumber, Otp, ServiceID, out resOtp, out otpID, out otmsg);
                if (resOtp != 1)
                {
                    return new
                    {
                        ResponseCode = -5,
                        Message = Otp.Trim().Length == 6 ? otmsg : ErrorMsg.OtpIncorrect
                    };
                }
                //xoa cache
               
                //lấy lại thông tin vay
                DateTime StartQuaterTime;
                DateTime EndQuaterTime;
                int currentQuater;
                DateUtil.GetQuatarStartEndDate(DateTime.Now, out currentQuater, out StartQuaterTime, out EndQuaterTime);
                int RankID;
                int VPQuaterAcc;
                int VipQuaterCoeff;
                int QuaterPrizeStatus;
                float LoanLimit;
                float LoanRate;
                long QuaterAcc;
                long LoanAmount;//số tiền được vay
                long OldDebt;//số tiền nợ

                VIPDAO.Instance.VIPCheckQuaterLoan(accountId, StartQuaterTime, EndQuaterTime, out RankID, out VPQuaterAcc, out VipQuaterCoeff, out QuaterPrizeStatus, out LoanLimit, out LoanRate, out QuaterAcc, out LoanAmount, out OldDebt);
                //check nếu còn nợ thì thông báo
                if (OldDebt > 0)
                {
                    return new
                    {
                        ResponseCode = -7,
                        Message = String.Format("Bạn đang vay hệ thống {0}. Bạn cần hoàn trả trước khi vay tiếp", OldDebt.LongToMoneyFormat())
                    };
                }
                var acceptRank = new List<long> { 3,4,5,6,7,8,9,10 };
                if (!acceptRank.Contains(RankID))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Tài khoản chưa đủ điều kiện vay tiền.Tài khoản cần đạt ít nhất VIP 3"
                    };
                }

                long LoandID = 0;
                int Response = 0;
                long RemainBalance = 0;
                VIPDAO.Instance.VIPLoanProcess(accountId, ServiceID, out LoanAmount, out LoandID, out RemainBalance, out Response);
                if (Response == 1)
                {
                    var cacheQuater = CachingHandler.ExistCache(cacheKey, ServiceID);
                    if (cacheQuater)
                    {
                        CachingHandler.Remove(cacheKey, ServiceID);
                    }

                    if (user != null)
                    {
                        string msg = String.Format("Bạn vay hệ thống {0} tại thời điểm {1}", LoanAmount.LongToMoneyFormat(), DateTime.Now);
                        GenerateSafeOtpMsg(user.PhoneSafeNo, user.SignalID, msg);
                    }
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Vay tiền hệ thống thành công",
                        Balance = RemainBalance
                    };
                }
                else if (Response == -202)
                {
                    return new
                    {
                        ResponseCode = -202,
                        Message = ErrorMsg.UserInValid
                    };
                }
                else if (Response == -51)
                {
                    return new
                    {
                        ResponseCode = -51,
                        Message = String.Format("Bạn đang vay hệ thống {0}. Bạn cần hoàn trả trước khi vay tiếp", OldDebt.LongToMoneyFormat())
                    };
                }
                else if (Response == -50)
                {
                    return new
                    {
                        ResponseCode = -50,
                        Message = "Vay tiền hệ thống thất bại"
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException+"|"+ Response
                    };
                }

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        /// <summary>
        /// hành động trả tiền ( vay ở quý trước)
        /// </summary>
        /// <param name="input"> p</param>
        /// <returns></returns>
        [ActionName("LoanReturn")]
        [HttpPost]
        public dynamic LoanReturn([FromBody]dynamic input)
        {
            try
            {


                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                var cacheKey = String.Format(KeyLoanInfoFormat, accountId);
                long LoandID = 0;
                int Response = 0;
                long RemainBalance = 0;
                long ReturnAmount = 0;
                VIPDAO.Instance.VIPLoanReturn(accountId, ServiceID, out LoandID, out ReturnAmount, out RemainBalance, out Response);
                if (Response == 1)
                {
                    if (user != null)
                    {
                        string msg = String.Format("Bạn hoàn trả hệ thống số tiền vay {0} tại thời điểm {1}", ReturnAmount.LongToMoneyFormat(), DateTime.Now);
                        GenerateSafeOtpMsg(user.PhoneSafeNo, user.SignalID, msg);
                    }
                    var cacheQuater = CachingHandler.ExistCache(cacheKey, ServiceID);
                    if (cacheQuater)
                    {
                        CachingHandler.Remove(cacheKey, ServiceID);
                    }
                    return new
                    {
                        ResponseCode = 1,
                        Balance = RemainBalance,
                        Message = "Trả tiền hệ thống thành công"
                    };
                }

                else if (Response == -52)
                {
                    return new
                    {
                        ResponseCode = -52,
                        Message = "Hoàn tiền thất bại",
                    };
                }
                else if (Response == -53)
                {
                    return new
                    {
                        ResponseCode = -53,
                        Message = "Đóng sổ hoàn tiền thất bại"
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.InProccessException
                    };
                }




            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        #endregion

        #region thưởng quý
        /// <summary>
        /// Nhận thưởng quý 
        /// check xem ngày hiện tại có phải từ ngày 1 quý này tới ngày mùng 7 ko 
        /// nhận thưởng quý trước
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("ReceiveQuaterPrize")]
        [HttpPost]
        public dynamic ReceiveQuaterPrize([FromBody]dynamic input)
        {
            try
            {


                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }


                long LoandID = 0;
                int Response = 0;
                long RemainBalance = 0;
                long QuaterAmount = 0;

                //lấy ra ngày quý trước nó
                var dateTime = DateTime.Now;//nếu nhận được sẽ rơi vào từ 1-7 quý tiếp theo


                DateTime StartQuaterTime;
                DateTime EndQuaterTime;
                int currentQuater;
                DateUtil.GetQuatarStartEndDate(dateTime, out currentQuater, out StartQuaterTime, out EndQuaterTime);
                if (!dateTime.InRange(StartQuaterTime, EndQuaterTime))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Thời gian nhận thưởng quý {0}-{1}", StartQuaterTime.ToString("dd/MM"), EndQuaterTime.AddSeconds(-1).ToString("dd/MM")),
                    };
                }
                // lấy ngày bắt đầu quý trước  quý này ngày bắt đầu -3
                VIPDAO.Instance.VIPReceiveQuaterPrize(accountId, ServiceID, StartQuaterTime.AddMonths(-3), out LoandID, out QuaterAmount, out RemainBalance, out Response);
                if (Response == 1)
                {
                    if (user != null)
                    {
                        string msg = String.Format("Bạn nhận thưởng quý với giá trị {0} tại thời điểm {1}", QuaterAmount.LongToMoneyFormat(), DateTime.Now);
                        GenerateSafeOtpMsg(user.PhoneSafeNo, user.SignalID, msg);
                    }
                    return new
                    {
                        ResponseCode = 1,
                        Balance = RemainBalance,
                        Message = "Nhận thưởng quý thành công"
                    };
                }

                else if (Response == -55)
                {
                    return new
                    {
                        ResponseCode = -55,
                        Message = "Nhận thưởng quý không tồn tại",
                    };
                }
                else if (Response == -56)
                {
                    return new
                    {
                        ResponseCode = -56,
                        Message = "Bạn đã nhận thưởng quý ."
                    };
                }
                else if (Response == -59)
                {
                    return new
                    {
                        ResponseCode = -59,
                        Message = "Nhận thưởng quý thất bại ."
                    };
                }
                else if (Response == -53)
                {
                    return new
                    {
                        ResponseCode = -53,
                        Message = "Nhận thưởng quý thất bại ."
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = String.Format(ErrorMsg.InProccessException + "{0}", Response),
                    };
                }

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        #endregion

        #region nạp thẻ
        [ActionName("TotalCardBonus")]
        [HttpGet]
        public dynamic TotalCardBonus()
        {
            long accountId = AccountSession.AccountID;
            var displayName = AccountSession.AccountName;
            if (accountId <= 0 || String.IsNullOrEmpty(displayName))
            {
                return new
                {
                    ResponseCode = Constants.UNAUTHORIZED,
                    Message = ErrorMsg.UnAuthorized
                };
            }
            var total = VIPDAO.Instance.VIPCardBonusCheckQuantity(accountId);
            return new
            {
                ResponseCode = 1,
                TotalCard = total
            };
        }
        /// <summary>
        /// hàm này dùng để lấy danh sách thẻ  khuyên mại
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("VIPCheckCardBonus")]
        [HttpPost]
        public dynamic VIPCheckCardBonus([FromBody]dynamic input)
        {
            try
            {
                return new 
                {
                    ResponseCode = 1,
                    CurrentCard = null as object,

                    CardNotYetReceived = null as object

                };

                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }

                //Get thong tin cache

                bool cacheCurent = CachingHandler.ExistCache(String.Format(KeyCurrentCardFormat, accountId), ServiceID);
                bool cacheNotYet = CachingHandler.ExistCache(String.Format(KeyNotYetCardFormat, accountId), ServiceID);
                //lay thong tin cache
                if (cacheCurent && cacheNotYet)
                {
                    var curent = CachingHandler.GetObjectCache<CurrentCard>(String.Format(KeyCurrentCardFormat, accountId), ServiceID);
                    var cardNotYet = CachingHandler.GetListCache<List<CardBonus>>(String.Format(KeyNotYetCardFormat, accountId), ServiceID);
                    return new
                    {
                        ResponseCode = 1,
                        CurrentCard = curent,

                        CardNotYetReceived = cardNotYet

                    };
                }

                int Response = 0;
                var list = VIPDAO.Instance.VIPCheckCardBonus(accountId);
                double Percent = 0;
                int CardVpPeriodBonus = 0;
                dynamic CardReceived = null;
                long PointAcc = 0;
                int CardVpPeriod = 0;
                var CardNotYetReceived = new List<CardBonus>();
                int rankResponse = 0;
                var currentRank = VipIndex;
                int VP = 0;
                var userRank = AccountDAO.Instance.GetUserRank(accountId, out rankResponse);

                if (list != null && list.Any())
                {
                    list = list.OrderBy(c => c.RankID).ThenBy(c => c.CardBonusNo).ToList();
                    //lấy ra thằng -1
                    var CurrentReceive = list.FirstOrDefault(c => c.CardBonusStatus == "-1");
                    if (CurrentReceive != null)
                    {
                        Percent = CurrentReceive.CardVpPeriodBonus == 0 ? 0 : (int)Math.Floor((double)(100 * CurrentReceive.PointAcc) / CurrentReceive.CardVpPeriod);
                        PointAcc = CurrentReceive.PointAcc;
                        CardVpPeriodBonus = CurrentReceive.CardVpPeriodBonus;
                        CardVpPeriod = CurrentReceive.CardVpPeriod;
                    }
                    else
                    {
                        //-1 =null thi fix du lieu cho rank 4, 5

                        if (userRank != null)
                        {

                            currentRank = (int)userRank.RankID;
                            VP = (int)userRank.VP;

                        }
                        
                        if (currentRank == 2)
                        {
                            Percent = (int)Math.Floor((double)(100 * VP) / 1000);
                            PointAcc = VP;
                            CardVpPeriodBonus = 3;
                            CardVpPeriod = 1000;
                        }
                        //change
                        if (currentRank == 1)
                        // if (currentRank == 5)
                        {
                            Percent = (int)Math.Floor((double)(100 * VP) / 100);
                            PointAcc = VP;
                            CardVpPeriodBonus = 1;
                            CardVpPeriod = 100;
                        }
                    }

                    //CardReceived = list.Where(c => !String.IsNullOrEmpty(c.CardBonusStatus) && c.CardBonusStatus != "-1").OrderBy(c=>c.CardBonusNo).Take(10).Select(c => new
                    //{
                    //    CardVpPeriodBonus = c.CardVpPeriodBonus,
                    //    PointAcc=c.PointAcc,
                    //    Percent = 100

                    //}).ToList();
                    var listTmp = list.Where(c => String.IsNullOrEmpty(c.CardBonusStatus)).OrderByDescending(c => c.CardBonusNo).Take(1);
                    foreach (var item in listTmp)
                    {
                        var itemCardBouns = new CardBonus();
                        itemCardBouns.UserID = item.UserID;
                        itemCardBouns.CardVpPeriodBonus = item.CardVpPeriodBonus;
                        itemCardBouns.CardRate = item.CardRate;
                        itemCardBouns.PointAcc = item.PointAcc;
                        itemCardBouns.CardLimit = item.CardLimit;
                        itemCardBouns.CardBonusNo = item.CardBonusNo;
                        itemCardBouns.RankID = item.RankID;
                        itemCardBouns.VP = item.VP;
                        itemCardBouns.CardVpPeriod = item.CardVpPeriod;
                        itemCardBouns.CardVpPeriodBonus = item.CardVpPeriodBonus;
                        CardNotYetReceived.Add(itemCardBouns);
                    }


                }
                else
                {
                    if (userRank != null)
                    {

                        currentRank = (int)userRank.RankID;
                        VP = (int)userRank.VP;

                    }
                    //de defalut la vip5
                    //change  if (currentRank == 5)
                    if (currentRank == 1)
                    {
                        Percent = (int)Math.Floor((double)(100 * VP) / 100);
                        PointAcc = VP;
                        CardVpPeriodBonus = 1;
                        CardVpPeriod = 100;
                    }
                    //change if (currentRank == 4)
                    if (currentRank == 2)
                    {
                        Percent = (int)Math.Floor((double)(100 * VP) / 1000);
                        PointAcc = VP;
                        CardVpPeriodBonus = 3;
                        CardVpPeriod = 1000;
                    }
                    //change if (currentRank == 3|| currentRank == 1|| currentRank == 2)
                    var accpetRanks = new List<int> { 3, 4, 5, 6, 7, 8, 9, 10 };
                    if (accpetRanks.Contains(currentRank))
                    {
                        Percent = 0;
                        PointAcc = 0;
                        CardVpPeriodBonus = 3;
                        CardVpPeriod = 1000;
                    }
                }
                //ad vao cache
                var CurrentCard = (CardNotYetReceived.Any()) ? null : new CurrentCard
                {
                    Percent = Percent,//%
                    PointAcc = PointAcc,
                    CardVpPeriodBonus = CardVpPeriodBonus,
                    CardVpPeriod = CardVpPeriod,

                };
                //cache  5phut
                CachingHandler.AddObjectCache<CurrentCard>(String.Format(KeyCurrentCardFormat, accountId), ServiceID, CurrentCard, 240);
                CachingHandler.AddListCache<CardBonus>(String.Format(KeyNotYetCardFormat, accountId), ServiceID, CardNotYetReceived, 240);



                //neu co CardNotYetReceived thi CurrentCard=null
                return new
                {
                    ResponseCode = 1,
                    CurrentCard = (CardNotYetReceived.Any()) ? null : new
                    {
                        Percent = Percent,//%
                        PointAcc = PointAcc,
                        CardVpPeriodBonus = CardVpPeriodBonus,
                        CardVpPeriod = CardVpPeriod,

                    },//danh  sách thẻ hiện tại

                    CardNotYetReceived = !CardNotYetReceived.Any() ? null : CardNotYetReceived//danh sách thẻ chưa nhận

                };


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }


        /// <summary>
        /// hàm này dùng để ấn nút nhận thẻ
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("VIPCardBonusReceive")]
        [HttpPost]
        public dynamic VIPCardBonusReceive([FromBody]dynamic input)
        {
            try
            {
                return AnphaHelper.Close();

                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                //remove het cach khi nhan the
                bool cacheCurent = CachingHandler.ExistCache(String.Format(KeyCurrentCardFormat, accountId), ServiceID);
                bool cacheNotYet = CachingHandler.ExistCache(String.Format(KeyNotYetCardFormat, accountId), ServiceID);
                if (cacheCurent)
                {
                    CachingHandler.Remove(String.Format(KeyCurrentCardFormat, accountId), ServiceID);

                }
                if (cacheNotYet)
                {
                    CachingHandler.Remove(String.Format(KeyNotYetCardFormat, accountId), ServiceID);

                }
                string RankID = input.RankID ?? string.Empty;
                var intRankID = ConvertUtil.ToInt(RankID);
                string VP = input.VP ?? string.Empty;
                var intVP = ConvertUtil.ToInt(VP);
                string CardBonusNo = input.CardBonusNo ?? string.Empty;
                var intCardBonusNo = ConvertUtil.ToInt(CardBonusNo);
                string CardRate = input.CardRate ?? string.Empty;
                var floatCardRate = ConvertUtil.ToFloat(CardRate);
                string CardLimit = input.CardLimit ?? string.Empty;
                var intCardLimit = ConvertUtil.ToInt(CardLimit);
                string Key = input.Key ?? string.Empty;

                var cardBonous = new CardBonus();

                cardBonous.UserID = accountId;
                cardBonous.RankID = intRankID;
                cardBonous.VP = intVP;
                cardBonous.CardBonusNo = intCardBonusNo;
                cardBonous.CardRate = floatCardRate;
                cardBonous.CardLimit = intCardLimit;

                if (cardBonous.Key != Key)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Tham số không hợp lệ"
                    };
                }

                int ValidateResponse = 0;
                VIPDAO.Instance.VIPCardBonusValidate(accountId, intRankID, intCardBonusNo, out ValidateResponse);
                if (ValidateResponse == -1)
                {
                    return new
                    {
                        ResponseCode = -1,
                        Message = "Tài khoản chưa đủ điều kiện nhận thẻ"
                    };
                }
                else if (ValidateResponse == 0)
                {
                    return new
                    {
                        ResponseCode = -2,
                        Message = "Tài khoản đã nhận thẻ"
                    };
                }
                else if (ValidateResponse != 1 && ValidateResponse != 0 && ValidateResponse != -1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = "Tài khoản đã nhận thẻ"
                    };
                }
                int Response = 0;
                VIPDAO.Instance.VIPCardBonusReceive(accountId, cardBonous.RankID, cardBonous.VP, cardBonous.CardBonusNo, cardBonous.CardLimit, cardBonous.CardRate, out Response);
                if (Response == 1)
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Nhận thẻ khuyến mại thành công"
                    };

                }
                else
                {
                    return new
                    {
                        ResponseCode = Response,
                        Message = "Nhận thẻ khuyến mại thất bại" + Response
                    };
                }




            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        #endregion
        #region nhận thưởng vip

        /// <summary>
        /// lấy ra mã bảng vp hiện tại của user
        /// </summary>
        /// <returns></returns>
        //[ActionName("GetUserVP")]
        //[HttpGet]
        //public dynamic GetUserVP()
        //{
        //    try
        //    {
        //        long accountId = AccountSession.AccountID;
        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        long VP = 0;
        //        int outResponse = 0;
        //        long currentUserRank = VipIndex;
        //        var userRank = AccountDAO.Instance.GetUserRank(accountId, out outResponse);
        //        if (userRank != null)
        //        {

        //            VP = userRank.VP;
        //            currentUserRank = userRank.RankID;

        //        }
        //        //get cache
        //        bool cacheUserPV = CachingHandler.ExistCache(String.Format(KeyUserVPFormat, accountId), ServiceID);
        //        bool cacheUserPV2 = CachingHandler.ExistCache(String.Format(KeyUserVPRewardFormat, accountId), ServiceID);

        //        //lay thong tin cache
        //        if (cacheUserPV && cacheUserPV2)
        //        {
        //            var cache = CachingHandler.GetListCache<UserVip>(String.Format(KeyUserVPFormat, accountId), ServiceID);
        //            var cache2 = CachingHandler.GetListCache<UserVip>(String.Format(KeyUserVPRewardFormat, accountId), ServiceID);
        //            return new
        //            {
        //                ResponseCode = 1,
        //                List = cache,//--đây là quỹ quà tặng,
        //                VP = VP,
        //                ListReWard = cache2
        //            };
        //        }

        //        var list = UserPrivilegeDAO.Instance.UserRedemptionPrize(accountId).Where(c => c.RankID < 11);
        //        var rtList = new List<UserVip>();

        //        //kiểm tra RankID
        //        var acceptRank = new List<long> { 2, 3, 4,5,6,7,8,9,10 };//ko hien th Rank 5
        //        if (acceptRank.Contains(currentUserRank))
        //        {
        //            //var tmpRanks = new List<long>() { currentUserRank == 5 ? currentUserRank - 1 : currentUserRank, currentUserRank - 1 <= 0 ? currentUserRank : currentUserRank - 1 };
        //            //list = list.Where(c => tmpRanks.Contains(c.RankID)).ToList();
        //            rtList = list.Where(c => (c.RedeemStatus == 0 && acceptRank.Contains(c.RankID))).ToList();//Lay ra vip chu nhan
        //            rtList = rtList.OrderBy(c => c.RankID).Take(1).ToList();
        //            if( !rtList.Any()&& currentUserRank==10) {
        //                rtList = list.Where(c => c.RankID == 10).Take(1).ToList();
        //            }

        //        }
        //        else//rank =1 hien thi rank =2
        //        {
        //            rtList = list.Where(c => c.RedeemStatus == 0 && c.RankID == 2).ToList();
        //        }
        //        //add to cache
        //        CachingHandler.AddListCache<UserVip>(String.Format(KeyUserVPFormat, accountId), ServiceID, rtList, cachetime);


        //        return new
        //        {
        //            ResponseCode = 1,
        //            List = rtList,//--đây là quỹ quà tặng,
        //            VP = VP,
        //            ListReWard = list
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Message = ErrorMsg.InProccessException
        //        };
        //    }

        //}


        /// <summary>
        /// lấy ra mã bảng vp hiện tại của user
        /// </summary>
        /// <returns></returns>
        [ActionName("GetUserVP")]
        [HttpGet]
        public dynamic GetUserVP()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                long VP = 0;
                int outResponse = 0;
                long currentUserRank = VipIndex;
                var userRank = AccountDAO.Instance.GetUserRank(accountId, out outResponse);
                if (userRank != null)
                {

                    VP = userRank.VP;
                    currentUserRank = userRank.RankID;

                }
                //get cache
                bool cacheUserPV = CachingHandler.ExistCache(String.Format(KeyUserVPFormat, accountId), ServiceID);
                bool cacheUserPV2 = CachingHandler.ExistCache(String.Format(KeyUserVPRewardFormat, accountId), ServiceID);

                //lay thong tin cache
                if (cacheUserPV && cacheUserPV2)
                {
                    var cache = CachingHandler.GetListCache<UserVip>(String.Format(KeyUserVPFormat, accountId), ServiceID);
                    var cache2 = CachingHandler.GetListCache<UserVip>(String.Format(KeyUserVPRewardFormat, accountId), ServiceID);
                    return new
                    {
                        ResponseCode = 1,
                        List = cache,//--đây là quỹ quà tặng,
                        VP = VP,
                        ListReWard = cache2
                    };
                }

                //var list = UserPrivilegeDAO.Instance.UserRedemptionPrize(accountId).Where(c => c.RankID < 11);
                var list = UserPrivilegeDAO.Instance.UserRedemptionPrize(accountId);
                var rtList = new List<UserVip>();

                //kiểm tra RankID
                var acceptRank = new List<long> { 2, 3, 4, 5, 6, 7, 8, 9, 10,11,12,13,14,15 };//ko hien th Rank 5
                if (acceptRank.Contains(currentUserRank))
                {
                    //var tmpRanks = new List<long>() { currentUserRank == 5 ? currentUserRank - 1 : currentUserRank, currentUserRank - 1 <= 0 ? currentUserRank : currentUserRank - 1 };
                    //list = list.Where(c => tmpRanks.Contains(c.RankID)).ToList();
                    rtList = list.Where(c => (c.RedeemStatus == 0 && acceptRank.Contains(c.RankID))).ToList();//Lay ra vip chu nhan
                    rtList = rtList.OrderBy(c => c.RankID).Take(1).ToList();
                    if (!rtList.Any() && currentUserRank == 15)
                    {
                        rtList = list.Where(c => c.RankID == 15).Take(1).ToList();
                    }

                }
                else//rank =1 hien thi rank =2
                {
                    rtList = list.Where(c => c.RedeemStatus == 0 && c.RankID == 2).ToList();
                }
                //add to cache
                CachingHandler.AddListCache<UserVip>(String.Format(KeyUserVPFormat, accountId), ServiceID, rtList, cachetime);


                return new
                {
                    ResponseCode = 1,
                    List = rtList,//--đây là quỹ quà tặng,
                    VP = VP,
                    ListReWard = list
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }

        }


        /// <summary>
        /// đổi quà và tiền
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ActionName("ChangeVPToGif")]
        [HttpPost]
        public dynamic ChangeVPToGif([FromBody]dynamic input)
        {
            try
            {

                string APPROVE = ConfigurationManager.AppSettings["VP_APPROVED"].ToString();
              
                  //  return AnphaHelper.Close();
             

                long accountId = AccountSession.AccountID;
                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }

                var user = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (user == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (user.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                //get cache
                bool cacheUserPV = CachingHandler.ExistCache(String.Format(KeyUserVPFormat, accountId), ServiceID);

                //remove cache
                if (cacheUserPV)
                {
                    CachingHandler.Remove(String.Format(KeyUserVPFormat, accountId), ServiceID);
                }

                string RankID = input.RankID ?? string.Empty;

                if ((String.IsNullOrEmpty(RankID)))
                {

                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }

                var intRankID = ConvertUtil.ToInt(RankID);//ranh muôn đổi 
                var acceptRanks = new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10 ,11,12,13,14,15};
                if(!acceptRanks.Contains(intRankID))
                //change
                //if (intRankID != 1 && intRankID != 2 && intRankID != 3 && intRankID != 4 && intRankID != 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.ParamaterInvalid
                    };
                }


                int Response = 0;

                int rankResponse = 0;
                var currentRank = VipIndex;
                var userRank = AccountDAO.Instance.GetUserRank(accountId, out rankResponse);
                if (userRank == null)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message= ErrorMsg.UserNotGetRank
                    };
                }
                if (userRank != null)
                {

                    currentRank = (int)userRank.RankID;

                }
                //nếu ranh muốn đổi lớn hơn ranh hiện tại báo lỗi 
                //change
                if (intRankID > currentRank)
               
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.UserRankNotVerify
                    };
                }



                long UserRedemptionID = 0;

                string Description = String.Format("Nhận thưởng VP");
                long RefundAmount = 0;
                long RemainBalance = 0;
                UserPrivilegeDAO.Instance.UserPrivilegeGratitude(accountId, intRankID, Description, ServiceID, out UserRedemptionID, out RefundAmount, out RemainBalance, out Response);
                NLogManager.LogMessageAuthen("UserPrivilegeGratitude |" + accountId + "|" + Response);
                if (Response == 1)
                {
                    if (user != null)
                    {
                        string msg = String.Format("Bạn nhận thưởng VP với giá trị {0} tại thời điểm {1}", RefundAmount.LongToMoneyFormat(), DateTime.Now);
                        GenerateSafeOtpMsg(user.PhoneSafeNo, user.SignalID, msg);
                    }
                    return new
                    {
                        ResponseCode = 1,
                        Message = ErrorMsg.VPSuccess,

                        Balance = RemainBalance
                    };

                }
                else if (Response == -212)
                {
                    return new
                    {
                        ResponseCode = -212,
                        Message = ErrorMsg.UserNotActive
                    };
                }
                else if (Response == -223)
                {
                    return new
                    {
                        ResponseCode = -223,
                        Message = ErrorMsg.UserNotRanked
                    };
                }
                else if (Response == -224)
                {
                    return new
                    {
                        ResponseCode = -224,
                        Message = ErrorMsg.UserChangeVP
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        Message = ErrorMsg.VPFail
                    };
                }

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
        }
        #endregion

    }
}