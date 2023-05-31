using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Areas.Admin.Models
{
    public class ProductCMSActive
    {
        public long? Id { get; set; }
        public string ProdName { get; set; }
        public string Code { get; set; }
        public string Serial { get; set; }
        public Nullable<DateTime> ActiveDate { get; set; }
        public Nullable<int> Limited { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string PhoneActive { get; set; }
        public string PhoneSend { get; set; }
        public Nullable<int> ProdStatus { get; set; }
        public string Cusname { get; set; }
        public string Address { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public Nullable<DateTime> Birthday { get; set; }
        public string Email { get; set; }
    }
}