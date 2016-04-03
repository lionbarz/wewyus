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

        internal static List<StatusView> MakeStatusViews(string userId, Group group, Status status, DateTime utc)
        {
            var utcOffset = new DateTimeOffset(utc, TimeSpan.Zero);
            List<StatusView> views = new List<StatusView>();

            foreach (ApplicationUser viewer in group.Members)
            {
                // Calculate user's local time.
                var tz = TimeZoneInfo.FindSystemTimeZoneById(viewer.CurrentCity.UserTimeZone.WindowsRegistryName);
                DateTimeOffset userLocalDateOffset = utcOffset.ToOffset(tz.GetUtcOffset(utcOffset));

                views.Add(new StatusView()
                {
                    ViewerId = viewer.Id,
                    Viewer = viewer,
                    City = viewer.CurrentCity,
                    CityId = viewer.CurrentCityId,
                    LocalTime = userLocalDateOffset.DateTime,
                    Status = status,
                    StatusId = status.StatusId
                });
            }

            return views;
        }

        internal static List<UIStatusView> GetUIViews(List<StatusView> statusViews)
        {
            return statusViews.Select(
                v => new UIStatusView()
                {
                    ViewerName = v.Viewer.Nickname,
                    CityName = v.City.Name,
                    ViewTimeLocal = v.LocalTime
                }).ToList();
        }
    }
}