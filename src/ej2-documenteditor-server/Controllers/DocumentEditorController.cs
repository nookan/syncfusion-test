using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using Syncfusion.EJ2.DocumentEditor;
using System.Threading.Tasks;


namespace EJ2DocumentEditorServer.Controllers
{
    [Route("api/[controller]")]
    public class DocumentEditorController : Controller
    {
        [AcceptVerbs("Post")]
        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        [Route("LoadFromS3")]
        public async Task<string> LoadFromS3()
        {
            string accessKey = "sdfsdfsdfsdfsdf";
            string secretKey = "sdfsdfsdfsdfsdfsdfs/";
            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
            IAmazonS3 s3Client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
            // Set the bucket name and object key of the DOCX document
            var bucketName = "testbucketnnk";
            var objectKey = "GettingStarted.docx";
            // Create a GetObjectRequest to retrieve the document
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };
            // Retrieve the document from S3
            var response = await s3Client.GetObjectAsync(request);
            // Read the content of the document
            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            await response.ResponseStream.CopyToAsync(stream);
            WordDocument document = WordDocument.Load(stream, FormatType.Docx);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            stream.Close();
            return json;
        }
        [AcceptVerbs("Post")]
        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        [Route("SaveToS3")]
        public string SaveToS3()
        {
            IFormFile file = HttpContext.Request.Form.Files[0];
            // string bucket = HttpContext.Request.Form["bucket"];
            // string filepath = HttpContext.Request.Form["filepath"];
            Stream stream = new MemoryStream();
            file.CopyTo(stream);
            UploadFileStreamToS3(stream, "testbucketnnk", "GettingStarted.docx");
            stream.Close();
            return "Sucess";
        }

        public bool UploadFileStreamToS3(System.IO.Stream localFilePath, string bucketName, string fileNameInS3)
        {
            // Replace 'YOUR_ACCESS_KEY' and 'YOUR_SECRET_KEY' with your actual AWS access key and secret key.
            string accessKey = "dsfsdfsdfdsfsdfdf";
            string secretKey = "sfsdfsdfsdfsdfsdfsd/";
            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
            IAmazonS3 client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
            try
            {
                TransferUtility transferUtility = new TransferUtility(client);
                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = localFilePath,
                    Key = fileNameInS3
                };
                transferUtility.Upload(uploadRequest);

                // Log success or any additional information
                Console.WriteLine("File uploaded successfully!");
            }
            catch (AmazonS3Exception s3Exception)
            {
                // Log the exception details for troubleshooting
                Console.WriteLine($"AmazonS3Exception: {s3Exception.Message}");
                Console.WriteLine($"Error code: {s3Exception.ErrorCode}");
                Console.WriteLine($"Request ID: {s3Exception.RequestId}");
                Console.WriteLine($"HTTP Status Code: {s3Exception.StatusCode}");
                Console.WriteLine($"Error Type: {s3Exception.ErrorType}");
                Console.WriteLine($"Amazon ID2: {s3Exception.AmazonId2}");
                // Rethrow the exception to propagate it
                throw;
            }
            return true; //indicate that the file was sent  
        }
    
    }

}
