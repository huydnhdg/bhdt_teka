using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class ReportImport
    {
        public int CountProduct { get; set; }
        public int Amount { get; set; }

        public List<int> Month { get; set; }
    }
}