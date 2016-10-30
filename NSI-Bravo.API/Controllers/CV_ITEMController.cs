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
    public class CV_ITEMController : ApiController
    {
        private MyEntities db = new MyEntities();

        // GET: api/CV_ITEM
        public IQueryable<CV_ITEM> GetCV_ITEM()
        {
            return db.CV_ITEM;
        }

        // GET: api/CV_ITEM/5
        [ResponseType(typeof(CV_ITEM))]
        public IHttpActionResult GetCV_ITEM(long id)
        {
            CV_ITEM cV_ITEM = db.CV_ITEM.Find(id);
            if (cV_ITEM == null)
            {
                return NotFound();
            }

            return Ok(cV_ITEM);
        }

        // PUT: api/CV_ITEM/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCV_ITEM(long id, CV_ITEM cV_ITEM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cV_ITEM.ID_ITEM)
            {
                return BadRequest();
            }

            db.Entry(cV_ITEM).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CV_ITEMExists(id))
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

        // POST: api/CV_ITEM
        [ResponseType(typeof(CV_ITEM))]
        public IHttpActionResult PostCV_ITEM(CV_ITEM cV_ITEM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CV_ITEM.Add(cV_ITEM);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CV_ITEMExists(cV_ITEM.ID_ITEM))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cV_ITEM.ID_ITEM }, cV_ITEM);
        }

        // DELETE: api/CV_ITEM/5
        [ResponseType(typeof(CV_ITEM))]
        public IHttpActionResult DeleteCV_ITEM(long id)
        {
            CV_ITEM cV_ITEM = db.CV_ITEM.Find(id);
            if (cV_ITEM == null)
            {
                return NotFound();
            }

            db.CV_ITEM.Remove(cV_ITEM);
            db.SaveChanges();

            return Ok(cV_ITEM);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CV_ITEMExists(long id)
        {
            return db.CV_ITEM.Count(e => e.ID_ITEM == id) > 0;
        }
    }
}