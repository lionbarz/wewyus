using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
    public class UserController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserService userService = new UserService();
        private RelationshipService relationshipService = new RelationshipService();

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> GetUser()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser me = userService.GetUser(userId);
            ApplicationUser lover = await relationshipService.GetLover(userId);

            var userInfo = new UIUserInfo()
            {
                Me = new UIUser()
                {
                    Name = me.Nickname,
                    Email = me.Email,
                    CityName = me.CurrentCity.Name
                },
                Lover = new UIUser()
                {
                    Name = lover.Nickname,
                    Email = lover.Email,
                    CityName = lover.CurrentCity.Name
                }
            };

            return Ok(userInfo);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(UIUser modifiedUser)
        {
            City city = null;

            if (modifiedUser.CityName != null)
            {
                city = await db.Cities
                    .Where(c => c.Name == modifiedUser.CityName)
                    .Select(c => c)
                    .FirstOrDefaultAsync();
            }
            
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            ApplicationUser updatedUser = new ApplicationUser()
            {
                Id = userId,
                CurrentCity = city,
                CurrentCityId = city.CityId,
                UserName = User.Identity.GetUserName()
            };
            
            db.Users.Attach(updatedUser);
            var entry = db.Entry(updatedUser);
            if (city != null)
            {
                entry.Property(e => e.CurrentCityId).IsModified = true;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                return InternalServerError(e);
            }

            return Ok();
        }
    }
}