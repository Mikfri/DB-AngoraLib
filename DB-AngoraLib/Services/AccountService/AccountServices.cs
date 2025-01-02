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
using DB_AngoraLib.Repository;
using Microsoft.EntityFrameworkCore;
using DB_AngoraLib.Events;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.Extensions.Caching.Memory;

namespace DB_AngoraLib.Services.AccountService
{
    public class AccountServices : IAccountService
    {
        private readonly IGRepository<User> _userRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public AccountServices(
            IGRepository<User> userRepository,
            IGRepository<Breeder> breederRepository,
            IEmailService emailService,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _userManager = userManager;
        }

        //---------------------------------: CREATE/REGISTER USER :---------------------------------
        public async Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDTO)
        {
            var newUser = new User();
            HelperServices.CopyProperties_FromAndTo(newUserDTO, newUser);
            newUser.UserName = newUserDTO.Email;

            var result = await _userManager.CreateAsync(newUser, newUserDTO.Password);
            var responseDTO = new Register_ResponseDTO
            {
                UserName = newUser.UserName,
                IsSuccessful = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "UserBasicFree");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                await Send_EmailConfirm_ToUser(newUser.Id, token);
            }

            return responseDTO;
        }

        //---------------------------------: GET USER METHODS :---------------------------------
        public async Task<List<User>> GetAll_Users()
        {
            return (await _userRepository.GetAllObjectsAsync()).ToList();
        }

        public async Task<User?> Get_UserByUserNameOrEmail(string userNameOrEmail)
        {
            return await _userRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
        }

        public async Task<User?> Get_UserById(string userId)
        {
            // Hent brugeren fra userRepository
            var user = await _userRepository.GetObject_ByStringKEYAsync(userId);

            // Hvis brugeren er en Breeder, returner den som en Breeder
            if (user is Breeder breeder)  // test, om dette er nødvendigt
            {
                return breeder;
            }

            // Returner brugeren (kan være en almindelig User eller null)
            return user;
        }

        public async Task<User_ProfileDTO> Get_User_Profile(string userId, string userProfileId, IList<Claim> userClaims)
        {
            var userProfile = await Get_UserById(userProfileId);
            if (userProfile == null)
            {
                return null;
            }

            var hasPermissionToGetAnyUser = userClaims.Any(
                c => c.Type == "User:Read" && c.Value == "Any");

            if (userId != userProfileId && !hasPermissionToGetAnyUser)
            {
                throw new UnauthorizedAccessException("You do not have permission to access this profile.");
            }

            var userProfileDTO = new User_ProfileDTO
            {
                BreederRegNo = (userProfile is Breeder breeder) ? breeder.BreederRegNo : null, // Set BreederRegNo if user is a Breeder
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                RoadNameAndNo = userProfile.RoadNameAndNo,
                ZipCode = userProfile.ZipCode,
                City = userProfile.City,
                Email = userProfile.Email,
                Phone = userProfile.PhoneNumber
            };

            return userProfileDTO;
        }
               


        //---------------------------------: EMAIL METHODS :-------------------------------
        public async Task Send_EmailConfirm_ToUser(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found");

            var confirmationLink = $"https://DB-Angora.dk/email-confirmation?userId={HttpUtility.UrlEncode(userId)}&token={HttpUtility.UrlEncode(token)}";

            var emailSubject = "DB-Angora: Velkommen til";
            var emailBody = $"Bekræft venligst din e-mail ved at klikke på følgende link, for at fuldende din profil: <a href=\"{confirmationLink}\">Bekræft e-mail</a>";

            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }

        public async Task Send_PWResetRequest(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://DB-Angora.dk/password-reset?userId={HttpUtility.UrlEncode(user.Id)}&token={HttpUtility.UrlEncode(resetToken)}";

            var emailSubject = "DB-Angora: Nulstil din adgangskode";
            var emailBody = $"For at nulstille din adgangskode, venligst klik på følgende link: <a href=\"{resetLink}\">Nulstil adgangskode</a>";

            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }

        public async Task Formular_ResetPW(string userId, string token, string newPassword)
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

        //---------------------------------: UPDATE ACCOUNT SETTINGS :-----------------------------------
        public async Task<IdentityResult> User_ChangePassword(User_ChangePasswordDTO userPwConfig)
        {
            var user = await _userManager.FindByIdAsync(userPwConfig.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            return await _userManager.ChangePasswordAsync(user, userPwConfig.CurrentPassword, userPwConfig.NewPassword);
        }
    }
}