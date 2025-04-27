using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TraditionGame.Utilities;
using MsWebGame.CSKH.Database.DTO;
namespace MsWebGame.CSKH.Database.DAO
{
    public class CcuDAO
    {
        private static readonly Lazy<CcuDAO> _instance = new Lazy<CcuDAO>(() => new CcuDAO());

        public static CcuDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<CuuListModel> GetLists(DateTime? DateStart, DateTime? DateEnd, out long TotalAndroid, out long TotalIos, out long TotalWeb)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);

                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_DateStart", DateStart));
                param.Add(new SqlParameter("@_DateEnd", DateEnd));
                param.Add(new SqlParameter("@_TotalAndroid", SqlDbType.BigInt) { Direction = ParameterDirection.Output });
                param.Add(new SqlParameter("@_TotalIos", SqlDbType.BigInt) { Direction = ParameterDirection.Output });
                param.Add(new SqlParameter("@_TotalWeb", SqlDbType.BigInt) { Direction = ParameterDirection.Output });

                var lstRs = db.GetListSP<CuuListModel>("SP_CCU_GetList", param.ToArray());

                TotalAndroid = ConvertUtil.ToLong(param[2].Value);
                TotalIos = ConvertUtil.ToLong(param[3].Value);
                TotalWeb = ConvertUtil.ToLong(param[4].Value);

                return lstRs;
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
            TotalAndroid = 0;
            TotalIos = 0;
            TotalWeb = 0;
            return null;
        }

    }
}