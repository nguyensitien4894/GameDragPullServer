using MsWebGame.Portal.Controllers.Transaction;
using MsWebGame.Portal.Database.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Collection;
using TraditionGame.Utilities.Log;

namespace MsWebGame.Portal.Database.DAO
{
    public class TransactionDAO
    {
        public static RateResponse GetRate(string type)//bank,momo,viettelpay,zalopay
        {
            DBHelper db = new DBHelper(Config.BettingConn);
            List<SqlParameter> pars = new List<SqlParameter>{
                new SqlParameter("@_Type", type),
                new SqlParameter("@_ServiceID", 1)
            };
            var rs = db.GetInstanceSP<RateResponse>("dbo.SP_GetTopupRate", pars.ToArray());
            return rs;
        }
      

        public static int AddChargeCode(string chargeCode, int chargeType, string data, long accountId, long amount, string ref_id, string expire_at, string partnerCode)
        {
            int response = 0;
            StringBuilder l = new StringBuilder();
            l.AppendLine("*********** AddChargeCode ************");
            l.AppendLine("chargeCode: " + chargeCode);
            l.AppendLine("chargeType: " + chargeType);
            l.AppendLine("data: " + data);
            try
            {
                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>();
                pars.Add(new SqlParameter("@ReferId", ref_id));
                pars.Add(new SqlParameter("@ChargeCode", chargeCode));
                pars.Add(new SqlParameter("@ChargeType", chargeType));
                pars.Add(new SqlParameter("@Data", data));
                pars.Add(new SqlParameter("@ResponseCode", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output });
                pars.Add(new SqlParameter("@AccountId", accountId));
                pars.Add(new SqlParameter("@Amount", amount));
                pars.Add(new SqlParameter("@ExpireAt", expire_at));
                pars.Add(new SqlParameter("@PartnerCode", partnerCode));
                db.ExecuteNonQuerySP("dbo.SP_AddChargeCode", pars.ToArray());
                response = int.Parse(pars[4].Value.ToString());
                l.AppendLine("result: " + response);
            }
            catch (Exception ex)
            {
                l.AppendLine("Error AddChargeCode" + ex);
                response = -99;
            }
            finally
            {
                l.AppendLine("***********************");
                NLogManager.MomoLog(l.ToString());
            }
            return response;
        }

        public static ChargeCodeInfo GetChargeCode(string chargeCode, int chargeType, string referId)
        {
            StringBuilder l = new StringBuilder();
            l.AppendLine("*********** GetChargeCode ************");
            l.AppendLine("chargeCode: " + chargeCode);
            l.AppendLine("chargeType: " + chargeType);
            l.AppendLine("referId: " + referId);
            try
            {
                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>();
                pars.Add(new SqlParameter("@ChargeCode", chargeCode));
                pars.Add(new SqlParameter("@ChargeType", chargeType));
                pars.Add(new SqlParameter("@ReferId", referId));
                var d = db.GetInstanceSP<ChargeCodeInfo>("dbo.SP_GetChargeCode", pars.ToArray());
                l.AppendLine(JsonConvert.SerializeObject(d));
                return d;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                l.AppendLine("" + ex);
                return null;
            }
            finally
            {
                l.AppendLine("***********************");
                NLogManager.MomoLog(l.ToString());
            }
        }

        public static int AddTopupBank(ChargeCodeInfo p, string note, string bankCode = "")
        {
            int response = 0;
            StringBuilder l = new StringBuilder();
            l.AppendLine("AddTopupBank: " + JsonConvert.SerializeObject(p));
            try
            {
                

                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>
                {
                    new SqlParameter("@_ChargeCodeId", p.Idx),
                    new SqlParameter("@_ChargeType", p.ChargeType),
                    new SqlParameter("@_BankCode", bankCode),
                    new SqlParameter("@_ReceivedMoney", p.ReceivedMoney),
                    new SqlParameter("@_Note", note),
                    new SqlParameter("@_Response", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_RemainBalance", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_AmountAdded", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_UserId", p.UserId),
                    new SqlParameter("@_ServiceID", 1),
                };
                db.ExecuteNonQuerySP("dbo.SP_AddTopupLog", pars.ToArray());
                response = Convert.ToInt32(pars[5].Value);
                long balance = Convert.ToInt64(pars[6].Value);
                long addAmount = Convert.ToInt64(pars[7].Value);
                l.AppendLine("Code: " + response +
                    "\r\nBalance: " + balance +
                    "\r\naddAmount: " + addAmount);
            }
            catch (Exception ex)
            {
                l.AppendLine("Error AddTopupBank: " + ex);
                response = -999;
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());
            }
            return response;
        }

        public static int AddTopupMomo(ChargeCodeInfo p, string note)
        {
            int response = 0;
            StringBuilder l = new StringBuilder();
            l.AppendLine("AddTopupMomo: " + JsonConvert.SerializeObject(p));
            try
            {


                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>
                {
                    new SqlParameter("@_ChargeCodeId", p.Idx),
                    new SqlParameter("@_ChargeType", p.ChargeType),
                    new SqlParameter("@_ReceivedMoney", p.ReceivedMoney),
                    new SqlParameter("@_Note", note),
                    new SqlParameter("@_Response", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_RemainBalance", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_AmountAdded", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_UserId", p.UserId),
                    new SqlParameter("@_ServiceID", 1),
                };
                db.ExecuteNonQuerySP("dbo.SP_AddTopupMomoLog", pars.ToArray());
                response = Convert.ToInt32(pars[4].Value);
                long balance = Convert.ToInt64(pars[5].Value);
                long addAmount = Convert.ToInt64(pars[6].Value);
                l.AppendLine("Code: " + response +
                    "\r\nBalance: " + balance +
                    "\r\naddAmount: " + addAmount);
            }
            catch (Exception ex)
            {
                l.AppendLine("Error AddTopupMomo: " + ex);
                response = -999;
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());
            }
            return response;
        }

        public static int AddTopupViettelPay(ChargeCodeInfo p, string note)
        {
            int response = 0;
            StringBuilder l = new StringBuilder();
            l.AppendLine("AddTopupViettelPay: " + JsonConvert.SerializeObject(p));
            try
            {


                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>
                {
                    new SqlParameter("@_ChargeCodeId", p.Idx),
                    new SqlParameter("@_ChargeType", p.ChargeType),
                    new SqlParameter("@_ReceivedMoney", p.ReceivedMoney),
                    new SqlParameter("@_Note", note),
                    new SqlParameter("@_Response", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_RemainBalance", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_AmountAdded", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_UserId", p.UserId),
                    new SqlParameter("@_ServiceID", 1),
                };
                db.ExecuteNonQuerySP("dbo.SP_AddTopupViettelPayLog", pars.ToArray());
                response = Convert.ToInt32(pars[4].Value);
                long balance = Convert.ToInt64(pars[5].Value);
                long addAmount = Convert.ToInt64(pars[6].Value);
                l.AppendLine("Code: " + response +
                    "\r\nBalance: " + balance +
                    "\r\naddAmount: " + addAmount);
            }
            catch (Exception ex)
            {
                l.AppendLine("Error AddTopupViettelPay: " + ex);
                response = -999;
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());
            }
            return response;
        }

        public static int AddTopupZalo(ChargeCodeInfo p, string note)
        {
            int response = 0;
            StringBuilder l = new StringBuilder();
            l.AppendLine("AddTopupZalo: " + JsonConvert.SerializeObject(p));
            try
            {


                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>
                {
                    new SqlParameter("@_ChargeCodeId", p.Idx),
                    new SqlParameter("@_ChargeType", p.ChargeType),
                    new SqlParameter("@_ReceivedMoney", p.ReceivedMoney),
                    new SqlParameter("@_Note", note),
                    new SqlParameter("@_Response", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_RemainBalance", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_AmountAdded", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_UserId", p.UserId),
                    new SqlParameter("@_ServiceID", 1),
                };
                db.ExecuteNonQuerySP("dbo.SP_AddTopupZaloLog", pars.ToArray());
                response = Convert.ToInt32(pars[4].Value);
                long balance = Convert.ToInt64(pars[5].Value);
                long addAmount = Convert.ToInt64(pars[6].Value);
                l.AppendLine("Code: " + response +
                    "\r\nBalance: " + balance +
                    "\r\naddAmount: " + addAmount);
            }
            catch (Exception ex)
            {
                l.AppendLine("Error AddTopupZalo: " + ex);
                response = -999;
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());
            }
            return response;
        }

        public static int AddTopupCard(ChargeCodeInfo p, string note)
        {
            int response = 0;
            StringBuilder l = new StringBuilder();
            l.AppendLine("AddTopupViettelPay: " + JsonConvert.SerializeObject(p));
            try
            {


                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>
                {
                    new SqlParameter("@_ChargeCodeId", p.Idx),
                    new SqlParameter("@_ChargeType", p.ChargeType),
                    new SqlParameter("@_ReceivedMoney", p.ReceivedMoney),
                    new SqlParameter("@_Note", note),
                    new SqlParameter("@_Response", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_RemainBalance", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_AmountAdded", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_UserId", p.UserId),
                    new SqlParameter("@_ServiceID", 1),
                };
                db.ExecuteNonQuerySP("dbo.SP_AddTopupCardLog", pars.ToArray());
                response = Convert.ToInt32(pars[4].Value);
                long balance = Convert.ToInt64(pars[5].Value);
                long addAmount = Convert.ToInt64(pars[6].Value);
                l.AppendLine("Code: " + response +
                    "\r\nBalance: " + balance +
                    "\r\naddAmount: " + addAmount);
            }
            catch (Exception ex)
            {
                l.AppendLine("Error AddTopupCard: " + ex);
                response = -999;
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());
            }
            return response;
        }

        public static int UpdateChargeCodeStatus(ChargeCodeInfo p, string note = "") //set status fail
        {
            int response = 0;
            StringBuilder l = new StringBuilder();
            l.AppendLine("UpdateChargeCodeStatus: " + JsonConvert.SerializeObject(p));
            try
            {
                DBHelper db = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>
                {
                    new SqlParameter("@_ChargeCodeId", p.Idx),
                    new SqlParameter("@_Response", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output },
                    new SqlParameter("@_Note", note),
                    new SqlParameter("@_ServiceID", 1),
                };
                db.ExecuteNonQuerySP("dbo.SP_UpdateChargeCodeStatus", pars.ToArray());
                response = Convert.ToInt32(pars[1].Value);

                l.AppendLine("Code: " + response );
            }
            catch (Exception ex)
            {
                l.AppendLine("Error UpdateChargeCodeStatus: " + ex);
                response = -999;
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());
            }
            return response;
        }


        public static CashoutResult CashoutMomo(long accountId, string accountName, int cardType, long cardAmount, long deductAmount, string phone, string account_momo)
        {
            LoggerHelper.LogMomoMessage("Cashout_Momo: AccountId = " + accountId +
                "\r\naccountName = " + accountName +
                "\r\ncardType = " + cardType +
                "\r\ncardAmount = " + cardAmount +
                "\r\ndeductAmount = " + deductAmount +
                "\r\nphoneNumber = " + phone +
                "\r\nAccount_momo = " + account_momo
                );
            var dbHelper = new DBHelper(Config.BettingConn);
            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@_AccountId", accountId));
            pars.Add(new SqlParameter("@_AccountName", accountName != null ? accountName : ""));
            pars.Add(new SqlParameter("@_CardType", cardType));
            pars.Add(new SqlParameter("@_CardAmount", cardAmount));
            pars.Add(new SqlParameter("@_DeductAmount", deductAmount));
            pars.Add(new SqlParameter("@_Balance", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output });
            pars.Add(new SqlParameter("@_ResponseStatus", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output });
            pars.Add(new SqlParameter("@_Msg", System.Data.SqlDbType.NVarChar, 200) { Direction = System.Data.ParameterDirection.Output });
            pars.Add(new SqlParameter("@_Phone", phone));
            pars.Add(new SqlParameter("@_Account_momo", account_momo != null ? account_momo : ""));
            pars.Add(new SqlParameter("@_ServiceID", 1));

            dbHelper.ExecuteNonQuerySP("[dbo].[SP_Cashout_momo]", pars.ToArray());
            long balance = Convert.ToInt64(pars[5].Value);
            int response = Convert.ToInt32(pars[6].Value);
            string msg = pars[7].Value.ToString();
            var result = new CashoutResult()
            {
                Status = response,
                Balance = balance,
                Msg = msg
            };
            LoggerHelper.LogMomoMessage("Result Cashout_Momo: " + JsonConvert.SerializeObject(result));
            return result;
        }

        public static CashoutResult CashoutBank(long accountId, string accountName, long amount, string accountBank, string accountBankName, string bankCode)
        {
            StringBuilder l = new StringBuilder();
            l.AppendLine("CashoutBank: AccountId = " + accountId +
                "\r\naccountName = " + accountName +
                "\r\ncardAmount = " + amount +
                "\r\naccountBank = " + accountBank +
                "\r\naccountBankName = " + accountBankName +
                "\r\nbankCode = " + bankCode);
            try
            {

                var dbHelper = new DBHelper(Config.BettingConn);
                List<SqlParameter> pars = new List<SqlParameter>();
                pars.Add(new SqlParameter("@_AccountId", accountId));
                pars.Add(new SqlParameter("@_AccountName", accountName));
                pars.Add(new SqlParameter("@_Amount", amount));
                pars.Add(new SqlParameter("@_DeductAmount", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output });
                pars.Add(new SqlParameter("@_Balance", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output });
                pars.Add(new SqlParameter("@_ResponseStatus", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output });
                pars.Add(new SqlParameter("@_Msg", System.Data.SqlDbType.NVarChar, 200) { Direction = System.Data.ParameterDirection.Output });
                pars.Add(new SqlParameter("@_BankCode", bankCode));
                pars.Add(new SqlParameter("@_AccountBankNumber", accountBank));
                pars.Add(new SqlParameter("@_AccountBankName", accountBankName));
                pars.Add(new SqlParameter("@_ServiceID", 1));
                dbHelper.ExecuteNonQuerySP("[dbo].[SP_Cashout_Bank]", pars.ToArray());

                long deductAmount = Convert.ToInt64(pars[3].Value);
                long balance = Convert.ToInt64(pars[4].Value);
                int response = Convert.ToInt32(pars[5].Value);
                string msg = pars[6].Value.ToString();
                var result = new CashoutResult()
                {
                    Status = response,
                    Balance = balance,
                    DeductAmount = deductAmount,
                    Msg = msg
                };
                l.AppendLine("Result CashoutBank: " + JsonConvert.SerializeObject(result));

                return result;
            }
            catch (Exception ex)
            {
                l.AppendLine("Error CashoutBank: " + ex);
                return new CashoutResult()
                {
                    Status = -99,
                    Msg = "Hệ thống bận!!!",
                    Balance = 0
                };
            }
            finally
            {
                NLogManager.MomoLog(l.ToString());
            }
        }



    }

    public class CashoutResult
    {
        public int Status { get; set; }

        public string Msg { get; set; }
        public long Balance { get; set; }

        public long DeductAmount { get; set; }
    }
}