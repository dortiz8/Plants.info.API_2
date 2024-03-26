using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Plants.info.API.Data.Repository;
using Plants.info.API.Data.Services.JwtFeatures;

using Plants.info.API.Data.Contexts;
using Plants.info.API.Data.Models;
using Plants.info.API.Models;
using Serilog;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using static Plants.info.API.Controllers.AuthenticationController;
using Plants.info.API.Data.Services.UserServices;
using Plants.info.API.Data.Models.Authentication;
using Plants.info.API.Business.Data.Services.AppAuditService.Interfaces;
using Plants.info.API.Common.Data.Utils;

namespace Plants.info.API.Controllers
{
	[Route("api/token")]
	[ApiController]
	public class TokenController: Controller
	{
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IAppAuditService _appAuditService;

        public IJwtHandler _jwtHandler { get; }

        public TokenController(IConfiguration configuration, IUserService userService, IJwtHandler jwtHandler, IAppAuditService appAuditService)
        {
            _config = configuration;
            _userService = userService;
            _jwtHandler = jwtHandler;
            _appAuditService = appAuditService;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<string>> Refresh([FromBody] RefreshTokenRequestBody refreshTokenRequestBody)
		{
			try
			{

				var oldAccessToken = refreshTokenRequestBody.Token;
				var refreshToken = refreshTokenRequestBody.RefreshToken;

				var principal = _jwtHandler.GetPrincipalFromExpiredToken(oldAccessToken);

				var userId = principal.Identity.Name;


				if (userId == null) return BadRequest("Invalid client Id");

				var user = await _userService.GetUserByIdAsync(Int32.Parse(userId));

				if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExiryTime <= DateTime.Now)
				{
					return BadRequest("Invalid client request"); 
				}

				var newAccessToken = _jwtHandler.GenerateAccessToken(user);
				var newRefreshToken = _jwtHandler.GenerateRefreshToken();

				user.RefreshToken = newRefreshToken;
				await _userService.SaveAllChangesAsync();

				return Ok(new AuthResponseBody { Token = newAccessToken, RefreshToken = newRefreshToken, IsAuthSuccessful = true, UserId = user.Id }); 
			}
			catch (Exception ex)
			{
                await _appAuditService.AddToAppAudit((int)ToolIds.Auth, "Error: TokenRefresh", ex.Message);
                return StatusCode(500, "A problem occurred while handling your request");
            }
        }

		[HttpPost, Authorize]
		[Route("revoke")]
		public async Task<ActionResult> Revoke()
		{
			try
			{
				var userId = User.Identity.Name;

				if (userId == null) return BadRequest("Invalid client Id");

				var user = await _userService.GetUserByIdAsync(Int32.Parse(userId));

				if (user == null) return BadRequest();

				user.RefreshToken = null;
				await _userService.SaveAllChangesAsync();
			
				return NoContent(); 
			}
			catch (Exception ex)
			{
                await _appAuditService.AddToAppAudit((int)ToolIds.Auth, "Error: TokenRevoke", ex.Message);
                return StatusCode(500, "A problem occurred while handling your request");
            }
		}
	}
}

