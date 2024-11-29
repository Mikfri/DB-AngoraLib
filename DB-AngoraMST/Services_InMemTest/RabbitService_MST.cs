using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.SeededData;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.BreederService;
using DB_AngoraLib.Services.EmailService;
using DB_AngoraLib.Services.HelperService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class RabbitServices_MST
    {
        private IRabbitService _rabbitService;
        private IBreederService _breederService;
        private DB_AngoraContext _context;

        public RabbitServices_MST()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();

            // Initialize BreederService with in-memory context
            var breederRepository = new GRepository<Breeder>(_context);
            _breederService = new BreederServices(breederRepository);

            // Initialize RabbitServices with actual BreederService
            var rabbitRepository = new GRepository<Rabbit>(_context);
            _rabbitService = new RabbitServices(rabbitRepository, _breederService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        //-------------------------: ADD TESTS
        [TestMethod]
        public async Task AddRabbit_ToMyCollectionAsync_TEST()
        {
            // Arrange
            var newUniqRabbit = new Rabbit_CreateDTO
            {
                RightEarId = "5095",
                LeftEarId = "004",
                NickName = "Yvonne",
                Race = Race.Angora,
                Color = Color.Jerngrå,
                DateOfBirth = new DateOnly(2020, 06, 12),
                DateOfDeath = null,
                Gender = Gender.Doe,
                ForSale = IsPublic.Nej,
                FatherId_Placeholder = "5050-188",
            };
            var existingRabbit = await _context.Rabbits.FirstAsync();
            Assert.IsNotNull(existingRabbit);

            var currentUser = await _context.Users.FirstAsync();
            Assert.IsNotNull(currentUser);

            // Act
            await _rabbitService.AddRabbit_ToMyCollection(currentUser.Id, newUniqRabbit);

            // Assert
            var addedRabbit = await _context.Rabbits.FirstOrDefaultAsync(r => r.RightEarId == newUniqRabbit.RightEarId && r.LeftEarId == newUniqRabbit.LeftEarId);
            Assert.IsNotNull(addedRabbit);
            Assert.AreEqual("5050-188", newUniqRabbit.FatherId_Placeholder);

            // Act & Assert
            var existingRabbitDto = new Rabbit_CreateDTO { RightEarId = existingRabbit.RightEarId, LeftEarId = existingRabbit.LeftEarId };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.AddRabbit_ToMyCollection(currentUser.Id, existingRabbitDto));

        }


        //-------------------------: GET TESTS
        [TestMethod]
        public async Task Get_AllRabbits_ByBreederReg_TEST()
        {
            // Arrange
            var breederRegNo = "5053";
            var expectedRabbitsCount = 25;

            // Act
            var rabbits = await _rabbitService.Get_AllRabbits_ByBreederRegNo(breederRegNo);

            Console.WriteLine($"User.BreederRegNo: {breederRegNo}");
            int i = 0;
            foreach (var rabbit in rabbits)
            {
                Console.WriteLine($"{++i}: {rabbit.NickName}");
            }

            // Assert
            Assert.IsNotNull(rabbits);
            Assert.AreEqual(expectedRabbitsCount, rabbits.Count);
        }

        [TestMethod]
        public async Task Get_Rabbit_Profile_MODERATOR_TEST()
        {
            // Arrange
            // Get the role ID for the "Moderator" role
            var moderatorRoleId = _context.Roles.First(r => r.Name == "Moderator").Id;

            // Get a user with the "Moderator" role
            var userId = _context.UserRoles
                .Where(ur => ur.RoleId == moderatorRoleId)
                .Select(ur => ur.UserId)
                .First();

            var moderator = _context.Users.OfType<Breeder>().First(u => u.Id == userId);

            // Get the user's claims from the database
            var userClaims = GetUserAndRoleClaims(moderator.Id);

            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == moderator.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != moderator.Id);

            // Print user details and claims for debugging
            PrintUserDetailsAndClaims(moderator, userClaims, mockRabbitOwned, mockRabbitNotOwned);

            // Act
            var resultOwned = await _rabbitService.Get_Rabbit_Profile(moderator.Id, mockRabbitOwned.EarCombId, userClaims);
            var resultNotOwned = await _rabbitService.Get_Rabbit_Profile(moderator.Id, mockRabbitNotOwned.EarCombId, userClaims);

            // Assert
            Assert.IsNotNull(resultOwned, "Expected to retrieve profile of owned rabbit");
            Assert.IsNotNull(resultNotOwned, "Expected to retrieve profile of not owned rabbit due to admin role");
        }

        [TestMethod]
        public async Task Get_Rabbit_Profile_BREEDERPREMIUM_TEST()
        {
            // Arrange
            // Get the role ID for the "BreederBasic" role
            var breederRoleId = _context.Roles.First(r => r.Name == "BreederPremium").Id;

            // Get a user with the "BreederBasic" role
            var userId = _context.UserRoles
                .Where(ur => ur.RoleId == breederRoleId)
                .Select(ur => ur.UserId)
                .First();

            var breeder = _context.Users.OfType<Breeder>().First(u => u.Id == userId);

            // Get the user's claims from the database
            var userClaims = GetUserAndRoleClaims(breeder.Id);

            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == breeder.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != breeder.Id);

            // Print user details and claims for debugging
            PrintUserDetailsAndClaims(breeder, userClaims, mockRabbitOwned, mockRabbitNotOwned);

            // Act
            var resultOwned = await _rabbitService.Get_Rabbit_Profile(breeder.Id, mockRabbitOwned.EarCombId, userClaims);
            var resultNotOwned = await _rabbitService.Get_Rabbit_Profile(breeder.Id, mockRabbitNotOwned.EarCombId, userClaims);

            // Assert
            Assert.IsNotNull(resultOwned, "Expected to retrieve profile of owned rabbit");
            Assert.IsNull(resultNotOwned, "Expected not to retrieve profile of not owned rabbit due to role: BreederPremium");
        }


        [TestMethod]
        public async Task Get_AllRabbits_ForSale_Filtered_TEST()
        {
            // Arrange
            var expectedRace = Race.Angora;
            //var expectedColor = Color.Jerngrå;
            //var expectedGender = Gender.Hun;

            // Create a list of expected rabbits
            var expectedRabbits = _context.Rabbits
                .Where(r =>
                r.Race == expectedRace &&
                // r.Color == expectedColor &&
                //r.Gender == expectedGender &&
                r.ForSale == IsPublic.Ja);

            // Print each rabbit's nickname
            foreach (var rabbit in expectedRabbits)
            {
                Console.WriteLine($"EXP-Rabbit: {rabbit.NickName}, EXP-AppovedColComb: {rabbit.ApprovedRaceColorCombination}");
            }
            Console.WriteLine($"EXP-Rabbit.Count: {expectedRabbits.Count()}\n");


            // Act
            var filter = new Rabbit_ForsaleFilterDTO { Race = expectedRace };
            var rabbits = await _rabbitService.Get_AllRabbits_Forsale_Filtered(filter);


            foreach (var rabbit in rabbits)
            {
                Console.WriteLine($"FOUND-Rabbit: {rabbit.NickName}");
            }
            Console.WriteLine($"FOUND-Rabbit.Count: {rabbits.Count}");


            // Assert
            Assert.IsNotNull(rabbits);

        }

        [TestMethod]
        public async Task Get_Rabbit_Pedigree_TEST()
        {
            // Arrange
            var earCombId = "5095-0124"; // Aron
                                        // Antag at _rabbitService allerede er initialiseret og klar til brug.

            // Act
            var pedigree = await _rabbitService.Get_RabbitPedigree(earCombId);

            // Assert
            Assert.IsNotNull(pedigree);
            PrintPedigree(pedigree);
        }

        private void PrintPedigree(Rabbit_PedigreeDTO rabbit, int generation = 0)
        {
            if (rabbit == null) return;

            var indent = new string(' ', generation * 2);
            Console.WriteLine($"{indent}Rabbit: {rabbit.NickName} (EarCombId: {rabbit.EarCombId})");

            if (rabbit.Father != null)
            {
                Console.WriteLine($"{indent}Father:");
                PrintPedigree(rabbit.Father, generation + 1);
            }

            if (rabbit.Mother != null)
            {
                Console.WriteLine($"{indent}Mother:");
                PrintPedigree(rabbit.Mother, generation + 1);
            }
        }

        [TestMethod]
        public async Task Get_Rabbit_ChildCollection_TEST()
        {
            // Arrange
            var earCombId = "4977-206";
            var parentRabbit = _context.Rabbits.First(r => r.EarCombId == earCombId);

            // Create a list of expected rabbits
            var expectedChildren = _context.Rabbits
                .Where(r =>
                r.Father_EarCombId == earCombId ||
                r.Mother_EarCombId == earCombId);


            Console.WriteLine($"ID: {parentRabbit.EarCombId}, NickName: {parentRabbit.NickName}\n");
            foreach (var rabbit in expectedChildren)
            {
                Console.WriteLine(rabbit.NickName);
            }

            // Act
            var rabbitCollection = await _rabbitService.Get_Rabbit_ChildCollection(earCombId);

            // Assert
            Assert.IsNotNull(rabbitCollection);
            // Add more assertions based on your test expectations
        }

        //-------------------------: UPDATE TESTS
        [TestMethod]
        public async Task UpdateRabbit_MODERATOR_TEST()
        {
            // Arrange
            // Get the role ID for the "Moderator" role
            var moderatorRoleId = _context.Roles.First(r => r.Name == "Moderator").Id;

            // Get a user with the "Moderator" role
            var userId = _context.UserRoles
                .Where(ur => ur.RoleId == moderatorRoleId)
                .Select(ur => ur.UserId)
                .First();

            var moderator = _context.Users.OfType<Breeder>().First(u => u.Id == userId);

            // Get the user's claims from the database
            var userClaims = GetUserAndRoleClaims(moderator.Id);

            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == moderator.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != moderator.Id);
            var updatedOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedOwnedName" };
            var updatedNotOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedNotOwnedName" };

            // Print user details and claims for debugging
            PrintUserDetailsAndClaims(moderator, userClaims, mockRabbitOwned, mockRabbitNotOwned);

            // Act
            var updatedOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(moderator.Id, mockRabbitOwned.EarCombId, updatedOwnedRabbitDTO, userClaims);
            var updatedNotOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(moderator.Id, mockRabbitNotOwned.EarCombId, updatedNotOwnedRabbitDTO, userClaims);

            Console.WriteLine($"\nRESULT\nOWN-Rabbit: {updatedOwnedRabbit.NickName}\nOTHER-Rabbit: {updatedNotOwnedRabbit.NickName}");

            // Assert
            Assert.AreEqual("UpdatedOwnedName", updatedOwnedRabbit.NickName);
            Assert.AreEqual("UpdatedNotOwnedName", updatedNotOwnedRabbit.NickName);
        }

        [TestMethod]
        public async Task UpdateRabbit_BREEDERPREMIUM_TEST()
        {
            // Arrange
            // Get the role ID for the "BreederBasic" role
            var breederRoleId = _context.Roles.First(r => r.Name == "BreederPremium").Id;

            // Get a user with the "BreederBasic" role
            var userId = _context.UserRoles
                .Where(ur => ur.RoleId == breederRoleId)
                .Select(ur => ur.UserId)
                .First();

            var breeder = _context.Users.OfType<Breeder>().First(u => u.Id == userId);

            // Get the user's claims from the database
            var userClaims = GetUserAndRoleClaims(breeder.Id);

            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == breeder.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != breeder.Id);
            var updatedOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedOwnedName" };
            var updatedNotOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedNotOwnedName" };

            // Print user details and claims for debugging
            PrintUserDetailsAndClaims(breeder, userClaims, mockRabbitOwned, mockRabbitNotOwned);

            // Act
            var updatedOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(breeder.Id, mockRabbitOwned.EarCombId, updatedOwnedRabbitDTO, userClaims);
            var updatedNotOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(breeder.Id, mockRabbitNotOwned.EarCombId, updatedNotOwnedRabbitDTO, userClaims);

            Console.WriteLine($"\nRESULT\nOWN-Rabbit: {updatedOwnedRabbit.NickName}\nOTHER-Rabbit: {updatedNotOwnedRabbit.NickName}");

            // Assert
            Assert.AreEqual("UpdatedOwnedName", updatedOwnedRabbit.NickName);
            Assert.AreNotEqual("UpdatedNotOwnedName", updatedNotOwnedRabbit.NickName);
        }

        //-------------------------: DELETE TESTS
        [TestMethod]
        public async Task DeleteRabbit_MODERATOR_TEST()
        {
            // Arrange
            // Get the role ID for the "Moderator" role
            var moderatorRoleId = _context.Roles.First(r => r.Name == "Moderator").Id;

            // Get a user with the "Moderator" role
            var userId = _context.UserRoles
                .Where(ur => ur.RoleId == moderatorRoleId)
                .Select(ur => ur.UserId)
                .First();

            var moderator = _context.Users.OfType<Breeder>().First(u => u.Id == userId);

            // Get the user's claims from the database
            var userClaims = GetUserAndRoleClaims(moderator.Id);

            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == moderator.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != moderator.Id);

            // Print user details and claims for debugging
            PrintUserDetailsAndClaims(moderator, userClaims, mockRabbitOwned, mockRabbitNotOwned);

            // Act
            await _rabbitService.DeleteRabbit_RBAC(moderator.Id, mockRabbitOwned.EarCombId, userClaims);
            await _rabbitService.DeleteRabbit_RBAC(moderator.Id, mockRabbitNotOwned.EarCombId, userClaims);

            // Assert
            // Verify that both rabbits were deleted from the database
            var deletedRabbitOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitOwned.RightEarId && r.LeftEarId == mockRabbitOwned.LeftEarId);
            Assert.IsNull(deletedRabbitOwned);

            var deletedRabbitNotOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitNotOwned.RightEarId && r.LeftEarId == mockRabbitNotOwned.LeftEarId);
            Assert.IsNull(deletedRabbitNotOwned);
        }

        [TestMethod]
        public async Task DeleteRabbit_BREEDERPREMIUM_TEST()
        {
            // Arrange
            // Get the role ID for the "BreederPremium" role
            var premiumBreederRoleId = _context.Roles.First(r => r.Name == "BreederPremium").Id;

            // Get a user with the "BreederPremium" role
            var userId = _context.UserRoles
                .Where(ur => ur.RoleId == premiumBreederRoleId)
                .Select(ur => ur.UserId)
                .First();

            var premiumBreeder = _context.Users.OfType<Breeder>().First(u => u.Id == userId);

            // Get the user's claims from the database
            var userClaims = GetUserAndRoleClaims(premiumBreeder.Id);

            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == premiumBreeder.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != premiumBreeder.Id);

            // Print user details and claims for debugging
            PrintUserDetailsAndClaims(premiumBreeder, userClaims, mockRabbitOwned, mockRabbitNotOwned);

            // Act & Assert
            // Expect an exception to be thrown because the user does not own the rabbit
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.DeleteRabbit_RBAC(premiumBreeder.Id, mockRabbitNotOwned.EarCombId, userClaims));

            // Act
            await _rabbitService.DeleteRabbit_RBAC(premiumBreeder.Id, mockRabbitOwned.EarCombId, userClaims);

            // Assert
            // Verify that the owned rabbit was deleted from the database
            var deletedRabbitOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitOwned.RightEarId && r.LeftEarId == mockRabbitOwned.LeftEarId);
            Assert.IsNull(deletedRabbitOwned);

            // Verify that the not owned rabbit was not deleted from the database
            var deletedRabbitNotOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitNotOwned.RightEarId && r.LeftEarId == mockRabbitNotOwned.LeftEarId);
            Assert.IsNotNull(deletedRabbitNotOwned);
        }

        private IList<Claim> GetUserAndRoleClaims(string userId)
        {
            // Get the user's claims from the database
            var userClaims = _context.UserClaims
                .Where(uc => uc.UserId == userId)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            // Get the user's role Id's
            var userRoles = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            // Get the role claims
            var roleClaims = _context.RoleClaims
                .Where(rc => userRoles.Contains(rc.RoleId))
                .Select(rc => new Claim(rc.ClaimType, rc.ClaimValue))
                .ToList();

            // Combine user claims and role claims
            userClaims.AddRange(roleClaims);

            return userClaims;
        }

        private IList<string> GetUserRoles(string userId)
        {
            // Get the user's role Id's
            var userRoleIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            // Get the role names
            var userRoles = _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToList();

            return userRoles;
        }

        private void PrintUserDetailsAndClaims(Breeder user, IList<Claim> userAndRoleClaims, Rabbit mockRabbitOwned, Rabbit mockRabbitNotOwned)
        {
            // Get the user's roles
            var userRoles = GetUserRoles(user.Id);

            // Print user details and claims for debugging
            Console.WriteLine($"User: {user.UserName}\n MY-Rabbit: {mockRabbitOwned.NickName}\n  Roles: {string.Join(", ", userRoles)}");
            foreach (var claim in userAndRoleClaims)
            {
                Console.WriteLine($"  ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }
            Console.WriteLine($"\nOTHER-Rabbit: {mockRabbitNotOwned.NickName}");
        }
    }
}
