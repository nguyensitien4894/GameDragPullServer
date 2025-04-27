
using MsWebGame.CSKH.Database.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class GameBaiDAO
    {
        private static readonly Lazy<GameBaiDAO> _instance = new Lazy<GameBaiDAO>(() => new GameBaiDAO());
        public static GameBaiDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<GameBai> GetList(string TextSearch, int GameID, int? ServiceID, int SearchBy, DateTime FromDate, DateTime ToDate,
            int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingLogConn);
                SqlParameter[] param = new SqlParameter[9];
                param[0] = new SqlParameter("@_TextSearch", SqlDbType.VarChar);
                param[0].Size = 100;
                param[0].Value = TextSearch;
                param[1] = new SqlParameter("@_GameID", SqlDbType.Int);
                param[1].Value = GameID;
                param[2] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[2].Value = ServiceID??-1;
                param[3] = new SqlParameter("@_SearchBy", SqlDbType.Int);
                param[3].Value = SearchBy;
                param[4] = new SqlParameter("@_FromDate", SqlDbType.DateTime);
                param[4].Value = FromDate;
                param[5] = new SqlParameter("@_ToDate", SqlDbType.DateTime);
                param[5].Value = ToDate;

                param[6] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[6].Value = CurrentPage;
                param[7] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[7].Value = RecordPerpage;
                param[8] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[8].Direction = ParameterDirection.Output;
              
                var _lstGameBai = db.GetListSP<GameBai>("SP_HistoryCardGame_Search", param.ToArray());
                TotalRecord = ConvertUtil.ToInt(param[8].Value);
                return _lstGameBai;
              
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