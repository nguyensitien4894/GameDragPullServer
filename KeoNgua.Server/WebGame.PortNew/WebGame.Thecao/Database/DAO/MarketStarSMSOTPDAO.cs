using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;


namespace MsWebGame.Thecao.Database.DAO
{
    public class MarketStarSMSOTPDAO
    {

        private static readonly Lazy<MarketStarSMSOTPDAO> _instance = new Lazy<MarketStarSMSOTPDAO>(() => new MarketStarSMSOTPDAO());

        public static MarketStarSMSOTPDAO Instance
        {
            get { return _instance.Value; }
        }


        public void OTPInit(long UserID,int UserType, string Type, string Otp, string Msisdn, out long Balance, out long CurrentBalance, out long Response, out double OtpFeePerTime)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];


                param[0] = new SqlParameter("@_Type", SqlDbType.NVarChar);
                param[0].Size = 20;
                param[0].Value = Type;
                param[1] = new SqlParameter("@_Otp", SqlDbType.NVarChar);
                param[1].Size = 40;
                param[1].Value = Otp;
                param[2] = new SqlParameter("@_Msisdn", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = Msisdn;

                param[3] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[3].Value = UserID;
                param[4] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_CurrentBalance", SqlDbType.BigInt);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_Response", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;

                param[7] = new SqlParameter("@_OtpFeePerTime", SqlDbType.Float);
                param[7].Direction = ParameterDirection.Output;
                param[8] = new SqlParameter("@_UserType", SqlDbType.Int);
                param[8].Value = UserType;
                db.ExecuteNonQuerySP("SP_AZTechOTP_Insert", param.ToArray());
             
                Response = Convert.ToInt32(param[6].Value);
                if (Response > 0)
                {
                    Balance = Convert.ToInt64(param[4].Value);
                    CurrentBalance = Convert.ToInt64(param[5].Value);
                    OtpFeePerTime = Convert.ToInt64(param[7].Value);
                }
                else
                {
                    Balance = 0;
                    CurrentBalance = 0;
                    OtpFeePerTime = 0;
                }

                return;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
            Balance = 0;
            CurrentBalance = 0;
            OtpFeePerTime = 0;
            Response = -99;
        }


        public void UpdateStatus(long RequestId, int Active, int Status, string RequestTime, out int Response, out double OtpFeePerTime, out long Balance, out long CurrentBalance)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];
                param[0] = new SqlParameter("@_RequestId", SqlDbType.BigInt);
                param[0].Value = RequestId;
                param[1] = new SqlParameter("@_RequestTime", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = RequestTime;
                param[2] = new SqlParameter("@_Active", SqlDbType.Int);
                param[2].Value = Active;
                param[3] = new SqlParameter("@_Status", SqlDbType.Int);
                param[3].Value = Status;
                param[4] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_OtpFeePerTime", SqlDbType.Float);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_CurrentBalance", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_AZTechOTP_Update", param.ToArray());
                Response = Convert.ToInt32(param[7].Value);
                if (Response == 1)
                {
                    Balance = Convert.ToInt64(param[4].Value);
                    OtpFeePerTime = Convert.ToDouble(param[5].Value);
                    CurrentBalance = Convert.ToInt64(param[6].Value);
                }
                else
                {
                    Balance = 0;
                    OtpFeePerTime = 0;
                    CurrentBalance = 0;
                }

                return;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
            Balance = 0;
            OtpFeePerTime = 0;
            CurrentBalance = 0;
            Response = -99;
        }
    }
}