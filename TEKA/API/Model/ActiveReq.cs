using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class ActiveReq
    {
        public string UserId { get; set; }
        public long IDProduct { get; set; }
        public string Buydate { get; set; }
        public string BuyAdr { get; set; }
        public APICustomer Customer { get; set; }
    }
    public class APICustomer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<int> Status { get; set; }
        public string CodeProduct { get; set; }
        public string Serial { get; set; }
        public Nullable<System.DateTime> ActiveDate { get; set; }
        public string NameAgency { get; set; }
        public string CodeAgency { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string ErrorNote { get; set; }
        public Nullable<int> Type { get; set; }
        public string Message { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public Nullable<int> ActiveWho { get; set; }
        public Nullable<System.DateTime> ErrorDate { get; set; }
        public Nullable<int> StatusError { get; set; }
        public Nullable<System.DateTime> Buydate { get; set; }
        public Nullable<System.DateTime> Installdate { get; set; }
        public string Ward { get; set; }
    }
}