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
                City = appUser.City
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
                City = appUser.City,
                TimezoneOffsetMinutes = appUser.TimezoneOffsetMinutes
            };

            return Ok(user);
        }
    }
}