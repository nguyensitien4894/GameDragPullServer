using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Http
{
    public class UrlParseResponse
    {
        public string path { get; set; }
        public string fragment { get; set; }
        public Dictionary<string, string> queryStrings { get;set;}
    }
   public  class UrlParseHelper
    {
        public  static UrlParseResponse UrlParse(string Url)
        {
            if (String.IsNullOrEmpty(Url)) return null;
            var model = new UrlParseResponse();
            var uri = new Uri(Url);

            var scheme = uri.GetLeftPart(UriPartial.Scheme);
            var path = uri.GetLeftPart(UriPartial.Path);
            model.path = path;
            var fragment = uri.Fragment.TrimStart('#');
            model.fragment = fragment;
            var splitQuery = uri.Query.TrimStart('?').Split('&');
            var queryString = new Dictionary<string, string>();

            foreach (var item in splitQuery)
            {
                var splitItem = item.Split('=');
                var itemKey = splitItem[0];
                var itemValue = splitItem.Length > 1 ? splitItem[1] : string.Empty;

                if (!queryString.ContainsKey(itemKey))
                {
                    queryString.Add(itemKey, itemValue);
                   
                }
            }
            model.queryStrings = queryString;
            return model;
        }
    }
}
