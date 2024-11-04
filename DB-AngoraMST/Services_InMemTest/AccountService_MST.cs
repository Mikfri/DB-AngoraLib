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
using System.Collections.Generic;
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

            // Create UserRepository and BreederRepository
            var userRepository = new GRepository<User>(_context);
            var breederRepository = new GRepository<Breeder>(_context);

            // Initialize AccountService with all required parameters
            _accountService = new AccountServices(userRepository, breederRepository, _emailServiceMock.Object, _userManagerMock.Object);
        }

        [TestInitialize]
        public void Setup()
        {
            // Add mock data to in-memory database
            var mockUsersWithRoles = MockUsers.GetMockUsersWithRoles();
            foreach (var mockUserWithRole in mockUsersWithRoles)
            {
                _context.Users.Add(mockUserWithRole.User);
                _context.SaveChanges();

                // Setup UserManager mock to return the user
                _userManagerMock.Setup(um => um.FindByEmailAsync(mockUserWithRole.User.Email))
                    .ReturnsAsync(mockUserWithRole.User);
                _userManagerMock.Setup(um => um.FindByNameAsync(mockUserWithRole.User.UserName))
                    .ReturnsAsync(mockUserWithRole.User);
            }
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
            var expectedUsersCount = MockUsers.GetMockUsersWithRoles().Count;

            // Act
            var users = await _accountService.GetAll_Users();

            // Assert
            Assert.AreEqual(expectedUsersCount, users.Count);
        }        

        [TestMethod]
        public async Task GetUserByUserNameOrEmailAsync_TEST()
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
    }
}
