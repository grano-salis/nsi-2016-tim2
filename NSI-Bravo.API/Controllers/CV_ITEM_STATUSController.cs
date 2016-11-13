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
    public class CV_ITEM_STATUSController : ApiController
    {
        private MyEntities db = new MyEntities();

        // GET: api/CV_ITEM_STATUS
        public IQueryable<CV_ITEM_STATUS> GetCV_ITEM_STATUS()
        {
            return db.CV_ITEM_STATUS;
        }

        // GET: api/CV_ITEM_STATUS/5
        [ResponseType(typeof(CV_ITEM_STATUS))]
        public IHttpActionResult GetCV_ITEM_STATUS(int id)
        {
            CV_ITEM_STATUS cV_ITEM_STATUS = db.CV_ITEM_STATUS.Find(id);
            if (cV_ITEM_STATUS == null)
            {
                return NotFound();
            }

            return Ok(cV_ITEM_STATUS);
        }

        // PUT: api/CV_ITEM_STATUS/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCV_ITEM_STATUS(int id, CV_ITEM_STATUS cV_ITEM_STATUS)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cV_ITEM_STATUS.ID)
            {
                return BadRequest();
            }

            db.Entry(cV_ITEM_STATUS).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CV_ITEM_STATUSExists(id))
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

        // POST: api/CV_ITEM_STATUS
        [ResponseType(typeof(CV_ITEM_STATUS))]
        public IHttpActionResult PostCV_ITEM_STATUS(CV_ITEM_STATUS cV_ITEM_STATUS)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CV_ITEM_STATUS.Add(cV_ITEM_STATUS);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CV_ITEM_STATUSExists(cV_ITEM_STATUS.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cV_ITEM_STATUS.ID }, cV_ITEM_STATUS);
        }

        // DELETE: api/CV_ITEM_STATUS/5
        [ResponseType(typeof(CV_ITEM_STATUS))]
        public IHttpActionResult DeleteCV_ITEM_STATUS(int id)
        {
            CV_ITEM_STATUS cV_ITEM_STATUS = db.CV_ITEM_STATUS.Find(id);
            if (cV_ITEM_STATUS == null)
            {
                return NotFound();
            }

            db.CV_ITEM_STATUS.Remove(cV_ITEM_STATUS);
            db.SaveChanges();

            return Ok(cV_ITEM_STATUS);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CV_ITEM_STATUSExists(int id)
        {
            return db.CV_ITEM_STATUS.Count(e => e.ID == id) > 0;
        }
    }
}