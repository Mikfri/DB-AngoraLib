using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class AccountServices_MST
    {
        public TestContext TestContext { get; set; }

        private IAccountService _accountService;
        private DB_AngoraContext _context;
        private Mock<UserManager<User>> _userManagerMock;

        public AccountServices_MST()
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
            _accountService = new AccountServices(userRepository, _userManagerMock.Object); // Added _userManagerMock.Object
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

        // Add your test methods here
        [TestMethod]
        public async Task GetAllUsersAsync_TEST()
        {
            // Arrange
            var expectedUsersCount = 3; // Replace with the actual number of mock users

            // Act
            var users = await _accountService.GetAllUsersAsync();

            // Assert
            Assert.AreEqual(expectedUsersCount, users.Count);
        }

        [TestMethod]
        public async Task GetUserByBreederRegNoAsync_TEST()
        {
            // Arrange
            var expectedUser = _context.Users.First();
            var breederRegNo = expectedUser.BreederRegNo;

            // Act
            var actualUser = await _accountService.GetUserByBreederRegNoAsync(breederRegNo);

            // Assert
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(expectedUser.BreederRegNo, actualUser.BreederRegNo);
        }

        [TestMethod]
        public async Task GetUserByUserNameOrEmailAsync_TEST()  // TODO: Gør denne test bedre
        {
            // Arrange
            var expectedUser = _context.Users.First();

            // Act
            var actualUserByUsername = await _accountService.GetUserByUserNameOrEmailAsync(expectedUser.UserName);
            var actualUserByEmail = await _accountService.GetUserByUserNameOrEmailAsync(expectedUser.Email);

            // Assert
            Assert.IsNotNull(actualUserByUsername);
            Assert.AreEqual(expectedUser.UserName, actualUserByUsername.UserName);
            Assert.IsNotNull(actualUserByEmail);
            Assert.AreEqual(expectedUser.Email, actualUserByEmail.Email);
        }

        [TestMethod]
        public async Task GetCurrentUsersRabbitCollection_TEST()
        {
            // Arrange
            var userId = "MajasId";

            // Act
            var rabbitCollection = await _accountService.GetMyRabbitCollection(userId);

            foreach (var rabbit in rabbitCollection)
            {
                Console.WriteLine(rabbit.NickName);
            }

            // Assert
            Assert.IsNotNull(rabbitCollection);
            // Add more assertions based on your test expectations
        }

        [TestMethod]
        public async Task GetFilteredRabbitCollection_TEST()
        {
            // Arrange
            var userId = "IdasId"; // Replace with the actual user id
            var race = Race.Satinangora;
            var approvedRaceColorCombination = true;


            // Get the user's rabbit collection
            var mockUserRabbitCollection = _context.Users
                .Include(u => u.Rabbits) // Load the related rabbits
                .Single(u => u.Id == userId) // Get the user
                .Rabbits; // Get the user's rabbit collection

            // Print each rabbit's nickname
            foreach (var rabbit in mockUserRabbitCollection)
            {
                Console.WriteLine($"Rabbit: {rabbit.NickName},Race: {rabbit.Race}, Color: {rabbit.Color} AppovedColComb: {rabbit.ApprovedRaceColorCombination}");
            }

            // Act
            var filteredRabbitCollection = await _accountService.GetMyRabbitCollection_Filtered(userId, race: race, approvedRaceColorCombination: approvedRaceColorCombination);

            // Assert
            Assert.IsNotNull(filteredRabbitCollection); // Check that the result is not null
            Assert.IsTrue(filteredRabbitCollection.All(rabbit => rabbit.Race == race)); // Check that all rabbits in the result have the expected race

            // Check that the number of rabbits in the result matches the expected number
            var allRabbits = await _context.Rabbits.ToListAsync();
            var expectedRabbitCount = allRabbits.Count(rabbit => rabbit.OwnerId == userId && rabbit.Race == race && rabbit.ApprovedRaceColorCombination == approvedRaceColorCombination);
            Assert.AreEqual(expectedRabbitCount, filteredRabbitCollection.Count);
        }







    }
}
