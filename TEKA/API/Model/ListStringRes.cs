using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class ListStringRes : Result
    {
        public List<string> Data { get; set; }
    }
}