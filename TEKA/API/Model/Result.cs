using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class Result
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class ResultData :Result
    {
        public List<AgentModel> Data { get; set; }
    }
    public class AgentModel
    {
        public string UserName { get; set; }
        public string AgentName { get; set; }
        public string Id { get; set; }
    }
}