using System;

namespace Wewy.Models
{
    /// <summary>
    /// A view is made by a person when he was in a certain city
    /// at a certain time, not necessarily the same city they're
    /// in now.
    /// </summary>
    public class StatusView
    {
        public int StatusViewId { get; set; }
        public int StatusId { get; set; }
        public string ViewerId { get; set; }
        public int CityId { get; set; }
        public DateTime LocalTime { get; set; }

        public virtual Status Status { get; set; }
        public virtual ApplicationUser Viewer { get; set; }
        public virtual City City { get; set; }
    }
}