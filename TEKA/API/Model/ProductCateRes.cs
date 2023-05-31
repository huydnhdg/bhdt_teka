using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace TEKA.API.Model
{
    public class ProductCateRes:Result
    {
        public List<SelectListItem> Data { get; set; }
    }
}