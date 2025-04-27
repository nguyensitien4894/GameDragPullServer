using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class BankDAO
    {

        private static readonly Lazy<BankDAO> _instance = new Lazy<BankDAO>(() => new BankDAO());

        public static BankDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// get danh sách bank
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="BankName"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="RecordPerpage"></param>
        /// <param name="TotalRecord"></param>
        /// <returns></returns>
        public List<Bank> GetList(string BankCode, string BankName, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[5];

                param[0] = new SqlParameter("@_BankCode", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = BankCode;
                param[1] = new SqlParameter("@_BankName", SqlDbType.NVarChar);
                param[1].Size = 200;
                param[1].Value = BankName;
                param[2] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[2].Value = CurrentPage;
                param[3] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[3].Value = RecordPerpage;
                param[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;

                var _lstBank = db.GetListSP<Bank>("SP_Bank_List", param.ToArray());
              
                TotalRecord = Convert.ToInt32(param[4].Value);
                return _lstBank;
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