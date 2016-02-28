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

        private RelationshipService relationshipService = new RelationshipService();

        const int QueryPageSize = 25;

        // Right now we just return the text for the status edit page.
        [ResponseType(typeof(UIStatus))]
        public async Task<IHttpActionResult> GetStatus(int id)
        {
            Status status = await db.Status.FindAsync(id);

            UIStatus uiStatus = new UIStatus()
            {
                Text = status.Text
            };

            return Ok(uiStatus);
        }

        // GET: api/Status
        [ResponseType(typeof(UIStatus[]))]
        public async Task<IHttpActionResult> GetStatuses()
        {
            string userId = User.Identity.GetUserId();

            Relationship relationship = await relationshipService.GetUserRelationshipIdAsync(userId);

            if (relationship == null)
            {
                return Ok();
            }

            string myName = User.Identity.GetUserName();

            var statuses = await db.Status
                .Where(s => s.RelationshipId == relationship.RelationshipId)
                .Select(s => s)
                .OrderByDescending(s => s.DateCreatedUtc)
                .Take(QueryPageSize)
                .ToListAsync();

            UIStatus[] uiStatuses = statuses.Select(
                s => new UIStatus()
                {
                    Id = s.StatusId,
                    CreatorName = s.Creator.UserName.Equals(myName) ? "Me" : s.Creator.UserName,
                    DateCreatedUtc = s.DateCreatedUtc,
                    DateCreatedCreator = s.DateCreatedCreator,
                    DateCreatedLover = s.DateCreatedLover,
                    Text = s.Text,
                    IsRtl = ControllerUtils.IsRtl(s.Text),
                    CreatorCity = s.CreatorCity.Name,
                    LoverCity = s.LoverCity.Name,
                    IsCreatedByUser = s.Creator.UserName.Equals(myName)
                }).ToArray();

            return Ok(uiStatuses);
        }

        // GET: api/Status
        [ResponseType(typeof(UIStatus[]))]
        public async Task<IHttpActionResult> GetStatuses(string date)
        {
            DateTime target = DateTime.Parse(date);
            DateTime targetPlusDay = target.AddDays(1);

            string userId = User.Identity.GetUserId();

            Relationship relationship = await relationshipService.GetUserRelationshipIdAsync(userId);

            if (relationship == null)
            {
                return Ok();
            }

            string myName = User.Identity.GetUserName();

            var statuses = await db.Status
                .Where(s => s.RelationshipId == relationship.RelationshipId && s.DateCreatedCreator > target && s.DateCreatedCreator < targetPlusDay)
                .Select(s => s)
                .OrderByDescending(s => s.DateCreatedCreator)
                .Take(QueryPageSize)
                .ToListAsync();

            UIStatus[] uiStatuses = statuses.Select(
                s => new UIStatus()
                {
                    Id = s.StatusId,
                    CreatorName = s.Creator.UserName.Equals(myName) ? "Me" : s.Creator.UserName,
                    DateCreatedUtc = s.DateCreatedUtc,
                    DateCreatedCreator = s.DateCreatedCreator,
                    DateCreatedLover = s.DateCreatedLover,
                    Text = s.Text,
                    IsRtl = ControllerUtils.IsRtl(s.Text),
                    CreatorCity = s.CreatorCity.Name,
                    LoverCity = s.LoverCity.Name,
                    IsCreatedByUser = s.Creator.UserName.Equals(myName)
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

            DateTime utc;
            DateTime dateFirst;
            DateTime dateSecond;
            ControllerUtils.GetNowDateTimes(status.Relationship, out utc, out dateFirst, out dateSecond);

            if (status.Creator.Id != userId)
            {
                return Unauthorized();
            }

            status.Text = uiStatus.Text;
            status.DateModifiedUtc = utc;
            status.DateModifiedCreator = (status.Relationship.FirstId == userId) ? dateFirst : dateSecond;
            status.DateModifiedLover = (status.Relationship.FirstId == userId) ? dateSecond : dateFirst;
            db.Status.Attach(status);
            var entry = db.Entry(status);
            entry.Property(e => e.Text).IsModified = true;
            entry.Property(e => e.DateModifiedUtc).IsModified = true;
            entry.Property(e => e.DateModifiedCreator).IsModified = true;
            entry.Property(e => e.DateModifiedLover).IsModified = true;
            await db.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Status
        [ResponseType(typeof(UIStatus))]
        public async Task<IHttpActionResult> PostStatus(UIStatus uiStatus)
        {
            string userId = User.Identity.GetUserId();

            Relationship relationship = await relationshipService.GetUserRelationshipIdAsync(userId);

            if (relationship == null)
            {
                return BadRequest("You're not in a relationship.");
            }

            ApplicationUser applicationUserMe = (relationship.FirstId == userId) ? relationship.First : relationship.Second;
            ApplicationUser applicationUserLover = (relationship.FirstId == userId) ? relationship.Second : relationship.First;
            
            DateTime utc;
            DateTime dateFirst;
            DateTime dateSecond;
            ControllerUtils.GetNowDateTimes(relationship, out utc, out dateFirst, out dateSecond);

            DateTimeOffset createDateMeOffset = (relationship.FirstId == userId) ? dateFirst : dateSecond;
            DateTimeOffset createDateLoverOffset = (relationship.FirstId == userId) ? dateSecond : dateFirst;

            Status status = new Status()
            {
                CreatorId = User.Identity.GetUserId(),
                RelationshipId = relationship.RelationshipId,
                Text = uiStatus.Text,
                DateCreatedUtc = utc,
                DateCreatedCreator = createDateMeOffset.DateTime,
                DateCreatedLover = createDateLoverOffset.DateTime,
                CreatorCityId = applicationUserMe.CurrentCity.CityId,
                LoverCityId = applicationUserLover.CurrentCity.CityId
            };

            db.Status.Add(status);
            await db.SaveChangesAsync();

            uiStatus.Id = status.StatusId;
            uiStatus.DateCreatedUtc = utc;
            uiStatus.DateCreatedCreator = createDateMeOffset.DateTime;
            uiStatus.DateCreatedLover = createDateLoverOffset.DateTime;
            uiStatus.CreatorCity = applicationUserMe.CurrentCity.Name;
            uiStatus.LoverCity = applicationUserLover.CurrentCity.Name;
            uiStatus.CreatorName = "Me";
            uiStatus.IsCreatedByUser = true;
            uiStatus.IsRtl = ControllerUtils.IsRtl(uiStatus.Text);

            return Ok(uiStatus);
        }
    }
}