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
        public int CreatorCityId { get; set; }
        public string Text { get; set; }

        [InverseProperty("Statuses")]
        public virtual Group Group { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        [ForeignKey("CreatorCityId")]
        public virtual City CreatorCity { get; set; }
        public virtual List<StatusView> StatusViews { get; set; }
    }
}