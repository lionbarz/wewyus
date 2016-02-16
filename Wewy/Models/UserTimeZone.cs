using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class UserTimeZone
    {
        public int UserTimeZoneId { get; set; }
        public string Name { get; set; }
        public int Offset { get; set; }

        public virtual List<City> Cities { get; set; }
    }
}