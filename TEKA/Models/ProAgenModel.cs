using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Models
{
    public class ProAgenModel
    {
        public string Serial { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Nullable<DateTime> ActiveDate { get; set; }
        public int? Status { get; set; }
        public int? TT { get; set; }
        public string ThanhToan { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string AgencyCode { get; set; }
        public string AgencyName { get; set; }
        public string Username { get; set; }
        public string Money { get; set; }
        public string PXK { get; set; }
        public Nullable<DateTime> ExportDate { get; set; }
    }
}