using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.UserService
{
    public class UserService : IUserService
    {   
        private readonly IGRepository<User> _dbRepository;
        private readonly UserManager<User> _userManager;

        public UserService(IGRepository<User> dbRepository, UserManager<User> userManager)
        {
            _dbRepository = dbRepository;
            _userManager = userManager;
        }

        //------------------------- USER METHODS -------------------------

        public async Task CreateBasicUserAsync(Register_CreateBasicUserDTO newUserDto)
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

            await _userManager.CreateAsync(newUser, "DefaultPassword123!"); // Replace with actual password
            await _userManager.AddToRoleAsync(newUser, "Guest");
        }


        /// <summary>
        /// Finder den den pågældende brugers 'EF-Core Rabbit Collection' i DB og returnerer en liste af RabbitDTO'er
        /// </summary>
        /// <param name="userId">En GUID string</param>
        /// <returns></returns>
        public async Task<List<Rabbit_PreviewDTO>> GetMyRabbitCollection(string userId)
        {
            var currentUserCollection = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.Rabbits)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUserCollection?.Rabbits == null)
            {
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

        /// <summary>
        /// Finder den den pågældende brugers 'EF-Core Rabbit Collection', med mulighed for filtrering
        /// </summary>
        /// <param name="userId">GUID id</param>
        /// <param name="rightEarId"></param>
        /// <param name="leftEarId"></param>
        /// <param name="nickName"></param>
        /// <param name="race">enum Race</param>
        /// <param name="color">enum Color</param>
        /// <param name="gender">enum Gender</param>
        /// <returns></returns>
        public async Task<List<Rabbit_PreviewDTO>> GetMyRabbitCollection_Filtered(string userId, string rightEarId = null, string leftEarId = null, string nickName = null, Race? race = null, Color? color = null, Gender? gender = null)
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


        public async Task UpdateUserAsync(User user)
        {
            await _dbRepository.UpdateObjectAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _dbRepository.DeleteObjectAsync(user);
        }

        //-------: ADMIN/MOD METHODS ONLY
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

    }
}
