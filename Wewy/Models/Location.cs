using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    // Used to track the geographical location of a user or status post.
    public class Location
    {
        public Position Position { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}