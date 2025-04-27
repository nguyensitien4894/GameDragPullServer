
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
    public class XoSoDAO
    {
        private static readonly Lazy<XoSoDAO> _instance = new Lazy<XoSoDAO>(() => new XoSoDAO());

        public static XoSoDAO Instance
        {
            get { return _instance.Value; }
        }

        public XoSo SessionGetInfo(DateTime OpenDate)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.XoSoConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_OpenDate", OpenDate));
                param.Add(new SqlParameter("@_CityID", 1));
                return  db.GetInstanceSP<XoSo>("SP_Session_GetInfo", param.ToArray());

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
        public int ResultSessionUpdate(long SessionID,string SpecialPrizeData,string FirstPrizeData,
            
            string SecondPrizeData,string ThirdPrizeData,string FourthPrizeData,string FifthPrizeData,string SixthPrizeData, string SeventhPrizeData,string EighthPrizeData)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.XoSoConn);
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@_CityID", 1));
                param.Add(new SqlParameter("@_SessionID", SessionID));
                param.Add(new SqlParameter("@_SpecialPrizeData", SpecialPrizeData));
                param.Add(new SqlParameter("@_FirstPrizeData",FirstPrizeData));
                param.Add(new SqlParameter("@_SecondPrizeData", SecondPrizeData));
                param.Add(new SqlParameter("@_ThirdPrizeData", ThirdPrizeData));
                param.Add(new SqlParameter("@_FourthPrizeData", FourthPrizeData));
                param.Add(new SqlParameter("@_FifthPrizeData", FifthPrizeData));
                param.Add(new SqlParameter("@_SixthPrizeData", SixthPrizeData));
                param.Add(new SqlParameter("@_SeventhPrizeData", SeventhPrizeData));
                param.Add(new SqlParameter("@_EighthPrizeData", EighthPrizeData));

                SqlParameter response = new SqlParameter("@_ResponseStatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                param.Add(response);

                db.ExecuteNonQuerySP("SP_ResultSession_Update", param.ToArray());

                return Convert.ToInt32(response.Value);
                

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

            return -99;
        }
    }
}