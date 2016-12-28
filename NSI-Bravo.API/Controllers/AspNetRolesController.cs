using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AngularJSAuthentication.API.Models;

namespace AngularJSAuthentication.API.Controllers
{
    public class AspNetRolesController : ApiController
    {
        private MyEntities db = new MyEntities();

        // GET: api/AspNetRoles
        public IQueryable<AspNetRoles> GetAspNetRoles()
        {
            return db.AspNetRoles;
        }

        // GET: api/AspNetRoles/5
        [ResponseType(typeof(AspNetRoles))]
        public IHttpActionResult GetAspNetRoles(string id)
        {
            AspNetRoles aspNetRoles = db.AspNetRoles.Find(id);
            if (aspNetRoles == null)
            {
                return NotFound();
            }

            return Ok(aspNetRoles);
        }

        // PUT: api/AspNetRoles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAspNetRoles(string id, AspNetRoles aspNetRoles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aspNetRoles.Id)
            {
                return BadRequest();
            }

            db.Entry(aspNetRoles).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetRolesExists(id))
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

        // POST: api/AspNetRoles
        [ResponseType(typeof(AspNetRoles))]
        public IHttpActionResult PostAspNetRoles(AspNetRoles aspNetRoles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AspNetRoles.Add(aspNetRoles);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AspNetRolesExists(aspNetRoles.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aspNetRoles.Id }, aspNetRoles);
        }

        // DELETE: api/AspNetRoles/5
        [ResponseType(typeof(AspNetRoles))]
        public IHttpActionResult DeleteAspNetRoles(string id)
        {
            AspNetRoles aspNetRoles = db.AspNetRoles.Find(id);
            if (aspNetRoles == null)
            {
                return NotFound();
            }

            db.AspNetRoles.Remove(aspNetRoles);
            db.SaveChanges();

            return Ok(aspNetRoles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AspNetRolesExists(string id)
        {
            return db.AspNetRoles.Count(e => e.Id == id) > 0;
        }
    }
}