using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;

namespace DB_AngoraLib.Services.AccountService
{
    public interface IAccountService
    {
        Task<Register_ResponseDTO> Register_BasicUserAsync(User_CreateBasicDTO newUserDto);
        Task ConfirmEmail_SendEmailToUserAsync(string email);
        Task ConfirmEmail_ConfirmAsync(string userId, string token);
        Task<IdentityResult> ChangePasswordAsync(User_ChangePasswordDTO userPwConfig);
        Task ResetPassword_SendResetTokenToUserEmailAsync(string email);
        Task ResetPasswordAsync(string userId, string token, string newPassword);


        //Task<IdentityResult> ChangeEmailAsync(User_ChangeEmailDTO userMailConfig); //TODO: Denne metode skal implementeres
    }
}