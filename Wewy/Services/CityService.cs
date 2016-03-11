using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wewy.Models;

namespace Wewy.Services
{
    public class CityService
    {
        public static List<City> GetCities()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Cities.ToList();
        }
    }
}