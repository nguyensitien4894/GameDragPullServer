using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class AreaDAO
    {
        private static readonly Lazy<AreaDAO> _instance = new Lazy<AreaDAO>(() => new AreaDAO());

        public static AreaDAO Instance
        {
            get { return _instance.Value; }
        }


        public List<Area>  GetList(int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[0].Value = CurrentPage;
                param[1] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[1].Value = RecordPerpage;
                param[2] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Output;

                var _lstGetList = db.GetListSP<Area>("SP_Area_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[2].Value);
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