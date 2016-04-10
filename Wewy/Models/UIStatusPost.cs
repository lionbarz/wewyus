using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    /// <summary>
    /// Used for posting a status from the site.
    /// </summary>
    public class UIStatusPost
    {
        public string Text { get; set; }
        public Position Position { get; set; }
        public int TimezoneOffsetMinutes { get; set; }
    }
}