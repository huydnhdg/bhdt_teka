using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class AddKTVReq
    {
        public string IDKTV { get; set; }
        public Nullable<long> IDWarranti { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public string Phone { get; set; }
    }
}