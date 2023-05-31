using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class DetailWarrantiRes:Result
    {
        public List<UpdateDetailReq> Data { get; set; }
    }
}