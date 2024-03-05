using System;
using Plants.info.API.Models;

namespace Plants.info.API.Data.Services.JwtFeatures.Interfaces
{
	public interface IJwtService
	{
		Tuple<string, string> GetAuthenticationTokens(User user); 
	}
}

