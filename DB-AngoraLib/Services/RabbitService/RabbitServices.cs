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
        public async Task<Rabbit_ProfileDTO> AddRabbit_ToMyCollectionAsync(string userId, Rabbit_CreateDTO newRabbitDTO)
        {
            //Console.WriteLine($"Trying to add rabbit with RightEarId: {newRabbitDto.RightEarId}, LeftEarId: {newRabbitDto.LeftEarId}");
            // Validate input
            if (string.IsNullOrEmpty(userId) || newRabbitDTO == null)
            {
                throw new ArgumentException("Invalid arguments");
            }
            // Create a new Rabbit from the DTO
            var newRabbit = new Rabbit
            {
                RightEarId = newRabbitDTO.RightEarId,
                LeftEarId = newRabbitDTO.LeftEarId,
                OwnerId = userId,
                NickName = newRabbitDTO.NickName,
                Race = newRabbitDTO.Race,
                Color = newRabbitDTO.Color,
                //ApprovedRaceColorCombination          // Ikke nødvendig, dictionary, automatisk udregnet
                DateOfBirth = newRabbitDTO.DateOfBirth,
                DateOfDeath = newRabbitDTO.DateOfDeath,
                //IsJuvenile = newRabbitDto.IsJuvenile, // Ikke nødvendig, automatisk udregnet - evt frontend udregning?
                Gender = newRabbitDTO.Gender,
                ForSale = newRabbitDTO.ForSale,
                // ... evt flere..

                FatherId_Placeholder = newRabbitDTO.Father_EarCombId,
                MotherId_Placeholder = newRabbitDTO.Mother_EarCombId,
                //Father = await GetRabbit_ByEarCombIdAsync(newRabbitDto.Father_EarCombId),
                //Mother = await GetRabbit_ByEarCombIdAsync(newRabbitDto.Mother_EarCombId),
                //Father_EarCombId = newRabbitDto.Father_EarCombId,
                //Mother_EarCombId = newRabbitDto.Mother_EarCombId,
            };

            //newRabbit.EarCombId = $"{newRabbit.RightEarId}-{newRabbit.LeftEarId}";
            _validatorService.ValidateRabbit(newRabbit);

            await _dbRepository.AddObjectAsync(newRabbit); // Add the new rabbit to the database

            await UserOriginFK_SetupAsync(newRabbit);
            await ParentsFK_SetupAsync(newRabbit);
            await ChildFK_SetupAsync(newRabbit);

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
            var rabbits = await _dbRepository.GetDbSet()
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

            // Tjek om der findes eksisterende Rabbits i systemet, som matcher de angivne EarCombIds som brugeren har angivet for forældrende
            await ParentsFK_SetupAsync(rabbit);
            await UserOriginFK_SetupAsync(rabbit);

            if (rabbit.ForSale == ForSale.Ja || rabbit.OwnerId == userId || hasPermissionToGetAnyRabbit)
            {
                var rabbitProfile = new Rabbit_ProfileDTO
                {
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
            return await GetRabbit_PedigreeRecursive(rabbit, 0, maxGeneration);
        }



        private async Task<Rabbit_PedigreeDTO> GetRabbit_PedigreeRecursive(Rabbit rabbit, int currentGeneration, int maxGeneration)
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
                Father = father != null ? await GetRabbit_PedigreeRecursive(father, currentGeneration + 1, maxGeneration) : null,
                Mother = mother != null ? await GetRabbit_PedigreeRecursive(mother, currentGeneration + 1, maxGeneration) : null,
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


            if (hasPermissionToUpdateAll || (hasPermissionToUpdateOwn && userId == rabbit_InDB.OwnerId))
            {
                // Copy all non-null properties from updatedRabbit to rabbitToUpdate
                HelperServices.CopyPropertiesTo(rabbit_updateDTO, rabbit_InDB);

                _validatorService.ValidateRabbit(rabbit_InDB);
                await _dbRepository.UpdateObjectAsync(rabbit_InDB);
            }

            await ParentsFK_SetupAsync(rabbit_InDB);

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
                throw new InvalidOperationException("Ingen kanin med den angivne ørermærke-kombi eksistere");
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
        /// Kigger efter om der findes eksisterende Rabbit-parents i systemet,
        /// hvor den angivne Rabbit's <Parent>_EarCombIds, mathcher andre Rabbits. 
        /// Hvis ja, etableres en forbindelse efter forbehold
        /// </summary>
        /// <param name="rabbit">Rabbit object hvis ParentId_Placeholders skal undersøges</param>
        /// <returns></returns>
        private async Task ParentsFK_SetupAsync(Rabbit rabbit)
        {
            // Antag at Rabbit modellen har properties for FatherId_Placeholder og MotherId_Placeholder
            string? fatherIdPlaceholder = rabbit.FatherId_Placeholder;
            string? motherIdPlaceholder = rabbit.MotherId_Placeholder;

            // Forsøg at finde en eksisterende far kanin, hvis placeholder ikke er null
            Rabbit existingFather = null;
            if (fatherIdPlaceholder != null)
            {
                existingFather = await GetRabbit_ByEarCombIdAsync(fatherIdPlaceholder);
            }

            // Opdater Father_EarCombId kun hvis en gyldig far kanin findes
            if (existingFather != null && existingFather.Gender == Gender.Han)
            {
                rabbit.Father_EarCombId = fatherIdPlaceholder;
            }
            else
            {
                rabbit.Father_EarCombId = null; // Sæt til null hvis ingen gyldig far kanin findes eller placeholder er null
            }

            // Samme som oven, men for Mother_EarCombId
            Rabbit existingMother = null;
            if (motherIdPlaceholder != null)
            {
                existingMother = await GetRabbit_ByEarCombIdAsync(motherIdPlaceholder);
            }

            if (existingMother != null && existingMother.Gender == Gender.Hun)
            {
                rabbit.Mother_EarCombId = motherIdPlaceholder;
            }
            else
            {
                rabbit.Mother_EarCombId = null;
            }

            await _dbRepository.UpdateObjectAsync(rabbit); // Update the rabbit in the database
        }


        /// <summary>
        /// Kigger efter om der eksistere Rabbits i DB som peger på det snarlige nye object, 'newRabbit', som forælder.
        /// Hvis nogen Rabbits i DB pejer på 'newRabbit' etableres en FK forbindelse, med forbehold for køn.
        /// </summary>
        /// <param name="newRabbit"></param>
        /// <returns></returns>
        private async Task ChildFK_SetupAsync(Rabbit newRabbit)
        {
            List<Rabbit> rabbitsToUpdate = new List<Rabbit>();

            if (newRabbit.Gender == Gender.Han)
            {
                var children = await _dbRepository.GetDbSet()
                    .Where(r => r.FatherId_Placeholder == newRabbit.EarCombId)
                    .ToListAsync();

                foreach (var child in children)
                {
                    child.Father_EarCombId = newRabbit.EarCombId;
                    rabbitsToUpdate.Add(child);
                }
            }
            else if (newRabbit.Gender == Gender.Hun)
            {
                var children = await _dbRepository.GetDbSet()
                    .Where(r => r.MotherId_Placeholder == newRabbit.EarCombId)
                    .ToListAsync();

                foreach (var child in children)
                {
                    child.Mother_EarCombId = newRabbit.EarCombId;
                    rabbitsToUpdate.Add(child);
                }
            }

            // Gem kun ændringerne, hvis der er nogen rabbits at opdatere
            if (rabbitsToUpdate.Any())
            {
                await _dbRepository.SaveObjects(rabbitsToUpdate);
            }
        }


        private async Task UserOriginFK_SetupAsync(Rabbit rabbit)
        {
            //string? rightEarId = rabbit.RightEarId;
            // Find the user with the BreederRegNo that matches the rabbit's RightEarId
            var user = await _accountService.GetUserByBreederRegNoAsync(rabbit.RightEarId);

            if (user != null)
            {
                // If the user exists, establish the relationship between the rabbit and the user
                rabbit.OriginId = user.Id;
            }
            else
            {
                // If the user does not exist, set the OriginId to null
                rabbit.OriginId = null;
            }

            // Update the rabbit in the database
            await _dbRepository.UpdateObjectAsync(rabbit); // Update the rabbit in the database
        }


    }
}
