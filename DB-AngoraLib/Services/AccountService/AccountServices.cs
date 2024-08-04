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
        private readonly IGRepository<Breeder> _breederRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public AccountServices(
            IGRepository<User> userRepository,
            IGRepository<Breeder> breederRepository,
            IEmailService emailService,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _breederRepository = breederRepository;
            _emailService = emailService;
            _userManager = userManager;
        }

        //---------------------------------: CREATE/REGISTER USER :---------------------------------
        public async Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDTO)
        {
            var newUser = new User();
            newUserDTO.CopyPropertiesTo(newUser);
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
                await _userManager.AddToRoleAsync(newUser, "Guest");

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
            var user = await Get_UserById(userId);
            if (user == null)
            {
                return null;
            }

            var hasPermissionToGetAnyUser = userClaims.Any(
                c => c.Type == "User:Read" && c.Value == "Any");

            if (user.Id != userProfileId && !hasPermissionToGetAnyUser)
            {
                throw new UnauthorizedAccessException("You do not have permission to access this profile.");
            }

            var userProfile = new User_ProfileDTO
            {
                BreederRegNo = (user is Breeder breeder) ? breeder.BreederRegNo : null, // Set BreederRegNo if user is a Breeder
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoadNameAndNo = user.RoadNameAndNo,
                ZipCode = user.ZipCode,
                City = user.City,
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            return userProfile;
        }

        //---------------------------------: GET BREEDER METHODS :---------------------------------
        public async Task<List<Breeder>> GetAll_Breeders()
        {
            return (await _breederRepository.GetAllObjectsAsync()).ToList();
        }

        public async Task<Breeder> Get_BreederById(string userId)
        {
            return await _breederRepository.GetObject_ByStringKEYAsync(userId);
        }

        public async Task<Breeder?> Get_BreederByBreederRegNo(string breederRegNo)
        {
            return await _breederRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.BreederRegNo == breederRegNo);
        }

        public async Task<Breeder?> Get_BreederByBreederRegNo_IncludingCollections(string breederRegNo)
        {
            return await _breederRepository.GetDbSet()
                .Include(b => b.RabbitTransfers_Issued)
                    .ThenInclude(rt => rt.Status == TransferStatus.Pending)
                .Include(b => b.RabbitTransfers_Received)
                    .ThenInclude(rt => rt.Status == TransferStatus.Pending)
                .FirstOrDefaultAsync(b => b.BreederRegNo == breederRegNo);
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