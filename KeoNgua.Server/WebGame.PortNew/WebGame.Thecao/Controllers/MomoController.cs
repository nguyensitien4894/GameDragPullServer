using MsWebGame.Thecao.Database.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MsWebGame.Thecao.Database.DTO;
using TraditionGame.Utilities;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Momos.Api.Charges;
using TraditionGame.Utilities.Momos.Models;
using System.Data.SqlClient;
using System.Data;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/Momo")]
    public class MomoController : BaseApiController
    {
        private string MOMO_SECRETKEY = ConfigurationManager.AppSettings["MOMO_SECRETKEY"].ToString();
        private List<string> AccpetProvider = new List<string>() { "MMO" };


        //[ActionName("MomoCallBackResult")]
        //[HttpPost]
        //public HttpResponseMessage MomoCallBackResult([FromBody] MomoCallbackRequest momoRequest)
        //{
        //    string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (string.IsNullOrEmpty(ip))
        //    {
        //        ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //    NLogManager.LogMessage("MomoCallBackResul-IP: " + ip);
        //    if (momoRequest == null)
        //    {
        //        return JsonMomoResult(-100, "ParaInvalid model null");
        //    }
        //    LoggerHelper.LogUSDTMessage(String.Format("MomoCharge.CallBackResult CallBackModel:{0}", JsonConvert.SerializeObject(momoRequest)));
        //    string momoApiKey = ConfigurationManager.AppSettings["MOMO_API_KEY_MM"].ToString();
        //    string momosecretkey = ConfigurationManager.AppSettings["MOMO_SECRETKEY_MM"].ToString();
        //    if (String.IsNullOrEmpty(momoRequest.transaction_id))
        //    {
        //        return JsonMomoResult(-105, "MomoId  null");
        //    }
        //    if (String.IsNullOrEmpty(momoRequest.content))
        //    {
        //        return JsonMomoResult(-106, "Message null");
        //    }
        //    if (momoRequest.amount <= 0 || momoRequest.amount < 10000 || momoRequest.amount > 30000000)
        //    {
        //        NLogManager.LogMessage(String.Format("MOMODB ERROR:Amount in valid : {0}", momoRequest.amount));
        //        return JsonMomoResult(-107, "Amount in valid");
        //    }
        //    if (String.IsNullOrEmpty(momoRequest.hash))
        //    {
        //        return JsonMomoResult(-109, "Hash in valid");
        //    }
        //    if (String.IsNullOrEmpty(momoRequest.method))
        //    {
        //        return JsonMomoResult(-108, "Method in valid");
        //    }
        //    var momoEntity = MOMODAO.Instance.UserMomoRequestGetByRefKey(momoRequest.transaction_id);
        //    if (momoEntity != null)
        //    {
        //        return JsonMomoResult(-110, "Record Exist With Transaction");
        //    }

        //    string content = momosecretkey + momoApiKey + momoRequest.amount + momoRequest.transaction_id;
        //    var sign = MomoChargeApi.MD5(content);
        //    if (sign != momoRequest.hash)
        //    {
        //        return JsonMomoResult(-104, "Not Authen");
        //    }
        //    int PartnerID = 1;
        //    string PartnerErrorCode = "200";
        //    long ReceivedMoney = 0;
        //    long RemainBalance = 0;
        //    long RequestID = 0;
        //    double RequestRate = 0;
        //    int RequestType = 1;
        //    int Response = 0;
        //    int outServiceID = 0;
        //    string mess = momoRequest.content.Trim().ToLower();
        //    Account acount = AccountDAO.Instance.GetAccountInfo(0, mess, null);
        //    if (momoRequest.status == 1)
        //    {
        //        if (acount != null)
        //        {
        //            MOMODAO.Instance.UserMomoRequestPartnerCheck(
        //                acount.AccountID, RequestType, momoRequest.amount, PartnerID, null,
        //                "1", PartnerErrorCode,
        //                momoRequest.content, null,
        //                null, momoRequest.transaction_id, momoRequest.transaction_id, momoRequest.hash, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
        //            );
        //            if (Response == 1)
        //            {
        //                var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
        //                dnaHelper.SendTransactionPURCHASE(acount.AccountID, 10, null, momoRequest.amount, momoRequest.amount);
        //                LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", acount.AccountID, 7, momoRequest.amount, momoRequest.amount, RequestID));
        //                return JsonMomoResult(0, "Success");
        //            }
        //            else
        //            {
        //                NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", acount.AccountID, Response));
        //                return JsonMomoResult(0, "Success");
        //            }
        //        }
        //        else
        //        {
        //            NLogManager.LogMessage(String.Format("MOMODB ERROR:User Null Message : {0}", mess));
        //            return JsonMomoResult(0, "Success");
        //        }
        //    }
        //    else
        //    {
        //        NLogManager.LogMessage(String.Format("MOMODB ERROR:Status != 1"));
        //        return JsonMomoResult(0, "Success");
        //    }

        //}
        /// <summary>
        /// call back khi xử lý
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[ActionName("CallBackResult")]
        //[HttpPost]
        //public HttpResponseMessage CallBackResult()
        //{
        //    var content = Request.Content.ReadAsByteArrayAsync();
        //    string result = System.Text.Encoding.UTF8.GetString(content.Result);
        //    if (String.IsNullOrEmpty(result))
        //    {
        //        return JsonMomoResult(-100, "Paramater invalid");
        //    }
        //    var headers = Request.Headers;
        //    string signSend = string.Empty;
        //    if (headers.Contains("sign"))
        //    {
        //        signSend = headers.GetValues("sign").First();
        //        NLogManager.LogMessage("SignSend+| " + signSend);
        //        if (String.IsNullOrEmpty(signSend))
        //        {
        //            return JsonMomoResult(-101, "Header Send Authen Is Null");
        //        }
        //    }
        //    else
        //    {
        //        return JsonMomoResult(-102, "ParaInvalid Not Authen");
        //    }
        //    LoggerHelper.LogUSDTMessage(String.Format("MomoCharge.CallBackResult CallBackModel:{0}", result));
        //    var sign = MomoChargeApi.GetHashHMACSHA256(result, MOMO_SECRETKEY);
        //    if (sign != signSend)
        //    {
        //        return JsonMomoResult(-104, "Not Authen");
        //    }
        //    MomoCallBack model = JsonConvert.DeserializeObject<MomoCallBack>(result);
        //    if (model == null)
        //    {
        //        return JsonMomoResult(-100, "ParaInvalid model null");
        //    }

        //    if (String.IsNullOrEmpty(model.TransactionId))
        //    {
        //        return JsonMomoResult(-105, "Transac  null");
        //    }

        //    if (String.IsNullOrEmpty(model.GameAcc))
        //    {
        //        return JsonMomoResult(-106, "GameAcc1 null");
        //    }
        //    if (model.Value <= 0)
        //    {
        //        return JsonMomoResult(-107, "Amount in valid");
        //    }
        //    if (String.IsNullOrEmpty(model.Message))
        //    {
        //        return JsonMomoResult(-108, "Mes in valid");
        //    }
        //    if (String.IsNullOrEmpty(model.Provider) || !AccpetProvider.Contains(model.Provider))
        //    {
        //        return JsonMomoResult(-109, "Provider in valid");
        //    }
        //    var momoEntity = MOMODAO.Instance.UserMomoRequestGetByRefKey(model.TransactionId);
        //    if (momoEntity != null)
        //    {
        //        return JsonMomoResult(-110, "Record Exist With Transaction");
        //    }
        //    int PartnerID = 1;

        //    string PartnerErrorCode = "200";
        //    long ReceivedMoney = 0;
        //    long RemainBalance = 0;
        //    long RequestID = 0;
        //    double RequestRate = 0;
        //    int RequestType = 1;
        //    int Response = 0;
        //    var userID = ConvertUtil.ToLong(model.GameAcc);
        //    int outServiceID = 0;
        //    MOMODAO.Instance.UserMomoRequestPartnerCheck(
        //    userID, RequestType, model.Value, PartnerID,null,
        //    "1", PartnerErrorCode,
        //    model.Message, null,
        //    null, model.TransactionId, model.RequestId, model.Provider,
        //    out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate,out outServiceID
        //    );
        //    if (Response == 1)
        //    {
        //        var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
        //        dnaHelper.SendTransactionPURCHASE(userID, 10, null, model.Value, model.Value);
        //        LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", userID, 7, model.Value, model.Value, RequestID));
        //        return JsonMomoResult(200, "Recharge successful");
        //    }
        //    else
        //    {
        //        NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", userID, Response));
        //        return JsonMomoResult(Response, "Nạp thất bại|"+Response);
        //    }
        //}

        public static void UpdateStatusMomo(string @_requestId, out int Response)
        {
            DBHelper db = null;
            Response = -99;
            try
            {
                db = new DBHelper(Config.BettingConn);
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = 0;

                param[1] = new SqlParameter("@_Id", SqlDbType.BigInt);
                param[1].Value = 0;

                param[2] = new SqlParameter("@_Status", SqlDbType.BigInt);
                param[2].Value = 1;

                param[3] = new SqlParameter("@_requestId", SqlDbType.BigInt);
                param[3].Value = @_requestId;

                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;


                db.ExecuteNonQuerySP("SP_Casout_Momo_UpdateStatus", param.ToArray());

                Response = Convert.ToInt32(param[3].Value);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
        [ActionName("CallBackResultVTP")]
        [HttpPost]
        public HttpResponseMessage CallBackResultVTP(string transaction_id,string phone,string money,string signature)
        {
            NLogManager.LogMessage(String.Format("Recharge ViettelPay-transaction_id:{0}-phone:{1}-money:{2}-signature:{3}",transaction_id,phone,money,signature));
            return JsonMomoResult(200, "Recharge successful");
        }
        [ActionName("HaDongCallBack")]
        [HttpPost]
        public HttpResponseMessage HaDongCallBack([FromBody] dynamic m)
        {
            NLogManager.LogMessage(String.Format("ok code ve: {0}", JsonConvert.SerializeObject(m)));
            return JsonMomoResult(200, "Recharge successful");
        }
        [ActionName("MomoCallBackResultAction")]
        [HttpGet]
        public HttpResponseMessage CallBackBankNew([FromBody] dynamic m)
        {
            NLogManager.LogMessage(Request.RequestUri.ToString());
            return JsonMomoResult(200, "Recharge successful");
        }
        [ActionName("MomoCallBackResultAction")]
        [HttpGet]
        public HttpResponseMessage MomoCallBackResultAction(long chargeId, string chargeType, string chargeCode, int regAmount, int chargeAmount, string momoTransId, string status, string signature, string requestId = "")
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            NLogManager.LogMessage(Request.RequestUri.ToString());
            string IpsMomo = System.Configuration.ConfigurationManager.AppSettings["IpsMomo"];
            //NLogManager.LogMessage("MomoCallBackResul-IP: " + ip + " " + id + " " + type + " amount:" + amount + " status:" + status + " "+ IpsMomo);

            if (!string.IsNullOrEmpty(IpsMomo) && ip != IpsMomo)
            {
                NLogManager.LogMessage("Ip Invalid: " + ip);
                var msg = String.Format("{0}|{1}", "-1", "Ip Invalid");
                return JsonMomoResult(-106, msg);
            }

            //if (momoRequest == null)
            //{
            //    return JsonMomoResult(-100, "ParaInvalid model null");
            //}
            //LoggerHelper.LogUSDTMessage(String.Format("MomoCharge.CallBackResult CallBackModel:{0}", JsonConvert.SerializeObject(momoRequest)));

            //if (string.IsNullOrEmpty(momoTransId))
            //{
            //    return JsonMomoResult(-105, "MomoId  null");
            //}
            if (String.IsNullOrEmpty(chargeCode))
            {
                return JsonMomoResult(-106, "Message null");
            }
            if (chargeAmount <= 0 || chargeAmount < 1000 || chargeAmount > 300000000)
            {
                NLogManager.LogMessage(String.Format("MOMODB ERROR:Amount in valid : {0}", chargeAmount));
                return JsonMomoResult(-107, "Amount in valid");
            }
            if (String.IsNullOrEmpty(signature))
            {
                return JsonMomoResult(-109, "Signature in valid");
            }
            if (String.IsNullOrEmpty(chargeType))
            {
                return JsonMomoResult(-108, "Method in valid.");
            }

            int index = requestId.IndexOf("_");
            string trimRequestID = requestId;
            if (index >= 0)
                trimRequestID = requestId.Substring(0, index);

            if (chargeType.ToString() == "momoout")
            {
                if (status == "unknown")
                {
                    int req;
                    try
                    {
                        UpdateStatusMomo(trimRequestID, out req);
                        if (req == 1)
                        {
                            return JsonMomoResult(0, "Success");
                        }
                    }
                    catch (Exception ex)
                    {
                        return JsonMomoResult(0, "Success" + ex.ToString());
                    }
                }
                return JsonMomoResult(0, "RequestID not exits");
            }

            if (chargeType.ToString() == "bank")
            {
                double rate = 0;

                var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, "BANK");
                var firstBanks = bankOperator.FirstOrDefault();
                rate = firstBanks.Rate;
       
                int PartnerID = chargeId.ToString().Length == 13 ? 2 : 1;
                string PartnerErrorCode = "200";
                long ReceivedMoney = 0;
                long RemainBalance = 0;
                long RequestID = 0;
                double RequestRate = 0;
                int RequestType = 1;
                int Response = 0;
                int outServiceID = 0;
                long TransID = 0;

                if (chargeId == 1) // nếu bank miss +20%
                {
                    //rate = rate + 0.2;
                    rate = rate;
                }
                var dbMoney = Math.Round(chargeAmount * rate);
                var AmountReceived = Convert.ToInt64(dbMoney);//Tiền Game
                string mess = trimRequestID.Trim();

                Account acount = AccountDAO.Instance.GetAccountInfo(0, mess.ToLower(), null);
                if (acount != null && status == "success" || mess.Contains("AuTo"))
                {
                    long accountid = mess.Contains("AuTo") ? long.Parse("20" + mess.Substring(mess.Length - 7)) : acount.AccountID;
                    USDTDAO.Instance.UserBankRequestCreate(RequestType, accountid, chargeAmount, 0, AmountReceived, 1, PartnerID
                        , ServiceID, chargeId.ToString(), null, null, null, rate, out Response, out RemainBalance, out RequestID, out TransID);


                    NLogManager.LogMessage(accountid + "|" + RequestType + "|");

                    if (Response == 1)
                    {
                        //var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
                        //dnaHelper.SendTransactionPURCHASE(acount.AccountID, 10, null, amount, amount);
                        LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", accountid, 7, chargeAmount, chargeAmount, RequestID));
                        //string msg = "Tài khoản " + acount.AccountName + " Nạp Ngân hàng số tiền :" + chargeAmount;
                        //SendTelePush(msg,8);
                        SendChargingHub(accountid, RemainBalance, "Successful Bank Recharge " + AmountReceived, 1, ServiceID);
                        return JsonMomoResult(0, "Success");
                        
                    }
                    else
                    {
                        NLogManager.LogMessage(String.Format("BANK ERROR:UserID:{0}|ERROR:{1}", accountid, Response));
                        return JsonMomoResult(Response, RequestID.ToString());
                    }
                }
                else
                {
                    NLogManager.LogMessage(String.Format("BANK ERROR:User Null Message : {0}", mess));
                    return JsonMomoResult(0, "Success");
                }
            }
            if (chargeType.ToString() == "momo")
            {
                if (!string.IsNullOrEmpty(momoTransId))
                {
                    var momoEntity = MOMODAO.Instance.UserMomoRequestGetByRefKey(momoTransId.ToString());
                    if (momoEntity != null)
                    {
                        return JsonMomoResult(-110, "Record Exist With Transaction");
                    }
                }
                //= md5(id + amount + status + mmTransId + loginPW)
                string content = chargeId.ToString() + chargeType.ToString() + chargeCode.ToString() + chargeAmount.ToString() + status + requestId + "Gd3@*yh80@9idjsa2%".ToString();
                var sign = MomoChargeApi.MD5(content);
                NLogManager.LogMessage("content: " + content + " - sign: " + sign + "- signature: " + signature);
                if (sign != signature)
                {
                    return JsonMomoResult(-104, "Not Authen");
                }

                //if (status != "unknown")
                //{
                //    NLogManager.LogMessage(String.Format("MOMODB ERROR:Code:{0}|ERROR Status :{1}", chargeCode, status));
                //    return JsonMomoResult(0, "Success");
                //}
                int PartnerID = 1;
                string PartnerErrorCode = "200";
                long ReceivedMoney = 0;
                long RemainBalance = 0;
                long RequestID = 0;
                double RequestRate = 0;
                int RequestType = 1;
                int Response = 0;
                int outServiceID = 0;
                string mess = trimRequestID.Trim().ToLower();
                Account acount = AccountDAO.Instance.GetAccountInfo(0, mess, null);
                if (acount != null)
                {
                    //NLogManager.LogMessage(String.Format("DEEEEBUUUUGGGGGG : {0}", acount.AccountID) + "|" + RequestType.ToString() + "|" + chargeAmount.ToString() + "|" + PartnerID.ToString() + "|" + PartnerErrorCode + "|" + trimRequestID + "|" + momoTransId.ToString() + "|" + signature);
                    NLogManager.LogMessage(String.Format("DEEEEBUUUUGGGGGG : {0}", acount.AccountID) + "|RequestType:" + RequestType.ToString() + "|chargeAmount:" + chargeAmount.ToString() + "|PartnerID:" + PartnerID.ToString() + "|PartnerErrorCode:" + PartnerErrorCode + "|trimRequestID:" + trimRequestID + "|signature:" + signature);

                    MOMODAO.Instance.UserMomoRequestPartnerCheck(
                        acount.AccountID, RequestType, chargeAmount, PartnerID, null,
                        "1", PartnerErrorCode,
                        trimRequestID, null,
                        null, momoTransId, momoTransId, signature, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
                    );
                    NLogManager.LogMessage(acount.AccountID + "|" + RequestType + "|");

                    if (Response == 1)
                    {
                        //var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
                        //dnaHelper.SendTransactionPURCHASE(acount.AccountID, 10, null, amount, amount);
                        LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", acount.AccountID, 7, chargeAmount, chargeAmount, RequestID));
                        string msg = "Account " + acount.AccountName + " Top up Momo amount :" + chargeAmount;
                        //SendTelePush(msg,8);
                        SendChargingHub(acount.AccountID, RemainBalance, "Successfully topup Momo " + ReceivedMoney, 1, ServiceID);
                        return JsonMomoResult(0, "Success");
                        
                    }
                    else
                    {
                        NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", acount.AccountID, Response));
                        return JsonMomoResult(0, "Success");
                    }
                }
                else
                {
                    NLogManager.LogMessage(String.Format("MOMODB ERROR:User Null Message : {0}", mess));
                    return JsonMomoResult(0, "Success");
                }

            }
            return JsonMomoResult(0, "Success");


        }
    }
}