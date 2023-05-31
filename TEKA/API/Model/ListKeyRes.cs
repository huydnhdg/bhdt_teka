using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.API.Model
{
    public class ListKeyRes : Result
    {
        public List<UserModel> Data { get; set; }
    }
}