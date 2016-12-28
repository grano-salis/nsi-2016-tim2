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
using Newtonsoft.Json;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/Log")]
    public class LOGsController : ApiController
    {
        private MyEntities db = new MyEntities();

        [JsonObject(IsReference = false)]
        public class LogModel
        {
            public long LOG_ID { get; set; }
            public Nullable<System.DateTime> EVENT_CREATED { get; set; }
            public string EVENT_TYPE { get; set; }
            public CV_ITEM DESCRIPTION { get; set; }
            public string CV_ITEM_ID { get; set; }
            public CV_TABLE USER { get; set; }

            public LogModel(long id, Nullable<System.DateTime> created, CV_ITEM cvitem, CV_TABLE user,string type,string cv_id)
            {
                LOG_ID = id;
                EVENT_CREATED = created;
                DESCRIPTION = cvitem;
                EVENT_TYPE = type;
                USER = user;
                CV_ITEM_ID = cv_id;
            }
        };

        // GET: api/LOGs
        [HttpGet]
        [Route("GetAllLogs")]
        public IHttpActionResult GetLOG()
        {
            List<LOG> lOGs = db.LOG.ToList();
            if (lOGs == null)
            {
                return NotFound();
            }
            CV_TABLE user;
            CV_ITEM cvitem;
            List<LogModel> returnData = new List<LogModel>();
            foreach (LOG log in lOGs) {
                user = db.CV_TABLE.Where(x => x.USER_ID ==log.USER_ID).FirstOrDefault();
                Int64 k = Convert.ToInt64(log.DESCRIPTION);
                cvitem = db.CV_ITEM.Where(x => x.ID_ITEM == k).FirstOrDefault();
                returnData.Add(new LogModel(log.LOG_ID,
                    log.EVENT_CREATED,
                    cvitem,
                    user,
                    log.EVENT_TYPE,
                    log.DESCRIPTION));
            }
            return Ok(returnData);
        }

        //TODO: Write the action for get log by date

        // GET: api/LOGs/5
        [HttpGet]
        [Route("GetLog/{id}")]
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
        //This action should be nested in specific controllers
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

        //This action will reset all tables with initial data (seed)
        [HttpDelete]
        [Route("ResetAllTables")]
        [ResponseType(typeof(void))]
        public IHttpActionResult ResetAllTables()
        {
            db.Database.ExecuteSqlCommand("delete from NSI02.log");
            db.SaveChanges();

            LOG log = new LOG();
            log.LOG_ID = 1; log.EVENT_CREATED = System.DateTime.Now; log.EVENT_TYPE = "ResetAllTables"; log.DESCRIPTION = ""; log.USER_ID = "1";
            db.LOG.Add(log);
            db.SaveChanges();

            return Ok();
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