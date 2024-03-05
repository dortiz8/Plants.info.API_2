using Plants.info.API.Data.Models;
using Plants.info.API.Models;

namespace Plants.info.API.Data.Repository
{
    public interface IUserRepository : IDbActions
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int Id);

        Task<bool> UserExistsAsync(int Id);
        Task<bool> UserNameExistsAsync(string userName); 

        Task<bool> ValidateUserAsync(string userName, string password);

        Task<User?> FindUserByUsernameAsync(string userName);
        Task<User?> FindUserByEmailAsync(string email);


        Task CreateUserAsync(User userObject);

        Task DeleteUserAsync(int Id); 
        
    }
}
