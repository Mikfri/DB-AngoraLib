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

        public async Task<IdentityResult> RegisterBasicUserAsync(User_CreateBasicDTO newUserDto, string password)
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

        public async Task ResetPassword_SendEmailResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Handle user not found error
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendEmailAsync(email, "Password Reset", $"Your password reset token is: {token}");
        }


    }
}
