using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

//using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Plants.info.API.Data.Models;
using Plants.info.API.Models;


namespace Plants.info.API.Data.Services.JwtFeatures
{
	public class JwtHandler: IJwtHandler
    {
        #region Service fields 
        private readonly IConfiguration _config;
        private readonly IConfigurationSection _googleSettings;
        #endregion

        public JwtHandler(IConfiguration config)
		{
            _config = config;
            _googleSettings = _config.GetSection("GoogleAuthSettings"); 
        }

        #region Public methods 
        // Refresh token generator
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParams = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:SecretForKey"])),
                ValidIssuer = _config["Authentication:Issuer"],
                ValidAudience = _config["Authentication:Audience"],
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token"); 
            }

            return principal; 
        }


        public string GenerateAccessToken(PlantInfoUser user)
        {
            var signingCredentials = this.GetSigningCredentials();
            var claimsForToken = this.GetClaims(user);
            
            var tokenDescription = this.GenerateTokenDescriptor(signingCredentials, claimsForToken);
            var tokenHandler = new JwtSecurityTokenHandler(); 
            var token = tokenHandler.CreateToken(tokenDescription);
            var jwtToken = tokenHandler.WriteToken(token); 

            return jwtToken;
        }
        #endregion

        #region Private methods 
        private SigningCredentials GetSigningCredentials()
        {
            var pathToSecurityKey = _config["Authentication:SecretForKey"];
            var key = Encoding.UTF8.GetBytes(pathToSecurityKey);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(PlantInfoUser user)
        {
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim(ClaimTypes.Name, user.Id.ToString())); 
            claimsForToken.Add(new Claim("access_role", user.UserRole.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("access_role", user.UserRole.ToString()));
            claimsForToken.Add(new Claim("userId", user.Id.ToString()));
            return claimsForToken;
        }


        private SecurityTokenDescriptor GenerateTokenDescriptor(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var date = DateTime.UtcNow; 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = date.AddMinutes(Convert.ToDouble(_config["Authentication:expireInMinutes"])),
                NotBefore = date,
                Issuer = _config["Authentication:Issuer"],
                Audience = _config["Authentication:Audience"],
                SigningCredentials = signingCredentials
            };
            return tokenDescriptor; 
        }
        #endregion
    }

}

