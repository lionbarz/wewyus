using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;
using Wewy.Models;

namespace Wewy.Services
{
    public class UserService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string GetUserNickname(string userId)
        {
            var user = GetUser(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            return user.Nickname;
        }

        public ApplicationUser GetUser(string userId)
        {
            return db.Users.Find(userId);
        }
    }
}