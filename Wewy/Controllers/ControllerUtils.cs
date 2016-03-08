using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wewy.Models;

namespace Wewy.Controllers
{
    public class ControllerUtils
    {
        internal static bool IsRtl(string text)
        {
            return text.Length > 0 && (char)text[0] >= 1568 && (char)text[0] <= 1919;
        }

        internal static List<StatusView> MakeStatusViews(string userId, Group group, Status status, DateTime utc)
        {
            DateTimeOffset dateUtcOffset = new DateTimeOffset(utc);
            List<StatusView> views = new List<StatusView>();

            foreach (ApplicationUser viewer in group.Members)
            {
                // Offset in hours.
                TimeSpan offset = new TimeSpan(viewer.CurrentCity.UserTimeZone.Offset, 0, 0);
                DateTimeOffset dateOffset = dateUtcOffset.ToOffset(offset);
                views.Add(new StatusView()
                {
                    ViewerId = viewer.Id,
                    Viewer = viewer,
                    City = viewer.CurrentCity,
                    CityId = viewer.CurrentCityId,
                    LocalTime = dateOffset.DateTime,
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