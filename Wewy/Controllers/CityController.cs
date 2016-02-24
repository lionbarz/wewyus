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

        [ResponseType(typeof(List<string>))]
        public async Task<IHttpActionResult> GetCities()
        {
            var cities = await db.Cities.ToArrayAsync();
            return Ok(cities.Select(c => c.Name).ToArray());
        }
    }
}