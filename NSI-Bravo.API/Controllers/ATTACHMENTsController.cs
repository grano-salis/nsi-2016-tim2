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
    [RoutePrefix("api/Attachment")]
    public class ATTACHMENTsController : ApiController
    {
        private MyEntities db = new MyEntities();

        // GET: api/ATTACHMENTs
        [HttpGet]
        [Route("GetAllAttachment")]
        public IQueryable<ATTACHMENT> GetATTACHMENT()
        {
            return db.ATTACHMENT;
        }

        // GET: api/ATTACHMENTs/5
        [HttpGet]
        [Route("GetAttachment/{id}")]
        [ResponseType(typeof(ATTACHMENT))]
        public IHttpActionResult GetATTACHMENT(long id)
        {
            ATTACHMENT aTTACHMENT = db.ATTACHMENT.Find(id);
            if (aTTACHMENT == null)
            {
                return NotFound();
            }

            return Ok(aTTACHMENT);
        }

        // PUT: api/ATTACHMENTs/5
        [HttpPut]
        [Route("UpdateAttachment/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutATTACHMENT(long id, ATTACHMENT aTTACHMENT)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aTTACHMENT.ID)
            {
                return BadRequest();
            }

            db.Entry(aTTACHMENT).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ATTACHMENTExists(id))
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

        // POST: api/ATTACHMENTs
        [HttpPost]
        [Route("PostAttachment")]
        [ResponseType(typeof(ATTACHMENT))]
        public IHttpActionResult PostATTACHMENT(ATTACHMENT aTTACHMENT)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ATTACHMENT.Add(aTTACHMENT);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ATTACHMENTExists(aTTACHMENT.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(aTTACHMENT);
            //return CreatedAtRoute("DefaultApi", new { id = aTTACHMENT.ID }, aTTACHMENT);
        }

        // DELETE: api/ATTACHMENTs/5
        [HttpDelete]
        [Route("DeleteAttachment/{id}")]
        [ResponseType(typeof(ATTACHMENT))]
        public IHttpActionResult DeleteATTACHMENT(long id)
        {
            ATTACHMENT aTTACHMENT = db.ATTACHMENT.Find(id);
            if (aTTACHMENT == null)
            {
                return NotFound();
            }

            db.ATTACHMENT.Remove(aTTACHMENT);
            db.SaveChanges();

            return Ok(aTTACHMENT);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ATTACHMENTExists(long id)
        {
            return db.ATTACHMENT.Count(e => e.ID == id) > 0;
        }
    }
}