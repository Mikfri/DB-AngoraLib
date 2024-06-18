using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.AccountService
{
    public interface IAccountService
    {
        Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDto);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByUserNameOrEmailAsync(string userNameOrEmail);
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByBreederRegNoAsync(string breederRegNo);
        Task<List<Rabbit_PreviewDTO>> GetMyRabbitCollection(string userId);
        Task<List<Rabbit_PreviewDTO>> GetMyRabbitCollection_Filtered(
            string userId, string rightEarId = null, string leftEarId = null, string nickName = null, Race? race = null, Color? color = null, Gender? gender = null, bool? isJuvenile = null, bool? approvedRaceColorCombination = null);
        Task Send_EmailConfirmAsync(string userId, string token);
        Task Formular_ResetPWAsync(string userId, string token, string newPassword);
        Task<IdentityResult> User_ChangePasswordAsync(User_ChangePasswordDTO userPwConfig);
      
    }
}
