using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wewy.Models
{
    public class UIUser
    {
        public UIUser(ApplicationUser me)
        {
            this.Id = me.Id;
            this.Name = me.Nickname;
            this.Email = me.Email;
            this.City = me.City;
            this.TimezoneOffsetMinutes = me.TimezoneOffsetMinutes;
        }

        public UIUser()
        {
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public int TimezoneOffsetMinutes { get; set; }
    }
}