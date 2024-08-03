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
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class AccountServices_MST
    {
        private IAccountService _accountService;
        private DB_AngoraContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IEmailService> _emailServiceMock;


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

            // Create EmailService mock
            _emailServiceMock = new Mock<IEmailService>();

            var userRepository = new GRepository<User>(_context);
            _accountService = new AccountServices(userRepository, _emailServiceMock.Object, _userManagerMock.Object);
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

        //---------------------------------: GET TESTS :---------------------------------
        [TestMethod]
        public async Task GetAllUsersAsync_TEST()
        {
            // Arrange
            var expectedUsersCount = 3; // Replace with the actual number of mock users

            // Act
            var users = await _accountService.GetAll_Users();

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
            var actualUser = await _accountService.Get_UserByBreederRegNo(breederRegNo);

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
            var actualUserByUsername = await _accountService.Get_UserByUserNameOrEmail(expectedUser.UserName);
            var actualUserByEmail = await _accountService.Get_UserByUserNameOrEmail(expectedUser.Email);

            // Assert
            Assert.IsNotNull(actualUserByUsername);
            Assert.AreEqual(expectedUser.UserName, actualUserByUsername.UserName);
            Assert.IsNotNull(actualUserByEmail);
            Assert.AreEqual(expectedUser.Email, actualUserByEmail.Email);
        }


        [TestMethod]
        public async Task Get_Rabbits_OwnedAlive_Filtered_TEST()
        {
            // Arrange
            var userId = "MajasId";
            var race = Race.Satin_Angora;
            var dateOfBirth = new DateOnly(2024, 3, 1);

            var filter = new Rabbit_FilteredRequestDTO
            {
                Race = race,
                FromDateOfBirth = dateOfBirth,
                OnlyDeceased = false
            };

            // Get the user's rabbit collection for comparison
            var mockUserRabbitCollection = _context.Users
                .Include(u => u.RabbitsOwned) // Load the related rabbits
                .Single(u => u.Id == userId) // Get the user
                .RabbitsOwned; // Get the user's rabbit collection

            // Filtrer mockUserRabbitCollection baseret på DateOfBirth
            var filteredMockUserRabbitCollection = mockUserRabbitCollection
                .Where(rabbit => rabbit.DateOfBirth >= filter.FromDateOfBirth)
                .ToList();

            // Print hver filtreret kanins kaldenavn og fødselsdato for debugging formål
            int i = 0;
            foreach (var rabbit in filteredMockUserRabbitCollection)
            {
                Console.WriteLine($"{i++}:{rabbit.EarCombId}: {rabbit.NickName}, DateOfBirth: {rabbit.DateOfBirth}");
            }


            // Act
            var filteredRabbitCollection = await _accountService.GetAll_RabbitsOwned_Filtered(userId, filter);

            // Assert
            Assert.IsNotNull(filteredRabbitCollection); // Check that the result is not null
            Assert.IsTrue(filteredRabbitCollection.All(rabbit => rabbit.Race == race)); // Check that all rabbits in the result have the expected race

            // Check that the number of rabbits in the result matches the expected number
            var allRabbits = await _context.Rabbits.ToListAsync();
            var expectedRabbitCount = allRabbits.Count(rabbit => rabbit.OwnerId == userId && rabbit.Race == race && rabbit.DateOfBirth >= filter.FromDateOfBirth);

            Assert.AreEqual(expectedRabbitCount, filteredRabbitCollection.Count);
            Debug.WriteLine($"Expected: {expectedRabbitCount}, Actual: {filteredRabbitCollection.Count}");
        }






    }
}
