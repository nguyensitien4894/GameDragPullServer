using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TraditionGame.Utilities.Api
{
   public  class ApiUtil<T>
    {
        public string ApiAddress { get; set; }
        public string URI { get; set; }

        private  void SetHeaderHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri(ApiAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public T Send(dynamic input)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    SetHeaderHttpClient(client);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
                    var response =  client.PostAsync(URI, content);
                    response.Wait();
                    var result = response.Result;
                   // NLogManager.LogMessage("Send API" + JsonConvert.SerializeObject(result.StatusCode));
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        return JsonConvert.DeserializeObject<T>(readTask.Result);
                    }
                }
                return default(T);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                NLogManager.LogMessage("Send API ERROR:" + URI);
                return default(T);
            }
        }
       
    }
}
