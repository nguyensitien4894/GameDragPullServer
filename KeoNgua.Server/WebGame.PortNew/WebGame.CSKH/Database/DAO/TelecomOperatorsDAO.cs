using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class TelecomOperatorsDAO
    {
        private static readonly Lazy<TelecomOperatorsDAO> _instance = new Lazy<TelecomOperatorsDAO>(() => new TelecomOperatorsDAO());

        public static TelecomOperatorsDAO Instance
        {
            get { return _instance.Value; }
        }

        public List<TelecomOperators> GetList(long id,string OperatorCode,int serviceid)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[0].Size = 20;
                param[0].Value = String.IsNullOrEmpty(OperatorCode)?(object)DBNull.Value:OperatorCode;
                param[1] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[1].Value = id;
                param[2] = new SqlParameter("@_ServiceID", serviceid);
                var _lstTelecomOperators = db.GetListSP<TelecomOperators>("SP_TelecomOperators_List", param.ToArray());
                return _lstTelecomOperators;
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
        /// update or edit telecom
        /// </summary>
        /// <param name="OperatorCode"></param>
        /// <param name="OperatorName"></param>
        /// <param name="Rate"></param>
        /// <param name="Status"></param>
        /// <param name="TelOperatorID"></param>
        /// <param name="CreateUser"></param>
        /// <param name=""></param>
        /// <param name="ResponseStatus"></param>
        public void TelecomOperatorsHandle(string OperatorCode, string OperatorName,double Rate,double ExchangeRate, bool Status,bool ExchangeStatus, long TelOperatorID, long CreateUser,int  serviceid, out int ResponseStatus)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[10];
                param[0] = new SqlParameter("@_TelOperatorID", SqlDbType.BigInt);
                param[0].Value = TelOperatorID;
                param[1] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
                param[1].Size = 20;
                param[1].Value = OperatorCode;
                param[2] = new SqlParameter("@_OperatorName", SqlDbType.NVarChar);
                param[2].Size = 100;
                param[2].Value = OperatorName;
                param[3] = new SqlParameter("@_Rate", SqlDbType.Float);
                param[3].Value = Rate;
                param[4] = new SqlParameter("@_Status", SqlDbType.Bit);
                param[4].Value = Status;
                param[5] = new SqlParameter("@_CreateUser", SqlDbType.BigInt);
                param[5].Value = CreateUser;
                param[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int);
                param[6].Direction = ParameterDirection.Output;
                param[7] = new SqlParameter("@_ExchangeRate", SqlDbType.Float);
                param[7].Value = ExchangeRate;
                param[8] = new SqlParameter("@_ExchangeStatus", SqlDbType.Bit);
                param[8].Value = ExchangeStatus;
                param[9] = new SqlParameter("@_ServiceID", serviceid);
                
                db.ExecuteNonQuerySP("SP_TelecomOperators_Handle", param.ToArray());
                ResponseStatus = Convert.ToInt32(param[6].Value);
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

        public TelecomOperators TelecomGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _Telecom = db.GetInstanceSP<TelecomOperators>("SP_TelecomOperators_GetByID", param.ToArray());
                return _Telecom;
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
        //public List<TelecomOperators> TelecomOperatorsList(long ID, string OperatorCode)
        //{
        //    DBHelper db = null;
        //    try
        //    {
        //        db = new DBHelper(Config.BettingConn);
        //        SqlParameter[] param =
        //     new SqlParameter[2];

        //        param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
        //        param[0].Value = ID;
        //        param[1] = new SqlParameter("@_OperatorCode", SqlDbType.VarChar);
        //        param[1].Size = 20;
        //        param[1].Value = OperatorCode;

        //        var _lstTelecomOperators = db.GetListSP<TelecomOperators>("SP_TelecomOperators_List", param.ToArray());
        //        return _lstTelecomOperators;
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //    }
        //    finally
        //    {
        //        if (db != null)
        //        {
        //            db.Close();
        //        }
        //    }
        //    return null;
        //}

    }
}