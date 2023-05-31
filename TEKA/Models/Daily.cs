using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Models
{
    public class Daily
    {
        //int id = 0, int StartPage = 0, string name = "", string city = "", string county = ""
        public int id { get; set; }
        public int StartPage { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public IEnumerable<AspNetUser> AspNetUsers { get; set; }
    }
}