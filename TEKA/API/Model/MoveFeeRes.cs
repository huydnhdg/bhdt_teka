using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class MoveFeeRes : Result
    {
        public List<MoveFee> Data { get; set; }
    }
}