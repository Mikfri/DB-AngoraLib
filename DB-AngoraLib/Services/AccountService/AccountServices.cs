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

namespace DB_AngoraLib.Services.AccountService
{
    public class AccountServices
    {
        private readonly UserManager<User> _userManager;
        private readonly EmailServices _emailService;

        public AccountServices(UserManager<User> userManager, EmailServices emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IdentityResult> Register_BasicUserAsync(User_CreateBasicDTO newUserDto, string password)
        {
            var newUser = new User
            {
                UserName = newUserDto.Email, // Assuming the username is the email
                Email = newUserDto.Email,
                PhoneNumber = newUserDto.Phone,
                FirstName = newUserDto.FirstName,
                LastName = newUserDto.LastName,
                RoadNameAndNo = newUserDto.RoadNameAndNo,
                City = newUserDto.City,
                ZipCode = newUserDto.ZipCode,
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Guest");

                // Add claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{newUser.FirstName} {newUser.LastName}"),
                    new Claim(ClaimTypes.Email, newUser.Email),
                    //new Claim(ClaimTypes.Role, newUser.Role),
                    // Add other claims as needed
                };

                await _userManager.AddClaimsAsync(newUser, claims);
            }

            return result;
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
        public async Task GenerateAndSaveEmailConfirmationToken(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);
        }

        /// <summary> //TODO: Vil alle feldter udfyldes i UserToken tabellen?
        /// Benytter UserManager til at generere et password reset token og sender det til brugerens email.
        /// Generere data i -UserTokens tabellen.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task ResetPassword_ResetTokenSendEmailAsync(string email)
        {
            var foundUser = await _userManager.FindByEmailAsync(email);
            if (foundUser == null)
            {
                // Handle user not found error
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(foundUser);
            await _emailService.SendEmailAsync(email, "Password Reset", $"Your password reset token is: {token}");
        }

        /// <summary>
        /// Denne metode vil genere data i -UserTokens tabellen.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task SendEmailConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Handle user not found error
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendEmailAsync(email, "Email Confirmation", $"Your email confirmation token is: {token}");
        }


        public async Task ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle user not found error
            }

            await _userManager.ConfirmEmailAsync(user, token);
        }


    }
}
