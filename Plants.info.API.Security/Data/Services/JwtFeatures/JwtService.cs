using System;
using Plants.info.API.Data.Models.Authentication;

using Plants.info.API.Data.Services.JwtFeatures.Interfaces;
using Plants.info.API.Models;

namespace Plants.info.API.Data.Services.JwtFeatures
{
	public class JwtService : IJwtService
	{
        public readonly IJwtHandler _jwtHandler; 

        public JwtService(IJwtHandler jwtHandler)
		{
            _jwtHandler = jwtHandler;
		}

        public Tuple<string, string> GetAuthenticationTokens(User user)
        {
            var token = _jwtHandler.GenerateAccessToken(user);

            var refreshToken = _jwtHandler.GenerateRefreshToken();

            return new Tuple<string, string>( token, refreshToken );
        }
    }
}

