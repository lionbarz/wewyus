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
                    CreatorName = s.Creator.UserName.Equals(myName) ? "Me" : s.Creator.UserName,
                    DateCreatedUtc = s.DateCreatedUtc,
                    DateCreatedCreator = s.DateCreatedCreator,
                    DateCreatedLover = s.DateCreatedLover,
                    Text = s.Text,
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
                    CreatorName = s.Creator.UserName.Equals(myName) ? "Me" : s.Creator.UserName,
                    DateCreatedUtc = s.DateCreatedUtc,
                    DateCreatedCreator = s.DateCreatedCreator,
                    DateCreatedLover = s.DateCreatedLover,
                    Text = s.Text,
                    CreatorCity = s.CreatorCity.Name,
                    LoverCity = s.LoverCity.Name,
                    IsCreatedByUser = s.Creator.UserName.Equals(myName)
                }).ToArray();

            return Ok(uiStatuses);
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

            // Offset in hours.
            TimeSpan offsetMe = new TimeSpan(applicationUserMe.CurrentCity.UserTimeZone.Offset, 0, 0);
            TimeSpan offsetLover = new TimeSpan(applicationUserLover.CurrentCity.UserTimeZone.Offset, 0, 0);

            DateTime createDateUtc = DateTime.UtcNow;
            DateTimeOffset createDateUtcOffset = new DateTimeOffset(createDateUtc);
            DateTimeOffset createDateMeOffset = createDateUtcOffset.ToOffset(offsetMe);
            DateTimeOffset createDateLoverOffset = createDateUtcOffset.ToOffset(offsetLover);

            Status status = new Status()
            {
                CreatorId = User.Identity.GetUserId(),
                RelationshipId = relationship.RelationshipId,
                Text = uiStatus.Text,
                DateCreatedUtc = createDateUtc,
                DateCreatedCreator = createDateMeOffset.DateTime,
                DateCreatedLover = createDateLoverOffset.DateTime,
                CreatorCityId = applicationUserMe.CurrentCity.CityId,
                LoverCityId = applicationUserLover.CurrentCity.CityId
            };

            db.Status.Add(status);
            await db.SaveChangesAsync();

            uiStatus.DateCreatedUtc = createDateUtc;
            uiStatus.DateCreatedCreator = createDateMeOffset.DateTime;
            uiStatus.DateCreatedLover = createDateLoverOffset.DateTime;
            uiStatus.CreatorCity = applicationUserMe.CurrentCity.Name;
            uiStatus.LoverCity = applicationUserLover.CurrentCity.Name;
            uiStatus.CreatorName = "Me";
            uiStatus.IsCreatedByUser = true;

            return Ok(uiStatus);
        }
    }
}