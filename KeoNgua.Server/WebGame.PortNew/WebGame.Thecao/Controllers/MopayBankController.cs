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
using System.Web.UI;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/MopayBank")]
    public class MopayBankController : BaseApiController
    {
        //private string MOMO_SECRETKEY = ConfigurationManager.AppSettings["MOMO_SECRETKEY"].ToString();
        //private List<string> AccpetProvider = new List<string>() { "MMO" };
        public static void UpdateStatus(string @_requestId, out int Response)
        {
            DBHelper db = null;
            Response = -99;
            try
            {
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

                db.ExecuteNonQuerySP("SP_Casout_Bank_UpdateStatus", param.ToArray());

                Response = Convert.ToInt32(param[4].Value);

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
        [ActionName("BankCallBackResultAction")]
        [HttpGet]
        public HttpResponseMessage BankCallBackResultAction(int chargeId, string chargeType, string chargeCode, string regAmount, string status, int chargeAmount, string requestId, string signature, string momoTransId)
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
           
            string IpsBank = System.Configuration.ConfigurationManager.AppSettings["IpsBank"];
            NLogManager.LogMessage("BankCallBackResult-IP: " + ip + " IpsBank:" + IpsBank);
            if (!string.IsNullOrEmpty(IpsBank) && ip != IpsBank)
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

            //if (string.IsNullOrEmpty(code))
            //{
            //    return JsonMomoResult(-105, "MomoId  null");
            //}


            if (String.IsNullOrEmpty(chargeCode))
            {
                return JsonMomoResult(-106, "Message null");
            }
            if (chargeAmount <= 0 || chargeAmount < 10000 || chargeAmount > 300000000)
            {
                NLogManager.LogMessage(String.Format("MOMODB ERROR:Amount in valid : {0}", chargeAmount));
                if (string.Compare(status,"delete")!=0) 
                return JsonMomoResult(-107, "Amount in valid");
                else goto _L1;
            }
            _L1:
            if (String.IsNullOrEmpty(signature))
            {
                return JsonMomoResult(-109, "Signature in valid");
            }
            if (String.IsNullOrEmpty(chargeType))
            {
                return JsonMomoResult(-108, "Method in valid.");
            }
            if (String.IsNullOrEmpty(requestId))
            {
                return JsonMomoResult(-105, "RequestID in valid.");
            }

            var acc = USDTDAO.Instance.UserBankRequestGetByID(long.Parse(requestId));

            if (acc != null)
            {
                return JsonMomoResult(-110, "Request not valid");
            }

            if (chargeType.ToString() == "bankout")
            {
                
                //Account account = AccountDAO.Instance.GetAccountInfo(0, acc.UserID.ToString(), null);

                if (status == "success")
                {
                    int req;
                    try
                    {
                        UpdateStatus(requestId, out req);
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
                else if (status == "delete")
                {
                    NLogManager.LogMessage("DEBUG: " + status + "|" + acc);
                    //SendChargingHub(acc.UserID, RemainBalance, "Nạp Ngân hàng thành công " + regAmount, 1, ServiceID);
                }
                return JsonMomoResult(0, "RequestID not exits");
            }
            var bankEntity = USDTDAO.Instance.UserBankRequestGetByCodeFromPartner(1, chargeCode.ToString());
            if (bankEntity != null)
            {
                return JsonMomoResult(-110, "Record Exist With Transaction");
            }

            string content = chargeId.ToString() + chargeType.ToString() + chargeCode.ToString() + chargeAmount.ToString() +  status.ToString() + requestId.ToString() + "Gd3@*yh80@9idjsa2%".ToString();
            var sign = MomoChargeApi.MD5(content);
            //NLogManager.LogMessage("content: "+ content+" - sign: " + sign + "- signature: "+ signature);
            if (sign != signature)
            {
                return JsonMomoResult(-104, "Not Authen");
            }


            double rate = 1.2;

            

            int PartnerID = 1;
            string PartnerErrorCode = "200";
            long ReceivedMoney = 0;
            long RemainBalance = 0;
            long RequestID = 0;
            double RequestRate = 0;
            int RequestType = 1;
            int Response = 0;
            int outServiceID = 0;
            string mess = requestId.Trim().ToLower();
            Account acount = AccountDAO.Instance.GetAccountInfo(0, mess, null);
            if (acount != null)
            {
                /*MOMODAO.Instance.UserMomoRequestPartnerCheck(
                    acount.AccountID, RequestType, amount, PartnerID, null,
                    "1", PartnerErrorCode,
                    code, null,
                    null, momoTransId.ToString(), momoTransId.ToString(), signature, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
                );*/

                if(status == "success")
                {
                    Bank_Success = 1;
                } else
                {
                    Bank_Success = 0;
                }

                long bonusAmount = Convert.ToInt64(rate * chargeAmount);

                NLogManager.LogMessage(String.Format("@_CheckerID:{0}|@_ChargeCode:{1}|@_UserID:{2}|@_CheckStatus:{3}|@_RequestAmount:{4}|@_RealAmount:{5}|@_RealReceivedMoney:{6}|@_RealUSDTAmount:{7}|@_RequestRate:{8}|@_PartnerStatus:{9}|@_ServiceID:{10}|@_RemainBalance:{11}|@_Response:{12}",
                    CHECKER_ID, chargeCode, acount.AccountID, Bank_Success, chargeAmount, chargeAmount, chargeAmount, chargeAmount,  0, status, ServiceID, RemainBalance, Response));
                USDTDAO.Instance.UserBankRequestPartnerCheckMopay(
                    chargeCode, 
                    acount.AccountID,
                    Bank_Success, 
                    ServiceID,
                    0, 
                    0,
                    CHECKER_ID,
                    bonusAmount, 
                    chargeAmount,
                    chargeAmount, 
                    status, out RemainBalance, out Response);


                if (Response == 1)
                {
                    //var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
                    //dnaHelper.SendTransactionPURCHASE(acount.AccountID, 10, null, amount, amount);
                    LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}|RemainBalance{4}", acount.AccountID, 7, chargeAmount, chargeAmount, RequestID, RemainBalance));
                    string msg = "Account " + acount.AccountName + " Top up the Bank amount :" + chargeAmount;
                    SendTelePush(msg, 2);
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
    }
}