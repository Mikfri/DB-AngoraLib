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
        public List<Rabbit> GetCurrentUsersRabbitCollection_ByProperties(User currentUser, string rightEarId, string leftEarId, string nickName, Race race, Color color, Gender gender, IsPublic isPublic, bool? isJuvenile, DateOnly? dateOfBirth, DateOnly? dateOfDeath)
        {
            return currentUser.Rabbits
                .Where(rabbit =>
                       rabbit.RightEarId == rightEarId
                    && rabbit.LeftEarId == leftEarId
                    && (nickName == null || rabbit.NickName == nickName)
                    && rabbit.Race == race
                    && rabbit.Color == color
                    && rabbit.Gender == gender
                    && rabbit.IsPublic == isPublic
                    && (isJuvenile == null || rabbit.IsJuvenile == isJuvenile)
                    && (dateOfBirth == null || rabbit.DateOfBirth == dateOfBirth)
                    && (dateOfDeath == null || rabbit.DateOfDeath == dateOfDeath))
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

        public async Task<User> GetUserByBreederRegNoAsync(string breederRegNo)
        {
            return await _dbRepository.GetObjectAsync(u => u.BreederRegNo == breederRegNo);
        }

        public async Task AddUserAsync(User newUser)
        {
            await _dbRepository.AddObjectAsync(newUser);
        }
    }
}
