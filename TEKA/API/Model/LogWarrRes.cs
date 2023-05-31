using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class LogWarrRes : Result
    {
        public List<LogWaranti> Data { get; set; }
    }
}