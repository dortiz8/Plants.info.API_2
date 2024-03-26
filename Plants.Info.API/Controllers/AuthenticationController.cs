using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Plants.info.API.Business.Data.Services.AppAuditService.Interfaces;
using Plants.info.API.Common.Data.Utils;
using Plants.info.API.Data.Models;
using Plants.info.API.Data.Models.Authentication;
using Plants.info.API.Data.Repository;
using Plants.info.API.Data.Services.JwtFeatures;
using Plants.info.API.Data.Services.JwtFeatures.Interfaces;
using Plants.info.API.Data.Services.UserServices;
using Plants.info.API.Models;
using Serilog;
using static Plants.info.API.Controllers.AuthenticationController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Plants.info.API.Controllers
{
    [Route("api/authentication")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthenticationController> _log;
        private readonly IAppAuditService _appAuditService;

        public IJwtHandler _jwtHandler { get; }

        public AuthenticationController(IConfiguration configuration, IUserService userService, IJwtHandler jwtHandler,
            IJwtService jwtService, ILogger<AuthenticationController> logger, IAppAuditService appAuditService)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userService = userService;
            _jwtHandler = jwtHandler;
            _jwtService = jwtService;
            _log = logger;
            _appAuditService = appAuditService;
        }


        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticationRequestBody authenticationRequestBody)
        {
            // Step #1 Validate user credentials
            try
            {
                var user = new User(); // fix

                if (user == null) return NotFound(new AuthResponseBody { ErrorMessage = "No account found" });

                var psw1 = user.Password;
                var psw2 = authenticationRequestBody.Password;
                var valid = ValidateUserCreds(psw1, psw2);

                if (valid != 0) return Unauthorized(new AuthResponseBody { ErrorMessage = "Invalid credentials" });


                // Step #2 & #3 create a security key and signing credentials


                var token = _jwtHandler.GenerateAccessToken(user);

                var refreshToken = _jwtHandler.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExiryTime = DateTime.Now.AddDays(2);

                await _userService.SaveAllChangesAsync();

                //var user = await _userService.FindUserByUsernameAsync(authenticationRequestBody.UserName);

                // Return a response body with the token included
                return Ok(new AuthResponseBody { IsAuthSuccessful = true, Token = token, RefreshToken = refreshToken, UserId = user.Id });
            }
            catch (Exception ex)
            {
                await _appAuditService.AddToAppAudit((int)ToolIds.Auth, "Error: Authenticate", ex.Message);
                return StatusCode(500, "A problem occurred while handling your request");
            }
        }

        [HttpPost("googleAuthenticate")]
        public async Task<ActionResult<string>> GoogleAuthenticate([FromBody] GoogleAuthenticateRequestBody requestBody)
        {
            try
            {
                //var tokenid = HttpContext.Request.Form["credential"];
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(requestBody.IdToken);
                if (!payload.Audience.Equals(_config["GoogleAuthSettings:clientId"]))
                    return BadRequest();

                var user = await _userService.FindUserByEmailAsync(requestBody.Email);

               // if (user == null) return NotFound(new AuthResponseBody { ErrorMessage = "No account found" });

                if(user == null)
                {
                    user = await _userService.CreateNewUserAsync(requestBody); 
                }

                var (token, refreshToken) =  _jwtService.GetAuthenticationTokens(user);

                if(token != null && refreshToken != null)
                {
                    await _userService.UpdateUserTokens(user, refreshToken);


                    return Ok(new AuthResponseBody { IsAuthSuccessful = true, Token = token, RefreshToken = refreshToken, UserId = user.Id }); 
                }
                return StatusCode(500); 
            }
            catch (Exception ex)
            {
                await _appAuditService.AddToAppAudit((int)ToolIds.Auth, "Error: GoogleAuthenticate", ex.Message);
                return StatusCode(500, "A problem occurred while handling your request");
            }
        }

        private int ValidateUserCreds(string? psw1, string? psw2) => psw1 != null ? String.Compare(psw1, psw2) : -1;
       

    }
}

