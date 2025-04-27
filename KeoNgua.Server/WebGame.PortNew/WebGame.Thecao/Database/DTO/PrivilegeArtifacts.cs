using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class PrivilegeArtifacts
    {
        public long PriArtID { get; set; }

        public int PrivilegeID { get; set; }

        public int ArtifactID { get; set; }

        public int? Quantity { get; set; }

        public int? RemainQuantity { get; set; }

        public long? TotalPrize { get; set; }

        public bool Status { get; set; }

        public string Description { get; set; }

        public long? CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public long? UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}