using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.CSKH.Database.DTO;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Database.DAO
{
    public class GameParamConfigDAO
    {
        private static readonly Lazy<GameParamConfigDAO> _instance = new Lazy<GameParamConfigDAO>(() => new GameParamConfigDAO());

        public static GameParamConfigDAO Instance
        {
            get { return _instance.Value; }
        }


        public List<GameParamConfig> GetList(string ParamType, string Code, string Value, int CurrentPage, int RecordPerpage, out int TotalRecord)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[6];

                param[0] = new SqlParameter("@_ParamType", SqlDbType.NVarChar);
                param[0].Size = 100;
                param[0].Value = ParamType;
                param[1] = new SqlParameter("@_Code", SqlDbType.NVarChar);
                param[1].Size = 100;
                param[1].Value = Code;
                param[2] = new SqlParameter("@_Value", SqlDbType.NVarChar);
                param[2].Size = 20;
                param[2].Value = Value;
                param[3] = new SqlParameter("@_CurrentPage", SqlDbType.Int);
                param[3].Value = CurrentPage;
                param[4] = new SqlParameter("@_RecordPerpage", SqlDbType.Int);
                param[4].Value = RecordPerpage;
                param[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                var _lstParamConfig = db.GetListSP<GameParamConfig>("SP_ParamConfig_List", param.ToArray());
                TotalRecord = Convert.ToInt32(param[5].Value);
                return _lstParamConfig;

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
            return new List<GameParamConfig>();
        }

        public GameParamConfig ParamConfigGetByKey(string ParamType, string Code)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[2];

                param[0] = new SqlParameter("@_ParamType", SqlDbType.VarChar);
                param[0].Size = 50;
                param[0].Value = ParamType;
                param[1] = new SqlParameter("@_Code", SqlDbType.VarChar);
                param[1].Size = 50;
                param[1].Value = Code;
                var _GameParamConfig = db.GetInstanceSP<GameParamConfig>("SP_ParamConfig_GetByKey", param.ToArray());
                return _GameParamConfig;
            
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

        public void Update(int ID, string Value, string Description, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
             new SqlParameter[4];

                param[0] = new SqlParameter("@_ID", SqlDbType.Int);
                param[0].Value = ID;
                param[1] = new SqlParameter("@_Value", SqlDbType.VarChar);
                param[1].Size = 20;
                param[1].Value = Value;
                param[2] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[2].Size = 400;
                param[2].Value = Description;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_ParamConfig_Update", param.ToArray());
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

    }
}