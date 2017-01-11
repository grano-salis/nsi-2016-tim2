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

    [RoutePrefix("api/CV_User")]
    public class CV_USERController : ApiController
    {
        private MyEntities db = new MyEntities();

        // GET: api/CV_USER
        [HttpGet]
        [Route("GetUsers")]
        public IQueryable<CV_USER> GetCV_USER()
        {
            return db.CV_USER;
        }

        // GET: api/CV_USER/5
        [ResponseType(typeof(CV_USER))]
        public IHttpActionResult GetCV_USER(int id)
        {
            CV_USER cV_USER = db.CV_USER.Find(id);
            if (cV_USER == null)
            {
                return NotFound();
            }

            return Ok(cV_USER);
        }

        // PUT: api/CV_USER/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCV_USER(int id, CV_USER cV_USER)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cV_USER.ID)
            {
                return BadRequest();
            }

            db.Entry(cV_USER).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CV_USERExists(id))
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

        // POST: api/CV_USER
        [ResponseType(typeof(CV_USER))]
        public IHttpActionResult PostCV_USER(CV_USER cV_USER)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CV_USER.Add(cV_USER);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = cV_USER.ID }, cV_USER);
        }

        // DELETE: api/CV_USER/5
        [ResponseType(typeof(CV_USER))]
        public IHttpActionResult DeleteCV_USER(int id)
        {
            CV_USER cV_USER = db.CV_USER.Find(id);
            if (cV_USER == null)
            {
                return NotFound();
            }

            db.CV_USER.Remove(cV_USER);
            db.SaveChanges();

            return Ok(cV_USER);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CV_USERExists(int id)
        {
            return db.CV_USER.Count(e => e.ID == id) > 0;
        }
    }
}