using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plants.info.API.Data.Models;

namespace Plants.info.API.Business.Data.Services.ImageServices.Interfaces
{
	public interface IImageService
	{
		Task<PlantImage> CreatePlantImage(int userId, int plantId, [FromForm] IFormFile file); 
	}
}

