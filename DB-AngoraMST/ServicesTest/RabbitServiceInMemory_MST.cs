using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DB_AngoraMST.ServicesTest
{
    [TestClass]
    public class RabbitServiceInMemory_MST
    {
        private RabbitService _rabbitService;
        private UserService _userService;
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

            // Add mock data to in-memory database
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
        public async Task AddRabbitAsync_TEST()
        {
            // Arrange

            var newUniqRabbit = new Rabbit(
            "5095",                 // RightEarId
            "004",                  // LeftEarId
            "5095",                 // Owner            
            "Yvonne",               // Nickname
            Race.Angora,
            Color.Jerngrå,
            new DateOnly(2020, 06, 12), // Format: yyyy-MM-dd
            new DateOnly(2022, 07, 22), // Format: yyyy-MM-dd
            Gender.Hun,
            null,                   // MotherRightEarId
            null,                   // MotherLeftEarId
            null,                   // FatherRightEarId
            null,                   // FatherLeftEarId
            IsPublic.No);

            // Get a rabbit from the mock data
            var existingRabbit = await _context.Rabbits.FirstAsync();
            Assert.IsNotNull(existingRabbit);

            // Set the current user for the test
            var currentUser = await _context.Users.FirstAsync();
            Assert.IsNotNull(currentUser);

            // Act
            await _rabbitService.AddRabbitAsync(newUniqRabbit, currentUser);

            // Assert
            var addedRabbit = await _context.Rabbits.FindAsync(newUniqRabbit.RightEarId, newUniqRabbit.LeftEarId);
            Assert.IsNotNull(addedRabbit);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _rabbitService.AddRabbitAsync(existingRabbit, currentUser));
        }

    }
}
