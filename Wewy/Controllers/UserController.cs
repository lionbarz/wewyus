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
    public class UserController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserService userService = new UserService();

        /// <summary>
        /// Used to query the current user's info.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(UIUser))]
        public IHttpActionResult GetUser()
        {
            string myId = User.Identity.GetUserId();
            ApplicationUser appUser = db.Users.Find(myId);
            
            var user = new UIUser()
            {
                Id = appUser.Id,
                Name = appUser.Nickname,
                Email = appUser.Email,
                CityName = appUser.CurrentCity.Name,
                TimeZoneName = appUser.CurrentCity.UserTimeZone.JavascriptName
            };

            return Ok(user);
        }

        /// <summary>
        /// Used to query users for adding to groups.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [ResponseType(typeof(UIUser))]
        public async Task<IHttpActionResult> GetUser(string email)
        {
            ApplicationUser appUser = await db.Users
                .Where(x => x.Email.Equals(email))
                .Select(x => x)
                .FirstOrDefaultAsync();
            
            if (appUser == null)
            {
                return Ok();
            }

            var user = new UIUser()
            {
                Id = appUser.Id,
                Name = appUser.Nickname,
                Email = appUser.Email,
                CityName = appUser.CurrentCity.Name,
                TimeZoneName = appUser.CurrentCity.UserTimeZone.JavascriptName
            };

            return Ok(user);
        }

        // Currently just for changing the city.
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