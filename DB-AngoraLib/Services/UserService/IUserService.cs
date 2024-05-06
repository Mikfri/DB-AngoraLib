using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;

namespace DB_AngoraLib.Services.UserService
{
    public interface IUserService
    {
        //Task<User> Login(UserLoginDTO userLoginDto);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByBreederRegNoAsync(User_KeyDTO userKeyDto);
        Task<List<Rabbit_PreviewDTO>> GetCurrentUsersRabbitCollection_ByProperties(User_KeyDTO userKeyDto, string rightEarId = null, string leftEarId = null, string nickName = null, Race? race = null, Color? color = null, Gender? gender = null, IsPublic? isPublic = null, bool? isJuvenile = null, DateOnly? dateOfBirth = null, DateOnly? dateOfDeath = null);
        Task AddUserAsync(User newUser);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}