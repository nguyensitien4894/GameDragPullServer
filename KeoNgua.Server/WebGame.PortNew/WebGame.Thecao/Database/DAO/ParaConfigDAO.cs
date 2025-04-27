using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using TraditionGame.Utilities;

namespace MsWebGame.Thecao.Database.DAO
{
    public class ParaConfigDAO
    {
        private static readonly Lazy<ParaConfigDAO> _instance = new Lazy<ParaConfigDAO>(() => new ParaConfigDAO());

        public static ParaConfigDAO Instance
        {
            get { return _instance.Value; }
        }
        /// <summary>
        /// get paramconfig
        /// </summary>
        /// <param name="ParamType"></param>
        /// <param name="Code"></param>
        /// <param name="Value"></param>
        public void GetConfigValue(string ParamType, string Code, out string Value)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_ParamType", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = ParamType;
                param[1] = new SqlParameter("@_Code", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = Code;
                param[2] = new SqlParameter("@_Value", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Param_Config_Get_Value", param.ToArray());
                Value = Convert.ToString(param[2].Value);
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
            Value = string.Empty;

        }
        public void GetCoreConfig(string ParamType, string Code, out string Value)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[3];

                param[0] = new SqlParameter("@_ParamType", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = ParamType;
                param[1] = new SqlParameter("@_Code", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = Code;
                param[2] = new SqlParameter("@_Value", SqlDbType.VarChar);
                param[2].Size = 50;
                param[2].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Param_Config_Get_Value", param.ToArray());
                Value = Convert.ToString(param[2].Value);
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
            Value = string.Empty;

        }
    }
}