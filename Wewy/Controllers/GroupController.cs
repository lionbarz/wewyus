using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Wewy.Models;
using Wewy.Services;

namespace Wewy.Controllers
{
    [Authorize]
    public class GroupController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserService userService = new UserService();

        // Return the id, name and names and ids of the members in the group info
        // and the number of unread messages.
        [ResponseType(typeof(List<UIGroup>))]
        public IHttpActionResult GetGroups()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser me = userService.GetUser(userId);
            List<UIGroup> uiGroups = new List<UIGroup>();

            foreach (Group group in me.Groups)
            {
                UIGroup uiGroup = new UIGroup()
                {
                    Id = group.GroupId,
                    Name = group.Name,
                    IsUserAdmin = group.AdminId != null && group.AdminId.Equals(userId),
                    Members = group.Members.Select(
                        m => new UIUser()
                        {
                            Id = m.Id,
                            Name = m.Nickname
                        }).ToList()
                };

                uiGroup.NumberOfNewPosts = FindNumberOfUnreadMessages(me, group);
                uiGroups.Add(uiGroup);
            }

            return Ok(uiGroups);
        }

        [ResponseType(typeof(UIGroup))]
        public async Task<IHttpActionResult> GetGroup(int groupId)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser me = userService.GetUser(userId);
            Group group = await db.Groups.FindAsync(groupId);

            if (group == null)
            {
                return BadRequest("Group does not exist.");
            }

            List<ApplicationUser> members = group.Members;

            if (!members.Any(m => m.Id.Equals(userId)))
            {
                return BadRequest("You are not in this group.");
            }

            var uiGroup = new UIGroup()
            {
                Name = group.Name,
                Id = group.GroupId,
                Admin = group.Admin == null ? null : new UIUser(group.Admin),
                IsUserAdmin = group.AdminId != null && group.AdminId.Equals(userId),
                Members = members.Select(
                    x => new UIUser()
                    {
                        Id = x.Id,
                        Name = x.Nickname,
                        City = x.City,
                        Email = x.Email,
                        TimezoneOffsetMinutes = x.TimezoneOffsetMinutes
                    }).ToList()
            };

            uiGroup.NumberOfNewPosts = FindNumberOfUnreadMessages(me, group);

            return Ok(uiGroup);
        }

        /// <summary>
        /// Just let you rename a group right now.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="uiGroup"></param>
        /// <returns></returns>
        [ResponseType(typeof(UIGroup))]
        public async Task<IHttpActionResult> PutGroup(int groupId, UIGroup uiGroup)
        {
            if (string.IsNullOrEmpty(uiGroup.Name))
            {
                return BadRequest("Group name cannot be empty.");
            }

            string myId = User.Identity.GetUserId();

            Group group = db.Groups.Find(groupId);

            if (group == null)
            {
                return BadRequest(string.Format("Group {0} doesn't exist.", groupId));
            }

            if (!group.Members.Any(m => m.Id.Equals(myId)))
            {
                return BadRequest("You are not a member of this group.");
            }
         
            if (group.AdminId.Equals(myId))
            {
                group.Name = uiGroup.Name;             
                ApplicationUser me = db.Users.Find(myId);
                group.Members = MakeGroupMembersList(me, uiGroup.Members);
                db.Groups.Attach(group);
                var entry = db.Entry(group);
                entry.State = EntityState.Modified;
            }
            else
            {
                // Non-admins can only change the group name.
                group.Name = uiGroup.Name;
                db.Groups.Attach(group);
                var entry = db.Entry(group);
                entry.Property(e => e.Name).IsModified = true;
            }
            
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(groupId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(uiGroup);
        }

        [ResponseType(typeof(UIGroup))]
        public async Task<IHttpActionResult> PostGroup(UIGroup uiGroup)
        {
            if (string.IsNullOrEmpty(uiGroup.Name))
            {
                return BadRequest("Group name cannot be empty.");
            }

            string myId = User.Identity.GetUserId();
            ApplicationUser me = db.Users.Find(myId);

            Group group = new Group();
            group.Name = uiGroup.Name;

            try
            {
                group.Members = MakeGroupMembersList(me, uiGroup.Members);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            if (group.Members.Count < 2)
            {
                return BadRequest("Not enough people to form a group.");
            }

            group.AdminId = myId;
            group.Admin = me;

            db.Groups.Add(group);
            await db.SaveChangesAsync();

            uiGroup.Id = group.GroupId;

            return Ok(uiGroup);
        }

        private List<ApplicationUser> MakeGroupMembersList(ApplicationUser me, List<UIUser> uiMembers)
        {
            
            List<ApplicationUser> members = new List<ApplicationUser>() { me };

            foreach (UIUser user in uiMembers)
            {
                ApplicationUser appUser = db.Users.Find(user.Id);

                if (appUser == null)
                {
                    throw new ArgumentException("The user doesn't exist: " + user.Id);
                }

                if (!members.Contains(appUser, new ControllerUtils.UserComparer()))
                {
                    members.Add(appUser);
                }
            }

            return members;
        }

        /// <summary>
        /// Just leaves the group, ie removes user from members list.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteGroup(int id)
        {
            string myId = User.Identity.GetUserId();

            Group group = db.Groups.Find(id);

            if (group == null)
            {
                return BadRequest(string.Format("Group {0} doesn't exist.", id));
            }

            if (!group.Members.Any(m => m.Id.Equals(myId)))
            {
                return BadRequest("You are not a member of this group.");
            }

            List<ApplicationUser> newMembers = group.Members.Where(x => !x.Id.Equals(myId)).ToList();
            group.Members = newMembers;

            // If user is group admin, assign a random admin (like WhatsApp).
            if (group.AdminId != null && group.AdminId.Equals(myId) && group.Members.Count > 0)
            {
                ApplicationUser newAdmin = group.Members[0];
                group.AdminId = newAdmin.Id;
                group.Admin = newAdmin;
            }

            db.Groups.Attach(group);
            var entry = db.Entry(group);
            entry.State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        private bool GroupExists(int id)
        {
            return db.Groups.Count(e => e.GroupId == id) > 0;
        }

        private int FindNumberOfUnreadMessages(ApplicationUser me, Group group)
        {
            // Find number of unread messages.
            LastGroupVisit lastVisit = me.LastGroupVisits
                .Where(x => x.GroupId == group.GroupId)
                .FirstOrDefault();
            if (lastVisit == null)
            {
                return group.Statuses.Count();
            }
            else
            {
                return group.Statuses
                    .Where(x => x.DateCreatedUtc > lastVisit.VisitTimeUtc && !x.CreatorId.Equals(me.Id))
                    .Count();
            }
        }
    }
}