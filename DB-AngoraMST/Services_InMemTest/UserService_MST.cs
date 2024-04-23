using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
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
        //private RabbitService _rabbitService;
        private UserService _userService;
        private DB_AngoraContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            #region ------------- MED SECRET SETUP -------------
            //// Setup mock IConfiguration
            //var mockConfigurationSection = new Mock<IConfigurationSection>();
            //mockConfigurationSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("YourConnectionString");

            //var mockConfiguration = new Mock<IConfiguration>();
            //mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfigurationSection.Object);

            //_context = new DB_AngoraContext(options, mockConfiguration.Object);
            //_context.Database.EnsureCreated();
            #endregion

            #region ------------- UDEN SECRET SETUP -------------
            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();
            #endregion

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
            //_rabbitService = new RabbitService(rabbitRepository, _userService, validatorService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public void GetCurrentUsersRabbitCollection_TEST()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var race = Race.Angora;
            var color = Color.Blå;
            var gender = Gender.Hun;
            var isPublic = IsPublic.No;
            var rightEarId = "5095";
            var leftEarId = "002";
            var nickName = "Sov";
            var isJuvenile = (bool?)null;
            var dateOfBirth = (DateOnly?)null;
            var dateOfDeath = (DateOnly?)null;

            // Act
            var result = _userService.GetCurrentUsersRabbitCollection_ByProperties(currentUser, rightEarId, leftEarId, nickName, race, color, gender, isPublic, isJuvenile, dateOfBirth, dateOfDeath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var rabbit = result.First();
            Assert.AreEqual(rightEarId, rabbit.RightEarId);
            Assert.AreEqual(leftEarId, rabbit.LeftEarId);
            Assert.AreEqual(nickName, rabbit.NickName);
            Assert.AreEqual(race, rabbit.Race);
            Assert.AreEqual(color, rabbit.Color);
            Assert.AreEqual(gender, rabbit.Gender);
            Assert.AreEqual(isPublic, rabbit.IsPublic);
        }
    }
}
