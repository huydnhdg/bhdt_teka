﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.API.Model
{
    public class SearchWarrantiRes : Result
    {
        public List<WarrantiModel> Data { get; set; }
    }
    public class WarrantiModel
    {
        public long Id { get; set; }
        public string Serial { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string CusName { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
        public DateTime? Createdate { get; set; }
        public string Createby { get; set; }
        public string KeyWarranti { get; set; }
        public string IdKey { get; set; }
        public string IdKTV { get; set; }
        public string KTV { get; set; }
        public string Comment { get; set; }
        public string ProductCate { get; set; }
        public string Model { get; set; }
        public string CodeWarr { get; set; }
        public List<LogWaranti> LogWarrantis { get; set; }
    }
}