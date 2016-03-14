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

namespace Wewy.Controllers
{
    [Authorize]
    public class CityController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Return all cities and include the user's city.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(List<UICities>))]
        public async Task<IHttpActionResult> GetCities()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var cities = await db.Cities.ToArrayAsync();
            return Ok(new UICities()
            {
                UserCityName = user.CurrentCity.Name,
                CityNames = cities.Select(c => c.Name).ToList()
            });
        }
    }
}