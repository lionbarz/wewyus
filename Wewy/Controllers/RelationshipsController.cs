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
        public IQueryable<Relationship> GetRelationships()
        {
            return db.Relationships;
        }

        // GET: api/Relationships/5
        [ResponseType(typeof(Relationship))]
        public IHttpActionResult GetRelationship(int id)
        {
            Relationship relationship = db.Relationships.Find(id);
            if (relationship == null)
            {
                return NotFound();
            }

            return Ok(relationship);
        }
        
        // PUT: api/Relationships/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRelationship(int id, Relationship relationship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != relationship.RelationshipId)
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
        [ResponseType(typeof(Relationship))]
        public IHttpActionResult PostRelationship(Relationship relationship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Relationships.Add(relationship);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = relationship.RelationshipId }, relationship);
        }

        // DELETE: api/Relationships/5
        [ResponseType(typeof(Relationship))]
        public IHttpActionResult DeleteRelationship(int id)
        {
            Relationship relationship = db.Relationships.Find(id);
            if (relationship == null)
            {
                return NotFound();
            }

            db.Relationships.Remove(relationship);
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
            return db.Relationships.Count(e => e.RelationshipId == id) > 0;
        }
    }
}