using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MsWebGame.CSKH.Helpers
{
    public class Esports
    {
        private static string channel_key = "HkhNY4xtUTX8b2Jm";
        public static EsportsDataBalance GetBalance(string displayName)
        {
            try
            {

                string url = "https://gon68.xyz/get-balance?username=" + displayName + "&channel_key=" + channel_key;
               
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                EsportsDataBalance objReturn = JsonConvert.DeserializeObject<EsportsDataBalance>(result);
                return objReturn;
            }
            catch (Exception ex)
            {

               
                return new EsportsDataBalance();
            }
        }
    }
    public class EsportsDataBalance
    {
        public class EsportsInfo
        {
            public string name { set; get; }
            public double balance { set; get; }
        }
        // //{"error_code":1,"message":"success","data":[{"name":"Casino","balance":0},{"name":"Saba","balance":0}]}
        public int error_code { set; get; } = -1;
        public string message { set; get; } = "none";
        public List<EsportsInfo> data { set; get; } = new List<EsportsInfo>();
        public double GetSabeCoin()
        {
            EsportsInfo esportsInfo = data.Where(e => { return e.name == "Saba"; }).FirstOrDefault();
            return esportsInfo != null ? esportsInfo.balance : 0;
        }
        public double GetCasinoCoin()
        {
            EsportsInfo esportsInfo = data.Where(e => { return e.name == "Casino"; }).FirstOrDefault();
            return esportsInfo != null ? esportsInfo.balance : 0;
        }
        public double GetSabaToGame()
        {
            return GetSabeCoin() / 0.00083d;
        }
    }
}