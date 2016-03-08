using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class UIGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UIUser> Members { get; set; }
    }
}