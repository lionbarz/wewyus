using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class Status
    {
        public int StatusId { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public DateTime DateCreatedLocal { get; set; } // Local to the creator.
        public DateTime? DateModifiedUtc { get; set; }
        public DateTime? DateModifiedLocal { get; set; }
        public string CreatorId { get; set; }
        public int GroupId { get; set; }
        public string Text { get; set; }
        // Location info for the creator.
        public string City { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [InverseProperty("Statuses")]
        public virtual Group Group { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public virtual List<StatusView> StatusViews { get; set; }
    }
}