using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    // To be used for the frontend serialization.
    public class UIStatus
    {
        public string Text { get; set; }
        public string CreatorName { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public DateTime DateCreatedCreator { get; set; }
        public DateTime DateCreatedLover { get; set; }

        public string CreatorCity { get; set; }
        public string LoverCity { get; set; }

        // True if the status was created by the viewing user.
        public bool IsCreatedByUser { get; set; }
    }
}