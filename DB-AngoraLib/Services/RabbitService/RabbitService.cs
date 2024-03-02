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

        //-------: GET BY EAR TAGs METODER
        public async Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId)
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
        //public async Task AddRabbitAsync(Rabbit rabbit, User thisUser)
        //{
        //    _validatorService.ValidateRabbit(rabbit);
        //    rabbit.Owner = thisUser.BreederRegNo;
        //    await _dbRepository.AddObjectAsync(rabbit);
        //}

        public async Task AddRabbitAsync(Rabbit newRabbit, User thisUser)
        {
            _validatorService.ValidateRabbit(newRabbit);

            // Check om kaninen med de givne øremærker allerede eksisterer
            var existingRabbit = await GetRabbitByEarTagsAsync(newRabbit.RightEarId, newRabbit.LeftEarId);
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("A rabbit with the same ear tags already exists.");
            }

            newRabbit.Owner = thisUser.BreederRegNo;
            await _dbRepository.AddObjectAsync(newRabbit);
        }

        //---------------------: UPDATE
        public async Task UpdateRabbitAsync(Rabbit rabbitId, User thisUser)
        {
            if (rabbitId.Owner != thisUser.BreederRegNo)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }
            _validatorService.ValidateRabbit(rabbitId);
            await _dbRepository.UpdateObjectAsync(rabbitId);
        }

        //---------------------: DELETE
        public async Task DeleteRabbitAsync(Rabbit rabbitToDelete, User thisUser)
        {
            if (rabbitToDelete.Owner != thisUser.BreederRegNo)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }

            await _dbRepository.DeleteObjectAsync(rabbitToDelete);
        }


    }
}
