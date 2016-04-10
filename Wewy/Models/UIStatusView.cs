using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class UIStatusView
    {
        public string ViewerName { get; set; }
        public DateTime ViewTimeLocal { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}