using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class CustomerRes : Result
    {
        public List<CustomerModel> Data { get; set; }
    }
    public class CustomerModel
    {
        public long? Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }
}