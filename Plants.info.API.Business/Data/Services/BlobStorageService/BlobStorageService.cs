using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Plants.info.API.Data.Models;
using Plants.info.API.Models;

namespace Plants.info.API.Business.Data.Services.BlobStorageService
{
	public class BlobStorageService : IBlobStorageService
	{
        public BlobServiceClient _blobServiceClient { get; }

		public BlobStorageService(BlobServiceClient blobServiceClient)
		{
            _blobServiceClient = blobServiceClient;
        }


        public async Task<string> SaveImage(string blobContainerName, IFormFile file)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var client =  _blobServiceClient.GetBlobContainerClient(file.FileName); 

                var newBlobName = $"{Guid.NewGuid().ToString()}.jpg";

                //var base64String = plantImage.Base64.Substring(plantImage.Base64.IndexOf(",")+1); 
                //var bytes = Convert.FromBase64String(plantImage.Base64);
                //Stream stream = new MemoryStream(bytes); 
                
                var content = await containerClient.UploadBlobAsync(newBlobName, file.OpenReadStream());
                return $"{containerClient.Uri.AbsoluteUri}/{newBlobName}"; 

            //return ""; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ""; 
            }
        }
    }
}

