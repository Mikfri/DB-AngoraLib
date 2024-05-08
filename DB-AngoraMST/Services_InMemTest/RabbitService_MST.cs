using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class RabbitService_MST
    {
        private IRabbitService _rabbitService;
        private IUserService _userService;
        private DB_AngoraContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();

            //-----: Add mock data to in-memory database
            var mockUsers = MockUsers.GetMockUsers();
            _context.Users.AddRange(mockUsers);
            var mockRabbits = MockRabbits.GetMockRabbits();
            _context.Rabbits.AddRange(mockRabbits);
            _context.SaveChanges();
            var userRepository = new GRepository<User>(_context);
            _userService = new UserService(userRepository);

            var rabbitRepository = new GRepository<Rabbit>(_context);
            var validatorService = new RabbitValidator();
            _rabbitService = new RabbitService(rabbitRepository, _userService, validatorService);
        }


        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddRabbit_ToCurrentUserAsync_TEST()
        {
            // Arrange
            var newUniqRabbit = new Rabbit_BasicsDTO
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

            // Create a UserKeyDTO from the User
            var currentUserKeyDto = new User_KeyDTO { BreederRegNo = currentUser.Id };

            // Act
            await _rabbitService.AddRabbit_ToCurrentUserAsync(currentUserKeyDto, newUniqRabbit);

            // Assert
            var addedRabbit = await _context.Rabbits.FindAsync(newUniqRabbit.RightEarId, newUniqRabbit.LeftEarId);
            Assert.IsNotNull(addedRabbit);

            // Get a rabbit from the mock data
            var existingRabbit = await _context.Rabbits.FirstAsync();
            Assert.IsNotNull(existingRabbit);

            // Act & Assert
            var existingRabbitDto = new Rabbit_BasicsDTO { RightEarId = existingRabbit.RightEarId, LeftEarId = existingRabbit.LeftEarId };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.AddRabbit_ToCurrentUserAsync(currentUserKeyDto, existingRabbitDto));
        }

        /// <summary>
        /// Påvirkes af RabbitService_MST.AddRabbit_ToCurrentUserAsync_TEST
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAllRabbits_ByBreederRegAsync_Test()
        {
            // Arrange
            var breederRegNo = "5053";      // Replace with the actual breeder registration number
            var expectedRabbitsCount = 17;  // Replace with the actual number of rabbits for the breeder

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
        public async Task UpdateRabbitAsync_TEST()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var existingRabbit = await _context.Rabbits.FirstAsync();
            existingRabbit.NickName = "New Nickname";
            existingRabbit.Color = Color.Hvid;

            // Act
            await _rabbitService.UpdateRabbitAsync(currentUser, existingRabbit);

            // Assert
            var updatedRabbit = await _context.Rabbits.FindAsync(existingRabbit.RightEarId, existingRabbit.LeftEarId);
            Assert.IsNotNull(updatedRabbit);
            Assert.AreEqual("New Nickname", updatedRabbit.NickName);
            Assert.AreEqual(Color.Hvid, updatedRabbit.Color);
        }

        [TestMethod]
        public async Task DeleteRabbitAsync_TEST()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var existingRabbit = await _context.Rabbits.FirstAsync();
            var initialCount = _context.Rabbits.Count();    // Første optælling

            // Act
            await _rabbitService.DeleteRabbitAsync(currentUser, existingRabbit);

            // Assert
            var finalCount = _context.Rabbits.Count();      // Anden optælling
            Assert.AreEqual(initialCount - 1, finalCount);
        }
    }
}
