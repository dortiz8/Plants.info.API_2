using System;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plants.info.API.Business.Data.Services.AppAuditService.Interfaces;
using Plants.info.API.Business.Data.Services.BlobStorageService;
using Plants.info.API.Business.Data.Services.ImageServices.Interfaces;
using Plants.info.API.Common.Data.Utils;
using Plants.info.API.Data.Models;
using Plants.info.API.Data.Services.PlantServices.Interfaces;
using Plants.info.API.Data.Services.UserServices;
using Plants.info.API.Models;

namespace Plants.info.API.Controllers
{
    [Route("api/users/{userId}/plants/")] // Since we need to gather user information to get plants we reflect the URL as such and set plants as a child resource of Users. 
    [Authorize] //we have no way to pass down id values to this policy, therefore a custom attribute needs to be created but it is not recommended.
    [ApiController]
    public class ImagesController : ControllerBase
	{
        private readonly IPlantService _plantService;
        private readonly ILogger<ImagesController> _log;
        private readonly IImageService _imageService;
        private readonly IUserService _userService;
        private readonly IAppAuditService _appAuditService;



        public ImagesController(ILogger<ImagesController> logger, IPlantService plantService, IImageService imageService, IUserService userService, IAppAuditService appAuditService)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger)); // Null check in case the container changes and it returns a null value
            _userService = userService;
            _plantService = plantService;
            _imageService = imageService;
            _appAuditService = appAuditService;
        }

        [HttpPost("{plantId}/images/addfile")]
        public async Task<ActionResult> CreatePlantImageByFile(int userId, int plantId, [FromForm] IFormFile file) // The attribute is optional because the type is complex and will come from body.
        {
            try
            {
                if (IdsDoNotMatch(userId)) return Forbid(); // returns 403 code
                if (!await _userService.UserExistsAsync(userId))
                {
                    _log.LogInformation($"The user with id {userId} was not found.");
                    return NotFound();
                }

                var plant = await _plantService.DoesPlantExists(userId, plantId);
                if (plant == false) return NotFound();
                //if (file == null) return BadRequest(); // Rely on default image if no image is passed
               
                await _imageService.CreatePlantImage(userId, plantId, file); 
               

                return Ok();
            }
            catch (Exception ex)
            {
                await _appAuditService.AddToAppAudit((int)ToolIds.Images, "Error: CreatePlantImageByFile", ex.Message);
                return StatusCode(500, "A problem occurred while handling your request");
            }

        }

        [HttpPost("{plantId}/images/addurl")]
        public async Task<ActionResult> CreatePlantImageByName(int userId, int plantId, [FromQuery] string plantName, [FromQuery] string url)
        {
            try
            {
                if (IdsDoNotMatch(userId)) return Forbid(); // returns 403 code
                if (!await _userService.UserExistsAsync(userId))
                {
                    _log.LogInformation($"The user with id {userId} was not found.");
                    return NotFound();
                }

                var plant = await _plantService.DoesPlantExists(userId, plantId);
                if (plant == false) return NotFound();

                await _imageService.CreatePlantImageByName(userId, plantId, plantName, url);

                return Ok();
            }
            catch (Exception ex)
            {
                await _appAuditService.AddToAppAudit((int)ToolIds.Images, "Error: CreatePlantImageByName", ex.Message);
                return StatusCode(500, "A problem occurred while handling your request");
            }

        }

        [HttpDelete("{plantId}/images", Name = "DeletePlantImages")]
        public async Task<ActionResult> DeletePlantImages(int userId, int plantId)
        {
            try
            {
                //if (IdsDoNotMatch(userId)) return Forbid(); // returns 403 code
                if (!await _userService.UserExistsAsync(userId))
                {
                    _log.LogInformation($"The user with id {userId} was not found.");
                    return NotFound();
                }

                var plant = await _plantService.DoesPlantExists(userId, plantId);
                if (plant == false) return NotFound();

                await _imageService.DeletePlantImages(userId, plantId);

                return Ok(); 
            }
            catch (Exception ex)
            {
                await _appAuditService.AddToAppAudit((int)ToolIds.Images, "Error: DeletePlantImages", ex.Message);
                return StatusCode(500, "A problem occurred while handling your request");
            }
        }


        private Boolean IdsDoNotMatch(int userId)
        {
            var tokenUserId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            return (tokenUserId == null || (tokenUserId != null && userId != Int32.Parse(tokenUserId)));
        }

    }
}

