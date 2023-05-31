using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class ProductRes : Result
    {
        public List<ProductActiveModel> Data { get; set; }
    }
    public class ProductActiveModel : Product
    {
        public DateTime? Activedate { get; set; }
        public string BuyAdr { get; set; }

    }
}