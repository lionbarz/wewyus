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
        public DateTime DateCreatedCreator { get; set; }
        public DateTime DateCreatedLover { get; set; }
        public string CreatorId { get; set; }
        public int RelationshipId { get; set; }
        public string Text { get; set; }
        public virtual Relationship Relationship { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public int CreatorCityId { get; set; }

        [ForeignKey("CreatorCityId")]
        public virtual City CreatorCity { get; set; }

        public int LoverCityId { get; set; }

        [ForeignKey("LoverCityId")]
        public virtual City LoverCity { get; set; }
    }
}