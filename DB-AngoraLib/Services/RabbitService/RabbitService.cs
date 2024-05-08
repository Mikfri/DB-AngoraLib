using DB_AngoraLib.DTOs;
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

        public RabbitService() { }

        //public void ValidateUniqueRabbitId(Rabbit rabbit)
        //{
        //    var existingRabbit = _dbRepository.GetObjectByIdAsync(Rabbit rabbit).Result;

        //    if (existingRabbit != null)
        //    {
        //        throw new InvalidOperationException($"A Rabbit with the id {rabbit.RightEarId}-{rabbit.LeftEarId} already exists.");
        //    }
        //}

        //---------------------: GET METODER
        //-------: ADMIN METODER

        public async Task<List<Rabbit>> GetAllRabbitsAsync()     // reeeally though? Skal vi bruge denne metode, udover test?
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<List<Rabbit>> GetAllRabbits_ByBreederRegAsync(string breederRegNo)
        {
            var user = await _userService.GetUserByBreederRegNoAsync(new User_BreederKeyDTO { BreederRegNo = breederRegNo });
            if (user == null)
            {
                return null;
            }
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.Where(rabbit => rabbit.OwnerId == user.Id).ToList();
        }



        //-------: GET BY EAR TAGs METODER
        public async Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId)
        {
            Expression<Func<Rabbit, bool>> filter = r => r.RightEarId == rightEarId && r.LeftEarId == leftEarId;

            Console.WriteLine($"Checking for existing Rabbit with ear tags: RightEarId: {rightEarId}, LeftEarId: {leftEarId}");

            var rabbit = await _dbRepository.GetObjectAsync(filter);

            if (rabbit != null)
            {
                Console.WriteLine($"A Rabbit with this ear-tag already exists!\nRabbitName: {rabbit.NickName}, OwnerId: {rabbit.OwnerId}, OwnerName: {rabbit.User.FirstName} {rabbit.User.LastName}");
            }
            else
            {
                Console.WriteLine("No rabbit found with the given ear tags.");
            }

            return rabbit;
        }



        //---------------------: ADD
        //public async Task AddRabbit_ToCurrentUserAsync(User currentUser, Rabbit newRabbit)
        //{
        //    Console.WriteLine($"Trying to add rabbit with RightEarId: {newRabbit.RightEarId}, LeftEarId: {newRabbit.LeftEarId}");
        //    _validatorService.ValidateRabbit(newRabbit);

        //    // Check om kaninen med de givne øremærker allerede eksisterer
        //    var existingRabbit = await GetRabbitByEarTagsAsync(newRabbit.RightEarId, newRabbit.LeftEarId);
        //    if (existingRabbit != null)
        //    {
        //        throw new InvalidOperationException("A rabbit with the same ear tags already exists.");
        //    }

        //    newRabbit.OwnerId = currentUser.BreederRegNo;
        //    await _dbRepository.AddObjectAsync(newRabbit);
        //    Console.WriteLine($"Rabbit added successfully with RightEarId: {newRabbit.RightEarId}, LeftEarId: {newRabbit.LeftEarId}, OwnerId: {newRabbit.OwnerId}");
        //}

        public async Task AddRabbit_ToCurrentUserAsync(string userId, RabbitDTO newRabbitDto)
        {
            Console.WriteLine($"Trying to add rabbit with RightEarId: {newRabbitDto.RightEarId}, LeftEarId: {newRabbitDto.LeftEarId}");

            // Convert RabbitDto to Rabbit
            var newRabbit = new Rabbit
            {
                RightEarId = newRabbitDto.RightEarId,
                LeftEarId = newRabbitDto.LeftEarId,
                NickName = newRabbitDto.NickName,
                Race = newRabbitDto.Race,
                Color = newRabbitDto.Color,
                DateOfBirth = newRabbitDto.DateOfBirth,
                DateOfDeath = newRabbitDto.DateOfDeath,
                Gender = newRabbitDto.Gender,
                IsPublic = newRabbitDto.IsPublic
                // ... evt flere..
            };

            _validatorService.ValidateRabbit(newRabbit);

            // Check om kaninen med de givne øremærker allerede eksisterer
            var existingRabbit = await GetRabbitByEarTagsAsync(newRabbit.RightEarId, newRabbit.LeftEarId);
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("A rabbit with the same ear tags already exists.");
            }

            // Find the corresponding User object
            var currentUser = await _userService.GetUserByIdAsync(userId);

            // Check if the currentUser is null
            if (currentUser == null)
            {
                throw new InvalidOperationException("No user found with the given BreederRegNo.");
            }

            newRabbit.OwnerId = currentUser.Id;
            await _dbRepository.AddObjectAsync(newRabbit);
            Console.WriteLine($"Rabbit added successfully with RightEarId: {newRabbit.RightEarId}, LeftEarId: {newRabbit.LeftEarId}, OwnerId: {newRabbit.OwnerId}");
        }



        //---------------------: UPDATE
        public async Task UpdateRabbitAsync(User currentUser, Rabbit rabbit)
        {
            if (rabbit.OwnerId != currentUser.Id)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }

            _validatorService.ValidateRabbit(rabbit);
            await _dbRepository.UpdateObjectAsync(rabbit);
        }

        public async Task RequestRabbitTransfer(string rightEarId, string leftEarId, string newBreederRegNo)
        {
            // Check if the new owner exists in the database
            var newOwner = await _userService.GetUserByBreederRegNoAsync(new User_BreederKeyDTO { BreederRegNo = newBreederRegNo });
            if (newOwner == null)
            {
                throw new Exception("New owner not found");
            }

            // Find the rabbit to transfer
            var rabbit = await GetRabbitByEarTagsAsync(rightEarId, leftEarId);
            if (rabbit == null)
            {
                throw new Exception("Rabbit not found");
            }

            // Create a new transfer request
            var transferRequest = new RabbitTransferRequest
            {
                RabbitRightEarId = rightEarId,
                RabbitLeftEarId = leftEarId,
                CurrentOwnerId = rabbit.OwnerId,
                NewOwnerId = newBreederRegNo,
                IsAccepted = false
            };

            // Save the transfer request to the database
            //await _transferRequestRepository.AddObjectAsync(transferRequest);
        }

        //---------------------: DELETE
        public async Task DeleteRabbitAsync(User currentUser, Rabbit rabbitToDelete)
        {
            if (rabbitToDelete.OwnerId != currentUser.Id)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }

            await _dbRepository.DeleteObjectAsync(rabbitToDelete);
        }


    }
}
