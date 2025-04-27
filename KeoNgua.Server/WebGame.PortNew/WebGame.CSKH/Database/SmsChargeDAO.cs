using MsWebGame.CSKH.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database
{
    public class SmsChargeDAO
    {
        private static readonly Lazy<SmsChargeDAO> _instance = new Lazy<SmsChargeDAO>(() => new SmsChargeDAO());

        public static SmsChargeDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<UserSmsRequest> UserSmsRequestList(long? RequestID, long? UserID, string RefKey, string Phone, string NickName,
            int? PartnerID, int? ServiceID, DateTime? FromRequestDate, DateTime ?ToRequestDate
            , int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];

                param[0] = new SqlParameter("@_RequestID", SqlDbType.BigInt);
                param[0].Value = RequestID??(object)DBNull.Value;
                param[1] = new SqlParameter("@_RefKey", SqlDbType.VarChar);
                param[1].Size = 250;
                param[1].Value = RefKey??(object)DBNull.Value;
                param[2] = new SqlParameter("@_Phone", SqlDbType.VarChar);
                param[2].Size = 20;
                param[2].Value = Phone??(object)DBNull.Value;
                param[3] = new SqlParameter("@_PartnerID", SqlDbType.Int);
                param[3].Value = PartnerID ?? (object)DBNull.Value;

                param[4] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[4].Size = 20;
                param[4].Value = NickName ?? (object)DBNull.Value;
                param[5] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[5].Value = UserID??(object)DBNull.Value;

                param[6] = new SqlParameter("@_FromRequestDate", SqlDbType.DateTime);
                param[6].Value = FromRequestDate ?? (object)DBNull.Value;
                param[7] = new SqlParameter("@_ToRequestDate", SqlDbType.DateTime);
                param[7].Value = ToRequestDate??(object)DBNull.Value;
                param[8] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[8].Value = ServiceID?? (object)DBNull.Value;
                param[9] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[9].Value = CurrentPage;
                param[10] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[10].Value = RecordPerpage;
                param[11] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;
             
             
            
             
              

                var _lstUserSmsRequest = db.GetListSP<UserSmsRequest>("SP_UserSmsRequest_List", param.ToArray());
                TotalRecord = ConvertUtil.ToInt(param[11].Value);
                return _lstUserSmsRequest;
               
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
            TotalRecord = 0;
            return null;
        }



        public void SmsOperatorrUpdate(long ID,bool Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];


                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;
                param[1] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[1].Value = Status;
               


                param[2] = new SqlParameter("@_Response", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_SmsOperator_Update", param.ToArray());
                Response = Convert.ToInt32(param[2].Value);
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

        public SmsOperators SmsOperatorrGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _CardPartners = db.GetInstanceSP<SmsOperators>("SP_SmsOperator_GetByID", param.ToArray());
                return _CardPartners;
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


        public List<SmsOperators> SmsOperatorList(int ServiceID, string OperatorCode, string Telecom, string OperatorName)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = OperatorCode ?? (object)DBNull.Value;
                param[1] = new SqlParameter("@_OperatorName", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = OperatorName ?? (object)DBNull.Value;
                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = ServiceID;

                param[3] = new SqlParameter("@_Telecom", SqlDbType.VarChar);
                param[3].Size = 20;
                param[3].Value = Telecom ?? (object)DBNull.Value;


                var _lstSmsOperator = db.GetListSP<SmsOperators>("SP_SmsOperator_List", param.ToArray());
                return _lstSmsOperator;
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
    }
}