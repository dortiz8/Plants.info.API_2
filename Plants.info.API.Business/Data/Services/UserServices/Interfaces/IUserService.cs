using System;
using Plants.info.API.Data.Models.Authentication;
using Plants.info.API.Models;

namespace Plants.info.API.Data.Services.UserServices
{
	public interface IUserService
	{
        Task<bool> UserExistsAsync(int userId);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> SaveAllChangesAsync();
        Task<User?> FindUserByEmailAsync(string email);
        Task<User?> CreateNewUserAsync(GoogleAuthenticateRequestBody requestBody);
        Task<User?> CreateNewUserAsync(GoogleAuthenticateRequestBody requestBody, int role);
        Task UpdateUserTokens(User user, string token); 
    }
}

