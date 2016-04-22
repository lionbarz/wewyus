using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wewy.Models;

namespace Wewy.Controllers
{
    public class ControllerUtils
    {
        internal class UserComparer : IEqualityComparer<ApplicationUser>
        {
            public bool Equals(ApplicationUser x, ApplicationUser y)
            {
                if (x == y)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(ApplicationUser obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        internal static bool IsRtl(string text)
        {
            return text.Length > 0 && (char)text[0] >= 1568 && (char)text[0] <= 1919;
        }

        /// <summary>
        /// Make status views for everyone except the author of the status.
        /// </summary>
        /// <param name="authorUserId">The user ID of the status author.</param>
        /// <param name="group">Group where the status is being posted.</param>
        /// <param name="status">The status that is being viewed.</param>
        /// <param name="utc">The time of the posting.</param>
        /// <returns>List of status views.</returns>
        internal static List<StatusView> MakeStatusViews(string authorUserId, Group group, Status status, DateTime utc)
        {
            var utcOffset = new DateTimeOffset(utc, TimeSpan.Zero);
            List<StatusView> views = new List<StatusView>();

            foreach (ApplicationUser viewer in group.Members.Where(u => !u.Id.Equals(authorUserId)))
            {
                // Calculate user's local time.
                TimeSpan offset = new TimeSpan(
                    seconds: 0,
                    minutes: viewer.TimezoneOffsetMinutes,
                    hours: 0);
                DateTimeOffset userLocalDateOffset = utcOffset.ToOffset(offset);

                views.Add(new StatusView()
                {
                    ViewerId = viewer.Id,
                    Viewer = viewer,
                    City = viewer.City,
                    Country = viewer.Country,
                    Latitude = viewer.Latitude,
                    Longitude = viewer.Longitude,
                    LocalTime = userLocalDateOffset.DateTime,
                    Status = status,
                    StatusId = status.StatusId
                });
            }

            return views;
        }

        internal static List<UIStatusView> GetUIStatusViews(List<StatusView> statusViews)
        {
            return statusViews.Select(
                v => new UIStatusView()
                {
                    ViewerName = v.Viewer.Nickname,
                    City = v.City,
                    Country = v.Country,
                    ViewTimeLocal = v.LocalTime
                }).ToList();
        }
    }
}