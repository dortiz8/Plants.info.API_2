using System;
using Plants.info.API.Data.Models.Authentication;
using Plants.info.API.Models;

namespace Plants.info.API.Data.Services.UserServices
{
	public interface IUserService
	{
        Task<bool> UserExistsAsync(int userId);
        Task<PlantInfoUser?> GetUserByIdAsync(int userId);
        Task<bool> SaveAllChangesAsync();
        Task<PlantInfoUser?> FindUserByEmailAsync(string email);
        Task<PlantInfoUser?> CreateNewUserAsync(GoogleAuthenticateRequestBody requestBody);
        Task<PlantInfoUser?> CreateNewUserAsync(GoogleAuthenticateRequestBody requestBody, int role);
        Task UpdateUserTokens(PlantInfoUser user, string token); 
    }
}

