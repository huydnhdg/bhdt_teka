using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.Models
{
    public class ActiveSearch : Customer
    {
        public string Product { get; set; }
        public string Model { get; set; }

        public DateTime? Exportdate { get; set; }
    }
}