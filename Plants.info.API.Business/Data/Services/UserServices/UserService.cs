using System;
using Plants.info.API.Data.Models;
using Plants.info.API.Data.Models.Authentication;
using Plants.info.API.Data.Repository;
using Plants.info.API.Models;

namespace Plants.info.API.Data.Services.UserServices
{
	public class UserService : IUserService
	{
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
		{
            _userRepository = userRepository;
        }

        public async Task<User?> CreateNewUserAsync(GoogleAuthenticateRequestBody requestBody)
        {
            var newUser = new User()
            {
                Email = requestBody?.Email,
                UserName = requestBody?.Email,
                FirstName = requestBody?.FirstName,
                LastName = requestBody?.LastName,
                CreatedDate = new DateTime(),
            };

            await _userRepository.CreateUserAsync(newUser);
            await _userRepository.SaveAllChangesAsync();

            return newUser; 
        }

        public Task<User?> CreateNewUserAsync(GoogleAuthenticateRequestBody requestBody, int role)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindUserByEmailAsync(string email)
        {
            return _userRepository.FindUserByEmailAsync(email); 
        }

        public Task<User?> GetUserByIdAsync(int userId)
        {
            return _userRepository.GetUserByIdAsync(userId); 
        }

        public Task<bool> SaveAllChangesAsync()
        {
            return _userRepository.SaveAllChangesAsync(); 
        }

        public async Task UpdateUserTokens(User user, string token)
        {
            user.RefreshToken = token;
            user.RefreshTokenExiryTime = DateTime.Now.AddDays(2);
            
             await SaveAllChangesAsync();
        }

        public Task<bool> UserExistsAsync(int userId)
        {
            return _userRepository.UserExistsAsync(userId); 
        }
    }
}

