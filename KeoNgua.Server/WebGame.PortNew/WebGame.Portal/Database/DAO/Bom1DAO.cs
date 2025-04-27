using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Database.DTO.EventBigBom;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Database.DAO
{
    public class Bom1DAO
    {
        private static readonly Lazy<Bom1DAO> _instance = new Lazy<Bom1DAO>(() => new Bom1DAO());


        public List<UserBom1AidBoxes> Bom1UserAidBoxHistory(long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =  new SqlParameter[1];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;

                var _lstBom = db.GetListSP<UserBom1AidBoxes>("SP_Bom1User_AidBox_History", param.ToArray());
                return _lstBom;
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



        public static Bom1DAO Instance
        {
            get { return _instance.Value; }
        }
        public Bom1User Bom1UserGetB1P(long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];

                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;

                var _Bom1User = db.GetInstanceSP<Bom1User>("SP_Bom1User_Get_B1P", param.ToArray());
                return _Bom1User;
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

        public List<DegreePrize> GetDegreePrize(long UserID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;

              
                var lstRs = db.GetListSP<DegreePrize>("SP_Bom1User_Degree_Prize", param.ToArray());
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public void Bom1AidBoxUserReceive(long AidBoxID, long UserID, out long TransID, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param = new SqlParameter[4];

                param[1] = new SqlParameter("@_AidBoxID", SqlDbType.BigInt);
                param[1].Value = AidBoxID;
             
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[2] = new SqlParameter("@_TransID", SqlDbType.BigInt);
                param[2].Direction = ParameterDirection.Output;
                param[3] = new SqlParameter("@_Response", SqlDbType.Int);
                param[3].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Bom1_AidBox_User_Receive", param.ToArray());
                Response = Convert.ToInt32(param[3].Value);
                if (Response == 1)
                {
                    TransID = Convert.ToInt64(param[2].Value);
                }else
                {
                    TransID = 0;
                }
               
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
            TransID = 0;
        }

        public int UserTransferReceive(long RemitterID, string RemitterName, int RemitterType, int RemitterLevel,long userId, long amount, string note, long transId, out long wallet)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_RemitterID", RemitterID);
                pars[1] = new SqlParameter("@_RemitterName", RemitterName);
                pars[2] = new SqlParameter("@_RemitterType", RemitterType);
                pars[3] = new SqlParameter("@_RemitterLevel", RemitterLevel);
                pars[4] = new SqlParameter("@_UserID", userId);
                pars[5] = new SqlParameter("@_Amount", amount);
                pars[6] = new SqlParameter("@_Note", note);
                pars[7] = new SqlParameter("@_TransID", transId);
                pars[8] = new SqlParameter("@_RemainWallet", SqlDbType.BigInt){ Direction = ParameterDirection.Output };
                pars[9] = new SqlParameter("@_Response", SqlDbType.Int) { Direction = ParameterDirection.Output };
                
                db.ExecuteNonQuerySP("SP_User_Transfer_Receive", pars);
                wallet = ConvertUtil.ToLong(pars[8].Value);
                return ConvertUtil.ToInt(pars[9].Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                wallet = 0;
                return -99;
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        public void Bom1UserDegree_Gratitude(int DegreeID, long UserID, string Description, out int Response, out long RefundAmount)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[5];
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[1] = new SqlParameter("@_DegreeID", SqlDbType.Int);
                param[1].Value = DegreeID;
               
                param[2] = new SqlParameter("@_Description", SqlDbType.NVarChar);
                param[2].Size = 1000;
                param[2].Value = Description;
                param[3] = new SqlParameter("@_RefundAmount", SqlDbType.BigInt);
                param[3].Direction = ParameterDirection.Output;
                param[4] = new SqlParameter("@_Response", SqlDbType.Int);
                param[4].Direction = ParameterDirection.Output;
                db.ExecuteNonQuerySP("SP_Bom1User_Degree_Gratitude", param.ToArray());
                Response = Convert.ToInt32(param[4].Value);
                if (Response == 1)
                {
                    RefundAmount = Convert.ToInt64(param[3].Value);
                }else
                {
                    RefundAmount = 0;
                }
            
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
            RefundAmount = 0;
            Response = -99;
        }
        public void Bom1UserRedemption(int DegreeID, long UserID, long RefundAmount, string Note, out long RemainBalance, out int Response)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =new SqlParameter[6];

                param[1] = new SqlParameter("@_DegreeID", SqlDbType.Int);
                param[1].Value = DegreeID;
                
                param[0] = new SqlParameter("@_UserID", SqlDbType.BigInt);
                param[0].Value = UserID;
                param[2] = new SqlParameter("@_RefundAmount", SqlDbType.BigInt);
                param[2].Value = RefundAmount;
                param[3] = new SqlParameter("@_Note", SqlDbType.NVarChar);
                param[3].Size = 400;
                param[3].Value = Note;
                param[4] = new SqlParameter("@_RemainBalance", SqlDbType.BigInt);
                param[4].Direction = ParameterDirection.Output;
                param[5] = new SqlParameter("@_Response", SqlDbType.Int);
                param[5].Direction = ParameterDirection.Output;

                db.ExecuteNonQuerySP("SP_Bom1User_Redemption", param.ToArray());
                Response = Convert.ToInt32(param[5].Value);
                if (Response == 1)
                {
                    RemainBalance = Convert.ToInt64(param[4].Value);
                }else
                {
                    RemainBalance = 0;
                }
                
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
            RemainBalance = 0;
            Response = -99;
        }

        public UserBom1AidBoxes UserBom1GiftBoxAidGetByID(long ID)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(Config.BettingConn);
                SqlParameter[] param =
             new SqlParameter[1];

                param[0] = new SqlParameter("@_ID", SqlDbType.BigInt);
                param[0].Value = ID;

                var _Bom1GiftBoxAid = db.GetInstanceSP<UserBom1AidBoxes>("SP_UserBom1AidBoxes_GetByID", param.ToArray());
                return _Bom1GiftBoxAid;
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