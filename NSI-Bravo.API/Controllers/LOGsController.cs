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
    public class LOGsController : ApiController
    {
        private MyEntities db = new MyEntities();

        // GET: api/LOGs
        public IQueryable<LOG> GetLOG()
        {
            return db.LOG;
        }

        // GET: api/LOGs/5
        [ResponseType(typeof(LOG))]
        public IHttpActionResult GetLOG(long id)
        {
            LOG lOG = db.LOG.Find(id);
            if (lOG == null)
            {
                return NotFound();
            }

            return Ok(lOG);
        }

        // PUT: api/LOGs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLOG(long id, LOG lOG)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lOG.LOG_ID)
            {
                return BadRequest();
            }

            db.Entry(lOG).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LOGExists(id))
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

        // POST: api/LOGs
        [ResponseType(typeof(LOG))]
        public IHttpActionResult PostLOG(LOG lOG)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LOG.Add(lOG);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (LOGExists(lOG.LOG_ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = lOG.LOG_ID }, lOG);
        }

        // DELETE: api/LOGs/5
        [ResponseType(typeof(LOG))]
        public IHttpActionResult DeleteLOG(long id)
        {
            LOG lOG = db.LOG.Find(id);
            if (lOG == null)
            {
                return NotFound();
            }

            db.LOG.Remove(lOG);
            db.SaveChanges();

            return Ok(lOG);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LOGExists(long id)
        {
            return db.LOG.Count(e => e.LOG_ID == id) > 0;
        }
    }
}