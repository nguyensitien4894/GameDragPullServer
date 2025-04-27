using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Thecao.Database.DAO
{
    public class VIPDAO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="LoanID"></param>
        /// <param name="Response"></param>
        public void VIPLoanSignBook( long UserID, out long LoanID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_LoanID", SqlDbType.BigInt);
                param[1].Direction = ParameterDirection.Output;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_Loan_SignBook", param.ToArray());
                LoanID = ConvertUtil.ToLong(param[1].Value);
                Response = ConvertUtil.ToInt(param[2].Value);
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
            Response = -99;
            LoanID = 0;
        }
        public void VIPLoanProcess(int ServiceID, long UserID, long LoanAmount, out long LoanID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingCardConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;
                param[2] = new SqlParameter("@_LoanAmount", SqlDbType.BigInt);
                param[2].Value = LoanAmount;
                param[3] = new SqlParameter("@_LoanID", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_Loan_Process", param.ToArray());
                LoanID = ConvertUtil.ToLong(param[3].Value);
                Response = ConvertUtil.ToInt(param[4].Value);
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
            Response = -99;
            LoanID = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID">ID user</param>
        /// <param name="StartQuaterTime">Thời gian bắt đầu quý</param>
        /// <param name="EndQuaterTime">Thời gian kết thúc quý</param>
        /// <param name="RankID"></param>
        /// <param name="VPQuaterAcc"></param>
        /// <param name="VipQuaterCoeff"></param>
        /// <param name="QuaterPrizeStatus"></param>
        /// <param name="LoanLimit"></param>
        /// <param name="LoanRate"></param>
        /// <param name="QuaterAcc"></param>
        /// <param name="LoanAmount"></param>
        /// <param name="OldDebt"></param>
        public void VIPCheckQuaterLoan(long UserID, DateTime StartQuaterTime, DateTime EndQuaterTime,out int RankID, out int VPQuaterAcc, out int VipQuaterCoeff, out int QuaterPrizeStatus, out double LoanLimit, out double LoanRate, out long QuaterAcc, out long LoanAmount, out long OldDebt)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[12];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_StartQuaterTime", SqlDbType.DateTime);
                param[1].Value = StartQuaterTime;
                param[2] = new SqlParameter("@_EndQuaterTime", SqlDbType.DateTime);
                param[2].Value = EndQuaterTime;
                param[3] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_VPQuaterAcc", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_VipQuaterCoeff", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
               
               
                param[6] = new SqlParameter("@_LoanLimit", SqlDbType.Float);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_LoanRate", SqlDbType.Float);
                param[7].Direction = ParameterDirection.Output;
               
                param[8] = new SqlParameter("@_QuaterAcc", SqlDbType.BigInt);
                param[8].Direction = ParameterDirection.Output;
                param[9] = new SqlParameter("@_LoanAmount", SqlDbType.BigInt);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@_OldDebt", SqlDbType.BigInt);
                param[10].Direction = ParameterDirection.Output;
                param[11] = new SqlParameter("@_QuaterPrizeStatus", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_Check_QuaterLoan", param.ToArray());
                RankID = ConvertUtil.ToInt(param[3].Value);
                VPQuaterAcc = ConvertUtil.ToInt(param[4].Value);
                VipQuaterCoeff = ConvertUtil.ToInt(param[5].Value);
                LoanLimit = ConvertUtil.ToFloat(param[6].Value);
                LoanRate = ConvertUtil.ToFloat(param[7].Value);
                QuaterAcc = ConvertUtil.ToLong(param[8].Value);
                LoanAmount = ConvertUtil.ToLong(param[9].Value);
                OldDebt = ConvertUtil.ToLong(param[10].Value);
                QuaterPrizeStatus = ConvertUtil.ToInt(param[11].Value);
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
            RankID = 0;
            VPQuaterAcc = 0;
            VipQuaterCoeff =0;
            LoanLimit = 0;
            LoanRate = 0;
            QuaterAcc = 0;
            LoanAmount = 0;
            OldDebt =0;
            QuaterPrizeStatus = 0;

        }



    }
}