using DB_AngoraLib.Models;

namespace DB_AngoraLib.Services.UserService
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByUsernameAsync(string breederRegNo);
        Task AddUserAsync(User newUser);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}