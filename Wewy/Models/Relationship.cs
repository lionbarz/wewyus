using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class Relationship
    {
        public int RelationshipId { get; set; }
        public string FirstId { get; set; }
        public string SecondId { get; set; }

        public virtual ApplicationUser First { get; set; }

        public virtual ApplicationUser Second { get; set; }

        public virtual List<Status> Statuses { get; set; }
    }
}