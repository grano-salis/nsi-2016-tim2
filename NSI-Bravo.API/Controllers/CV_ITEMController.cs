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

            CV_ITEM cv = new CV_ITEM();
            List<ATTACHMENT> links= new List<ATTACHMENT>();
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
                links= Newtonsoft.Json.JsonConvert.DeserializeObject<List<ATTACHMENT>>( provider.FormData.GetValues("LINKS").First());
                cv.NAME = provider.FormData.GetValues("NAME").First();
                cv.DESCRIPTION= provider.FormData.GetValues("DESCRIPTION").First();
                cv.CV_TABLE_ID_CV=Convert.ToInt64(provider.FormData.GetValues("CV_TABLE_ID_CV").First());
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
                    cv.ATTACHMENT_LINK = blob.Uri.ToString();
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
            foreach (ATTACHMENT link in links)
                link.CV_ITEM_ID = cv.ID_ITEM;

            db.ATTACHMENT.AddRange(links);
            db.SaveChanges();
           //returns cv_item atributes incuding list of ATTACHMENTS
            return Ok(cv);

        }
        //Get CV_ITEM list via ID_CV (CV_TABLE primary key)
        //Route e.g. : http://localhost:26264/api/CVitem/GetAll/3
        [HttpGet]
        [Route("GetAll/{ID_CV}")]
        [ResponseType(typeof(List<CV_ITEM>))]
        public IHttpActionResult GetAllItems(long ID_CV)
        {
            List<CV_ITEM> temp = new List<CV_ITEM>();
            try
            {
                temp = db.CV_ITEM.Where(a => a.CV_TABLE_ID_CV == ID_CV && a.STATUS_ID==2).ToList();
            }
            catch (DBConcurrencyException e)
            {
                return NotFound();
            }
            return Ok(temp); ;
        }

        [HttpGet]
        [Route("GetProcessedRequests/{ID_CV}")]
        [ResponseType(typeof(List<CV_ITEM>))]
        public IHttpActionResult GetProcessedRequests(long ID_CV)
        {

            CV_ITEM_STATUS confirmed;
            CV_ITEM_STATUS rejected;
            try
            {
                confirmed = db.CV_ITEM_STATUS.Where(s => s.STATUS == "confirmed").Single();
                rejected = db.CV_ITEM_STATUS.Where(s => s.STATUS == "rejected").Single();
                var temp = db.CV_ITEM.Join(db.CV_TABLE, s => s.CV_TABLE_ID_CV, sa => sa.ID_CV, (s, sa) => new { cv_item = s, cv = sa }).Where(a => a.cv_item.CV_TABLE_ID_CV == ID_CV && (a.cv_item.CV_ITEM_STATUS.ID == confirmed.ID || a.cv_item.CV_ITEM_STATUS.ID == rejected.ID)).Select(a => new { a.cv_item, a.cv }).ToList();
                return Ok(temp);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetAllUnconfirmedAndModified/{ID_CV}")]
        [ResponseType(typeof(List<CV_ITEM>))]
        public IHttpActionResult GetAllUnconfirmedItems(long ID_CV)
        {
            
            CV_ITEM_STATUS unconfirmed;
            CV_ITEM_STATUS modified;
            try
            {
                unconfirmed = db.CV_ITEM_STATUS.Where(s => s.STATUS == "unconfirmed").Single();
                modified = db.CV_ITEM_STATUS.Where(s => s.STATUS == "modified").Single();
                var temp = db.CV_ITEM.Join(db.CV_TABLE, s => s.CV_TABLE_ID_CV, sa => sa.ID_CV, (s, sa) => new { cv_item = s, cv = sa }).Where(a => a.cv_item.CV_TABLE_ID_CV == ID_CV && (a.cv_item.CV_ITEM_STATUS.ID == unconfirmed.ID || a.cv_item.CV_ITEM_STATUS.ID == modified.ID)).Select(a => new { a.cv_item, a.cv }).ToList();
                return Ok(temp);
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
        [Route("Update/{cv_item_id}/{status_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateStatus(long cv_item_id, int status_id)
        {
            try
            {
                var status = db.CV_ITEM_STATUS.Where(a => a.ID == status_id).Single();
                var result = db.CV_ITEM.Where(a => a.ID_ITEM == cv_item_id).Single();
                result.STATUS_ID = status_id;
                // treba postaviti user_id

                if (status.STATUS == "confirmed" || status.STATUS == "rejected")
                {
                    var log = new LOG();
                    log.EVENT_CREATED = DateTime.Now;
                    log.EVENT_TYPE = status.STATUS;
                    log.DESCRIPTION = cv_item_id.ToString();
                    // treba postaviti pravi user_id
                    log.USER_ID = "1";
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

            /* if (id != item.ID_ITEM)
            {
                return BadRequest("id doesn't match");
            }*/
            CV_ITEM cv = new CV_ITEM();
            CV_ITEM currentCV = new CV_ITEM();
            List<ATTACHMENT> links = new List<ATTACHMENT>();
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
                links = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ATTACHMENT>>(provider.FormData.GetValues("LINKS").First());
                cv.ID_ITEM = id;
                cv.NAME = provider.FormData.GetValues("NAME").First();
                cv.DESCRIPTION = provider.FormData.GetValues("DESCRIPTION").First();
                cv.CV_TABLE_ID_CV = Convert.ToInt64(provider.FormData.GetValues("CV_TABLE_ID_CV").First());
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
                    if (currentCV.ATTACHMENT_LINK != null)
                    {
                        //delete old ATTACHMENT_LINK from blob storage 
                        string a = currentCV.ATTACHMENT_LINK.Replace("https://etfnsi.blob.core.windows.net/attachment-files/", "");
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
                    cv.ATTACHMENT_LINK = blob.Uri.ToString();
                }
                //no new file uploaded => use old file
                else
                {
                    cv.ATTACHMENT_LINK = currentCV.ATTACHMENT_LINK;
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
            db.ATTACHMENT.RemoveRange(db.ATTACHMENT.Where(l => l.CV_ITEM_ID == cv.ID_ITEM));
           
            //update CV_ITEM_ID in every link; In case that CV_ITEM_ID field in links is not set
            foreach (ATTACHMENT link in links)
                link.CV_ITEM_ID = id;

            //add new links to database
            db.ATTACHMENT.AddRange(links);     

            //db.ATTACHMENT.AddRange(links);
            db.SaveChanges();

            return Ok(cv);
        }



        [HttpDelete]
        [Route("Delete/{id}")]
        [ResponseType(typeof(CV_ITEM))]
        public IHttpActionResult DeleteCV_ITEM(long id)
        {
            CV_ITEM cV_ITEM = db.CV_ITEM.Find(id);
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

        private void UploadToBlobStorage()
        {

            var cvItemId = 23;
            var fileExtension = ".zip";
            var fileName = Path.GetFileName("attachment-" + cvItemId+fileExtension);
            
            // string directoryPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "attachments");
            //var directoryPath = Server.MapPath("~/Resources/Attachments/NotificationRuns");
            string directoryPath = string.Format(@"{0}\{1}", @"C:\", "attachments");

            if (!System.IO.Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);
            var path = Path.Combine(directoryPath, fileName);
            //  model.File.SaveAs(path);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureAttachmentsStorage"].ToString());
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer blobContainer = blobClient.GetContainerReference("attachment-files");
            blobContainer.CreateIfNotExists();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName);
            blob.UploadFromFile(path);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

        }

    }
    
    

}