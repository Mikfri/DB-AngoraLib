using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;


namespace DB_AngoraLib.Services.RabbitService
{
    //using IRabbitRepository = IGRepository<Rabbit>;
    //using RabbitList = List<Rabbit>;

    public class RabbitServices : IRabbitService
    {
        private readonly IGRepository<Rabbit> _dbRepository;
        private readonly IUserService _userService;
        private readonly RabbitValidator _validatorService;

        public RabbitServices(IGRepository<Rabbit> dbRepository, IUserService userService, RabbitValidator validatorService)
        {
            _dbRepository = dbRepository;
            _userService = userService;
            _validatorService = validatorService;
        }

        public RabbitServices() { }
                

        //---------------------: GET METODER
        //-------: ADMIN METODER

        public async Task<List<Rabbit>> GetAllRabbitsAsync()     // reeeally though? Skal vi bruge denne metode, udover test?
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<List<Rabbit>> GetAllRabbits_ByBreederRegAsync(string breederRegNo)
        {
            var user = await _userService.GetUserByBreederRegNoAsync(breederRegNo);
            if (user == null)
            {
                return null;
            }
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.Where(rabbit => rabbit.OwnerId == user.Id).ToList();
        }


        //-------: GET BY EAR TAGs METODER
        /// <summary>
        /// Finder en Rabbit ud fra dens øremærkerne kombinationen
        /// </summary>
        /// <param name="rightEarId"></param>
        /// <param name="leftEarId"></param>
        /// <returns></returns>
        public async Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId)
        {
            Expression<Func<Rabbit, bool>> filter = r => r.RightEarId == rightEarId && r.LeftEarId == leftEarId;

            //Console.WriteLine($"Checking for existing Rabbit with ear tags: RightEarId: {rightEarId}, LeftEarId: {leftEarId}");

            var rabbit = await _dbRepository.GetObjectAsync(filter);

            //if (rabbit != null)
            //{
            //    Console.WriteLine($"A Rabbit with this ear-tag already exists!\nRabbitName: {rabbit.NickName}, OwnerId: {rabbit.OwnerId}, OwnerName: {rabbit.User.FirstName} {rabbit.User.LastName}");
            //}
            //else
            //{
            //    Console.WriteLine("No rabbit found with the given ear tags.");
            //}

            return rabbit;
        }

        /// <summary>
        /// Tilføjer en Rabbit til den nuværende bruger og tilføjer den til Collectionen
        /// </summary>
        /// <param name="userId">GUID fra brugeren</param>
        /// <param name="newRabbitDto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Rabbit> AddRabbit_ToMyCollectionAsync(string userId, RabbitDTO newRabbitDto)
        {
            //Console.WriteLine($"Trying to add rabbit with RightEarId: {newRabbitDto.RightEarId}, LeftEarId: {newRabbitDto.LeftEarId}");

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
                //IsJuvenile = newRabbitDto.IsJuvenile,
                Gender = newRabbitDto.Gender,
                IsPublic = newRabbitDto.IsPublic
                // ... evt flere..
            };

            _validatorService.ValidateRabbit(newRabbit);

            var existingRabbit = await GetRabbitByEarTagsAsync(newRabbit.RightEarId, newRabbit.LeftEarId);
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("A rabbit with the same ear tags already exists.");
            }

            var currentUser = await _userService.GetUserByIdAsync(userId);

            if (currentUser == null)
            {
                throw new InvalidOperationException("No user found with the given GUID");
            }

            newRabbit.OwnerId = currentUser.Id;
            await _dbRepository.AddObjectAsync(newRabbit);
            Console.WriteLine($"Rabbit added successfully with RightEarId: {newRabbit.RightEarId}, LeftEarId: {newRabbit.LeftEarId}, OwnerId: {newRabbit.OwnerId}");
            
            return newRabbit;
        }



        //---------------------: UPDATE
        public async Task UpdateMyRabbitAsync(User currentUser, Rabbit rabbit)
        {
            if (rabbit.OwnerId != currentUser.Id)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }

            _validatorService.ValidateRabbit(rabbit);
            await _dbRepository.UpdateObjectAsync(rabbit);
        }

        public async Task UpdateRabbit_RBAC_Async(User currentUser, Rabbit rabbit, IList<Claim> userClaims)
        {
            var hasPermissionToUpdateOwn = userClaims.Any(c => c.Type == "Permission" && c.Value == "Update_Own_Rabbits");
            var hasPermissionToUpdateAll = userClaims.Any(c => c.Type == "Permission" && c.Value == "CRUD_All_Rabbits");

            if (!hasPermissionToUpdateAll && (!hasPermissionToUpdateOwn || currentUser.Id != rabbit.OwnerId))
            {
                throw new InvalidOperationException("You do not have permission to update this rabbit.");
            }

            _validatorService.ValidateRabbit(rabbit);
            await _dbRepository.UpdateObjectAsync(rabbit);
        }


        //---------------------: DELETE
        public async Task DeleteMyRabbitAsync(User currentUser, Rabbit rabbitToDelete)
        {
            if (rabbitToDelete.OwnerId != currentUser.Id)
            {
                throw new InvalidOperationException("You are not the owner of this rabbit.");
            }

            await _dbRepository.DeleteObjectAsync(rabbitToDelete);
        }



        public async Task DeleteRabbit_RBAC_Async(User currentUser, Rabbit rabbitToDelete, IList<Claim> userClaims)
        {
            var hasPermissionToDeleteOwn = userClaims.Any(c => c.Type == "Permission" && c.Value == "Delete_Own_Rabbits");
            var hasPermissionToDeleteAll = userClaims.Any(c => c.Type == "Permission" && c.Value == "CRUD_All_Rabbits");

            if (!hasPermissionToDeleteAll && (!hasPermissionToDeleteOwn || currentUser.Id != rabbitToDelete.OwnerId))
            {
                throw new InvalidOperationException("You do not have permission to delete this rabbit.");
            }

            await _dbRepository.DeleteObjectAsync(rabbitToDelete);
        }


        //---------------------: TRANSFER
        public async Task RequestRabbitTransfer(string rightEarId, string leftEarId, string breederRegNo)
        {
            // Check if the new owner exists in the database
            var newOwner = await _userService.GetUserByBreederRegNoAsync(breederRegNo);
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
                NewOwnerId = breederRegNo,
                IsAccepted = false
            };

            // Save the transfer request to the database
            //await _transferRequestRepository.AddObjectAsync(transferRequest);
        }
    }
}
