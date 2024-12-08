using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.BreederService;
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
        private readonly IBreederService _breederService;

        public RabbitServices(IGRepository<Rabbit> dbRepository, IBreederService breederService)
        {
            _dbRepository = dbRepository;
            _breederService = breederService;
        }

        //---------------------: ADD
        /// <summary>
        /// Tilføjer en Rabbit til den nuværende bruger og tilføjer den til Collectionen
        /// </summary>
        /// <param name="userId">GUID fra brugeren</param>
        /// <param name="newRabbitDto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Rabbit_ProfileDTO> AddRabbit_ToMyCollection(string userId, Rabbit_CreateDTO newRabbitDTO)
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
                DateOfBirth = newRabbitDTO.DateOfBirth,
                DateOfDeath = newRabbitDTO.DateOfDeath,
                Gender = newRabbitDTO.Gender,
                ForSale = newRabbitDTO.ForSale,
                ForBreeding = newRabbitDTO.ForBreeding,
                //ProfilePicture = newRabbitDTO.ProfilePicture,

                FatherId_Placeholder = newRabbitDTO.FatherId_Placeholder,
                MotherId_Placeholder = newRabbitDTO.MotherId_Placeholder,
            };
            newRabbit.ValidateRabbit();

            var existingRabbit = await Get_Rabbit_ByEarCombId($"{newRabbit.RightEarId}-{newRabbit.LeftEarId}");
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("En kanin med den angivne ørermærke-kombi eksistere allerede");
            }


            await _dbRepository.AddObjectAsync(newRabbit); // Add the new rabbit to the database



            await UserOriginFK_SetupAsync(newRabbit);
            await ParentsFK_SetupAsync(newRabbit);
            await ChildFK_SetupAsync(newRabbit.EarCombId, newRabbit.Gender);

            // Create a new RabbitDTO and copy properties from newRabbit
            var newRabbit_ProfileDTO = new Rabbit_ProfileDTO();
            HelperServices.CopyProperties_FromAndTo(newRabbit, newRabbit_ProfileDTO);

            return newRabbit_ProfileDTO;
        }

        //---------------------: GET METODER
        public async Task<List<Rabbit>> Get_AllRabbits()     // reeeally though? Skal vi bruge denne metode, udover test?
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<List<Rabbit_ForSalePreviewDTO>> Get_AllRabbits_Forsale_Filtered(Rabbit_ForsaleFilterDTO filter)
        {
            var query = _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(rabbit => rabbit.UserOwner)
                .Where(rabbit =>
                    rabbit.ForSale == IsPublic.Ja &&
                    rabbit.DateOfDeath == null &&
                    (filter.RightEarId == null || rabbit.RightEarId == filter.RightEarId) &&
                    (filter.Race == null || rabbit.Race == filter.Race) &&
                    (filter.Color == null || rabbit.Color == filter.Color) &&
                    (filter.Gender == null || rabbit.Gender == filter.Gender) &&
                    (filter.BornAfter == null || rabbit.DateOfBirth >= filter.BornAfter)
                );

            if (filter.MinZipCode.HasValue && filter.MaxZipCode.HasValue)
            {
                query = query.Where(rabbit =>
                    rabbit.UserOwner != null &&
                    rabbit.UserOwner.ZipCode >= filter.MinZipCode.Value &&
                    rabbit.UserOwner.ZipCode <= filter.MaxZipCode.Value
                );
            }
            else if (filter.MinZipCode.HasValue)
            {
                query = query.Where(rabbit =>
                    rabbit.UserOwner != null &&
                    rabbit.UserOwner.ZipCode >= filter.MinZipCode.Value
                );
            }
            else if (filter.MaxZipCode.HasValue)
            {
                query = query.Where(rabbit =>
                    rabbit.UserOwner != null &&
                    rabbit.UserOwner.ZipCode <= filter.MaxZipCode.Value
                );
            }

            var rabbits = await query
                .Select(rabbit => new Rabbit_ForSalePreviewDTO
                {
                    EarCombId = rabbit.EarCombId,
                    NickName = rabbit.NickName,
                    DateOfBirth = rabbit.DateOfBirth,
                    Race = rabbit.Race,
                    Color = rabbit.Color,
                    Gender = rabbit.Gender,
                    ProfilePicture = rabbit.ProfilePicture,
                    UserOwner = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                })
                .ToListAsync();

            return rabbits;
        }

        public async Task<List<Rabbit_ForSalePreviewDTO>> Get_AllRabbits_Forbreeding_Filtered(Rabbit_ForbreedingFilterDTO filter)
        {
            var rabbits = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(rabbit => rabbit.UserOwner) // Include the owner of the rabbit
                .Where(rabbit =>
                    rabbit.ForBreeding == IsPublic.Ja &&
                    rabbit.DateOfDeath == null)
                .ToListAsync();

            return rabbits
                .Where(rabbit =>
                    (filter.RightEarId == null || rabbit.RightEarId == filter.RightEarId) // Vi vil kunne søge på hvor den kommer fra!
                    && (filter.Race == null || rabbit.Race == filter.Race)
                    && (filter.Color == null || rabbit.Color == filter.Color)
                    && (filter.Gender == null || rabbit.Gender == filter.Gender)
                    && (filter.IsJuvenile == null || rabbit.IsJuvenile == filter.IsJuvenile)
                    && (filter.ApprovedRaceColorCombination == null || rabbit.ApprovedRaceColorCombination == filter.ApprovedRaceColorCombination))

                .Select(rabbit => new Rabbit_ForSalePreviewDTO
                {
                    EarCombId = rabbit.EarCombId,
                    NickName = rabbit.NickName,
                    Race = rabbit.Race,
                    Color = rabbit.Color,
                    Gender = rabbit.Gender,
                    UserOwner = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                    //UserOrigin = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null,
                })
                .ToList();
        }

        public async Task<List<Rabbit_OwnedPreviewDTO>> Get_AllRabbits_ByBreederRegNo(string breederRegNo)
        {
            var user = await _breederService.Get_BreederByBreederRegNo(breederRegNo);
            if (user == null)
            {
                return new List<Rabbit_OwnedPreviewDTO>(); // Returner en tom liste i stedet for null
            }

            // Brug GetAll_RabbitsOwned_Filtered metoden til at hente kaninerne
            var filter = new Rabbit_OwnedFilterDTO(); // Du kan tilføje filtreringskriterier her, hvis nødvendigt
            var rabbits = await _breederService.GetAll_RabbitsOwned_Filtered(user.Id, filter);

            return rabbits;
        }


        private async Task<Rabbit?> Get_Rabbit_ByEarCombId_IncludingUserRelations(string earCombId)
        {
            return await _dbRepository
                .GetDbSet()
                .AsNoTracking() // Tilføjer AsNoTracking for bedre ydeevne
                .Include(r => r.UserOrigin)
                .Include(r => r.UserOwner)
                .FirstOrDefaultAsync(r => r.EarCombId == earCombId);
        }


        public async Task<Rabbit> Get_Rabbit_ByEarCombId(string earCombId)
        {
            return await _dbRepository.GetObject_ByStringKEYAsync(earCombId);
        }

        public async Task<Rabbit_ProfileDTO> Get_Rabbit_Profile(string userId, string earCombId, IList<Claim> userClaims)
        {
            var rabbit = await _dbRepository
                .GetDbSet()
                //.AsNoTracking() // Tilføjer AsNoTracking for bedre ydeevne
                .Include(r => r.UserOrigin)
                .Include(r => r.UserOwner)
                .FirstOrDefaultAsync(r => r.EarCombId == earCombId);

            var hasPermissionToGetAnyRabbit = userClaims.Any(
                c => c.Type == "Rabbit:Read" && c.Value == "Any");

            if (rabbit == null)
            {
                return null;
            }

            await ParentsFK_SetupAsync(rabbit);
            await UserOriginFK_SetupAsync(rabbit);

            if (rabbit.ForSale == IsPublic.Ja || rabbit.OwnerId == userId || hasPermissionToGetAnyRabbit)
            {
                var rabbitProfileDTO = new Rabbit_ProfileDTO
                {
                    Children = await Get_Rabbit_ChildCollection(earCombId),
                };

                HelperServices.CopyProperties_FromAndTo(rabbit, rabbitProfileDTO);
                rabbitProfileDTO.OriginFullName = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null;
                rabbitProfileDTO.OwnerFullName = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null;

                return rabbitProfileDTO;
            }
            return null;
        }

        /// <summary>
        /// Henter en Rabbits ICollection af <Fath-/Mothered>Children.
        /// Kun er af de to lister har værdi, alt efter om Rabbit er far eller mor.
        /// </summary>
        /// <param name="earCombId">Rabbit Id</param>
        /// <returns>En kombineret liste af de to ICollections</returns>
        public async Task<List<Rabbit_ChildPreviewDTO>> Get_Rabbit_ChildCollection(string earCombId)
        {
            var rabbitParent = await _dbRepository.GetDbSet()
                .AsNoTracking()
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


        //private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        //------: PEDIGREE

        public async Task<Rabbit_PedigreeDTO> Get_RabbitPedigree(string earCombId/*, int maxGeneration*/)
        {
            int maxGeneration = 3; // Eksempel: Sætter maksimalt antal generationer til 3

            var rabbit = await Get_Rabbit_ByEarCombId_IncludingUserRelations(earCombId); // Antager denne metode returnerer en Rabbit inklusiv UserOrigin og UserOwner
            if (rabbit == null) return null;
            return await Get_Rabbit_PedigreeRecursive(rabbit, 0, maxGeneration, "Selv");
        }

        private async Task<Rabbit_PedigreeDTO> Get_Rabbit_PedigreeRecursive(Rabbit rabbit, int currentGeneration, int maxGeneration, string relationPrefix)
        {
            if (rabbit == null || currentGeneration > maxGeneration)
            {
                return null;
            }

            var father = await Get_Rabbit_ByEarCombId_IncludingUserRelations(rabbit.Father_EarCombId);
            var mother = await Get_Rabbit_ByEarCombId_IncludingUserRelations(rabbit.Mother_EarCombId);

            // Opbyg relation for far og mor baseret på den nuværende relationPrefix
            string fatherRelation = FamRelation(relationPrefix, "Far");
            string motherRelation = FamRelation(relationPrefix, "Mor");

            return new Rabbit_PedigreeDTO
            {
                Generation = currentGeneration,
                Relation = relationPrefix,
                EarCombId = rabbit.EarCombId,
                NickName = rabbit.NickName,
                Race = rabbit.Race,
                Color = rabbit.Color,
                DateOfBirth = rabbit.DateOfBirth,
                UserOwnerName = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                UserOriginName = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null,
                
                Father = father != null ? await Get_Rabbit_PedigreeRecursive(father, currentGeneration + 1, maxGeneration, fatherRelation) : null,
                Mother = mother != null ? await Get_Rabbit_PedigreeRecursive(mother, currentGeneration + 1, maxGeneration, motherRelation) : null,
            };
        }

        private string FamRelation(string currentRelation, string newRelation)
        {
            if (currentRelation == "Selv") return newRelation;
            return currentRelation + newRelation;
        }

        public async Task<Rabbit_PedigreeDTO> Get_RabbitTestParingPedigree(string fatherEarCombId, string motherEarCombId)
        {
            if (string.IsNullOrEmpty(fatherEarCombId))
            {
                throw new ArgumentNullException(nameof(fatherEarCombId), "Father EarCombId cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(motherEarCombId))
            {
                throw new ArgumentNullException(nameof(motherEarCombId), "Mother EarCombId cannot be null or empty.");
            }

            var father = await Get_Rabbit_ByEarCombId_IncludingUserRelations(fatherEarCombId);
            var mother = await Get_Rabbit_ByEarCombId_IncludingUserRelations(motherEarCombId);

            if (father == null || mother == null)
            {
                throw new ArgumentException("One or both of the provided ear comb IDs do not correspond to existing rabbits.");
            }

            if (father.Gender != Gender.Buck)
            {
                throw new ArgumentException("The rabbit specified as the father is not male.");
            }

            if (mother.Gender != Gender.Doe)
            {
                throw new ArgumentException("The rabbit specified as the mother is not female.");
            }

            var testRabbit = new Rabbit
            {
                Father = father,
                Mother = mother,
                Father_EarCombId = fatherEarCombId,
                Mother_EarCombId = motherEarCombId
            };

            return await Get_Rabbit_PedigreeRecursive(testRabbit, 0, 3, "Selv"); // Assuming maxGeneration is 3 for the test pedigree
        }

        //---------------------: UPDATE
        public async Task<Rabbit_ProfileDTO> UpdateRabbit_RBAC(string userId, string earCombId, Rabbit_UpdateDTO rabbit_updateDTO, IList<Claim> userClaims)
        {
            var hasPermissionToUpdateOwn = userClaims.Any(c => c.Type == "Rabbit:Update" && c.Value == "Own");
            var hasPermissionToUpdateAll = userClaims.Any(c => c.Type == "Rabbit:Update" && c.Value == "Any");

            var rabbit_InDB = await Get_Rabbit_ByEarCombId(earCombId);
            if (rabbit_InDB == null)
            {
                return null;
            }


            if (hasPermissionToUpdateAll || (hasPermissionToUpdateOwn && userId == rabbit_InDB.OwnerId))
            {
                // Overfør alle properties fra Rabbit_UpdateDTO til rabbit_InDB
                rabbit_InDB.NickName = rabbit_updateDTO.NickName;
                rabbit_InDB.Race = rabbit_updateDTO.Race;
                rabbit_InDB.Color = rabbit_updateDTO.Color;
                rabbit_InDB.DateOfBirth = rabbit_updateDTO.DateOfBirth;
                rabbit_InDB.DateOfDeath = rabbit_updateDTO.DateOfDeath;
                rabbit_InDB.Gender = rabbit_updateDTO.Gender;
                rabbit_InDB.ForSale = rabbit_updateDTO.ForSale;
                rabbit_InDB.ForBreeding = rabbit_updateDTO.ForBreeding;
                rabbit_InDB.FatherId_Placeholder = rabbit_updateDTO.FatherId_Placeholder;
                rabbit_InDB.MotherId_Placeholder = rabbit_updateDTO.MotherId_Placeholder;

                rabbit_InDB.ValidateRabbit();

                await _dbRepository.UpdateObjectAsync(rabbit_InDB);
            }

            await ParentsFK_SetupAsync(rabbit_InDB);

            // Create a new Rabbit_ProfileDTO and copy properties from updatedRabbit
            var rabbit_ProfileDTO = new Rabbit_ProfileDTO();
            HelperServices.CopyProperties_FromAndTo(rabbit_InDB, rabbit_ProfileDTO);

            return rabbit_ProfileDTO; // Return the updated rabbit as a Rabbit_ProfileDTO
        }





        //---------------------: DELETE
        public async Task<Rabbit_OwnedPreviewDTO> DeleteRabbit_RBAC(string userId, string earCombId, IList<Claim> userClaims)
        {
            var hasPermissionToDeleteOwn = userClaims.Any(c => c.Type == "Rabbit:Delete" && c.Value == "Own");
            var hasPermissionToDeleteAll = userClaims.Any(c => c.Type == "Rabbit:Delete" && c.Value == "Any");

            var rabbitToDelete = await Get_Rabbit_ByEarCombId(earCombId);
            if (rabbitToDelete == null)
            {
                throw new InvalidOperationException("Ingen kanin med den angivne ørermærke-kombi eksistere");
            }

            if (!hasPermissionToDeleteAll && (!hasPermissionToDeleteOwn || userId != rabbitToDelete.OwnerId))
            {
                throw new InvalidOperationException("Du har ikke rettigheder til at slette kaniner, du ikke ejer");
            }

            // Fjern referencer til faren
            var childrenWithThisFatherList = await _dbRepository.GetDbSet()
                .Where(r => r.Father_EarCombId == earCombId)
                .ToListAsync();
            foreach (var child in childrenWithThisFatherList)
            {
                child.Father_EarCombId = null;
            }

            // Fjern referencer til moren
            var childrenWithThisMother = await _dbRepository.GetDbSet()
                .Where(r => r.Mother_EarCombId == earCombId)
                .ToListAsync();
            foreach (var child in childrenWithThisMother)
            {
                child.Mother_EarCombId = null;
            }

            await _dbRepository.UpdateObjectsListAsync(childrenWithThisFatherList);
            await _dbRepository.UpdateObjectsListAsync(childrenWithThisMother);

            // Slet kaninen
            await _dbRepository.DeleteObjectAsync(rabbitToDelete);

            return new Rabbit_OwnedPreviewDTO()
            {
                EarCombId = rabbitToDelete.EarCombId,
                NickName = rabbitToDelete.NickName,
                Race = rabbitToDelete.Race,
                Color = rabbitToDelete.Color,
                Gender = rabbitToDelete.Gender,
                ProfilePicture = rabbitToDelete.ProfilePicture,
                //UserOwner = rabbitToDelete.UserOwner != null ? $"{rabbitToDelete.UserOwner.FirstName} {rabbitToDelete.UserOwner.LastName}" : null,
                //UserOrigin = rabbitToDelete.UserOrigin != null ? $"{rabbitToDelete.UserOrigin.FirstName} {rabbitToDelete.UserOrigin.LastName}" : null,
            };
        }




        //---------------------: TRANSFER
        public async Task UpdateRabbitOwnershipAsync(Rabbit rabbit)
        {
            await _dbRepository.UpdateObjectAsync(rabbit);
        }


        //---------------------: HELPER METHODs

        /// <summary>
        /// Kigger efter om der findes eksisterende Rabbit-parents i systemet,
        /// hvor den angivne Rabbit's <Parent>_EarCombIds, mathcher andre Rabbits. 
        /// Hvis ja, UPDATEs rabbit, med en etableret forbindelse efter forbehold
        /// </summary>
        /// <param name="rabbit">Rabbit object hvis ParentId_Placeholders skal undersøges</param>
        /// <returns></returns>
        private async Task ParentsFK_SetupAsync(Rabbit rabbit)  
        {
            // Antag at Rabbit modellen har properties for FatherId_Placeholder og MotherId_Placeholder
            string? fatherIdPlaceholder = rabbit.FatherId_Placeholder;
            string? motherIdPlaceholder = rabbit.MotherId_Placeholder;

            // Forsøg at finde en eksisterende far kanin, hvis placeholder ikke er null
            Rabbit? existingFather = null;
            if (fatherIdPlaceholder != null)
            {
                existingFather = await Get_Rabbit_ByEarCombId(fatherIdPlaceholder);
            }

            // Opdater Father_EarCombId kun hvis en gyldig far kanin findes
            if (existingFather != null && existingFather.Gender == Gender.Buck)
            {
                rabbit.Father_EarCombId = fatherIdPlaceholder;
            }
            else
            {
                rabbit.Father_EarCombId = null; // Sæt til null hvis ingen gyldig far kanin findes eller placeholder er null
            }

            // Samme som oven, men for Mother_EarCombId
            Rabbit? existingMother = null;
            if (motherIdPlaceholder != null)
            {
                existingMother = await Get_Rabbit_ByEarCombId(motherIdPlaceholder);
            }

            if (existingMother != null && existingMother.Gender == Gender.Doe)
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
        /// <param name="earCombId"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        private async Task ChildFK_SetupAsync(string earCombId, Gender gender)
        {
            List<Rabbit> rabbitsToUpdateList = new List<Rabbit>();

            if (gender == Gender.Buck)
            {
                var childrenWithFatherList = await _dbRepository.GetDbSet()
                    .Where(r => r.FatherId_Placeholder == earCombId)
                    .ToListAsync();

                foreach (var child in childrenWithFatherList)
                {
                    child.Father_EarCombId = earCombId;
                    rabbitsToUpdateList.Add(child);
                    await _dbRepository.UpdateObjectAsync(child);
                }
            }
            else if (gender == Gender.Doe)
            {
                var childrenWithMotherList = await _dbRepository.GetDbSet()
                    .Where(r => r.MotherId_Placeholder == earCombId)
                    .ToListAsync();

                foreach (var child in childrenWithMotherList)
                {
                    child.Mother_EarCombId = earCombId;
                    rabbitsToUpdateList.Add(child);
                    await _dbRepository.UpdateObjectAsync(child);
                }
            }

            // Gem kun ændringerne, hvis der er nogen rabbits at opdatere
            //if (rabbitsToUpdateList.Any())
            //{
            //    await _dbRepository.SaveObjectsList(rabbitsToUpdateList);
            //}
        }


        /// <summary>
        /// Opdaterer Rabbit.OriginId baseret på Rabbit.RightEarId og User.BreederRegNo
        /// </summary>
        /// <param name="rabbit"></param>
        /// <returns></returns>
        private async Task UserOriginFK_SetupAsync(Rabbit rabbit)
        {
            //string? rightEarId = rabbit.RightEarId;
            // Find the user with the BreederRegNo that matches the rabbit's RightEarId
            var user = await _breederService.Get_BreederByBreederRegNo(rabbit.RightEarId);

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
            await _dbRepository.UpdateObjectAsync(rabbit);
        }

        /// <summary>
        /// Finder alle kaniner vis Rabbit.RightEarId matcher avlerens nye User.BreederRegNo
        /// og opdaterer alle disse Rabbit.OriginId til at pege på brugeren.
        /// </summary>
        /// <param name="breederRegNo">Brugerens avlernummer</param>
        /// <param name="userId">Brugerens ID</param>
        /// <returns></returns>
        public async Task LinkRabbits_ToNewBreederAsync(string userId, string breederRegNo)
        {
            // Filtrer kaniner baseret på RightEarId, der matcher breederRegNo, direkte i databasen
            var rabbitsToUpdateList = await _dbRepository.GetDbSet()
                .Where(rabbit => rabbit.RightEarId == breederRegNo)
                .ToListAsync();

            // Opdater OriginId for hver relevant kanin til at pege på brugeren
            foreach (var rabbit in rabbitsToUpdateList)
            {
                rabbit.OriginId = userId; // Brug userId direkte
                //await _dbRepository.UpdateObjectAsync(rabbit);
            }
            await _dbRepository.UpdateObjectsListAsync(rabbitsToUpdateList);
        }
    }
}
