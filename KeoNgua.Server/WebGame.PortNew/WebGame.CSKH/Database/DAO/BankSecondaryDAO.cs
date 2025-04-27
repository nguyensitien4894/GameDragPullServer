using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class BankSecondaryDAO
    {
        private static readonly Lazy<BankSecondaryDAO> _instance = new Lazy<BankSecondaryDAO>(() => new BankSecondaryDAO());

        public static BankSecondaryDAO Instance
        {
            get { return _instance.Value; }
        }
        public List<Bank> GetList(long ID, long TellID, string CardCode, string CardName, bool? Status, int CurrentPage, int RecordPerpage, int ServiceID, out long TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[9];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;
                param[1] = new SqlParameter("@_TellID", SqlDbType.BigInt);
                param[1].Value = TellID;
                param[2] = new SqlParameter("@_CardCode", SqlDbType.NVarChar);
                param[2].Size = 40;
                param[2].Value = CardCode;
                param[3] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[3].Value = Status.HasValue ? Status.Value : (object)DBNull.Value;
                param[4] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[4].Value = CurrentPage;
                param[5] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[5].Value = RecordPerpage;
                param[6] = new SqlParameter("@_TotalRecord", SqlDbType.BigInt);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_CardName", SqlDbType.NVarChar);
                param[7].Size = 40;
                param[7].Value = CardName;
                param[8] = new SqlParameter("@_ServiceID", ServiceID);

                var _lstCard = db.GetListSP<Bank>("SP_Bank_List_Paging", param.ToArray());
                TotalRecord = Convert.ToInt64(param[6].Value);
                return _lstCard;

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

        public void Cards_Handle(string CardCode, string CardName, long TeleID,  bool? Status, long CreateUser,  int ServiceID, out int ResponseStatus)
        {
            
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[7];


                param[0] = new SqlParameter("@_CardCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = CardCode;
                param[1] = new SqlParameter("@_CardName", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = CardName;
                param[2] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[2].Value = Status;
                param[3] = new SqlParameter("@_TeleID", SqlDbType.BigInt);
                param[3].Value = TeleID;
                param[4] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[4].Value = CreateUser;
                param[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_ServiceID", ServiceID);
                db.ExecuteNonQuerySP("SP_BankSecondary_Handle", param.ToArray());
                ResponseStatus = Convert.ToInt32(param[5].Value);
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
            ResponseStatus = -99;
        }
        public void Cards_Handle_Edit(string CardCode, string CardName, long TeleID, bool? Status, long CreateUser, int ServiceID, out int ResponseStatus,int ID)
        {

            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[8];
                param[0] = new SqlParameter("@_CardCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = CardCode;
                param[1] = new SqlParameter("@_CardName", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = CardName;
                param[2] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[2].Value = Status;
                param[3] = new SqlParameter("@_TeleID", SqlDbType.BigInt);
                param[3].Value = TeleID;
                param[4] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[4].Value = CreateUser;
                param[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;
                param[6] = new SqlParameter("@_ServiceID", ServiceID);
                param[7] = new SqlParameter("@_ID", ID);
                db.ExecuteNonQuerySP("SP_BankSecondary_Handle_Edit", param.ToArray());
                ResponseStatus = Convert.ToInt32(param[5].Value);
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
            ResponseStatus = -99;
        }
        public Bank BanksByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _Card = db.GetInstanceSP<Bank>("SP_Banks_GetByID", param.ToArray());
                return _Card;
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

    }
}