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

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/CVitem")]
    public class CV_ITEMController : ApiController
    {
        private MyEntities db = new MyEntities();


        //Insert CV_ITEM into database and upload to azure blob storage
        //Route: http://localhost:26264/api/CVitem/Create
        [HttpPost]
        [Route("Create")]
        public IHttpActionResult PostCV_ITEM(CV_ITEM item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureAttachmentsStorage"].ToString());
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("attachment-files");


            var extension = Path.GetExtension(item.ATTACHMENT_LINK);
            //file must be one of these extensions
            string[] _supportedExtensions = { ".zip", ".rar", ".doc", ".pdf", ".docx", ".odt" };
            if (!_supportedExtensions.Contains(extension))
            {
                return BadRequest("File not supported");
            }
            string identifier = Guid.NewGuid().ToString();
            var userId = 10;
            string path = userId+"-" + identifier + extension;
            var fileName = Path.GetFileName(path);

            blobContainer.CreateIfNotExists();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName);

            blob.UploadFromFile(item.ATTACHMENT_LINK);
            item.ATTACHMENT_LINK = blob.Uri.ToString();
           
            //saving CV_item to database
            db.CV_ITEM.Add(item);
           
            db.Database.Log = s =>
            {
                // You can put a breakpoint here and examine s with the TextVisualizer
                // Note that only some of the s values are SQL statements
                Debug.Print(s);
            };
            db.SaveChanges();
            //string a = item.ATTACHMENT_LINK.Replace("https://etfnsi.blob.core.windows.net/attachment-files/", "");
            // blob = blobContainer.GetBlockBlobReference(a);
            // blob.DeleteIfExists();
            return Ok($"File: {fileName} has successfully uploaded");

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
                temp = db.CV_ITEM.Where(a => a.CV_TABLE_ID_CV == ID_CV).ToList();
            }
            catch (DBConcurrencyException e)
            {
                return NotFound();
            }
            return Ok(temp); ;
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


        [HttpPut]
        [Route("Update/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCV_ITEM(long id, CV_ITEM item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureAttachmentsStorage"].ToString());
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("attachment-files");

            if (id != item.ID_ITEM)
            {
                return BadRequest("id doesn't match");
            }
            CV_ITEM temp = new CV_ITEM();
            try
            {
                //AsNOTracking(): no caching of in DBcontext or ObjectContext. 
                //Without this, internal server error happens when saving to database
                temp = db.CV_ITEM.AsNoTracking().First(a => a.ID_ITEM == id);
            }
            catch (DBConcurrencyException e)
            {
                return NotFound();
            }
           
            if (item.ATTACHMENT_LINK != null)
            {
                var extension = Path.GetExtension(item.ATTACHMENT_LINK);
                //file must be one of these extensions
                string[] _supportedExtensions = { ".zip", ".rar", ".doc", ".pdf", ".docx", ".odt" };
                if (!_supportedExtensions.Contains(extension))
                {
                    return BadRequest("File not supported");
                }
                //gets blob reference of CV_ITEM currently in database
                               
                if (temp.ATTACHMENT_LINK != null)
                {
                    string a = temp.ATTACHMENT_LINK.Replace("https://etfnsi.blob.core.windows.net/attachment-files/", "");
                    blobContainer.CreateIfNotExists();
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(a);

                    //delete old ATTACHMENT_LINK from blob storage 
                    blob.DeleteIfExists();
                }

                //generate new filename
                string identifier = Guid.NewGuid().ToString();
                var userId = 10;
                string path = userId + "-" + identifier + extension;
                var fileName = Path.GetFileName(path);

                //upload new attachment
                CloudBlockBlob blob1 = blobContainer.GetBlockBlobReference(fileName);
                blob1.UploadFromFile(item.ATTACHMENT_LINK);
                item.ATTACHMENT_LINK = blob1.Uri.ToString();
            }

            //saving to database                   
            db.Entry(item).State = EntityState.Modified;
           
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

            return Ok(item);
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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureAttachmentsStorage"].ToString());
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("attachment-files");


            if (cV_ITEM.ATTACHMENT_LINK != null)
            {
                string a = cV_ITEM.ATTACHMENT_LINK.Replace("https://etfnsi.blob.core.windows.net/attachment-files/", "");
                blobContainer.CreateIfNotExists();
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(a);

                //delete file(ATTACHMENT_LINK) from blob storage 
                blob.DeleteIfExists();
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