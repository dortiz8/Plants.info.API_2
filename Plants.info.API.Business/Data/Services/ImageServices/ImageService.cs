using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plants.info.API.Business.Data.Services.BlobStorageService;
using Plants.info.API.Business.Data.Services.ImageServices.Interfaces;
using Plants.info.API.Data.Models;
using Plants.info.API.Data.Repository;

namespace Plants.info.API.Business.Data.Services.ImageServices
{
	public class ImageService : IImageService
	{
        private readonly IBlobStorageService _blobStorageService;
        private readonly IPlantsRepository _plantsRepo;

        public ImageService(IBlobStorageService blobStorageService, IPlantsRepository plantsRepo)
        {
            _blobStorageService = blobStorageService;
            _plantsRepo = plantsRepo;
        }

        public async Task<PlantImage> CreatePlantImage(int userId, int plantId, [FromForm] IFormFile file)
        {
            var plantImage = new PlantImage()
            {
                Name = file.FileName,
                UserId = userId,
                PlantId = plantId,
                Size = file.Length,
                Type = file.ContentType
            };

            string containerName = "containerforuserid" + userId; 

            var imageUrl = await _blobStorageService.SaveImage(containerName, file);

            if(imageUrl != null)
            {
                plantImage.Url = imageUrl;  
            }

            // Save Plant
            await _plantsRepo.CreatePlantImageAsync(plantImage); 
            await _plantsRepo.SaveAllChangesAsync();

            return plantImage; 
        }
    }
}

