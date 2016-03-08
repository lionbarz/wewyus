using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Wewy.Models;

namespace Wewy.Controllers
{
    [Authorize]
    public class RelationshipsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Relationships
        public IQueryable<Group> GetRelationships()
        {
            return db.Groups;
        }

        // GET: api/Relationships/5
        [ResponseType(typeof(Group))]
        public IHttpActionResult GetRelationship(int id)
        {
            Group relationship = db.Groups.Find(id);
            if (relationship == null)
            {
                return NotFound();
            }

            return Ok(relationship);
        }
        
        // PUT: api/Relationships/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRelationship(int id, Group relationship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != relationship.GroupId)
            {
                return BadRequest();
            }

            db.Entry(relationship).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelationshipExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Relationships
        [ResponseType(typeof(Group))]
        public IHttpActionResult PostRelationship(Group relationship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(relationship);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = relationship.GroupId }, relationship);
        }

        // DELETE: api/Relationships/5
        [ResponseType(typeof(Group))]
        public IHttpActionResult DeleteRelationship(int id)
        {
            Group relationship = db.Groups.Find(id);
            if (relationship == null)
            {
                return NotFound();
            }

            db.Groups.Remove(relationship);
            db.SaveChanges();

            return Ok(relationship);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RelationshipExists(int id)
        {
            return db.Groups.Count(e => e.GroupId == id) > 0;
        }
    }
}