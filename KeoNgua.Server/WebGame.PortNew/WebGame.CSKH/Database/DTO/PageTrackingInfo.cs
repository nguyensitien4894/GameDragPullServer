using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class PageTrackingInfo
    {
        public long ID { get; set; }

        public string UrlPage { get; set; }

        public string ClientIP { get; set; }

        public string DeviceID { get; set; }

        public int? ServiceID { get; set; }
        public int DownloadCount { get; set; }

        public int? AppType { get; set; }
        public string AppTypeStr
        {
            get
            {
                if (AppType == 1) return "GameIos";
                else if (AppType == 2) return "GameAndroid";
                else if (AppType == 3) return "AppSafeIos";
                else if (AppType == 4) return "AppSafeAndroid";
                else return "Không xác định";
            }
        }

        public DateTime? CreateDate { get; set; }
    }
}