using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace TraditionGame.Utilities.IpAddress
{
    //public class IPGeographicalLocation
    //{
    //    [JsonProperty("ip")]
    //    public string IP { get; set; }

    //    [JsonProperty("type")]
    //    public string Type { get; set; }

    //    [JsonProperty("continent_code")]
    //    public string ContinentCode { get; set; }

    //    [JsonProperty("continent_name")]
    //    public string ContinentName { get; set; }

    //    [JsonProperty("country_code")]
    //    public string CountryCode { get; set; }

    //    [JsonProperty("country_name")]
    //    public string CountryName { get; set; }

    //    [JsonProperty("region_code")]
    //    public string RegionCode { get; set; }

    //    [JsonProperty("region_name")]
    //    public string RegionName { get; set; }

    //    [JsonProperty("city")]
    //    public string City { get; set; }

    //    [JsonProperty("zip")]
    //    public string Zip { get; set; }

    //    [JsonProperty("latitude")]
    //    public float Latitude { get; set; }

    //    [JsonProperty("longitude")]
    //    public float Longitude { get; set; }


    //    private IPGeographicalLocation() { }

    //    public static IPGeographicalLocation QueryGeographicalLocationAsync(string ipAddress)
    //    {
    //        try
    //        {
    //            using (var client = new HttpClient())
    //            {
    //                var response = client.GetStringAsync("http://api.ipstack.com/" + ipAddress + "?access_key=9c62228748c1da6696e6d589c76f6098");
    //                response.Wait();
    //                var result = response.Result;
    //                return JsonConvert.DeserializeObject<IPGeographicalLocation>(result);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            NLogManager.PublishException(ex);
    //        }
    //        return null;
    //    }
    //}
}
