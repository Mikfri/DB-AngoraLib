using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class UserService_MST
    {
        private IUserService _userService;
        private DB_AngoraContext _context;
        private Mock<UserManager<User>> _userManagerMock;

        public UserService_MST()
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
        }

        [TestInitialize]
        public void Setup()
        {
            // Add mock data to in-memory database
            var mockUsers = MockUsers.GetMockUsers();
            _context.Users.AddRange(mockUsers);
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

        //[TestMethod]
        //public async Task Login_Test()
        //{
        //    // Arrange
        //    var userLoginDto = new UserLoginDTO
        //    {
        //        Email = "IdaFriborg87@gmail.com", // Replace with the actual email of a mock user
        //        Password = "Ida123!" // Replace with the actual password of a mock user
        //    };

        //    // Act
        //    var user = await _userService.Login(userLoginDto);

        //    // Assert
        //    Assert.IsNotNull(user);
        //    Assert.AreEqual(userLoginDto.Email, user.Email);
        //}


        [TestMethod]
        public async Task GetAllUsersAsync_TEST()
        {
            // Arrange
            var expectedUsersCount = 3; // Replace with the actual number of mock users

            // Act
            var users = await _userService.GetAllUsersAsync();

            // Assert
            Assert.AreEqual(expectedUsersCount, users.Count);
        }

        [TestMethod]
        public async Task GetUserByBreederRegNoAsync_TEST()
        {
            // Arrange
            var expectedUser = _context.Users.First();
            var userKeyDto = new User_BreederKeyDTO { BreederRegNo = expectedUser.BreederRegNo };

            // Act
            var actualUser = await _userService.GetUserByBreederRegNoAsync(userKeyDto);

            // Assert
            Assert.IsNotNull(actualUser);
            Assert.AreEqual("IdasId", actualUser.Id);
            Assert.AreEqual("Ida", actualUser.FirstName);
            Assert.AreEqual(expectedUser.LastName, actualUser.LastName);
        }

        [TestMethod]
        public async Task GetCurrentUsersRabbitCollection_TEST()
        {
            // Arrange
            var userId = "5053";

            // Act
            var rabbitCollection = await _userService.GetCurrentUsersRabbitCollection(userId);

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
            var userId = "5053"; // Replace with the actual user id
            var race = Race.Satinangora; // Replace with the actual right ear id

            // Act
            var filteredRabbitCollection = await _userService.GetFilteredRabbitCollection(userId, race: race);

            foreach (var rabbit in filteredRabbitCollection)
            {
                Console.WriteLine(rabbit.NickName);
            }

            // Assert
            Assert.IsNotNull(filteredRabbitCollection);
            // Add more assertions based on your test expectations
        }


    }
}
