using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MsWebGame.CSKH.Database.DTO;
using MsWebGame.CSKH.Models.LuckySpin;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class LuckySpinDAO
    {
        private static readonly Lazy<LuckySpinDAO> _instance = new Lazy<LuckySpinDAO>(() => new LuckySpinDAO());

        public static LuckySpinDAO Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Lấy danh sách các cấu hình tặng số lượt quay trong ngày
        /// </summary>
        /// <returns></returns>
        public List<PresentSpins> GetPresentSpinsList(int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmConn);
                SqlParameter[] pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ServiceID", serviceid);
                return db.GetListSP<PresentSpins>("SP_PresentSpins_GetList",pars);
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

        /// <summary>
        /// Thêm mới cấu hình
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertPresentSpins(PresentSpinsModel model,int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmConn);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_Quantity", model.Quantity);
                pars[1] = new SqlParameter("@_StartDate", model.StartDate);
                pars[2] = new SqlParameter("@_EndDate", model.EndDate);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_ServiceID", serviceid);
                db.ExecuteNonQuerySP("SP_PresentSpins_Create", pars);
                return Int32.Parse(pars[3].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Xóa cấu hình 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeletePresentSpins(PresentSpinsModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmConn);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_PresentSpinID", model.ID);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db.ExecuteNonQuerySP("SP_PresentSpins_Delete", pars);
                return Int32.Parse(pars[1].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Danh sách giải vòng quay lớn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DBit> GetDBitList(int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmConn);
                SqlParameter[] pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ServiceID", serviceid);
                return db.GetListSP<DBit>("SP_DBit_GetList", pars);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Danh sách giải vòng quay lớn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<FreeSpin> GetFreeSpinList(int ServiceID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmConn);
                
                SqlParameter[] pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ServiceID", ServiceID);
                return db.GetListSP<FreeSpin>("SP_FreeSpin_GetList", pars);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Update Số lượng giải vòng quay lớn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateDBit(DBitModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmConn);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_PrizeID", model.PrizeID);
                pars[1] = new SqlParameter("@_New", model.New);
                pars[2] = new SqlParameter("@_Stone", model.Stone);
                pars[3] = new SqlParameter("@_BronzeSilver", model.BronzeSilver);
                pars[4] = new SqlParameter("@_GoldDiamond", model.GoldDiamond);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_ServiceID", model.ServiceID);
                db.ExecuteNonQuerySP("SP_Prize_NumberAward_Update", pars);
                return Int32.Parse(pars[5].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Update Số lượng giải vòng quay lớn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateFreeSpin(FreeSpinModel model)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmConn);
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_FreeSpinID", model.FreeSpinID);
                pars[1] = new SqlParameter("@_RoomID", model.RoomID);
                pars[2] = new SqlParameter("@_SpinQuantity", model.SpinQuantity);
                pars[3] = new SqlParameter("@_New", model.New);
                pars[4] = new SqlParameter("@_Stone", model.Stone);
                pars[5] = new SqlParameter("@_BronzeSilver", model.BronzeSilver);
                pars[6] = new SqlParameter("@_GoldDiamond", model.GoldDiamond);
                pars[7] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_ServiceID", model.ServiceID);
                db.ExecuteNonQuerySP("SP_FreeSpin_NumberAward_Update", pars);
                return Int32.Parse(pars[7].Value.ToString());
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return -99;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Lấy báo cáo cho vòng quay lớn
        /// </summary>
        /// <returns></returns>
        public List<DBitReport> GetDBitReport(ReportModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_PrizeID", model.PrizeID);
                pars[1] = new SqlParameter("@_Rank", model.Rank);
                pars[2] = new SqlParameter("@_StartDate", model.StartDate);
                pars[3] = new SqlParameter("@_EndDate", model.EndDate);
                pars[4] = new SqlParameter("@_CurrentPage", currentPage);
                pars[5] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.VqmmLogConn);
                var list = db.GetListSP<DBitReport>("SP_Report_DBit", pars);
                totalRecord = ConvertUtil.ToInt(pars[6].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Lấy báo cáo cho vòng quay lớn
        /// </summary>
        /// <returns></returns>
        public List<DBitReport> GetFreeSpinReport(ReportModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_FreeSpinID", model.FreeSpinID);
                pars[1] = new SqlParameter("@_Rank", model.Rank);
                pars[2] = new SqlParameter("@_StartDate", model.StartDate);
                pars[3] = new SqlParameter("@_EndDate", model.EndDate);
                pars[4] = new SqlParameter("@_CurrentPage", currentPage);
                pars[5] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.VqmmLogConn);
                var list = db.GetListSP<DBitReport>("SP_Report_FreeSpin", pars);
                totalRecord = ConvertUtil.ToInt(pars[6].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Lấy báo cáo cho vòng quay lớn
        /// </summary>
        /// <returns></returns>
        public List<DBitReport> GetAccountReport(ReportModel model, int currentPage, int recordPerpage, out int totalRecord)
        {
            DBHelper db = null;
            try
            {
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_FreeSpinID", model.FreeSpinID);
                pars[1] = new SqlParameter("@_PrizeID", model.PrizeID);
                pars[2] = new SqlParameter("@_Username", model.Username);
                pars[3] = new SqlParameter("@_Rank", model.Rank);
                pars[4] = new SqlParameter("@_StartDate", model.StartDate);
                pars[5] = new SqlParameter("@_EndDate", model.EndDate);
                pars[6] = new SqlParameter("@_ServiceID", model.ServiceID);
                pars[7] = new SqlParameter("@_CurrentPage", currentPage);
                pars[8] = new SqlParameter("@_RecordPerpage", recordPerpage);
                pars[9] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                db = new DBHelper(Config.VqmmLogConn);
                var list = db.GetListSP<DBitReport>("SP_Report_Account", pars);
                totalRecord = Convert.ToInt32(pars[9].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                totalRecord = 0;
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Danh sách rank user
        /// </summary>
        /// <returns></returns>
        public List<RankUser> GetRankUserList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmLogConn);
                return db.GetListSP<RankUser>("SP_Rank_GetList");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Lấy danh sách cái giải vòng quay lớn
        /// </summary>
        /// <returns></returns>
        public List<DBit> GetPrizeList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmLogConn);
                return db.GetListSP<DBit>("SP_Prize_GetList");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// Lấy danh sách các giải FreeSping
        /// </summary>
        /// <returns></returns>
        public List<FreeSpin> GetFreeSpinsList()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.VqmmLogConn);
                return db.GetListSP<FreeSpin>("SP_FreeSpin_GetList");
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                {
                    db.Close();
                }
            }
        }
    }
}