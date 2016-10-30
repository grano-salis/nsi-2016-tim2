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
    public class CV_TABLEController : ApiController
    {
        private MyEntities db = new MyEntities();

        // GET: api/CV_TABLE
        public IQueryable<CV_TABLE> GetCV_TABLE()
        {
            return db.CV_TABLE;
        }

        // GET: api/CV_TABLE/5
        [ResponseType(typeof(CV_TABLE))]
        public IHttpActionResult GetCV_TABLE(long id)
        {
            CV_TABLE cV_TABLE = db.CV_TABLE.Find(id);
            if (cV_TABLE == null)
            {
                return NotFound();
            }

            return Ok(cV_TABLE);
        }

        // PUT: api/CV_TABLE/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCV_TABLE(long id, CV_TABLE cV_TABLE)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cV_TABLE.ID_CV)
            {
                return BadRequest();
            }

            db.Entry(cV_TABLE).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CV_TABLEExists(id))
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

        // POST: api/CV_TABLE
        [ResponseType(typeof(CV_TABLE))]
        public IHttpActionResult PostCV_TABLE(CV_TABLE cV_TABLE)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CV_TABLE.Add(cV_TABLE);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CV_TABLEExists(cV_TABLE.ID_CV))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cV_TABLE.ID_CV }, cV_TABLE);
        }

        // DELETE: api/CV_TABLE/5
        [ResponseType(typeof(CV_TABLE))]
        public IHttpActionResult DeleteCV_TABLE(long id)
        {
            CV_TABLE cV_TABLE = db.CV_TABLE.Find(id);
            if (cV_TABLE == null)
            {
                return NotFound();
            }

            db.CV_TABLE.Remove(cV_TABLE);
            db.SaveChanges();

            return Ok(cV_TABLE);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CV_TABLEExists(long id)
        {
            return db.CV_TABLE.Count(e => e.ID_CV == id) > 0;
        }
    }
}