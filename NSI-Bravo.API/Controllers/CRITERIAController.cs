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
    public class masterCriteria
    {
        public long ID_CRITERIA { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<int> CRITERIA_LEVEL { get; set; }
        public Nullable<long> PARENT_CRITERIA { get; set; }
        public int POINTS { get; set; }
        public Nullable<System.DateTime> DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_MODIFIED { get; set; }
        
        public masterCriteria(long id_criteria, string name, string description, Nullable<int> criteria_level, Nullable<long> parent_criteria, int points,
                               Nullable<System.DateTime> date_created, Nullable<System.DateTime> date_modified)
        {
            ID_CRITERIA = id_criteria;
            NAME = name;
            DESCRIPTION = description;
            CRITERIA_LEVEL = criteria_level;
            PARENT_CRITERIA = parent_criteria;
            POINTS = points;
            DATE_CREATED = date_created;
            DATE_MODIFIED = date_modified;

        }
    };

    [RoutePrefix("api/Criteria")]
    public class CRITERIAController : ApiController
    {
        private MyEntities db = new MyEntities();

       
        // GET: api/CRITERIA/GetCriteria/5
        [HttpGet]
        [Route("GetCriteria/{id}")]
        [ResponseType(typeof(CRITERIA))]
        //Returns a JSON with requested criteria but also returns other parent or child criteria. R
        // Requested criteria is always at the end of JSON
        public IHttpActionResult GetCriteria(long id)
        {
            CRITERIA cRITERIA = db.CRITERIA.Find(id);
            if (cRITERIA == null)
            {
                return NotFound();
            }
            /*var returnData = new
            {
                id_criteria = cRITERIA.ID_CRITERIA,
                name = cRITERIA.NAME,
                description = cRITERIA.DESCRIPTION,
                parent_criteria=cRITERIA.PARENT_CRITERIA,
                points=cRITERIA.POINTS,
                date_created=cRITERIA.DATE_CREATED,
                date_modified=cRITERIA.DATE_MODIFIED,
            };*/
            return Ok(cRITERIA);
        }

        [HttpGet]
        [Route("GetAllCriteria")]
        
        //Returns a JSON with all criteria entries
        public IHttpActionResult GetAllCriteria()
        {
            return Ok(db.CRITERIA);
        }

        [HttpGet]
        [Route("GetAllMasterCriteria")]

        //Returns a JSON with all criteria entries
        public IHttpActionResult GetAllMasterCriteria()
        {
            List<CRITERIA> masterlist = db.CRITERIA.Where(u => u.PARENT_CRITERIA == null).ToList();
            List<masterCriteria> temp = new List<masterCriteria>();

            foreach(CRITERIA crit in masterlist)
            {
                temp.Add(new masterCriteria(crit.ID_CRITERIA,
                                            crit.NAME,
                                            crit.DESCRIPTION,
                                            crit.CRITERIA_LEVEL,
                                            crit.PARENT_CRITERIA,
                                            crit.POINTS,
                                            crit.DATE_CREATED,
                                            crit.DATE_MODIFIED
                                            ));

            }
            return Ok(temp);
        }
        // PUT: api/CRITERIA/UpdateCriteria/5
        [HttpPut]
        [Route("UpdateCriteria/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateCriteria(long id, CRITERIA cRITERIA)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cRITERIA.ID_CRITERIA)
            {
                return BadRequest("id doesn't match");
            }
            cRITERIA.DATE_MODIFIED = DateTime.Now;
            db.Entry(cRITERIA).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CRITERIAExists(id))
                {
                    return NotFound();
                }
            }

            return Ok(cRITERIA);
        }

        // POST: api/CRITERIA/PostCriteria

        //Writes criteria to the database but causes internal server error. Need fix
        [HttpPost]
        [Route("PostCriteria")]
        [ResponseType(typeof(CRITERIA))]
        public IHttpActionResult PostCriteria(CRITERIA cRITERIA)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            cRITERIA.DATE_CREATED = DateTime.Now;
            db.CRITERIA.Add(cRITERIA);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CRITERIAExists(cRITERIA.ID_CRITERIA))
                {
                    return Conflict();
                }
            }

            return Ok(cRITERIA);
        }

        // DELETE: api/CRITERIA/5

        //Delete works but not as it should. It only needs to allow delete of the last child
        [HttpDelete]
        [Route("DeleteCriteria/{id}")]
        [ResponseType(typeof(CRITERIA))]
        public IHttpActionResult DeleteCRITERIA(long id)
        {
            CRITERIA cRITERIA = db.CRITERIA.Find(id);
            foreach(CRITERIA c in db.CRITERIA)
            {
                if (c.PARENT_CRITERIA == id)
                    return BadRequest("Cannot delete because criteria has subcriteria!");
            }
            if (cRITERIA == null)
            {
                return NotFound();
                
            }

            db.CRITERIA.Remove(cRITERIA);
            db.SaveChanges();

            return Ok(cRITERIA);
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CRITERIAExists(long id)
        {
            return db.CRITERIA.Count(e => e.ID_CRITERIA == id) > 0;
        }
    }
}