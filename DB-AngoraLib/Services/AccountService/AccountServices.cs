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

namespace DB_AngoraLib.Services.AccountService
{
    public class AccountServices : IAccountService
    {
        private readonly IGRepository<User> _dbRepository;
        private readonly UserManager<User> _userManager;

        public AccountServices(IGRepository<User> dbRepository, UserManager<User> userManager)
        {
            _dbRepository = dbRepository;
            _userManager = userManager;
        }

        //---------------------------------: CREATE/REGISTER USER :---------------------------------
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

                //// Trigger UserRegisteredEvent
                //var userRegisteredEvent = new UserRegistered_Event { Email = newUser.UserName };
                //await _eventBus.Trigger(userRegisteredEvent);
            }

            return responseDTO;
        }

        //---------------------------------: GET USER METHODS :---------------------------------
        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _dbRepository.GetAllObjectsAsync()).ToList();
        }

        public async Task<User> GetUserByUserNameOrEmailAsync(string userNameOrEmail)
        {
            return await _dbRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _dbRepository.GetObjectByKEYAsync(userId);
        }

        public async Task<User> GetUserByBreederRegNoAsync(string breederRegNo)
        {
            return await _dbRepository.GetObjectAsync(u => u.BreederRegNo == breederRegNo);
        }


        //---------------------------------: GET USERs ICOLLECTION METHODS :--------------------       
        public async Task<List<Rabbit_PreviewDTO>> GetMyRabbitCollection(string userId)
        {
            var currentUserCollection = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.Rabbits)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUserCollection == null)
            {
                Console.WriteLine("User not found");
                return new List<Rabbit_PreviewDTO>();
            }

            if (currentUserCollection.Rabbits.Count < 1)
            {
                Console.WriteLine("No rabbits found in collection");
                return new List<Rabbit_PreviewDTO>();
            }

            return currentUserCollection.Rabbits
                .Select(rabbit => new Rabbit_PreviewDTO
                {
                    RightEarId = rabbit.RightEarId,
                    LeftEarId = rabbit.LeftEarId,
                    NickName = rabbit.NickName,
                    Race = rabbit.Race,
                    Color = rabbit.Color,
                    Gender = rabbit.Gender
                })
                .ToList();
        }


        public async Task<List<Rabbit_PreviewDTO>> GetMyRabbitCollection_Filtered(
            string userId, string rightEarId = null, string leftEarId = null, string nickName = null, Race? race = null, Color? color = null, Gender? gender = null, bool? isJuvenile = null, bool? approvedRaceColorCombination = null)
        {
            var rabbitCollection = await GetMyRabbitCollection(userId);

            return rabbitCollection
                .Where(rabbit =>
                       (rightEarId == null || rabbit.RightEarId == rightEarId)
                    && (leftEarId == null || rabbit.LeftEarId == leftEarId)
                    && (nickName == null || rabbit.NickName == nickName)
                    && (race == null || rabbit.Race == race)
                    && (color == null || rabbit.Color == color)
                    && (gender == null || rabbit.Gender == gender))
                .ToList();
        }






        //---------------------------------: EMAIL CONFIRMATION :-------------------------------
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


        //---------------------------------: PASSWORD RESET :-----------------------------------
        public async Task<IdentityResult> ChangePasswordAsync(User_ChangePasswordDTO userPwConfig)
        {
            var user = await _userManager.FindByIdAsync(userPwConfig.UserId);
            if (user == null)
            {
                // Handle user not found error
            }

            return await _userManager.ChangePasswordAsync(user, userPwConfig.CurrentPassword, userPwConfig.NewPassword);
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
