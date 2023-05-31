using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.Areas.Admin.Models
{
    public class DeviceSendKeyModel : DeviceSendKey
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }
}