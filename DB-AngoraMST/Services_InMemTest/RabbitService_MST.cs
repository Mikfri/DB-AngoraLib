using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
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
        private IUserService _userService;
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
            _userService = new UserService(userRepository, _userManagerMock.Object);

            var rabbitRepository = new GRepository<Rabbit>(_context);
            var validatorService = new RabbitValidator();
            _rabbitService = new RabbitServices(rabbitRepository, _userService, validatorService);
        }

        //[TestInitialize]
        //public void Setup()
        //{
        //    // Add mock data to in-memory database
        //    var mockUsers = MockUsers.GetMockUsers();
        //    _context.Users.AddRange(mockUsers);
        //    var mockRabbits = MockRabbits.GetMockRabbits();
        //    _context.Rabbits.AddRange(mockRabbits);
        //    _context.SaveChanges();
        //}
        [TestInitialize]
        public void Setup()
        {
            // Add mock data to in-memory database
            var mockUsersWithRoles = MockUsers.GetMockUsersWithRoles();
            foreach (var mockUserWithRole in mockUsersWithRoles)
            {
                _context.Users.Add(mockUserWithRole.User);
                var userClaims = MockUserClaims.GetMockUserClaimsForUser(mockUserWithRole);
                _context.UserClaims.AddRange(userClaims.Select(uc => new IdentityUserClaim<string>
                {
                    UserId = mockUserWithRole.User.Id,
                    ClaimType = uc.Type,
                    ClaimValue = uc.Value
                }));
            }
            var mockRabbits = MockRabbits.GetMockRabbits();
            _context.Rabbits.AddRange(mockRabbits);
            _context.SaveChanges();
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
            var newUniqRabbit = new RabbitDTO
            {
                RightEarId = "5095",
                LeftEarId = "004",
                NickName = "Yvonne",
                Race = Race.Angora,
                Color = Color.Jerngrå,
                DateOfBirth = new DateOnly(2020, 06, 12),
                DateOfDeath = null,
                Gender = Gender.Hun,
                IsPublic = IsPublic.No
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
            var existingRabbitDto = new RabbitDTO { RightEarId = existingRabbit.RightEarId, LeftEarId = existingRabbit.LeftEarId };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.AddRabbit_ToMyCollectionAsync(currentUser.Id, existingRabbitDto));
        }

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
        public async Task UpdateMyRabbitAsync_TEST()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var existingRabbit = await _context.Rabbits.FirstAsync();
            existingRabbit.NickName = "New Nickname";
            existingRabbit.Color = Color.Hvid;

            // Act
            await _rabbitService.UpdateMyRabbitAsync(currentUser, existingRabbit);

            // Assert
            var updatedRabbit = await _context.Rabbits.FindAsync(existingRabbit.RightEarId, existingRabbit.LeftEarId);
            Assert.IsNotNull(updatedRabbit);
            Assert.AreEqual("New Nickname", updatedRabbit.NickName);
            Assert.AreEqual(Color.Hvid, updatedRabbit.Color);
        }

        [TestMethod]
        public async Task DeleteMyRabbitAsync_TEST()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var existingRabbit = await _context.Rabbits.FirstAsync();
            var initialCount = _context.Rabbits.Count();    // Første optælling

            // Act
            await _rabbitService.DeleteMyRabbitAsync(currentUser, existingRabbit);

            // Assert
            var finalCount = _context.Rabbits.Count();      // Anden optælling
            Assert.AreEqual(initialCount - 1, finalCount);
        }

        [TestMethod]
        public async Task DeleteRabbit_RBAC_Async_TEST()
        {
            // Arrange
            var mockUser = _context.Users.First(); // Get the first user from the database
            var mockRabbit = _context.Rabbits.First(); // Get the first rabbit from the database

            // Get the user's claims from the database
            var mockUserClaims = _context.UserClaims
                .Where(uc => uc.UserId == mockUser.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToList();

            Console.WriteLine($"User: {mockUser.UserName}\nRabbit: {mockRabbit.NickName}");
            foreach (var claim in mockUserClaims)
            {
                Console.WriteLine($"ClaimType: '{claim.Type}' | ClaimValue: '{claim.Value}'");
            }

            // Act
            await _rabbitService.DeleteRabbit_RBAC_Async(mockUser, mockRabbit, mockUserClaims);

            // Assert
            // Verify that the rabbit was deleted from the database
            var deletedRabbit = await _context.Rabbits
                .FirstOrDefaultAsync(r => r.RightEarId == mockRabbit.RightEarId && r.LeftEarId == mockRabbit.LeftEarId);
            Assert.IsNull(deletedRabbit);
        }

    }
}
