using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Wewy.Models;

namespace Wewy.Services
{
    public class RelationshipService
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public async Task<Relationship> GetUserRelationshipIdAsync(string userId)
        {
            var relationship = await db.Relationships
                .Where(r => r.FirstId == userId || r.SecondId == userId)
                .Select(q => q)
                .FirstOrDefaultAsync();
            
            return relationship;
        }

        public async Task<ApplicationUser> GetLover(string userId)
        {
            var relationship = await GetUserRelationshipIdAsync(userId);
            return (relationship.FirstId == userId) ? relationship.Second : relationship.First;
        }
    }
}