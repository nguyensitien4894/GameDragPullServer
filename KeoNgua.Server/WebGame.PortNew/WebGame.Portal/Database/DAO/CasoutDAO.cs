using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class CasoutDAO
    {
        private static readonly Lazy<CasoutDAO> _instance = new Lazy<CasoutDAO>(() => new CasoutDAO());

        public static CasoutDAO Instance
        {
            get { return _instance.Value; }
        }
        public void Casout_Momo(long UserId,string Nickname, int Fee, string Phone, string Name,long Amount, int Service, out long Balance, out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = -99;

            /*
                @_UserId bigint,
	             @_Fee int,
	             @_SoTK varchar(50),
	             @_TenTK varchar(50),
	             @_Amount bigint,
	             @_BankCode varchar(50),
	             @_BankName varchar(50),
	             @_Status tinyint,
	             @_Service int,
             
             */
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[10];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = UserId;

                param[1] = new SqlParameter("@_Nickname", SqlDbType.NVarChar);
                param[1].Value = Nickname;

                param[2] = new SqlParameter("@_Fee", SqlDbType.Int);
                param[2].Size = 20;
                param[2].Value = Fee;

                param[3] = new SqlParameter("@_Phone", SqlDbType.NVarChar);
                param[3].Size = 20;
                param[3].Value = Phone; 
                
                param[4] = new SqlParameter("@_Name", SqlDbType.NVarChar);
                param[4].Size = 150;
                param[4].Value = Name;

                param[5] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[5].Value = Amount;

                param[6] = new SqlParameter("@_Status", SqlDbType.Int);
                param[6].Size = 20;
                param[6].Value = 10;

                param[7] = new SqlParameter("@_Service", SqlDbType.Int);
                param[7].Value = Service;


              

                param[8] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[8].Direction = ParameterDirection.Output;

                param[9] = new SqlParameter("@_Response", SqlDbType.Int);
                param[9].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Casout_Momo", param.ToArray());

                Balance = Convert.ToInt64(param[8].Value);
                Response = Convert.ToInt32(param[9].Value);
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
        }
        public void Casout_Bank(long UserId,int Fee,string Nickname, string SoTK,string TenTK,long Amount,string BankCode,string BankName,int Service, out long Balance, out int Response)
        {
            DBHelper db = null;
            Balance = 0;
            Response = -99;

            /*
                @_UserId bigint,
	             @_Fee int,
	             @_SoTK varchar(50),
	             @_TenTK varchar(50),
	             @_Amount bigint,
	             @_BankCode varchar(50),
	             @_BankName varchar(50),
	             @_Status tinyint,
	             @_Service int,
             
             */
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[12];
                param[0] = new SqlParameter("@_UserId", SqlDbType.BigInt);
                param[0].Value = UserId;

               

                param[1] = new SqlParameter("@_Fee", SqlDbType.Int);
                param[1].Size = 20;
                param[1].Value = Fee;

                param[2] = new SqlParameter("@_Nickname", SqlDbType.NVarChar);
                param[2].Size = 50;
                param[2].Value = Nickname;

                param[3] = new SqlParameter("@_SoTK", SqlDbType.NVarChar);
                param[3].Size = 50;
                param[3].Value = SoTK;

                param[4] = new SqlParameter("@_TenTK", SqlDbType.NVarChar);
                param[4].Size = 50;
                param[4].Value = TenTK;

                param[5] = new SqlParameter("@_Amount", SqlDbType.BigInt);
                param[5].Value = Amount;

                param[6] = new SqlParameter("@_BankCode", SqlDbType.NVarChar);
                param[6].Size = 50;
                param[6].Value = BankCode;

                param[7] = new SqlParameter("@_BankName", SqlDbType.NVarChar);
                param[7].Size = 50;
                param[7].Value = BankName;

                param[8] = new SqlParameter("@_Status", SqlDbType.Int);
                param[8].Size = 20;
                param[8].Value = 10;

                param[9] = new SqlParameter("@_Service", SqlDbType.Int);
                param[9].Value = Service;

                param[10] = new SqlParameter("@_RequestId", SqlDbType.Int);
                param[10].Value = Service;

                param[10] = new SqlParameter("@_Balance", SqlDbType.BigInt);
                param[10].Direction = ParameterDirection.Output;

                param[11] = new SqlParameter("@_Response", SqlDbType.Int);
                param[11].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Casout_Bank", param.ToArray());

                Balance = Convert.ToInt64(param[10].Value);
                Response = Convert.ToInt32(param[11].Value);
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
        }
    }
}