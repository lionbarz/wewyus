using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string AdminId { get; set; }
        public virtual ApplicationUser Admin { get; set; }
        public virtual List<ApplicationUser> Members { get; set; }
        public virtual List<Status> Statuses { get; set; }
        public virtual List<LastGroupVisit> LastGroupVisits { get; set; }
    }
}