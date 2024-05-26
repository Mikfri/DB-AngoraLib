using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using DB_AngoraLib.Services.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DB_AngoraLib.Services.HelperService;

namespace DB_AngoraLib.Services.AccountService
{
    public class AccountServices : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public AccountServices(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newUserDto"></param>
        /// <returns></returns>
        public async Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDTO)
        {
            var newUser = new User();
            newUserDTO.CopyPropertiesTo(newUser);
            newUser.UserName = newUserDTO.Email;

            var result = await _userManager.CreateAsync(newUser, newUserDTO.Password);
            // returnere IdentityResult OG UserName
            var responseDTO = new Register_ResponseDTO // <IdentityResult> alternativ!
            {
                UserName = newUser.UserName,
                IsSuccessful = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Guest"); // hardcoded role
            }

            return responseDTO;
        }


        public async Task ConfirmEmail_SendEmailToUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://yourwebsite.com/confirmemail?userid={user.Id}&token={HttpUtility.UrlEncode(token)}";
            await _emailService.SendEmailAsync(email, "Email Confirmation", $"Please confirm your email by clicking on this link: {confirmationLink}");
        }

        public async Task ConfirmEmail_ConfirmAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new Exception("Email confirmation failed");
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(User_ChangePasswordDTO userPwConfig)
        {
            var user = await _userManager.FindByIdAsync(userPwConfig.UserId);
            if (user == null)
            {
                // Handle user not found error
            }

            return await _userManager.ChangePasswordAsync(user, userPwConfig.CurrentPassword, userPwConfig.NewPassword);
        }

        public async Task ResetPassword_SendResetTokenToUserEmailAsync(string email)
        {
            var foundUser = await _userManager.FindByEmailAsync(email);
            if (foundUser == null)
            {
                throw new Exception("User not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(foundUser);
            var resetLink = $"https://yourwebsite.com/resetpassword?userid={foundUser.Id}&token={HttpUtility.UrlEncode(token)}";
            await _emailService.SendEmailAsync(email, "Password Reset", $"You can reset your password by clicking on this link: {resetLink}");
        }

        /// <summary>
        /// Formular til brugeren på frontend, hvor brugeren indtaster ny password.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Password reset failed");
            }
        }

    }
}
