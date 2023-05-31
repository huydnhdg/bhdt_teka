using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Models
{
    public class Khachhang
    {

        //int id = 0, int StartPage = 0, string name = "", string serial = "", string status = "", string phone = "", string tungay = "", string denngay = ""
        public int id { get; set; }
        public int StartPage { get; set; }
        public string name { get; set; }
        public string serial { get; set; }
        public string status { get; set; }
        public string phone { get; set; }
        public string tungay { get; set; }
        public string denngay { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
    }
}