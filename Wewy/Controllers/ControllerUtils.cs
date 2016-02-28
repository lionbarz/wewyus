using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wewy.Models;

namespace Wewy.Controllers
{
    public class ControllerUtils
    {
        public static bool IsRtl(string text)
        {
            return text.Length > 0 && (char)text[0] >= 1568 && (char)text[0] <= 1919;
        }

        public static void GetNowDateTimes(Relationship relationship, out DateTime utc, out DateTime first, out DateTime second)
        {
            ApplicationUser applicationUserFirst = relationship.First;
            ApplicationUser applicationUserSecond = relationship.Second;

            // Offset in hours.
            TimeSpan offsetFirst = new TimeSpan(applicationUserFirst.CurrentCity.UserTimeZone.Offset, 0, 0);
            TimeSpan offsetSecond = new TimeSpan(applicationUserSecond.CurrentCity.UserTimeZone.Offset, 0, 0);

            utc = DateTime.UtcNow;
            DateTimeOffset dateUtcOffset = new DateTimeOffset(utc);
            DateTimeOffset dateFirstOffset = dateUtcOffset.ToOffset(offsetFirst);
            DateTimeOffset dateSecondOffset = dateUtcOffset.ToOffset(offsetSecond);
            first = dateFirstOffset.DateTime;
            second = dateSecondOffset.DateTime;
        }
    }
}