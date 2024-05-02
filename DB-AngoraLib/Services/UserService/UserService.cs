using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.UserService
{
    public class UserService : IUserService
    {
        //private readonly List<Rabbit> _rabbits = new List<Rabbit>();
        //public IReadOnlyList<Rabbit> Rabbits => _rabbits.AsReadOnly();

        private readonly IGRepository<User> _dbRepository;

        public UserService(IGRepository<User> dbRepository)
        {
            _dbRepository = dbRepository;
        }

        //------------------------- USER METHODS -------------------------

        /// <summary>
        /// Denne metode behøver IKKE at være asynkron.
        /// Benytter <User> classen ICollection<Rabbit>.
        /// Den opererer på data, der allerede er indlæst i hukommelsen 
        /// (dvs., currentUser.Rabbits), så der er ingen asynkrone operationer,
        /// der skal vente på. Derfor kan metoden returnere resultatet
        /// direkte som en List<Rabbit> i stedet for en Task<List<Rabbit>>.
        /// </summary>
        //public async Task<List<Rabbit>> GetCurrentUsersRabbitCollection_ByProperties(User_KeyDTO userKeyDto, string rightEarId = null, string leftEarId = null, string nickName = null, Race? race = null, Color? color = null, Gender? gender = null, IsPublic? isPublic = null, bool? isJuvenile = null, DateOnly? dateOfBirth = null, DateOnly? dateOfDeath = null)
        //{
        //    var currentUser = await _dbRepository.GetObjectAsync(u => u.BreederRegNo == userKeyDto.BreederRegNo);

        //    return currentUser.Rabbits
        //        .Where(rabbit =>
        //               (rightEarId == null || rabbit.RightEarId == rightEarId)
        //            && (leftEarId == null || rabbit.LeftEarId == leftEarId)
        //            && (nickName == null || rabbit.NickName == nickName)
        //            && (race == null || rabbit.Race == race)
        //            && (color == null || rabbit.Color == color)
        //            && (gender == null || rabbit.Gender == gender)
        //            && (isPublic == null || rabbit.IsPublic == isPublic)
        //            && (isJuvenile == null || rabbit.IsJuvenile == isJuvenile)
        //            && (dateOfBirth == null || rabbit.DateOfBirth == dateOfBirth)
        //            && (dateOfDeath == null || rabbit.DateOfDeath == dateOfDeath))
        //        .ToList();
        //}

        public async Task<List<Rabbit_PreviewDTO>> GetCurrentUsersRabbitCollection_ByProperties(User_KeyDTO userKeyDto, string rightEarId = null, string leftEarId = null, string nickName = null, Race? race = null, Color? color = null, Gender? gender = null, IsPublic? isPublic = null, bool? isJuvenile = null, DateOnly? dateOfBirth = null, DateOnly? dateOfDeath = null)
        {
            var currentUser = await _dbRepository.GetObjectAsync(u => u.Id == userKeyDto.BreederRegNo);

            return currentUser.Rabbits
                .Where(rabbit =>
                       (rightEarId == null || rabbit.RightEarId == rightEarId)
                    && (leftEarId == null || rabbit.LeftEarId == leftEarId)
                    && (nickName == null || rabbit.NickName == nickName)
                    && (race == null || rabbit.Race == race)
                    && (color == null || rabbit.Color == color)
                    && (gender == null || rabbit.Gender == gender)
                    && (isPublic == null || rabbit.IsPublic == isPublic)
                    && (isJuvenile == null || rabbit.IsJuvenile == isJuvenile)
                    && (dateOfBirth == null || rabbit.DateOfBirth == dateOfBirth)
                    && (dateOfDeath == null || rabbit.DateOfDeath == dateOfDeath))
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

        public async Task UpdateUserAsync(User user)
        {
            await _dbRepository.UpdateObjectAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _dbRepository.DeleteObjectAsync(user);
        }

        //-------: ADMIN METHODS ONLY
        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _dbRepository.GetAllObjectsAsync()).ToList();
        }

        public async Task<User> GetUserByBreederRegNoAsync(User_KeyDTO userKeyDto)
        {
            return await _dbRepository.GetObjectAsync(u => u.Id == userKeyDto.BreederRegNo);
        }

        public async Task AddUserAsync(User newUser)
        {
            await _dbRepository.AddObjectAsync(newUser);
        }
    }
}
