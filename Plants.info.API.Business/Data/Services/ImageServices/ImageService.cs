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
                Type = file.ContentType,
                DateAdded = DateTime.Now
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

        public async Task<PlantImage> CreatePlantImageByName(int userId, int plantId, string plantName, string url)
        {
            var plantImage = new PlantImage()
            {
                Name = plantName,
                UserId = userId,
                PlantId = plantId,
                Size = default,
                Type = "image/jpeg",
                Url = url,
                DateAdded = DateTime.Now
            };

            await _plantsRepo.CreatePlantImageAsync(plantImage);
            await _plantsRepo.SaveAllChangesAsync();

            return plantImage;

        }

        public async Task DeletePlantImages(int userId, int plantId)
        {
            var images = await _plantsRepo.GetPlantImages(userId, plantId);

            if (images == null) return;

            string containerName = "containerforuserid" + userId;

            var deleted = await _blobStorageService.DeleteImages(containerName, images);

            if (deleted == null || deleted == false) return;

            await _plantsRepo.DeletePlantImages(userId, plantId);
            await _plantsRepo.SaveAllChangesAsync(); 
        }
    }
}

