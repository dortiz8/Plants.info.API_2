using System;
using Microsoft.AspNetCore.Http;
using Plants.info.API.Data.Models;

namespace Plants.info.API.Business.Data.Services.BlobStorageService
{
	public interface IBlobStorageService
	{
		Task<string> SaveImage(string blobContainerName, IFormFile file); 
	}
}

