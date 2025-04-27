using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using TraditionGame.Utilities;
using TraditionGame.Utilities;
using MsWebGame.Thecao.Database.DTO;

namespace MsWebGame.Thecao.Database.DAO
{
    public class AgencyDAO
    {
        private static readonly Lazy<AgencyDAO> _instance = new Lazy<AgencyDAO>(() => new AgencyDAO());

        public static AgencyDAO Instance
        {
            get { return _instance.Value; }
        }
        public void CheckAgencyExist(string UserName, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@_UserName", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = UserName;
                param[1] = new SqlParameter("@_Response", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_CheckAgencyExist", param.ToArray());
                Response = Convert.ToInt32(param[1].Value);
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
        public List<Agency> GetList(string AccountName, string AccountCode, string PhoneNo, int? AccountLevel, long? ParrentID, int? Status, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_AccountName", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = AccountName;
                param[1] = new SqlParameter("@_AccountCode", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = AccountCode;
                param[2] = new SqlParameter("@_PhoneNo", SqlDbType.NVarChar);
                param[2].Size = 20;
                param[2].Value = PhoneNo;
                param[3] = new SqlParameter("@_AccountLevel", SqlDbType.SmallInt);
                param[3].Value = AccountLevel;
                param[4] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[4].Value = CurrentPage;
                param[5] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[5].Value = RecordPerpage;
                param[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_ParentID", SqlDbType.BigInt);
                param[7].Value = ParrentID;
                param[8] = new SqlParameter("@_Status", SqlDbType.Int);
                param[8].Value = Status.HasValue?Status.Value:(object)DBNull.Value;
                var _lstGetList = db.GetListSP<Agency>("SP_Agency_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[6].Value);
                return _lstGetList;

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