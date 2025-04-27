using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TraditionGame.Utilities.MyUSDT.Models.Exchanges;
using System.IO;

namespace MsWebGame.Thecao.Controllers
{
    [RoutePrefix("api/CastOutBank")]
    public class CastOutBankController
    {
        [ActionName("GetListBankCode")]
        [HttpPost]
        public dynamic GetListBankCode()
        {
            // string apiKey = "4e344611-b324-47b1-9bb8-df353da48d03";
            string apiKey = "-";
            string url = "http://cucku.net:10007/api/Bank/getListBankCode?apiKey=" + apiKey;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                MopayListBankResponse objReturn = JsonConvert.DeserializeObject<MopayListBankResponse>(result);
                return objReturn;
            }
            catch (Exception ex)
            {
                MopayListBankResponse objReturn = new MopayListBankResponse();
                objReturn.stt = -99;
                
                return objReturn;
            }
        }

        [ActionName("ChargeOut")]
        [HttpPost]
        public dynamic ChargeOut()
        {
            return null;
        }
        [ActionName("CallBackResult")]
        [HttpPost]
        public dynamic CallBackResult([FromBody] CallBackSellorderRequestModel model)
        {
            return null;
        }

    }
    public class BankDataAccount
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class MopayListBankResponse
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public List<BankDataAccount> data { get; set; }
    }
}