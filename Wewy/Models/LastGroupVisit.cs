using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class LastGroupVisit
    {
        public int Id { get; set; }
        public DateTime VisitTimeUtc { get; set; }
        public string VisitorId { get; set; }
        public int GroupId { get; set; }
        public virtual ApplicationUser Visitor { get; set; }
        public virtual Group Group { get; set; }
    }
}