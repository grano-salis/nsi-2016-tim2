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
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace AngularJSAuthentication.API.Controllers
{

    [RoutePrefix("api/CVtable")]
    public class CV_TABLEController : ApiController
    {
        private MyEntities db = new MyEntities();

        //Route: http://localhost:26264/api/CVtable/GetAll
        [HttpGet]
        [Route("GetAll")]
        public IQueryable<CV_TABLE> GetCV_TABLE()
        {
            return db.CV_TABLE;
        }

        //Route: http://localhost:26264/api/CVtable/Get/5
        [HttpGet]
        [Route("Get/{id}")]
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

        //Route: http://localhost:26264/api/CVtable/Update/5
        [HttpPut]
        [Route("Update/{id}")]
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

        //Route: http://localhost:26264/api/CVtable/Create
        [HttpPost]
        [Route("Create")]
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

            return Ok(cV_TABLE);
            //return CreatedAtRoute("DefaultApi", new { id = cV_TABLE.ID_CV }, cV_TABLE);
        }

        //Route: http://localhost:26264/api/CVtable/Delete/5
        [HttpDelete]
        [Route("Delete/{id}")]
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
        //Route: http://localhost:26264/api/CVtable/Delete/5
        //Returns sum of CV_ITEM points(CV_ITEM.CRITERIA_ID_CRITERIA.POINTS)
        [HttpGet]
        [Route("Score/{id}")]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetScore(long id)
        {
            int score;
            try {
                //first find all CV_ITEMs with CV_TABLE_ID_CV==id, than sum points of criteria in all CV_ITEMs
                 score = (int) db.CV_ITEM.Where(o => o.CV_TABLE_ID_CV == id).Sum(o => o.CRITERIA.POINTS);
            }
            catch (DBConcurrencyException)
            {
                return BadRequest("Error");
            }

            return Ok(score);
        }

        [HttpPost]
        [Route("GetByDateRange/{id}")]
        [ResponseType(typeof(List<CV_ITEM>))]
        public IHttpActionResult GetByDateRange([FromUri()]int id, [FromBody()] JObject dateRange)
        {
           
            List<CV_ITEM> items=new List<CV_ITEM>();
            DateTime from = (DateTime) dateRange.GetValue("from");
            DateTime to = (DateTime) dateRange.GetValue("to");
          
       
            foreach (CV_ITEM c in db.CV_ITEM)
                if (c.CV_TABLE_ID_CV==id && (c.DATE_CREATED.Value.Year >= from.Year && c.DATE_CREATED.Value.Year <= to.Year)&&
                                            (c.DATE_CREATED.Value.Month >= from.Month && c.DATE_CREATED.Value.Month <= to.Month)&&
                                            (c.DATE_CREATED.Value.Day >=from.Day && c.DATE_CREATED.Value.Day<=to.Day))
                    items.Add(c);

            List<CV_ITEM> cc = items.Except(items).ToList();
            return Ok(items);
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