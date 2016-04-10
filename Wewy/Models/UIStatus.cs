using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    // To be used for the frontend serialization.
    public class UIStatus
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsRtl { get; set; }
        public string CreatorName { get; set; }
        public string CreatorId { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public DateTime DateCreatedLocal { get; set; }
        public DateTime? DateModifiedUtc { get; set; }
        public DateTime? DateModifiedLocal { get; set; }
        // Location of the creator when creating this status.
        public string City { get; set; }
        public string Country { get; set; }
        // True if the status was created by the viewing user.
        public bool IsCreatedByUser { get; set; }
        public List<UIStatusView> Views { get; set; }
    }
}