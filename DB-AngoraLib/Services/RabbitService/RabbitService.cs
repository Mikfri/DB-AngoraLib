using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace DB_AngoraLib.Services.RabbitService
{
    //using IRabbitRepository = IGRepository<Rabbit>;
    //using RabbitList = List<Rabbit>;

    public class RabbitService : IRabbitService
    {
        private readonly IGRepository<Rabbit> _dbRepository;
        private readonly IUserService _userService;
        private readonly RabbitValidator _validatorService;


        public RabbitService(IGRepository<Rabbit> dbRepository, IUserService userService, RabbitValidator validatorService)
        {
            _dbRepository = dbRepository;
            _userService = userService;
            _validatorService = validatorService;
        }

        //public void ValidateUniqueRabbitId(Rabbit rabbit)
        //{
        //    var existingRabbit = _dbRepository.GetObjectByIdAsync(Rabbit rabbit).Result;

        //    if (existingRabbit != null)
        //    {
        //        throw new InvalidOperationException($"A Rabbit with the id {rabbit.RightEarId}-{rabbit.LeftEarId} already exists.");
        //    }
        //}

        //---------------------: GET METODER
        public async Task<List<Rabbit>> GetAllRabbitsAsync()
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<Rabbit> GetRabbitByIdAsync(int rabbitId)
        {
            return await _dbRepository.GetObjectByIdAsync(rabbitId);
        }

        public async Task<Rabbit> GetRabbitByEarIdAsync(string rightEarId, string leftEarId)
        {
            Expression<Func<Rabbit, bool>> filter = r => r.RightEarId == rightEarId && r.LeftEarId == leftEarId;
            return await _dbRepository.GetObjectAsync(filter);
        }


        //-------: GET BY OWNER METODER
        public async Task<List<Rabbit>> GetAllRabbitsByOwnerAsync(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.Where(rabbit => rabbit.Owner == user.BreederRegNo).ToList();
        }


        //---------------------: ADD
        //public async Task AddRabbitAsync(Rabbit rabbit, User user)
        //{
        //    _validatorService.ValidateRabbit(rabbit);
        //    rabbit.Owner = user.BreederRegNo;
        //    await _dbRepository.AddObjectAsync(rabbit);
        //}

        public async Task AddRabbitAsync(Rabbit newRabbit, User user)
        {
            _validatorService.ValidateRabbit(newRabbit);

            // Check om kaninen med det givne id allerede eksisterer
            var existingRabbit = await GetRabbitByIdAsync(newRabbit.Id);
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("A rabbit with the same id already exists.");
            }

            newRabbit.Owner = user.BreederRegNo;
            await _dbRepository.AddObjectAsync(newRabbit);
        }

        //---------------------: UPDATE
        public async Task UpdateRabbitAsync(Rabbit rabbitId, User userId)
        {
            if (rabbitId.Owner != userId.BreederRegNo)
            {

            }
            _validatorService.ValidateRabbit(rabbitId);
            await _dbRepository.UpdateObjectAsync(rabbitId);
        }

        //---------------------: DELETE
        public async Task DeleteRabbitAsync(int rabbitId, User userId)
        {
            var rabbitToDelete = await GetRabbitByIdAsync(rabbitId);
            {
                await _dbRepository.DeleteObjectAsync(rabbitToDelete);
            }
        }

    }
}
