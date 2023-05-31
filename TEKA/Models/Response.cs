using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TEKA.Models
{
    public class Response
    {
        public string message { get; set; }
        public int status { get; set; }//0 success -1 serial k ton tai 1 da duoc kich hoat truoc do
        public string activedate { get; set; }
        public string name { get; set; }
        public string limited { get; set; }
        public string productname { get; set; }
        public string enddate { get; set; }
        //public string maduthuong { get; set; }
    }
}