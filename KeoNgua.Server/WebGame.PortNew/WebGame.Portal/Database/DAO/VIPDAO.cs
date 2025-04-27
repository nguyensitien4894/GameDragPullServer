using MsWebGame.Portal.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class VIPDAO
    {
        private static readonly Lazy<VIPDAO> _instance = new Lazy<VIPDAO>(() => new VIPDAO());

        public static VIPDAO Instance
        {
            get { return _instance.Value; }
        }
        public void VIPCardBonusTick(long UserID, out long VipCardID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_VipCardID", SqlDbType.BigInt);
                param[1].Direction = ParameterDirection.Output;
                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;


                db.ExecuteNonQuerySP("SP_VIP_CardBonus_Tick", param.ToArray());
                VipCardID = ConvertUtil.ToLong(param[1].Value);
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
            VipCardID = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RequestType">1 trừ ,2 roll back</param>
        /// <param name="UserID"></param>
        /// <param name="VipCardID">khi RequestType=1 mới cần ,requestType=2 thì null</param>
        /// <param name="RequestID"></param>
        /// <param name="Response"></param>
        public void VIPCardBonusUpdate(int RequestType, long UserID, long? VipCardID, long RequestID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[2] = new SqlParameter("@_RequestType", SqlDbType.Int);
                param[2].Value = RequestType;

                param[1] = new SqlParameter("@_VipCardID", SqlDbType.BigInt);
                param[1].Value = VipCardID ?? (object)DBNull.Value;
                param[3] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[3].Value = RequestID;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_CardBonus_Update", param.ToArray());
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
        }

        public void VIPCardBonusValidate(long UserID, int RankID, int CardBonusNo, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[1].Value = RankID;
                param[2] = new SqlParameter("@_CardBonusNo", SqlDbType.Int);
                param[2].Value = CardBonusNo;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;


                db.ExecuteNonQuerySP("SP_VIP_CardBonus_Validate", param.ToArray());
                Response = ConvertUtil.ToInt(param[3].Value);
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
        }

        /// <summary>
        /// nhận 
        /// </summary>
        /// <param name="RankID"></param>
        /// <param name="VP"></param>
        /// <param name="CardBonusNo"></param>
        /// <param name="CardLimit"></param>
        /// <param name="Response"></param>
        /// <param name="CardRate"></param>
        /// <param name="UserID"></param>
        public void VIPCardBonusReceive(long UserID, int RankID, long VP, int CardBonusNo, int CardLimit, double CardRate, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_RankID", SqlDbType.Int);
                param[1].Value = RankID;
                param[2] = new SqlParameter("@_VP", SqlDbType.BigInt);
                param[2].Value = VP;
                param[3] = new SqlParameter("@_CardBonusNo", SqlDbType.Int);
                param[3].Value = CardBonusNo;
                param[4] = new SqlParameter("@_CardLimit", SqlDbType.Int);
                param[4].Value = CardLimit;
                param[5] = new SqlParameter("@_CardRate", SqlDbType.Float);
                param[5].Value = CardRate;
                param[6] = new SqlParameter("@_Response", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_CardBonus_Receive", param.ToArray());
                Response = ConvertUtil.ToInt(param[6].Value);
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
        }
        
        /// <summary>
        /// tổng số thẻ được nhận
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int VIPCardBonusCheckQuantity(long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;


                var result = db.ExecuteScalarSP("SP_VIP_CardBonus_CheckQuantity", param.ToArray());
                return ConvertUtil.ToInt(result);
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

            return 0;
        }
        /// <summary>
        /// lấy danh sách nhận thẻ
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<CardBonus> VIPCheckCardBonus(long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;


                var _lst = db.GetListSP<CardBonus>("SP_VIP_Check_CardBonus", param.ToArray());
                return _lst;
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

            return null;
        }
        /// <summary>
        /// mục đích kiểm tra xem có
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public VipCardRedemption VIPCardBonusCheck(long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;

                var _lstVipCardRedemption = db.GetInstanceSP<VipCardRedemption>("SP_VIP_CardBonus_Check", param.ToArray());
                return _lstVipCardRedemption;
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
            return null;
        }

        /// <summary>
        /// Nhận thưởng quý
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ServiceID"></param>
        /// <param name="QuaterDate"></param>
        /// <param name="LoanID"></param>
        /// <param name="QuaterAmount"></param>
        /// <param name="Response"></param>
        public void VIPReceiveQuaterPrize(long UserID, int ServiceID, DateTime QuaterDate, out long LoanID, out long QuaterAmount, out long RemainBalance, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_QuaterDate", SqlDbType.DateTime);
                param[1].Value = QuaterDate;

                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = ServiceID;
                param[3] = new SqlParameter("@_LoanID", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_QuaterAmount", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_Receive_QuaterPrize", param.ToArray());
                LoanID = ConvertUtil.ToLong(param[3].Value);
                QuaterAmount = ConvertUtil.ToLong(param[4].Value);
                Response = ConvertUtil.ToInt(param[5].Value);
                RemainBalance = ConvertUtil.ToLong(param[6].Value);
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
            QuaterAmount = 0;
            RemainBalance = 0;
            LoanID = 0;
        }


        /// <summary>
        /// hàm này hiển thị vay tiền và hiển thị thưởng quý
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
        public void VIPCheckQuaterLoan(long UserID, DateTime StartQuaterTime, DateTime EndQuaterTime, out int RankID, out int VPQuaterAcc, out int VipQuaterCoeff, out int QuaterPrizeStatus, out float LoanLimit, out float LoanRate, out long QuaterAcc, out long LoanAmount, out long OldDebt)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];
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
                QuaterPrizeStatus = param[11].Value==DBNull.Value?4:ConvertUtil.ToInt(param[11].Value);
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
            VipQuaterCoeff = 0;
            LoanLimit = 0;
            LoanRate = 0;
            QuaterAcc = 0;
            LoanAmount = 0;
            OldDebt = 0;
            QuaterPrizeStatus = 0;

        }


        /// <summary>
        /// xử lý quy trình vay
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <param name="Response"></param>
        /// <param name="UserID"></param>
        /// <param name="LoanAmount"></param>
        /// <param name="LoanID"></param>
        /// <param name="RemainBalance"></param>
        public void VIPLoanProcess(long UserID, int ServiceID, out long LoanAmount, out long LoanID, out long RemainBalance, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;

                param[2] = new SqlParameter("@_LoanAmount", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_LoanID", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_Loan_Process", param.ToArray());
                LoanAmount = ConvertUtil.ToLong(param[2].Value);
                LoanID = ConvertUtil.ToLong(param[3].Value);
                RemainBalance = ConvertUtil.ToLong(param[4].Value);
                Response = ConvertUtil.ToInt(param[5].Value);
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
            LoanAmount = 0;
            LoanID = 0;
            RemainBalance = 0;

        }
        /// <summary>
        /// xử lý quy trình hoàn tiền khi vay
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ServiceID"></param>
        /// <param name="Response"></param>
        /// <param name="LoanID"></param>
        /// <param name="ReturnAmount"></param>
        /// <param name="RemainBalance"></param>
        public void VIPLoanReturn(long UserID, int ServiceID, out long LoanID, out long ReturnAmount, out long RemainBalance, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

                param[1] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[1].Value = ServiceID;

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[2] = new SqlParameter("@_LoanID", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_ReturnAmount", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_VIP_Loan_Return", param.ToArray());
                LoanID = ConvertUtil.ToLong(param[2].Value);
                ReturnAmount = ConvertUtil.ToLong(param[3].Value);
                RemainBalance = ConvertUtil.ToLong(param[4].Value);
                Response = ConvertUtil.ToInt(param[5].Value);
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
            ReturnAmount = 0;
            RemainBalance = 0;

        }



    }
}