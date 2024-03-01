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

        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _dbRepository.GetAllObjectsAsync()).ToList();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbRepository.GetObjectByIdAsync(userId);
        }

        public async Task<User> GetUserByUsernameAsync(string breederRegNo)
        {
            return await _dbRepository.GetObjectAsync(u => u.BreederRegNo == breederRegNo);
        }

        public async Task AddUserAsync(User newUser)
        {
            await _dbRepository.AddObjectAsync(newUser);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _dbRepository.UpdateObjectAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _dbRepository.DeleteObjectAsync(user);
        }

    }
}
