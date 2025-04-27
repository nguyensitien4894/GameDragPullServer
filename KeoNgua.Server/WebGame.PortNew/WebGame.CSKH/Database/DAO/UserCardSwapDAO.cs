using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class UserCardSwapDAO
    {
        private static readonly Lazy<UserCardSwapDAO> _instance = new Lazy<UserCardSwapDAO>(() => new UserCardSwapDAO());

        public static UserCardSwapDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<UserCardSwap> GetList( long AccountID, long?CardValue, string CardNumber, string CardSerial, string TelOperatorCode, int Status, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[9];

                param[0] = new SqlParameter("@_AccountID", SqlDbType.BigInt);
                param[0].Value = AccountID;
                param[1] = new SqlParameter("@_CardNumber", SqlDbType.NVarChar);
                param[1].Size = 40;
                param[1].Value = CardNumber;
                param[2] = new SqlParameter("@_CardSerial", SqlDbType.NVarChar);
                param[2].Size = 40;
                param[2].Value = CardSerial;
                param[3] = new SqlParameter("@_CardValue", SqlDbType.BigInt);
                param[3].Value = CardValue;
                param[4] = new SqlParameter("@_TelOperatorCode", SqlDbType.NVarChar);
                param[4].Size = 100;
                param[4].Value = TelOperatorCode;
                param[5] = new SqlParameter("@_Status", SqlDbType.Int);
                param[5].Value = Status;
                param[6] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[6].Value = CurrentPage;
                param[7] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[7].Value = RecordPerpage;
                param[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;
                var _lstUserCardSwap = db.GetListSP<UserCardSwap>("SP_UserCardSwap_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[8].Value);
                return _lstUserCardSwap;
                
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