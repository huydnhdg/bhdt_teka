using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class ReportImportRes : Result
    {
        public List<ReportImport> Data { get; set; }
    }
}