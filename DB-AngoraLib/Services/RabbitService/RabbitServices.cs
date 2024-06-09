using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.HelperService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
            // Validate input
            if (string.IsNullOrEmpty(userId) || newRabbitDto == null)
            {
                throw new ArgumentException("Invalid arguments");
            }
            // Create a new Rabbit from the DTO
            var newRabbit = new Rabbit
            {
                RightEarId = newRabbitDto.RightEarId,
                LeftEarId = newRabbitDto.LeftEarId,
                OwnerId = userId,
                NickName = newRabbitDto.NickName,
                Race = newRabbitDto.Race,
                Color = newRabbitDto.Color,
                //ApprovedRaceColorCombination          // Ikke nødvendig, dictionary, automatisk udregnet
                DateOfBirth = newRabbitDto.DateOfBirth,
                DateOfDeath = newRabbitDto.DateOfDeath,
                //IsJuvenile = newRabbitDto.IsJuvenile, // Ikke nødvendig, automatisk udregnet - evt frontend udregning?
                Gender = newRabbitDto.Gender,
                ForSale = newRabbitDto.ForSale,
                // ... evt flere..

                FatherId_Placeholder = newRabbitDto.Father_EarCombId,
                MotherId_Placeholder = newRabbitDto.Mother_EarCombId,
                //Father = await GetRabbit_ByEarCombIdAsync(newRabbitDto.Father_EarCombId),
                //Mother = await GetRabbit_ByEarCombIdAsync(newRabbitDto.Mother_EarCombId),
                //Father_EarCombId = newRabbitDto.Father_EarCombId,
                //Mother_EarCombId = newRabbitDto.Mother_EarCombId,
            };

            //newRabbit.EarCombId = $"{newRabbit.RightEarId}-{newRabbit.LeftEarId}";
            _validatorService.ValidateRabbit(newRabbit);

            await _dbRepository.AddObjectAsync(newRabbit); // Add the new rabbit to the database

            // Update the Father_EarCombId and Mother_EarCombId if the corresponding rabbits exist
            await UpdateParentIdAsync(newRabbit, newRabbit.FatherId_Placeholder, newRabbit.MotherId_Placeholder);

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

        public async Task<List<Rabbit_PreviewDTO>> GetAllRabbits_Forsale_Filtered(Rabbit_ForsaleFilterDTO filter)
        {
            var rabbits = await _dbRepository.GetDbSet()        // TODO: Overvej om døde Rabbits skal medtages (skind?)
                .Where(rabbit => 
                    rabbit.ForSale == ForSale.Ja &&
                    rabbit.DateOfDeath == null)
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




        public async Task<Rabbit> GetRabbit_ByEarCombIdAsync(string earCombId)
        {
            return await _dbRepository.GetObject_ByKEYAsync(earCombId);
        }

        



        public async Task<Rabbit_ProfileDTO> GetRabbit_ProfileAsync(string userId, string earCombId, IList<Claim> userClaims)
        {
            var rabbit = await GetRabbit_ByEarCombIdAsync(earCombId);
            var hasPermissionToGetAnyRabbit = userClaims.Any(
                c => c.Type == "RolePermission" && c.Value == "Get_Any_Rabbit"); // tilføj evt. "SpecialPermission" "Get_Any_Rabbit"

            if (rabbit == null)
            {
                return null;
            }

            await UpdateParentIdAsync(rabbit, rabbit.FatherId_Placeholder, rabbit.MotherId_Placeholder);

            if (rabbit.ForSale == ForSale.Ja || rabbit.OwnerId == userId || hasPermissionToGetAnyRabbit)
            {
                var rabbitProfile = new Rabbit_ProfileDTO
                {
                    // Set FatherStatusMessage and MotherStatusMessage properties
                    FatherStatusMessage = rabbit.FatherId_Placeholder == null ? null :
                                          rabbit.Father_EarCombId != null ? "Kanin fundet i systemet" : "Ikke fundet i systemet",
                    MotherStatusMessage = rabbit.MotherId_Placeholder == null ? null :
                                          rabbit.Mother_EarCombId != null ? "Kanin fundet i systemet" : "Ikke fundet i systemet",
                    Children = await GetRabbit_ChildCollection(earCombId),
                    //Pedigree = await GetRabbitPedigreeAsync(earCombId, 3)

                };

                HelperServices.CopyPropertiesTo(rabbit, rabbitProfile);

                return rabbitProfile;
            }

            return null;
        }

        /// <summary>
        /// Henter en Rabbits ICollection af <parent>Children.
        /// Kun er af de to lister har værdi, alt efter om Rabbit er far eller mor.
        /// </summary>
        /// <param name="earCombId">Rabbit Id</param>
        /// <returns>En kombineret liste af de to ICollections</returns>
        public async Task<List<Rabbit_ChildPreviewDTO>> GetRabbit_ChildCollection(string earCombId)
        {
            var rabbitParent = await _dbRepository.GetDbSet()
                .Include(r => r.FatheredChildren)
                .Include(r => r.MotheredChildren)
                .FirstOrDefaultAsync(r => r.EarCombId == earCombId);

            if (rabbitParent == null)
            {
                return null;
            }

            var allChildrenList = rabbitParent.FatheredChildren.Concat(rabbitParent.MotheredChildren)
                .Select(rabbitChild => new Rabbit_ChildPreviewDTO
                {
                    EarCombId = rabbitChild.EarCombId,
                    DateOfBirth = rabbitChild.DateOfBirth,
                    NickName = rabbitChild.NickName,
                    Color = rabbitChild.Color,
                    Gender = rabbitChild.Gender,
                    OtherParentId = rabbitParent.EarCombId == rabbitChild.Father_EarCombId ? rabbitChild.Mother_EarCombId : rabbitChild.Father_EarCombId
                })
                .ToList();

            return allChildrenList;
        }

        //public async Task<List<Rabbit>> GetRabbit_ChildCollection(string earCombId)
        //{
        //    var rabbit = await _dbRepository.GetDbSet()
        //        .Include(r => r.FatheredChildren)
        //        .Include(r => r.MotheredChildren)
        //        .FirstOrDefaultAsync(r => r.EarCombId == earCombId);

        //    if (rabbit == null)
        //    {
        //        return null;
        //    }

        //    var allChildrenList = rabbit.FatheredChildren.Concat(rabbit.MotheredChildren).ToList();
        //    return allChildrenList;
        //}




        //private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public async Task<Rabbit_PedigreeDTO> GetRabbitPedigreeAsync(string earCombId, int maxGeneration)
        {
            var rabbit = await _dbRepository.GetObject_ByKEYAsync(earCombId);
            return await GetRabbitPedigreeRecursive(rabbit, 0, maxGeneration);
        }



        private async Task<Rabbit_PedigreeDTO> GetRabbitPedigreeRecursive(Rabbit rabbit, int currentGeneration, int maxGeneration)
        {
            if (currentGeneration >= maxGeneration)
            {
                return new Rabbit_PedigreeDTO
                {
                    Rabbit = rabbit,
                    Generation = currentGeneration
                };
            }

            var father = await _dbRepository.GetObject_ByKEYAsync(rabbit.Father_EarCombId);
            var mother = await _dbRepository.GetObject_ByKEYAsync(rabbit.Mother_EarCombId);

            return new Rabbit_PedigreeDTO
            {
                Rabbit = rabbit,
                Father = father != null ? await GetRabbitPedigreeRecursive(father, currentGeneration + 1, maxGeneration) : null,
                Mother = mother != null ? await GetRabbitPedigreeRecursive(mother, currentGeneration + 1, maxGeneration) : null,
                Generation = currentGeneration
            };
        }




        //---------------------: UPDATE
        public async Task<Rabbit_ProfileDTO> UpdateRabbit_RBAC_Async(string userId, string earCombId, Rabbit_UpdateDTO rabbit_updateDTO, IList<Claim> userClaims)
        {
            var hasPermissionToUpdateOwn = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Update_Own_Rabbit"); // tilføj evt. "SpecialPermission" "Update_Own_Rabbit"
            var hasPermissionToUpdateAll = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Update_Any_Rabbit"); // tilføj evt. "SpecialPermission" "Update_Any_Rabbit"

            var rabbit_InDB = await GetRabbit_ByEarCombIdAsync(earCombId);
            if (rabbit_InDB == null)
            {
                return null;
            }

            await UpdateParentIdAsync(rabbit_InDB, rabbit_InDB.FatherId_Placeholder, rabbit_InDB.MotherId_Placeholder);

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
        public async Task<Rabbit_PreviewDTO> DeleteRabbit_RBAC_Async(string userId, string earCombId, IList<Claim> userClaims)
        {
            var hasPermissionToDeleteOwn = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Delete_Own_Rabbit"); // tilføj evt. "SpecialPermission" "Delete_Own_Rabbit"
            var hasPermissionToDeleteAll = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "Delete_Any_Rabbit"); // tilføj evt. "SpecialPermission" "Delete_Any_Rabbit"

            var rabbitToDelete = await GetRabbit_ByEarCombIdAsync(earCombId);
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
        public async Task RequestRabbitTransfer(string earCombId, string breederRegNo)
        {
            // Check if the new owner exists in the database
            var newOwner = await _accountService.GetUserByBreederRegNoAsync(breederRegNo);
            if (newOwner == null)
            {
                throw new Exception("New owner not found");
            }

            // Find the rabbit to transfer
            var rabbit = await GetRabbit_ByEarCombIdAsync(earCombId);
            if (rabbit == null)
            {
                throw new Exception("Rabbit not found");
            }

            // Create a new transfer request
            var transferRequest = new RabbitTransferRequest
            {
                EarCombId = earCombId,
                CurrentOwnerId = rabbit.OwnerId,
                NewOwnerId = breederRegNo,
                IsAccepted = false
            };

            // Save the transfer request to the database
            //await _transferRequestRepository.AddObjectAsync(transferRequest);
        }

        //---------------------: HELPER METHODs

        /// <summary>
        /// Kigger efter om der findes nogle eksisterende Rabbits i systemet,
        /// som matcher de angivne EarCombIds som brugeren har angivet for forældrende
        /// </summary>
        /// <param name="rabbit">Rabbit objekt hvis forældre skal undersøges</param>
        /// <param name="fatherIdPlaceholder">Id på angivne far</param>
        /// <param name="motherIdPlaceholder">Id på angivne mor</param>
        /// <returns></returns>
        private async Task UpdateParentIdAsync(Rabbit rabbit, string? fatherIdPlaceholder, string? motherIdPlaceholder)
        {
            // If fatherIdPlaceholder is not null, update Father_EarCombId
            if (fatherIdPlaceholder != null)
            {
                var existingFather = await GetRabbit_ByEarCombIdAsync(fatherIdPlaceholder);
                if (existingFather != null && existingFather.Gender == Gender.Han)
                {
                    rabbit.Father_EarCombId = fatherIdPlaceholder;
                }
                else // hvis existingFather ikke eksistere, så fjern forælderen
                {
                    rabbit.Father_EarCombId = null;
                }
            }

            // If motherIdPlaceholder is not null, update Mother_EarCombId
            if (motherIdPlaceholder != null)
            {
                var existingMother = await GetRabbit_ByEarCombIdAsync(motherIdPlaceholder);
                if (existingMother != null && existingMother.Gender == Gender.Hun)
                {
                    rabbit.Mother_EarCombId = motherIdPlaceholder;
                }
                else // hvis existingMother ikke eksistere, så fjern forælderen
                {
                    rabbit.Mother_EarCombId = null;
                }
            }

            await _dbRepository.UpdateObjectAsync(rabbit); // Update the rabbit in the database
        }



    }
}
