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
        private Mock<IBreederService> _breederServiceMock;
        private Mock<Rabbit_Validator> _validatorServiceMock;
        private DB_AngoraContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DB_AngoraContext(options);

            // Seed the database with initial data
            SeedDatabase(_context);

            // Mock dependencies
            _breederServiceMock = new Mock<IBreederService>();
            _validatorServiceMock = new Mock<Rabbit_Validator>();

            // Initialize RabbitServices with mocked dependencies
            var repository = new GRepository<Rabbit>(_context);
            _rabbitService = new RabbitServices(repository, _breederServiceMock.Object, _validatorServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedDatabase(DB_AngoraContext context)
        {
            var modelBuilder = new ModelBuilder(new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet());
            SeedData.Seed(modelBuilder);
            context.Database.EnsureCreated();
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
                Father_EarCombId = "5050-188",
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
            Assert.AreEqual("5050-188", newUniqRabbit.Father_EarCombId);

            // Act & Assert
            var existingRabbitDto = new Rabbit_CreateDTO { RightEarId = existingRabbit.RightEarId, LeftEarId = existingRabbit.LeftEarId };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.AddRabbit_ToMyCollection(currentUser.Id, existingRabbitDto));

        }


        //-------------------------: GET TESTS
        /// <summary>
        /// Påvirkes af RabbitService_MST.AddRabbit_ToMyCollectionAsync_TEST
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Get_AllRabbits_ByBreederReg_TEST()
        {
            // Arrange
            var breederRegNo = "5053";
            var expectedRabbitsCount = 19;

            // Act
            var rabbits = await _rabbitService.Get_AllRabbits_ByBreederRegNo(breederRegNo);

            Console.WriteLine($"User.BreederRegNo: {breederRegNo}");
            int i = 0;
            foreach (var rabbit in rabbits)
            {
                Console.WriteLine($"{++i}: {rabbit.NickName}");
                //Console.WriteLine(rabbit.NickName);
            }

            // Assert
            Assert.AreEqual(expectedRabbitsCount, rabbits.Count);
        }

        [TestMethod]
        public async Task Get_Rabbit_Profile_MODERATOR_TEST()
        {
            // Arrange
            //var mockUser = _context.Users.First();
            var mockUser = _context.Users.OfType<Breeder>().First();


            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            Console.WriteLine($"User: {mockUser.FirstName}\nMY-Rabbit: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act
            var resultOwned = await _rabbitService.Get_Rabbit_Profile(mockUser.Id, mockRabbitOwned.EarCombId, mockUserClaims);
            var resultNotOwned = await _rabbitService.Get_Rabbit_Profile(mockUser.Id, mockRabbitNotOwned.EarCombId, mockUserClaims);

            // Assert
            Assert.IsNotNull(resultOwned, "Expected to retrieve profile of owned rabbit");
            Assert.IsNotNull(resultNotOwned, "Expected to retrieve profile of not owned rabbit due to admin role");
        }

        [TestMethod]
        public async Task Get_Rabbit_Profile_BREEDER_TEST()
        {
            // Arrange
            //var mockUser = _context.Users.Skip(1).First();
            var mockUser = _context.Users.OfType<Breeder>().Skip(1).First();

            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            Console.WriteLine($"User: {mockUser.UserName}\nMY-Rabbit: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act
            var resultOwned = await _rabbitService.Get_Rabbit_Profile(mockUser.Id, mockRabbitOwned.EarCombId, mockUserClaims);
            var resultNotOwned = await _rabbitService.Get_Rabbit_Profile(mockUser.Id, mockRabbitNotOwned.EarCombId, mockUserClaims);

            // Assert
            Assert.IsNotNull(resultOwned, "Expected to retrieve profile of owned rabbit");
            Assert.IsNull(resultNotOwned, "Expected not to retrieve profile of not owned rabbit due to breeder role");
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
            var mockUser = _context.Users.OfType<Breeder>().First();
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            var updatedOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedOwnedName" };
            var updatedNotOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedNotOwnedName" };
            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            Console.WriteLine($"User: {mockUser.UserName}\nMY-Rabbit: {mockRabbitOwned.EarCombId}: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.EarCombId}: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act
            var updatedOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(mockUser.Id, mockRabbitOwned.EarCombId, updatedOwnedRabbitDTO, mockUserClaims);
            var updatedNotOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(mockUser.Id, mockRabbitNotOwned.EarCombId, updatedNotOwnedRabbitDTO, mockUserClaims);

            // Assert
            Assert.AreEqual("UpdatedOwnedName", updatedOwnedRabbit.NickName);
            Assert.AreEqual("UpdatedNotOwnedName", updatedNotOwnedRabbit.NickName);
        }


        [TestMethod]
        public async Task UpdateRabbit_BREEDER_TEST()
        {
            // Arrange
            var mockUser = _context.Users.OfType<Breeder>().Skip(1).First();
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            var updatedOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedOwnedName" };
            var updatedNotOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedNotOwnedName" };
            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            Console.WriteLine($"User: {mockUser.UserName}\nMY-Rabbit: {mockRabbitOwned.EarCombId}: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.EarCombId}: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act
            var updatedOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(mockUser.Id, mockRabbitOwned.EarCombId, updatedOwnedRabbitDTO, mockUserClaims);
            var updatedNotOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC(mockUser.Id, mockRabbitNotOwned.EarCombId, updatedNotOwnedRabbitDTO, mockUserClaims);

            // Assert
            Assert.AreEqual("UpdatedOwnedName", updatedOwnedRabbit.NickName);
            Assert.AreNotEqual("UpdatedNotOwnedName", updatedNotOwnedRabbit.NickName);
        }


        //-------------------------: DELETE TESTS
        [TestMethod]
        public async Task DeleteRabbit_MODERATOR_TEST()
        {
            // Arrange
            var mockUser = _context.Users.OfType<Breeder>().First(); // Get the first user from the database

            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            // Get an owned and a not owned rabbit from the database
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            Console.WriteLine($"User: {mockUser.UserName} {mockUser.FirstName}\nMY-Rabbit: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act
            await _rabbitService.DeleteRabbit_RBAC(mockUser.Id, mockRabbitOwned.EarCombId, mockUserClaims);
            await _rabbitService.DeleteRabbit_RBAC(mockUser.Id, mockRabbitNotOwned.EarCombId, mockUserClaims);

            // Assert
            // Verify that both rabbits was deleted from the database
            var deletedRabbitOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitOwned.RightEarId && r.LeftEarId == mockRabbitOwned.LeftEarId);
            Assert.IsNull(deletedRabbitOwned);

            var deletedRabbitNotOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitNotOwned.RightEarId && r.LeftEarId == mockRabbitNotOwned.LeftEarId);
            Assert.IsNull(deletedRabbitNotOwned);
        }


        [TestMethod]
        public async Task DeleteRabbit_BREEDER_TEST()
        {
            // Arrange
            var mockUser = _context.Users.OfType<Breeder>().Skip(1).First(); // Get the second user from the database
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);

            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            // Get a rabbit not owned by the user
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            Console.WriteLine($"User: {mockUser.UserName} {mockUser.FirstName} {mockUser.Email} {mockUser.NormalizedEmail}\nMY-Rabbit: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act & Assert
            // Expect an exception to be thrown because the user does not own the rabbit
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.DeleteRabbit_RBAC(mockUser.Id, mockRabbitNotOwned.EarCombId, mockUserClaims));

            // Act
            await _rabbitService.DeleteRabbit_RBAC(mockUser.Id, mockRabbitOwned.EarCombId, mockUserClaims);

            // Assert
            // Verify that the rabbit was deleted from the database
            var deletedRabbitOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitOwned.RightEarId && r.LeftEarId == mockRabbitOwned.LeftEarId);
            Assert.IsNull(deletedRabbitOwned);
        }        
    }
}
