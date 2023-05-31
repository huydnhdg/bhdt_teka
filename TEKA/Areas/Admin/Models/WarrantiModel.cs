using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.Areas.Admin.Models
{
    public class WarrantiModel : Warranti
    {
        public string txtSearch1 { get; set; }
        public string txtSearch2 { get; set; }
        public string txtSearch3 { get; set; }
        public List<String> Image { get; set; }
        //public string Serial { get; set; }
        //public string Product { get; set; }
        public DateTime? Exportdate { get; set; }
        //public DateTime? Activedate { get; set; }
        //public int? Limited { get; set; }

        //--
        public long? IdKey { get; set; }
        public string Key { get; set; }
        public DateTime? Senddate { get; set; }
        public DateTime? Deadline { get; set; }

        //
        public DateTime? Successdate { get; set; }
        public string KTVNote { get; set; }
        public int? Cost { get; set; }
        public int? Amount { get; set; }
        public string Comback { get; set; }
        public List<Phutung_Warranti> Devices { get; set; }
        public string KTV { get; set; }
        //public string Image1 { get; set; }
        //public string Image2 { get; set; }
        //public string Image3 { get; set; }
        //public string Image4 { get; set; }
        //public string Image5 { get; set; }
        //public string Image6 { get; set; }

        public string Sign { get; set; }
        public string KM { get; set; }
   
    }
}