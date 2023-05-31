using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class ProvinceRes:Result
    {
        public List<Adr> Data { get; set; }
    }
    public class DistrictRes : Result
    {
        public List<Adr> Data { get; set; }

    }
    public class WardRes : Result
    {
        public List<Adr> Data { get; set; }
    }

    [Serializable]
    public class Adr
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}