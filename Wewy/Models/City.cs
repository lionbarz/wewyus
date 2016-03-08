using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public int UserTimeZoneId { get; set; }

        public virtual UserTimeZone UserTimeZone { get; set; }

        // Statuses created from this city.
        [InverseProperty("CreatorCity")]
        public virtual List<Status> StatusesCreated { get; set; }

        // Users who currently reside in this city.
        [InverseProperty("CurrentCity")]
        public virtual List<ApplicationUser> Residents { get; set; }
    }
}