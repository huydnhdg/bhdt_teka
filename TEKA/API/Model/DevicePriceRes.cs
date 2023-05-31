using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class DevicePriceRes : Result
    {
        public List<PhutungPriceModel> Data { get; set; }
    }
    public class PhutungPriceModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Cate { get; set; }
        public string Phanloai { get; set; }
        public int? Price { get; set; }
        public DateTime? Createdate { get; set; }
    }
}