using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Areas.Admin.Models
{
    public class CreateWithProduct
    {
        public List<Prod> Products { get; set; }
        public Cus Cus { get; set; }
        public War War { get; set; }
        
    }
    public class Prod
    {
        public long? Id { get; set; }
        public string Serial { get; set; }
        public string Name { get; set; }
        public string ProductCate { get; set; }
        public string Model { get; set; }
        public DateTime? Buydate { get; set; }
        public string Agent { get; set; }
        public DateTime? Activedate { get; set; }
        public int? Limited { get; set; }
    }
    public class Cus
    {
        public long ? Id { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
    }
    public class War
    {
        public string Category { get; set; }
        public string Chanel { get; set; }
        public string Note { get; set; }
        public string Extra { get; set; }

    }
}