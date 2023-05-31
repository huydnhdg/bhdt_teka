using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.Areas.Admin.Models
{
    public class NoteModel : AddKey
    {
        [DisplayName("Trạng thái")]
        public int? status { get; set; }
        public string Category { get; set; }
        public List<Phutung_Warranti> ListPhutung { get; set; }
        public List<ImageWarr> Checkin { get; set; }
        public List<ImageWarr> Checkout { get; set; }



        public NoteModel()
        {
            this.ListPhutung = new List<Phutung_Warranti>();
            this.Checkin = new List<ImageWarr>();
            this.Checkout = new List<ImageWarr>();

        }
    }
}