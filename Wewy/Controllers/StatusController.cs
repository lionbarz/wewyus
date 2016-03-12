using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Wewy.Models;
using Wewy.Services;

namespace Wewy.Controllers
{
    [Authorize]
    public class StatusController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        const int QueryPageSize = 25;

        // Right now we just return the text for the status edit page.
        [ResponseType(typeof(UIStatus))]
        public async Task<IHttpActionResult> GetStatus(int id)
        {
            string userId = User.Identity.GetUserId();

            Status status = await db.Status.FindAsync(id);

            if (!status.Creator.Id.Equals(userId))
            {
                return BadRequest("You don't own this status.");
            }

            UIStatus uiStatus = new UIStatus()
            {
                Text = status.Text
            };

            return Ok(uiStatus);
        }

        // GET: api/Status
        [ResponseType(typeof(UIStatus[]))]
        public async Task<IHttpActionResult> GetStatuses(int groupId)
        {
            string userId = User.Identity.GetUserId();

            Group group = await db.Groups.FindAsync(groupId);

            if (group == null)
            {
                return BadRequest(string.Format("Group {0} doesn't exit.", groupId));
            }

            if (!group.Members.Any(x => x.Id.Equals(userId)))
            {
                return BadRequest("You're not in this group.");
            }

            string myName = User.Identity.GetUserName();

            var statuses = await db.Status
                .Where(s => s.GroupId == group.GroupId)
                .Select(s => s)
                .OrderByDescending(s => s.DateCreatedUtc)
                .Take(QueryPageSize)
                .ToListAsync();

            UIStatus[] uiStatuses = statuses.Select(
                s => new UIStatus()
                {
                    Id = s.StatusId,
                    CreatorName = s.Creator.Nickname,
                    CreatorId = s.Creator.Id,
                    DateCreatedUtc = s.DateCreatedUtc,
                    DateCreatedLocal = s.DateCreatedLocal,
                    DateModifiedUtc = s.DateModifiedUtc,
                    DateModifiedLocal = s.DateModifiedLocal,
                    Text = s.Text,
                    IsRtl = ControllerUtils.IsRtl(s.Text),
                    CreatorCity = s.CreatorCity.Name,
                    IsCreatedByUser = s.Creator.UserName.Equals(myName),
                    Views = ControllerUtils.GetUIViews(s.StatusViews)
                }).ToArray();

            return Ok(uiStatuses);
        }

        // GET: api/Status
        [ResponseType(typeof(UIStatus[]))]
        public async Task<IHttpActionResult> GetStatuses(int groupId, string date)
        {
            DateTime target = DateTime.Parse(date);
            DateTime targetPlusDay = target.AddDays(1);

            string userId = User.Identity.GetUserId();

            Group group = await db.Groups.FindAsync(groupId);

            if (group == null)
            {
                return BadRequest(string.Format("Group {0} doesn't exit.", groupId));
            }

            if (!group.Members.Any(x => x.Id.Equals(userId)))
            {
                return BadRequest("You're not in this group.");
            }

            string myName = User.Identity.GetUserName();

            var statuses = await db.Status
                .Where(s => s.GroupId == group.GroupId && s.DateCreatedLocal > target && s.DateCreatedLocal < targetPlusDay)
                .Select(s => s)
                .OrderByDescending(s => s.DateCreatedLocal)
                .Take(QueryPageSize)
                .ToListAsync();

            UIStatus[] uiStatuses = statuses.Select(
                s => new UIStatus()
                {
                    Id = s.StatusId,
                    CreatorName = s.Creator.Nickname,
                    CreatorId = s.Creator.Id,
                    DateCreatedUtc = s.DateCreatedUtc,
                    DateCreatedLocal = s.DateCreatedLocal,
                    DateModifiedUtc = s.DateModifiedUtc,
                    DateModifiedLocal = s.DateModifiedLocal,
                    Text = s.Text,
                    IsRtl = ControllerUtils.IsRtl(s.Text),
                    CreatorCity = s.CreatorCity.Name,
                    IsCreatedByUser = s.Creator.UserName.Equals(myName),
                    Views = ControllerUtils.GetUIViews(s.StatusViews)
                }).ToArray();

            return Ok(uiStatuses);
        }

        // PUT: api/Status
        // Right now we only allow editing the text.
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStatus(int id, UIStatus uiStatus)
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            Status status = await db.Status.FindAsync(id);

            if (status == null)
            {
                return BadRequest(string.Format("Status {0} doesn't exist.", id));
            }

            if (!status.Creator.Id.Equals(userId))
            {
                return BadRequest("You don't own this status.");
            }

            ApplicationUser applicationUser = db.Users.Find(userId);

            // Offset in hours.
            TimeSpan offset = new TimeSpan(applicationUser.CurrentCity.UserTimeZone.Offset, 0, 0);

            DateTime utc = DateTime.UtcNow;
            DateTimeOffset dateUtcOffset = new DateTimeOffset(utc);
            DateTimeOffset dateLocalOffset = dateUtcOffset.ToOffset(offset);
            DateTime localTime = dateLocalOffset.DateTime;

            if (status.Creator.Id != userId)
            {
                return Unauthorized();
            }

            status.Text = uiStatus.Text;
            status.DateModifiedUtc = utc;
            status.DateModifiedLocal = localTime;
            db.Status.Attach(status);
            var entry = db.Entry(status);
            entry.Property(e => e.Text).IsModified = true;
            entry.Property(e => e.DateModifiedUtc).IsModified = true;
            entry.Property(e => e.DateModifiedLocal).IsModified = true;
            await db.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Status
        [ResponseType(typeof(UIStatus))]
        public async Task<IHttpActionResult> PostStatus(int groupId, UIStatus uiStatus)
        {
            string userId = User.Identity.GetUserId();

            Group group = await db.Groups.FindAsync(groupId);

            if (group == null)
            {
                return BadRequest(string.Format("Group {0} doesn't exit.", groupId));
            }

            if (!group.Members.Any(x => x.Id.Equals(userId)))
            {
                return BadRequest("You're not in this group.");
            }

            ApplicationUser applicationUser = db.Users.Find(userId);

            // Offset in hours.
            TimeSpan offset = new TimeSpan(applicationUser.CurrentCity.UserTimeZone.Offset, 0, 0);

            DateTime utc = DateTime.UtcNow;
            DateTimeOffset dateUtcOffset = new DateTimeOffset(utc);
            DateTimeOffset dateLocalOffset = dateUtcOffset.ToOffset(offset);
            DateTime localTime = dateLocalOffset.DateTime;

            Status status = new Status()
            {
                CreatorId = User.Identity.GetUserId(),
                GroupId = group.GroupId,
                Text = uiStatus.Text,
                DateCreatedUtc = utc,
                DateCreatedLocal = localTime,
                CreatorCityId = applicationUser.CurrentCity.CityId,
                Creator = applicationUser,
                CreatorCity = applicationUser.CurrentCity,
                Group = group,
            };
            
            status.StatusViews = ControllerUtils.MakeStatusViews(userId, group, status, utc);

            db.Status.Add(status);
            await db.SaveChangesAsync();

            uiStatus.Id = status.StatusId;
            uiStatus.DateCreatedUtc = status.DateCreatedUtc;
            uiStatus.DateCreatedLocal = status.DateCreatedLocal;
            uiStatus.CreatorCity = status.CreatorCity.Name;
            uiStatus.CreatorName = status.Creator.Nickname;
            uiStatus.CreatorId = status.Creator.Id;
            uiStatus.IsCreatedByUser = true;
            uiStatus.IsRtl = ControllerUtils.IsRtl(uiStatus.Text);
            uiStatus.Views = ControllerUtils.GetUIViews(status.StatusViews);
            uiStatus.Text = status.Text;

            return Ok(uiStatus);
        }
    }
}