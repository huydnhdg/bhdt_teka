using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class AddKeyWarrReq
    {
        public long IDWarranti { get; set; }
        public string KeyWarranti { get; set; }
        public int Status { get; set; }
        public DateTime? Deadline { get; set; }
    }
}