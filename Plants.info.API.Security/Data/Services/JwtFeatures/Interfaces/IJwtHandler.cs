using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Plants.info.API.Models;


namespace Plants.info.API.Data.Services.JwtFeatures
{
	public interface IJwtHandler
	{
		string GenerateAccessToken(PlantInfoUser user);
		string GenerateRefreshToken();
		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}

