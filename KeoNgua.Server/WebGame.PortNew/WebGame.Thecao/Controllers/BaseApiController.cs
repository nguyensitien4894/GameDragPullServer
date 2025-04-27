using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebGame.Header.Utils._1Pays.SMSs;
using WebGame.Payment.Database.DAO;
using MsWebGame.Thecao.Database.DAO;
using MsWebGame.Thecao.Helpers.OTPs.MobileSMS;
using TraditionGame.Utilities;
using System.Text;
using Newtonsoft.Json;
using TraditionGame.Utilities.Api;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Log;
using System.Threading;
using System.Web.Script.Serialization;
using System.IO;

namespace MsWebGame.Thecao.Controllers
{
    public class BaseApiController : ApiController
    {
        protected HttpContext _Context;
        protected int ServiceID = ConvertUtil.ToInt(ConfigurationManager.AppSettings["SERVICE_ID"].ToString());
        private static readonly string URL = ConfigurationManager.AppSettings["PortalUrl"].ToString();
        private static readonly string uri = "/api/CardCharging/SendChargeResult";
        protected int STATUS_SMG_NOT_REFUND = -3;
        protected string STATUS_SMG_NOT_REFUND_REASON = "Nạp thẻ thất bại-sai mệnh giá";
        protected int DNA_PLATFORM = ConvertUtil.ToInt(ConfigurationManager.AppSettings["DNA_PLATFORM"].ToString());//1 dev,2 deploy
        public BaseApiController()
        {

        }
        protected HttpResponseMessage StringLogResult(string parnerID,string RequestID,string cardCode,String cardSeri,string mgs)
        {
            return StringResult(mgs);
        }
        /// <summary>
        /// danh sách giá trị thẻ valid (thẻ có giá trị 0 chính là thẻ sai )
        /// </summary>
        protected List<int> ArryAmountValid = new List<int>() { 0, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };
        protected  bool SendChargingHub(long AccountId, long Balance, string Msg, int Status, int ServiceID)
        {
           
            try
            {
                
                var api = new ApiUtil<bool>();
                api.ApiAddress = URL;
                api.URI = uri;
                var res = api.Send(new { AccountId = AccountId, Balance = Balance, Msg = Msg, Status = Status, ServiceID = ServiceID });
                NLogManager.LogMessage("SendChargingNap"+ res);
                return res;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }
        protected int GetCardCodeIndex(String cardOperatorCode)
        {
            try
            {
                if (cardOperatorCode.Contains("VTT")) return 1;
                else if (cardOperatorCode.Contains("VNP")) return 2;
                else if (cardOperatorCode.Contains("ZING")) return 8;
                else if (cardOperatorCode.Contains("VCOIN")) return 9;


                else return 3;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        protected bool SendDNA(long requestId, long accountId, int transType, long amount, long amountGame,bool status,double RateKM)
        {
            try
            {
                var dnaHelper = new DNAHelpers(ServiceID, DNA_PLATFORM);
                dnaHelper.SendTransactionPURCHASE(accountId, transType, null, amount, amountGame);
                ProfileLogger.LogMessage(String.Format("accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", accountId, transType, amount, amountGame, requestId));
                //Nap the thanh cong cong them carrot
                if (status)
                {
                   

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        int intResponse = 0;
                        TreasureDAO.Instance.CarrotCollectRechargeCard(accountId, amount, ServiceID, out intResponse);
                        if (RateKM > 0)
                        {
                            var valueKh = ConvertUtil.ToLong(amount * RateKM);//gui len gia tri khuyen mai
                            dnaHelper.SendTransactionBounus(accountId, transType, null, valueKh);
                        }
                        


                    });

                }
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;

            }

        }


        protected HttpResponseMessage LogStringResult(string requestId,string code,string seri,string ls,string pushMsg)
        {
            var msg = String.Format("{0}|{1}", "00", String.Format(ls+"{0}", pushMsg));// không tìm thấy user khi cập nhật trạng thái -1
            NLogManager.LogMessage(String.Format("RequestID{0}|code{1}|pin{2}|seri{3}", requestId, code, seri, pushMsg));
            return StringResult(msg);
        }

        /// <summary>
        /// string result set success
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected HttpResponseMessage StringResult(string msg)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(msg, Encoding.UTF8, "text/plain")
            };
        }
        protected HttpResponseMessage JsonResult(int errorid, string errordes, string transactionId)
        {
            var resp = new HttpResponseMessage();
            var obj = new
            {
                transactionId,
                errordes,
                errorid
            };
            string jsonRes = JsonConvert.SerializeObject(obj);
            resp.Content = new StringContent(jsonRes, System.Text.Encoding.UTF8, "application/json");
            return resp;
        }
        /// <summary>
        /// gửi msg cộng tiền thành công
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="ammount"></param>
        /// <param name="msg"></param>
        /// <returns></returns>


        protected HttpResponseMessage JsonResult(int status, string msg)
        {
            var resp = new HttpResponseMessage();
            var obj = new
            {
                status = status,
                msg = msg,
            };
            string jsonRes = JsonConvert.SerializeObject(obj);
            resp.Content = new StringContent(jsonRes, System.Text.Encoding.UTF8, "application/json");
            return resp;
        }

        protected HttpResponseMessage JsonMomoResult(int status, string msg)
        {
            var resp = new HttpResponseMessage();
            var obj = new
            {
                errorCode = status,
                errorDescription = msg,
            };
            string jsonRes = JsonConvert.SerializeObject(obj);
            LoggerHelper.LogMomoMessage("Momo Result:"+ jsonRes);
            resp.Content = new StringContent(jsonRes, System.Text.Encoding.UTF8, "application/json");
            return resp;
        }


        #region Bank
        protected static int HTTP_OK = 200;

        //protected int PartnerID = 1;
        protected int PENDING_STATUS = 0;
        protected int CONFIRM_STATUS = 2;
        protected int APPROVED_STATUS = 3;
        protected int FAIL_API1 = -1;
        protected int FAIL_API2 = -2;
        protected int SUCCESS_STATUS = 1;
        protected int CANCELLED_STATUS = 4;
        protected int REFUSED_STATUS = 5;


        protected int Bank_Success = 1;
        protected int Bank_Fail = 0;

        protected static string PENDING = "pending";//Đang chờ xử lý, khi mới tạo order
        protected static string PROCESSING = "processing";//Sau khi tiền/USDT nộp vào tk thành công
        protected static string COMPLETED = "completed";//xử lý thành công
        protected static string CANCELED = "canceled";//order bị cancel sau khi hết thừoi gian xử lý
        protected static string FAILED = "failed";//Order bị lỗi trogn quá tình xử lý
        protected string BankOperatorConfig
        {

            get
            {
                if (ServiceID == 1) return "BANK B1";
                if (ServiceID == 2) return "BANK B2";
                if (ServiceID == 3) return "BANK B3";
                return string.Empty;
            }
        }
        protected static int CHECKER_ID = 5;

        protected int MappingStatus(string USDTStatus)
        {
            if (USDTStatus == PENDING)
            {
                return PENDING_STATUS;
            }
            if (USDTStatus == PROCESSING)
            {
                return PENDING_STATUS;
            }
            if (USDTStatus == COMPLETED)
            {
                return SUCCESS_STATUS;
            }
            if (USDTStatus == CANCELED)
            {
                return CANCELLED_STATUS;
            }
            if (USDTStatus == FAILED)
            {
                return REFUSED_STATUS;
            }
            return 0;

        }

        protected string MappingStatuStr(int Status)
        {
            if (Status == 0)
            {
                return "Chờ xử lý";
            }
            if (Status == 1)
            {
                return "Thành công";
            }
            if (Status == 3)
            {
                return "Chờ duyệt";
            }
            if (Status == 4)
            {
                return "Admin-Hủy duyệt";
            }
            if (Status == 5)
            {
                return "Thất bại";
            }
            if (Status == 6)
            {
                return "Admin-Hoàn tiền";
            }
            var fail = new List<int> { -1, -2 };
            if (fail.Contains(Status))
            {
                return "Thất bại";
            }

            return "Thất bại";

        }
        #endregion
        public OtpResponse SendTelePush(string Content, long TelegramID = 0)
        {
            OtpResponse model = new OtpResponse();
            String result = string.Empty;
            String url = "http://localhost:5005/push-system-send";
            string urlParameter = new JavaScriptSerializer().Serialize(new
            {
                Content = Content,
                Action = TelegramID
            });
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.UTF8.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();
                model.code = result;
                model.des = "";
            }
            catch (Exception e)
            {
                model.code = "-99";
                model.des = e.Message;
                NLogManager.PublishException(e);
            }
            return model;
        }

    }
}
