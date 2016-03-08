using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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

        // Just return the id, name and names of the members in the group info.
        [ResponseType(typeof(List<UIGroup>))]
        public IHttpActionResult GetGroups()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser me = userService.GetUser(userId);
            var groups = me.Groups.Select(
                x => new UIGroup()
                {
                    Id = x.GroupId,
                    Name = x.Name,
                    Members = x.Members.Select(
                        m => new UIUser()
                        {
                            Name = m.Nickname
                        }).ToList()
                }).ToList();
            return Ok(groups);
        }

        [ResponseType(typeof(UIGroup))]
        public async Task<IHttpActionResult> GetGroup(int groupId)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser me = userService.GetUser(userId);
            Group group = await db.Groups.FindAsync(groupId);
            List<ApplicationUser> members = group.Members;

            var uiGroup = new UIGroup()
            {
                Name = group.Name,
                Id = group.GroupId,
                Members = members.Select(
                    x => new UIUser()
                    {
                        Name = x.Nickname,
                        CityName = x.CurrentCity.Name,
                        Email = x.Email,
                        TimeZoneName = x.CurrentCity.UserTimeZone.Name
                    }).ToList()
            };

            return Ok(uiGroup);
        }

        [ResponseType(typeof(UIGroup))]
        public async Task<IHttpActionResult> PostGroup(UIGroup uiGroup)
        {
            string myId = User.Identity.GetUserId();

            Group group = new Group();
            group.Name = uiGroup.Name;
            group.Members = new List<ApplicationUser>()
            {
                db.Users.Find(myId)
            };
            
            foreach (UIUser user in uiGroup.Members)
            {
                ApplicationUser appUser = db.Users.Find(user.Id);
                group.Members.Add(appUser);
            }

            db.Groups.Add(group);
            await db.SaveChangesAsync();

            uiGroup.Id = group.GroupId;

            return Ok(uiGroup);
        }
    }
}