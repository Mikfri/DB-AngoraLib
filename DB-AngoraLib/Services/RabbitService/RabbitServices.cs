using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.HelperService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;


namespace DB_AngoraLib.Services.RabbitService
{    
    public class RabbitServices : IRabbitService
    {
        private readonly IGRepository<Rabbit> _dbRepository;
        private readonly IAccountService _accountService;
        private readonly RabbitValidator _validatorService;

        public RabbitServices(IGRepository<Rabbit> dbRepository, IAccountService userService, RabbitValidator validatorService)
        {
            _dbRepository = dbRepository;
            _accountService = userService;
            _validatorService = validatorService;
        }

        public RabbitServices() { }



        //---------------------: ADD
        /// <summary>
        /// Tilføjer en Rabbit til den nuværende bruger og tilføjer den til Collectionen
        /// </summary>
        /// <param name="userId">GUID fra brugeren</param>
        /// <param name="newRabbitDto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Rabbit_ProfileDTO> AddRabbit_ToMyCollectionAsync(string userId, Rabbit_CreateDTO newRabbitDto)
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
                //ApprovedRaceColorCombination          // Ikke nødvendig, dictionary, automatisk udregnet
                DateOfBirth = newRabbitDto.DateOfBirth,
                DateOfDeath = newRabbitDto.DateOfDeath,
                //IsJuvenile = newRabbitDto.IsJuvenile, // Ikke nødvendig, automatisk udregnet - evt frontend udregning?
                Gender = newRabbitDto.Gender,
                OpenProfile = newRabbitDto.OpenProfile
                // ... evt flere..
            };

            _validatorService.ValidateRabbit(newRabbit);

            var existingRabbit = await GetRabbit_ByEarTagsAsync(newRabbit.RightEarId, newRabbit.LeftEarId);
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("En kanin med samme øremærke kombination eksistere allerede");
            }

            var thisUser = await _accountService.GetUserByIdAsync(userId);

            if (thisUser == null)
            {
                throw new InvalidOperationException("No user found with the given GUID");
            }

            newRabbit.OwnerId = thisUser.Id;
            await _dbRepository.AddObjectAsync(newRabbit);

            // Create a new RabbitDTO and copy properties from newRabbit
            var newRabbit_ProfileDTO = new Rabbit_ProfileDTO();
            HelperServices.CopyPropertiesTo(newRabbit, newRabbit_ProfileDTO);

            return newRabbit_ProfileDTO;
        }



        //---------------------: GET METODER
        public async Task<List<Rabbit>> GetAllRabbitsAsync()     // reeeally though? Skal vi bruge denne metode, udover test?
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<List<Rabbit>> GetAllRabbits_ByBreederRegAsync(string breederRegNo)
        {
            var user = await _accountService.GetUserByBreederRegNoAsync(breederRegNo);
            if (user == null)
            {
                return null;
            }
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.Where(rabbit => rabbit.OwnerId == user.Id).ToList();
        }


        //-------: GET BY EAR TAGs METODER
        /// <summary>
        /// Finder en Rabbit ud fra dens composite-KEY (øremærkerne kombinationen)
        /// </summary>
        /// <param name="rightEarId"></param>
        /// <param name="leftEarId"></param>
        /// <returns></returns>
        public async Task<Rabbit> GetRabbit_ByEarTagsAsync(string rightEarId, string leftEarId)
        {
            Expression<Func<Rabbit, bool>> filter = r => r.RightEarId == rightEarId && r.LeftEarId == leftEarId;

            //Console.WriteLine($"Checking for existing Rabbit with ear tags: RightEarId: {rightEarId}, LeftEarId: {leftEarId}");

            var rabbit = await _dbRepository.GetObject_ByFilterAsync(filter);

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

        public async Task<List<Rabbit_PreviewDTO>> GetAllRabbits_OpenProfile_Filtered(Rabbit_ForsaleFilterDTO filter)
        {
            var rabbits = await _dbRepository.GetDbSet()
                .Where(rabbit => rabbit.OpenProfile == OpenProfile.Ja)
                .ToListAsync();

            return rabbits
                .Where(rabbit =>
                       (filter.RightEarId == null || rabbit.RightEarId == filter.RightEarId) // Vi vil kunne søge på hvor den kommer fra!
                    //&& (filter.LeftEarId == null || rabbit.LeftEarId == filter.LeftEarId) // hvorfor sq man søge på venstre øre?
                    && (filter.NickName == null || rabbit.NickName == filter.NickName)      // hvorfor søge på navn?
                    && (filter.Race == null || rabbit.Race == filter.Race)
                    && (filter.Color == null || rabbit.Color == filter.Color)
                    && (filter.Gender == null || rabbit.Gender == filter.Gender)
                    && (filter.IsJuvenile == null || rabbit.IsJuvenile == filter.IsJuvenile)
                    && (filter.ApprovedRaceColorCombination == null || rabbit.ApprovedRaceColorCombination == filter.ApprovedRaceColorCombination))

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



        public async Task<Rabbit_ProfileDTO> GetRabbit_ProfileAsync(
            string userId, string rightEarId, string leftEarId, IList<Claim> userClaims)
        {
            var rabbit = await GetRabbit_ByEarTagsAsync(rightEarId, leftEarId);
            var hasPermissionToGetAnyRabbit = userClaims.Any(
                c => c.Type == "RolePermission" && c.Value == "Get_Any_Rabbit"); // tilføj evt. "SpecialPermission" "Get_Any_Rabbit"

            if (rabbit == null)
            {
                return null;
            }            

            if (rabbit.OpenProfile == OpenProfile.Ja || rabbit.OwnerId == userId || hasPermissionToGetAnyRabbit)
            {
                var rabbitProfile = new Rabbit_ProfileDTO();
                HelperServices.CopyPropertiesTo(rabbit, rabbitProfile);

                return rabbitProfile;
            }

            return null;
        }





        //---------------------: UPDATE
        public async Task<Rabbit_ProfileDTO> UpdateRabbit_RBAC_Async(string userId, string rightEarId, string leftEarId, Rabbit_UpdateDTO rabbit_updateDTO, IList<Claim> userClaims)
        {
            var hasPermissionToUpdateOwn = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Update_Own_Rabbit"); // tilføj evt. "SpecialPermission" "Update_Own_Rabbit"
            var hasPermissionToUpdateAll = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Update_Any_Rabbit"); // tilføj evt. "SpecialPermission" "Update_Any_Rabbit"

            var rabbit_InDB = await GetRabbit_ByEarTagsAsync(rightEarId, leftEarId);
            if (rabbit_InDB == null)
            {
                return null;
            }

            if (hasPermissionToUpdateAll || (hasPermissionToUpdateOwn && userId == rabbit_InDB.OwnerId))
            {
                // Copy all non-null properties from updatedRabbit to rabbitToUpdate
                HelperServices.CopyPropertiesTo(rabbit_updateDTO, rabbit_InDB);

                _validatorService.ValidateRabbit(rabbit_InDB);
                await _dbRepository.UpdateObjectAsync(rabbit_InDB);
            }

            // Create a new Rabbit_ProfileDTO and copy properties from updatedRabbit
            var rabbit_ProfileDTO = new Rabbit_ProfileDTO();
            HelperServices.CopyPropertiesTo(rabbit_InDB, rabbit_ProfileDTO);

            return rabbit_ProfileDTO; // Return the updated rabbit as a Rabbit_ProfileDTO
        }





        //---------------------: DELETE
        public async Task<Rabbit_PreviewDTO> DeleteRabbit_RBAC_Async(string userId, string rightEarId, string leftEarId, IList<Claim> userClaims)
        {
            var hasPermissionToDeleteOwn = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Delete_Own_Rabbit"); // tilføj evt. "SpecialPermission" "Delete_Own_Rabbit"
            var hasPermissionToDeleteAll = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Delete_Any_Rabbit"); // tilføj evt. "SpecialPermission" "Delete_Any_Rabbit"

            var rabbitToDelete = await GetRabbit_ByEarTagsAsync(rightEarId, leftEarId);
            if (rabbitToDelete == null)
            {
                throw new InvalidOperationException("Ingen kanin med angivede ørermærker eksistere");
            }

            if (!hasPermissionToDeleteAll && (!hasPermissionToDeleteOwn || userId != rabbitToDelete.OwnerId))
            {
                throw new InvalidOperationException("Du har ikke tilladelse til denne handling");
            }

            // Create a new Rabbit_PreviewDTO and copy properties from rabbitToDelete
            var rabbitPreviewDTO = new Rabbit_PreviewDTO();
            HelperServices.CopyPropertiesTo(rabbitToDelete, rabbitPreviewDTO);

            await _dbRepository.DeleteObjectAsync(rabbitToDelete);

            return rabbitPreviewDTO; // Return the rabbit to be deleted as a Rabbit_PreviewDTO
        }




        //---------------------: TRANSFER
        public async Task RequestRabbitTransfer(string rightEarId, string leftEarId, string breederRegNo)
        {
            // Check if the new owner exists in the database
            var newOwner = await _accountService.GetUserByBreederRegNoAsync(breederRegNo);
            if (newOwner == null)
            {
                throw new Exception("New owner not found");
            }

            // Find the rabbit to transfer
            var rabbit = await GetRabbit_ByEarTagsAsync(rightEarId, leftEarId);
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
