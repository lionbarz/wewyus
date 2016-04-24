using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            return await GetStatuses(groupId, new DateTime(0));
        }

        /// <summary>
        /// Get statuses for a group.
        /// </summary>
        /// <param name="groupId">The group to which the statuses belong.</param>
        /// <param name="sinceWhen">The time constraint.</param>
        /// <returns>
        /// Returns one page size of statuses that belong to the group and that
        /// were created strictly after the given timestamp and in descending order
        /// according to their creation date.
        /// </returns>
        // GET: api/Status
        [ResponseType(typeof(UIStatus[]))]
        public async Task<IHttpActionResult> GetStatuses(int groupId, DateTime sinceWhen)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser me = db.Users.Find(userId);

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
                .Where(s => s.GroupId == group.GroupId && s.DateCreatedUtc > sinceWhen)
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
                    City = s.City,
                    IsCreatedByUser = s.Creator.UserName.Equals(myName),
                    Views = ControllerUtils.GetUIStatusViews(s.StatusViews)
                }).ToArray();

            // Record group visit.
            LastGroupVisit lastVisit = me.LastGroupVisits.Where(x => x.GroupId == groupId).FirstOrDefault();
            if (lastVisit == null)
            {
                db.LastGroupVisits.Add(new LastGroupVisit()
                {
                    GroupId = groupId,
                    Group = group,
                    Visitor = me,
                    VisitorId = me.Id,
                    VisitTimeUtc = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
            else
            {
                lastVisit.VisitTimeUtc = DateTime.UtcNow;
                db.LastGroupVisits.Attach(lastVisit);
                db.Entry(lastVisit).Property(x => x.VisitTimeUtc).IsModified = true;
                await db.SaveChangesAsync();
            }

            return Ok(uiStatuses);
        }

        // PUT: api/Status
        // Right now we only allow editing the text.
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStatus(int id, UIStatusPost uiStatusPost)
        {
            string userId = User.Identity.GetUserId();

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

            DateTime utc = DateTime.UtcNow;
            DateTimeOffset dateUtcOffset = new DateTimeOffset(utc, TimeSpan.Zero);
            TimeSpan offset = new TimeSpan(
                hours: 0,
                minutes: -uiStatusPost.TimezoneOffsetMinutes, // Negative sign is important.
                seconds: 0);
            DateTimeOffset userLocalDateOffset = dateUtcOffset.ToOffset(offset);
            DateTime localTime = userLocalDateOffset.DateTime;

            if (status.Creator.Id != userId)
            {
                return Unauthorized();
            }

            status.Text = uiStatusPost.Text;
            status.DateModifiedUtc = utc;
            status.DateModifiedLocal = localTime;
            db.Status.Attach(status);
            var entry = db.Entry(status);
            entry.Property(e => e.Text).IsModified = true;
            entry.Property(e => e.DateModifiedUtc).IsModified = true;
            entry.Property(e => e.DateModifiedLocal).IsModified = true;
            
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Status
        [ResponseType(typeof(UIStatus))]
        public async Task<IHttpActionResult> PostStatus(int groupId, UIStatusPost uiStatusPost)
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
            
            DateTime utc = DateTime.UtcNow;
            DateTimeOffset dateUtcOffset = new DateTimeOffset(utc, TimeSpan.Zero);
            TimeSpan offset = new TimeSpan(
                hours: 0,
                minutes: uiStatusPost.TimezoneOffsetMinutes,
                seconds: 0);
            DateTimeOffset userLocalDateOffset = dateUtcOffset.ToOffset(offset);
            DateTime localTime = userLocalDateOffset.DateTime;

            // Update user's location.
            LocationService.UpdateUserLocation(
                db,
                applicationUser,
                uiStatusPost.Position,
                uiStatusPost.TimezoneOffsetMinutes);

            Status status = new Status()
            {
                CreatorId = User.Identity.GetUserId(),
                GroupId = group.GroupId,
                Text = uiStatusPost.Text,
                DateCreatedUtc = utc,
                DateCreatedLocal = localTime,
                Creator = applicationUser,
                City = applicationUser.City,
                Country = applicationUser.Country,
                Latitude = applicationUser.Latitude,
                Longitude = applicationUser.Longitude,
                Group = group,
            };
            
            status.StatusViews = ControllerUtils.MakeStatusViews(userId, group, status, utc);

            db.Status.Add(status);
            db.Users.Attach(applicationUser);
            var entry = db.Entry(applicationUser);
            entry.Property(e => e.Latitude).IsModified = true;
            entry.Property(e => e.Longitude).IsModified = true;
            entry.Property(e => e.TimezoneOffsetMinutes).IsModified = true;
            entry.Property(e => e.City).IsModified = true;
            entry.Property(e => e.Country).IsModified = true;
            await db.SaveChangesAsync();

            UIStatus uiStatus = new UIStatus();
            uiStatus.Id = status.StatusId;
            uiStatus.DateCreatedUtc = status.DateCreatedUtc;
            uiStatus.DateCreatedLocal = status.DateCreatedLocal;
            uiStatus.CreatorName = status.Creator.Nickname;
            uiStatus.CreatorId = status.Creator.Id;
            uiStatus.IsCreatedByUser = true;
            uiStatus.IsRtl = ControllerUtils.IsRtl(uiStatusPost.Text);
            uiStatus.Views = ControllerUtils.GetUIStatusViews(status.StatusViews);
            uiStatus.Text = status.Text;
            uiStatus.City = status.City;
            uiStatus.Country = status.Country;

            return Ok(uiStatus);
        }

        private bool StatusExists(int id)
        {
            return db.Status.Count(e => e.StatusId == id) > 0;
        }
    }
}