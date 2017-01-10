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
using System.IO;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Reflection;
using System.Web;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using AngularJSAuthentication.API.SSO;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/CVitem")]
    public class CV_ITEMController : ApiController
    {
        private MyEntities db = new MyEntities();

        CloudStorageAccount storageAccount;
        CloudBlobClient blobClient;
        CloudBlobContainer blobContainer;
        CloudBlockBlob blob;
        SSO.IdentityClient identity = new SSO.IdentityClient();
        AuthResponse response = new AuthResponse();

        public CV_ITEMController()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureAttachmentsStorage"].ToString());
            blobClient = storageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference("attachment-files");
        
        }


        //Insert CV_ITEM into database and upload to azure blob storage
        //Route: http://localhost:26264/api/CVitem/Create
        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> PostCV_ITEM()
        {
            
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("sid"))
            {
                try
                {
                    response = identity.Auth(HttpContext.Current.Request.Cookies.Get("sid").Value);
                }
                catch
                {
                    return BadRequest("Invalid token. Login in again!");
                }
                if (!(response.Roles.Contains("CV_ADMIN") || response.Roles.Contains("ADMIN")))
                    return BadRequest("You are not authorized for this action");
            }
            else
            {

                return BadRequest("You are not logged in. Please login and try again.");
            }

            CV_ITEM cv = new CV_ITEM();
            List<CV_ITEM_LINK> links= new List<CV_ITEM_LINK>();
            try {
                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

                //lopp for going trough all key:values pairs
                /* foreach (var key in provider.FormData.AllKeys)
                 {
                     //next loop is used for the case when one key has multiple values
                     foreach (var val in provider.FormData.GetValues(key))
                     {
                     }
                 }*/
                cv.CV_TABLE_ID_CV = response.UserId;

                links = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CV_ITEM_LINK>>( provider.FormData.GetValues("LINKS").First());
                cv.NAME = provider.FormData.GetValues("NAME").First();
                cv.DESCRIPTION= provider.FormData.GetValues("DESCRIPTION").First();

                cv.CRITERIA_ID_CRITERIA = Convert.ToInt64(provider.FormData.GetValues("CRITERIA_ID_CRITERIA").First());
                cv.START_DATE= Convert.ToDateTime(provider.FormData.GetValues("START_DATE").First());
                cv.END_DATE= Convert.ToDateTime(provider.FormData.GetValues("END_DATE").First());
                //status=unconfirmed
                cv.STATUS_ID = 1;
                cv.DATE_CREATED = DateTime.Now;
                if (provider.FileData.Count > 0)
                {
                    string uploadedFile = "";
                    string localfilename = "";

                    //loop for multiple files if needed
                    foreach (var file in provider.FileData)
                    {
                        //deletes "" / signs in filename
                        uploadedFile = JsonConvert.DeserializeObject(file.Headers.ContentDisposition.FileName).ToString();
                        localfilename = file.LocalFileName;
                    }
                    var userId = response.UserId;
                    string identifier = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(uploadedFile);
                    string path = userId + "-" + identifier + extension;
                    var fileName = Path.GetFileName(path);

                    string[] _supportedExtensions = { ".zip", ".rar", ".doc", ".pdf", ".docx", ".odt" };
                    if (!_supportedExtensions.Contains(extension))
                    {
                        return BadRequest("File not supported");
                    }

                    blobContainer.CreateIfNotExists();
                    blob = blobContainer.GetBlockBlobReference(fileName);
                    //localfilename: path of the file on server
                    blob.UploadFromFile(localfilename);
                    cv.CV_ITEM_LINK_LINK = blob.Uri.ToString();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            //saving CV_item to database
            db.CV_ITEM.Add(cv);
            db.SaveChanges();

            //now update CV_ITEM_ID in every link
            foreach (CV_ITEM_LINK link in links)
                link.CV_ITEM_ID = cv.ID_ITEM;

            db.CV_ITEM_LINK.AddRange(links);
            db.SaveChanges();
           //returns cv_item atributes incuding list of ATTACHMENTS
            return Ok(cv);

        }
        //Get CV_ITEM list via ID_CV (CV_TABLE primary key)
        //Route e.g. : http://localhost:26264/api/CVitem/GetAll/3
        [HttpGet]
        [Route("GetAll")]
        [ResponseType(typeof(List<CV_ITEM>))]
        public IHttpActionResult GetAllItems()
        {
            //currently available without authentification
            /* 
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("sid"))
            {
                try
                {
                    response = identity.Auth(HttpContext.Current.Request.Cookies.Get("sid").Value);
                }
                catch
                {
                    return BadRequest("Invalid token. Login in again!");
                }
            }
            else
            {

                return BadRequest("You are not logged in. Please login and try again.");
            }*/
            List<CV_ITEM> temp = new List<CV_ITEM>();
            try
            {
                temp = db.CV_ITEM.Where(a => a.STATUS_ID == 2).ToList();
            }
            catch (DBConcurrencyException e)
            {
                return NotFound();
            }
            return Ok(temp); ;
        }

        //Get CV_ITEM list via ID_CV (CV_TABLE primary key)
        //Route e.g. : http://localhost:26264/api/CVitem/GetAll/3
        [HttpGet]
        [Route("GetMy")]
        [ResponseType(typeof(List<CV_ITEM>))]
        public IHttpActionResult GetMy()
        {
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("sid"))
            {
                try
                {
                    response = identity.Auth(HttpContext.Current.Request.Cookies.Get("sid").Value);
                }
                catch
                {
                    return BadRequest("Invalid token. Login in again!");
                }
                if (!(response.Roles.Contains("CV_ADMIN") || response.Roles.Contains("ADMIN")))
                    return BadRequest("You are not authorized for this action");
            }
            else
            {

                return BadRequest("You are not logged in. Please login and try again.");
            }

            List<CV_ITEM> temp = new List<CV_ITEM>();
            try
            {
                temp = db.CV_ITEM.Where(a =>a.CV_TABLE_ID_CV ==response.UserId && a.STATUS_ID == 2).ToList();
            }
            catch (DBConcurrencyException e)
            {
                return NotFound();
            }
            return Ok(temp); ;
        }



        [HttpGet]
        [Route("GetProcessedRequests")]
        [ResponseType(typeof(List<CV_ITEM>))]
                                                       
        public IHttpActionResult GetProcessedRequests()
        {

            List<CV_ITEM> confirmedRejected = new List<CV_ITEM>();
          
            try
            {
                confirmedRejected = db.CV_ITEM.Where(s => s.CV_ITEM_STATUS.STATUS == "confirmed" || s.CV_ITEM_STATUS.STATUS == "rejected").ToList();
                return Ok(confirmedRejected);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetAllUnconfirmedAndModified")]
        [ResponseType(typeof(List<CV_ITEM>))]
                                                       
        public IHttpActionResult GetAllUnconfirmedItems()
        {
           
            List<CV_ITEM> unconfirmedModified=new List<CV_ITEM>();
            try
            {
                unconfirmedModified = db.CV_ITEM.Where(s => s.CV_ITEM_STATUS.STATUS == "unconfirmed" || s.CV_ITEM_STATUS.STATUS =="modified").ToList();
                return Ok(unconfirmedModified);
            }
            catch (Exception)
            {
                return NotFound();
            }  
        }

        [HttpGet]
        [Route("GetMyUnconfirmedRequests")]
        [ResponseType(typeof(List<CV_ITEM>))]

        public IHttpActionResult GetMyUnconfirmedRequests()
        {
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("sid"))
            {
                try
                {
                    response = identity.Auth(HttpContext.Current.Request.Cookies.Get("sid").Value);
                }
                catch
                {
                    return BadRequest("Invalid token. Login in again!");
                }
                if (!(response.Roles.Contains("CV_ADMIN") || response.Roles.Contains("ADMIN")))
                    return BadRequest("You are not authorized for this action");
            }
            else
            {

                return BadRequest("You are not logged in. Please login and try again.");
            }


            List<CV_ITEM> unconfirmedModified = new List<CV_ITEM>();
            try
            {
                unconfirmedModified = db.CV_ITEM.Where(s =>( s.CV_ITEM_STATUS.STATUS == "unconfirmed" || s.CV_ITEM_STATUS.STATUS == "modified") && s.CV_TABLE_ID_CV==response.UserId).ToList();
                return Ok(unconfirmedModified);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        //Get CV_ITEM via ID_ITEM
        //Route e.g. : http://localhost:26264/api/CVitem/Get/42
        [HttpGet]
        [Route("Get/{ID_ITEM}")]
        [ResponseType(typeof(List<CV_ITEM>))]
        public IHttpActionResult GetCV_ITEM(long ID_ITEM)
        {
            CV_ITEM temp = new CV_ITEM();
            try {
               temp = db.CV_ITEM.Find(ID_ITEM);
            }
            catch (DBConcurrencyException e)
            {
                return NotFound();
            }
            return Ok(temp);
        }

        [HttpPost]
        [Route("UpdateStatus/{cv_item_id}/{status_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateStatus(long cv_item_id, int status_id)
        {
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("sid"))
            {
                try
                {
                    response = identity.Auth(HttpContext.Current.Request.Cookies.Get("sid").Value);
                }
                catch
                {
                    return BadRequest("Invalid token. Login in again!");
                }
                if (!(response.Roles.Contains("ADMIN") || response.Roles.Contains("STUDENTSKA")))
                    return BadRequest("You are not authorized for this action");
            }
            else
            {

                return BadRequest("You are not logged in. Please login and try again.");
            }



            try
            {
                var status = db.CV_ITEM_STATUS.Where(a => a.ID == status_id).Single();
                var result = db.CV_ITEM.Where(a => a.ID_ITEM == cv_item_id).Single();
                result.STATUS_ID = status_id;

                if (status.STATUS == "confirmed" || status.STATUS == "rejected")
                {
                    var log = new LOG();
                    log.EVENT_CREATED = DateTime.Now;
                    log.EVENT_TYPE = status.STATUS;
                    log.DESCRIPTION = cv_item_id.ToString();
                    log.USER_ID = response.UserId;
                    db.LOG.Add(log);
                    db.SaveChanges();
                }
                
                //saving to database                   
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPut]
        [Route("Update/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCV_ITEM(long id)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }


            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("sid"))
            {
                try
                {
                    response = identity.Auth(HttpContext.Current.Request.Cookies.Get("sid").Value);
                }
                catch
                {
                    return BadRequest("Invalid token. Login in again!");
                }
                if (!(response.Roles.Contains("CV_ADMIN") || response.Roles.Contains("ADMIN")))
                    return BadRequest("You are not authorized for this action");
            }
            else
            {

                return BadRequest("You are not logged in. Please login and try again.");
            }


            CV_ITEM cv = new CV_ITEM();
            CV_ITEM currentCV = new CV_ITEM();
            List<CV_ITEM_LINK> links = new List<CV_ITEM_LINK>();
            try
            {
                //AsNOTracking(): no caching of in DBcontext or ObjectContext. 
                //Without this, internal server error happens when saving to database
                currentCV = db.CV_ITEM.AsNoTracking().First(a => a.ID_ITEM == id);
            }
            catch (DBConcurrencyException e)
            {
                return NotFound();
            }


            try
            {
                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);
                links = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CV_ITEM_LINK>>(provider.FormData.GetValues("LINKS").First());

                cv.CV_TABLE_ID_CV = response.UserId;
                cv.ID_ITEM = id;
                cv.NAME = provider.FormData.GetValues("NAME").First();
                cv.DESCRIPTION = provider.FormData.GetValues("DESCRIPTION").First();
                //ispravka ToInt32 je bilo 64
                cv.CRITERIA_ID_CRITERIA = Convert.ToInt64(provider.FormData.GetValues("CRITERIA_ID_CRITERIA").First());
                cv.START_DATE =Convert.ToDateTime(provider.FormData.GetValues("START_DATE").First());
                cv.END_DATE = Convert.ToDateTime(provider.FormData.GetValues("END_DATE").First());
                cv.DATE_CREATED = currentCV.DATE_CREATED;
               
                //STATUS=modified
                cv.STATUS_ID = 3;
                cv.DATE_MODIFIED = DateTime.Now;

                //current file is deleted only if new is provided
                if (provider.FileData.Count > 0)
                {
                    if (currentCV.CV_ITEM_LINK_LINK != null)
                    {
                        //delete old ATTACHMENT_LINK from blob storage 
                        string a = currentCV.CV_ITEM_LINK_LINK.Replace("https://etfnsi.blob.core.windows.net/attachment-files/", "");
                        blobContainer.CreateIfNotExists();
                        blob = blobContainer.GetBlockBlobReference(a);
                        blob.DeleteIfExists();
                    }
                    string uploadedFile = "";
                    string localfilename = "";

                    //loop for multiple files if needed
                    foreach (var file in provider.FileData)
                    {
                        //deletes "" / signs in filename
                        uploadedFile = JsonConvert.DeserializeObject(file.Headers.ContentDisposition.FileName).ToString();
                        localfilename = file.LocalFileName;
                    }
                    var userId = 10;
                    string identifier = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(uploadedFile);
                    string path = userId + "-" + identifier + extension;
                    var fileName = Path.GetFileName(path);

                    string[] _supportedExtensions = { ".zip", ".rar", ".doc", ".pdf", ".docx", ".odt" };
                    if (!_supportedExtensions.Contains(extension))
                    {
                        return BadRequest("File not supported");
                    }

                    blobContainer.CreateIfNotExists();
                    blob = blobContainer.GetBlockBlobReference(fileName);
                    //localfilename: path of the file on server
                    blob.UploadFromFile(localfilename);
                    cv.CV_ITEM_LINK_LINK = blob.Uri.ToString();
                }
                //no new file uploaded => use old file
                else
                {
                    cv.CV_ITEM_LINK_LINK = currentCV.CV_ITEM_LINK_LINK;
                }

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            //saving to database                   
            db.Entry(cv).State = EntityState.Modified;

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
            //remove all current links from database
            db.CV_ITEM_LINK.RemoveRange(db.CV_ITEM_LINK.Where(l => l.CV_ITEM_ID == cv.ID_ITEM));
           
            //update CV_ITEM_ID in every link; In case that CV_ITEM_ID field in links is not set
            foreach (CV_ITEM_LINK link in links)
                link.CV_ITEM_ID = id;

            //add new links to database
            db.CV_ITEM_LINK.AddRange(links);    

            //db.ATTACHMENT.AddRange(links);
            db.SaveChanges();

            return Ok(cv);
        }



        [HttpDelete]
        [Route("Delete/{id}")]
        [ResponseType(typeof(CV_ITEM))]
        public IHttpActionResult DeleteCV_ITEM(long id)
        {
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("sid"))
            {
                try
                {
                    response = identity.Auth(HttpContext.Current.Request.Cookies.Get("sid").Value);
                }
                catch
                {
                    return BadRequest("Invalid token. Login in again!");
                }
                if (!(response.Roles.Contains("CV_ADMIN") || response.Roles.Contains("ADMIN")))
                    return BadRequest("You are not authorized for this action");
            }
            else
            {

                return BadRequest("You are not logged in. Please login and try again.");
            }


            CV_ITEM cV_ITEM = db.CV_ITEM.Find(id);
            if (cV_ITEM.CV_TABLE_ID_CV != response.UserId)
                return BadRequest("You cannot delete item from other user!");

            if (cV_ITEM == null)
            {
                return NotFound();
            }
            //soft delete: change status to "deleted"
            cV_ITEM.STATUS_ID = 5;
            try {
                db.Entry(cV_ITEM).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return InternalServerError();
            }
   
            return Ok("Deleted CV_ITEM: "+cV_ITEM.ID_ITEM);
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