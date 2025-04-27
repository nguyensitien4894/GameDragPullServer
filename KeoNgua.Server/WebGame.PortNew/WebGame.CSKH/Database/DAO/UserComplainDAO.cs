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
    public class UserComplainDAO
    {
        private static readonly Lazy<UserComplainDAO> _instance = new Lazy<UserComplainDAO>(() => new UserComplainDAO());

        public static UserComplainDAO Instance
        {
            get { return _instance.Value; }
        }

        public void UserComplainCreate(long UserID, long ComplainTypeID, int ServiceID, string ResponseText, string Content, bool Status,  long UpdateUser, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];

               
                param[0] = new SqlParameter("@_ResponseText", SqlDbType.NText);
                param[0].Size = 1000;
                param[0].Value = ResponseText;
                param[1] = new SqlParameter("@_Content", SqlDbType.NText);
                param[1].Size = 1000;
                param[1].Value = Content;
              
                param[2] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[2].Value = UserID;
                param[3] = new SqlParameter("@_ComplainTypeID", SqlDbType.BigInt);
                param[3].Value = ComplainTypeID;
                param[4] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[4].Value = Status;
                param[5] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[5].Value = UpdateUser;
                param[6] = new SqlParameter("@_ServiceID", SqlDbType.Int);
                param[6].Value = ServiceID;
                param[7] = new SqlParameter("@_Response", SqlDbType.Int);
                param[7].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_UserComplain_Create", param.ToArray());
                Response = ConvertUtil.ToInt(param[7].Value);
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

        public List<UserComplain> UserComplainAdminList(long? ComplainType, bool? Status,long UserID, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

                param[0] = new SqlParameter("@_ComplainType", SqlDbType.Int);
                param[0].Value = ComplainType??(object)DBNull.Value;
                param[1] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[1].Value = UserID;
                param[2] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[2].Value = CurrentPage;
                param[3] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[3].Value = RecordPerpage;
                param[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[5].Value = Status??(object)DBNull.Value;
                var _lstUserComplain = db.GetListSP<UserComplain>("SP_UserComplain_Admin_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[4].Value);
                return _lstUserComplain;
              
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

        public List<UserComplainType> UserComplainTypeList(string Code,int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[4];
                param[0] = new SqlParameter("@_Code", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = Code;
                param[1] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[1].Value = CurrentPage;
                param[2] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[2].Value = RecordPerpage;
                param[3] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
              

                var _lstUserComplainType = db.GetListSP<UserComplainType>("SP_UserComplainType_List", param.ToArray());
                TotalRecord = ConvertUtil.ToInt(param[3].Value);
                return _lstUserComplainType;
               
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

        public UserComplain UserComplainGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _UserComplain = db.GetInstanceSP<UserComplain>("SP_UserComplain_GetByID", param.ToArray());
                return _UserComplain;
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


        public void UserComplainUpdate(long ID,string ResponseText, string Content, bool Status , long UpdateUser, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;
              
                param[1] = new SqlParameter("@_ResponseText", SqlDbType.NText);
                param[1].Size = 16;
                param[1].Value = ResponseText;
                param[2] = new SqlParameter("@_Content", SqlDbType.NText);
                param[2].Size = 16;
                param[2].Value = Content;
                param[3] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[3].Value = Status;
               
                param[4] = new SqlParameter("@_UpdateUser", SqlDbType.BigInt);
                param[4].Value = UpdateUser;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_UserComplain_Update", param.ToArray());
                Response = ConvertUtil.ToInt(param[5].Value);
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


    }
}