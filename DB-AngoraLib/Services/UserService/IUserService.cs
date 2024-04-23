using DB_AngoraLib.Models;

namespace DB_AngoraLib.Services.UserService
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByBreederRegNoAsync(string breederRegNo);
        List<Rabbit> GetCurrentUsersRabbitCollection_ByProperties(User currentUser, string rightEarId, string leftEarId, string nickName, Race race, Color color, Gender gender, IsPublic isPublic, bool? isJuvenile, DateOnly? dateOfBirth, DateOnly? dateOfDeath);
        Task AddUserAsync(User newUser);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}