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
            this.CityName = me.CurrentCity.Name;
            this.TimeZoneName = me.CurrentCity.UserTimeZone.Name;
        }

        public UIUser()
        {
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CityName { get; set; }
        public string TimeZoneName { get; set; }
    }
}