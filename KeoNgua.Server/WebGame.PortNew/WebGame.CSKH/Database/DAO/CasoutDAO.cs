using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TraditionGame.Utilities;
using MsWebGame.CSKH.Database.DTO;

using Newtonsoft.Json;

namespace MsWebGame.CSKH.Database.DAO
{
    public class CasoutDAO
    {
        private static readonly Lazy<CasoutDAO> _instance = new Lazy<CasoutDAO>(() => new CasoutDAO());

        public static CasoutDAO Instance
        {
            get { return _instance.Value; }
        }
        public void UpdateStatus(long UserId,long Id,int Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = UserId;

                param[1] = new SqlParameter("@_Id", SqlDbType.BigInt);
                param[1].Value = Id;

                param[2] = new SqlParameter("@_Status", SqlDbType.BigInt);
                param[2].Value = Status;

                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                if (Status == 5)
                {
                    db.ExecuteNonQuerySP("SP_Casout_Bank_Cancel", param.ToArray());
                }
                else
                {
                    db.ExecuteNonQuerySP("SP_Casout_Bank_UpdateStatus", param.ToArray());
                }
                

                Response = Convert.ToInt32(param[3].Value);

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
        public List<CasoutBank> GetListBank(DateTime DateStart, DateTime DateEnd ,int Status,int maxRow, int CurrentPage,out int totalrecord)
        {
            DBHelper db = null;
            totalrecord = 0;
            List<CasoutBank> lstRs = new List<CasoutBank>();
            try
            {
                db = new DBHelper(Config.BettingConn);

                List<SqlParameter> param = new List<SqlParameter>();

                param.Add(new SqlParameter("@START_ROW", CurrentPage));
                param.Add(new SqlParameter("@MAX_ROWS", maxRow));
                param.Add(new SqlParameter("@STATUS", Status));

                param.Add(new SqlParameter("@DATE_START", DateStart));
                param.Add(new SqlParameter("@DATE_END", DateEnd));

                param.Add(new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output });


                //   @START_ROW int = NULL,
                //   @MAX_ROWS int = NULL,
                //   @STATUS int = NULL,
                //   @DATE_START datetime = NULL,
                //@DATE_END datetime = NULL,
                NLogManager.Debug(JsonConvert.SerializeObject(param));

                lstRs = db.GetListSP<CasoutBank>("SP_Casout_Bank_Pagination", param.ToArray());

                totalrecord = ConvertUtil.ToInt(param[5].Value);

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
            return lstRs;
        }
        public void UpdateStatusMomo(long UserId, long Id, int Status, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = UserId;

                param[1] = new SqlParameter("@_Id", SqlDbType.BigInt);
                param[1].Value = Id;

                param[2] = new SqlParameter("@_Status", SqlDbType.BigInt);
                param[2].Value = Status;

                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                if (Status == 5)
                {
                    db.ExecuteNonQuerySP("SP_Casout_Momo_Cancel", param.ToArray());
                }
                else
                {
                    db.ExecuteNonQuerySP("SP_Casout_Momo_UpdateStatus", param.ToArray());
                }
               

                Response = Convert.ToInt32(param[3].Value);

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
        public List<CasoutMomo> GetListMomo(DateTime DateStart, DateTime DateEnd, int Status, int maxRow, int CurrentPage, out int totalrecord)
        {
            DBHelper db = null;
            totalrecord = 0;
            List<CasoutMomo> lstRs = new List<CasoutMomo>();
            try
            {
                db = new DBHelper(Config.BettingConn);

                List<SqlParameter> param = new List<SqlParameter>();

                param.Add(new SqlParameter("@START_ROW", CurrentPage));
                param.Add(new SqlParameter("@MAX_ROWS", maxRow));
                param.Add(new SqlParameter("@STATUS", Status));

                param.Add(new SqlParameter("@DATE_START", DateStart));
                param.Add(new SqlParameter("@DATE_END", DateEnd));

                param.Add(new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output });


                //   @START_ROW int = NULL,
                //   @MAX_ROWS int = NULL,
                //   @STATUS int = NULL,
                //   @DATE_START datetime = NULL,
                //@DATE_END datetime = NULL,
                NLogManager.Debug(JsonConvert.SerializeObject(param));

                lstRs = db.GetListSP<CasoutMomo>("SP_Casout_Momo_Pagination", param.ToArray());

                totalrecord = ConvertUtil.ToInt(param[5].Value);

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
            return lstRs;
        }
    }

}