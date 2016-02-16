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
    public class LoverController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private RelationshipService relationshipService = new RelationshipService();

        // GET: api/Lover
        [ResponseType(typeof(UIUser))]
        public async Task<IHttpActionResult> GetLoverAsync()
        {
            string userId = User.Identity.GetUserId();

            Relationship relationship = await relationshipService.GetUserRelationshipIdAsync(userId);

            if (relationship == null)
            {
                return Ok();
            }

            ApplicationUser lover = (relationship.FirstId == userId) ? relationship.Second : relationship.First;

            UIUser user = new UIUser()
            {
                Name = lover.UserName.Split(' ').First(),
                Email = lover.Email
            };

            return Ok(user);
        }
    }
}