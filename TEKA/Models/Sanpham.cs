using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Models
{
    public class Sanpham
    {
        public int id { get; set; }
        public int StartPage { get; set; }
        public string name { get; set; }
        public string serial { get; set; }
        public string status { get; set; }
        public string tungay { get; set; }
        public string denngay { get; set; }
        public IEnumerable<Product> Products { get; set; }

    }
}