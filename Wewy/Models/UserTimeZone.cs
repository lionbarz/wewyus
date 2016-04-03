using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class UserTimeZone
    {
        public int UserTimeZoneId { get; set; }
        public string JavascriptName { get; set; }
        public string WindowsRegistryName { get; set; }

        public virtual List<City> Cities { get; set; }
    }
}