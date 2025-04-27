using MsWebGame.CSKH.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class SMSOTPDAO
    {
        private static readonly Lazy<SMSOTPDAO> _instance = new Lazy<SMSOTPDAO>(() => new SMSOTPDAO());

        public static SMSOTPDAO Instance
        {
            get { return _instance.Value; }
        }


        public List<SmsOtp> GetList(string NickName, string PhoneNo, string Type, int ServiceID,  DateTime? FromDate, DateTime? ToDate, int CurrentPage, int RecordPerpage, out long TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[9];
                param[0] = new SqlParameter("@_NickName", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = NickName;
                param[1] = new SqlParameter("@_PhoneNo", SqlDbType.VarChar);
                param[1].Size = 20;
                param[1].Value = PhoneNo;
                param[2] = new SqlParameter("@_Type", SqlDbType.VarChar);
                param[2].Size = 10;
                param[2].Value = Type;
                param[3] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[3].Value = ServiceID;
                param[4] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[4].Value = FromDate??(object)DBNull.Value;
                param[5] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[5].Value = ToDate?? (object)DBNull.Value;
                param[6] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[6].Value = CurrentPage;
                param[7] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[7].Value = RecordPerpage;
                
                param[8] = new SqlParameter("@_TotalRecord", SqlDbType.BigInt);
                param[8].Direction = ParameterDirection.Output;
               

                var _lstSmsOTP = db.GetListSP<SmsOtp>("SP_SmsOTP_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[8].Value);
                return _lstSmsOTP;
                
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
    }
}