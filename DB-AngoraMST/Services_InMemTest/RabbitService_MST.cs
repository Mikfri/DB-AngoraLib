using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.HelperService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
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
    public class RabbitService_MST
    {
        private IRabbitService _rabbitService;
        private IAccountService _accountService; // Changed from IUserService
        private DB_AngoraContext _context;
        private Mock<UserManager<User>> _userManagerMock;

        public RabbitService_MST()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();

            // Create UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var userRepository = new GRepository<User>(_context);
            _accountService = new AccountServices(userRepository, _userManagerMock.Object); // Changed from UserService

            var rabbitRepository = new GRepository<Rabbit>(_context);
            var validatorService = new RabbitValidator();
            _rabbitService = new RabbitServices(rabbitRepository, _accountService, validatorService); // Changed from _userService
        }

        [TestInitialize]
        public void Setup()
        {
            // Add mock data to in-memory database
            var mockDataInitializer = new MockDataInitializer(_context, _userManagerMock.Object);
            mockDataInitializer.Initialize();
        }


        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

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
                Gender = Gender.Hun,
                OpenProfile = OpenProfile.Nej
            };

            // Set the current user for the test
            var currentUser = await _context.Users.FirstAsync();
            Assert.IsNotNull(currentUser);


            // Act
            await _rabbitService.AddRabbit_ToMyCollectionAsync(currentUser.Id, newUniqRabbit);

            // Assert
            var addedRabbit = await _context.Rabbits.FindAsync(newUniqRabbit.RightEarId, newUniqRabbit.LeftEarId);
            Assert.IsNotNull(addedRabbit);

            // Get a rabbit from the mock data
            var existingRabbit = await _context.Rabbits.FirstAsync();
            Assert.IsNotNull(existingRabbit);

            // Act & Assert
            var existingRabbitDto = new Rabbit_CreateDTO { RightEarId = existingRabbit.RightEarId, LeftEarId = existingRabbit.LeftEarId };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.AddRabbit_ToMyCollectionAsync(currentUser.Id, existingRabbitDto));
        }


        //-------------------------: GET TESTS
        /// <summary>
        /// Påvirkes af RabbitService_MST.AddRabbit_ToMyCollectionAsync_TEST
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAllRabbits_ByBreederRegAsync_TEST()
        {
            // Arrange
            var breederRegNo = "5053";
            var expectedRabbitsCount = 17;

            // Act
            var rabbits = await _rabbitService.GetAllRabbits_ByBreederRegAsync(breederRegNo);

            // Debug: Print the names of the returned rabbits
            foreach (var rabbit in rabbits)
            {
                Console.WriteLine(rabbit.NickName);
            }

            // Assert
            Assert.AreEqual(expectedRabbitsCount, rabbits.Count);
        }

        [TestMethod]
        public async Task GetRabbit_ProfileAsync_MODERATOR_Async_TEST()
        {
            // Arrange
            var mockUser = _context.Users.First();
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
            var resultOwned = await _rabbitService.GetRabbit_ProfileAsync(mockUser.Id, mockRabbitOwned.RightEarId, mockRabbitOwned.LeftEarId, mockUserClaims);
            var resultNotOwned = await _rabbitService.GetRabbit_ProfileAsync(mockUser.Id, mockRabbitNotOwned.RightEarId, mockRabbitNotOwned.LeftEarId, mockUserClaims);

            // Assert
            Assert.IsNotNull(resultOwned, "Expected to retrieve profile of owned rabbit");
            Assert.IsNotNull(resultNotOwned, "Expected to retrieve profile of not owned rabbit due to admin role");
        }

        [TestMethod]
        public async Task GetRabbit_ProfileAsync_BREEDER_Async_TEST()
        {
            // Arrange
            var mockUser = _context.Users.Skip(1).First();
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
            var resultOwned = await _rabbitService.GetRabbit_ProfileAsync(mockUser.Id, mockRabbitOwned.RightEarId, mockRabbitOwned.LeftEarId, mockUserClaims);
            var resultNotOwned = await _rabbitService.GetRabbit_ProfileAsync(mockUser.Id, mockRabbitNotOwned.RightEarId, mockRabbitNotOwned.LeftEarId, mockUserClaims);

            // Assert
            Assert.IsNotNull(resultOwned, "Expected to retrieve profile of owned rabbit");
            Assert.IsNull(resultNotOwned, "Expected not to retrieve profile of not owned rabbit due to breeder role");
        }


        //-------------------------: UPDATE TESTS
        [TestMethod]
        public async Task UpdateRabbit_MODERATOR_Async_TEST()
        {
            // Arrange
            var mockUser = _context.Users.First();
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            var updatedOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedOwnedName" };
            var updatedNotOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedNotOwnedName" };
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
            var updatedOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC_Async(mockUser, mockRabbitOwned.RightEarId, mockRabbitOwned.LeftEarId, updatedOwnedRabbitDTO, mockUserClaims);
            var updatedNotOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC_Async(mockUser, mockRabbitNotOwned.RightEarId, mockRabbitNotOwned.LeftEarId, updatedNotOwnedRabbitDTO, mockUserClaims);

            // Assert
            Assert.AreEqual("UpdatedOwnedName", updatedOwnedRabbit.NickName);
            Assert.AreEqual("UpdatedNotOwnedName", updatedNotOwnedRabbit.NickName);
        }


        [TestMethod]
        public async Task UpdateRabbit_BREEDER_Async_TEST()
        {
            // Arrange
            var mockUser = _context.Users.Skip(1).First();
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            var updatedOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedOwnedName" };
            var updatedNotOwnedRabbitDTO = new Rabbit_UpdateDTO { NickName = "UpdatedNotOwnedName" };
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
            var updatedOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC_Async(mockUser, mockRabbitOwned.RightEarId, mockRabbitOwned.LeftEarId, updatedOwnedRabbitDTO, mockUserClaims);
            var updatedNotOwnedRabbit = await _rabbitService.UpdateRabbit_RBAC_Async(mockUser, mockRabbitNotOwned.RightEarId, mockRabbitNotOwned.LeftEarId, updatedNotOwnedRabbitDTO, mockUserClaims);

            // Assert
            Assert.AreEqual("UpdatedOwnedName", updatedOwnedRabbit.NickName);
            Assert.AreNotEqual("UpdatedNotOwnedName", updatedNotOwnedRabbit.NickName);     
        }


        //-------------------------: DELETE TESTS
        [TestMethod]
        public async Task DeleteRabbit_MODERATOR_Async_TEST()
        {
            // Arrange
            var mockUser = _context.Users.First(); // Get the first user from the database

            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            // Get an owned and a not owned rabbit from the database
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            Console.WriteLine($"User: {mockUser.UserName}\nMY-Rabbit: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act
            await _rabbitService.DeleteRabbit_RBAC_Async(mockUser, mockRabbitOwned.RightEarId, mockRabbitOwned.LeftEarId, mockUserClaims);
            await _rabbitService.DeleteRabbit_RBAC_Async(mockUser, mockRabbitNotOwned.RightEarId, mockRabbitNotOwned.LeftEarId, mockUserClaims);

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
        public async Task DeleteRabbit_BREEDER_Async_TEST()
        {
            // Arrange
            var mockUser = _context.Users.Skip(1).First(); // Get the second user from the database
            var mockRabbitOwned = _context.Rabbits.First(r => r.OwnerId == mockUser.Id);

            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            // Get a rabbit not owned by the user
            var mockRabbitNotOwned = _context.Rabbits.First(r => r.OwnerId != mockUser.Id);
            Console.WriteLine($"User: {mockUser.UserName}\nMY-Rabbit: {mockRabbitOwned.NickName}\nOTHER-Rabbit: {mockRabbitNotOwned.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act & Assert
            // Expect an exception to be thrown because the user does not own the rabbit
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.DeleteRabbit_RBAC_Async(mockUser, mockRabbitNotOwned.RightEarId, mockRabbitNotOwned.LeftEarId, mockUserClaims));
            
            // Act
            await _rabbitService.DeleteRabbit_RBAC_Async(mockUser, mockRabbitOwned.RightEarId, mockRabbitOwned.LeftEarId, mockUserClaims);

            // Assert
            // Verify that the rabbit was deleted from the database
            var deletedRabbitOwned = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbitOwned.RightEarId && r.LeftEarId == mockRabbitOwned.LeftEarId);
            Assert.IsNull(deletedRabbitOwned);
        }



    }
}
